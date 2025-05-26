using SFML.Graphics;
using SFML.System;
using SFML.Window;

using MazeVisualiser.Generators;
using MazeVisualiser.State;

namespace MazeVisualiser
{
    internal class MazeApp
    {
        private readonly RenderWindow window;
        private readonly MazeConfig config;
        private MazeState mazeState;

        private bool paused = false;
        private float stepInterval = 0.05f;
        private float elapsedTime = 0f;

        public MazeApp(MazeConfig config)
        {
            this.config = config;
            mazeState = new MazeState(config.Width, config.Height, new BacktrackingGenerator());
            window = new RenderWindow(new VideoMode((uint)(config.Width * config.CellSize), (uint)(config.Height * config.CellSize)), "Maze Visualiser");

            window.Closed += (_, __) => window.Close();
            window.KeyPressed += OnKeyPressed;
        }

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

        private void Update(float deltaTime)
        {
            elapsedTime += deltaTime;

            if (stepInterval <= 0f)
                while (mazeState.Step()) { }
            else
                while (elapsedTime >= stepInterval)
                {
                    if (!mazeState.Step()) break;
                    elapsedTime -= stepInterval;
                }
        }

        private void Draw()
        {
            var rect = new RectangleShape(new Vector2f(config.CellSize, config.CellSize))
            {
                FillColor = Color.White
            };

            for (int y = 0; y < mazeState.Height; y++)
                for (int x = 0; x < mazeState.Width; x++)
                    if (mazeState.Maze[y, x])
                    {
                        rect.Position = new Vector2f(x * config.CellSize, y * config.CellSize);
                        window.Draw(rect);
                    }
        }

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
                    mazeState.Reset(new BacktrackingGenerator());
                    paused = false;
                    elapsedTime = 0f;
                    break;

                // Increase visualisation speed
                case Keyboard.Key.Up:
                    stepInterval = MathF.Max(0f, stepInterval - 0.01f);
                    break;

                // Decrease visualisation speed
                case Keyboard.Key.Down:
                    stepInterval += 0.01f;
                    break;
            }
        }
    }
}
