[<AutoOpen>]
module CsTasks.Globals

open System 


///
let MaxTimeSpan() = TimeSpan.MaxValue
let CsTasksToolPathFromFileName fileName = FindInExecutingAssemglyLocation "ExportImportPromotion.exe"
