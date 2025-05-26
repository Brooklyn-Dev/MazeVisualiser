namespace MazeVisualiser.Generators
{
    public struct GeneratorStep
    {
        public ushort X, Y;
        public bool IsPath;

        public GeneratorStep(ushort x, ushort y, bool isPath)
        {
            X = x;
            Y = y;
            IsPath = isPath;
        }
    }
}
