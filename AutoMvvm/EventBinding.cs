﻿// --------------------------------------------------------------------------------
// <copyright file="EventBinding.cs" company="AutoMvvm Development Team">
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
    /// Defines an event binding.
    /// </summary>
    public class EventBinding
    {
        /// <summary>
        /// Gets the target of the bound event if still available, otherwise <c>null</c>.
        /// </summary>
        public object Target => Action.Target;

        /// <summary>
        /// Indicates whether this event binding is still valid.
        /// </summary>
        public bool IsValid => Action.IsValid;

        /// <summary>
        /// Gets the event bound.
        /// </summary>
        public Event Event { get; }

        /// <summary>
        /// Gets the action to perform for the event.
        /// </summary>
        public IWeakAction<ReceivedEvent> Action { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBinding"/> class.
        /// </summary>
        /// <param name="event">The event bound.</param>
        /// <param name="action">The action to perform for the event along with the target.</param>
        public EventBinding(Event @event, Action<ReceivedEvent> action)
            : this(@event, (WeakAction<ReceivedEvent>)action)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBinding"/> class.
        /// </summary>
        /// <param name="event">The event bound.</param>
        /// <param name="action">The weak action to perform for the event.</param>
        public EventBinding(Event @event, IWeakAction<ReceivedEvent> action)
        {
            if (string.IsNullOrEmpty(@event.SourceName) || string.IsNullOrEmpty(@event.TargetMethodName))
                throw new ArgumentOutOfRangeException(nameof(@event), "The event provided must specify a source entity name and provide a non-empty target method name.");

            Event = @event;
            Action = action;
        }

        /// <summary>
        /// Executes the event handler with the given event source and <typeparamref name="TEventArgs"/>.
        /// </summary>
        /// <param name="e">The <typeparamref name="TEventArgs"/> details.</param>
        public void HandleEvent<TEventArgs>(object source, TEventArgs e)
            where TEventArgs : EventArgs
        {
            HandleEvent(ReceivedEvent.From(source, Event, e));
        }

        /// <summary>
        /// Executes the event handler with the given <see cref="ReceivedEvent"/> details.
        /// </summary>
        /// <param name="e">The <see cref="ReceivedEvent"/> details.</param>
        public void HandleEvent(ReceivedEvent e) => Action?.Invoke(e);
    }
}
