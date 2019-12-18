// --------------------------------------------------------------------------------
// <copyright file="WeakAction.cs" company="AutoMvvm Development Team">
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

using AutoMvvm.Reflection;
using System;
using System.Reflection;

namespace AutoMvvm
{
    /// <summary>
    /// The common base class for all weak action types.
    /// </summary>
    /// <remarks>
    /// Handles all common weak reference target behavior.
    /// </remarks>
    public class WeakActionBase
    {
        private readonly WeakReference _target;

        /// <summary>
        /// Gets the target of the action if it is still accessible.
        /// </summary>
        public object Target => _target.Target;

        /// <summary>
        /// Gets a value indicating whether this weak action is still valid.
        /// </summary>
        public bool IsValid => _target.IsAlive;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakActionBase"/> class.
        /// </summary>
        /// <param name="method">The method containing the target of the action.</param>
        public WeakActionBase(Delegate method)
            : this(method?.Target)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakActionBase"/> class.
        /// </summary>
        /// <param name="target">The target of the action.</param>
        public WeakActionBase(object target)
        {
            _target = new WeakReference(target, false);
        }

        /// <summary>
        /// Try to invoke the given action with the target.
        /// </summary>
        /// <param name="action">The action to invoke.</param>
        protected virtual void InvokeWithTarget(Action<object> action)
        {
            var target = Target;
            if (target == null)
                return;

            action?.Invoke(target);
        }
    }

    public class WeakAction : WeakActionBase, IWeakAction
    {
        private readonly Action<object> _action;

        public WeakAction(Action action)
            : base(action)
        {
            _action = action.GetMethodInfo().GetAction();
        }

        public WeakAction(object target, Action<object> action)
            : base(target)
        {
            _action = action;
        }

        public static explicit operator WeakAction(Action action) =>
            new WeakAction(action);

        public void Invoke() => InvokeWithTarget(_action);
    }

    public class WeakAction<T> : WeakActionBase, IWeakAction<T>
    {
        private readonly Action<object, T> _action;

        public WeakAction(Action<T> action)
            : base(action)
        {
            _action = action.GetMethodInfo().GetAction<T>();
        }

        public WeakAction(object target, Action<object, T> action)
            : base(target)
        {
            _action = action;
        }

        public static explicit operator WeakAction<T>(Action<T> action) =>
            new WeakAction<T>(action);

        public void Invoke(T param) => InvokeWithTarget(target => _action?.Invoke(target, param));
    }

    public class WeakAction<T1, T2> : WeakActionBase, IWeakAction<T1, T2>
    {
        private readonly Action<object, T1, T2> _action;

        public WeakAction(Action<T1, T2> action)
            : base(action)
        {
            _action = action.GetMethodInfo().GetAction<T1, T2>();
        }

        public WeakAction(object target, Action<object, T1, T2> action)
            : base(target)
        {
            _action = action;
        }

        public static explicit operator WeakAction<T1, T2>(Action<T1, T2> action) =>
            new WeakAction<T1, T2>(action);

        public void Invoke(T1 param1, T2 param2) => InvokeWithTarget(target => _action?.Invoke(target, param1, param2));
    }
}