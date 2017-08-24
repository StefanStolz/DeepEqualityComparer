using System.Collections.Immutable;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using NUnit.Framework;

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
            
            var result = DeepEqualityComparer.Default.Equals(collection1, collection2);

            Assert.That(result, Is.True);
        }        
    }
}

