using System.Collections;
using System.IO;

using deepequalitycomparer;

using NUnit.Framework;

namespace UsageOfNugetPackage
{
    [TestFixture]
    public class UseWithNUnit
    {
        private class SomeObject
        {
            public string Text { get; set; }

            public int Number { get; set; }
        }

        [Test]
        public void CompareSomeObjects()
        {
            var so1 = new SomeObject { Number = 12, Text = "abc" };
            var so2 = new SomeObject { Number = 12, Text = "abc" };
            
            Assert.That(so1, Is.EqualTo(so2).Using(DeepEqualityComparer.Default));
        }

        [Test]
        public void CompareWithLoggingToConsole()
        {
            var so1 = new SomeObject { Number = 12, Text = "abc" };
            var so2 = new SomeObject { Number = 12, Text = "abcd" };

            Assert.That(so1, Is.EqualTo(so2).Using(DeepEqualityComparer.DefaultWithConsoleOutput));
        }


        [Test]
        public void LogToTextWriter()
        {
            var so1 = new SomeObject { Number = 12, Text = "abc" };
            var so2 = new SomeObject { Number = 12, Text = "abcd" };

            var textWriter = new StringWriter();

            var comparer = DeepEqualityComparer
                .CreateConfiguration()
                .SetLoggingTextWriter(textWriter)
                .CreateEqualityComparer();

            Assert.That(so1, Is.EqualTo(so2).Using(comparer));
        }

        [Test]
        public void IgnorePropertiesByName()
        {
            var so1 = new SomeObject { Number = 12, Text = "abc" };
            var so2 = new SomeObject { Number = 12, Text = "abcd" };

            var comparer = DeepEqualityComparer
                .CreateConfiguration()
                .IgnorePropertyByName("Text")
                .CreateEqualityComparer();

            Assert.That(so1, Is.EqualTo(so2).Using(comparer));
        }
    }
}