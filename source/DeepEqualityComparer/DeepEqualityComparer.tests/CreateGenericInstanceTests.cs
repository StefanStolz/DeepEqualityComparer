using System.Threading.Tasks;

using NUnit.Framework;

// ReSharper disable ConsiderUsingAsyncSuffix

namespace deepequalitycomparer.tests
{
    [TestFixture]
    public sealed class CreateGenericInstanceTests
    {
        [Test]
        public void DefaultConfiguration()
        {
            var sut = DeepEqualityComparer.CreateDefault<string>();

            var result = sut.Equals("a", "b");

            Assert.That(result, Is.False);
        }

        [Test]
        public void DefaultWithLogging()
        {
            
        }
    }
}

