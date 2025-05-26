using MazeVisualiser.Generators;

namespace MazeVisualiser.State
{
    public class MazeState
    {
        public bool[,] Maze { get; private set; }
        public IEnumerator<GeneratorStep> Steps { get; private set; }
        public IMazeGenerator Generator { get; private set; }

        public ushort Width { get; }
        public ushort Height { get; }

        public MazeState(ushort width, ushort height, IMazeGenerator generator)
        {
            Width = width % 2 == 0 ? (ushort)(width + 1) : width;
            Height = height % 2 == 0 ? (ushort)(height + 1) : height;

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
