// --------------------------------------------------------------------------------
// <copyright file="EventBuilder.cs" company="AutoMvvm Development Team">
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
using AutoMvvm.Design;

namespace AutoMvvm.Fluent
{
    /// <summary>
    /// The fluent class for building an event binding.
    /// </summary>
    internal class EventBuilder : IEventBuilder
    {
        /// <summary>
        /// Gets the source of the event.
        /// </summary>
        private object Source { get; }

        /// <summary>
        /// Gets the event to bind.
        /// </summary>
        private Event Event { get; }

        /// <summary>
        /// Initializes a new instance of the 
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="event">The event to bind.</param>
        public EventBuilder(object source, Event @event)
        {
            Source = source;
            Event = @event;
        }

        /// <summary>
        /// Binds the given action to this event.
        /// </summary>
        /// <param name="action">The action to bind.</param>
        public void Bind(Action<ReceivedEvent> action)
        {
            var eventStore = Source.Get<EventBindingStore>();
            eventStore.EventBindings.Add(new EventBinding(Event, action));
        }
    }
}