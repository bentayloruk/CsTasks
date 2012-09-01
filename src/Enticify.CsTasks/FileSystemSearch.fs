[<AutoOpen>]
module CsTasks.FileSystemSearch
open System
open System.IO
open System.Reflection
   
type FolderSearchOptions =
    | SearchRootOnly
    | SearchAllBelow 

let SearchFolder patterns path option =
    let option = 
        match option with 
        | SearchAllBelow -> SearchOption.AllDirectories 
        | SearchRootOnly -> SearchOption.TopDirectoryOnly
    try
        seq {
            for pattern in patterns do
                let files = Directory.GetFiles(path, pattern, option) 
                for file in files do
                    yield file
            }
    with
    | ex -> Seq.empty

let FindInProgramFiles fileName =
    [ Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles); ]
    |> Seq.map (fun path -> SearchFolder [fileName] path SearchAllBelow) 
    |> Seq.concat 
    |> List.ofSeq 
    |> function 
        | [] -> "" 
        | h::_ -> h//TODO decide what happens if more than one hit.

let FindInExecutingAssemglyLocation fileName =
    let directory =
        Assembly.GetExecutingAssembly().Location
        |> Path.GetDirectoryName
    Path.Combine(directory, fileName)

