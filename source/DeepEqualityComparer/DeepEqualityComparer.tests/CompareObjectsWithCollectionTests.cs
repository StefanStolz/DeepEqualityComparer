using System.Collections.Generic;

using NUnit.Framework;

namespace deepequalitycomparer.tests
{
    [TestFixture]
    public sealed class CompareObjectsWithCollectionTests
    {
        public class SomeTypeWithCollections
        {
            public List<ComplexType> ComplexItems { get; } = new List<ComplexType>();
        }

        public class ComplexType
        {
            public string Text { get; set; }
            public int Value { get; set; }
        }

        [Test]
        public void CompareSomeTypeWithCollectionsInstances()
        {
            var instance1 = new SomeTypeWithCollections
            {
                ComplexItems =
                {
                    new ComplexType { Text = "a", Value = 1 },
                    new ComplexType { Text = "b", Value = 2 },
                }
            };

            var instance2 = new SomeTypeWithCollections
            {
                ComplexItems =
                {
                    new ComplexType { Text = "a", Value = 1 },
                    new ComplexType { Text = "c", Value = 2 },
                }
            };

            var sut = DeepEqualityComparer.CreateConfiguration().SetIgnoreIndexer(true).CreateEqualityComparer();

            var result = sut.Equals(instance1, instance2);

            Assert.That(result, Is.False);
        }
    }
}
