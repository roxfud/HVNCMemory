# HVNCMemory - Hidden Virtual Network Computing Module

<div align="center">
  <p>An advanced, lightweight, and efficient Hidden VNC (hVNC) implementation for Windows, written in C# and targeting the .NET Framework 4.8.</p>
</div>

---

## 📖 Overview

HVNCMemory is a specialized remote administration module designed to create and interact with a **Hidden Desktop** environment on Windows systems. It allows administrators, developers, or security researchers to spawn a secondary, completely invisible workspace. 

In this hidden environment, you can silently execute applications (such as web browsers, file managers, or custom tools) and control them remotely using simulated hardware input. All of this happens entirely in the background, ensuring that the primary user's visible session remains completely uninterrupted and unaware of the background activities.

## ✨ Key Features

- **🛡️ Invisible Desktop Instantiation:** Leverages deep Windows Native APIs (`CreateDesktop`, `OpenDesktop`) to construct a secure and completely hidden secondary desktop session.
- **🚀 Silent Application Execution:** Launch and operate applications directly onto the hidden desktop. Target applications will not spawn taskbar icons, window frames, or any visible traces on the user's main screen.
- **🔄 Recursive Process Tracking:** Uses WMI (Windows Management Instrumentation) to dynamically track, collect, and bind complex child processes (like browser tabs, extensions, and child workers) strictly to the hidden desktop.
- **🖱️ Raw Input Emulation Engine:** Standard cursor and keyboard controls do not function on invisible desktops. This module mathematically calculates coordinates and dispatches low-level Windows messages (`WM_LBUTTONDOWN`, `WM_KEYDOWN`, `WM_MOUSEMOVE`, etc.) to directly interact with target window handles (`HWND`).
- **📸 High-Performance Screen Streaming:** Features an optimized frame-capture engine that takes snapshots of the hidden environment, compresses them into a memory-efficient JPEG stream, and prepares them for rapid socket transmission.
- **⚙️ Window Manipulation:** Includes full support for minimizing, maximizing, restoring, and resizing hidden application windows remotely.

## 🧠 Core Architecture

The repository is modularized into several key components to separate concerns and maintain clean code:

### 1. `HideDesktop.cs` (The Environment Manager)
The core engine that interfaces with `user32.dll` and `kernel32.dll`. It is responsible for the creation, selection, and lifecycle management of the hidden desktop. It also handles the complex task of spawning new processes (`CreateProcess`) safely inside this isolated environment.

### 2. `HandelMouse.cs` (The Input Simulator)
Because the desktop is physically invisible to the local graphics driver, this class acts as a virtual interaction layer. It uses `WindowFromPoint` to identify GUI elements and injects hardware-level instructions (Clicks, Scrolls, Keystrokes) directly into the application's message loop.

### 3. `RemoteDesktop.cs` (The Display Server)
Acts as the visual bridge between the hidden desktop and the remote operator. It securely captures the screen buffer, applies quality-based image compression, and formats the payload (Base64/Byte Array) for network transport.

## 💻 Technical Specifications

- **Language:** C#
- **Framework Target:** .NET Framework 4.8 (Compatible with modern Windows OS)
- **Primary Dependencies:** `System.Management`, `System.Drawing`
- **Native API Interop:** Extensive use of P/Invoke for `user32.dll`, `kernel32.dll`, and `gdi32.dll`.

## ⚠️ Disclaimer

**Educational and Research Purposes Only.**
This project is developed and published strictly for educational purposes, cybersecurity research, and authorized systems administration. The developer assumes no liability and is not responsible for any misuse or damage caused by this software. You must only deploy this on systems you own or have explicit, documented permission to interact with.
