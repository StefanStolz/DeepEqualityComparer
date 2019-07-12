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

using System.Collections.Generic;

using NUnit.Framework;

#endregion

namespace deepequalitycomparer.tests
{
    [TestFixture]
    public sealed class CompareObjectsWithCollectionTests
    {
        [Test]
        public void CompareSomeTypeWithCollectionsInstances()
        {
            var instance1 = new SomeTypeWithCollections {
                ComplexItems = {
                    new ComplexType { Text = "a", Value = 1 },
                    new ComplexType { Text = "b", Value = 2 },
                }
            };

            var instance2 = new SomeTypeWithCollections {
                ComplexItems = {
                    new ComplexType { Text = "a", Value = 1 },
                    new ComplexType { Text = "c", Value = 2 },
                }
            };

            var sut = DeepEqualityComparer.CreateConfiguration().SetIgnoreIndexer(true).CreateEqualityComparer();

            var result = sut.Equals(instance1, instance2);

            Assert.That(result, Is.False);
        }

        public class ComplexType
        {
            public string Text { get; set; }
            public int Value { get; set; }
        }

        public class SomeTypeWithCollections
        {
            public List<ComplexType> ComplexItems { get; } = new List<ComplexType>();
        }
    }
}
