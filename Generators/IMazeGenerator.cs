namespace MazeVisualiser.Generators
{
    public interface IMazeGenerator
    {
        IEnumerable<GeneratorStep> GenerateSteps(ushort width, ushort height);
    }
}
