using MazeVisualiser.Utils;

namespace MazeVisualiser.Solvers
{
    public class BFSSolver : IMazeSolver
    {
        // Performs BFS on maze, yieldingsolver steps for visualisation
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
                int levelCount = queue.Count;  // Number of nodes at current BFS level

                for (uint i = 0; i < levelCount; i++)
                {
                    var current = queue.Dequeue();
                    stepList.Add(new SolverStep(current.x, current.y, SolverStepType.Visited));

                    // Explore all adjacent neighbours
                    foreach (var neighbour in Directions.GetNeighbours(Directions.Cardinal, current, width, height))
                    {
                        var (nx, ny) = neighbour;

                        if (!IsValidStep(maze, visited, nx, ny))
                            continue;

                        visited[ny, nx] = true;
                        queue.Enqueue(neighbour);
                        parent[neighbour] = current;

                        stepList.Add(new SolverStep(nx, ny, SolverStepType.Frontier));

                        // If end is reached, yield path steps and stop
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

        // Backtrack from end to start using parent map to yield solution path steps
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

        // Check step is bounded, on a path cell and not visited yet
        private static bool IsValidStep(bool[,] maze, bool[,] visited, ushort x, ushort y)
            => x < maze.GetLength(1) && y < maze.GetLength(0) && maze[y, x] && !visited[y, x];
    }
}