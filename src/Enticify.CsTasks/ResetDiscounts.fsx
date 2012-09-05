//CHANGE THESE REFERENCE PATHS TO POINT AT THE FAKE AND CSTASKS PACKAGE LOCATIONS ON YOUR MACHINE.
#r @".\src\packages\FAKE.1.64.8\tools\FakeLib.dll"
#r @".\src\packages\CsTasks.0.1.1-ctp\tools\Enticify.CsTasks.dll"

open CsTasks
open System.IO

//Setup the Commerce Server site access details.
let siteName = "StarterSite"
let marketingWebServiceUrl = @"""http://localhost/MarketingWebService/MarketingWebService.asmx""" 
let tempPath = Path.GetTempPath()
let purgeTool = PurgeCommerceDataTool(siteName)
let marketingTool = CampaignItemDestroyer(siteName)

//Export current to Temp, just in case.
ExportDiscounts (fun defaultArgs ->
    { defaultArgs with
        DiscountExportArgs.MarketingWebServiceUrl = marketingWebServiceUrl
        ExportDirectoryPath = tempPath })

//Delete and purge the discounts.
marketingTool.DeleteAllCampaignItems(0)
let retCode = purgeTool.PurgeAllMarketingData()
marketingTool.DeleteAllExpressions(0)

//Do the discount import.
ImportDiscounts (fun defaultArgs -> { defaultArgs with DiscountImportArgs.MarketingWebServiceUrl = marketingWebServiceUrl})