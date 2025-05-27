namespace MazeVisualiser
{
    // Store maze configuration parameters
    internal record MazeConfig(ushort Width, ushort Height, ushort CellSize);

    internal static class ArgsParser
    {
        // Parses command-line arguments into MazeConfig with validation and error logging
        public static MazeConfig Parse(string[] args)
        {
            // Check for help flag
            if (args.Contains("--help"))
            {
                PrintHelp();
                Environment.Exit(0);
            }

            // Default parameter values
            ushort width = 33, height = 33, cellSize = 20;

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

        private static void PrintHelp()
        {
            Console.WriteLine("MazeVisualiser");
            Console.WriteLine();
            Console.WriteLine("  Options:");
            Console.WriteLine("    -w, --width <integer>       Set maze width (default 33)");
            Console.WriteLine("    -h, --height <integer>      Set maze height (default 33)");
            Console.WriteLine("    -c, --cellsize <integer>    Set cell size in pixels (default 20)");
            Console.WriteLine("    --help                      Show this help message");
            Console.WriteLine();
            Console.WriteLine("  Controls:");
            Console.WriteLine("    SPACE  Pause visualisation");
            Console.WriteLine("    R      Restart generation");
            Console.WriteLine("    UP     Increase visualisation speed");
            Console.WriteLine("    DOWN   Decrease visualisation speed");
            Console.WriteLine("    LMB    Place solver start point");
            Console.WriteLine("    RMB    Place solver end point");
        }
    }
}