#I @"src\packages\FAKE.2.18.2\tools"
#r "FakeLib.dll"
open Fake
open System.IO
open System

let version = "0.4.1"
let packageDesc = "Simple and time-saving Commerce Server task automation library (Microsoft & Sitecore)."
let buildTypes = ["Debug";"Release"]
let enticifyDependencies = []
let buildOutputPath = @".\build\output"
let releaseOutputPath = @".\build\dist"
let nugetOutputPath = @".\build\packages"
let nugetToolsOutputPath = @".\build\packages\tools"
let nugetSpecFilePath = @".\nuget\cstasks.nuspec"
let buildDirs = [ releaseOutputPath; buildOutputPath; nugetOutputPath; nugetToolsOutputPath; ]
let projFiles = !! @"src\**\*.fsproj" -- @"src\**\*Tests.fsproj"

//Single run tasks.
Target "Clean" (fun _ -> CleanDirs buildDirs)

Target "BuildMs" (fun _ ->
    MSBuild buildOutputPath "Build" ["Configuration", "Release"]  !! @"src\Enticify.CsTasks\Enticify.CsTasks-MS.fsproj"
    |> Log "BuildOutput:"
)

Target "BuildNonMs" (fun _ ->
    MSBuild buildOutputPath "Build" ["Configuration", "Release"]  !! @"src\Enticify.CsTasks\Enticify.CsTasks.fsproj"
    |> Log "BuildOutput:"
)

Target "AssInfo" (fun _ ->
    AssemblyInfo
        (fun p -> 
        {p with
            CodeLanguage = FSharp;
            AssemblyVersion = version;
            AssemblyTitle = "CsTasks";
            AssemblyCopyright = "Copyright Shape Factory Limited " + DateTime.Now.Year.ToString();
            AssemblyCompany = "Shape Factory Limited";
            AssemblyDescription = "Task automation library for Commerce Server.";
            AssemblyProduct = "CsTasks";
            AssemblyConfiguration = "Release";
            Guid = "D26579B2-8BE0-4BD4-AB87-985531440EE6";
            OutputFileName = @".\src\Enticify.CsTasks\AssemblyInfo.fs"})
)

//Zip
Target "Zip" (fun _ ->
    let zipFileName = Path.Combine(releaseOutputPath, "CsTasks-" + version +  ".zip")
    !! (buildOutputPath + "\*.*") 
        -- "*.zip" //TODO exclude XML docs too?
        |> Zip buildOutputPath zipFileName 
)

//Nuget
Target "Nuget" (fun _ ->
    let nuspecFileName = @".\nuget\cstasks.nuspec"
    XCopy @".\nuget\template\tools" nugetToolsOutputPath 
    Copy (nugetToolsOutputPath) [buildOutputPath @@ "Enticify.CsTasksMS.dll"]
    Copy (nugetToolsOutputPath) [buildOutputPath @@ "Enticify.CsTasksMS.pdb"]
    Copy (nugetToolsOutputPath) [buildOutputPath @@ "Enticify.CsTasks.dll"]
    Copy (nugetToolsOutputPath) [buildOutputPath @@ "Enticify.CsTasks.pdb"]
    NuGet (fun p -> 
        {p with               
            WorkingDir = nugetOutputPath
            ToolPath = @".\src\.nuget\nuget.exe"
            Authors = ["enticify"; "bentayloruk";]
            Project = "CsTasks"
            Version = version
            Dependencies = enticifyDependencies 
            Description = packageDesc 
            OutputPath = nugetOutputPath
            AccessKey = getBuildParamOrDefault "enticifynugetkey" ""
            Publish = false }) nuspecFileName
)

"Clean"
    ==> "AssInfo"
    ==> "BuildMs"
    ==> "BuildNonMs"
    ==> "Zip"
    ==> "Nuget"

Run "Nuget" 


