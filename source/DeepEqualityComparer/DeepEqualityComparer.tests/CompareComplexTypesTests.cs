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
    public sealed class CompareComplexTypesTests
    {
        [Test]
        public void CompareDifferentInstancesOfTypesWithoutEquals()
        {
            var instance1 = new SomeType { Text = "T", Number = 12 };
            var instance2 = new SomeType { Text = "X", Number = 12 };

            Assert.That(DeepEqualityComparer.AreEqual(instance1, instance2), Is.False);
        }

        [Test]
        public void CompareEqualInstancesOfTypesWithoutEquals()
        {
            var instance1 = new SomeType { Text = new string(new[] { 'T' }), Number = 12 };
            var instance2 = new SomeType { Text = new string(new[] { 'T' }), Number = 12 };

            Assert.That(DeepEqualityComparer.AreEqual(instance1, instance2), Is.True);
        }

        private class SomeType
        {
            public int Number { get; set; }
            public string Text { get; set; }
        }
    }
}
