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

        static void Main(string[] args)
        {
            ushort mazeWidth = 32;
            ushort mazeHeight = 32;

            var gen = new BacktrackingGenerator();
            maze = gen.Generate(mazeWidth, mazeHeight);

            var win = new RenderWindow(new VideoMode((uint)maze.GetLength(0) * CellSize, (uint)maze.GetLength(1) * CellSize), "Maze Visualiser");
            win.Closed += (_, __) => win.Close();
            win.SetFramerateLimit(30);
            win.SetActive(true);

            while (win.IsOpen)
            {
                win.DispatchEvents();
                win.Clear(Color.Black);

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