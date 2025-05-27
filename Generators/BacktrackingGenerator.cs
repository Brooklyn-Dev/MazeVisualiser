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
            var visited = new bool[height, width];

            stack.Push(startPos);
            visited[startPos.Y, startPos.X] = true;
            yield return new GeneratorStep(startPos.X, startPos.Y, GeneratorStepType.Stack);

            while (stack.Count > 0)
            {
                var (x, y) = stack.Peek();

                bool carved = false;
                var dirs = ShuffleDirections();

                foreach (var (dx, dy) in dirs)
                {
                    ushort newX = (ushort)(x + dx * 2);
                    ushort newY = (ushort)(y + dy * 2);

                    if (IsValidPosition(newX, newY, width, height) && !maze[newY, newX])
                    {
                        ushort midX = (ushort)(x + dx);
                        ushort midY = (ushort)(y + dy);

                        visited[midY, midX] = true;
                        visited[newY, newX] = true;

                        maze[midY, midX] = true;
                        maze[newY, newX] = true;

                        stack.Push((midX, midY));
                        stack.Push((newX, newY));

                        yield return new GeneratorStep(midX, midY, GeneratorStepType.Stack);
                        yield return new GeneratorStep(newX, newY, GeneratorStepType.Stack);

                        carved = true;
                        break;
                    }
                }

                if (!carved)
                {
                    var cell = stack.Pop();
                    yield return new GeneratorStep(cell.X, cell.Y, GeneratorStepType.Carved);
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