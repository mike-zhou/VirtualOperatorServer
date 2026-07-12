using CoordinatesModel = VirtualOperatorServer.Coordinates.Coordinates;
using PointModel = VirtualOperatorServer.Coordinates.Point;

namespace VirtualOperatorServer.Coordinates
{
    public sealed class Point
    {
        public Point() { }

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

        public void AddPoint(Point point) => Points.Add(point);
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
        private static readonly System.Text.Json.JsonSerializerOptions FileSerializerOptions = new()
        {
            WriteIndented = true
        };

        public CardBay[] CardBays { get; set; } = { new(), new(), new() };
        public OPT[] OPTs { get; set; } =
        {
            new() { Name = "OPT 1" },
            new() { Name = "OPT 2" }
        };

        public string Serialize() => System.Text.Json.JsonSerializer.Serialize(this);

        public string SerializeIndented() => System.Text.Json.JsonSerializer.Serialize(this, FileSerializerOptions);

        public bool Deserialize(string json)
        {
            if (json == null)
                return false;

            try
            {
                var coordinates = System.Text.Json.JsonSerializer.Deserialize<Coordinates>(json);
                if (coordinates == null ||
                    coordinates.CardBays == null ||
                    coordinates.CardBays.Any(cardBay => cardBay == null || cardBay.Points == null) ||
                    coordinates.OPTs == null ||
                    coordinates.OPTs.Any(opt => opt == null))
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

        public static Coordinates LoadFromFile(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            if (!File.Exists(path))
            {
                var emptyCoordinates = new Coordinates();
                emptyCoordinates.SaveToFile(path);
                return emptyCoordinates;
            }

            var coordinates = new Coordinates();
            if (!coordinates.Deserialize(File.ReadAllText(path)))
                throw new InvalidDataException($"Unable to deserialize coordinates from '{path}'.");

            return coordinates;
        }

        public void SaveToFile(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(path, SerializeIndented());
        }
    }

    public sealed class CoordinateStore
    {
        private readonly object _lock = new();
        private readonly string _configFile;
        private CoordinatesModel _coordinates;

        public CoordinateStore(IWebHostEnvironment environment)
        {
            _configFile = Path.Join(environment.ContentRootPath, "configs", "coordinates.json");
            _coordinates = CoordinatesModel.LoadFromFile(_configFile);
        }

        public CoordinatesModel GetCoordinates()
        {
            lock (_lock)
            {
                return Clone(_coordinates);
            }
        }

        public PointModel AddCardBayPoint(int cardBayIndex, PointModel point)
        {
            var savedPoint = Clone(point);
            Mutate(coordinates => GetCardBay(coordinates, cardBayIndex).Points.Add(savedPoint));
            return Clone(savedPoint);
        }

        public PointModel UpdateCardBayPoint(int cardBayIndex, int pointIndex, PointModel point)
        {
            var savedPoint = Clone(point);
            Mutate(coordinates =>
            {
                var points = GetCardBay(coordinates, cardBayIndex).Points;
                if (pointIndex < 0 || pointIndex >= points.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(pointIndex), $"Card point index {pointIndex} does not exist.");
                }

                points[pointIndex] = savedPoint;
            });
            return Clone(savedPoint);
        }

        public PointModel UpdateOptPoint(int optIndex, string pointName, PointModel point)
        {
            var savedPoint = Clone(point);
            Mutate(coordinates =>
            {
                if (optIndex < 0 || optIndex >= coordinates.OPTs.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(optIndex), $"OPT index {optIndex} does not exist.");
                }

                var opt = coordinates.OPTs[optIndex];
                switch (pointName)
                {
                    case "msrSwipingStartPoint":
                        opt.MsrSwipingStartPoint = savedPoint;
                        break;
                    case "msrSwipingStopPoint":
                        opt.MsrSwipingStopPoint = savedPoint;
                        break;
                    case "contactInsertingStartPoint":
                        opt.ContactInsertingStartPoint = savedPoint;
                        break;
                    case "contactInsertingStopPoint":
                        opt.ContactInsertingStopPoint = savedPoint;
                        break;
                    case "contactlessTappingStartPoint":
                        opt.ContactlessTappingStartPoint = savedPoint;
                        break;
                    case "contactlessTappingStopPoint":
                        opt.ContactlessTappingStopPoint = savedPoint;
                        break;
                    case "barcodeReadingStartPoint":
                        opt.BarcodeReadingStartPoint = savedPoint;
                        break;
                    case "barcodeReadingStopPoint":
                        opt.BarcodeReadingStopPoint = savedPoint;
                        break;
                    case "screenTopLeft":
                        opt.ScreenTopLeft = savedPoint;
                        break;
                    case "screenTopRight":
                        opt.ScreenTopRight = savedPoint;
                        break;
                    case "screenBottomRight":
                        opt.ScreenBottomRight = savedPoint;
                        break;
                    case "screenBottomLeft":
                        opt.ScreenBottomLeft = savedPoint;
                        break;
                    default:
                        throw new ArgumentException($"Unknown OPT point '{pointName}'.", nameof(pointName));
                }
            });
            return Clone(savedPoint);
        }

        private void Mutate(Action<CoordinatesModel> mutation)
        {
            lock (_lock)
            {
                var updatedCoordinates = Clone(_coordinates);
                mutation(updatedCoordinates);
                updatedCoordinates.SaveToFile(_configFile);
                _coordinates = updatedCoordinates;
            }
        }

        private static VirtualOperatorServer.Coordinates.CardBay GetCardBay(CoordinatesModel coordinates, int index)
        {
            if (index < 0 || index >= coordinates.CardBays.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), $"Card bay index {index} does not exist.");
            }

            return coordinates.CardBays[index];
        }

        private static CoordinatesModel Clone(CoordinatesModel coordinates)
        {
            var clone = new CoordinatesModel();
            if (!clone.Deserialize(coordinates.Serialize()))
            {
                throw new InvalidOperationException("Unable to copy coordinate configuration.");
            }

            return clone;
        }

        private static PointModel Clone(PointModel point)
        {
            ArgumentNullException.ThrowIfNull(point);
            return new PointModel(point.X, point.Y, point.Z, point.U, point.V);
        }
    }

    public static class CoordinateApi
    {
        public static IEndpointRouteBuilder MapCoordinateApi(IEndpointRouteBuilder endpoints)
        {
            var api = endpoints.MapGroup("/api/coordinates");

            api.MapGet("/", (CoordinateStore store) => Results.Ok(store.GetCoordinates()));

            api.MapPost("/card-bays/{cardBayIndex:int}/points", (int cardBayIndex, PointModel point, CoordinateStore store) =>
                HandleMutation(() => store.AddCardBayPoint(cardBayIndex, point)));

            api.MapPut("/card-bays/{cardBayIndex:int}/points/{pointIndex:int}",
                (int cardBayIndex, int pointIndex, PointModel point, CoordinateStore store) =>
                    HandleMutation(() => store.UpdateCardBayPoint(cardBayIndex, pointIndex, point)));

            api.MapPut("/opts/{optIndex:int}/points/{pointName}",
                (int optIndex, string pointName, PointModel point, CoordinateStore store) =>
                    HandleMutation(() => store.UpdateOptPoint(optIndex, pointName, point)));

            return endpoints;
        }

        private static IResult HandleMutation(Func<PointModel> mutation)
        {
            try
            {
                return Results.Ok(mutation());
            }
            catch (ArgumentException exception)
            {
                return Results.BadRequest(new { error = exception.Message });
            }
            catch (IOException exception)
            {
                return Results.Problem(
                    title: "Unable to save coordinates",
                    detail: exception.Message,
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
