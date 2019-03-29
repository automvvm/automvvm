// --------------------------------------------------------------------------------
// <copyright file="NotifyPropertyChangeWrapperExtensionsTests.cs" company="AutoMvvm Development Team">
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
using System.ComponentModel;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace AutoMvvm.Reflection.Tests
{
    public abstract class NotifyPropertyChangeWrapperExtensionsTests
    {
        private PropertyChangingEventHandler _mockChangingHandler;
        private PropertyChangedEventHandler _mockChangedHandler;
        private Type _resultType;

        [SetUp]
        protected virtual void Setup()
        {
            _mockChangingHandler = Substitute.For<PropertyChangingEventHandler>();
            _mockChangedHandler = Substitute.For<PropertyChangedEventHandler>();
        }

        [TestFixture]
        public class WhenWrappingWithNoVirtualProperties : NotifyPropertyChangeWrapperExtensionsTests
        {
            [Test]
            public void ItMustNotWrap()
            {
                var value = typeof(PocoNoVirtualProperties);

                var result = value.GetNotificationWrappedType();

                result.Should().Be(typeof(PocoNoVirtualProperties));
            }
        }

        [TestFixture]
        public class WhenWrappingWithOnlyVirtualProperties : NotifyPropertyChangeWrapperExtensionsTests
        {
            private PocoOnlyVirtualProperties _result;

            protected override void Setup()
            {
                base.Setup();

                _resultType = typeof(PocoOnlyVirtualProperties).GetNotificationWrappedType();
                _result = (PocoOnlyVirtualProperties)Activator.CreateInstance(_resultType);
            }

            [Test]
            public void ItMustWrap()
            {
                _result.Should().NotBeOfType<PocoOnlyVirtualProperties>();
            }

            [Test]
            public void ItMustNotWrapTwice()
            {
                var wrapRequestedTwice = _resultType.GetNotificationWrappedType();
                
                wrapRequestedTwice.Should().Be(_resultType);
            }

            [Test]
            public void ItMustHaveSameName()
            {
                _resultType.Name.Should().Be(nameof(PocoOnlyVirtualProperties));
            }

            [Test]
            public void ItMustHaveSubNamespace()
            {
                _resultType.Namespace.Should().Be($"{typeof(PocoOnlyVirtualProperties).Namespace}.NotifyPropertyChange");
            }

            [Test]
            public void ItMustHaveNotifyPropertyChangingEvent()
            {
                var resultEvents = _resultType.GetEvents();

                resultEvents.Should().ContainSingle(eventInfo => eventInfo.Name == nameof(INotifyPropertyChanging.PropertyChanging));
            }

            [Test]
            public void ItMustHaveNotifyPropertyChangedEvent()
            {
                var resultEvents = _resultType.GetEvents();

                resultEvents.Should().ContainSingle(eventInfo => eventInfo.Name == nameof(INotifyPropertyChanged.PropertyChanged));
            }

            [Test]
            public void ChangingValueWithoutBoundEventsShouldNotThrow()
            {
                Action action = () => _result.VirtualMember1 = 9;

                action.Should().NotThrow<NullReferenceException>();
            }

            [Test]
            public void ItShouldRaisePropertyChangingEventForVirtualMember1()
            {
                _result.AddPropertyChangingEvent(_mockChangingHandler);

                _result.VirtualMember1 = 9;

                _mockChangingHandler.Received(1)(_result, Arg.Is<PropertyChangingEventArgs>(eventArgs => eventArgs.PropertyName == nameof(PocoOnlyVirtualProperties.VirtualMember1)));
            }

            [Test]
            public void ItShouldRaisePropertyChangedEventForVirtualMember1()
            {
                _result.AddPropertyChangedEvent(_mockChangedHandler);

                _result.VirtualMember1 = 9;

                _mockChangedHandler.Received(1)(_result, Arg.Is<PropertyChangedEventArgs>(eventArgs => eventArgs.PropertyName == nameof(PocoOnlyVirtualProperties.VirtualMember1)));
            }

            [Test]
            public void ItShouldRaisePropertyChangingEventForVirtualMember2()
            {
                _result.AddPropertyChangingEvent(_mockChangingHandler);

                _result.VirtualMember2 = 7;

                _mockChangingHandler.Received(1)(_result, Arg.Is<PropertyChangingEventArgs>(eventArgs => eventArgs.PropertyName == nameof(PocoOnlyVirtualProperties.VirtualMember2)));
            }

            [Test]
            public void ItShouldRaisePropertyChangedEventForVirtualMember2()
            {
                _result.AddPropertyChangedEvent(_mockChangedHandler);

                _result.VirtualMember2 = 7;

                _mockChangedHandler.Received(1)(_result, Arg.Is<PropertyChangedEventArgs>(eventArgs => eventArgs.PropertyName == nameof(PocoOnlyVirtualProperties.VirtualMember2)));
            }
        }

        [TestFixture]
        public class WhenWrappingWithMixedVirtualAndNonVirtualProperties : NotifyPropertyChangeWrapperExtensionsTests
        {
            private PocoMixedVirtualAndNonVirtualProperties _result;

            protected override void Setup()
            {
                base.Setup();

                _resultType = typeof(PocoMixedVirtualAndNonVirtualProperties).GetNotificationWrappedType();
                _result = (PocoMixedVirtualAndNonVirtualProperties)Activator.CreateInstance(_resultType);
            }

            [Test]
            public void ItMustWrap()
            {
                _resultType.Should().NotBe(typeof(PocoMixedVirtualAndNonVirtualProperties));
            }

            [Test]
            public void ItShouldNotRaisePropertyChangedEventForNonVirtualMember()
            {
                _result.AddPropertyChangedEvent(_mockChangedHandler);

                _result.NonVirtualMember = 3;

                _mockChangedHandler.DidNotReceive()(_result, Arg.Is<PropertyChangedEventArgs>(eventArgs => eventArgs.PropertyName == nameof(PocoMixedVirtualAndNonVirtualProperties.NonVirtualMember)));
            }

            [Test]
            public void ItShouldRaisePropertyChangedEventForVirtualMember1()
            {
                _result.AddPropertyChangedEvent(_mockChangedHandler);

                _result.VirtualMember1 = 9;

                _mockChangedHandler.Received(1)(_result, Arg.Is<PropertyChangedEventArgs>(eventArgs => eventArgs.PropertyName == nameof(PocoOnlyVirtualProperties.VirtualMember1)));
            }

            [Test]
            public void ItShouldRaisePropertyChangedEventForVirtualMember2()
            {
                _result.AddPropertyChangedEvent(_mockChangedHandler);

                _result.VirtualMember2 = 7;

                _mockChangedHandler.Received(1)(_result, Arg.Is<PropertyChangedEventArgs>(eventArgs => eventArgs.PropertyName == nameof(PocoOnlyVirtualProperties.VirtualMember2)));
            }
        }
    }

    public static class PropertyChangeEventBindingExtensions
    {
        public static void AddPropertyChangingEvent(this object source, PropertyChangingEventHandler handler)
        {
            var resultType = source.GetType();
            var propertyChangingEvent = resultType.GetEvents().FirstOrDefault(eventInfo => eventInfo.Name == nameof(INotifyPropertyChanging.PropertyChanging));
            propertyChangingEvent.AddEventHandler(source, handler);
        }

        public static void AddPropertyChangedEvent(this object source, PropertyChangedEventHandler handler)
        {
            var resultType = source.GetType();
            var propertyChangedEvent = resultType.GetEvents().FirstOrDefault(eventInfo => eventInfo.Name == nameof(INotifyPropertyChanged.PropertyChanged));
            propertyChangedEvent.AddEventHandler(source, handler);
        }
    }

    public class PocoNoVirtualProperties
    {
        public int NonVirtualMember { get; set; } = 5;
    }

    public class PocoOnlyVirtualProperties
    {
        public virtual int VirtualMember1 { get; set; } = 5;
        public virtual int VirtualMember2 { get; set; } = 8;
    }

    public class PocoMixedVirtualAndNonVirtualProperties
    {
        public int NonVirtualMember { get; set; } = 5;
        public virtual int VirtualMember1 { get; set; } = 5;
        public virtual int VirtualMember2 { get; set; } = 8;
    }
}
