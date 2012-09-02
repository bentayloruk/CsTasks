//CHANGE THESE REFERENCE PATHS TO POINT AT THE FAKE AND CSTASKS PACKAGE LOCATIONS ON YOUR MACHINE.
#r @".\src\packages\FAKE.1.64.8\tools\FakeLib.dll"
#r @".\src\packages\CsTasks.0.1.1-ctp\tools\Enticify.CsTasks.dll"

open CsTasks
open Fake
open System

//Setup the Commerce Server site access details.
let siteName = "StarterSite"
let marketingWebServiceUrl = @"""http://localhost/MarketingWebService/MarketingWebService.asmx""" 
let marketingContext = MarketingContextSingleton siteName
let starterSitePurgeTool = PurgeCommerceDataTool(siteName)

Target "ExportDiscountsToTemp" (fun _ ->
    ExportDiscounts (fun defaultArgs ->
        { defaultArgs with
              DiscountExportArgs.MarketingWebServiceUrl = marketingWebServiceUrl
              ExportDirectoryPath = @"c:\temp" })
)

Target "DelAndPurgeDiscounts" (fun _ ->
    DeleteDiscounts marketingContext 
    let retCode = starterSitePurgeTool.PurgeAllMarketingData()
    DeleteExpressions marketingContext 
)

Target "DelExpressions" (fun _ ->
    DeleteExpressions marketingContext 
)

Target "DelDirectMail" (fun _ ->
    DeleteDirectMail marketingContext 
)

Target "DelAds" (fun _ ->
    DeleteAdvertisments marketingContext
)

Target "ImportTestDiscounts" (fun _ ->
    ImportDiscounts (fun defaultArgs -> { defaultArgs with DiscountImportArgs.MarketingWebServiceUrl = marketingWebServiceUrl})
)

"DelAds" 
    ==> "ExportDiscountsToTemp"
    ==> "DelAndPurgeDiscounts"
    ==> "DelDirectMail"
    ==> "ImportTestDiscounts"

Run "ImportTestDiscounts"