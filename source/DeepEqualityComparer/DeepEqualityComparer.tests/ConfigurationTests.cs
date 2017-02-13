using System.Collections;

using NUnit.Framework;

namespace deepequalitycomparer.tests
{
    [TestFixture]
    public sealed class ConfigurationTests
    {
        [Test]
        public void ImplicitConversionToIComparer()
        {
            var configuration = DeepEqualityComparer.CreateConfiguration();

            DeepEqualityComparer comparer = configuration;

            Assert.That(1, Is.EqualTo(2).Using(comparer));
        }
    }
}
