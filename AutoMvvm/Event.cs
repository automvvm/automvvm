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
using System.Collections.Generic;
using System.Linq;
using AutoMvvm.Design;

namespace AutoMvvm
{
    /// <summary>
    /// An event that can be raised or handled.
    /// </summary>
    public class Event : IEquatable<Event>
    {
        /// <summary>
        /// Gets the name of the source entity of the event.
        /// </summary>
        public string SourceName { get; }

        /// <summary>
        /// Gets the name of the event.
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// Gets a collection of the source entity name prefix filters which will
        /// be ignored during event binding.
        /// </summary>
        public IList<string> PrefixFilters { get; }

        /// <summary>
        /// Initializes a new instance of an <see cref="Event"/> class.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        public Event(string eventName)
            : this(null, eventName)
        {
        }

        /// <summary>
        /// Initializes a new instance of an <see cref="Event"/> class.
        /// </summary>
        /// <param name="sourceName">The name of the source entity of the event.</param>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="prefixFilters">
        /// The source entity name prefix filters which will be ignored during event binding.
        /// </param>
        public Event(string sourceName, string eventName, params string[] prefixFilters)
        {
            if (string.IsNullOrWhiteSpace(eventName))
                throw new ArgumentOutOfRangeException(nameof(eventName), "The event name must not be null, empty, or contain only whitespace.");

            SourceName = sourceName?.Trim();
            EventName = eventName?.Trim();
            var sortedPrefixFilters = (prefixFilters ?? new string[] { }).OrderByDescending(prefix => prefix.Length).ToList();
            PrefixFilters = sortedPrefixFilters.AsReadOnly();
        }

        /// <summary>
        /// Gets the event's target method name.
        /// </summary>
        public string TargetMethodName
        {
            get
            {
                var sourceName = SourceName;
                if (string.IsNullOrEmpty(sourceName))
                    return string.Empty;

                var filter = PrefixFilters?.FirstOrDefault(sourceName.StartsWith);
                if (!string.IsNullOrEmpty(filter))
                    sourceName = sourceName?.Substring(filter.Length);

                return $"{sourceName}{EventName}";
            }
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
        public bool Equals(Event other) => other?.SourceName == SourceName && other?.EventName == EventName;

        /// <summary>
        /// Returns the hash code for this event.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode() => Hash.Combine(SourceName?.GetHashCode() ?? 0, EventName?.GetHashCode() ?? 0);
    }
}
