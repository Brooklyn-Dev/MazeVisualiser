using SFML.Graphics;
using SFML.System;
using SFML.Window;

using MazeVisualiser.Generators;
using MazeVisualiser.Solvers;
using MazeVisualiser.State;

namespace MazeVisualiser
{
    // Handles maze generation, solving, rendering and input events
    internal class MazeApp
    {
        private readonly RenderWindow window;
        private readonly MazeConfig config;
        private MazeGenerationState generationState;
        private MazeSolvingState? solvingState = null;

        private AppState state;
        private (ushort X, ushort Y)? startPoint = null;
        private (ushort X, ushort Y)? endPoint = null;

        private bool paused = false;
        private float stepInterval = 0.05f;
        private float elapsedTime = 0f;

        public MazeApp(MazeConfig config)
        {
            this.config = config;

            window = new RenderWindow(new VideoMode((uint)(config.Width * config.CellSize), (uint)(config.Height * config.CellSize)), "Maze Visualiser");
            window.Closed += (_, __) => window.Close();
            window.KeyPressed += OnKeyPressed;
            window.MouseButtonPressed += OnMouseButtonPressed;

            generationState = new MazeGenerationState(config.Width, config.Height, new BacktrackingGenerator());
            state = AppState.Generating;
        }

        // Main loop
        public void Run()
        {
            var clock = new Clock();
            window.SetFramerateLimit(60);
            window.SetActive(true);

            while (window.IsOpen)
            {
                window.DispatchEvents();
                window.Clear(Color.Black);

                var deltaTime = clock.Restart().AsSeconds();
                if (!paused)
                    Update(deltaTime);

                Draw();
                window.Display();
            }
        }

        // Initialise maze solving state
        private void StartSolving()
        {
            if (!startPoint.HasValue || !endPoint.HasValue)
                return;

            solvingState = new MazeSolvingState(generationState.Maze, new BFSSolver(), startPoint.Value, endPoint.Value);
            state = AppState.Solving;
            paused = false;
            elapsedTime = 0f;
        }

        // Update application logic based on current state
        private void Update(float deltaTime)
        {
            elapsedTime += deltaTime;

            switch (state)
            {
                case AppState.Generating:
                    StepGeneration();
                    break;

                case AppState.Solving:
                    StepSolving();
                    break;

                default: break;
            }
        }

        // Advance maze generation by one or more steps, controlled by stepInterval
        private void StepGeneration()
        {
            if (stepInterval <= 0f)
            {
                while (generationState.Step()) { }

                state = AppState.Generated;
                elapsedTime = 0f;
                paused = false;
            }
            else
                while (elapsedTime >= stepInterval)
                {
                    if (!generationState.Step())
                    {
                        state = AppState.Generated;
                        elapsedTime = 0f;
                        paused = false;
                        break;
                    }

                    elapsedTime -= stepInterval;
                }
        }

        // Advance maze solving by one or more steps, controlled by stepInterval
        private void StepSolving()
        {
            if (solvingState == null)
                return;

            if (stepInterval <= 0f)
                while (solvingState.Step()) { }
            else
                while (elapsedTime >= stepInterval)
                {
                    if (!solvingState.Step())
                    {
                        state = AppState.Solved;
                        elapsedTime = 0f;
                        paused = true;
                        break;
                    }

                    elapsedTime -= stepInterval;
                }
        }

        // Draws maze cells with colours coding for generation and solving states
        private void Draw()
        {
            var rect = new RectangleShape(new Vector2f(config.CellSize, config.CellSize))
            {
                FillColor = Color.White
            };

            for (int y = 0; y < generationState.Height; y++)
                for (int x = 0; x < generationState.Width; x++)
                    if (generationState.Maze[y, x])
                    {
                        rect.FillColor = Color.White;

                        if (state == AppState.Generating)
                        { 
                            if (generationState.CellStates[y, x] == GeneratorStepType.Stack) 
                                rect.FillColor = Color.Blue;
                        }

                        else if ((state == AppState.Solving || state == AppState.Solved) && solvingState != null)
                            switch (solvingState.CellStates[y, x])
                            {
                                case SolverStepType.Visited:
                                    rect.FillColor = Color.Blue;
                                    break;

                                case SolverStepType.Frontier:
                                    rect.FillColor = Color.Cyan;
                                    break;

                                case SolverStepType.Path:
                                    rect.FillColor = Color.Yellow;
                                    break;
                            }
                        
                        rect.Position = new Vector2f(x * config.CellSize, y * config.CellSize);
                        window.Draw(rect);
                    }

            if (startPoint.HasValue)
            {
                rect.FillColor = Color.Green;
                rect.Position = new Vector2f(startPoint.Value.X * config.CellSize, startPoint.Value.Y * config.CellSize);
                window.Draw(rect);
            }

            if (endPoint.HasValue)
            {
                rect.FillColor = Color.Red;
                rect.Position = new Vector2f(endPoint.Value.X * config.CellSize, endPoint.Value.Y * config.CellSize);
                window.Draw(rect);
            }
        }

        // Handles keyboard inputs
        private void OnKeyPressed(object? sender, KeyEventArgs e)
        {
            switch (e.Code)
            {
                // Pause visualisation
                case Keyboard.Key.Space:
                    paused = !paused;
                    break;

                // Restart generation
                case Keyboard.Key.R:
                    state = AppState.Generating;
                    generationState.Reset(new BacktrackingGenerator());
                    solvingState = null;
                    startPoint = null;
                    endPoint = null;
                    paused = false;
                    elapsedTime = 0f;
                    break;

                // Increase visualisation speed
                case Keyboard.Key.Up:
                    stepInterval = MathF.Max(0f, stepInterval - 0.005f);
                    break;

                // Decrease visualisation speed
                case Keyboard.Key.Down:
                    stepInterval += 0.005f;
                    break;
            }
        }

        // Handles mouse inputs
        private void OnMouseButtonPressed(object? sender, MouseButtonEventArgs e)
        {
            if (state != AppState.Generated)
                return;

            var x = (ushort)(e.X / config.CellSize);
            var y = (ushort)(e.Y / config.CellSize);

            if (x >= config.Width || y >= config.Height || !generationState.Maze[y, x])
                return;

            // Set solver start point
            if (e.Button == Mouse.Button.Left)
            {
                startPoint = (x, y);
                if (startPoint == endPoint)
                    endPoint = null;
            }

            // Set solver end point
            else if (e.Button == Mouse.Button.Right)
            {   
                endPoint = (x, y);
                if (endPoint == startPoint)
                    startPoint = null;
            }

            // Start solving when start and end points are defined
            if (startPoint.HasValue && endPoint.HasValue)
                StartSolving();
        }
    }
}