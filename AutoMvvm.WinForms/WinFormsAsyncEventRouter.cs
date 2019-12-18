// --------------------------------------------------------------------------------
// <copyright file="WinFormsAsyncEventProvider.cs" company="AutoMvvm Development Team">
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
using System.Windows.Forms;
using AutoMvvm.Design;

namespace AutoMvvm.WinForms
{
    /// <summary>
    /// Provides an asynchronous event routing using the Windows Forms Idle event.
    /// </summary>
    public class WinFormsAsyncEventRouter : AsyncEventRouter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WinFormsControlProvider"/> class.
        /// </summary>
        public WinFormsAsyncEventRouter()
        {
            // Automatically bind the application idle event only once.
            Application.Idle -= Application_Idle;
            Application.Idle += Application_Idle;
        }

        /// <summary>
        /// Handles the application idle event for windows forms applications.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The event arguments.</param>
        /// <remarks>This will ensure that asynchronous events get routed on each application idle moment.</remarks>
        private static void Application_Idle(object sender, EventArgs e) => RouteEvents();
    }
}
