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

using System.Collections.Immutable;

using NUnit.Framework;

#endregion

// ReSharper disable ConsiderUsingAsyncSuffix

namespace deepequalitycomparer.tests
{
    [TestFixture]
    public sealed class CompareImmutableArrays
    {
        [Test]
        public void CompareByteArrays()
        {
            var source = new byte[] { 1, 2, 3, 4, 5 };

            var collection1 = source.ToImmutableArray();
            var collection2 = source.ToImmutableArray();

           var comparer =  DeepEqualityComparer.CreateConfiguration()
                                               .SetIgnoreIndexer(true)
                                               .CreateEqualityComparer();

            var result = comparer.Equals(collection1, collection2);

            Assert.That(result, Is.True);
        }
    }
}
