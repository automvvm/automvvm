// --------------------------------------------------------------------------------
// <copyright file="PropertyBindingEnumerator.cs" company="AutoMvvm Development Team">
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using AutoMvvm.Design;

namespace AutoMvvm.Reflection
{
    /// <summary>
    /// Collects the target property bindings for a source entity using reflection.
    /// </summary>
    public class PropertyBindingEnumerator : IEnumerable<PropertyBinding>
    {
        /// <summary>
        /// Gets the source entity to bind.
        /// </summary>
        public object Source { get; }

        /// <summary>
        /// Gets the target entity to bind to.
        /// </summary>
        public object Target { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBindingEnumerator"/> class.
        /// </summary>
        /// <param name="source">The source entity to bind.</param>
        public PropertyBindingEnumerator(object source, object target)
        {
            Source = source;
            Target = target;
        }

        /// <summary>
        /// Gets a property binding enumerator for the given source entity.
        /// </summary>
        /// <param name="source">The source entity to bind.</param>
        /// <param name="target">The target entity to bind to.</param>
        /// <returns>A property binding enumerator.</returns>
        public static PropertyBindingEnumerator For(object source, object target) =>
            new PropertyBindingEnumerator(source, target);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<PropertyBinding> GetEnumerator()
        {
            var source = Source;
            var target = Target;
            var sourcePropertyBindings = GetPropertyBindings(source, target);
            foreach (var propertyBinding in sourcePropertyBindings)
                yield return propertyBinding;
        }

        /// <summary>
        /// Gets a collection of property bindings from the source to the target.
        /// </summary>
        /// <param name="source">The source entity.</param>
        /// <param name="target">The target entity.</param>
        /// <returns>A collection of property bindings.</returns>
        private static IEnumerable<PropertyBinding> GetPropertyBindings(object source, object target)
        {
            var targetType = target.GetType();
            var targetProperties = targetType.GetProperties();
            var sourceName = source.GetName();
            var sourceType = source.GetType();
            var defaultProperty = GetDefaultProperty(sourceType);
            var childProperties = sourceType.GetProperties();
            foreach (var childProperty in childProperties)
            {
                var targetPropertyName = (childProperty.Name == defaultProperty) ? $"{sourceName}" : $"{sourceName}{childProperty.Name}";
                var foundTargetProperty = targetProperties.FirstOrDefault(targetProperty => targetProperty.Name == targetPropertyName);
                if (foundTargetProperty == null)
                    continue;

                yield return PropertyBinding.Create(PropertyDetails.Create(source, childProperty), PropertyDetails.Create(target, foundTargetProperty));
            }
        }

        /// <summary>
        /// Gets the default property for the given source entity type.
        /// </summary>
        /// <param name="sourceType">The type of the source entity to get the default property for.</param>
        /// <returns>The default property for the given source entity type.</returns>
        private static string GetDefaultProperty(Type sourceType)
        {
            // By default it will map default binding properties, followed by default properties.
            var controlAttributes = sourceType.GetCustomAttributes(true);
            var defaultBindingProperty = controlAttributes.OfType<DefaultBindingPropertyAttribute>()
                                                          .FirstOrDefault();
            if (defaultBindingProperty != null)
                return defaultBindingProperty.Name;

            var defaultProperty = controlAttributes.OfType<DefaultPropertyAttribute>()
                                                   .FirstOrDefault();
            if (defaultProperty != null)
                return defaultProperty.Name;

            return string.Empty;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
