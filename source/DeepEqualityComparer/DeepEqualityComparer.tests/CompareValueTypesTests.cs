#region File Header

// MIT License
// 
// Copyright (c) 2016 Stefan Stolz
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

using NUnit.Framework;

#endregion

namespace deepequalitycomparer.tests
{
    [TestFixture]
    public sealed class CompareValueTypesTests
    {
        [Test]
        public void CompareEnum()
        {
            Assert.That(DeepEqualityComparer.Equals(SomeEnum.Value1, SomeEnum.Value1), Is.True);
            Assert.That(DeepEqualityComparer.Equals(SomeEnum.Value1, SomeEnum.Value2), Is.False);
        }

        [Test]
        public void CompareIntegers()
        {
            Assert.That(DeepEqualityComparer.Equals(1, 1), Is.True);
            Assert.That(DeepEqualityComparer.Equals(1, 2), Is.False);
            Assert.That(DeepEqualityComparer.Equals(2, 1), Is.False);
        }

        [Test]
        public void CompareStruct()
        {
            var struct1 = new SomeStruct { Short = 234, Value = 456 };
            var struct2 = new SomeStruct { Short = 234, Value = 456 };
            var struct3 = new SomeStruct { Short = -234, Value = 1456 };

            Assert.That(DeepEqualityComparer.Equals(struct1, struct2), Is.True);
            Assert.That(DeepEqualityComparer.Equals(struct1, struct3), Is.False);
        }

        private enum SomeEnum
        {
            Value1,
            Value2
        }

        private struct SomeStruct
        {
            public int Value { get; set; }
            public short Short { get; set; }
        }
    }
}
