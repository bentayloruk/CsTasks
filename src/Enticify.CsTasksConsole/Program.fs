open CsTasks.Marketing

[<EntryPoint>]
let main argv = 
    
    let mds = MarketingDataStore("StarterSite")
    mds.DeleteAllDiscounts()
    mds.DeleteAllExpressions()
    mds.DeleteAllPromoCodeDefinitions()
    //mds.MakeTestDiscounts(1, 6000)
    0
