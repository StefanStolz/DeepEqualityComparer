using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using NUnit.Framework;


// ReSharper disable ConsiderUsingAsyncSuffix

namespace deepequalitycomparer.tests
{
    [TestFixture]
    public sealed class Issues_Github_No5
    {
        public class Foo : IEnumerable {
            private readonly List<string> data = new List<string>();
            public int Bar { get; }

            public Foo(int bar) {
                Bar = bar;
            }

            public void Add(string d) => data.Add(d);

            IEnumerator IEnumerable.GetEnumerator() => data.GetEnumerator();
        }


        [Test]
        public void Test() {
            var comparer = DeepEqualityComparer.CreateConfiguration()
                                               .SetLoggingTextWriter(Console.Out, false)
                                               .CreateEqualityComparer();

            var v1 = new Foo(1) { "x" };
            var v2 = new Foo(1) { "x" };
            var v3 = new Foo(1) { "x", "y" };
            var v4 = new Foo(1) { "y" };
            var v5 = new Foo(2) { "x" };

            Assert.IsTrue(comparer.Equals(v1, v2));
            Assert.IsFalse(comparer.Equals(v1, v3));
            Assert.IsFalse(comparer.Equals(v1, v4));
            Assert.IsFalse(comparer.Equals(v1, v4));
            Assert.IsFalse(comparer.Equals(v1, v5)); // fails
        }
    }
}

