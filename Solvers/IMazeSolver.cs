namespace MazeVisualiser.Solvers
{
    public interface IMazeSolver
    {
        IEnumerable<List<SolverStep>> SolveSteps(bool[,] maze, (ushort X, ushort Y) start, (ushort X, ushort Y) end);
    }
}