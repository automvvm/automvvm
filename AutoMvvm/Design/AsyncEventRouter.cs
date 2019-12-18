// --------------------------------------------------------------------------------
// <copyright file="AsyncEventRouter.cs" company="AutoMvvm Development Team">
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
using AutoMvvm.Fluent;

namespace AutoMvvm.Design
{
    /// <summary>
    /// The base class for asynchronous event routing.
    /// </summary>
    public abstract class AsyncEventRouter
    {
        private static readonly IList<AsyncEventRouter> _eventRouters = new List<AsyncEventRouter>();
        private WeakReference<object> _sourceWeakReference;

        /// <summary>
        /// Gets the event source entity if it is available.
        /// </summary>
        public object Source
        {
            get
            {
                object source = null;
                return (_sourceWeakReference?.TryGetTarget(out source) == true) ? source : null;
            }

            set
            {
                if (_sourceWeakReference != null)
                    throw new InvalidOperationException("Cannot bind a source entity more than once.");

                _sourceWeakReference = new WeakReference<object>(value);
                _eventRouters.Add(this);
            }
        }

        /// <summary>
        /// Routes the queued events for all instances of <see cref="AsyncEventRouter"/>.
        /// </summary>
        protected static void RouteEvents()
        {
            // Copy the list, because event routers with source entities that are null must 
            // be deleted from the event routers list.
            var eventRouters = new List<AsyncEventRouter>(_eventRouters);
            foreach (var eventRouter in eventRouters)
            {
                // If the source entity hasn't been assigned yet;
                if (eventRouter._sourceWeakReference == null)
                    continue;

                // If the source entity is null, remove this event router.
                var source = eventRouter.Source;
                if (source == null)
                {
                    _eventRouters.Remove(eventRouter);
                    continue;
                }

                source.RouteEvents();
            }
        }
    }
}
