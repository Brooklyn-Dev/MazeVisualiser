using MazeVisualiser.Utils;

namespace MazeVisualiser.Generators
{
    public class BacktrackingGenerator : IMazeGenerator
    {
        public IEnumerable<GeneratorStep> GenerateSteps(ushort width, ushort height)
        {
            var maze = new bool[height, width];  // default false = wall

            foreach (var step in CarvePath((1, 1), maze, width, height))
                yield return step;
        }

        private IEnumerable<GeneratorStep> CarvePath((ushort X, ushort Y) startPos, bool[,] maze, ushort width, ushort height)
        {
            var stack = new Stack<(ushort X, ushort Y)>();
            stack.Push(startPos);

            while (stack.Count > 0)
            {
                var (x, y) = stack.Pop();

                if (!maze[y, x])
                {
                    maze[y, x] = true;
                    yield return new GeneratorStep(x, y, true);
                }

                var dirs = ShuffleDirections();

                foreach (var (dx, dy) in dirs)
                {
                    ushort newX = (ushort)(x + dx * 2);
                    ushort newY = (ushort)(y + dy * 2);

                    if (IsValidPosition(newX, newY, width, height) && !maze[newY, newX])
                    {
                        ushort midX = (ushort)(x + dx);
                        ushort midY = (ushort)(y + dy);

                        maze[midY, midX] = true;
                        yield return new GeneratorStep(midX, midY, true);

                        stack.Push((x, y));
                        stack.Push((newX, newY));
                        break;
                    }
                }
            }
        }

        private static List<(sbyte, sbyte)> ShuffleDirections()
        {
            var random = new Random();
            var dirs = new List<(sbyte, sbyte)>(Directions.Cardinal);
            // Fisher-Yates shuffle
            for (int i = dirs.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (dirs[i], dirs[j]) = (dirs[j], dirs[i]);
            }

            return dirs;
        }

        private static bool IsValidPosition(ushort x, ushort y, ushort width, ushort height)
            => x < width && y < height && x % 2 == 1 && y % 2 == 1;
    }
}