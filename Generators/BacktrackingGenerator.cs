using MazeVisualiser.Utils;

namespace MazeVisualiser.Generators
{
    public class BacktrackingGenerator : IMazeGenerator
    {
        // Performs DFS to make a maze, yielding generator steps for visualisation
        public IEnumerable<GeneratorStep> GenerateSteps(ushort width, ushort height)
        {
            var maze = new bool[height, width];  // false = wall, true = path

            // Start carving from top-left corner
            foreach (var step in CarvePath((1, 1), maze, width, height))
                yield return step;
        }

        private IEnumerable<GeneratorStep> CarvePath((ushort X, ushort Y) startPos, bool[,] maze, ushort width, ushort height)
        {
            var stack = new Stack<(ushort X, ushort Y)>();

            stack.Push(startPos);
            yield return new GeneratorStep(startPos.X, startPos.Y, GeneratorStepType.Stack);  // Mark initial stack cell

            while (stack.Count > 0)
            {
                var (x, y) = stack.Peek();

                bool carved = false;
                var dirs = Directions.ShuffleDirections(Directions.Cardinal);  // Randomise direction order

                foreach (var (dx, dy) in dirs)
                {
                    ushort newX = (ushort)(x + dx * 2);
                    ushort newY = (ushort)(y + dy * 2);

                    // Check if new position is valid and not already carved
                    if (IsValidPosition(newX, newY, width, height) && !maze[newY, newX])
                    {
                        ushort midX = (ushort)(x + dx);
                        ushort midY = (ushort)(y + dy);

                        maze[midY, midX] = true;
                        maze[newY, newX] = true;

                        // Push new cells to the stack for searching
                        stack.Push((midX, midY));
                        stack.Push((newX, newY));

                        // Yield steps to mark these cells as part of the stack
                        yield return new GeneratorStep(midX, midY, GeneratorStepType.Stack);
                        yield return new GeneratorStep(newX, newY, GeneratorStepType.Stack);

                        carved = true;
                        break;
                    }
                }

                if (!carved)
                {
                    // Dead end reached, backtrack and mark the cell as carved
                    var (X, Y) = stack.Pop();
                    yield return new GeneratorStep(X, Y, GeneratorStepType.Carved);
                }
            }
        }

        // Check position is bounded and has odd indicies
        private static bool IsValidPosition(ushort x, ushort y, ushort width, ushort height)
            => x < width && y < height && x % 2 == 1 && y % 2 == 1;
    }
}