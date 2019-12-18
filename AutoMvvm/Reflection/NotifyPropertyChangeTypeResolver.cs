// --------------------------------------------------------------------------------
// <copyright file="NotifyPropertyChangeTypeResolver.cs" company="AutoMvvm Development Team">
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
using System.ComponentModel;

namespace AutoMvvm.Reflection
{
    /// <summary>
    /// Builds a type resolver wrapper around any given factory to wrap
    /// notifications around the generated types so the changes can be hooked.
    /// </summary>
    /// <remarks>
    /// The only requirement is to make your properties virtual and class
    /// able to be inherited and you can hook any property change or method
    /// call.  It binds to and exposes all property changes through the
    /// <see cref="INotifyPropertyChanged"/> and <see cref="INotifyPropertyChanging"/>
    /// interfaces.
    /// </remarks>
    public class NotifyPropertyChangeTypeResolver : ITypeResolver
    {
        /// <summary>
        /// Gets the type resolver to wrap.
        /// </summary>
        protected ITypeResolver Resolver { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyPropertyChangeTypeResolver"/>
        /// </summary>
        /// <param name="resolver">The type resolver method to wrap.</param>
        public NotifyPropertyChangeTypeResolver(Func<Type, Type> resolver)
            : this(new FuncTypeResolver(resolver))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotifyPropertyChangeTypeResolver"/>
        /// </summary>
        /// <param name="resolver">The type resolver to wrap.</param>
        public NotifyPropertyChangeTypeResolver(ITypeResolver resolver)
        {
            Resolver = resolver;
        }

        /// <summary>
        /// Resolves a concrete type for the given type <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The type to resolve.</param>
        /// <returns>A type reference to the concrete type satisfying the request.</returns>
        public Type ResolveType(Type type)
        {
            var resolvedType = Resolver.ResolveType(type);
            return resolvedType.GetNotificationWrappedType() ?? resolvedType;
        }
    }
}
