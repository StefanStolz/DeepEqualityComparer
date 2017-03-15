using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

using NUnit.Framework;

namespace deepequalitycomparer.tests
{
    [TestFixture]
    public sealed class ComparePropertiesWithTypeSpecificComparer
    {
        private class SomeClass
        {
            public string Text { get; set; }

            public int Value { get; set; }
        }

        private class AbsIntegerEqualityComparer : IEqualityComparer, IEqualityComparer<int>
        {
            public bool Equals(object x, object y)
            {
                var ix = (int)x;
                var iy = (int)y;

                return Math.Abs(ix) == Math.Abs(iy);
            }

            public int GetHashCode(object obj)
            {
                var i = (int)obj;

                return this.GetHashCode(i);
            }

            public bool Equals(int x, int y)
            {
                return Math.Abs(x) == Math.Abs(y);
            }

            public int GetHashCode(int obj)
            {
                return obj.GetHashCode();
            }
        }

        [Test]
        public void CompareWithoutTypeSpecificComparer()
        {
            var so1 = new SomeClass
            {
                Text = "Shibby",
                Value = 25,
            };

            var so2 = new SomeClass
            {
                Text = "Shibby",
                Value = -25,
            };

            var comparer = DeepEqualityComparer.CreateConfiguration().CreateEqualityComparer();

            Assert.That(so1, Is.Not.EqualTo(so2).Using(comparer));
        }

        [Test]
        public void CompareWithTypeSpecificComparer()
        {
            var so1 = new SomeClass
            {
                Text = "Shibby",
                Value = 25,
            };

            var so2 = new SomeClass
            {
                Text = "Shibby",
                Value = -25,
            };

            var comparer = DeepEqualityComparer.CreateConfiguration()
                .RegisterEqualityComparerForType(typeof(int), new AbsIntegerEqualityComparer())
                .CreateEqualityComparer();

            Assert.That(so1, Is.EqualTo(so2).Using(comparer));
        }

        [Test]
        public void CompareWithGenericTypeSpecificComparer()
        {
            var so1 = new SomeClass
            {
                Text = "Shibby",
                Value = 25,
            };

            var so2 = new SomeClass
            {
                Text = "Shibby",
                Value = -25,
            };

            var comparer = DeepEqualityComparer.CreateConfiguration()
                .RegisterEqualityComparerForType(new AbsIntegerEqualityComparer())
                .CreateEqualityComparer();

            Assert.That(so1, Is.EqualTo(so2).Using(comparer));
        }
    }
}
