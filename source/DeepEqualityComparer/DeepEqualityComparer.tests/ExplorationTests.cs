#region File Header

// MIT License
// 
// Copyright (c) 2016 Stefan Stolz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

#region using directives

using System;
using System.Linq;

using NUnit.Framework;

#endregion

namespace deepequalitycomparer.tests
{
    [TestFixture]
    [Explicit]
    public sealed class ExplorationTests
    {
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
    }
}
