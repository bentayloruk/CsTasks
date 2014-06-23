[<AutoOpen>]
module CsTasks.ExportImportPromotionTasks
#if MS
    open Microsoft.CommerceServer.Marketing
    open Microsoft.CommerceServer
#else
    open CommerceServer.Core.Marketing
    open CommerceServer.Core
#endif
    open System.Data
    open System
    open System.IO
    open System.Diagnostics

#if MS
    let exportImportToolPath = CsTasksToolPathFromFileName "ExportImportPromotion-MS.exe" 
#else
    let exportImportToolPath = CsTasksToolPathFromFileName "ExportImportPromotion.exe" 
#endif
    let CurrentDirectory () = Directory.GetCurrentDirectory()

    type DiscountExportArgs =
        { MarketingWebServiceUrl : string
          ExportDirectoryPath : string
          Timeout : TimeSpan }

    let DiscountExportArgsDefaults() =
        { DiscountExportArgs.MarketingWebServiceUrl = ""
          ExportDirectoryPath = CurrentDirectory() 
          Timeout = MaxTimeSpan() }

    type DiscountImportArgs =
        { MarketingWebServiceUrl : string
          DiscountsPath : string
          GlobalExpressionsPath : string
          ImportDirectoryPath : string
          PromoCodesPath : string
          Timeout : TimeSpan }

    let DiscountImportArgsDefaults() =
        { DiscountImportArgs.MarketingWebServiceUrl = ""
          DiscountsPath = @"Discount_.xml" 
          GlobalExpressionsPath = @"GlobalExpressions_.xml" 
          PromoCodesPath = @"PromoCodes_.xml" 
          ImportDirectoryPath = CurrentDirectory() 
          Timeout = MaxTimeSpan() }

    let ExportDiscounts (argsAction:(DiscountExportArgs->DiscountExportArgs)) =
        let args = DiscountExportArgsDefaults() |> argsAction
        if String.IsNullOrEmpty(args.MarketingWebServiceUrl) then failwith "You must set the MarketingWebServiceUrl."
        let formattedArgs =
            //TODO add this as method on record so users can get command line?
            sprintf "/ex /con %s" 
                args.MarketingWebServiceUrl
        let exitCode = 
            ExecProcess (fun psi ->
                psi.FileName <- exportImportToolPath 
                psi.WorkingDirectory <- args.ExportDirectoryPath
                psi.Arguments <- formattedArgs) args.Timeout
        ()

    //Imports discounts and associated global expressions and promo codes.
    let ImportDiscounts argsAction =
        let args = DiscountImportArgsDefaults() |> argsAction
        if String.IsNullOrEmpty(args.MarketingWebServiceUrl) then failwith "You must set the MarketingWebServiceUrl."
        let formattedArgs =
            //TODO add this as method on record so users can get command line?
            sprintf "/im /con %s /d %s /p %s /ge %s" 
                args.MarketingWebServiceUrl
                args.DiscountsPath
                args.PromoCodesPath
                args.GlobalExpressionsPath
        let exitCode = 
            ExecProcess (fun psi ->
                psi.FileName <- exportImportToolPath 
                psi.Arguments <- formattedArgs) args.Timeout
        ()


