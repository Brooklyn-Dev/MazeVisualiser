using SFML.Graphics;
using SFML.System;
using SFML.Window;

using MazeVisualiser.Generators;
using MazeVisualiser.State;

namespace MazeVisualiser
{
    internal class Program
    {
        private static ushort cellSize;

        private static MazeState? mazeState;

        private static bool paused = false;
        private static float stepInterval = 0.05f;
        private static float elapsedTime = 0f;

        static void Main(string[] args)
        {
            ushort width = 32;
            ushort height = 32;
            cellSize = 20;

            var options = new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["-w"] = val => width = ushort.Parse(val),
                ["--width"] = val => width = ushort.Parse(val),
                ["-h"] = val => height = ushort.Parse(val),
                ["--height"] = val => height = ushort.Parse(val),
                ["-c"] = val => cellSize = ushort.Parse(val),
                ["--cellsize"] = val => cellSize = ushort.Parse(val),
            };

            for (int i = 0; i < args.Length; i++)
            {
                if (options.TryGetValue(args[i], out var setter))
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        try
                        {
                            setter(args[i + 1]);
                            i++;
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine($"Error: Invalid value for {args[i]}: {args[i + 1]}");
                            Environment.Exit(1);
                        }
                    }
                    else
                    { 
                        Console.WriteLine($"Error: Missing value for {args[i]}");
                        Environment.Exit(1);
                    }
                else
                {
                    Console.WriteLine($"Error: Unknown argument: {args[i]}");
                    Environment.Exit(1);
                }
            }

            mazeState = new MazeState(width, height, new BacktrackingGenerator());

            var clock = new Clock();

            var win = new RenderWindow(new VideoMode((uint)(mazeState.Width * cellSize), (uint)(mazeState.Height * cellSize)), "Maze Visualiser");

            win.Closed += (_, __) => win.Close();

            win.KeyPressed += (_, e) =>
            {
                // Pause visualisation
                if (e.Code == Keyboard.Key.Space)
                    paused = !paused;

                // Restart generation
                else if (e.Code == Keyboard.Key.R)
                {
                    mazeState.Reset(new BacktrackingGenerator());
                    paused = false; 
                    elapsedTime = 0f;
                }

                // Increase visualisation speed
                else if (e.Code == Keyboard.Key.Up)
                    stepInterval = MathF.Max(0f, stepInterval - 0.01f);

                // Decrease visualisation speed
                else if (e.Code == Keyboard.Key.Down)
                    stepInterval += 0.01f;
            };

            win.SetFramerateLimit(60);
            win.SetActive(true);

            while (win.IsOpen)
            {
                win.DispatchEvents();
                win.Clear(Color.Black);

                var deltaTime = clock.Restart().AsSeconds();

                if (!paused)
                {
                    elapsedTime += deltaTime;

                    if (stepInterval <= 0f)
                        while (mazeState.Step()) { }
                    else
                        while (elapsedTime >= stepInterval)
                        {
                            if (!mazeState.Step()) break;

                            elapsedTime -= stepInterval;
                        }
                }

                DrawMaze(win);

                win.Display();
            }
        }

        private static void DrawMaze(RenderWindow win)
        {
            var rect = new RectangleShape(new Vector2f(cellSize, cellSize))
            {
                FillColor = Color.White
            };

            for (int y = 0; y < mazeState?.Height; y++)
                for (int x = 0; x < mazeState.Width; x++)
                    if (mazeState.Maze[y, x])
                    {
                        rect.Position = new Vector2f(x * cellSize, y * cellSize);
                        win.Draw(rect);
                    }
        }
    }
}