// --------------------------------------------------------------------------------
// <copyright file="PredicatedAction.cs" company="AutoMvvm Development Team">
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
    /// A basic predicated action executed on an entity of type <typeparamref name="TTarget"/>
    /// </summary>
    public class PredicatedWeakAction : WeakAction, IPredicatedWeakAction
    {
        /// <summary>
        /// Gets the predicate upon which to execute the action.
        /// </summary>
        public Func<object, bool> Predicate { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PredicatedAction{TTarget}"/> class.
        /// </summary>
        /// <param name="predicate">The action to decide whether to execute.</param>
        /// <param name="action">The action to perform.</param>
        public PredicatedWeakAction(object target, Action<object> action, Func<object, bool> predicate)
            : base(target, action)
        {
            Predicate = predicate;
        }

        /// <summary>
        /// Executes the action if the predicate is <c>true</c>.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        protected override void InvokeWithTarget(Action<object> action)
        {
            var target = Target;
            if (Predicate?.Invoke(target) == false)
                return;

            base.InvokeWithTarget(action);
        }
    }
}
