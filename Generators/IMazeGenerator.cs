namespace MazeVisualiser.Generators
{
    public interface IMazeGenerator
    {
        bool[,] Generate(int width, int height);
    }
}
