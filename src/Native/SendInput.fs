module Strokes.Windows.Native.SendInput

open System
open System.ComponentModel
open System.Runtime.InteropServices
open System.Text

[<StructLayout(LayoutKind.Sequential)>]
type private MOUSEINPUT = struct
    val dx: int32
    val dy:int32
    val mouseData:uint32
    val dwFlags: uint32
    val time: uint32
    val dwExtraInfo: int
    new(_dx, _dy, _mouseData, _dwFlags, _time, _dwExtraInfo) = {dx=_dx; dy=_dy; mouseData=_mouseData; dwFlags=_dwFlags; time=_time; dwExtraInfo=_dwExtraInfo}
end

[<StructLayout(LayoutKind.Sequential)>]
type private KEYBDINPUT = struct
    val wVk: uint16
    val wScan: uint16
    val dwFlags: uint32
    val time: uint32
    val dwExtraInfo:int
    new(_wVk, _wScan, _dwFlags, _time, _dwExtraInfo) = {wVk =_wVk; wScan = _wScan; dwFlags = _dwFlags; time = _time; dwExtraInfo = _dwExtraInfo}
end

[<StructLayout(LayoutKind.Sequential)>]
type private HARDWAREINPUT = struct
    val uMsg: uint32
    val wParamL: uint16
    val wParamH: uint16
    new(_uMsg, _wParamL, _wParamH) = {uMsg = _uMsg; wParamL = _wParamL; wParamH = _wParamH}
end

[<StructLayout(LayoutKind.Explicit)>]
type private LPINPUT  = struct
    [<FieldOffset(0)>]
    val mutable ``type``:int // 1 is keyboard

    [<FieldOffset(4)>]
    val mutable mi : MOUSEINPUT

    [<FieldOffset(4)>]
    val mutable ki : KEYBDINPUT

    [<FieldOffset(4)>]
    val mutable hi : HARDWAREINPUT 
end

module private InputModes =
    let INPUT_MOUSE = 0;
    let INPUT_KEYBOARD = 1;
    let INPUT_HARDWARE = 2;

module private Dwords = 
    let KEYEVENTF_KEYDOWN = 0x0000u
    let KEYEVENTF_EXTENDEDKEY = 0x0001u
    let KEYEVENTF_KEYUP = 0x0002u
    let KEYEVENTF_UNICODE = 0x0004u
    let KEYEVENTF_SCANCODE = 0x0008u


module private NativeMethods =
    [<DllImport("user32.dll", SetLastError=true, CharSet=CharSet.Unicode)>]
    extern uint32 SendInput(uint32 nInputs, LPINPUT* pInputs, int cbSize)

let appSignature = 0xA8969


let private createPressInput (code: int) =
    let mutable input = LPINPUT()
    input.``type`` <- InputModes.INPUT_KEYBOARD
    input.ki <- KEYBDINPUT(uint16  code, uint16 0, Dwords.KEYEVENTF_KEYDOWN, uint32 0, appSignature)
    input

let private createReleaseInput (code: int) =
    let mutable input = LPINPUT()
    input.ki <- KEYBDINPUT(uint16  code, uint16  0, Dwords.KEYEVENTF_KEYUP, uint32 0, appSignature)
    input


let pressKey (code: int) =
    let input = createPressInput code
    let ptr = &&input
    let result = NativeMethods.SendInput(uint32 1, &&input, Marshal.SizeOf(input)) 
    let err = Win32Exception(Marshal.GetLastWin32Error()).Message
    printfn "handled %i, sizeof %i (%i) last error code is %s" result ( Marshal.SizeOf(input)) (sizeof<LPINPUT>)  (err)

let releaseKey (code: int) =
    let input = createReleaseInput code
    NativeMethods.SendInput(uint32 1, &&input, Marshal.SizeOf(input)) |> ignore

