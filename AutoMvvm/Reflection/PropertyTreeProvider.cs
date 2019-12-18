// --------------------------------------------------------------------------------
// <copyright file="PropertyTreeProvider.cs" company="AutoMvvm Development Team">
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
using System.Reflection;
using AutoMvvm.Design;

namespace AutoMvvm.Reflection
{
    /// <summary>
    /// A <see cref="ITreeMappingProvider"/> that provides a tree mapping of the
    /// properties of the source entity using reflection.
    /// </summary>
    public class PropertyTreeProvider : ITreeMappingProvider<object>
    {
        private object _source;
        private readonly List<WeakReference<object>> _children = new List<WeakReference<object>>();
        private WeakReference<object> _parent;

        /// <summary>
        /// Gets the source entity.
        /// </summary>
        public object Source
        {
            get => _source;
            set
            {
                // The source can only be assigned once.
                if (Source != null)
                    return;

                _source = value;

                // Add the interface type mappings so the AutoMvvm base methods can access
                // the properties via the interfaces.
                value.AddTypeMapping<ITreeMappingProvider, PropertyTreeProvider>();
                value.AddTypeMapping<INameProvider, PropertyTreeProvider>();
                value.AddTypeMapping<IParentProvider, PropertyTreeProvider>();
                value.AddTypeMapping<IChildrenProvider, PropertyTreeProvider>();

                BuildPropertyTree();
            }
        }

        /// <summary>
        /// Gets the name of the source entity.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// Gets the parent of the source entity.
        /// </summary>
        public object Parent
        {
            get => _parent.TryGetTarget(out var parent) ? parent : null;
            private set => _parent = new WeakReference<object>(value);
        }

        /// <summary>
        /// Gets the collection of child properties.
        /// </summary>
        public IEnumerable<object> Children => _children.Select(child => child.TryGetTarget(out var target) ? target : null)
                                                        .Where(child => child != null)
                                                        .ToList()
                                                        .AsReadOnly();

        private void BuildPropertyTree()
        {
            var source = Source;
            var sourceType = source.GetType();
            var sourceTypeInfo = sourceType.GetTypeInfo();
            var sourceProperties = sourceTypeInfo.GetProperties();
            _children.Clear();
            foreach (var sourceProperty in sourceProperties)
            {
                // If the property for any reason can't be read, is a value or string type, or is an index property, skip it.
                var sourcePropertyTypeInfo = sourceProperty.PropertyType.GetTypeInfo();
                if (!sourceProperty.CanRead || sourcePropertyTypeInfo.IsValueType ||
                    sourceProperty.PropertyType == typeof(string) ||
                    sourceProperty.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                var sourcePropertyValue = sourceProperty.GetValue(source);
                _children.Add(new WeakReference<object>(sourcePropertyValue));
                var treeMappingProvider = sourcePropertyValue.GetTreeMappingProvider(sourceProperty.Name);
                (treeMappingProvider as PropertyTreeProvider).Parent = source;
            }
        }
    }
}
