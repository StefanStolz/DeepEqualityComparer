using NUnit.Framework;

namespace deepequalitycomparer.tests
{
    [TestFixture]
    public sealed class TreatNullAsOtherValue
    {
        private class SampleData
        {
            public string Text { get; set; }
        }

        [Test]
        public void TreatNullAsOtherStringIsEqual()
        {
            var item1 = new SampleData();
            var item2 = new SampleData(){Text = "X"};

            var sut = DeepEqualityComparer.CreateConfiguration()
                .TreatNullAs<string>("X")
                .CreateEqualityComparer();

            var result = sut.Equals(item1, item2);

            Assert.That(result, Is.True);
        }

        [Test]
        public void TreatNullAsOtherStringIsNotEqual()
        {
            var item1 = new SampleData();
            var item2 = new SampleData() { Text = "X" };

            var sut = DeepEqualityComparer.CreateConfiguration()
                .TreatNullAs<string>("Y")
                .CreateEqualityComparer();

            var result = sut.Equals(item1, item2);

            Assert.That(result, Is.False);
        }
    }
}

