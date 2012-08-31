//CHANGE THESE REFERENCE PATHS TO POINT AT THE FAKE AND CSTASKS PACKAGE LOCATIONS ON YOUR MACHINE.
#r @".\src\packages\FAKE.1.64.8\tools\FakeLib.dll"
#r @".\src\packages\CsTasks.0.1.1-ctp\tools\Enticify.CsTasks.dll"

//Open the namespaces we need
open CsTasks
open Fake
open System

//Setup the Commerce Server site access details.
let siteName = "StarterSite"
let mc = MarketingContextSingleton siteName 

//Set up the other details we need.
let maxTimeSpan = System.TimeSpan.MaxValue

let purgeToolArgs = {
    ToolPath = @"""C:\Program Files (x86)\Microsoft Commerce Server 9.0\Tools\PurgeCommerceData.exe"""
    SiteName = siteName
    Timeout = maxTimeSpan }

let discountImportArgs = {
        MarketingWebServiceUrl = @"""http://localhost/MarketingWebService/MarketingWebService.asmx"""
        DiscountsPath = @""".\Discount_.xml""" 
        GlobalExpressionsPath = @""".\GlobalExpressions_.xml""" 
        PromoCodesPath = @""".\PromoCodes_.xml""" 
        Timeout = maxTimeSpan }

//Set up our FAKE Targets utilising the CsTasks functions.
Target "DeleteAndPurgeDiscounts" (fun _ ->
    DeleteDiscounts mc 
    PurgeDiscounts purgeToolArgs 
)

Target "DeleteExpressions" (fun _ ->
    DeleteExpressions mc 
)

Target "ImportTestDiscounts" (fun _ ->
    ImportDiscounts discountImportArgs 
)

//Set up the Target dependency change (this is a FAKE thing).
"DeleteAndPurgeDiscounts" ==> "DeleteExpressions" ==> "ImportTestDiscounts"

//Run the Targets.  ImportTestDiscounts depends on the others so they will run too.
Run "ImportTestDiscounts"