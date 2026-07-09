namespace VirtualOperatorServer.Coordinates
{
    public sealed class Point
    {
        public Point()
        {
        }

        public Point(int x, int y, int z, int u, int v)
        {
            X = x;
            Y = y;
            Z = z;
            U = u;
            V = v;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int U { get; set; }
        public int V { get; set; }
    }

    public sealed class CardBay
    {
        public List<Point> Points { get; set; } = new();

        public void AddPoint(Point point)
        {
            Points.Add(point);
        }
    }

    public sealed class OPT
    {
        public string Name { get; set; } = string.Empty;
        public Point MsrSwipingStartPoint { get; set; } = new();
        public Point MsrSwipingStopPoint { get; set; } = new();
        public Point ContactInsertingStartPoint { get; set; } = new();
        public Point ContactInsertingStopPoint { get; set; } = new();
        public Point ContactlessTappingStartPoint { get; set; } = new();
        public Point ContactlessTappingStopPoint { get; set; } = new();
        public Point BarcodeReadingStartPoint { get; set; } = new();
        public Point BarcodeReadingStopPoint { get; set; } = new();
        public Point ScreenTopLeft { get; set; } = new();
        public Point ScreenTopRight { get; set; } = new();
        public Point ScreenBottomRight { get; set; } = new();
        public Point ScreenBottomLeft { get; set; } = new();
    }

    public sealed class Coordinates
    {
        public CardBay[] CardBays { get; set; } = { new(), new(), new() };
        public OPT[] OPTs { get; set; } = { new(), new() };

        public string Serialize()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }

        public bool Deserialize(string json)
        {
            if (json == null)
                return false;

            try
            {
                var coordinates = System.Text.Json.JsonSerializer.Deserialize<Coordinates>(json);
                if (coordinates == null)
                    return false;

                CardBays = coordinates.CardBays;
                OPTs = coordinates.OPTs;
                return true;
            }
            catch (System.Text.Json.JsonException)
            {
                return false;
            }
            catch (NotSupportedException)
            {
                return false;
            }
        }
    }
}
