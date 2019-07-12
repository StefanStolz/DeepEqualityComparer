#region File Header

// MIT License
// 
// Copyright (c) 2019 Stefan Stolz
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

using System.IO;

using NUnit.Framework;

#endregion

namespace deepequalitycomparer.tests
{
    [TestFixture]
    public sealed class CompareWithLoggingTests
    {
        [Test]
        public void LogNotEqualComplexType()
        {
            var instance1 = new SomeType { Text = "T", Number = 12 };
            var instance2 = new SomeType { Text = "X", Number = 12 };

            var textWriter = new StringWriter();

            Assert.That(instance1, Is.Not.EqualTo(instance2).Using(new DeepEqualityComparer(textWriter)));

            var result = textWriter.ToString();

            string expected = @"(root): not equal - x: deepequalitycomparer.tests.CompareWithLoggingTests+SomeType y: deepequalitycomparer.tests.CompareWithLoggingTests+SomeType
  Number: equal - x: 12 y: 12
  Text: not equal - x: X y: T
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

        private class SomeType
        {
            public int Number { get; set; }
            public string Text { get; set; }
        }
    }
}
