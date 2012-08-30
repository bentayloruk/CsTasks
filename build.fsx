#I @"src\packages\FAKE.1.64.8\tools"
#r "FakeLib.dll"
open Fake
open System.IO
open System

let version = "0.1"
let buildTypes = ["Debug";"Release"]
let buildVsVersions = [""]
let buildOutputPath = @".\build\output"
let releaseOutputPath = @".\build\dist"
let projFiles =
    !+ @"src\**\*.fsproj"
    -- @"src\**\*Tests.fsproj"
    |> Scan

//We assume build configuration name is %buildType%%vsVersion% (e.g. Debug2012)
let buildConfigNames = 
    [
        for buildType in buildTypes do
            for buildVsVersion in buildVsVersions do
                yield sprintf "%s%s" buildType buildVsVersion
    ]

//Single run tasks.
Target "Clean" (fun _ ->
    CleanDir buildOutputPath 
    CleanDir releaseOutputPath 
)

Run "Clean"

//Once per build configuration tasks.
for buildConfigName in buildConfigNames do
    let name taskName = taskName + buildConfigName
    let assConfiguration = if buildConfigName.Contains("Debug") then "Debug" else "Release"
    //Build
    let buildTaskName = name "Build"
    Target buildTaskName (fun _ ->
        trace buildConfigName
        MSBuild buildOutputPath "Build" ["Configuration", buildConfigName]  projFiles
        |> Log "BuildOutput:"
    )
    let versionTaskName = name "SetVersion"
    Target versionTaskName (fun _ ->
        AssemblyInfo 
            (fun p -> 
            {p with
                CodeLanguage = FSharp;
                AssemblyVersion = version;
                AssemblyTitle = "CsTasks";
                AssemblyCopyright = "Copyright Shape Factory Limited " + DateTime.Now.Year.ToString();
                AssemblyCompany = "Shape Factory Limited";
                AssemblyDescription = "FAKE build tasks for Commerce Server" + buildConfigName + ").";
                AssemblyProduct = "CsTasks";
                AssemblyConfiguration = assConfiguration 
                Guid = "D26579B2-8BE0-4BD4-AB87-985531440EE6";
                OutputFileName = @".\src\Enticify.CsTasks\AssemblyInfo.fs"})
    )
    //Zip
    let zipTaskName = name "Zip"
    Target zipTaskName (fun _ ->
        let zipFileName = Path.Combine(releaseOutputPath, "CsTasks-" + buildConfigName + "-" + version +  ".zip")
        !+ (buildOutputPath + "\*.*") 
            -- "*.zip" //TODO exclude XML docs too?
            |> Scan
            |> Zip buildOutputPath zipFileName 
    )

    //Do it!
    versionTaskName 
        ==> buildTaskName
        ==> zipTaskName
        |> ignore
    Run zipTaskName 
