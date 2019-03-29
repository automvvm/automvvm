// --------------------------------------------------------------------------------
// <copyright file="BackingStore.cs" company="AutoMvvm Development Team">
// Copyright © 2019 AutoMvvm Development Team
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;
using AutoMvvm.Reflection;

namespace AutoMvvm.Design
{
    /// <summary>
    /// Extension methods for getting an automatic class extension object.
    /// </summary>
    public static class BackingStoreExtensions
    {
        /// <summary>
        /// Gets an extension object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type of the object extension.</typeparam>
        /// <param name="source">The source object to bind to.</param>
        /// <returns>The object extension.</returns>
        public static T Get<T>(this object source)
            where T : class
        {
            return BackingStore<T>.GetValue(source);
        }

        /// <summary>
        /// Defines a backing store for an object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of objects to store.</typeparam>
        private static class BackingStore<T>
            where T : class
        {
            private static ConditionalWeakTable<object, T> _store = new ConditionalWeakTable<object, T>();

            /// <summary>
            /// Gets the value of the backing extension for the given source.
            /// </summary>
            /// <param name="source">The source object to bind to.</param>
            /// <returns>The backing object of <typeparamref name="T"/>.</returns>
            public static T GetValue(object source)
            {
                // If the source defines itself an a type resolver, use that to get the type,
                // or fall back to the generic type.
                var resolver = source as ITypeResolver;
                Type type = resolver?.ResolveType(typeof(T)) ?? typeof(T);

                // If the source defines itself as a factory, use that.
                if (source is IFactory factory)
                    return _store.GetValue(source, s => (T)factory.Create(type));

                // Otherwise, attempt to generate type with default constructor.
                return _store.GetValue(source, s => (T)type.CreateInstance());
            }
        }
    }
}
