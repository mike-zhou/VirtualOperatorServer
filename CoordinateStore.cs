using CoordinatesModel = VirtualOperatorServer.Coordinates.Coordinates;
using PointModel = VirtualOperatorServer.Coordinates.Point;

namespace VirtualOperatorServer.Configuration;

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
