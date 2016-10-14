﻿#region File Header

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
    public sealed class CompareArraysTests
    {
        [Test]
        public void CompareArraysWithDifferentLengths()
        {
            var array1 = new[] { 1, 2, 3 };
            var array2 = new[] { 1, 2, 3, 4 };

            Assert.That(DeepEqualityComparer.Equals(array1, array2), Is.False);
            Assert.That(DeepEqualityComparer.Equals(array2, array1), Is.False);
        }

        [Test]
        public void CompareArraysWithDifferntValues()
        {
            var array1 = new[] { 1, 2, 3 };
            var array2 = new[] { 1, 2, 4 };

            Assert.That(DeepEqualityComparer.Equals(array1, array2), Is.False);
        }

        [Test]
        public void CompareEqualArrays()
        {
            var array1 = new[] { 1, 2, 3 };
            var array2 = new[] { 1, 2, 3 };

            Assert.That(DeepEqualityComparer.Equals(array1, array2), Is.True);
        }
    }
}