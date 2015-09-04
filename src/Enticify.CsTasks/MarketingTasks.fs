[<AutoOpen>]
module CsTasks.Marketing
#if MS
    open Microsoft.CommerceServer.Marketing
    open Microsoft.CommerceServer
    open Microsoft.CommerceServer.Runtime.Configuration
#else
    open CommerceServer.Core.Marketing
    open CommerceServer.Core
    open CommerceServer.Core.Runtime.Configuration
#endif
    open System.Data
    open System.Diagnostics
    open CsTasks.Sql 
          
    let marketingTableSeedSql tableName (seed:int) = 
        sprintf @"set identity_insert mktg_campaign off 
        dbcc checkident('%s', reseed, %i)" tableName seed 

    let MarketingContextFactory siteName =
        let mc = MarketingContext.Create(siteName, null, AuthorizationMode.NoAuthorization)
        fun () -> mc

    type MarketingDataStore (siteName) =
        //Not much point taking the factory if only call it once, but not sure about usage yet.
        let mc = (MarketingContextFactory siteName)()

        let connectionString =
            let siteResCollection = CommerceResourceCollection(siteName)
            let mr = siteResCollection.Item("Marketing")
            StripProvider (mr.["connstr_db_marketing"].ToString())

        let resultPageSize =
            let siteResCollection = CommerceResourceCollection(siteName)
            let mr = siteResCollection.Item("Marketing")
            System.Convert.ToInt32(mr.["i_SearchResultsLimit"])

        let ReSeed tableName seed =
            trace (sprintf "Before seed %s" tableName)
            let sql = new SqlHelper (connectionString)
            let exec = marketingTableSeedSql  tableName seed
            sql.Execute [] exec 
            trace (sprintf "After seed %s" tableName)
            ()

        let rec searchCampaignItemIdsAndModified (searcher : SearchOptions -> DataSet) resultPageSize startIndex = seq {
            let so = SearchOptions(PropertiesToReturn = "LastModifiedDate, Id", StartRecord = startIndex, NumberOfRecordsToReturn = resultPageSize)
            let maybeResults = 
                try
                    let dataSet = searcher so
                    Some dataSet
                with | :? SearchPageNumberException as ex -> None
            match maybeResults with
            | Some dataSet -> let table = dataSet.Tables.["SearchResults"]
//                              for x in table.Columns do
//                                printfn "%s" x.ColumnName
                              let idLastModifiedPairs = table.AsEnumerable() |> Seq.map (fun row -> row.["Id"] :?> int, row.["LastModifiedDate"] :?> System.DateTime)
                              yield! idLastModifiedPairs
                              if table.Rows.Count < resultPageSize then
                                  yield! searchCampaignItemIdsAndModified searcher resultPageSize (startIndex + resultPageSize) 
                              else ()
            | None -> ()
        }

        let searchCampaignItemIds (searcher : SearchOptions -> DataSet) resultPageSize startIndex = 
            searchCampaignItemIdsAndModified searcher resultPageSize startIndex |> Seq.map fst

        let DeleteItems description (searcher:(SearchOptions->DataSet)) deleter = 
            trace (sprintf "Before delete %s." description)
            let ids = searchCampaignItemIds searcher resultPageSize 1
            for id in ids do
                trace (sprintf "Deleting %s %s" description (id.ToString()))
                try deleter(id)
                with | ex -> trace (sprintf "Failed to delete %s with Id %s.  Exception: %s" description (id.ToString()) ex.Message)
            trace (sprintf "After delete %s." description)

        member x.DeleteAllExpressions() =
            let description = "Expression"
            let searcher (so:SearchOptions) = 
                let clause = 
                    let scf = mc.Expressions.GetSearchClauseFactory()
                    scf.CreateClause()
                mc.Expressions.Search(clause, so)
            let deleter id = mc.Expressions.Delete(id)
            DeleteItems description searcher deleter 
            ()
            
        member x.DeleteCampaignItems (ciType:CampaignItemType) = 
            let description = ciType.ToString() 
            let searcher (so:SearchOptions) = 
                let clause = 
                    let scf = mc.CampaignItems.GetSearchClauseFactory(ciType);
                    scf.CreateClause()
                mc.CampaignItems.Search(clause, so)
            let deleter id = mc.CampaignItems.Delete(id)
            DeleteItems description searcher deleter 
            ()

        member x.DeleteAllPromoCodeDefinitions() =
            let searcher (so:SearchOptions) =
                let clause = 
                    let scf = mc.PromoCodeDefinitions.GetSearchClauseFactory();
                    scf.CreateClause()
                mc.PromoCodeDefinitions.Search(clause, so)
            let deleter id = mc.PromoCodeDefinitions.Delete(id)
            DeleteItems "PromoCodeDefinitions" searcher deleter
            ()

        member x.MakeTestDiscounts(campaignId, count) =
            for x = 0 to count do
                let discount = mc.CampaignItems.NewDiscount(campaignId)
                discount.Name <- System.Guid.NewGuid().ToString()
                discount.TemplateName <- "No Display"
                discount.SizeName <- "Full Banner"
                discount.MultilingualBasketDisplay.Add(LanguageString("Hello", "en-US"))
                discount.Save(true)
                mc.CampaignItems.Activate(discount.Id, discount.LastModifiedDate)
                if x % 100 = 0 then printfn "%i" x

        member x.DeleteAllDiscounts() = x.DeleteCampaignItems CampaignItemType.Discount
        member x.DeleteAllAds() = x.DeleteCampaignItems CampaignItemType.Advertisement
        ///Delete direct mail.  Does nothing for CS versions with deprecated direct mail.
        member x.DeleteAllDirectMail() =
            #if MS
            x.DeleteCampaignItems CampaignItemType.DirectMail
            #else
            ()
            #endif
        member x.DeleteAllCampaignItems() =
            x.DeleteAllAds()
            #if MS
            x.DeleteAllDirectMail()
            #endif
            x.DeleteAllDiscounts()
        member x.ReseedCampaignIds seed = ReSeed "mktg_campaign" seed
        member x.ReseedCampaignItemIds seed = ReSeed "mktg_campaign_item" seed
        member x.ReseedExpressionIds seed = ReSeed "mktg_expression" seed

        member x.EnableAll (ciType:CampaignItemType) = 
            let searcher (so:SearchOptions) = 
                let clause = 
                    let scf = mc.CampaignItems.GetSearchClauseFactory(ciType);
                    scf.CreateClause()
                mc.CampaignItems.Search(clause, so)
            for id, lastMod in (searchCampaignItemIdsAndModified searcher resultPageSize 1) do
                try mc.CampaignItems.Activate(id, lastMod)
                with | ex -> printfn "Error activating ID %i: %s" id ex.Message

        member x.EnableAllDiscounts () = 
            x.EnableAll (CampaignItemType.Discount)
            

        //Deletes all campaign items, then the campaigns.
        //REMOVED until we can backup and restore these.
//        member x.DeleteAllCampaigns() = 
//            x.DeleteAllCampaignItems()
//            let description = "Campaigns"
//            let searcher (so:SearchOptions) =
//                let clause =
//                    let scf = mc.Campaigns.GetSearchClauseFactory()
//                    scf.CreateClause()
//                mc.Campaigns.Search(clause, so)
//            let deleter id = mc.Campaigns.Delete(id)
//            DeleteItems description searcher deleter