```diff
+ ████████╗ ███████╗ ██████╗  ███╗   ███╗ ██║ ██║   ██╗  █████╗  ██║      ██╗    ██╗  █████╗  ██║   ██╗ ███████╗
+ ╚══██╔══╝ ██╔════╝ ██╔══██╗ ████╗ ████║ ██║ ████╗ ██║ ██╔══██╗ ██║      ██║    ██║ ██╔══██╗ ██║   ██║ ██╔════╝
+    ██║    █████╗   ██████╔╝ ██╔████╔██║ ██║ ██╔██╗██║ ███████║ ██║      ██║ █╗ ██║ ███████║ ██║   ██║ █████╗  
+    ██║    ██╔══╝   ██╔══██╗ ██║╚██╔╝██║ ██║ ██║╚████║ ██╔══██║ ██║      ██║███╗██║ ██╔══██║ ╚██╗ ██╔╝ ██╔══╝  
+    ██║    ███████╗ ██║  ██║ ██║ ╚═╝ ██║ ██║ ██║ ╚███║ ██║  ██║ ███████╗ ╚███╔███╔╝ ██║  ██║  ╚████╔╝  ███████╗
+    ╚═╝    ╚══════╝ ╚═╝  ╚═╝ ╚═╝     ╚═╝ ╚═╝ ╚═╝  ╚══╝ ╚═╝  ╚═╝ ╚══════╝  ╚══╝╚══╝  ╚═╝  ╚═╝   ╚═══╝   ╚══════╝
```
## 💾 About the Project

**TerminalWave** is a console-based music player built with C# and .NET. Designed with a clean Terminal User Interface (TUI), it automatically discovers and plays `.mp3`, `.flac` and `.wav` files located in the user's standard `MyMusic` directory.

The application strictly adheres to the **MVVM (Model-View-ViewModel)** architectural pattern, ensuring a clean separation of concerns between the underlying audio processing logic and the console rendering loop.

<img width="972" height="719" alt="image" src="https://github.com/user-attachments/assets/ed9d021e-1b66-433d-bf7d-d93fb9e738ec" />

## ⚡ Features

* **Automated Library Discovery:** Recursively scans the system's `MyMusic` folder for `.mp3`, `.flac`, and `.wav` files on startup.
* **Real-Time TUI:** Features a responsive, flicker-free console interface with a dynamic playback progress bar.
* **Playback Controls:** Supports play, pause, next/previous track navigation, and ±5-second seeking.
* **Continuous Playback:** Automatically advances to the next track in the playlist when the current song finishes.
* **Resource Management:** Implements `IDisposable` interfaces to ensure safe memory handling and audio device release upon exit.
* **Dynamic Artist Visuals:** Automatically displays artist-specific ASCII art. The system looks for embedded resources and applies custom color schemes (e.g., ArtistName-Color.txt) to match the artist's vibe.

## 🛠️ Technology Stack & Architecture
* **Language/Framework**: C#, .NET 10

* **Audio Engine:** Windows Media Playback (MediaPlayer)

* **Meta Data:** TagLibSharp

* **Architecture:** MVVM

* **Entities:** Core MusicEntity and playback states.

* **Services:** MusicService for file discovery; PlayerService for audio control; ArtistService for embedded ASCII art retrieval.

* **ViewModel:** PlayerViewModel managing state, search logic, and UI commands.

* **View**: Program.cs handling the dynamic rendering loop and user input, the UI related code will be in the Views folder's files, as the project will use the Spectre.Console library in the near future.

## 🎮 Controls

The TUI is entirely keyboard-driven. Ensure the console window is active to use the following shortcuts:

| Key | Action |
| :--- | :--- |
| **Arrows Up/Down** | Navigate through the tracklist |
| **Enter** | Play selected track |
| **Space** | Toggle Play / Pause |
| **N** | Next Track |
| **P** | Previous Track |
| **F / B** | Fast Forward / Rewind (±5 seconds) |
| **T** | Search by Title |
| **A** | Search by Artist |
| **M** | Clear Search (Show all songs) |
| **R** | Refresh Music Library |
| **Q** | Quit Application |

## 🎨 Adding New Artist Art

TerminalWave supports custom ASCII art for your favorite artists. Simply:

* Create a `.txt` file with your ASCII art.

* Name it using the format: `Artist_Name-ConsoleColor.txt` (e.g., `Linkin_Park-Cyan.txt`).

* Add it to the `Artists/` folder as an EmbeddedResource.

## 👨‍💻 Development Team

This project was developed collaboratively, dividing the underlying logic and the visual presentation:

* **Backend / Audio Engine:** [@dombi-balazs](https://github.com/dombi-balazs)
* **Frontend / TUI (Console UI):** [@KiralyGergo](https://github.com/kiralygergo)

---

_Stay sync'd._ 🌊
