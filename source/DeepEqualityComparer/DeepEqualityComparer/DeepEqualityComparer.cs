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

#endregion

namespace deepequalitycomparer
{
    public static class DeepEqualityComparer
    {
        public static DeepEqualityComparer<T> Default<T>()
        {
            return DeepEqualityComparer<T>.Default;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <typeparam name="T">The Type of the objects to compare.</typeparam>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public static bool Equals<T>(T x, T y)
        {
            return Default<T>().Equals(x, y);
        }
    }

    public class DeepEqualityComparer<T> : IEqualityComparer<T>
    {
        internal static DeepEqualityComparer<T> Default { get; } = new DeepEqualityComparer<T>();

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first object of type T to compare.</param>
        /// <param name="y">The second object of type T to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(T x, T y)
        {
            return AreEqual(x, y);
        }

        /// <summary>
        /// Always returns 0 to force a full compare
        /// </summary>
        /// <returns>
        /// Always returns 0 to force a full compare
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// The type of <paramref name="obj" /> is a reference type and
        /// <paramref name="obj" /> is null.
        /// </exception>
        public int GetHashCode(T obj)
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
        internal bool AreEqual(object x, object y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;

            if (AreBothArrays(x, y))
            {
                return this.AreArraysEqual((Array)x, (Array)y);
            }

            if (!AreTypesEqual(x, y)) return false;

            return x.Equals(y);
        }

        /// <summary>
        /// Tests whether the specified arrays are equal
        /// </summary>
        /// <param name="x">The first Array</param>
        /// <param name="y">The second Array</param>
        /// <returns></returns>
        private bool AreArraysEqual(Array x, Array y)
        {
            if (x.Length !=
                y.Length) return false;

            for (int i = 0; i < x.Length; i++)
            {
                if (!this.AreEqual(x.GetValue(i), y.GetValue(i))) return false;
            }

            return true;
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
