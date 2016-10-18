# DeepEqualityComparer

DeepEqualityComparer checks two objects trees for equality.
It implements the .NET Framekwork IComparer<T> interface and can be uses everywhere where an IComparer<T>
is accepted.

It primary intended to be used in Unit-Tests.

## Usage

```csharp
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
            
            Assert.That(so1, Is.EqualTo(so2).Using(DeepEqualityComparer.Default));
        }
    }
```
