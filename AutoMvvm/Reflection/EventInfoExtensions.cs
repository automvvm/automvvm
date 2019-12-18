// --------------------------------------------------------------------------------
// <copyright file="EventInfoExtensions.cs" company="AutoMvvm Development Team">
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
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Concurrent;
using AutoMvvm.Design;
using AutoMvvm.Fluent;
using System.Linq;
using System.Collections.Generic;

namespace AutoMvvm.Reflection
{
    /// <summary>
    /// Extension methods for <see cref="EventInfo"/> event subscribe,
    /// unsubscribe delegate and event hook generation.
    /// </summary>
    public static class EventInfoExtensions
    {
        /// <summary>
        /// The collection of cached SendEvent generators.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Func<Event, Delegate>> _eventHandlerGenerators = new ConcurrentDictionary<Type, Func<Event, Delegate>>();

        /// <summary>
        /// The collection of cached <see cref="Event"/> handlers.
        /// </summary>
        private static readonly ConcurrentDictionary<Event, Delegate> _cachedEventHandlers = new ConcurrentDictionary<Event, Delegate>();

        /// <summary>
        /// The generic method definition for the <see cref="EventExtensions.SendEvent{TEventArgs}(object, Event, TEventArgs)"/> method.
        /// </summary>
        private static readonly MethodInfo _genericSendEventMethod = ((Action<object, Event, EventArgs>)EventExtensions.SendEvent).Method.GetGenericMethodDefinition();

        /// <summary>
        /// Gets a collection of event bindings for the given source and target entity pair.
        /// </summary>
        /// <param name="source">The source entity to bind.</param>
        /// <param name="target">The target entity to bind to.</param>
        /// <returns>A collection of event bindings.</returns>
        public static IEnumerable<EventBinding> GetEventBindings(this object source, object target) =>
            EventBindingEnumerator.For(source, target);

        /// <summary>
        /// Gets a collection of property bindings for the given source entity.
        /// </summary>
        /// <param name="source">The source entity to bind.</param>
        /// <param name="target">The target entity to bind to.</param>
        /// <returns>A collection of property bindings.</returns>
        public static IEnumerable<PropertyBinding> GetPropertyBindings(this object source, object target) =>
            PropertyBindingEnumerator.For(source, target);

        /// <summary>
        /// Hooks all events on the given source entity.
        /// </summary>
        /// <param name="source">The source entity with events to hook.</param>
        /// <param name="sourceName">The name of the source entity.</param>
        public static void HookAllEvents(this object source)
        {
            var sourceType = source.GetType();
            var sourceEvents = sourceType.GetTypeInfo().GetEvents();
            foreach (var sourceEventInfo in sourceEvents)
            {
                var sourceName = source.Get<INameProvider>()?.Name;
                var @event = new Event(sourceName, sourceEventInfo.Name);
                source.HookEvent(sourceEventInfo, @event);
            }
        }

        /// <summary>
        /// Hooks the given event for automatic handling.
        /// </summary>
        /// <param name="source">The source to hook events for.</param>
        /// <param name="eventInfo">The event information.</param>
        /// <param name="event">The event to hook.</param>
        public static bool HookEvent(this object source, EventInfo eventInfo, Event @event)
        {
            var eventHandler = GetEventHandler(eventInfo, @event);

            // Get the generic delegate event binding methods.
            var removeEvent = eventInfo.GetRemoveEventAction();
            var addEvent = eventInfo.GetAddEventAction();
            if (addEvent == null || removeEvent == null)
                return false;

            // Only bind event handlers once, so unbind first just in case.
            removeEvent(source, eventHandler);
            addEvent(source, eventHandler);
            return true;
        }

        private static Delegate GetEventHandler(EventInfo eventInfo, Event @event)
        {
            var eventHandlerGenerator = GetEventHandlerGenerator(eventInfo);
            if (eventHandlerGenerator == null)
                return null;

            return _cachedEventHandlers.GetOrAdd(@event, eventHandlerGenerator);
        }

        private static Func<Event, Delegate> GetEventHandlerGenerator(EventInfo eventInfo)
        {
            var eventHandlerType = eventInfo.EventHandlerType;
            return _eventHandlerGenerators.GetOrAdd(
                eventHandlerType,
                type =>
                {
                    // Find the EventArgs parameter type.
                    var raiseMethod = eventHandlerType.GetMethod("Invoke");
                    var raiseMethodParameters = raiseMethod.GetParameters();
                    var eventArgsType = raiseMethodParameters.Select(raiseMethodParameter => raiseMethodParameter.ParameterType)
                                                             .FirstOrDefault(raiseMethodParameterType => typeof(EventArgs).IsAssignableFrom(raiseMethodParameterType));
                    if (eventArgsType == null)
                        return null;

                    var eventParameter = Expression.Parameter(typeof(Event));
                    var senderParameter = Expression.Parameter(typeof(object));
                    var eventArgsParameter = Expression.Parameter(eventArgsType);
                    var sendEventMethod = _genericSendEventMethod.MakeGenericMethod(eventArgsType);
                    var sendEventExpression = Expression.Call(sendEventMethod, senderParameter, eventParameter, eventArgsParameter);
                    var lambdaExpression = Expression.Convert(Expression.Lambda(eventHandlerType, sendEventExpression, senderParameter, eventArgsParameter), typeof(Delegate));
                    return Expression.Lambda<Func<Event, Delegate>>(lambdaExpression, eventParameter).Compile();
                });
        }

        /// <summary>
        /// Gets an <see cref="Action{object, Delegate}"/> delegate for the add event method, of the
        /// given event information.
        /// </summary>
        /// <typeparam name="T">The type of the first parameter.</typeparam>
        /// <param name="eventInfo">The reflected method information to create an action for.</param>
        /// <returns>
        /// An <see cref="Action{object, Delegate}"/> delegate for the add event method.
        /// </returns>
        public static Action<object, Delegate> GetAddEventAction(this EventInfo eventInfo)
        {
            if (eventInfo == null)
                throw new ArgumentNullException(nameof(eventInfo));

            var type = eventInfo.DeclaringType;
            var parameter1 = Expression.Parameter(typeof(object));
            var parameter2 = Expression.Parameter(typeof(Delegate));
            var instanceParam = Expression.TypeAs(parameter1, type);
            var delegateParam = Expression.TypeAs(parameter2, eventInfo.EventHandlerType);
            var methodCall = Expression.Call(instanceParam, eventInfo.GetAddMethod(), delegateParam);
            return Expression.Lambda<Action<object, Delegate>>(methodCall, new[] { parameter1, parameter2 }).Compile();
        }

        /// <summary>
        /// Gets an <see cref="Action{object, Delegate}"/> delegate for the remove event method, of the
        /// given event information.
        /// </summary>
        /// <typeparam name="T">The type of the first parameter.</typeparam>
        /// <param name="eventInfo">The reflected method information to create an action for.</param>
        /// <returns>
        /// An <see cref="Action{object, Delegate}"/> delegate for the remove event method.
        /// </returns>
        public static Action<object, Delegate> GetRemoveEventAction(this EventInfo eventInfo)
        {
            if (eventInfo == null)
                throw new ArgumentNullException(nameof(eventInfo));

            var type = eventInfo.DeclaringType;
            var parameter1 = Expression.Parameter(typeof(object));
            var parameter2 = Expression.Parameter(typeof(Delegate));
            var instanceParam = Expression.TypeAs(parameter1, type);
            var delegateParam = Expression.TypeAs(parameter2, eventInfo.EventHandlerType);
            var methodCall = Expression.Call(instanceParam, eventInfo.GetRemoveMethod(), delegateParam);
            return Expression.Lambda<Action<object, Delegate>>(methodCall, new[] { parameter1, parameter2 }).Compile();
        }
    }
}
