using System;
using System.Collections.Generic;

namespace MazeVisualiser.Generators
{
    public class BacktrackingGenerator : IMazeGenerator
    {
        private static readonly (int, int)[] Directions =
        {
            (0, -1),  // UP
            (0, 1),  // DOWN
            (-1, 0),  // LEFT
            (1, 0)  // RIGHT
        };


        private readonly Random _random;
        private bool[,] _maze;
        private int _width;
        private int _height;

        public BacktrackingGenerator() => _random = new Random();

        public BacktrackingGenerator(int seed) => _random = new Random(seed);

        public bool[,] Generate(int width, int height)
        {
            _width = width % 2 == 0 ? width + 1 : width;
            _height = height % 2 == 0 ? height + 1 : height;

            _maze = new bool[_height, _width];  // default false = wall

            CarvePath(1, 1);

            return _maze;
        }

        private void CarvePath(int x, int y)
        {
            _maze[y, x] = true;

            var directions = new List<(int, int)>(Directions);
            // Fisher-Yates shuffle
            for (int i = directions.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                var temp = directions[i];
                directions[i] = directions[j];
                directions[j] = temp;
            }

            foreach (var (dx, dy) in directions)
            {
                int newX = x + dx * 2;
                int newY = y + dy * 2;

                if (IsValidPosition(newX, newY) && !_maze[newY, newX])
                {
                    _maze[y + dy, x + dx] = true;
                    CarvePath(newX, newY);
                }
            }
        }

        private bool IsValidPosition(int x, int y)
        {
            return x < _width && y < _height && x % 2 == 1 && y % 2 == 1;
        }
    }
}
