// --------------------------------------------------------------------------------
// <copyright file="SourcePropertyProviderExtensions.cs" company="AutoMvvm Development Team">
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
using System.Collections.Generic;
using System.Linq;
using AutoMvvm.Reflection;

namespace AutoMvvm.Design
{
    /// <summary>
    /// Extension methods for quick access to the properties of the property providers.
    /// </summary>
    public static class SourcePropertyProviderExtensions
    {
        /// <summary>
        /// Gets the name of this entity.
        /// </summary>
        /// <param name="source">The source entity.</param>
        /// <returns>The name of this object.</returns>
        /// <exception cref="InvalidOperationException">if no <see cref="INameProvider"/> exists for this entity.</exception>
        public static string GetName(this object source) =>
            source.Get<INameProvider>()?.Name;

        /// <summary>
        /// Gets the parent of this entity.
        /// </summary>
        /// <param name="source">The source entity.</param>
        /// <returns>The parent of this object.</returns>
        /// <exception cref="InvalidOperationException">if no <see cref="IParentProvider"/> exists for this entity.</exception>
        public static IEnumerable<object> GetAllParents(this object source) =>
            source.GetParent().ToEnumerable(parent => parent.GetParent());

        /// <summary>
        /// Gets the parent of this entity.
        /// </summary>
        /// <param name="source">The source entity.</param>
        /// <returns>The parent of this object.</returns>
        /// <exception cref="InvalidOperationException">if no <see cref="IParentProvider"/> exists for this entity.</exception>
        public static object GetParent(this object source) =>
            source.Get<IParentProvider>()?.Parent;

        /// <summary>
        /// Gets the children of this entity.
        /// </summary>
        /// <param name="source">The source entity.</param>
        /// <returns>The children of this object.</returns>
        /// <exception cref="InvalidOperationException">if no <see cref="IChildrenProvider"/> exists for this entity.</exception>
        public static IEnumerable<object> GetChildren(this object source) =>
            source.Get<IChildrenProvider>()?.Children;

        /// <summary>
        /// Gets the children of this entity.
        /// </summary>
        /// <param name="source">The source entity.</param>
        /// <returns>The children of this object.</returns>
        /// <exception cref="InvalidOperationException">if no <see cref="IChildrenProvider"/> exists for this entity.</exception>
        public static IEnumerable<object> GetAllChildren(this object source) =>
            source.GetChildren().SelectMany(child => child.GetChildren());

        /// <summary>
        /// Gets the <see cref="IBinding{T}"/> parent of the given source entity.
        /// </summary>
        /// <param name="source">The source entity.</param>
        /// <param name="targetType">The target type.</param>
        /// <returns>The <see cref="IBinding{T}"/> parent of the given source entity if found, otherwise <c>null</c>.</returns>
        public static object GetBindableParent(this object source, Type targetType)
        {
            // Search through the all parents until we find a binding parent.
            return source?.GetAllParents()?.FirstOrDefault(parent => parent.HasTargetType(targetType));
        }

        private static IEnumerable<T> ToEnumerable<T>(this T item, Func<T, T> nextItem)
            where T : class
        {
            while (item != null)
            {
                yield return item;
                item = nextItem?.Invoke(item);
            }
        }
    }
}
