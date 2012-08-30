#r ".\FakeLib.dll"
#r ".\Enticify.CsTasks.dll"

open CsTasks
open Fake

let siteName = "StarterSite"
let mc = MarketingContextSingleton siteName 

Target "WipeDiscounts" (fun _ ->
    DeleteDiscounts mc 
    PurgeDiscounts siteName 30l
    DeleteExpressions mc 
)

Run "WipeDiscounts"
