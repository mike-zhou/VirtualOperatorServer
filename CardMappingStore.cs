using System.Text.Json;

namespace VirtualOperatorServer.CardMappings
{
    public sealed record CardSlot(int CardBay, int CardSlotNumber);

    public sealed record AddCardMappingRequest(string CardName, int CardBay, int CardSlotNumber);

    public sealed class DuplicateCardNameException(string cardName)
        : ArgumentException($"A mapping for card name '{cardName}' already exists.", nameof(cardName));

    public sealed class CardMappingStore
    {
        private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };
        private readonly object _lock = new();
        private readonly string _configFile;
        private Dictionary<string, CardSlot> _mappings;

        public CardMappingStore(IWebHostEnvironment environment)
            : this(Path.Join(environment.ContentRootPath, "configs", "cardMapping.json"))
        {
        }

        public CardMappingStore(string configFile)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(configFile);
            _configFile = configFile;
            _mappings = Load(_configFile);
        }

        public IReadOnlyDictionary<string, CardSlot> GetMappings()
        {
            lock (_lock)
            {
                return new Dictionary<string, CardSlot>(_mappings, StringComparer.OrdinalIgnoreCase);
            }
        }

        public CardSlot AddMapping(AddCardMappingRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var cardName = request.CardName.Trim();
            if (cardName.Length == 0)
                throw new ArgumentException("Card name is required.", nameof(request));
            if (request.CardBay is < 0 or > 2)
                throw new ArgumentOutOfRangeException(nameof(request), "Card bay must be between 0 and 2.");
            if (request.CardSlotNumber is < 0 or > 11)
                throw new ArgumentOutOfRangeException(nameof(request), "Card slot number must be between 0 and 11.");

            lock (_lock)
            {
                if (_mappings.ContainsKey(cardName))
                    throw new DuplicateCardNameException(cardName);

                var updatedMappings = new Dictionary<string, CardSlot>(_mappings, StringComparer.OrdinalIgnoreCase)
                {
                    [cardName] = new CardSlot(request.CardBay, request.CardSlotNumber)
                };

                Save(_configFile, updatedMappings);
                _mappings = updatedMappings;
                return updatedMappings[cardName];
            }
        }

        public bool DeleteMapping(string cardName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cardName);

            lock (_lock)
            {
                var updatedMappings = new Dictionary<string, CardSlot>(_mappings, StringComparer.OrdinalIgnoreCase);
                if (!updatedMappings.Remove(cardName.Trim()))
                    return false;

                Save(_configFile, updatedMappings);
                _mappings = updatedMappings;
                return true;
            }
        }

        private static Dictionary<string, CardSlot> Load(string path)
        {
            if (!File.Exists(path))
            {
                var emptyMappings = new Dictionary<string, CardSlot>(StringComparer.OrdinalIgnoreCase);
                Save(path, emptyMappings);
                return emptyMappings;
            }

            var savedMappings = JsonSerializer.Deserialize<Dictionary<string, CardSlot>>(File.ReadAllText(path))
                ?? throw new InvalidDataException($"Unable to deserialize card mappings from '{path}'.");
            return new Dictionary<string, CardSlot>(savedMappings, StringComparer.OrdinalIgnoreCase);
        }

        private static void Save(string path, Dictionary<string, CardSlot> mappings)
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            File.WriteAllText(path, JsonSerializer.Serialize(mappings, SerializerOptions));
        }
    }

    public static class CardMappingApi
    {
        public static IEndpointRouteBuilder MapCardMappingApi(this IEndpointRouteBuilder endpoints)
        {
            var api = endpoints.MapGroup("/api/card-mappings");
            api.MapGet("/", (CardMappingStore store) => Results.Ok(store.GetMappings()));
            api.MapPost("/", (AddCardMappingRequest request, CardMappingStore store) =>
            {
                try
                {
                    return Results.Ok(store.AddMapping(request));
                }
                catch (DuplicateCardNameException exception)
                {
                    return Results.Conflict(new { error = exception.Message });
                }
                catch (ArgumentException exception)
                {
                    return Results.BadRequest(new { error = exception.Message });
                }
                catch (IOException exception)
                {
                    return Results.Problem(
                        title: "Unable to save card mapping",
                        detail: exception.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            });
            api.MapDelete("/{cardName}", (string cardName, CardMappingStore store) =>
            {
                try
                {
                    return store.DeleteMapping(cardName)
                        ? Results.NoContent()
                        : Results.NotFound(new { error = $"No mapping exists for card name '{cardName}'." });
                }
                catch (ArgumentException exception)
                {
                    return Results.BadRequest(new { error = exception.Message });
                }
                catch (IOException exception)
                {
                    return Results.Problem(
                        title: "Unable to delete card mapping",
                        detail: exception.Message,
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            });

            return endpoints;
        }
    }
}
