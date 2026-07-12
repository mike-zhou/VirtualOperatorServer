using PointModel = VirtualOperatorServer.Coordinates.Point;

namespace VirtualOperatorServer.Configuration;

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
