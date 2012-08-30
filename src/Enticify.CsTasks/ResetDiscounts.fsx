#r ".\FakeLib.dll"
#r ".\Enticify.CsTasks.dll"

open CsTasks
open Fake
open System

let siteName = "StarterSite"
let mc = MarketingContextSingleton siteName 
let maxTimeSpan = System.TimeSpan.MaxValue

let purgeToolArgs = {
    ToolPath = @"""C:\Program Files (x86)\Microsoft Commerce Server 9.0\Tools\PurgeCommerceData.exe"""
    SiteName = siteName
    Timeout = maxTimeSpan
    }

let discountImportArgs = 
    {   MarketingWebServiceUrl = @"""http://localhost/MarketingWebService/MarketingWebService.asmx"""
        ToolPath = @"""..\..\tools\ExportImportDiscounts\ExportImportPromotion.exe"""
        DiscountsPath = @""".\Discount_.xml""" 
        GlobalExpressionsPath = @""".\GlobalExpressions_.xml""" 
        PromoCodesPath = @""".\PromoCodes_.xml""" 
        Timeout = maxTimeSpan
    }

Target "ResetDiscounts" (fun _ ->
    DeleteDiscounts mc 
    PurgeDiscounts purgeToolArgs 
    DeleteExpressions mc 
    ImportDiscounts discountImportArgs 
)

Run "ResetDiscounts"