namespace MazeVisualiser.Generators
{
    public class BacktrackingGenerator : IMazeGenerator
    {
        private static readonly (sbyte, sbyte)[] Directions =
        {
            (0, -1),  // UP
            (0, 1),  // DOWN
            (-1, 0),  // LEFT
            (1, 0)  // RIGHT
        };

        private readonly Random _random = new();

        public IEnumerable<GeneratorStep> GenerateSteps(ushort width, ushort height)
        {
            var maze = new bool[height, width];  // default false = wall
            foreach (var step in CarvePath(1, 1, maze, width, height))
                yield return step;
        }

        private IEnumerable<GeneratorStep> CarvePath(ushort x, ushort y, bool[,] maze, ushort width, ushort height)
        {
            maze[y, x] = true;
            yield return new GeneratorStep(x, y, true);

            var dirs = new List<(sbyte, sbyte)>(Directions);
            // Fisher-Yates shuffle
            for (int i = dirs.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (dirs[i], dirs[j]) = (dirs[j], dirs[i]);
            }

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

                    foreach (var step in CarvePath(newX, newY, maze, width, height))
                        yield return step;
                }
            }
        }

        private bool IsValidPosition(ushort x, ushort y, ushort width, ushort height)
            => x < width && y < height && x % 2 == 1 && y % 2 == 1;
    }
}