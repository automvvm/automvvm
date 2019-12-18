// --------------------------------------------------------------------------------
// <copyright file="WinFormsSourceProviderExtensions.cs" company="AutoMvvm Development Team">
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

using System.Windows.Forms;
using AutoMvvm.Design;

namespace AutoMvvm.WinForms
{
    /// <summary>
    /// Extension methods for the windows forms property providers.
    /// </summary>
    public static class WinFormsSourceProviderExtensions
    {
        /// <summary>
        /// Gets the <see cref="WinFormsControlProvider"/> for this <see cref="Control"/>.
        /// </summary>
        /// <param name="source">The source control.</param>
        /// <returns>The <see cref="WinFormsControlProvider"/> for this <see cref="Control"/>.</returns>
        public static ITreeMappingProvider GetTreeMappingProvider(this Control source)
        {
            var controlProvider = source.Get<WinFormsControlProvider>();

            // Assign the Control source.
            controlProvider.Source = source;
            return controlProvider;
        }

        /// <summary>
        /// Gets the <see cref="WinFormsMenuProvider"/> for this <see cref="Menu"/>.
        /// </summary>
        /// <param name="source">The source menu.</param>
        /// <returns>The <see cref="WinFormsMenuProvider"/> for this <see cref="Menu"/>.</returns>
        public static ITreeMappingProvider GetTreeMappingProvider(this Menu source)
        {
            var controlProvider = source.Get<WinFormsMenuProvider>();

            // Assign the Menu source.
            controlProvider.Source = source;
            return controlProvider;
        }

        /// <summary>
        /// Gets the <see cref="WinFormsToolStripItemProvider"/> for this <see cref="ToolStripItem"/>.
        /// </summary>
        /// <param name="source">The source menu.</param>
        /// <returns>The <see cref="WinFormsToolStripItemProvider"/> for this <see cref="ToolStripItem"/>.</returns>
        public static ITreeMappingProvider GetTreeMappingProvider(this ToolStripItem source)
        {
            var controlProvider = source.Get<WinFormsToolStripItemProvider>();

            // Assign the ToolStripItem source.
            controlProvider.Source = source;
            return controlProvider;
        }
    }
}
