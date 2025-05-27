namespace MazeVisualiser
{
    // Store maze configuration parameters
    internal record MazeConfig(ushort Width, ushort Height, ushort CellSize);

    internal static class ArgsParser
    {
        // Parses command-line arguments into MazeConfig with validation and error logging
        public static MazeConfig Parse(string[] args)
        {
            // Default parameter values
            ushort width = 32, height = 32, cellSize = 20;

            // Maps command-line flags to actions that set config values
            var options = new Dictionary<string, Action<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["-w"] = val => width = ushort.Parse(val),
                ["--width"] = val => width = ushort.Parse(val),
                ["-h"] = val => height = ushort.Parse(val),
                ["--height"] = val => height = ushort.Parse(val),
                ["-c"] = val => cellSize = ushort.Parse(val),
                ["--cellsize"] = val => cellSize = ushort.Parse(val),
            };

            // Process each argument and set corresponding config values
            for (int i = 0; i < args.Length; i++)
            {
                if (options.TryGetValue(args[i], out var setter))
                    // Validate that flag has value following it
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        try
                        {
                            setter(args[i + 1]);
                            i++;  // Skip the value as it's consumed
                        }
                        catch (FormatException)
                        {
                            Console.WriteLine($"Error: Invalid value for {args[i]}: {args[i + 1]}");
                            Environment.Exit(1);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error: Missing value for {args[i]}");
                        Environment.Exit(1);
                    }
                else
                {
                    Console.WriteLine($"Error: Unknown argument: {args[i]}");
                    Environment.Exit(1);
                }
            }

            // Ensure width and height are odd numbers (required for maze structure)
            if (width % 2 == 0) width++;
            if (height % 2 == 0) height++;

            return new MazeConfig(width, height, cellSize);
        }
    }
}