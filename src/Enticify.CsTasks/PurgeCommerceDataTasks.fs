[<AutoOpen>]
module CsTasks.PurgeCommerceDataTasks
    open Microsoft.CommerceServer.Marketing
    open Microsoft.CommerceServer
    open System.Data
    open System
    open System.Diagnostics
    open Fake
    //TODO search for tool (grab expandoc code).
    let toolPath = @"""C:\Program Files (x86)\Microsoft Commerce Server 9.0\Tools\PurgeCommerceData.exe"""
    let PurgeDiscounts siteName secsTimeout =
        let psiSetter (psi:ProcessStartInfo) = 
            psi.FileName <- toolPath
            psi.Arguments <- (sprintf "%s -m -d 0" siteName)
        let timeout = TimeSpan(0l,0l,10l)
        let exitCode = 
            ExecProcess (fun psi ->
            psi.FileName <- toolPath
            psi.Arguments <- (sprintf "%s -m -d 0" siteName)) timeout
        ()
    



