using System;
using System.IO;

using deepequalitycomparer;

using NUnit.Framework;

namespace UsageOfNetStandardNugetPackage
{
    [TestFixture]
    public class UseWithNUnit
    {
        private class SomeObject
        {
            public string Text { get; set; }

            public int Number { get; set; }
        }

        private class SomeObjectWithIndexer
        {
            public string this[int index] => string.Empty;
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

            [Test]
            public void ConfigureStringComparison()
            {
                var text1 = "abcd";
                var text2 = "ABCD";

                var comparer =
                    DeepEqualityComparer.CreateConfiguration()
                        .ConfigureStringComparison(StringComparison.OrdinalIgnoreCase)
                        .CreateEqualityComparer();

                Assert.That(text1, Is.EqualTo(text2).Using(comparer));
            }

            [Test]
            public void TreatNullAsEmptyString()
            {
                var so1 = new SomeObject { Number = 12, Text = string.Empty };
                var so2 = new SomeObject { Number = 12, Text = null };

                var comparer = DeepEqualityComparer.CreateConfiguration().TreatNullAsEmptyString(true).CreateEqualityComparer();

                Assert.That(so1, Is.EqualTo(so2).Using(comparer));
            }

            [Test]
            public void IgnoreIndexer()
            {
                var so1 = new SomeObjectWithIndexer();
                var so2 = new SomeObjectWithIndexer();

                var comparer = DeepEqualityComparer.CreateConfiguration()
                    .SetIgnoreIndexer(true)
                    .CreateEqualityComparer();

                Assert.That(so1, Is.EqualTo(so2).Using(comparer));
            }
        }
    }
