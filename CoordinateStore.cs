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
        // The card bay has many slots and each slot can contain a card. 
        // the key is actually the slot index.
        public Dictionary<uint, Point> Cards { get; set; } = new();

        public void AddCard(Point point) => Cards.Add((uint)Cards.Count, point);
    }

    public sealed class OPT
    {
        public string Name { get; set; } = string.Empty;
        public int SwipeDeltaX { get; set; }
        public int SwipeDeltaY { get; set; }
        public int ContactInsertDistance { get; set; }
        public int ContactPushDistance { get; set; }
        public Point? MsrSwipingStartPoint { get; set; }
        public Point? ContactInsertingStartPoint { get; set; }
        public Point? ContactInsertingStopPoint { get; set; }
        public Point? ContactlessTappingStartPoint { get; set; }
        public Point? ContactlessTappingStopPoint { get; set; }
        public Point? BarcodeReadingStartPoint { get; set; }
        public Point? BarcodeReadingStopPoint { get; set; }
        public Point? ScreenTopLeft { get; set; }
        public Point? ScreenTopRight { get; set; }
        public Point? ScreenBottomRight { get; set; }
        public Point? ScreenBottomLeft { get; set; }
    }

    public sealed class Coordinates
    {
        private static readonly System.Text.Json.JsonSerializerOptions FileSerializerOptions = new()
        {
            WriteIndented = true
        };

        public Point Idle { get; set; } = new();
        public int ReplaceDistance { get; set; }
        public int ReplaceDistanceSlow { get; set; }
        public int FetchDistance { get; set; }
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
                    coordinates.Idle == null ||
                    coordinates.CardBays == null ||
                    coordinates.CardBays.Any(cardBay => cardBay == null || cardBay.Cards == null) ||
                    coordinates.OPTs == null ||
                    coordinates.OPTs.Any(opt => opt == null))
                    return false;

                Idle = coordinates.Idle;
                ReplaceDistance = coordinates.ReplaceDistance;
                ReplaceDistanceSlow = coordinates.ReplaceDistanceSlow;
                FetchDistance = coordinates.FetchDistance;
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

    public sealed record DistanceSettings(int ReplaceDistance, int ReplaceDistanceSlow, int FetchDistance);

    public sealed record OptSettings(
        int SwipeDeltaX,
        int SwipeDeltaY,
        int ContactInsertDistance,
        int ContactPushDistance);

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

        public PointModel UpdateIdlePoint(PointModel point)
        {
            var savedPoint = Clone(point);
            Mutate(coordinates => coordinates.Idle = savedPoint);
            return Clone(savedPoint);
        }

        public DistanceSettings UpdateDistances(DistanceSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);
            Mutate(coordinates =>
            {
                coordinates.ReplaceDistance = settings.ReplaceDistance;
                coordinates.ReplaceDistanceSlow = settings.ReplaceDistanceSlow;
                coordinates.FetchDistance = settings.FetchDistance;
            });
            return settings;
        }

        public PointModel AddCardBayPoint(int cardBayIndex, PointModel point)
        {
            var savedPoint = Clone(point);
            Mutate(coordinates => GetCardBay(coordinates, cardBayIndex).AddCard(savedPoint));
            return Clone(savedPoint);
        }

        public PointModel UpdateCardBayPoint(int cardBayIndex, int pointIndex, PointModel point)
        {
            var savedPoint = Clone(point);
            Mutate(coordinates =>
            {
                var points = GetCardBay(coordinates, cardBayIndex).Cards;
                if (pointIndex < 0 || !points.ContainsKey((uint)pointIndex))
                {
                    throw new ArgumentOutOfRangeException(nameof(pointIndex), $"Card point index {pointIndex} does not exist.");
                }

                points[(uint)pointIndex] = savedPoint;
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

        public OptSettings UpdateOptSettings(int optIndex, OptSettings settings)
        {
            ArgumentNullException.ThrowIfNull(settings);
            Mutate(coordinates =>
            {
                if (optIndex < 0 || optIndex >= coordinates.OPTs.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(optIndex), $"OPT index {optIndex} does not exist.");
                }

                var opt = coordinates.OPTs[optIndex];
                opt.SwipeDeltaX = settings.SwipeDeltaX;
                opt.SwipeDeltaY = settings.SwipeDeltaY;
                opt.ContactInsertDistance = settings.ContactInsertDistance;
                opt.ContactPushDistance = settings.ContactPushDistance;
            });
            return settings;
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
        public static IEndpointRouteBuilder MapCoordinateApi(this IEndpointRouteBuilder endpoints)
        {
            var api = endpoints.MapGroup("/api/coordinates");

            api.MapGet("/", (CoordinateStore store) => Results.Ok(store.GetCoordinates()));

            api.MapPut("/idle", (PointModel point, CoordinateStore store) =>
                HandleMutation(() => store.UpdateIdlePoint(point)));

            api.MapPut("/distances", (DistanceSettings settings, CoordinateStore store) =>
                HandleMutation(() => store.UpdateDistances(settings)));

            api.MapPost("/card-bays/{cardBayIndex:int}/points", (int cardBayIndex, PointModel point, CoordinateStore store) =>
                HandleMutation(() => store.AddCardBayPoint(cardBayIndex, point)));

            api.MapPut("/card-bays/{cardBayIndex:int}/points/{pointIndex:int}",
                (int cardBayIndex, int pointIndex, PointModel point, CoordinateStore store) =>
                    HandleMutation(() => store.UpdateCardBayPoint(cardBayIndex, pointIndex, point)));

            api.MapPut("/opts/{optIndex:int}/points/{pointName}",
                (int optIndex, string pointName, PointModel point, CoordinateStore store) =>
                    HandleMutation(() => store.UpdateOptPoint(optIndex, pointName, point)));

            api.MapPut("/opts/{optIndex:int}/settings",
                (int optIndex, OptSettings settings, CoordinateStore store) =>
                    HandleMutation(() => store.UpdateOptSettings(optIndex, settings)));

            return endpoints;
        }

        private static IResult HandleMutation<T>(Func<T> mutation)
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
