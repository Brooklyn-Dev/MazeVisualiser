using MazeVisualiser.Generators;

namespace MazeVisualiser.State
{
    public class MazeGenerationState
    {
        public bool[,] Maze { get; private set; }
        public IEnumerator<GeneratorStep> Steps { get; private set; }
        public IMazeGenerator Generator { get; private set; }

        public ushort Width { get; }
        public ushort Height { get; }

        public MazeGenerationState(ushort width, ushort height, IMazeGenerator generator)
        {
            Width = width;
            Height = height;

            Reset(generator);
        }

        public void Reset(IMazeGenerator generator)
        {
            Generator = generator;
            Maze = new bool[Height, Width];
            Steps = Generator.GenerateSteps(Width, Height).GetEnumerator();
        }

        public bool Step()
        {
            if (Steps.MoveNext())
            {
                var step = Steps.Current;
                Maze[step.Y, step.X] = step.IsPath;
                return true;
            }

            return false;
        }
    }
}
