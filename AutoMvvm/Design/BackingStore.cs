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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AutoMvvm.Reflection;

namespace AutoMvvm.Design
{
    /// <summary>
    /// Extension methods for getting an automatic class extension object.
    /// </summary>
    public static class BackingStoreExtensions
    {
        private static readonly ConditionalWeakTable<object, ConcurrentDictionary<Type, object>> _store = new ConditionalWeakTable<object, ConcurrentDictionary<Type, object>>();
        private static readonly ConditionalWeakTable<object, IDictionary<Type, Type>> _typeMapping = new ConditionalWeakTable<object, IDictionary<Type, Type>>();

        /// <summary>
        /// Gets an extension object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type of the object extension.</typeparam>
        /// <param name="source">The source object to bind to.</param>
        /// <returns>The object extension.</returns>
        public static T Get<T>(this object source)
        {
            // The type mappings cache how to map types for the requested type.
            var mappedType = source.GetTypeMapping(typeof(T));

            // If the mapped type defines itself as a type resolver, use that to get the type,
            // or fall back to the generic type.
            var resolver = source as ITypeResolver;
            Type type = resolver?.ResolveType(mappedType) ?? mappedType;

            // Make sure we can still create and assign this type.
            if (!type.IsClass || !typeof(T).IsAssignableFrom(type))
                throw new InvalidOperationException($"Unable to request a type that is not a class or cannot be cast to type '{typeof(T).FullName}' without a type mapping.");

            // Get the type lookup dictionary.
            var lookup = _store.GetOrCreateValue(source);

            // If the source defines itself as a factory, use that.
            if (source is IFactoryProvider factoryProvider && factoryProvider.GetFactory() is Func<Type, object> factory)
                return (T)lookup.GetOrAdd(type, t => (T)factory(type));

            // Otherwise, attempt to generate type with default constructor if it is a class.
            return (T)lookup.GetOrAdd(type, t => (T)type.CreateInstance());
        }

        /// <summary>
        /// Gets an extension object of type <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="TReturn">The type to map the actual object extension to.</typeparam>
        /// <typeparam name="TActual">The actual type of the object extension.</typeparam>
        /// <param name="source">The source object to bind to.</param>
        /// <returns>The object extension.</returns>
        public static TReturn Get<TReturn, TActual>(this object source)
            where TActual : class, TReturn
        {
            // Add a type mapping first, this ensures it can be reached with that same
            // type mapping in the future.
            source.AddTypeMapping<TReturn, TActual>();
            return source.Get<TReturn>();
        }

        /// <summary>
        /// Adds type mapping that maps <typeparamref name="TActual"/> to type <typeparamref name="TReturn"/>.
        /// </summary>
        /// <typeparam name="TReturn">The type to return.</typeparam>
        /// <typeparam name="TActual">The actual backing type.</typeparam>
        /// <param name="source">The source object to bind to.</param>
        public static void AddTypeMapping<TReturn, TActual>(this object source)
            where TActual : class, TReturn
        {
            source.AddTypeMapping(typeof(TReturn), typeof(TActual));
        }

        private static Type GetTypeMapping(this object source, Type requested)
        {
            // The type mappings cache how to map types for the requested type.
            var typeMapping = source.GetTypeMappings();
            lock (typeMapping)
            {
                return typeMapping.TryGetValue(requested, out var mappedType) ? mappedType : requested;
            }
        }

        private static void AddTypeMapping(this object source, Type returnType, Type actualType)
        {
            var typeMapping = source.GetTypeMappings();
            lock (typeMapping)
            {
                // Is this type mapped already?
                if (typeMapping.TryGetValue(returnType, out var mappedType))
                {
                    // If the type mapping is the same, this is an attempt to remap to
                    // the same type, there is nothing to do, just return.
                    if (mappedType == actualType)
                        return;

                    // Remapping a type is not permitted.
                    throw new InvalidOperationException($"The return type '{returnType.FullName}' is already mapped to type '{mappedType.FullName}' and cannot be mapped to type '{actualType.FullName}'");
                }

                if (!returnType.IsAssignableFrom(actualType) || !actualType.IsClass || actualType.IsAbstract || (actualType.IsGenericType && !actualType.IsConstructedGenericType))
                    throw new InvalidOperationException($"The type {actualType} must be able to cast to return type '{returnType.FullName}' and it must be a concrete or constructed generic class.");

                typeMapping.Add(returnType, actualType);
            }
        }

        private static IDictionary<Type, Type> GetTypeMappings(this object source) =>
            _typeMapping.GetValue(source, s => new Dictionary<Type, Type>());
    }
}
