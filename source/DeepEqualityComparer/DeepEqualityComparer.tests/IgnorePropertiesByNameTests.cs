using NUnit.Framework;

namespace deepequalitycomparer.tests
{
    [TestFixture]
    public sealed class IgnorePropertiesByNameTests
    {
        private class SomeType
        {
            public string Text { get; set; }
            public int Number { get; set; }
        }

        [Test]
        public void CompareEqualInstancesOfTypesWithoutEquals()
        {
            var instance1 = new SomeType { Text = "T", Number = 12 };
            var instance2 = new SomeType { Text = "X", Number = 12 };

            var sut = DeepEqualityComparer
                .CreateConfiguration()
                .IgnorePropertyByName("Text")
                .CreateEqualityComparer();

            Assert.That(instance1, Is.EqualTo(instance2).Using(sut));
        }
    }
}
