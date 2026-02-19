```diff
+ ████████╗ ███████╗ ██████╗  ███╗   ███╗ ██║ ██║   ██╗  █████╗  ██║      ██╗    ██╗  █████╗  ██║   ██╗ ███████╗
+ ╚══██╔══╝ ██╔════╝ ██╔══██╗ ████╗ ████║ ██║ ████╗ ██║ ██╔══██╗ ██║      ██║    ██║ ██╔══██╗ ██║   ██║ ██╔════╝
+    ██║    █████╗   ██████╔╝ ██╔████╔██║ ██║ ██╔██╗██║ ███████║ ██║      ██║ █╗ ██║ ███████║ ██║   ██║ █████╗  
+    ██║    ██╔══╝   ██╔══██╗ ██║╚██╔╝██║ ██║ ██║╚████║ ██╔══██║ ██║      ██║███╗██║ ██╔══██║ ╚██╗ ██╔╝ ██╔══╝  
+    ██║    ███████╗ ██║  ██║ ██║ ╚═╝ ██║ ██║ ██║ ╚███║ ██║  ██║ ███████╗ ╚███╔███╔╝ ██║  ██║  ╚████╔╝  ███████╗
+    ╚═╝    ╚══════╝ ╚═╝  ╚═╝ ╚═╝     ╚═╝ ╚═╝ ╚═╝  ╚══╝ ╚═╝  ╚═╝ ╚══════╝  ╚══╝╚══╝  ╚═╝  ╚═╝   ╚═══╝   ╚══════╝
```
## 💾 About the Project

**TerminalWave** is a console-based music player built with C# and .NET. Designed with a clean Terminal User Interface (TUI), it automatically discovers and plays `.mp3` and `.wav` files located in the user's standard `MyMusic` directory.

The application strictly adheres to the **MVVM (Model-View-ViewModel)** architectural pattern, ensuring a clean separation of concerns between the underlying audio processing logic and the console rendering loop.

<img width="1464" height="746" alt="image" src="https://github.com/user-attachments/assets/c1560aa9-3a0d-42c4-a22e-272f5626ecd4" />

## ⚡ Features

* **Automated Library Discovery:** Recursively scans the system's `MyMusic` folder for `.mp3` files on startup.
* **Real-Time TUI:** Features a responsive, flicker-free console interface with a dynamic playback progress bar.
* **Playback Controls:** Supports play, pause, next/previous track navigation, and ±5-second seeking.
* **Continuous Playback:** Automatically advances to the next track in the playlist when the current song finishes.
* **Resource Management:** Implements `IDisposable` interfaces to ensure safe memory handling and audio device release upon exit.

## 🛠️ Technology Stack & Architecture

* **Language/Framework:** C#, .NET
* **Audio Engine:** [NAudio](https://github.com/naudio/NAudio) (`WaveOutEvent`, `AudioFileReader`)
* **Architecture:** MVVM  
   * **Entities:** Defines the core `MusicEntity` data structure.  
   * **Services:** `MusicService` handles file system operations; `PlayerService` encapsulates the NAudio engine logic.  
   * **ViewModel:** `PlayerViewModel` acts as the intermediary, exposing state and commands to the UI.  
   * **View:** `Program.cs` manages the synchronous console rendering loop and input interception.

## 🎮 Controls

The TUI is entirely keyboard-driven. Ensure the console window is active to use the following shortcuts:

| **Key**   | **Action**                |
| --------- | ------------------------- |
| \[Space\] | Toggle Play / Pause       |
| \[N\]     | Next Track                |
| \[P\]     | Previous Track            |
| \[F\]     | Fast Forward (+5 seconds) |
| \[B\]     | Rewind (-5 seconds)       |
| \[Q\]     | Quit Application          |

## 👨‍💻 Development Team

This project was developed collaboratively, dividing the underlying logic and the visual presentation:

* **Backend / Audio Engine:** [@dombi-balazs](https://github.com/dombi-balazs)
* **Frontend / TUI (Console UI):** [@K.Gergo2024](https://www.google.com/search?q=https://github.com/K.Gergo2024)

---

_Stay sync'd._ 🌊
