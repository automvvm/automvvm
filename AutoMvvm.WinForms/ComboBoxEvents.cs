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
        public static Event Click = new Event(nameof(ComboBox.Click));
        public static Event TextChanged = new Event(nameof(ComboBox.TextChanged));
        public static Event SelectedIndexChanged = new Event(nameof(ComboBox.SelectedIndexChanged));
        public static Event MouseDown = new Event(nameof(ComboBox.MouseDown));
        public static Event MouseHover = new Event(nameof(ComboBox.MouseHover));
        public static Event MouseEnter = new Event(nameof(ComboBox.MouseEnter));
        public static Event MouseLeave = new Event(nameof(ComboBox.MouseLeave));
        public static Event MouseMove = new Event(nameof(ComboBox.MouseMove));
    }
}
