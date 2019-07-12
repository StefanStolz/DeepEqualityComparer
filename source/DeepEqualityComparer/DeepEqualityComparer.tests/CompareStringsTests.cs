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
using System.Text;

using NUnit.Framework;

#endregion

namespace deepequalitycomparer.tests
{
    [TestFixture]
    public sealed class CompareStringsTests
    {
        [Test]
        public void CompareDifferentStrings()
        {
            var text1 = new StringBuilder().Append("abcd").ToString();
            var text2 = new StringBuilder().Append("abce").ToString();

            Assert.That(DeepEqualityComparer.AreEqual(text1, text2), Is.False);
        }

        [Test]
        public void CompareEqualStrings()
        {
            var text1 = new StringBuilder().Append("abcd").ToString();
            var text2 = new StringBuilder().Append("abcd").ToString();

            Assert.That(DeepEqualityComparer.AreEqual(text1, text2), Is.True);
        }

        [Test]
        public void CompareStringWithDifferentCasingAndConfigureStringComparison()
        {
            var text1 = "abcd";
            var text2 = "ABCD";

            var comparer =
                DeepEqualityComparer.CreateConfiguration()
                                    .ConfigureStringComparison(StringComparison.OrdinalIgnoreCase)
                                    .CreateEqualityComparer();

            Assert.That(comparer.Equals(text1, text2), Is.True);
        }

        [Test]
        public void TreatNullAsEmptyString()
        {
            var comparer = DeepEqualityComparer
                           .CreateConfiguration()
                           .TreatNullAsEmptyString(true)
                           .CreateEqualityComparer();

            Assert.That(comparer.Equals(null, string.Empty), Is.True);
            Assert.That(comparer.Equals(string.Empty, null), Is.True);
        }
    }
}
