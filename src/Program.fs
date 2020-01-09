open System.Threading
open Strokes.Windows.Native.SendInput

[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    while true do
        pressKey 0x41
        Thread.Sleep 1000
    0 // return an integer exit code
