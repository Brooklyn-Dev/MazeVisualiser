using SFML.Graphics;
using SFML.System;
using SFML.Window;

using MazeVisualiser.Generators;

namespace MazeVisualiser
{
    internal class Program
    {
        private const ushort CellSize = 20;
        private static bool[,]? maze;

        static void Main()
        {
            ushort mazeWidth = 32;
            ushort mazeHeight = 32;

            mazeWidth = mazeWidth % 2 == 0 ? (ushort)(mazeWidth + 1) : mazeWidth;
            mazeHeight = mazeHeight % 2 == 0 ? (ushort)(mazeHeight + 1) : mazeHeight;

            var gen = new BacktrackingGenerator();
            var steps = gen.GenerateSteps(mazeWidth, mazeHeight).GetEnumerator();

            maze = new bool[mazeHeight, mazeWidth];

            var clock = new Clock();
            float elapsedTime = 0f;
            float stepInterval = 0.01f;
            bool paused = false;

            var win = new RenderWindow(new VideoMode((uint)(mazeWidth * CellSize), (uint)(mazeHeight * CellSize)), "Maze Visualiser");
            win.Closed += (_, __) => win.Close();
            win.SetFramerateLimit(60);
            win.SetActive(true);

            while (win.IsOpen)
            {
                win.DispatchEvents();
                win.Clear(Color.Black);

                var deltaTime = clock.Restart().AsSeconds();
                elapsedTime += deltaTime;

                if (!paused)
                {
                    if (stepInterval <= 0f)
                        while (steps.MoveNext())
                        {
                            var step = steps.Current;
                            maze[step.Y, step.X] = step.IsPath;
                        }
                    else
                        while (elapsedTime >= stepInterval)
                        {
                            if (!steps.MoveNext()) break;

                            var step = steps.Current;
                            maze[step.Y, step.X] = step.IsPath;

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

            for (int y = 0; y < maze?.GetLength(0); y++)
                for (int x = 0; x < maze.GetLength(1); x++)
                    if (maze[y, x])
                    {
                        rect.Position = new Vector2f(x * CellSize, y * CellSize);
                        win.Draw(rect);
                    }
        }
    }
}