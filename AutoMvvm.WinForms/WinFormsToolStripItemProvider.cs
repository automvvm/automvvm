// --------------------------------------------------------------------------------
// <copyright file="WinFormsToolStripItemProvider.cs" company="AutoMvvm Development Team">
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
using System.Windows.Forms;
using AutoMvvm.Design;
using AutoMvvm.Reflection;

namespace AutoMvvm.WinForms
{
    /// <summary>
    /// Provides common interfaces for accessing the name, parent, and children of a
    /// <see cref="Source"/> <see cref="ToolStripItem"/> as well as asynchronous event routing.
    /// </summary>
    public class WinFormsToolStripItemProvider : WinFormsAsyncEventRouter, ITreeMappingProvider
    {
        private Lazy<bool> _hasInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="WinFormsMenuProvider"/> class.
        /// </summary>
        public WinFormsToolStripItemProvider()
        {
        }

        /// <summary>
        /// Gets or sets the <see cref="ToolStripItem"/> associated with this provider.
        /// </summary>
        public new ToolStripItem Source
        {
            get => (ToolStripItem)base.Source;
            set
            {
                _hasInterface = new Lazy<bool>(() => value?.GetType()?.HasInterfaceLike(typeof(IBinding<>)) == true, true);
                base.Source = value;

                // Add the interface type mappings so the AutoMvvm base methods can access
                // the control properties via the interfaces.
                value.AddTypeMapping<ITreeMappingProvider, WinFormsToolStripItemProvider>();
                value.AddTypeMapping<INameProvider, WinFormsToolStripItemProvider>();
                value.AddTypeMapping<IParentProvider, WinFormsToolStripItemProvider>();
                value.AddTypeMapping<IChildrenProvider, WinFormsToolStripItemProvider>();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ToolStripItem"/> is a binding source.
        /// </summary>
        public bool IsBindingSource => _hasInterface?.Value == true;

        /// <summary>
        /// Gets the name of the <see cref="ToolStripItem"/>.
        /// </summary>
        /// <remarks>
        /// If this <see cref="ToolStripItem"/> is the source entity (i.e. implements <see cref="IBinding{T}"/>) use the event
        /// object name "Source", otherwise use the source <see cref="ToolStripItem"/> name.
        /// </remarks>
        public string Name => IsBindingSource ? "Source" : Source?.Name;

        /// <summary>
        /// Gets the parent of the <see cref="ToolStripItem"/>.
        /// </summary>
        public object Parent => (object)Source.OwnerItem ?? Source?.Owner;

        /// <summary>
        /// Gets the children of this <see cref="ToolStripItem"/>.
        /// </summary>
        public IEnumerable<object> Children => Enumerable.Empty<object>();
    }
}
