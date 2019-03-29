// --------------------------------------------------------------------------------
// <copyright file="PropertyDetailsTests.cs" company="AutoMvvm Development Team">
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
using FluentAssertions;
using NUnit.Framework;

namespace AutoMvvm.Reflection.Tests
{
    public abstract class PropertyDetailsTests
    {
        [SetUp]
        protected virtual void Setup()
        {
        }

        [TestFixture]
        public class WhenConstructingPropertyDetails : PropertyDetailsTests
        {
            private TestSourceObject _sourceObject;
            private PropertyDetails<int> _propertyDetails;

            protected override void Setup()
            {
                _sourceObject = new TestSourceObject();
                _propertyDetails = PropertyDetails.Create(() => _sourceObject.TestProperty1);
            }

            [Test]
            public void ItShouldGetSourceObject()
            {
                _propertyDetails.Source.Should().BeSameAs(_sourceObject);
            }

            [Test]
            public void ItShouldGetPropertyDetails()
            {
                _propertyDetails.Name.Should().Be(nameof(TestSourceObject.TestProperty1));
            }

            [Test]
            public void ItShouldNotKeepADisposedSourceObject()
            {
                _sourceObject = null;
                GC.Collect();
                GC.WaitForFullGCComplete();
                _propertyDetails.Source.Should().BeNull();
            }
        }

        [TestFixture]
        public class WhenConstructingPropertyDetailsWithTypeCasting : PropertyDetailsTests
        {
            private TestSourceInheritedObject _sourceObject;
            private PropertyDetails<int> _propertyDetails;

            protected override void Setup()
            {
                _sourceObject = new TestSourceInheritedObject();
                _propertyDetails = PropertyDetails.Create(() => ((IHiddenInterface)_sourceObject).TestHiddenProperty);
            }

            [Test]
            public void ItShouldGetSourceObject()
            {
                _propertyDetails.Source.Should().BeSameAs(_sourceObject);
            }

            [Test]
            public void ItShouldGetPropertyDetails()
            {
                _propertyDetails.Name.Should().Be(nameof(IHiddenInterface.TestHiddenProperty));
            }
        }

        [TestFixture]
        public class WhenGetting : PropertyDetailsTests
        {
            private TestSourceObject _sourceObject;
            private PropertyDetails<int> _propertyDetails;

            protected override void Setup()
            {
                _sourceObject = new TestSourceObject();
                _propertyDetails = PropertyDetails.Create(() => _sourceObject.TestProperty1);
            }

            [Test]
            public void ItShouldGetValue()
            {
                _sourceObject.TestProperty1 = 7;
                _propertyDetails.GetValue().Should().Be(7);
            }

            [Test]
            public void ItShouldSetValue()
            {
                _propertyDetails.SetValue(9);
                _sourceObject.TestProperty1.Should().Be(9);
            }
        }

        [TestFixture]
        public class WhenComparing : PropertyDetailsTests
        {
            private TestSourceObject _sourceObject;
            private PropertyDetails<int> _propertyDetails;

            protected override void Setup()
            {
                _sourceObject = new TestSourceObject();
                _propertyDetails = PropertyDetails.Create(() => _sourceObject.TestProperty1);
            }

            [Test]
            public void ItShouldBeTheSameIfCreatedWithSameObjectAndProperty()
            {
                var newPropertyDetails = PropertyDetails.Create(() => _sourceObject.TestProperty1);
                newPropertyDetails.Should().Be(_propertyDetails);
            }
        }

        public class TestSourceObject
        {
            public int TestProperty1 { get; set; }
            public int TestProperty2 { get; set; }
        }

        public interface IHiddenInterface
        {
            int TestHiddenProperty { get; }
        }

        public class TestSourceInheritedObject : IHiddenInterface
        {
            public int TestProperty1 { get; set; }
            public int TestProperty2 { get; set; }

            int IHiddenInterface.TestHiddenProperty => 5;
        }
    }
}
