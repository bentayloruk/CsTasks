[<AutoOpen>]
module CsTasks.Marketing
    open Microsoft.CommerceServer.Marketing
    open Microsoft.CommerceServer
    open Microsoft.CommerceServer.Runtime.Configuration
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
        let ReSeed tableName seed =
            trace (sprintf "Before seed %s" tableName)
            let sql = new SqlHelper (connectionString)
            let exec = marketingTableSeedSql  tableName seed
            sql.Execute [] exec 
            trace (sprintf "After seed %s" tableName)
            ()
        let DeleteItems description (searcher:(SearchOptions->DataSet)) deleter = 
            trace (sprintf "Before delete %s." description)
            let itemIds = 
                let so = SearchOptions(PropertiesToReturn = "Id", StartRecord = 1, NumberOfRecordsToReturn = 500)
                let dataSet = searcher so
                let table = dataSet.Tables.["SearchResults"];
                table.AsEnumerable()
                |> Seq.map (fun row -> row.["Id"] :?> int)
            for id in itemIds do
                try
                    trace (sprintf "Deleting %s %s" description (id.ToString()))
                    deleter(id)
                with
                | ex -> trace (sprintf "Failed to delete %s with Id %s.  Exception: %s" description (id.ToString()) ex.Message)
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
        member x.DeleteAllDiscounts() = x.DeleteCampaignItems CampaignItemType.Discount
        member x.DeleteAllAds() = x.DeleteCampaignItems CampaignItemType.Advertisement
        member x.DeleteAllDirectMail() = x.DeleteCampaignItems CampaignItemType.DirectMail
        member x.DeleteAllCampaignItems() = x.DeleteAllAds(); x.DeleteAllDirectMail(); x.DeleteAllDiscounts();
        member x.ReseedCampaignItemIds seed = ReSeed "mktg_campaign_item" seed
        member x.ReseedExpressionIds seed = ReSeed "mktg_expression" seed
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