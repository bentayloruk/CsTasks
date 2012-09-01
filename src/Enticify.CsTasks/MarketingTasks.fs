[<AutoOpen>]
module CsTasks.Marketing
    open Microsoft.CommerceServer.Marketing
    open Microsoft.CommerceServer
    open System.Data
    open System.Diagnostics
    open Fake
          
    let MarketingContextSingleton siteName =
        let mc = MarketingContext.Create(siteName, null, AuthorizationMode.NoAuthorization)
        fun () -> mc

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

    let DeleteExpressions(marketingContextFactory:(unit->MarketingContext)) =
        let mc = marketingContextFactory()
        let description = "Expression"
        let searcher (so:SearchOptions) = 
            let clause = 
                let scf = mc.Expressions.GetSearchClauseFactory()
                scf.CreateClause()
            mc.Expressions.Search(clause, so)
        let deleter id = mc.Expressions.Delete(id)
        DeleteItems description searcher deleter 
        ()

    let DeleteCampaignItems (marketingContextFactory:(unit->MarketingContext)) (ciType:CampaignItemType) = 
        let mc = marketingContextFactory()
        let description = ciType.ToString() 
        let searcher (so:SearchOptions) = 
            let clause = 
                let scf = mc.CampaignItems.GetSearchClauseFactory(ciType);
                scf.CreateClause()
            mc.CampaignItems.Search(clause, so)
        let deleter id = mc.CampaignItems.Delete(id)
        DeleteItems description searcher deleter 
        ()

    let DeleteDiscounts(marketingContextFactory:(unit->MarketingContext)) =
        DeleteCampaignItems marketingContextFactory CampaignItemType.Discount
        ()

    let DeleteAdvertisments(marketingContextFactory:(unit->MarketingContext)) =
        DeleteCampaignItems marketingContextFactory CampaignItemType.Advertisement
        ()

    let DeleteDirectMail(marketingContextFactory:(unit->MarketingContext)) =
        DeleteCampaignItems marketingContextFactory CampaignItemType.DirectMail
        ()