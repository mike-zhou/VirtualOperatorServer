
using System.Text.Json;
using VirtualOperatorServer.CommandAndReply;
using VirtualOperatorServer.Services;

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
        private static readonly HashSet<byte> _stepperPowerIndices = [6, 7, 8, 9, 10, 11];
        private readonly object _dataLock = new();
        private readonly ILogger<Operations> _logger;
        private readonly BackSocket _backSocket;
        private Action _action = Action.Idle;
        private Position _position = Position.Unknown;

        public Operations(ILogger<Operations> logger, BackSocket backSocket)
        {
            _logger = logger;
            _backSocket = backSocket;
        }

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
            Facade.StatusFacade.Facade? facade;
            lock (_dataLock)
            {
                if (_action != Action.Idle)
                    return false;
                if (_position != Position.Unknown)
                    return false;

                facade = Facade.StatusFacade.GetFacade();
                if (facade == null)
                    return false;

                _action = Action.Initializing;
            }

            _ = Task.Run(() => initializeMachine(facade));
            return true;
        }

        private async Task<bool> powerOutput(byte index, bool enableOutput)
        {
            if (index >= Facade.StatusFacade.Facade.PowerOutputCount)
                return false;

            JsonElement payload = JsonSerializer.SerializeToElement(new
            {
                powerOutput = index,
                enable = enableOutput
            });
            var command = new CmdSetPowerOutput(payload);
            command.Reply = await _backSocket.SendAndReceiveAsync(command.Command);
            return command.ParseReply().result;
        }

        private async Task<bool> bdcPowerOutput(bool enableOutput)
        {
            CmdSetGPIO.GPIO[] gpioArray =
            [
                new CmdSetGPIO.GPIO
                {
                    PortName = "PC",
                    BitIndex = 15,
                    Level = enableOutput ? (byte)1 : (byte)0
                }
            ];

            var command = new CmdSetGPIO(gpioArray);
            command.Reply = await _backSocket.SendAndReceiveAsync(command.Command);
            return command.ParseReply().result;
        }

        private async Task initializeMachine(Facade.StatusFacade.Facade facade)
        {
            var succeeded = false;
            
            try
            {
                succeeded = true;
                foreach (var index in _stepperPowerIndices)
                {
                    if (!await powerOutput(index, true))
                    {
                        succeeded = false;
                        _logger.LogError("Failed to enable power output {StepperPowerIndex}.", index);
                    }
                }
                if (!await bdcPowerOutput(true))
                {
                    succeeded = false;
                    _logger.LogError("Failed to enable BDC power output.");
                }
            }
            finally
            {
                lock (_dataLock)
                {
                    _action = Action.Idle;
                    if (succeeded)
                        _position = Position.Idle;
                }
            }
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

            _ = Task.Run(() => swipe(cardName, opt));
            return true;
        }

        private async Task swipe(string cardName, uint opt)
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

            _ = Task.Run(() => insert(cardName, opt));
            return true;
        }

        private async Task insert(string cardName, uint opt)
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

            _ = Task.Run(() => extract(cardName, opt));
            return true;
        }

        private async Task extract(string cardName, uint opt)
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

            _ = Task.Run(() => tap(cardName, opt));
            return true;
        }

        private async Task tap(string cardName, uint opt)
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

            _ = Task.Run(() => barCode(cardName, opt));
            return true;
        }

        private async Task barCode(string cardName, uint opt)
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

            _ = Task.Run(() => touch(opt, coordinates));
            return true;
        }

        private async Task touch(uint opt, IReadOnlyList<(int x, int y)> coordinates)
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
