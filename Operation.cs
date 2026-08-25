
namespace VirtualOperatorServer.Operation
{
    enum Action
    {
        Idle,
        Initializing,
        Swiping,
        Inserting,
        Extracting,
        Tapping,
        Barcoding,
        Touching
    }

    enum Position
    {
        Unknown,
        Idle,
        CardBay,
        Swipe,
        InsertStart,
        InsertEnd,
        Tap,
        BarCode,
        Touch
    }

    class Operations
    {
        private readonly object _dataLock = new();
        private Action _action = Action.Idle;
        private Position _position = Position.Unknown;

        public Position PositionState
        {
            get
            {
                lock (_dataLock)
                {
                    return _position;
                }
            }
        }
        
        public Action ActionState
        {
            get
            {
                lock (_dataLock)
                {
                    return _action;
                }
            }
        }

        public bool StartInitializingMachine()
        {
            lock (_dataLock)
            {
                if (_action != Action.Idle)
                    return false;
                if (_position != Position.Unknown)
                    return false;

                _action = Action.Initializing;
            }

            _ = Task.Run(InitializeMachine);
            return true;
        }

        private async Task InitializeMachine()
        {
            await Task.CompletedTask;
        }

        public bool StartSwipingCard(string cardName, uint opt)
        {
            lock (_dataLock)
            {
                if (_action != Action.Idle)
                    return false;
                if (_position == Position.Unknown)
                    return false;

                _action = Action.Swiping;
            }

            _ = Task.Run(() => Swipe(cardName, opt));
            return true;
        }

        private async Task Swipe(string cardName, uint opt)
        {
            await Task.CompletedTask;
        }

        public bool StartInsertingCard(string cardName, uint opt)
        {
            lock (_dataLock)
            {
                if (_action != Action.Idle)
                    return false;
                if (_position == Position.Unknown)
                    return false;

                _action = Action.Inserting;
            }

            _ = Task.Run(() => Insert(cardName, opt));
            return true;
        }

        private async Task Insert(string cardName, uint opt)
        {
            await Task.CompletedTask;
        }

        public bool StartExtractingCard(string cardName, uint opt)
        {
            lock (_dataLock)
            {
                if (_action != Action.Idle)
                    return false;
                if (_position == Position.Unknown)
                    return false;

                _action = Action.Extracting;
            }

            _ = Task.Run(() => Extract(cardName, opt));
            return true;
        }

        private async Task Extract(string cardName, uint opt)
        {
            await Task.CompletedTask;
        }

        public bool StartTappingCard(string cardName, uint opt)
        {
            lock (_dataLock)
            {
                if (_action != Action.Idle)
                    return false;
                if (_position == Position.Unknown)
                    return false;

                _action = Action.Tapping;
            }

            _ = Task.Run(() => Tap(cardName, opt));
            return true;
        }

        private async Task Tap(string cardName, uint opt)
        {
            await Task.CompletedTask;
        }

        public bool StartBarCoding(string cardName, uint opt)
        {
            lock (_dataLock)
            {
                if (_action != Action.Idle)
                    return false;
                if (_position == Position.Unknown)
                    return false;

                _action = Action.Barcoding;
            }

            _ = Task.Run(() => BarCode(cardName, opt));
            return true;
        }

        private async Task BarCode(string cardName, uint opt)
        {
            await Task.CompletedTask;
        }

        public bool StartTouching(uint opt, IReadOnlyList<(int x, int y)> coordinates)
        {
            lock (_dataLock)
            {
                if (_action != Action.Idle)
                    return false;
                if (_position == Position.Unknown)
                    return false;

                _action = Action.Touching;
            }

            _ = Task.Run(() => Touch(opt, coordinates));
            return true;
        }

        private async Task Touch(uint opt, IReadOnlyList<(int x, int y)> coordinates)
        {
            await Task.CompletedTask;
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
                return Results.Ok(new { success = operations.StartInitializingMachine() });
            });

            api.MapPost("/swipe", (OperationRequest request, Operations operations) =>
            {
                return Results.Ok(new { success = operations.StartSwipingCard(request.CardName, request.Opt) });
            });

            api.MapPost("/tap", (OperationRequest request, Operations operations) =>
            {
                return Results.Ok(new { success = operations.StartTappingCard(request.CardName, request.Opt) });
            });

            api.MapPost("/barcode", (OperationRequest request, Operations operations) =>
            {
                return Results.Ok(new { success = operations.StartBarCoding(request.CardName, request.Opt) });
            });

            api.MapPost("/insert", (OperationRequest request, Operations operations) =>
            {
                return Results.Ok(new { success = operations.StartInsertingCard(request.CardName, request.Opt) });
            });

            api.MapPost("/extract", (OperationRequest request, Operations operations) =>
            {
                return Results.Ok(new { success = operations.StartExtractingCard(request.CardName, request.Opt) });
            });

            return endpoints;
        }
    }
}
