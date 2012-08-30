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
        
    let DeleteExpressions(marketingContextFactory:(unit->MarketingContext)) =
        let mc = marketingContextFactory()
        let clause = 
            let scf = mc.Expressions.GetSearchClauseFactory()
            scf.CreateClause()
        let so = SearchOptions(PropertiesToReturn = "Id", StartRecord = 1, NumberOfRecordsToReturn = 500)
        let expressionIds = 
            let dataSet = mc.Expressions.Search(clause, so)
            let table = dataSet.Tables.["SearchResults"];
            table.AsEnumerable()
            |> Seq.map (fun row -> row.["Id"] :?> int)
        for id in expressionIds do
            trace (sprintf "Deleting expression %s" (id.ToString()))
            mc.Expressions.Delete(id)
        ()

    let DeleteDiscounts(marketingContextFactory:(unit->MarketingContext)) =
        trace "Starting DeleteDiscounts"
        let mc = marketingContextFactory()
        let clause = 
            let scf = mc.CampaignItems.GetSearchClauseFactory(CampaignItemType.Discount);
            scf.CreateClause()
        let so = SearchOptions(PropertiesToReturn = "Id", StartRecord = 1, NumberOfRecordsToReturn = 500)
        let discountIds = 
            let dataSet = mc.CampaignItems.Search(clause, so, false)
            let table = dataSet.Tables.["SearchResults"];
            table.AsEnumerable()
            |> Seq.map (fun row -> row.["Id"] :?> int)
        for id in discountIds do
            trace (sprintf "Deleting %s" (id.ToString()))
            mc.CampaignItems.Delete(id)
        trace "Ending DeleteDiscounts"
        ()
        


