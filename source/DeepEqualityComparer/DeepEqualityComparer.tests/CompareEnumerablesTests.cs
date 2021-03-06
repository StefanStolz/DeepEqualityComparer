﻿#region File Header

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

using System.Collections.Generic;

using NUnit.Framework;

#endregion

namespace deepequalitycomparer.tests
{
    [TestFixture]
    public sealed class CompareEnumerablesTests
    {
        [Test]
        public void CompareDifferentEnumerables()
        {
            var enumerable1 = this.GetIntegerEnumeration1();
            var enumerable2 = this.GetIntegerEnumeration3();

            Assert.That(DeepEqualityComparer.AreEqual(enumerable1, enumerable2), Is.False);
        }

        [Test]
        public void CompareEqualEnumerables()
        {
            var enumerable1 = this.GetIntegerEnumeration1();
            var enumerable2 = this.GetIntegerEnumeration2();

            Assert.That(DeepEqualityComparer.AreEqual(enumerable1, enumerable2), Is.True);
        }

        private IEnumerable<int> GetIntegerEnumeration1()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        private IEnumerable<int> GetIntegerEnumeration2()
        {
            yield return 1;
            yield return 2;
            yield return 3;
        }

        private IEnumerable<int> GetIntegerEnumeration3()
        {
            yield return 1;
            yield return 4;
            yield return 6;
        }
    }
}
