// --------------------------------------------------------------------------------
// <copyright file="PropertyExtensions.cs" company="AutoMvvm Development Team">
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

using System.Collections.Generic;
using AutoMvvm.Design;

namespace AutoMvvm.Fluent
{
    /// <summary>
    /// Extension methods for property bindings.
    /// </summary>
    public static class PropertyExtensions
    {
        /// <summary>
        /// Adds all the property bindings in the collection to the binding store for the given source entity.
        /// </summary>
        /// <param name="source">The source entity to bind.</param>
        /// <param name="propertyBindings">The property bindings to add.</param>
        public static void AddPropertyBindings(this object source, IEnumerable<PropertyBinding> propertyBindings)
        {
            var propertyBindingStore = source.Get<PropertyBindingStore>();
            foreach (var propertyBinding in propertyBindings)
                propertyBindingStore.PropertyBindings.Add(propertyBinding);
        }

        /// <summary>
        /// Adds an property binding to the binding store for the given source entity.
        /// </summary>
        /// <param name="source">The source entity to bind.</param>
        /// <param name="propertyBinding">The property binding to add.</param>
        public static void AddEventBinding(this object source, PropertyBinding propertyBinding)
        {
            var propertyBindingStore = source.Get<PropertyBindingStore>();
            propertyBindingStore.PropertyBindings.Add(propertyBinding);
        }
    }
}
