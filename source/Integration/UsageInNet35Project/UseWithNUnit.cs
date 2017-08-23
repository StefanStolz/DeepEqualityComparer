#region File Header

// MIT License
// 
// Copyright (c) 2017 Stefan Stolz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

#region using directives

using System;
using System.IO;

using deepequalitycomparer;

using NUnit.Framework;

#endregion

namespace UsageInNet35Project
{
    [TestFixture]
    public class UseWithNUnit
    {
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
            var so2 = new SomeObject { Number = 12, Text = "abc" };

            Assert.That(so1, Is.EqualTo(so2).Using(DeepEqualityComparer.DefaultWithConsoleOutput));
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
        public void IgnoreIndexer()
        {
            var so1 = new SomeObjectWithIndexer();
            var so2 = new SomeObjectWithIndexer();

            var comparer = DeepEqualityComparer.CreateConfiguration()
                                               .SetIgnoreIndexer(true)
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
        public void LogToTextWriter()
        {
            var so1 = new SomeObject { Number = 12, Text = "abc" };
            var so2 = new SomeObject { Number = 12, Text = "abc" };

            var textWriter = new StringWriter();

            var comparer = DeepEqualityComparer
                .CreateConfiguration()
                .SetLoggingTextWriter(textWriter)
                .CreateEqualityComparer();

            Assert.That(so1, Is.EqualTo(so2).Using(comparer));
        }

        [Test]
        public void TreatNullAsEmptyString()
        {
            var so1 = new SomeObject { Number = 12, Text = string.Empty };
            var so2 = new SomeObject { Number = 12, Text = null };

            var comparer = DeepEqualityComparer
                .CreateConfiguration()
                .TreatNullAsEmptyString(true)
                .CreateEqualityComparer();

            Assert.That(so1, Is.EqualTo(so2).Using(comparer));
        }

        private class SomeObject
        {
            public int Number { get; set; }
            public string Text { get; set; }
        }

        private class SomeObjectWithIndexer
        {
            public string this[int index] => string.Empty;
        }
    }
}
