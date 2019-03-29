﻿// --------------------------------------------------------------------------------
// <copyright file="QueuedEventExtensions.cs" company="AutoMvvm Development Team">
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
using System.Text;
using AutoMvvm.Design;

namespace AutoMvvm.Fluent
{
    /// <summary>
    /// Extensions methods for queuing and handling events.
    /// </summary>
    public static class QueuedEventExtensions
    {
        /// <summary>
        /// Binds the given event.
        /// </summary>
        /// <param name="source">The source to bind.</param>
        /// <param name="event">The event to bind to.</param>
        /// <returns>An event binding builder.</returns>
        public static IEventBuilder WithEvent(this object source, Event @event)
        {
            return new EventBuilder(source, @event);
        }

        /// <summary>
        /// Sends an event for the source object.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="event">The event to send.</param>
        /// <param name="e">The event arguments</param>
        public static void SendEvent(this object source, Event @event, EventArgs e)
        {
            var events = source.Get<EventQueue>();
            events.Events.Enqueue(new ReceivedEvent(source, @event, e));
        }
    }
}