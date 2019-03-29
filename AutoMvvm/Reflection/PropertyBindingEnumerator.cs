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

namespace AutoMvvm.Reflection
{
    /// <summary>
    /// Collects the view model bindings for a view entity using reflection.
    /// </summary>
    public class PropertyBindingEnumerator<T> : IEnumerable<PropertyBinding>
        where T : class
    {
        /// <summary>
        /// Gets the source entity to bind.
        /// </summary>
        public IWithViewModel<T> Source { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBindingEnumerator{T}"/> class.
        /// </summary>
        /// <param name="source">The source entity to bind.</param>
        public PropertyBindingEnumerator(IWithViewModel<T> source)
        {
            Source = source;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<PropertyBinding> GetEnumerator()
        {
            var source = Source;
            var sourceType = source.GetType();
            var viewProperties = sourceType.GetProperties();
            var viewModel = source.GetViewModel();
            var viewModelType = viewModel.GetType();
            var viewModelProperties = viewModelType.GetProperties();
            foreach (var viewControl in viewProperties)
            {
                var controlType = viewControl.PropertyType;
                var defaultProperty = GetDefaultProperty(controlType);
                var controlProperties = controlType.GetProperties();
                foreach (var controlProperty in controlProperties)
                {
                    var viewModelPropertyName = (controlProperty.Name == defaultProperty) ? $"{viewControl.Name}" : $"{viewControl.Name}{controlProperty.Name}";
                    var viewModelProperty = viewModelProperties.FirstOrDefault(vmProperty => vmProperty.Name == viewModelPropertyName);
                    if (viewModelProperty == null)
                        continue;

                    var sourceControl = viewControl.GetValue(source);
                    yield return PropertyBinding.Create(PropertyDetails.Create(sourceControl, controlProperty), PropertyDetails.Create(viewModel, viewModelProperty));
                }
            }
        }

        /// <summary>
        /// Gets the default property for the given control type.
        /// </summary>
        /// <param name="controlType">The type of control to get the default property for.</param>
        /// <returns>The default property for the given control type.</returns>
        private static string GetDefaultProperty(Type controlType)
        {
            // By default it will map default binding properties, followed by default properties.
            var controlAttributes = controlType.GetCustomAttributes(true);
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
