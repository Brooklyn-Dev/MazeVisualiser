using MazeVisualiser.Solvers;

namespace MazeVisualiser.State
{
    public class MazeSolvingState
    {
        public SolverStepType[,] CellStates { get; private set; }
        public IEnumerator<List<SolverStep>> StepsList { get; private set; }
        public IMazeSolver Solver { get; private set; }

        public ushort Width { get; }
        public ushort Height { get; }

        public MazeSolvingState(bool[,] maze, IMazeSolver solver, (ushort X, ushort Y) start, (ushort X, ushort Y) end)
        {
            CellStates = new SolverStepType[maze.GetLength(0), maze.GetLength(1)];
            Solver = solver;
            StepsList = Solver.SolveSteps(maze, start, end).GetEnumerator();
        }

        public bool Step()
        {
            if (StepsList.MoveNext())
            {
                var steps = StepsList.Current;
                foreach (var step in steps)
                    CellStates[step.Y, step.X] = step.Type;

                return true;
            }

            return false;
        }
    }
}
