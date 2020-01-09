open System
open System.Threading
open Strokes.Core.API
open Strokes.Windows.Native.SendInput
open Strokes.Core.EventManager // TODO different namespace for init()
open Strokes.Windows.Native.Hooks

// Implement LowLevelEventEmitter
type WindowsEventEmitter() =
    let mutable handlers: list<LowLevelEventHandler> = []

    member this.WindowsHandler: WindowsEventHandler = fun event -> 
        printfn "Got event %A from windows" event
        false

    interface LowLevelEventEmitter with
        member this.OnNativeEvent(handler) =
            handlers <- [handler]


// Implement NativeKeyEventSender
type WindowsKeyEventSender() = 
    interface NativeKeyEventSender with
        member this.PressKey(code: int) =
            pressKey code
            printfn "sending %i press" code
        member this.ReleaseKey(code: int) =
            releaseKey code
            printfn "sending %i release" code


[<EntryPoint>]
let main argv =
    printfn "Hello World from F#!"
    let emitter = WindowsEventEmitter()
    let sender = WindowsKeyEventSender()

    // setupKeyListener emitter.WindowsHandler
    setupKeyListener (fun event -> 
        printfn "hi"
        false)

    //init emitter sender

    while true do
        (sender :> NativeKeyEventSender).PressKey(0x41)
        Thread.Sleep 1000
        //System.Console.ReadLine() |> ignore
    0 // return an integer exit code
