[<AutoOpen>]
module CsTasks.PurgeCommerceDataTasks
    open Microsoft.CommerceServer.Marketing
    open Microsoft.CommerceServer
    open System.Data
    open System
    open System.Diagnostics

    type LastModified = 
        | MinDaysAgo of uint32 
        | Anytime

    //TODO this could be an empty string.
    let purgeToolFileName = "PurgeCommerceData.exe" 
    let useOrFindPathOrFail maybePath = 
        match maybePath with 
        | Some(path) -> path
        | None -> 
            match FindInProgramFiles purgeToolFileName with
            | Some(path) -> path
            | None -> failwith "No toolPath provided and unable to locate %s in program files directories." purgeToolFileName

    type PurgeCommerceDataTool(siteName, ?toolPath, ?toolTimeout) = 
        let execPurge cmdLine = 
            let fullCmdLine = siteName + " " + cmdLine
            trace fullCmdLine
            ExecProcess (fun psi ->
            psi.FileName <- useOrFindPathOrFail toolPath 
            psi.Arguments <- fullCmdLine) (defaultArg toolTimeout (MaxTimeSpan()))
        let execPurgeLastModified cmdLine period =
            let cmdLineWithPeriod = 
                cmdLine +
                    match period with 
                    | MinDaysAgo(d) -> " -d " + d.ToString() 
                    | Anytime -> " -d 0"
            execPurge cmdLineWithPeriod 
        member x.PurgeAllMarketingData() = x.PurgeOldMarketingData(Anytime)
        member x.PurgeOldMarketingData(lastModified) = execPurgeLastModified "-m" lastModified
        member x.PurgeAllBaskets() = x.PurgeOldBaskets(Anytime)
        member x.PurgeOldBaskets(lastModified) = execPurgeLastModified  "-b" lastModified
        member x.PurgeNamedBaskets(basketName, lastModified) = execPurgeLastModified  ("-b -n " + basketName) lastModified
        member x.PurgeAllCatalogData() = execPurge "-c"
        member x.PurgeAllPurchaseOrders() = x.PurgeOldPurchaseOrders(Anytime)
        member x.PurgeAllPurchaseOrdersOfStatus(status) = x.PurgeOldPurchaseOrdersOfStatus(Anytime, status)
        member x.PurgeOldPurchaseOrdersOfStatus(lastModified, status) = execPurgeLastModified  ("-p -s " + status) lastModified
        member x.PurgeOldPurchaseOrders(lastModified) = execPurgeLastModified  "-p" lastModified


