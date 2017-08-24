#region File Header

// MIT License
// 
// Copyright (c) 2017 Stefan Stolz
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
#if NET35
    public class Tuple<T1, T2>
    {
        public Tuple(T1 item1, T2 item2)
        {
            this.Item1 = item1;
            this.Item2 = item2;
        }

        public T1 Item1 { get; }   
        public T2 Item2 { get; }
    }

    public static class Tuple
    {
        public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
        {
            return new Tuple<T1, T2>(item1, item2);
        }
    }
#endif

    public class DeepEqualityComparer<T> : DeepEqualityComparer, IEqualityComparer<T>
    {
        public DeepEqualityComparer()
        { }

        public DeepEqualityComparer(TextWriter loggingTextWriter)
            : base(loggingTextWriter)
        { }

        public DeepEqualityComparer(TextWriter loggingTextWriter, bool logOnlyNotEqualItems)
            : base(loggingTextWriter, logOnlyNotEqualItems)
        { }

        public DeepEqualityComparer(TextWriter loggingTextWriter,
                                    ReadOnlyCollection<string> propertiesToIgnore,
                                    ReadOnlyCollection<Type> typesToIgnore,
                                    StringComparison? stringComparison,
                                    bool treatNullAsEmptyString,
                                    bool ignoreIndexer,
                                    bool logOnlyNotEqualItems,
                                    IEnumerable<KeyValuePair<Type, IEqualityComparer>>
                                        comparerForSpecificType,
                                    IEnumerable<Tuple<Type, object>> nullReplacements)
            : base(loggingTextWriter,
                   propertiesToIgnore,
                   typesToIgnore,
                   stringComparison,
                   treatNullAsEmptyString,
                   ignoreIndexer,
                   logOnlyNotEqualItems,
                   comparerForSpecificType,
                   nullReplacements)
        { }

        public bool Equals(T x, T y)
        {
            return base.Equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return base.GetHashCode(obj);
        }
    }

    public class DeepEqualityComparer : IEqualityComparer
    {
        public static DeepEqualityComparer Default { get; } = new DeepEqualityComparer();
        public static DeepEqualityComparer DefaultWithConsoleOutput => new DeepEqualityComparer(Console.Out);
        public static DeepEqualityComparer DefaultWithLogNotEqualToConsole =>
            new DeepEqualityComparer(Console.Out, true);

        private readonly Dictionary<Type, IEqualityComparer> comparerForSpecificType =
            new Dictionary<Type, IEqualityComparer>();
        private readonly bool ignoreIndexer;

        private readonly TextWriter loggingTextWriter;
        private readonly bool logOnlyNotEqualItems;

        private readonly Dictionary<Type, object> nullReplacements = new Dictionary<Type, object>();
        private readonly ReadOnlyCollection<string> propertiesToIgnore =
            new ReadOnlyCollection<string>(new string[0]);
        private readonly StringComparison? stringComparison;
        private readonly bool treatNullAsEmptyString;
        private readonly ReadOnlyCollection<Type> typesToIgnore = new ReadOnlyCollection<Type>(new Type[0]);

        protected DeepEqualityComparer()
        { }

        protected DeepEqualityComparer(
            TextWriter loggingTextWriter,
            ReadOnlyCollection<string> propertiesToIgnore,
            ReadOnlyCollection<Type> typesToIgnore,
            StringComparison? stringComparison,
            bool treatNullAsEmptyString,
            bool ignoreIndexer,
            bool logOnlyNotEqualItems,
            IEnumerable<KeyValuePair<Type, IEqualityComparer>> comparerForSpecificType,
            IEnumerable<Tuple<Type, object>> nullReplacements)
        {
            this.loggingTextWriter = loggingTextWriter;
            this.propertiesToIgnore = propertiesToIgnore;
            this.typesToIgnore = typesToIgnore;
            this.stringComparison = stringComparison;
            this.treatNullAsEmptyString = treatNullAsEmptyString;
            this.ignoreIndexer = ignoreIndexer;
            this.logOnlyNotEqualItems = logOnlyNotEqualItems;

            foreach (var keyValuePair in comparerForSpecificType) {
                this.comparerForSpecificType.Add(keyValuePair.Key, keyValuePair.Value);
            }

            foreach (var nullReplacement in nullReplacements) {
                this.nullReplacements.Add(nullReplacement.Item1, nullReplacement.Item2);
            }
        }

        public DeepEqualityComparer(TextWriter loggingTextWriter)
        {
            this.loggingTextWriter =
                loggingTextWriter ?? throw new ArgumentNullException(nameof(loggingTextWriter));
        }

        protected DeepEqualityComparer(TextWriter loggingTextWriter, bool logOnlyNotEqualItems)
        {
            this.loggingTextWriter =
                loggingTextWriter ?? throw new ArgumentNullException(nameof(loggingTextWriter));
            this.logOnlyNotEqualItems = logOnlyNotEqualItems;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public new bool Equals(object x, object y)
        {
            var context = new Context("(root)");
            this.AreEqualInternal(context, x, y);

            this.PrintResult(context);

            return Context.AllEqual(context);
        }

        /// <inheritdoc />
        public int GetHashCode(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return 0;
        }

        internal void PrintResult(Context context)
        {
            if (this.loggingTextWriter == null) return;

            using (var textWriter = new IndentedTextWriter(this.loggingTextWriter, "  ")) {
                textWriter.Indent = 0;
                this.PrintItem(textWriter, context);
                this.PrintItems(textWriter, context.GetAllChildren());
            }
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

            for (int i = 0; i < x.Length; i++) {
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
            context.SetPrintableValues(this.GetPrintableValue(x), this.GetPrintableValue(y));

            if (ReferenceEquals(x, y)) {
                context.SetResult(true, "Equal Reference");
                return;
            }

            if (this.treatNullAsEmptyString) {
                if (x is string ||
                    y is string) {
                    x = x ?? string.Empty;
                    y = y ?? string.Empty;
                }
            }

            if (ReferenceEquals(x, null) ^ ReferenceEquals(y, null)) {
                if (ReferenceEquals(x, null)) {
                    x = this.GetNullReplacementValue(y.GetType());
                }
                else {
                    y = this.GetNullReplacementValue(x.GetType());
                }
            }

            if (ReferenceEquals(x, null)) {
                context.SetResult(false, "x == null");
                return;
            }
            if (ReferenceEquals(y, null)) {
                context.SetResult(false, "y == null");
                return;
            }

            if (AreBothArrays(x, y)) {
                var value = this.AreArraysEqual(context, (Array)x, (Array)y);
                context.SetResult(value, "Array");
                return;
            }

            if (this.AreBothPureIEnumerable(x, y)) {
                var value = this.AreIEnumerablesEqual(context, x, y);
                context.SetResult(value, "IEnumerable");
                return;
            }

            if (!AreTypesEqual(x, y)) {
                context.SetResult(false, "Types not equal");
                return;
            }

            if (this.TypeSpecificComparerExists(x.GetType())) {
                var value = this.CompareWithTypeSpecificComparer(x, y);
                context.SetResult(value, "Type Specifiy Comparer");
                return;
            }

            if (IsPrimitiveValueType(x)) {
                var value = x.Equals(y);

                context.SetResult(value, "Valuetype");
                return;
            }

            if (this.IsEnum(x)) {
                var value = x.Equals(y);

                context.SetResult(value, "Enum");
                return;
            }

            if (this.IsString(x)) {
                var value = this.AreStringsEqual((string)x, (string)y);
                context.SetResult(value, "String");
                return;
            }

            if (HasTypeSpecificEuquals(x)) {
                var value = this.AreEqualBySpecificEquals(x, y);
                if (value) {
                    context.SetResult(true, "Equals");
                    return;
                }
            }

            if (this.IsIEnumerable(x) &&
                this.IsIEnumerable(y)) {
                var value = this.AreIEnumerablesEqual(context, x, y);
                context.SetResult(value, "IEnumerable");
                return;
            }

            this.ArePropertiesEqual(context, x, y);
        }

        private bool AreIEnumerablesEqual(Context context, object x, object y)
        {
            var listX = this.MakeArrayList(x);
            var listY = this.MakeArrayList(y);

            return this.AreArraysEqual(context, listX.ToArray(), listY.ToArray());
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
            if (x.GetType() != y.GetType())
                throw new ArgumentNullException(nameof(y), "y must have the same type as x");

            var properties = x.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var propertyInfo in properties) {
                if (!propertyInfo.CanRead) continue;
                if (this.propertiesToIgnore.Contains(propertyInfo.Name)) continue;
                if (this.typesToIgnore.Contains(propertyInfo.PropertyType)) continue;

                if (!this.IsIndexer(propertyInfo)) {
                    object valueOfX = propertyInfo.GetValue(x, null);
                    object valueOfY = propertyInfo.GetValue(y, null);

                    this.AreEqualInternal(context.CreateChild(propertyInfo.Name), valueOfX, valueOfY);
                }
                else {
                    if (!this.AreIndexerEqual(propertyInfo, x, y)) {
                        throw new NotImplementedException();
                    }
                }
            }

            context.SetResult(Context.AllChildrenEqual(context), "Properties");
        }

        private bool AreStringsEqual(string x, string y)
        {
            if (this.treatNullAsEmptyString) {
                x = x ?? string.Empty;
                y = y ?? string.Empty;
            }

            if (this.stringComparison.HasValue) {
                return x.Equals(y, this.stringComparison.Value);
            }

            return x.Equals(y);
        }

        private bool CompareWithTypeSpecificComparer(object x, object y)
        {
            var comparer = this.comparerForSpecificType[x.GetType()];

            return comparer.Equals(x, y);
        }

        private object GetNullReplacementValue(Type type)
        {
            this.nullReplacements.TryGetValue(type, out var result);

            return result;
        }

        private string GetPrintableValue(object obj)
        {
            if (ReferenceEquals(obj, null)) return "(null)";

            if (IsArray(obj)) {
                return "(array)";
            }

            if (this.IsPureIEnumerable(obj)) {
                return "(IEnumerable)";
            }

            return obj.ToString();
        }

        private bool IsEnum(object x)
        {
            return x.GetType().IsEnum;
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

        private bool IsIndexer(PropertyInfo propertyInfo)
        {
            return propertyInfo.GetIndexParameters().Length > 0;
        }

        private bool IsPureIEnumerable(object obj)
        {
            var type = obj.GetType();

            if (type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Any()) return false;
            var isIEnumerable = type.GetInterfaces().Any(i => i == typeof(IEnumerable));

            return isIEnumerable;
        }

        private bool IsString(object obj)
        {
            return obj is string;
        }

        private ArrayList MakeArrayList(object obj)
        {
            if (!this.IsIEnumerable(obj)) throw new ArgumentException("obj must be an IEnumerable");

            var enumerable = (IEnumerable)obj;

            var result = new ArrayList();

            foreach (var value in enumerable) {
                result.Add(value);
            }

            return result;
        }

        private void PrintItem(TextWriter textWriter, Context context)
        {
            if (this.logOnlyNotEqualItems &&
                context.Result) return;

            var itemEqual = context.Result ? "equal" : "not equal";
            textWriter.WriteLine($"{context.Caption}: {itemEqual} - x: {context.XtoString} y: {context.YtoString}");
        }

        private void PrintItems(IndentedTextWriter textWriter, IEnumerable<Context> items)
        {
            textWriter.Indent += 1;
            foreach (var item in items) {
                this.PrintItem(textWriter, item);
                this.PrintItems(textWriter, item.GetAllChildren());
            }
            textWriter.Indent -= 1;
        }

        private bool TypeSpecificComparerExists(Type type)
        {
            return this.comparerForSpecificType.ContainsKey(type);
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

        public static DeepEqualityComparer<T> CraeteDefaultWithLogOnlyNotEqualToConsole<T>() =>
            new DeepEqualityComparer<T>(Console.Out, true);

        public static Configuration CreateConfiguration()
        {
            return new Configuration();
        }

        public static DeepEqualityComparer<T> CreateDefault<T>() => new DeepEqualityComparer<T>();

        public static DeepEqualityComparer<T> CreateDefaultWithConsoleOutput<T>() =>
            new DeepEqualityComparer<T>(Console.Out);

        private static MethodInfo GetTypeSpecificEquals(object obj)
        {
            var type = obj.GetType();

            var methods = type.GetMethods();

            return
                methods.FirstOrDefault(x => x.Name.Equals("Equals") &&
                                            obj.GetType() ==
                                            x.GetParameters().SingleOrDefault()?.ParameterType);
        }

        private static bool HasTypeSpecificEuquals(object obj)
        {
            var method = GetTypeSpecificEquals(obj);

            return method != null;
        }

        private static bool IsArray(object obj)
        {
            return obj.GetType().IsArray;
        }

        private static bool IsPrimitiveValueType(object x)
        {
            var type = x.GetType();
            return type.IsValueType && type.IsPrimitive;
        }

        public class Configuration
        {
            private readonly Dictionary<Type, IEqualityComparer> comparerForSpecificType =
                new Dictionary<Type, IEqualityComparer>();
            private readonly Dictionary<Type, object> nullReplacements = new Dictionary<Type, object>();
            private readonly List<string> propertiesToIgnore = new List<string>();
            private readonly List<Type> typesToIgnore = new List<Type>();
            private bool ignoreIndexer;

            private TextWriter loggingTextWriter;
            private bool logOnlyNotEqualItems;
            private StringComparison? stringComparison;

            private bool treatNullAsEmptyString;

            public Configuration ConfigureStringComparison(StringComparison stringComparison)
            {
                this.stringComparison = stringComparison;

                return this;
            }

            public DeepEqualityComparer<T> CreateDeepEqualityComparer<T>()
            {
                var propsToIgnore = this.propertiesToIgnore.Distinct().ToList().AsReadOnly();
                var typesToIgnore = this.typesToIgnore.Distinct().ToList().AsReadOnly();

                return new DeepEqualityComparer<T>(
                                                   this.loggingTextWriter,
                                                   propsToIgnore,
                                                   typesToIgnore,
                                                   this.stringComparison,
                                                   this.treatNullAsEmptyString,
                                                   this.ignoreIndexer,
                                                   this.logOnlyNotEqualItems,
                                                   this.comparerForSpecificType,
                                                   this.nullReplacements
                                                       .Select(x => Tuple.Create(x.Key, x.Value))
                                                       .ToList()
                                                       .AsReadOnly());
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
                                                this.logOnlyNotEqualItems,
                                                this.comparerForSpecificType,
                                                this.nullReplacements
                                                    .Select(x => Tuple.Create(x.Key, x.Value))
                                                    .ToList()
                                                    .AsReadOnly());
            }

            public Configuration IgnorePropertyByName(string nameOfProperty)
            {
                this.propertiesToIgnore.Add(nameOfProperty);

                return this;
            }

            public Configuration IgnorePropertyByType(Type type)
            {
                this.typesToIgnore.Add(type);
                return this;
            }

            public Configuration RegisterEqualityComparerForType(Type type, IEqualityComparer comparer)
            {
                if (type == null) throw new ArgumentNullException(nameof(type));
                if (comparer == null) throw new ArgumentNullException(nameof(comparer));

                this.comparerForSpecificType.Add(type, comparer);

                return this;
            }

            public Configuration RegisterEqualityComparerForType<T>(IEqualityComparer<T> comparer)
            {
                if (comparer == null) throw new ArgumentNullException(nameof(comparer));

                this.comparerForSpecificType.Add(typeof(T), new ComparerAdapter<T>(comparer));

                return this;
            }

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

            public Configuration TreatNullAs<T>(T other)
            {
                this.nullReplacements[typeof(T)] = other;

                return this;
            }

            public Configuration TreatNullAsEmptyString(bool treatNullAsEmptyString)
            {
                this.treatNullAsEmptyString = treatNullAsEmptyString;
                return this;
            }

            public class ComparerAdapter<T> : IEqualityComparer
            {
                private readonly IEqualityComparer<T> comparer;

                public ComparerAdapter(IEqualityComparer<T> comparer)
                {
                    this.comparer = comparer;
                }

                public bool Equals(object x, object y)
                {
                    return this.comparer.Equals((T)x, (T)y);
                }

                public int GetHashCode(object obj)
                {
                    return this.comparer.GetHashCode((T)obj);
                }
            }
        }

        internal class Context
        {
            private readonly List<Context> children = new List<Context>();

            public Context(string caption)
            {
#if NET35
                if (string.IsNullOrEmpty(caption?.Trim())) throw new ArgumentException("Value cannot be null or whitespace.", nameof(caption));
#else
                if (string.IsNullOrWhiteSpace(caption))
                    throw new ArgumentException("Value cannot be null or whitespace.", nameof(caption));
#endif
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
                foreach (var child in this.children) {
                    yield return child;

                    foreach (var grandChild in child.GetAllChildren()) {
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
