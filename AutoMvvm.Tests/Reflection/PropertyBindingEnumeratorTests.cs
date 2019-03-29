// --------------------------------------------------------------------------------
// <copyright file="PropertyBindingEnumeratorTests.cs" company="AutoMvvm Development Team">
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

using System.Collections.Generic;
using System.ComponentModel;
using FluentAssertions;
using NUnit.Framework;

namespace AutoMvvm.Reflection.Tests
{
    public abstract class PropertyBindingEnumeratorTests
    {
        [SetUp]
        protected virtual void Setup()
        {
        }

        [TestFixture]
        public class WhenEnumerating : PropertyBindingEnumeratorTests
        {
            private TestSourceEntity _testSourceEntity;
            private TestTargetEntity _testTargetEntity;
            private PropertyBindingEnumerator<TestTargetEntity> _propertyBindingEnumerator;

            protected override void Setup()
            {
                base.Setup();
                _testSourceEntity = new TestSourceEntity();
                _testTargetEntity = _testSourceEntity.GetViewModel();
                _propertyBindingEnumerator = new PropertyBindingEnumerator<TestTargetEntity>(_testSourceEntity);
            }

            [Test]
            public void ItShouldCreateBindingWithSourcePropertyName()
            {
                _propertyBindingEnumerator.Should().Contain(
                    x => x.SourceProperty.Source == _testSourceEntity.TestControl &&
                    x.SourceProperty.Name == "Text" && x.TargetProperty.Name == nameof(TestTargetEntity.TestControlText));
            }

            [Test]
            public void ItShouldCreateDefaultPropertyBinding()
            {
                _propertyBindingEnumerator.Should().Contain(
                    x => x.SourceProperty.Source == _testSourceEntity.TestControlWithDefaultProperty &&
                    x.SourceProperty.Name == "Text" && x.TargetProperty.Name == nameof(TestTargetEntity.TestControlWithDefaultProperty));
            }

            [Test]
            public void ItShouldCreateDefaultBindingPropertyBinding()
            {
                _propertyBindingEnumerator.Should().Contain(
                    x => x.SourceProperty.Source == _testSourceEntity.TestControlWithDefaultBindingProperty &&
                    x.SourceProperty.Name == "Text" && x.TargetProperty.Name == nameof(TestTargetEntity.TestControlWithDefaultBindingProperty));
            }

            [Test]
            public void ItShouldCreateNonDefaultBinding()
            {
                _propertyBindingEnumerator.Should().Contain(
                    x => x.SourceProperty.Source == _testSourceEntity.TestControlWithDefaultBindingProperty &&
                    x.SourceProperty.Name == "List" && x.TargetProperty.Name == nameof(TestTargetEntity.TestControlWithDefaultBindingPropertyList));
            }
        }

        public class TestControl
        {
            public string Text { get; set; }
        }

        [DefaultProperty("Text")]
        public class TestControlWithDefaultProperty
        {
            public string Text { get; set; }
        }

        [DefaultProperty("List")]
        [DefaultBindingProperty("Text")]
        public class TestControlWithDefaultBindingProperty
        {
            public string Text { get; set; }
            public IList<string> List { get; } = new List<string>();
        }

        public class TestSourceEntity : IWithViewModel<TestTargetEntity>
        {
            public TestControl TestControl { get; } = new TestControl();
            public TestControlWithDefaultProperty TestControlWithDefaultProperty { get; } = new TestControlWithDefaultProperty();
            public TestControlWithDefaultBindingProperty TestControlWithDefaultBindingProperty { get; } = new TestControlWithDefaultBindingProperty();
        }

        public class TestTargetEntity
        {
            public string TestControlText { get; set; }
            public string TestControlWithDefaultProperty { get; set; }
            public string TestControlWithDefaultBindingProperty { get; set; }
            public IList<string> TestControlWithDefaultBindingPropertyList { get; set; }
        }
    }
}
