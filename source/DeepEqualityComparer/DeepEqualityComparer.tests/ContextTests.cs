using System;
using System.Linq;
using System.Runtime.Remoting.Contexts;

using NUnit.Framework;

namespace deepequalitycomparer.tests
{
    [TestFixture]
    public sealed class ContextTests
    {
        [Test]
        public void AllEqual()
        {
            var root = new DeepEqualityComparer.Context("(root)");
            root.SetResult(true, "shibby");

            for (int i = 0; i < 10; i++)
            {
                var child = root.CreateChild($"[{i}]");
                child.SetResult(true, "xxxx");

                for (int j = 0; j < 20; j++)
                {
                    child.CreateChild($"XXX {j}").SetResult(true, "yyy");
                }
            }

            Assert.That(DeepEqualityComparer.Context.AllEqual(root), Is.True);
        }

        [Test]
        public void GetAllChildren()
        {
            var root = new DeepEqualityComparer.Context("x");

            var parent = root;

            for (int i = 0; i < 10; i++)
            {
                parent = parent.CreateChild($"{i}");
            }

            var all = root.AllChildren().ToArray();

            Assert.That(all.Length, Is.EqualTo(10));
        }
    }
}
