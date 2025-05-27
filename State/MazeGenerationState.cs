using MazeVisualiser.Generators;

namespace MazeVisualiser.State
{
    public class MazeGenerationState
    {
        public bool[,] Maze { get; private set; }
        public GeneratorStepType[,] CellStates { get; private set; }
        public IMazeGenerator Generator { get; private set; }
        public IEnumerator<GeneratorStep> Steps { get; private set; }

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
            Maze = new bool[Height, Width];
            CellStates = new GeneratorStepType[Height, Width];
            Generator = generator;
            Steps = Generator.GenerateSteps(Width, Height).GetEnumerator();
        }

        public bool Step()
        {
            if (Steps.MoveNext())
            {
                var step = Steps.Current;
                Maze[step.Y, step.X] = true;
                CellStates[step.Y, step.X] = step.Type;
                return true;
            }

            return false;
        }
    }
}
