namespace MazeVisualiser.Generators
{
    public struct GeneratorStep
    {
        public ushort X { get; }
        public ushort Y { get; }
        public GeneratorStepType Type { get; }

        public GeneratorStep(ushort x, ushort y, GeneratorStepType type)
        {
            X = x;
            Y = y;
            Type = type;
        }
    }
}
