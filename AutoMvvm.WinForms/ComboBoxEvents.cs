// --------------------------------------------------------------------------------
// <copyright file="ComboBoxEvents.cs" company="AutoMvvm Development Team">
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

using System.Windows.Forms;

namespace AutoMvvm.WinForms
{
    /// <summary>
    /// A collection of <see cref="ComboBox"/> events.
    /// </summary>
    public static class ComboBoxEvents
    {
        public static ComboBoxEvent Click = new ComboBoxEvent(nameof(ComboBox.Click));
        public static ComboBoxEvent TextChanged = new ComboBoxEvent(nameof(ComboBox.TextChanged));
        public static ComboBoxEvent SelectedIndexChanged = new ComboBoxEvent(nameof(ComboBox.SelectedIndexChanged));
        public static ComboBoxEvent MouseDown = new ComboBoxEvent(nameof(ComboBox.MouseDown));
        public static ComboBoxEvent MouseHover = new ComboBoxEvent(nameof(ComboBox.MouseHover));
        public static ComboBoxEvent MouseEnter = new ComboBoxEvent(nameof(ComboBox.MouseEnter));
        public static ComboBoxEvent MouseLeave = new ComboBoxEvent(nameof(ComboBox.MouseLeave));
        public static ComboBoxEvent MouseMove = new ComboBoxEvent(nameof(ComboBox.MouseMove));
    }
}
