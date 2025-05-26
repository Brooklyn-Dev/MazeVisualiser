using MazeVisualiser.Utils;

namespace MazeVisualiser.Solvers
{
    public class BFSSolver : IMazeSolver
    {
        public IEnumerable<List<SolverStep>> SolveSteps(bool[,] maze, (ushort X, ushort Y) start, (ushort X, ushort Y) end)
        {
            var width = (ushort)maze.GetLength(1);
            var height = (ushort)maze.GetLength(0);

            var visited = new bool[height, width];
            var parent = new Dictionary<(ushort, ushort), (ushort, ushort)>();
            var queue = new Queue<(ushort x, ushort y)>();

            visited[start.Y, start.X] = true;
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var stepList = new List<SolverStep>();
                int levelCount = queue.Count;

                for (uint i = 0; i < levelCount; i++)
                {
                    var current = queue.Dequeue();
                    stepList.Add(new SolverStep(current.x, current.y, SolverStepType.Visited));

                    foreach (var neighbour in GetNeighbours(current, width, height))
                    {
                        var (nx, ny) = neighbour;
                        if (!IsValidStep(maze, visited, nx, ny))
                            continue;

                        visited[ny, nx] = true;
                        queue.Enqueue(neighbour);
                        parent[neighbour] = current;

                        stepList.Add(new SolverStep(nx, ny, SolverStepType.Frontier));

                        if (neighbour == end)
                        {
                            yield return stepList;

                            foreach (var pathStep in BacktrackSteps(start, end, parent))
                                yield return new List<SolverStep> { pathStep };

                            yield break;
                        }                  
                    }
                }

                yield return stepList;
            }
        }

        private static IEnumerable<(ushort X, ushort Y)> GetNeighbours((ushort X, ushort Y) pos, ushort width, ushort height)
        {
            foreach (var (dx, dy) in Directions.Cardinal)
            {
                var nx = (ushort)(pos.X + dx);
                var ny = (ushort)(pos.Y + dy);

                if (nx < width && ny < height)
                    yield return (nx, ny);
            }
        }

        private static IEnumerable<SolverStep> BacktrackSteps((ushort X, ushort Y) start, (ushort X, ushort Y) end, Dictionary<(ushort, ushort), (ushort, ushort)> parent)
        {
            var current = end;

            while (!current.Equals(start))
            {
                yield return new SolverStep(current.X, current.Y, SolverStepType.Path);
                current = parent[current];
            }

            yield return new SolverStep(start.X, start.Y, SolverStepType.Path);
        }

        private static bool IsValidStep(bool[,] maze, bool[,] visited, ushort x, ushort y)
            => x < maze.GetLength(1) && y < maze.GetLength(0) && maze[y, x] && !visited[y, x];
    }
}