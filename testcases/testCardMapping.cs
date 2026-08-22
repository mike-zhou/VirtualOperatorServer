using VirtualOperatorServer.CardMappings;
using Xunit;

namespace VirtualOperatorServer.Tests
{
    public sealed class TestCardMapping
    {
        [Fact]
        public void AddMappingRejectsDuplicateCardNamesIgnoringCase()
        {
            WithStore(store =>
            {
                store.AddMapping(new AddCardMappingRequest("Visa", 0, 1));

                Assert.Throws<DuplicateCardNameException>(() =>
                    store.AddMapping(new AddCardMappingRequest("visa", 1, 2)));
            });
        }

        [Fact]
        public void DifferentCardNamesCanShareSlotAndMappingsPersist()
        {
            WithStore((store, path) =>
            {
                store.AddMapping(new AddCardMappingRequest("Visa", 1, 4));
                store.AddMapping(new AddCardMappingRequest("Mastercard", 1, 4));

                var restored = new CardMappingStore(path).GetMappings();

                Assert.Equal(2, restored.Count);
                Assert.Equal(new CardSlot(1, 4), restored["Visa"]);
                Assert.Equal(new CardSlot(1, 4), restored["Mastercard"]);
            });
        }

        [Fact]
        public void DeleteMappingRemovesMappingIgnoringCaseAndPersists()
        {
            WithStore((store, path) =>
            {
                store.AddMapping(new AddCardMappingRequest("Visa", 1, 4));

                Assert.True(store.DeleteMapping("visa"));
                Assert.False(store.DeleteMapping("Visa"));
                Assert.Empty(new CardMappingStore(path).GetMappings());
            });
        }

        private static void WithStore(Action<CardMappingStore> test)
        {
            WithStore((store, _) => test(store));
        }

        private static void WithStore(Action<CardMappingStore, string> test)
        {
            var path = Path.Combine(Path.GetTempPath(), $"card-mapping-{Guid.NewGuid():N}.json");
            try
            {
                test(new CardMappingStore(path), path);
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}
