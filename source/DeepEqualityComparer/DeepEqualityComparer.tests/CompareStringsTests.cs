using System.Text;

using NUnit.Framework;

namespace deepequalitycomparer.tests
{
    [TestFixture]
    public sealed class CompareStringsTests
    {
        [Test]
        public void CompareEqualStrings()
        {
            var text1 = new StringBuilder().Append("abcd").ToString();
            var text2 = new StringBuilder().Append("abcd").ToString();

            Assert.That(DeepEqualityComparer.Equals(text1, text2), Is.True);
        }

        [Test]
        public void CompareDifferentStrings()
        {
            var text1 = new StringBuilder().Append("abcd").ToString();
            var text2 = new StringBuilder().Append("abce").ToString();

            Assert.That(DeepEqualityComparer.Equals(text1, text2), Is.False);
        }
    }
}
