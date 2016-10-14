using System;
using System.Linq;

using NUnit.Framework;

namespace deepequalitycomparer.tests
{
    [TestFixture]
    [Explicit]
    public sealed class ExplorationTests
    {
        [Test]
        public void CompareStringsByReflection()
        {
            string text = "shibby";
            
            var type = text.GetType();

            var properties = type.GetProperties();

            var lengthProp = properties.FirstOrDefault(p => p.Name.Equals("Length") && p.PropertyType == typeof(int));

            int? length = null;

            if (lengthProp != null)
            {
                length = (int)lengthProp.GetValue(text);
            }

            var charsProp = properties.First(p => p.Name.Equals("Chars", StringComparison.OrdinalIgnoreCase));

            var parameters = charsProp.GetIndexParameters();

            if (parameters.Length == 1 &&
                parameters[0].ParameterType == typeof(int) &&
                length.HasValue)
            {
                for (int i = 0; i < length.Value; i++)
                {
                    var value = charsProp.GetValue(text, new object[] { i });
                }
            }
        }

        [Test]
        public void CompareStringByLookingForEqualsWithStringParameter()
        {
            var text = "shibby";

            var type = text.GetType();
            
            var method = type.GetMethod("Equals", new Type[] { type });

            if (method != null)
            {
                var result = method.Invoke(text, new object[] { "shibby" });
            }
        }
    }
}
