[<AutoOpen>]
module CsTasks.PurgeCommerceDataTasks
    open Microsoft.CommerceServer.Marketing
    open Microsoft.CommerceServer
    open System.Data
    open System
    open System.Diagnostics
    open Fake

    let purgeCommerceDataExePath = FindInProgramFiles "PurgeCommerceData.exe" 
    //TODO mark those that must be set with an attribute?
    type PurgeCommerceDataArgs =
        { ToolPath : string
          SiteName : string
          Timeout : TimeSpan }

    let DefaultPurgeCommerceDataArgs() =
        { ToolPath = purgeCommerceDataExePath 
          SiteName = "" 
          Timeout = MaxTimeSpan() }

    let PurgeDiscounts argsAction =
        let args = DefaultPurgeCommerceDataArgs() |> argsAction
        let purgeToolArgs = sprintf "%s -m -d 0" args.SiteName
        let exitCode = 
            ExecProcess (fun psi ->
            psi.FileName <- args.ToolPath
            psi.Arguments <- purgeToolArgs) args.Timeout
        ()