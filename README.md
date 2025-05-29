# MazeVisualiser

![License](https://img.shields.io/badge/license-MIT-green)
![GitHub release](https://img.shields.io/github/v/release/Brooklyn-Dev/MazeVisualiser)
![GitHub issues](https://img.shields.io/github/issues/Brooklyn-Dev/MazeVisualiser)
![.Net version](https://img.shields.io/badge/.NET-9.0-blue)
![SFML version](https://img.shields.io/badge/SFML-2.6.2-orange)

Generate and solve mazes with real-time visualisation, using DFS for generation and BFS for solving.

![Example](Example.gif)

## Features

- **Maze Generation**: Create mazes using depth-first search (DFS) algorithm
- **Maze Solving**: Find optimal paths using breadth-first search (BFS) algorithm
- **Real-time Visualisation**: Watch algorithms work step-by-step
- **Speed Control**: Adjust visualisation speed with UP/DOWN arrow keys
- **Interactive Controls**: Pause, restart, and control the visualisation
- **Custom Start/End Points**: Click to place solver start and end points
- **Configurable Parameters**: Set maze dimensions and cell size via command-line arguments

## Installation

### Building from Source

Before building MazeVisualiser, ensure you have the following installed:

- .NET 9 SDK
- SFML 2.6.2

Then follow the build steps:

1. Clone the repository: `git clone https://github.com/Brooklyn-Dev/MazeVisualiser.git`

2. Navigate to the project directory: `cd MazeVisualiser`

3. Restore NuGet packages: `dotnet restore`

4. Build the project: `dotnet build --configuration Release`

5. Run the application: `dotnet run --configuration Release`
   - Executable: `./bin/Release/net9.0/MazeVisualiser [options]`
   - With dotnet: `dotnet run --configuration Release -- [options]`

### Installing Pre-built Releases

1. Download the latest release from the [Releases](https://github.com/Brooklyn-Dev/MazeVisualiser/releases) page
2. Extract the archive to your desired location
3. Run the executable

## Usage

```
MazeVisualiser

  Options:
    -w, --width <integer>       Set maze width (default 33)
    -h, --height <integer>      Set maze height (default 33)
    -c, --cellsize <integer>    Set cell size in pixels (default 20)
    --help                      Show this help message

  Controls:
    SPACE  Pause visualisation
    R      Restart generation
    UP     Increase visualisation speed
    DOWN   Decrease visualisation speed
    LMB    Place solver start point
    RMB    Place solver end point
```

## Colour Legend

- **Black**: Walls
- **White**: Path
- **Blue**: Tracked
- **Cyan**: Frontier
- **Green**: Solver start point
- **Red**: Solver end point
- **Yellow**: Shortest Path

## Built with

- C# / .NET 9 - Core language and framework
- SFML 2.6.2 - Simple and Fast Multimedia Library for graphics, window management, and input handling
- SFML.Net 2.6.1 - C# binding for SFML

## Like this project?

If you find this project interesting or useful, consider giving it a star ⭐️!
