[<AutoOpen>]
module CsTasks.ExportImportPromotionTasks
    open Microsoft.CommerceServer.Marketing
    open Microsoft.CommerceServer
    open System.Data
    open System
    open System.IO
    open System.Reflection
    open System.Diagnostics
    open Fake

    type DiscountExportArgs = {
        MarketingWebServiceUrl : string
        Timeout : TimeSpan 
       }

    type DiscountImportArgs = {
        MarketingWebServiceUrl : string
        DiscountsPath : string
        GlobalExpressionsPath : string
        PromoCodesPath : string
        Timeout : TimeSpan 
       }

    let locateImportExportExe() =
        let directory =
            Assembly.GetExecutingAssembly().Location
            |> Path.GetDirectoryName
        Path.Combine(directory, "ExportImportPromotion.exe")

    //Imports discounts and associated global expressions and promo codes.
    let ImportDiscounts args =
        let formattedArgs =
            sprintf "/im /con %s /d %s /p %s /ge %s" 
                args.MarketingWebServiceUrl
                args.DiscountsPath
                args.PromoCodesPath
                args.GlobalExpressionsPath
        let exitCode = 
            ExecProcess (fun psi ->
                psi.FileName <- locateImportExportExe()
                psi.Arguments <- formattedArgs) args.Timeout
        ()

