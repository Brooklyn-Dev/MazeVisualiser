namespace MazeVisualiser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = ArgsParser.Parse(args);
            var app = new MazeApp(config);
            app.Run();
        }
    }
}