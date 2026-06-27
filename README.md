# Custom Console Library

C# library designed to bypass standard terminal bottlenecks and unlock graphics capability within the terminal. 
Supports 24-bit TrueColor rendering at >60 fps at ~900 chars wide. 

## Usage

```csharp
using CustomConsole;
```
```csharp
ConsoleBuffer.Clear(); // fills ConsoleBuffer to be black

ConsoleBuffer.Fill(255, 0, 0); / fills ConsoleBuffer to be red

ConsoleBuffer.ResizeBuffer(); // resizes ConsoleBuffer to be the size of active window

ConsoleBuffer.Write(0, 0, "Hello, World!", 255, 255, 255); // Write "Hello, World!" at 0, 0 in white

ConsoleBuffer.Write("Yo.", 0, 255, 0); // Write "Yo" at current Cursor position in green

ConsoleBuffer.Write(10, 7, "Line 1\nLine 2\n\tTabbed Indent"); // multi-line escape sequence tracking
```
```csharp
// You *must* call this once per frame, otherwise it will not work
ConsoleBuffer.Draw(); // Draws the ConsoleBuffer to the Console
```
<img width="1901" height="1079" alt="Screenshot 2026-06-23 165445" src="https://github.com/user-attachments/assets/e5a3554f-94c1-4eed-8256-ded8060f73b3" />
