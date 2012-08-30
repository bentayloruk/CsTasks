[<AutoOpen>]
module CsTasks.ExportImportPromotionTasks
    open Microsoft.CommerceServer.Marketing
    open Microsoft.CommerceServer
    open System.Data
    open System
    open System.Diagnostics
    open Fake

    type DiscountExportArgs = {
        ToolPath : string
        MarketingWebServiceUrl : string
        Timeout : TimeSpan 
       }

    type DiscountImportArgs = {
        MarketingWebServiceUrl : string
        ToolPath : string
        DiscountsPath : string
        GlobalExpressionsPath : string
        PromoCodesPath : string
        Timeout : TimeSpan 
       }

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
                psi.FileName <- args.ToolPath
                psi.Arguments <- formattedArgs) args.Timeout
        ()

