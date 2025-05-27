namespace MazeVisualiser.Utils
{
    public static class Directions
    {
        private static readonly Random _random = new();

        public static readonly (sbyte dx, sbyte dy)[] Cardinal =
        {
            (0, -1),  // UP
            (0, 1),  // DOWN
            (-1, 0),  // LEFT
            (1, 0)  // RIGHT
        };

        // Shuffle directions, randomising their order
        public static List<(sbyte, sbyte)> ShuffleDirections(IEnumerable<(sbyte, sbyte)> directions)
        {
            var list = new List<(sbyte, sbyte)>(directions);
            // Fisher-Yates shuffle
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = _random.Next(i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
            return list;
        }

        // Generate valid neighbours positions in the given directions
        public static IEnumerable<(ushort X, ushort Y)> GetNeighbours(IEnumerable<(sbyte, sbyte)> directions, (ushort X, ushort Y) pos, ushort width, ushort height)
        {
            foreach (var (dx, dy) in directions)
            {
                var nx = (ushort)(pos.X + dx);
                var ny = (ushort)(pos.Y + dy);

                if (nx < width && ny < height)
                    yield return (nx, ny);
            }
        }
    }
}