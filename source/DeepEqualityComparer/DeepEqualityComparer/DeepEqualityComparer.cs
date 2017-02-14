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
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;

#endregion

namespace deepequalitycomparer
{
    public class DeepEqualityComparer : IEqualityComparer
    {
        public static DeepEqualityComparer Default { get; } = new DeepEqualityComparer();

        public static DeepEqualityComparer DefaultWithConsoleOutput => new DeepEqualityComparer(Console.Out);

        private readonly TextWriter loggingTextWriter;
        private readonly IReadOnlyCollection<string> propertiesToIgnore = new ReadOnlyCollection<string>(new string[0]);
        private readonly IReadOnlyCollection<Type> typesToIgnore = new ReadOnlyCollection<Type>(new Type[0]);
        private readonly StringComparison? stringComparison;
        private readonly bool treatNullAsEmptyString;
        private readonly bool ignoreIndexer;
        private readonly bool logOnlyNotEqualItems;

        private DeepEqualityComparer()
        {}

        public class Configuration
        {
            private StringComparison? stringComparison;

            private TextWriter loggingTextWriter;

            private bool treatNullAsEmptyString;

            private readonly List<string> propertiesToIgnore = new List<string>();
            private readonly List<Type> typesToIgnore = new List<Type>();

            private bool ignoreIndexer;
            private bool logOnlyNotEqualItems;

            public Configuration SetIgnoreIndexer(bool ignoreIndexer)
            {
                this.ignoreIndexer = ignoreIndexer;

                return this;
            }

            public Configuration SetLoggingTextWriter(TextWriter textWriter)
            {
                this.loggingTextWriter = textWriter;

                return this;
            }


            public Configuration SetLoggingTextWriter(TextWriter textWriter, bool logOnlyNotEqualItems)
            {
                this.loggingTextWriter = textWriter;
                this.logOnlyNotEqualItems = logOnlyNotEqualItems;

                return this;
            }

            public Configuration IgnorePropertyByName(string nameOfProperty)
            {
                this.propertiesToIgnore.Add(nameOfProperty);

                return this;
            }

            public Configuration ConfigureStringComparison(StringComparison stringComparison)
            {
                this.stringComparison = stringComparison;

                return this;
            }

            public Configuration TreatNullAsEmptyString(bool treatNullAsEmptyString)
            {
                this.treatNullAsEmptyString = treatNullAsEmptyString;
                return this;
            }

            public DeepEqualityComparer CreateEqualityComparer()
            {
                var propsToIgnore = this.propertiesToIgnore.Distinct().ToList().AsReadOnly();
                var typesToIgnore = this.typesToIgnore.Distinct().ToList().AsReadOnly();

                return new DeepEqualityComparer(
                    this.loggingTextWriter,
                    propsToIgnore,
                    typesToIgnore,
                    this.stringComparison,
                    this.treatNullAsEmptyString,
                    this.ignoreIndexer,
                    this.logOnlyNotEqualItems);
            }

            public Configuration IgnorePropertyByType(Type type)
            {
                this.typesToIgnore.Add(type);
                return this;
            }
        }

        public static Configuration CreateConfiguration()
        {
            return new Configuration();
        }

        private DeepEqualityComparer(
            TextWriter loggingTextWriter,
            IReadOnlyCollection<string> propertiesToIgnore,
            IReadOnlyCollection<Type> typesToIgnore,
            StringComparison? stringComparison,
            bool treatNullAsEmptyString,
            bool ignoreIndexer, bool logOnlyNotEqualItems)
        {
            this.loggingTextWriter = loggingTextWriter;
            this.propertiesToIgnore = propertiesToIgnore;
            this.typesToIgnore = typesToIgnore;
            this.stringComparison = stringComparison;
            this.treatNullAsEmptyString = treatNullAsEmptyString;
            this.ignoreIndexer = ignoreIndexer;
            this.logOnlyNotEqualItems = logOnlyNotEqualItems;
        }

        public DeepEqualityComparer(TextWriter loggingTextWriter)
        {
            if (loggingTextWriter == null) throw new ArgumentNullException(nameof(loggingTextWriter));
            this.loggingTextWriter = loggingTextWriter;
        }

        internal void PrintResult(Context context)
        {
            if (this.loggingTextWriter == null) return;

            using (var textWriter = new IndentedTextWriter(this.loggingTextWriter, "  "))
            {
                textWriter.Indent = 0;
                this.PrintItem(textWriter, context);
                this.PrintItems(textWriter, context.GetAllChildren());
            }
        }

        private void PrintItems(IndentedTextWriter textWriter, IEnumerable<Context> items)
        {
            textWriter.Indent += 1;
            foreach (var item in items)
            {
                this.PrintItem(textWriter, item);
                this.PrintItems(textWriter, item.GetAllChildren());
            }
            textWriter.Indent -= 1;
        }

        private void PrintItem(TextWriter textWriter, Context context)
        {
            if (this.logOnlyNotEqualItems &&
                context.Result) return;

            var itemEqual = context.Result ? "equal" : "not equal";
            textWriter.WriteLine($"{context.Caption}: {itemEqual} - x: {context.XtoString} y: {context.YtoString}");
        }

        /// <summary>
        /// Tests whether the specified arrays are equal
        /// </summary>
        /// <param name="context">The Context of the equal operation</param>
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

        private bool AreBothPureIEnumerable(object x, object y)
        {
            var isXanIEnumerable = this.IsPureIEnumerable(x);
            var ixYanIEnumerable = this.IsPureIEnumerable(y);

            return isXanIEnumerable && ixYanIEnumerable;
        }

        private bool AreEqualBySpecificEquals(object x, object y)
        {
            var method = GetTypeSpecificEquals(x);

            return (bool)method.Invoke(x, new[] { y });
        }

        /// <summary>
        /// Tests two objects recursive for equality
        /// </summary>
        /// <param name="context">The Context of the equal operation</param>
        /// <param name="x">The first object</param>
        /// <param name="y">The second object</param>
        private void AreEqualInternal(Context context, object x, object y)
        {
            context.SetPrintableValues(GetPrintableValue(x), GetPrintableValue(y));

            if (ReferenceEquals(x, y))
            {
                context.SetResult(true, "Equal Reference");
                return;
            }

            if (this.treatNullAsEmptyString)
            {
                if (x is string ||
                    y is string)
                {
                    x = x ?? string.Empty;
                    y = y ?? string.Empty;
                }
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

            if (IsString(x))
            {
                var value = AreStringsEqual((string)x, (string)y);
                context.SetResult(value, "String");
                return;
            }

            if (HasTypeSpecificEuquals(x))
            {
                var value = AreEqualBySpecificEquals(x, y);
                context.SetResult(value, "Equals");
                return;
            }

            if (IsIEnumerable(x) &&
                IsIEnumerable(y))
            {
                var value = this.AreIEnumerablesEqual(context, x, y);
                context.SetResult(value, "IEnumerable");
                return;
            }

            ArePropertiesEqual(context, x, y);
        }

        private bool IsString(object obj)
        {
            return obj is string;
        }

        private bool AreStringsEqual(string x, string y)
        {
            if (this.treatNullAsEmptyString)
            {
                x = x ?? string.Empty;
                y = y ?? string.Empty;
            }

            if (this.stringComparison.HasValue)
            {
                return x.Equals(y, this.stringComparison.Value);
            }

            return x.Equals(y);
        }

        private string GetPrintableValue(object obj)
        {
            if (object.ReferenceEquals(obj, null)) return "(null)";

            if (IsArray(obj))
            {
                return "(array)";
            }

            if (this.IsPureIEnumerable(obj))
            {
                return "(IEnumerable)";
            }

            return obj.ToString();
        }

        private bool AreIEnumerablesEqual(Context context, object x, object y)
        {
            var listX = this.MakeArrayList(x);
            var listY = this.MakeArrayList(y);

            return AreArraysEqual(context, listX.ToArray(), listY.ToArray());
        }

        private bool AreIndexerEqual(PropertyInfo propertyInfo, object x, object y)
        {
            if (this.ignoreIndexer) return true;
            throw new NotSupportedException("Indexers are currently not supported");
        }

        /// <summary>
        /// Tests all Properties of the objects for euqality.
        /// </summary>
        /// <param name="context">The Context of the equal operation</param>
        /// <param name="x">The first object</param>
        /// <param name="y">The second object</param>
        private void ArePropertiesEqual(Context context, object x, object y)
        {
            if (x.GetType() != y.GetType()) throw new ArgumentNullException(nameof(y), "y must have the same type as x");

            var properties = x.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var propertyInfo in properties)
            {
                if (!propertyInfo.CanRead) continue;
                if (this.propertiesToIgnore.Contains(propertyInfo.Name)) continue;
                if (this.typesToIgnore.Contains(propertyInfo.PropertyType)) continue;

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

        /// <summary>
        /// Tests whether the specified object implements <see cref="IEnumerable" />.
        /// </summary>
        /// <param name="obj">The object to test.</param>
        /// <returns>
        /// <c>true</c> whether <paramref name="obj" /> implements <see cref="IEnumerable" />; otherwise <c>false</c>
        /// </returns>
        private bool IsIEnumerable(object obj)
        {
            var isIEnumerable = obj.GetType().GetInterfaces().Any(i => i == typeof(IEnumerable));

            return isIEnumerable;
        }

        private bool IsPureIEnumerable(object obj)
        {
            var type = obj.GetType();

            if (type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Any()) return false;
            var isIEnumerable = type.GetInterfaces().Any(i => i == typeof(IEnumerable));

            return isIEnumerable;
        }

        private bool IsIndexer(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetIndexParameters().Length > 0;
        }

        private ArrayList MakeArrayList(object obj)
        {
            if (!IsIEnumerable(obj)) throw new ArgumentException("obj must be an IEnumerable");

            var enumerable = (IEnumerable)obj;

            var result = new ArrayList();

            foreach (var value in enumerable)
            {
                result.Add(value);
            }

            return result;
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

            this.PrintResult(context);

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

        private static bool IsArray(object obj)
        {
            return obj.GetType().IsArray;
        }

        /// <summary>
        /// Tests whether the specified objects are arrays.
        /// </summary>
        /// <param name="x">The first object</param>
        /// <param name="y">The second object</param>
        /// <returns><c>true</c> whether both objects are arrays; otherwise <c>false</c></returns>
        private static bool AreBothArrays(object x, object y)
        {
            return IsArray(x) && IsArray(y);
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
        /// Test whether the specified objects have the same type
        /// </summary>
        /// <param name="x">The first object</param>
        /// <param name="y">The second object</param>
        /// <returns><c>true</c> whether both objects have the same type; otherwise <c>false</c></returns>
        private static bool AreTypesEqual(object x, object y)
        {
            return x.GetType() == y.GetType();
        }

        private static MethodInfo GetTypeSpecificEquals(object obj)
        {
            var type = obj.GetType();

            var methods = type.GetMethods();

            return
                methods.FirstOrDefault(x => x.Name.Equals("Equals") && obj.GetType() == x.GetParameters().SingleOrDefault()?.ParameterType);
        }

        private static bool HasTypeSpecificEuquals(object obj)
        {
            var method = GetTypeSpecificEquals(obj);

            return method != null;
        }

        private static bool IsValueType(object x)
        {
            return x.GetType().IsValueType;
        }

        internal class Context
        {
            private readonly List<Context> children = new List<Context>();

            public Context(string caption)
            {
                if (string.IsNullOrWhiteSpace(caption)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(caption));
                this.Caption = caption;
            }

            public Context CreateChild(string childcaption)
            {
                var child = new Context(childcaption);
                this.children.Add(child);
                return child;
            }

            public IEnumerable<Context> GetAll()
            {
                return new[] { this }.Concat(this.GetAllChildren());
            }

            public IEnumerable<Context> GetAllChildren()
            {
                foreach (var child in this.children)
                {
                    yield return child;

                    foreach (var grandChild in child.GetAllChildren())
                    {
                        yield return grandChild;
                    }
                }
            }

            public void SetPrintableValues(string xToString, string yToString)
            {
                this.XtoString = xToString;
                this.YtoString = yToString;
            }

            public void SetResult(bool equal, string description)
            {
                this.Result = equal;
                this.ResultDescription = description;
            }

            public string Caption { get; }

            public bool Result { get; private set; }
            public string ResultDescription { get; private set; }

            public string XtoString { get; private set; } = string.Empty;
            public string YtoString { get; private set; } = string.Empty;

            public static bool AllChildrenEqual(Context context)
            {
                var allChildren = context.GetAllChildren().ToArray();
                return allChildren.All(c => c.Result);
            }

            public static bool AllEqual(Context context)
            {
                var all = context.GetAll();
                return all.All(c => c.Result);
            }
        }
    }
}
