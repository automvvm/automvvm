// --------------------------------------------------------------------------------
// <copyright file="PropertyTreeProviderExtensions.cs" company="AutoMvvm Development Team">
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

using AutoMvvm.Design;

namespace AutoMvvm.Reflection
{
    /// <summary>
    /// Extension methods to provide a property tree mapped source entity.
    /// </summary>
    public static class PropertyTreeProviderExtensions
    {
        /// <summary>
        /// Gets the <see cref="ITreeMappingProvider"/> for the given source object
        /// and assigns it the given name.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="name">The name of the source object.</param>
        /// <returns>The tree mapping provider.</returns>
        public static ITreeMappingProvider GetTreeMappingProvider(this object source, string name)
        {
            if (source == null)
                return null;

            var provider = source.Get<PropertyTreeProvider>();
            if (provider.Source != null)
                return provider;

            // Assign the name and source.  This builds the property tree.
            provider.Name = name;
            provider.Source = source;
            return provider;
        }
    }
}
