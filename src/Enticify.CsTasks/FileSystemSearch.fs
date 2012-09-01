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

///Version of code from http://stackoverflow.com/a/194223/418492
let ProgramFilesx86() =
    if 8 = IntPtr.Size || not (String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))) then
        Environment.GetEnvironmentVariable("ProgramFiles(x86)");
    else
        Environment.GetEnvironmentVariable("ProgramFiles");

let FindInProgramFiles fileName =
    //Search x64 then x86.
    [ Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles); 
      ProgramFilesx86(); ]
    |> Seq.distinct//in case on x86 when both will be the same.
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

