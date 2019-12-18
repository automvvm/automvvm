// --------------------------------------------------------------------------------
// <copyright file="ViewModelExtensions.cs" company="AutoMvvm Development Team">
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
using AutoMvvm.Design;
using AutoMvvm.Fluent;

namespace AutoMvvm
{
    /// <summary>
    /// Extension methods for handling the <see cref="IBinding{T}"/> interface.
    /// </summary>
    public static class BindingExtensions
    {
        /// <summary>
        /// Gets the bound target instance.
        /// </summary>
        /// <typeparam name="T">The bound target type.</typeparam>
        /// <param name="source">The source.</param>
        /// <returns>The bound target instance.</returns>
        public static T GetTarget<T>(this IBinding<T> source)
            where T : class
        {
            return source.Get<T>();
        }

        /// <summary>
        /// Defines a predicated binding to an action.
        /// </summary>
        /// <param name="source">The source entity to bind.</param>
        /// <param name="predicate">The predicate defining when to perform the action.</param>
        /// <returns>A predicate builder fluent interface.</returns>
        public static IPredicateBuilder<T> When<T>(this T source, Func<T, bool> predicate)
            where T : class
        {
            return new PredicateBuilder<T>(source, predicate);
        }
    }
}
