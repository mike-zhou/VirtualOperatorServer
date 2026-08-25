
namespace VirtualOperatorServer.Operation
{
    enum Position
    {
        Idle,
        CardBay,
        Swipe,
        InsertStart,
        InsertEnd,
        Tap,
        BarCode,
        Touch
    }

    enum Action
    {
        Idle,
        Swiping,
        Inserting,
        Extracting,
        Tapping,
        Barcoding,
        Touching
    }

    class Operations
    {
        public bool InitializeMachine()
        {
            // Hardware initialization will be implemented here.
            return true;
        }

        public bool Swipe(string cardName, uint opt)
        {
            return true;
        }

        public bool Insert(string cardName, uint opt)
        {
            return true;
        }

        public bool Extract(string cardName, uint opt)
        {
            return true;
        }

        public bool Tap(string cardName, uint opt)
        {
            return true;
        }

        public bool BarCode(string cardName, uint opt)
        {
            return true;
        }

    }

    internal static class OperationsApi
    {
        private sealed record OperationRequest(string CardName, uint Opt);

        public static IEndpointRouteBuilder MapOperationsApi(this IEndpointRouteBuilder endpoints)
        {
            var api = endpoints.MapGroup("/api/operations");

            api.MapPost("/initialize", (Operations operations) =>
            {
                return Results.Ok(new { success = operations.InitializeMachine() });
            });

            api.MapPost("/swipe", (OperationRequest request, Operations operations) =>
            {
                return Results.Ok(new { success = operations.Swipe(request.CardName, request.Opt) });
            });

            api.MapPost("/tap", (OperationRequest request, Operations operations) =>
            {
                return Results.Ok(new { success = operations.Tap(request.CardName, request.Opt) });
            });

            api.MapPost("/barcode", (OperationRequest request, Operations operations) =>
            {
                return Results.Ok(new { success = operations.BarCode(request.CardName, request.Opt) });
            });

            api.MapPost("/insert", (OperationRequest request, Operations operations) =>
            {
                return Results.Ok(new { success = operations.Insert(request.CardName, request.Opt) });
            });

            api.MapPost("/extract", (OperationRequest request, Operations operations) =>
            {
                return Results.Ok(new { success = operations.Extract(request.CardName, request.Opt) });
            });

            return endpoints;
        }
    }
}
