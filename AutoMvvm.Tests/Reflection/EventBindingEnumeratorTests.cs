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

using System;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace AutoMvvm.Reflection.Tests
{
    public abstract class EventBindingEnumeratorTests
    {
        [SetUp]
        protected virtual void Setup()
        {
        }

        [TestFixture]
        public class WhenEnumerating : EventBindingEnumeratorTests
        {
            private TestSourceEntity _testSourceEntity;
            private TestTargetEntity _testTargetEntity;
            private EventBindingEnumerator _eventBindingEnumerator;

            protected override void Setup()
            {
                base.Setup();
                _testTargetEntity = Substitute.For<TestTargetEntity>();
                _testSourceEntity = new TestSourceEntity();
                _testSourceEntity.GetTreeMappingProvider("Source");
                _eventBindingEnumerator = new EventBindingEnumerator(_testSourceEntity.TestControlWithEvent, _testTargetEntity);
            }

            [Test]
            public void ItShouldHaveEventBindingTestControlWithEventTestEvent()
            {
                _eventBindingEnumerator.Should().Contain(
                    x => x.Event.SourceName == nameof(TestSourceEntity.TestControlWithEvent) &&
                    x.Event.EventName == nameof(TestControlWithEvent.TestEvent));
            }

            [Test]
            public void ItShouldConnectTestControlWithEventTestEventToTargetMethod()
            {
                var testEventBinding = _eventBindingEnumerator.FirstOrDefault(x => x.Event.SourceName == nameof(_testSourceEntity.TestControlWithEvent) && x.Event.EventName == nameof(TestControlWithEvent.TestEvent));
                testEventBinding?.HandleEvent(_testTargetEntity, new TestEventArgs());
                _testTargetEntity.Received(1).TestControlWithEventTestEvent(Arg.Any<ReceivedEvent<TestEventArgs>>());
            }
        }

        public class TestControl
        {
            public string Text { get; set; }
        }

        public class TestControlWithEvent
        {
            public event EventHandler<TestEventArgs> TestEvent;
        }

        public class TestControlWithTwoEvents
        {
            public event EventHandler<TestEventArgs> TestEvent1;
            public event EventHandler<TestEventArgs> TestEvent2;
            protected virtual void OnTestEvent1(TestEventArgs e) => TestEvent1?.Invoke(this, e);
            protected virtual void OnTestEvent2(TestEventArgs e) => TestEvent2?.Invoke(this, e);
        }

        public class TestSourceEntity
        {
            public event EventHandler<TestEventArgs> TestEvent;
            public TestControl TestControl { get; } = new TestControl();
            public TestControlWithEvent TestControlWithEvent { get; } = new TestControlWithEvent();
            public TestControlWithTwoEvents TestControlWithTwoEvents { get; } = new TestControlWithTwoEvents();

            protected virtual void OnTestEvent(TestEventArgs e) => TestEvent?.Invoke(this, e);
        }

        public class TestEventArgs : EventArgs
        {
        }

        public abstract class TestTargetEntity
        {
            public abstract void SourceTestEvent(ReceivedEvent<TestEventArgs> e);
            public abstract void TestControlWithEventTestEvent(ReceivedEvent<TestEventArgs> e);
            public abstract void TestControlWithTeoEventsTestEvent1(ReceivedEvent<TestEventArgs> e);
            public abstract void TestControlWithTeoEventsTestEvent2(ReceivedEvent<TestEventArgs> e);
        }
    }
}
