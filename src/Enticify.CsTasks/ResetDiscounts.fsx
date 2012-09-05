//CHANGE THESE REFERENCE PATHS TO POINT AT THE FAKE AND CSTASKS PACKAGE LOCATIONS ON YOUR MACHINE.

open CsTasks
open System.IO

//Setup our site environment.
let siteName = "StarterSite"
let marketingWebServiceUrl = @"""http://localhost/MarketingWebService/MarketingWebService.asmx""" 
let tempPath = Path.GetTempPath()
let purgeTool = PurgeCommerceDataTool(siteName)
let marketingDataStore = MarketingDataStore(siteName)

ExportDiscounts (fun defaultArgs ->
    { defaultArgs with
        DiscountExportArgs.MarketingWebServiceUrl = marketingWebServiceUrl
        ExportDirectoryPath = tempPath })

//Delete all campaign items.
marketingDataStore.DeleteAllCampaignItems()

//Purge marketing data (so we can delete expressions)
let retCode = purgeTool.PurgeAllMarketingData()

//Delete expressions.
marketingDataStore.DeleteAllExpressions()

//Reseed all Ids so our imported discounts get the same IDs each time.
marketingDataStore.ReseedCampaignItemIds(0)
marketingDataStore.ReseedExpressionIds(0)

//Do the discount import.
ImportDiscounts (fun defaultArgs -> { defaultArgs with DiscountImportArgs.MarketingWebServiceUrl = marketingWebServiceUrl})