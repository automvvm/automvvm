// --------------------------------------------------------------------------------
// <copyright file="EventExtensions.cs" company="AutoMvvm Development Team">
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
using AutoMvvm.Design;

namespace AutoMvvm.Fluent
{
    /// <summary>
    /// Extensions methods for queuing and handling events.
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// Binds the given event.
        /// </summary>
        /// <param name="source">The source to bind.</param>
        /// <param name="event">The event to bind to.</param>
        /// <returns>An event binding builder.</returns>
        public static void BindEvent<T>(this IBinding<T> source, Event @event, Action<ReceivedEvent> action)
            where T : class
        {
            new EventBuilder<T>(source, @event).Bind(action);
        }

        /// <summary>
        /// Adds all the event bindings in the collection to the binding store for the given source entity.
        /// </summary>
        /// <param name="source">The source entity to bind.</param>
        /// <param name="eventBindings">The event bindings to add.</param>
        public static void AddEventBindings(this object source, IEnumerable<EventBinding> eventBindings)
        {
            var eventBindingStore = source.Get<EventBindingStore>();
            foreach (var eventBinding in eventBindings)
                eventBindingStore.EventBindings.Add(eventBinding);
        }

        /// <summary>
        /// Adds an event binding to the binding store for the given source entity.
        /// </summary>
        /// <param name="source">The source entity to bind.</param>
        /// <param name="eventBinding">The event binding to add.</param>
        public static void AddEventBinding(this object source, EventBinding eventBinding)
        {
            var eventBindingStore = source.Get<EventBindingStore>();
            eventBindingStore.EventBindings.Add(eventBinding);
        }

        /// <summary>
        /// Sends an event for the source object.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="event">The event to send.</param>
        /// <param name="e">The event arguments</param>
        public static void SendEvent<TEventArgs>(this object source, Event @event, TEventArgs e)
            where TEventArgs : EventArgs
        {
            source.SendEventAsync(@event, e);
            source.RouteEvents();
        }

        /// <summary>
        /// Sends an event for the source object asynchronously which will be picked up on the
        /// event queue at the next synchronous event or asynchronous timer event.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="event">The event to send.</param>
        /// <param name="e">The event arguments</param>
        public static void SendEventAsync<TEventArgs>(this object source, Event @event, TEventArgs e)
            where TEventArgs : EventArgs
        {
            var events = source.Get<ReceivedEventQueue>();
            events.ReceivedEvents.Enqueue(ReceivedEvent.From(source, @event, e));
        }

        /// <summary>
        /// Routes the received events to the bound event handlers.
        /// </summary>
        /// <param name="source">The source of the events.</param>
        public static void RouteEvents(this object source)
        {
            var receivedEvents = source.Get<ReceivedEventQueue>().ReceivedEvents;
            if (receivedEvents.Count == 0)
                return;

            var eventBindings = source.Get<EventBindingStore>().EventBindings;
            var lookup = eventBindings.ToLookup(eventBinding => eventBinding.Event);
            while (receivedEvents.Count > 0)
            {
                var receivedEvent = receivedEvents.Dequeue();
                if (!lookup.Contains(receivedEvent.Event))
                    continue;

                foreach (var eventBinding in lookup[receivedEvent.Event])
                    eventBinding.HandleEvent(receivedEvent);
            }
        }
    }
}
