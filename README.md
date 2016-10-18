# DeepEqualityComparer

DeepEqualityComparer checks two objects trees for equality.
It implements the .NET Framekwork IComparer interface and can be used everywhere where an IComparer
is accepted.

The primary us is in Unit-Tests.

## Usage

### Default

Using the Default Comparere without logging or any special configuration.
All Properties are compared.


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

### Print comparison result to Console

Using the pre-configured instance DefaultWithConsoleOutput the DeepEqualityComparer prints the result
to Console.Out.

```csharp
        [Test]
        public void CompareWithLogging()
        {
            var so1 = new SomeObject { Number = 12, Text = "abc" };
            var so2 = new SomeObject { Number = 12, Text = "abcd" };

            Assert.That(so1, Is.EqualTo(so2).Using(DeepEqualityComparer.DefaultWithConsoleOutput));
        }
```

This will print something like this

    (root): not equal - x: UsageOfNugetPackage.UseWithNUnit+SomeObject y: UsageOfNugetPackage.UseWithNUnit+SomeObject
      Text: not equal - x: abcd y: abc
      Number: equal - x: 12 y: 12


### Write comparison result to a TextWriter

```csharp
     var textWriter = new StringWriter();

     var comparer = DeepEqualityComparer
        .CreateConfiguration()
        .SetLoggingTextWriter(textWriter)
        .CreateEqualityComparer();

      Assert.That(so1, Is.EqualTo(so2).Using(comparer));
```

### Ignore Properties by name

```csharp
    var so1 = new SomeObject { Number = 12, Text = "abc" };
    var so2 = new SomeObject { Number = 12, Text = "abcd" };

    var comparer = DeepEqualityComparer
        .CreateConfiguration()
        .IgnorePropertyByName("Text")
        .CreateEqualityComparer();

        Assert.That(so1, Is.EqualTo(so2).Using(comparer));
```