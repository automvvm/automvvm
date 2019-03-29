// --------------------------------------------------------------------------------
// <copyright file="FuncTypeResolver.cs" company="AutoMvvm Development Team">
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

namespace AutoMvvm
{
    /// <summary>
    /// Defines a default type resolver from a <see cref="Func{Type, Type}"/> type mapping method.
    /// </summary>
    public class FuncTypeResolver : ITypeResolver
    {
        /// <summary>
        /// Gets the type resolver method.
        /// </summary>
        public Func<Type, Type> Resolver { get; }

        /// <summary>
        /// Initializes a new instance of the 
        /// </summary>
        /// <param name="resolver">The type resolver method.</param>
        public FuncTypeResolver(Func<Type, Type> resolver)
        {
            Resolver = resolver;
        }

        /// <summary>
        /// Resolves a concrete type for the given type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to resolve.</param>
        /// <returns>A type reference to the concrete type satisfying the request.</returns>
        public Type ResolveType(Type type) => Resolver?.Invoke(type);
    }
}
