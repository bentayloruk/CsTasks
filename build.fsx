#I @"src\packages\FAKE.1.64.8\tools"
//#I @"..\FAKE\tools\FAKE"
#r "FakeLib.dll"
open Fake
open System.IO
open System

let version = "0.1.5"
let packageDesc = "Simple and time-saving Microsoft/Ascentium Commerce Server task automation library."
let buildTypes = ["Debug";"Release"]
let enticifyDependencies = []
let buildVariations = [""] //No variation for CsTasks.
let buildOutputPath = @".\build\output"
let releaseOutputPath = @".\build\dist"
let nugetOutputPath = @".\build\packages"
let nugetToolsOutputPath = @".\build\packages\tools"
let nugetSpecFilePath = @".\nuget\cstasks.nuspec"
let buildDirs = [ releaseOutputPath; buildOutputPath; nugetOutputPath; nugetToolsOutputPath; ]
let projFiles =
    !+ @"src\**\*.fsproj"
    -- @"src\**\*Tests.fsproj"
    |> Scan

//We assume build configuration name is %buildType%%vsVersion% (e.g. Debug2012)
let buildConfigNames = 
    [
        for buildType in buildTypes do
            for variation in buildVariations do
                yield sprintf "%s%s" buildType variation 
    ]

//Single run tasks.
Target "Clean" (fun _ -> CleanDirs buildDirs)

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


//Nuget
Target "Nuget" (fun _ ->
    let nuspecFileName = @".\nuget\cstasks.nuspec"
    XCopy @".\nuget\template\tools" (nugetOutputPath @@ "tools")
    XCopy (buildOutputPath @@ "Enticify.CsTasks.dll") (nugetOutputPath @@ "tools")
    XCopy (buildOutputPath @@ "Enticify.CsTasks.pdb") (nugetOutputPath @@ "tools")
    //XCopy (buildOutputPath @@ "Enticify.CsTasks.pdb") (nugetOutputPath @@ "tools")
    NuGet (fun p -> 
        //System.Diagnostics.Debugger.Break()
        {p with               
            ToolPath = @".\src\.nuget\nuget.exe"
            Authors = ["enticify"; "bentayloruk";]
            Project = "CsTasks"
            Version = version
            //ProjectFile = @".\src\Enticify.CsTasks\Enticify.CsTasks.fsproj"
            Dependencies = enticifyDependencies 
            Description = packageDesc 
            OutputPath = nugetOutputPath
            AccessKey = getBuildParamOrDefault "enticifynugetkey" ""
            Publish = false }) nuspecFileName
)

Run "Nuget"
