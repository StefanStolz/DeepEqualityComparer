using System.Collections;

using deepequalitycomparer;

using NUnit.Framework;

namespace UsageOfNugetPackage
{
    [TestFixture]
    public class UseWithNUnit
    {
        private class SomeObject
        {
            public string Text { get; set; }

            public int Number { get; set; }
        }

        [Test]
        public void CompareSomeObjects()
        {
            var so1 = new SomeObject { Number = 12, Text = "abc" };
            var so2 = new SomeObject { Number = 12, Text = "abc" };
            
            Assert.That(so1, Is.EqualTo(so2).Using(DeepEqualityComparer.Default<SomeObject>()));
        }
    }
}