// --------------------------------------------------------------------------------
// <copyright file="EventBindingEnumerator.cs" company="AutoMvvm Development Team">
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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using AutoMvvm.Design;

namespace AutoMvvm.Reflection
{
    /// <summary>
    /// Collects the event bindings for a source and target entity pair using reflection.
    /// </summary>
    public class EventBindingEnumerator : IEnumerable<EventBinding>
    {
        /// <summary>
        /// Gets the source entity to bind.
        /// </summary>
        public object Source { get; }

        /// <summary>
        /// Gets the target entity to bind to.
        /// </summary>
        public object Target { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBindingEnumerator"/> class.
        /// </summary>
        /// <param name="source">The source entity to bind.</param>
        /// <param name="target">The target entity to bind to.</param>
        public EventBindingEnumerator(object source, object target)
        {
            Source = source;
            Target = target;
        }

        /// <summary>
        /// Gets the event bindings for the given source and target entity pair.
        /// </summary>
        /// <param name="source">The source entity to bind.</param>
        /// <param name="target">The target entity to bind to.</param>
        /// <returns>An event binding enumerator.</returns>
        public static EventBindingEnumerator For(object source, object target) =>
            new EventBindingEnumerator(source, target);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<EventBinding> GetEnumerator()
        {
            var source = Source;
            var target = Target;
            var sourceType = source.GetType();
            foreach (var eventBinding in GetEventBindings(sourceType, source.GetName(), target))
                yield return eventBinding;
        }

        /// <summary>
        /// Gets the event bindings for a given type and source name that match
        /// methods in the target type.
        /// </summary>
        /// <param name="sourceType">The source object type with events to bind</param>
        /// <param name="sourceName">The source name expected to be used in the event method.</param>
        /// <param name="target">The target to bind.</param>
        /// <returns>An enumerable list of event bindings.</returns>
        private static IEnumerable<EventBinding> GetEventBindings(Type sourceType, string sourceName, object target)
        {
            var propertyEvents = sourceType.GetRuntimeEvents();
            foreach (var propertyEvent in propertyEvents)
            {
                // TODO: Add event name prefix handling.
                var eventBinding = GetEventBinding(target, new Event(sourceName, propertyEvent.Name));
                if (eventBinding == null)
                    continue;

                yield return eventBinding;
            }
        }

        private static EventBinding GetEventBinding(object target, Event @event)
        {
            var targetType = target.GetType();
            var targetEventMethod = targetType.GetAction<ReceivedEvent>(@event.TargetMethodName);
            if (targetEventMethod == null)
                return null;

            return new EventBinding(@event, new WeakAction<ReceivedEvent>(target, targetEventMethod));
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
