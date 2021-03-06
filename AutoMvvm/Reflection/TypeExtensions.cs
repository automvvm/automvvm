﻿// --------------------------------------------------------------------------------
// <copyright file="TypeExtensions.cs" company="AutoMvvm Development Team">
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
using System.Linq;

namespace AutoMvvm.Reflection
{
    /// <summary>
    /// Extension methods for handling the creation, caching and instantiation of types
    /// with better performance than <see cref="Activator.CreateInstance(Type)"/>.
    /// </summary>
    public static class TypeExtensions
    {
        private static readonly ConcurrentDictionary<TypeDefinition, Type> _typeCache = new ConcurrentDictionary<TypeDefinition, Type>();

        /// <summary>
        /// Gets the constructed generic type matching the given type parameters.
        /// </summary>
        /// <param name="type">The generic type definition.</param>
        /// <param name="genericTypeParameters">The generic type parameters.</param>
        /// <returns>The constructed generic type matching the given type parameters.</returns>
        public static Type GetGenericType(this Type type, params Type[] genericTypeParameters) =>
            _typeCache.GetOrAdd(new TypeDefinition(type, genericTypeParameters), typeDefinition => typeDefinition.MakeGenericType());

        /// <summary>
        /// Creates a new instance of the given type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <returns>The instance of the given type.</returns>
        /// <remarks>TODO: Stop using Activator.CreateInstance and create a type generator.</remarks>
        public static object CreateInstance(this Type type) =>
            Activator.CreateInstance(type);

        /// <summary>
        /// Creates a new instance of the given type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to create.</param>
        /// <param name="parameters">The constructor parameters.</param>
        /// <returns>The instance of the given type.</returns>
        /// <remarks>TODO: Stop using Activator.CreateInstance and create a type generator.</remarks>
        public static object CreateInstance(this Type type, params object[] parameters) =>
            Activator.CreateInstance(type, parameters);

        /// <summary>
        /// Gets a value indicating whether the source entity has an <see cref="IBinding{T}"/>
        /// interface with the given target entity.
        /// </summary>
        /// <param name="source">The source entity.</param>
        /// <param name="targetType">The target type.</param>
        /// <returns>
        /// A value indicating whether the source entity has an <see cref="IBinding{T}"/> interface
        /// with the given target entity.
        /// </returns>
        public static bool HasTargetType(this object source, Type targetType)
        {
            var sourceType = source.GetType();
            return typeof(IBinding<>).MakeGenericType(targetType).IsAssignableFrom(sourceType);
        }

        /// <summary>
        /// Determines if this type has an interface that matches the given match interface type.
        /// </summary>
        /// <param name="type">The type to match interfaces for.</param>
        /// <param name="matchInterfaceType">The interface type to match.</param>
        /// <returns><c>true</c> if this type has an interface that matches the given match interface type.</returns>
        public static bool HasInterfaceLike(this Type type, Type matchInterfaceType) =>
            GetInterfacesLike(type, matchInterfaceType).FirstOrDefault() != null;

        /// <summary>
        /// Gets a collection of interfaces that match the given match interface type.
        /// </summary>
        /// <param name="type">The type to match interfaces for.</param>
        /// <param name="matchInterfaceType">The interface type to match.</param>
        /// <returns>A collection of interfaces that match the given match interface type.</returns>
        public static IEnumerable<Type> GetInterfacesLike(this Type type, Type matchInterfaceType)
        {
            var interfaceTypes = type.GetInterfaces();
            foreach (var interfaceType in interfaceTypes)
            {
                if (interfaceType != matchInterfaceType &&
                    (!interfaceType.IsGenericType ||
                    !matchInterfaceType.IsGenericTypeDefinition ||
                    interfaceType.GetGenericTypeDefinition() != matchInterfaceType))
                {
                    continue;
                }

                yield return interfaceType;
            }
        }
    }
}
