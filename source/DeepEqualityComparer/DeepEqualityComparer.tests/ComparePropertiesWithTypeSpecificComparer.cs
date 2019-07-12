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

using System;
using System.Collections;
using System.Collections.Generic;

using NUnit.Framework;

#endregion

namespace deepequalitycomparer.tests
{
    [TestFixture]
    public sealed class ComparePropertiesWithTypeSpecificComparer
    {
        [Test]
        public void CompareWithGenericTypeSpecificComparer()
        {
            var so1 = new SomeClass {
                Text = "Shibby",
                Value = 25,
            };

            var so2 = new SomeClass {
                Text = "Shibby",
                Value = -25,
            };

            var comparer = DeepEqualityComparer.CreateConfiguration()
                                               .RegisterEqualityComparerForType(new AbsIntegerEqualityComparer())
                                               .CreateEqualityComparer();

            Assert.That(so1, Is.EqualTo(so2).Using(comparer));
        }

        [Test]
        public void CompareWithoutTypeSpecificComparer()
        {
            var so1 = new SomeClass {
                Text = "Shibby",
                Value = 25,
            };

            var so2 = new SomeClass {
                Text = "Shibby",
                Value = -25,
            };

            var comparer = DeepEqualityComparer.CreateConfiguration().CreateEqualityComparer();

            Assert.That(so1, Is.Not.EqualTo(so2).Using(comparer));
        }

        [Test]
        public void CompareWithTypeSpecificComparer()
        {
            var so1 = new SomeClass {
                Text = "Shibby",
                Value = 25,
            };

            var so2 = new SomeClass {
                Text = "Shibby",
                Value = -25,
            };

            var comparer = DeepEqualityComparer.CreateConfiguration()
                                               .RegisterEqualityComparerForType(typeof(int), new AbsIntegerEqualityComparer())
                                               .CreateEqualityComparer();

            Assert.That(so1, Is.EqualTo(so2).Using(comparer));
        }

        private class AbsIntegerEqualityComparer : IEqualityComparer, IEqualityComparer<int>
        {
            public bool Equals(object x, object y)
            {
                var ix = (int)x;
                var iy = (int)y;

                return Math.Abs(ix) == Math.Abs(iy);
            }

            public bool Equals(int x, int y)
            {
                return Math.Abs(x) == Math.Abs(y);
            }

            public int GetHashCode(object obj)
            {
                var i = (int)obj;

                return this.GetHashCode(i);
            }

            public int GetHashCode(int obj)
            {
                return obj.GetHashCode();
            }
        }

        private class SomeClass
        {
            public string Text { get; set; }

            public int Value { get; set; }
        }
    }
}
