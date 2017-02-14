using System.IO;

using NUnit.Framework;

namespace deepequalitycomparer.tests
{
    [TestFixture]
    public sealed class CompareWithLoggingTests
    {
        private class SomeType
        {
            public string Text { get; set; }
            public int Number { get; set; }
        }

        [Test]
        public void LogNotEqualComplexType()
        {
            var instance1 = new SomeType { Text = "T", Number = 12 };
            var instance2 = new SomeType { Text = "X", Number = 12 };

            var textWriter = new StringWriter();

            Assert.That(instance1, Is.Not.EqualTo(instance2).Using(new DeepEqualityComparer(textWriter)));

            var result = textWriter.ToString();

            string expected = @"(root): not equal - x: deepequalitycomparer.tests.CompareWithLoggingTests+SomeType y: deepequalitycomparer.tests.CompareWithLoggingTests+SomeType
  Text: not equal - x: X y: T
  Number: equal - x: 12 y: 12
";
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void LogOnlyNotEqualItems()
        {
            var instance1 = new SomeType { Text = "T", Number = 12 };
            var instance2 = new SomeType { Text = "X", Number = 12 };

            var textWriter = new StringWriter();

            var comparer = DeepEqualityComparer.CreateConfiguration().SetLoggingTextWriter(textWriter, true).CreateEqualityComparer();

            Assert.That(instance1, Is.Not.EqualTo(instance2).Using(comparer));

            var result = textWriter.ToString();

            string expected = @"(root): not equal - x: deepequalitycomparer.tests.CompareWithLoggingTests+SomeType y: deepequalitycomparer.tests.CompareWithLoggingTests+SomeType
  Text: not equal - x: X y: T
";
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
