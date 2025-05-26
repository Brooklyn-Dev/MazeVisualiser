namespace MazeVisualiser.Solvers
{
    public struct SolverStep
    {
        public ushort X { get; }
        public ushort Y { get; }
        public SolverStepType Type { get; }

        public SolverStep(ushort x, ushort y, SolverStepType type)
        {
            X = x;
            Y = y;
            Type = type;
        }
    }
}