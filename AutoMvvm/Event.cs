// --------------------------------------------------------------------------------
// <copyright file="Event.cs" company="AutoMvvm Development Team">
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
    /// An event that can be raised or handled.
    /// </summary>
    public class Event : IEquatable<Event>
    {
        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Defines an event to handle.
        /// </summary>
        /// <param name="name">The name of the event.</param>
        public Event(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Indicates whether the current object definition is equal to another one.
        /// </summary>
        /// <param name="obj">An object to compare with this one.</param>
        /// <returns><c>true</c> if the current object and <paramref name="obj"/> is equal; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) => Equals(obj as Event);

        /// <summary>
        /// Indicates whether the current event definition is equal to another one.
        /// </summary>
        /// <param name="other">An event to compare with this event.</param>
        /// <returns><c>true</c> if the current event definition is equal to <paramref name="other"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(Event other) => other?.Name == Name;

        public override int GetHashCode() => Name?.GetHashCode() ?? 0;
    }
}
