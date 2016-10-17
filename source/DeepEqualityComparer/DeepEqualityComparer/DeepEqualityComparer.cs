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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace deepequalitycomparer
{
    public class DeepEqualityComparer : IEqualityComparer
    {
        internal static DeepEqualityComparer Default { get; } = new DeepEqualityComparer();

        internal class Context
        {
            private readonly List<Context> children = new List<Context>();

            public Context(string caption)
            {
                if (string.IsNullOrWhiteSpace(caption)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(caption));
                this.Caption = caption;
            }

            public string Caption { get; }

            public Context CreateChild(string childcaption)
            {
                var child = new Context(childcaption);
                this.children.Add(child);
                return child;
            }

            public bool Result { get; private set; }
            public string ResultDescription { get; private set; }

            public IEnumerable<Context> AllChildren()
            {
                foreach (var child in this.children)
                {
                    yield return child;

                    foreach (var grandChild in child.AllChildren())
                    {
                        yield return grandChild;
                    }
                }
            }

            public void SetResult(bool equal, string description)
            {
                this.Result = equal;
                this.ResultDescription = description;
            }

            public static bool AllChildrenEqual(Context context)
            {
                var all = context.AllChildren().ToArray();
                return all.All(c => c.Result);
            }

            public static bool AllEqual(Context context)
            {
                var all = new[] {context}.Concat(context.AllChildren()).ToArray();
                return all.All(c => c.Result);
            }
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <typeparam name="T">The Type of the objects to compare.</typeparam>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public static bool AreEqual(object x, object y)
        {
            return Default.Equals(x, y);
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(object x, object y)
        {
            var context = new Context("(root)");
            this.AreEqualInternal(context, x, y);

            return Context.AllEqual(context);
        }

        /// <summary>
        /// Always returns 0 to force a full compare
        /// </summary>
        /// <returns>
        /// Always returns 0 to force a full compare
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// The type of <paramref name="obj" /> is a reference type and
        /// <paramref name="obj" /> is null.
        /// </exception>
        public int GetHashCode(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return 0;
        }

        /// <summary>
        /// Tests two objects recursive for equality
        /// </summary>
        /// <param name="x">The first object</param>
        /// <param name="y">The second object</param>
        /// <returns><c>true</c> whether the specified objects are equal; otherwise false</returns>
        private void AreEqualInternal(Context context, object x, object y)
        {
            if (ReferenceEquals(x, y))
            {
                context.SetResult(true, "Equal Reference");
                return;
            }
            if (ReferenceEquals(x, null))
            {
                context.SetResult(false, "x == null");
                return;
            }
            if (ReferenceEquals(y, null))
            {
                context.SetResult(false, "y == null");
                return;
            }

            if (AreBothArrays(x, y))
            {
                var value = this.AreArraysEqual(context, (Array)x, (Array)y);
                context.SetResult(value, "Array");
                return;
            }

            if (AreBothPureIEnumerable(x, y))
            {
                var value = AreIEnumerablesEqual(context, x, y);
                context.SetResult(value, "IEnumerable");
                return;
            }

            if (!AreTypesEqual(x, y))
            {
                context.SetResult(false, "Types not equal");
                return;
            }

            if (IsValueType(x))
            {
                var value = x.Equals(y);
                context.SetResult(value, "Valuetype");
                return;
            }

            if (HasTypeSpecificEuquals(x))
            {
                var value = AreEqualBySpecificEquals(x, y);
                context.SetResult(value, "Equals");
                return;
            }

            ArePropertiesEqual(context, x, y);
        }

        private bool AreEqualBySpecificEquals(object x, object y)
        {
            var method = GetTypeSpecificEquals(x);

            return (bool)method.Invoke(x, new[] { y });
        }

        private static bool HasTypeSpecificEuquals(object obj)
        {
            var method = GetTypeSpecificEquals(obj);

            return method != null;
        }

        private static MethodInfo GetTypeSpecificEquals(object obj)
        {
            var type = obj.GetType();

            var methods = type.GetMethods();

            return methods.FirstOrDefault(x => x.Name.Equals("Equals") && obj.GetType() == x.GetParameters().SingleOrDefault()?.ParameterType);
        }

        private ArrayList MakeArrayList(object obj)
        {
            if (!IsIEnumerable(obj)) throw new ArithmeticException("obj must be an IEnumerable");

            var enumerable = (IEnumerable)obj;

            var result = new ArrayList();

            foreach (var value in enumerable)
            {
                result.Add(value);
            }

            return result;
        }

        private bool AreIEnumerablesEqual(Context context, object x, object y)
        {
            var listX = this.MakeArrayList(x);
            var listY = this.MakeArrayList(y);

            return AreArraysEqual(context, listX.ToArray(), listY.ToArray());
        }

        private bool IsIEnumerable(object obj)
        {
            var isIEnumerable = obj.GetType().GetInterfaces().Any(i => i == typeof(IEnumerable));

            return isIEnumerable;
        }

        private bool AreBothPureIEnumerable(object x, object y)
        {
            var propertiesOfX = x.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var propertiesOfY = y.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (propertiesOfX.Any() || propertiesOfY.Any()) return false;

            var isXanIEnumerable = IsIEnumerable(x);
            var ixYanIEnumerable = IsIEnumerable(y);

            return isXanIEnumerable && ixYanIEnumerable;
        }

        private static bool IsValueType(object x)
        {
            return x.GetType().IsValueType;
        }

        private void ArePropertiesEqual(Context context, object x, object y)
        {
            if (x.GetType() != y.GetType()) throw new ArgumentNullException(nameof(y), "y must have the same type as x");

            var properties = x.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var propertyInfo in properties)
            {
                if (!propertyInfo.CanRead) continue;

                if (!IsIndexer(propertyInfo))
                {
                    object valueOfX = propertyInfo.GetValue(x, null);
                    object valueOfY = propertyInfo.GetValue(y, null);

                    this.AreEqualInternal(context.CreateChild(propertyInfo.Name), valueOfX, valueOfY);
                }
                else
                {
                    if (!AreIndexerEqual(propertyInfo, x, y))
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            context.SetResult(Context.AllChildrenEqual(context), "Properties");
        }

        private bool AreIndexerEqual(PropertyInfo propertyInfo, object x, object y)
        {
            throw new NotSupportedException("Indexers are currently not supported");
        }

        private bool IsIndexer(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetIndexParameters().Length > 0;
        }

        /// <summary>
        /// Tests whether the specified arrays are equal
        /// </summary>
        /// <param name="x">The first Array</param>
        /// <param name="y">The second Array</param>
        /// <returns></returns>
        private bool AreArraysEqual(Context context, Array x, Array y)
        {
            if (x.Length !=
                y.Length) return false;

            for (int i = 0; i < x.Length; i++)
            {
                this.AreEqualInternal(context.CreateChild($"[{i}]"), x.GetValue(i), y.GetValue(i));
            }

            return Context.AllChildrenEqual(context);
        }

        /// <summary>
        /// Test whether the specified objects have the same type
        /// </summary>
        /// <param name="x">The first object</param>
        /// <param name="y">The second object</param>
        /// <returns><c>true</c> whether both objects have the same type; otherwise <c>false</c></returns>
        private static bool AreTypesEqual(object x, object y)
        {
            return x.GetType() == y.GetType();
        }

        /// <summary>
        /// Tests whether the specified objects are arrays.
        /// </summary>
        /// <param name="x">The first object</param>
        /// <param name="y">The second object</param>
        /// <returns><c>true</c> whether both objects are arrays; otherwise <c>false</c></returns>
        private static bool AreBothArrays(object x, object y)
        {
            return x.GetType().IsArray && y.GetType().IsArray;
        }
    }
}
