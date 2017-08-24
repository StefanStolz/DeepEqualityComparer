using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

using deepequalitycomparer;

using NUnit.Framework;

// ReSharper disable ConsiderUsingAsyncSuffix

namespace UsageOfNugetPackage
{
    [TestFixture]
    public sealed class CompareImmutableArrays
    {
        [Test]
        public void EmtpyArraysWithBytesAreEqual()
        {
            var c1 = ImmutableArray<byte>.Empty;
            var c2 = ImmutableArray<byte>.Empty;

            var result = DeepEqualityComparer.Default.Equals(c1, c2);

            Assert.That(result, Is.True);
        }

        [Test]
        public void ComapreByteArraysWithEqualContent()
        {
            var source = new byte[] { 1, 2, 3, 4, 5 };

            var c1 = source.ToImmutableArray();
            var c2 = source.ToImmutableArray();


            var result = DeepEqualityComparer.Default.Equals(c1, c2);

            Assert.That(result, Is.True);
        }
    }
}

