// --------------------------------------------------------------------------------
// <copyright file="IWeakAction.cs" company="AutoMvvm Development Team">
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

namespace AutoMvvm
{
    /// <summary>
    /// Defines an action delegate interface with a weak reference to the target.
    /// </summary>
    public interface IWeakAction : IValidatedTarget
    {
        /// <summary>
        /// Invokes this action delegate if the target reference is still accessible.
        /// </summary>
        public void Invoke();
    }

    /// <summary>
    /// Defines an action delegate interface with a weak reference to the target.
    /// </summary>
    /// <typeparam name="T">The type of the parameter</typeparam>
    public interface IWeakAction<in T> : IValidatedTarget
    {
        /// <summary>
        /// Invokes this action delegate if the target reference is still accessible.
        /// </summary>
        public void Invoke(T param);
    }

    /// <summary>
    /// Defines an action delegate interface with a weak reference to the target.
    /// </summary>
    /// <typeparam name="T1">The type of the first parameter</typeparam>
    /// <typeparam name="T1">The type of the second parameter</typeparam>
    public interface IWeakAction<in T1, in T2> : IValidatedTarget
    {
        /// <summary>
        /// Invokes this action delegate if the target reference is still accessible.
        /// </summary>
        public void Invoke(T1 param1, T2 param2);
    }
}
