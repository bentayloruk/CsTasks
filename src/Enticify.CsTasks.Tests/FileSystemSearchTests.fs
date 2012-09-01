module Enticify.CsTasks.FileSystemSearchTests

open Xunit
open CsTasks

[<Fact>]
let ``gets x86 folder location`` () =
    //TODO finish Environment.fs code so we can test if we are on x64
    //This test is pointless on x86 machine.
    let path = ProgramFilesx86() 
    Assert.Contains("(x86)", path)
    ()

