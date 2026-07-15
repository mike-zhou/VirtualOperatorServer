using CardBayModel = VirtualOperatorServer.Coordinates.CardBay;
using CoordinatesModel = VirtualOperatorServer.Coordinates.Coordinates;
using OptModel = VirtualOperatorServer.Coordinates.OPT;
using PointModel = VirtualOperatorServer.Coordinates.Point;
using Xunit;

namespace VirtualOperatorServer.Tests
{
    public sealed class TestCoordinates
    {
        [Fact]
        public void SerializeReturnsJsonWithAllCoordinateData()
        {
            var coordinates = CreateSampleCoordinates();
            var json = coordinates.Serialize();
            var restored = System.Text.Json.JsonSerializer.Deserialize<CoordinatesModel>(json);

            Assert.NotNull(restored);
            AssertCoordinatesEqual(coordinates, restored!, "Serialized coordinates");
        }

        [Fact]
        public void DeserializeRestoresSerializedCoordinateData()
        {
            var source = CreateSampleCoordinates();
            var target = new CoordinatesModel();

            var success = target.Deserialize(source.Serialize());

            Assert.True(success);
            AssertCoordinatesEqual(source, target, "Deserialized coordinates");
        }

        [Fact]
        public void DeserializeReturnsFalseOnInvalidJsonAndKeepsExistingData()
        {
            var coordinates = CreateSampleCoordinates();
            var before = coordinates.Serialize();

            var success = coordinates.Deserialize("{invalid json");

            Assert.False(success);
            Assert.Equal(before, coordinates.Serialize());
        }

        [Fact]
        public void SaveToFileAndLoadFromFileRoundTripCoordinateData()
        {
            var path = Path.Combine(Path.GetTempPath(), $"coordinates-{Guid.NewGuid():N}.json");
            try
            {
                var source = CreateSampleCoordinates();

                source.SaveToFile(path);
                var restored = CoordinatesModel.LoadFromFile(path);

                AssertCoordinatesEqual(source, restored, "File coordinates");
            }
            finally
            {
                File.Delete(path);
            }
        }

        private static CoordinatesModel CreateSampleCoordinates()
        {
            var coordinates = new CoordinatesModel
            {
                Idle = CreatePoint(41)
            };

            coordinates.CardBays[0].AddCard(CreatePoint(1));
            coordinates.CardBays[0].AddCard(CreatePoint(11));
            coordinates.CardBays[1].AddCard(CreatePoint(21));
            coordinates.CardBays[2].AddCard(CreatePoint(31));

            coordinates.OPTs[0] = CreateOpt("OPT 1", 100);
            coordinates.OPTs[1] = CreateOpt("OPT 2", 200);

            return coordinates;
        }

        private static OptModel CreateOpt(string name, int offset)
        {
            return new OptModel
            {
                Name = name,
                MsrSwipingStartPoint = CreatePoint(offset + 1),
                MsrSwipingStopPoint = CreatePoint(offset + 11),
                ContactInsertingStartPoint = CreatePoint(offset + 21),
                ContactInsertingStopPoint = CreatePoint(offset + 31),
                ContactlessTappingStartPoint = CreatePoint(offset + 41),
                ContactlessTappingStopPoint = CreatePoint(offset + 51),
                BarcodeReadingStartPoint = CreatePoint(offset + 61),
                BarcodeReadingStopPoint = CreatePoint(offset + 71),
                ScreenTopLeft = CreatePoint(offset + 81),
                ScreenTopRight = CreatePoint(offset + 91),
                ScreenBottomRight = CreatePoint(offset + 101),
                ScreenBottomLeft = CreatePoint(offset + 111)
            };
        }

        private static PointModel CreatePoint(int offset)
        {
            return new PointModel(offset, offset + 1, offset + 2, offset + 3, offset + 4);
        }

        private static void AssertCoordinatesEqual(CoordinatesModel expected, CoordinatesModel actual, string path)
        {
            AssertPointEqual(expected.Idle, actual.Idle, $"{path}.Idle");

            Assert.Equal(expected.CardBays.Length, actual.CardBays.Length);
            for (var i = 0; i < expected.CardBays.Length; i++)
            {
                AssertCardBayEqual(expected.CardBays[i], actual.CardBays[i], $"{path}.CardBays[{i}]");
            }

            Assert.Equal(expected.OPTs.Length, actual.OPTs.Length);
            for (var i = 0; i < expected.OPTs.Length; i++)
            {
                AssertOptEqual(expected.OPTs[i], actual.OPTs[i], $"{path}.OPTs[{i}]");
            }
        }

        private static void AssertCardBayEqual(CardBayModel expected, CardBayModel actual, string path)
        {
            Assert.Equal(expected.Cards.Count, actual.Cards.Count);
            for (var i = 0; i < expected.Cards.Count; i++)
            {
                AssertPointEqual(expected.Cards[(uint)i], actual.Cards[(uint)i], $"{path}.Cards[{i}]");
            }
        }

        private static void AssertOptEqual(OptModel expected, OptModel actual, string path)
        {
            Assert.Equal(expected.Name, actual.Name);
            AssertPointEqual(expected.MsrSwipingStartPoint, actual.MsrSwipingStartPoint, $"{path}.MsrSwipingStartPoint");
            AssertPointEqual(expected.MsrSwipingStopPoint, actual.MsrSwipingStopPoint, $"{path}.MsrSwipingStopPoint");
            AssertPointEqual(expected.ContactInsertingStartPoint, actual.ContactInsertingStartPoint, $"{path}.ContactInsertingStartPoint");
            AssertPointEqual(expected.ContactInsertingStopPoint, actual.ContactInsertingStopPoint, $"{path}.ContactInsertingStopPoint");
            AssertPointEqual(expected.ContactlessTappingStartPoint, actual.ContactlessTappingStartPoint, $"{path}.ContactlessTappingStartPoint");
            AssertPointEqual(expected.ContactlessTappingStopPoint, actual.ContactlessTappingStopPoint, $"{path}.ContactlessTappingStopPoint");
            AssertPointEqual(expected.BarcodeReadingStartPoint, actual.BarcodeReadingStartPoint, $"{path}.BarcodeReadingStartPoint");
            AssertPointEqual(expected.BarcodeReadingStopPoint, actual.BarcodeReadingStopPoint, $"{path}.BarcodeReadingStopPoint");
            AssertPointEqual(expected.ScreenTopLeft, actual.ScreenTopLeft, $"{path}.ScreenTopLeft");
            AssertPointEqual(expected.ScreenTopRight, actual.ScreenTopRight, $"{path}.ScreenTopRight");
            AssertPointEqual(expected.ScreenBottomRight, actual.ScreenBottomRight, $"{path}.ScreenBottomRight");
            AssertPointEqual(expected.ScreenBottomLeft, actual.ScreenBottomLeft, $"{path}.ScreenBottomLeft");
        }

        private static void AssertPointEqual(PointModel expected, PointModel actual, string path)
        {
            Assert.Equal(expected.X, actual.X);
            Assert.Equal(expected.Y, actual.Y);
            Assert.Equal(expected.Z, actual.Z);
            Assert.Equal(expected.U, actual.U);
            Assert.Equal(expected.V, actual.V);
        }
    }
}
