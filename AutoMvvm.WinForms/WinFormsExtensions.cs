// --------------------------------------------------------------------------------
// <copyright file="WinformsExtensions.cs" company="AutoMvvm Development Team">
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
using System.Windows.Forms;
using AutoMvvm.Fluent;

namespace AutoMvvm.WinForms
{
    /// <summary>
    /// Extension methods for windows forms binding.
    /// </summary>
    public static class WinFormsExtensions
    {
        /// <summary>
        /// Binds all properties and events by convention for the given source entity with its view model.
        /// This binds the properties which are named the same as the view controls and event methods
        /// named [controlName][eventName].
        /// <example>
        /// <para>
        /// If the control name is "TestControl" and the property name is "Text", the binding will be
        /// to the property in the view model called "TestControlText" or "TestControl" if the "Text"
        /// property is defined by the <see cref="DefaultBindingPropertyAttribute"/> or the
        /// <see cref="DefaultPropertyAttribute"/> for that control.  If both properties exist it
        /// will only bind the default property to avoid the properties getting out of sync.
        /// </para>
        /// <para>
        /// If the control name is "TestControl" and the event name is "Click", the binding will be
        /// to the method in the view model called "TestControlClick".
        /// </para>
        /// </example>
        /// </summary>
        /// <param name="source">The source view to bind.</param>
        public static void BindByConvention<T>(this IWithViewModel<T> source)
            where T : class
        {
            //TODO: Add automatic binding support for view.
            throw new NotImplementedException("Need to implement binding support.");
        }

        public static IPredicateBuilder<ComboBox> When(this ComboBox source, Func<ComboBox, bool> predicate)
        {
            return new PredicateBuilder<ComboBox>(source, predicate);
        }

    }
}
