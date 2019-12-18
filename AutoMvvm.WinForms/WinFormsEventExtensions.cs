// --------------------------------------------------------------------------------
// <copyright file="WinformsExtensions.cs" company="AutoMvvm Development Team">
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
using System.Windows.Forms;
using AutoMvvm.Fluent;
using AutoMvvm.Reflection;

namespace AutoMvvm.WinForms
{
    /// <summary>
    /// Extension methods for binding events on Windows Forms controls.
    /// </summary>
    public static class WinFormsEventExtensions
    {
        /// <summary>
        /// Binds the given event.
        /// </summary>
        /// <param name="source">The source to bind.</param>
        /// <param name="event">The event to bind to.</param>
        /// <returns>An event binding builder.</returns>
        public static void BindEvent(this Control source, Event @event, Action<ReceivedEvent> action)
        {
            var controlProvider = source.GetTreeMappingProvider();
            var actualEvent = new Event(controlProvider.Name, @event.EventName);
            source.AddEventBinding(new EventBinding(actualEvent, action));
            source.HookEvent(source.GetEventInfo(actualEvent), actualEvent);
        }

        /// <summary>
        /// Hooks the given event for automatic handling.
        /// </summary>
        /// <param name="source">The source entity to get the event info.</param>
        /// <param name="event">The event.</param>
        public static EventInfo GetEventInfo(this object source, Event @event)
        {
            var sourceType = source.GetType();
            return sourceType.GetEvent(@event.EventName);
        }
    }
}
