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
using AutoMvvm.Design;
using AutoMvvm.Fluent;
using AutoMvvm.Reflection;

namespace AutoMvvm.WinForms
{
    /// <summary>
    /// Extension methods for windows forms binding.
    /// </summary>
    public static class WinFormsBindingExtensions
    {
        /// <summary>
        /// Binds all properties and events by convention for the given source entity with its target.
        /// This binds the properties of the target which are named the same as the source properties
        /// and event methods named [sourcePropertyName][eventName] or Source[eventName] for events
        /// directly on the source object.
        /// <example>
        /// <para>
        /// If the source property name is "TestProperty" and the class property name is "Text", the binding will be
        /// to the target property called "TestPropertyText" or "TestProperty" if the "Text"
        /// class property is defined by the <see cref="DefaultBindingPropertyAttribute"/> or the
        /// <see cref="DefaultPropertyAttribute"/> in that class.
        /// </para>
        /// <para>
        /// Warning: If both default and explicit properties exist in the target it will only bind to the default
        /// property name to prevent the properties from getting out of sync.  This is subject to change, so don't
        /// rely on this behavior.
        /// </para>
        /// <para>
        /// If the source property name is "TestProperty" and the event name is "Click", the binding will be
        /// to the method in the target called "TestPropertyClick".
        /// </para>
        /// </example>
        /// </summary>
        /// <typeparam name="T">The target binding type.</typeparam>
        /// <param name="source">The source entity to bind.</param>
        public static void BindByConvention<T>(this IBinding<T> source)
            where T : class
        {
            source.BindPropertiesByConvention();
            source.BindEventsByConvention();
        }

        /// <summary>
        /// Binds all properties by convention for the given source entity with its target.
        /// This binds the properties of the target which are named the same as the source properties.
        /// <example>
        /// <para>
        /// If the source property name is "TestProperty" and the class property name is "Text", the binding will be
        /// to the target property called "TestPropertyText" or "TestProperty" if the "Text"
        /// class property is defined by the <see cref="DefaultBindingPropertyAttribute"/> or the
        /// <see cref="DefaultPropertyAttribute"/> in that class.
        /// </para>
        /// <para>
        /// Warning: If both default and explicit properties exist in the target it will only bind to the default
        /// property name to prevent the properties from getting out of sync.  This is subject to change, so don't
        /// rely on this behavior.
        /// </para>
        /// </example>
        /// </summary>
        /// <typeparam name="T">The target binding type.</typeparam>
        /// <param name="source"></param>
        public static void BindPropertiesByConvention<T>(this IBinding<T> source)
            where T : class
        {
            (source as Control)?.GetTreeMappingProvider();
            var target = source.GetTarget();
            var propertyBindings = source.GetPropertyBindings(target);
            source.AddPropertyBindings(propertyBindings);
            foreach (var sourceChild in source.GetAllChildren())
            {
                var childPropertyBindings = sourceChild.GetPropertyBindings(target);
                source.AddPropertyBindings(childPropertyBindings);
            }
        }

        /// <summary>
        /// Binds all events by convention for the given source entity with its target.
        /// This binds the event methods named [sourcePropertyName][eventName] or Source[eventName] for events
        /// directly on the source object.
        /// <example>
        /// <para>
        /// If the source property name is "TestProperty" and the event name is "Click", the binding will be
        /// to the method in the target called "TestPropertyClick".
        /// </para>
        /// </example>
        /// </summary>
        /// <typeparam name="T">The target binding type.</typeparam>
        /// <param name="source">The source entity to bind.</param>
        public static void BindEventsByConvention<T>(this IBinding<T> source)
            where T : class
        {
            var target = source.GetTarget();
            (source as Control)?.GetTreeMappingProvider();
            source.HookAllEvents();
            source.AddEventBindings(source.GetEventBindings(target));

            var sourceChildren = source.Get<IChildrenProvider>()?.Children;
            foreach (var sourceChild in sourceChildren)
            {
                (sourceChild as Control)?.GetTreeMappingProvider();
                sourceChild.HookAllEvents();
                sourceChild.AddEventBindings(sourceChild.GetEventBindings(target));
            }
        }
    }
}
