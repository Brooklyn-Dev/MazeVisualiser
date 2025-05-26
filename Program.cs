using SFML.Graphics;
using SFML.System;
using SFML.Window;

using MazeVisualiser.Generators;
using MazeVisualiser.State;

namespace MazeVisualiser
{
    internal class Program
    {
        private const ushort CellSize = 20;

        private static MazeState? mazeState;

        private static bool paused = false;
        private static float stepInterval = 0.05f;
        private static float elapsedTime = 0f;

        static void Main()
        {
            ushort mazeWidth = 32;
            ushort mazeHeight = 32;

            mazeState = new MazeState(mazeWidth, mazeHeight, new BacktrackingGenerator());

            var clock = new Clock();

            var win = new RenderWindow(new VideoMode((uint)(mazeState.Width * CellSize), (uint)(mazeState.Height * CellSize)), "Maze Visualiser");

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
            var rect = new RectangleShape(new Vector2f(CellSize, CellSize))
            {
                FillColor = Color.White
            };

            for (int y = 0; y < mazeState.Height; y++)
                for (int x = 0; x < mazeState.Width; x++)
                    if (mazeState.Maze[y, x])
                    {
                        rect.Position = new Vector2f(x * CellSize, y * CellSize);
                        win.Draw(rect);
                    }
        }
    }
}