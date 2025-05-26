namespace MazeVisualiser.Utils
{
    public static class Directions
    {
        public static readonly (sbyte dx, sbyte dy)[] Cardinal =
        {
            (0, -1),  // UP
            (0, 1),  // DOWN
            (-1, 0),  // LEFT
            (1, 0)  // RIGHT
        };
    }
}