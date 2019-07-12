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
            Assert.That(DeepEqualityComparer.AreEqual(SomeEnum.Value1, SomeEnum.Value1), Is.True);
            Assert.That(DeepEqualityComparer.AreEqual(SomeEnum.Value1, SomeEnum.Value2), Is.False);
        }

        [Test]
        public void CompareIntegers()
        {
            Assert.That(DeepEqualityComparer.AreEqual(1, 1), Is.True);
            Assert.That(DeepEqualityComparer.AreEqual(1, 2), Is.False);
            Assert.That(DeepEqualityComparer.AreEqual(2, 1), Is.False);
        }

        [Test]
        public void CompareStruct()
        {
            var struct1 = new SomeStruct { Short = 234, Value = 456 };
            var struct2 = new SomeStruct { Short = 234, Value = 456 };
            var struct3 = new SomeStruct { Short = -234, Value = 1456 };

            Assert.That(DeepEqualityComparer.AreEqual(struct1, struct2), Is.True);
            Assert.That(DeepEqualityComparer.AreEqual(struct1, struct3), Is.False);
        }

        [Test]
        public void CompareStructWithEqulals()
        {
            var s1 = new StructWithEquals(new Guid("1C569499-61CA-444D-BC2C-55D44A7CAED4"));
            var s2 = new StructWithEquals(new Guid("1C569499-61CA-444D-BC2C-55D44A7CAED4"));
            var s3 = new StructWithEquals(new Guid("9BBADE46-8824-4927-81DD-2987AA035515"));

            Assert.That(DeepEqualityComparer.Default.Equals(s1, s2), Is.True);
            Assert.That(DeepEqualityComparer.Default.Equals(s1, s3), Is.False);
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

        public struct StructWithEquals : IEquatable<StructWithEquals>
        {
            public Guid Id { get; }

            public StructWithEquals(Guid id)
            {
                this.Id = id;
            }

            public static StructWithEquals CreateNew()
            {
                return new StructWithEquals(Guid.NewGuid());
            }

            public bool Equals(StructWithEquals other)
            {
                return this.Id.Equals(other.Id);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is StructWithEquals && this.Equals((StructWithEquals)obj);
            }

            public static bool operator !=(StructWithEquals a, StructWithEquals b)
            {
                return !(a == b);
            }

            public static bool operator ==(StructWithEquals a, StructWithEquals b)
            {
                return a.Equals(b);
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            public override string ToString()
            {
                return this.Id.ToString();
            }
        }
    }
}
