// --------------------------------------------------------------------------------
// <copyright file="ReceivedEvent.cs" company="AutoMvvm Development Team">
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

namespace AutoMvvm
{
    /// <summary>
    /// An event that was received.
    /// </summary>
    public class ReceivedEvent
    {
        /// <summary>
        /// Gets the source of the event.
        /// </summary>
        public object Source { get; }

        /// <summary>
        /// Gets the event that was received.
        /// </summary>
        public Event Event { get; }

        /// <summary>
        /// Gets the event arguments.
        /// </summary>
        public EventArgs e { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedEvent"/> class.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="event">The event that was received.</param>
        /// <param name="e">The event arguments.</param>
        public ReceivedEvent(object source, Event @event, EventArgs e)
        {
            Source = source;
            Event = @event;
            this.e = e;
        }
    }

    /// <summary>
    /// An event that was received with event arguments of type <typeparamref name="TEventArgs"/>.
    /// </summary>
    /// <typeparam name="TEventArgs">The type of the event arguments.</typeparam>
    public class ReceivedEvent<TEventArgs> : ReceivedEvent where TEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the event arguments.
        /// </summary>
        public new TEventArgs e => (TEventArgs)base.e;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedEvent{T}"/> class.
        /// </summary>
        /// <param name="source">The source of the event.</param>
        /// <param name="event">The event that was received.</param>
        /// <param name="e">The event arguments.</param>
        public ReceivedEvent(object source, Event @event, TEventArgs e)
            : base(source, @event, e)
        {
        }
    }
}
