// --------------------------------------------------------------------------------
// <copyright file="PredcateBuilder.cs" company="AutoMvvm Development Team">
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

namespace AutoMvvm.Fluent
{
    /// <summary>
    /// A fluent interface predicate builder.
    /// </summary>
    /// <typeparam name="T">The type to build the predicate for.</typeparam>
    public class PredicateBuilder<T> : IPredicateBuilder<T>
        where T : class
    {
        /// <summary>
        /// Gets the source object to bind to.
        /// </summary>
        protected object Source { get; }

        /// <summary>
        /// Gets the predicate.
        /// </summary>
        protected Func<T, bool> Predicate { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PredicateBuilder{T}"/> class.
        /// </summary>
        /// <param name="source">The source object to bind to.</param>
        /// <param name="predicate">The predicate.</param>
        public PredicateBuilder(object source, Func<T, bool> predicate)
        {
            Source = source;
            Predicate = predicate;
        }

        /// <summary>
        /// Does the given action on the entity.
        /// </summary>
        /// <param name="action">The action to perform.</param>
        public void Do(Action<T> action)
        {
            var predicateStore = Source.Get<PredicateStore<T>>();
            predicateStore.Actions.Add(new PredicatedAction<T>(Predicate, action));
        }
    }
}