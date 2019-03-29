// --------------------------------------------------------------------------------
// <copyright file="PropertyBindingTests.cs" company="AutoMvvm Development Team">
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
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;

namespace AutoMvvm.Tests
{
    public abstract class PropertyBindingTests
    {
        private TestEntity _testEntitySource;
        private TestEntity _testEntityTarget;

        private const int TestEntitySourceDefault = 5;
        private const int TestEntityTargetDefault = 8;
        private const int TestEntityNewValue = 10;
        private static int[] TestEntitySourceListDefault = { 2, 3, 4, 5 };
        private static int[] TestEntityTargetListDefault = { 10, 11, 12, 13 };
        private static int[] TestEntityNewList = { 6, 7, 8, 9 };

        [SetUp]
        protected virtual void Setup()
        {
            _testEntitySource = new TestEntity();
            _testEntitySource.TestField1 = TestEntitySourceDefault;
            _testEntitySource.TestField2 = (TestEntitySourceDefault + 1).ToString();
            Array.ForEach(TestEntitySourceListDefault, value => _testEntitySource.TestField3.Add(value));
            _testEntityTarget = new TestEntity();
            _testEntityTarget.TestField1 = TestEntityTargetDefault;
            _testEntityTarget.TestField2 = (TestEntityTargetDefault + 1).ToString();
            Array.ForEach(TestEntityTargetListDefault, value => _testEntityTarget.TestField3.Add(value));
        }

        public class WhenCreatingBindingDefault : PropertyBindingTests
        {
            private PropertyBinding _binding;

            protected override void Setup()
            {
                base.Setup();

                _binding = PropertyBinding.Create(
                    () => _testEntitySource.TestField1,
                    () => _testEntityTarget.TestField1);
            }

            [Test]
            public void ItShouldResetTheSourcePropertyByDefault()
            {
                _testEntitySource.TestField1.Should().Be(TestEntityTargetDefault);
            }

            [Test]
            public void UpdateTargetValueShouldUpdateTheTargetProperty()
            {
                _testEntitySource.TestField1 = TestEntityNewValue;
                _binding.UpdateTargetValue();
                _testEntityTarget.TestField1.Should().Be(TestEntityNewValue);
            }

            [Test]
            public void UpdateSourceValueShouldUpdateTheSourceProperty()
            {
                _testEntityTarget.TestField1 = TestEntityNewValue;
                _binding.UpdateSourceValue();
                _testEntitySource.TestField1.Should().Be(TestEntityNewValue);
            }
        }

        public class WhenCreatingBindingTwoWayResetSource : PropertyBindingTests
        {
            private PropertyBinding _binding;

            protected override void Setup()
            {
                base.Setup();

                _binding = PropertyBinding.Create(
                    () => _testEntitySource.TestField1,
                    () => _testEntityTarget.TestField1,
                    PropertyBindingDirection.TwoWayResetSource);
            }

            [Test]
            public void ItShouldResetTheSourcePropertyByDefault()
            {
                _testEntitySource.TestField1.Should().Be(TestEntityTargetDefault);
            }

            [Test]
            public void UpdateTargetValueShouldUpdateTheTargetProperty()
            {
                _testEntitySource.TestField1 = TestEntityNewValue;
                _binding.UpdateTargetValue();
                _testEntityTarget.TestField1.Should().Be(TestEntityNewValue);
            }

            [Test]
            public void UpdateSourceValueShouldUpdateTheSourceProperty()
            {
                _testEntityTarget.TestField1 = TestEntityNewValue;
                _binding.UpdateSourceValue();
                _testEntitySource.TestField1.Should().Be(TestEntityNewValue);
            }
        }

        public class WhenCreatingBindingTwoWayResetTarget : PropertyBindingTests
        {
            private PropertyBinding _binding;

            protected override void Setup()
            {
                base.Setup();

                _binding = PropertyBinding.Create(
                    () => _testEntitySource.TestField1,
                    () => _testEntityTarget.TestField1,
                    PropertyBindingDirection.TwoWayResetTarget);
            }

            [Test]
            public void ItShouldResetTheTargetPropertyByDefault()
            {
                _testEntityTarget.TestField1.Should().Be(TestEntitySourceDefault);
            }

            [Test]
            public void UpdateTargetValueShouldUpdateTheTargetProperty()
            {
                _testEntitySource.TestField1 = TestEntityNewValue;
                _binding.UpdateTargetValue();
                _testEntityTarget.TestField1.Should().Be(TestEntityNewValue);
            }

            [Test]
            public void UpdateSourceValueShouldUpdateTheSourceProperty()
            {
                _testEntityTarget.TestField1 = TestEntityNewValue;
                _binding.UpdateSourceValue();
                _testEntitySource.TestField1.Should().Be(TestEntityNewValue);
            }
        }

        public class WhenCreatingBindingOneWayToSource : PropertyBindingTests
        {
            private PropertyBinding _binding;

            protected override void Setup()
            {
                base.Setup();

                _binding = PropertyBinding.Create(
                    () => _testEntitySource.TestField1,
                    () => _testEntityTarget.TestField1,
                    PropertyBindingDirection.OneWayToSource);
            }

            [Test]
            public void ItShouldResetTheSourcePropertyByDefault()
            {
                _testEntitySource.TestField1.Should().Be(TestEntityTargetDefault);
            }

            [Test]
            public void UpdateTargetValueShouldFail()
            {
                _testEntitySource.TestField1 = TestEntityNewValue;
                var updateSuccessful = _binding.UpdateTargetValue();
                updateSuccessful.Should().BeFalse();
            }

            [Test]
            public void UpdateTargetValueShouldNotUpdateTheTargetProperty()
            {
                _testEntitySource.TestField1 = TestEntityNewValue;
                _binding.UpdateTargetValue();
                _testEntityTarget.TestField1.Should().Be(TestEntityTargetDefault);
            }

            [Test]
            public void UpdateSourceValueShouldUpdateTheSourceProperty()
            {
                _testEntityTarget.TestField1 = TestEntityNewValue;
                _binding.UpdateSourceValue();
                _testEntitySource.TestField1.Should().Be(TestEntityNewValue);
            }
        }

        public class WhenCreatingBindingOneWayToTarget : PropertyBindingTests
        {
            private PropertyBinding _binding;

            protected override void Setup()
            {
                base.Setup();

                _binding = PropertyBinding.Create(
                    () => _testEntitySource.TestField1,
                    () => _testEntityTarget.TestField1,
                    PropertyBindingDirection.OneWayToTarget);
            }

            [Test]
            public void ItShouldResetTheTargetPropertyByDefault()
            {
                _testEntityTarget.TestField1.Should().Be(TestEntitySourceDefault);
            }

            [Test]
            public void UpdateSourceValueShouldFail()
            {
                _testEntityTarget.TestField1 = TestEntityNewValue;
                var updateSuccessful = _binding.UpdateSourceValue();
                updateSuccessful.Should().BeFalse();
            }

            [Test]
            public void UpdateSourceValueShouldNotUpdateTheSourceProperty()
            {
                _testEntityTarget.TestField1 = TestEntityNewValue;
                _binding.UpdateSourceValue();
                _testEntitySource.TestField1.Should().Be(TestEntitySourceDefault);
            }

            [Test]
            public void UpdateTargetValueShouldUpdateTheTargetProperty()
            {
                _testEntitySource.TestField1 = TestEntityNewValue;
                _binding.UpdateTargetValue();
                _testEntityTarget.TestField1.Should().Be(TestEntityNewValue);
            }
        }

        public class WhenCreatingBindingWithDifferentTypes : PropertyBindingTests
        {
            private PropertyBinding _binding;

            protected override void Setup()
            {
                base.Setup();

                _binding = PropertyBinding.Create(
                    () => _testEntitySource.TestField1,
                    () => _testEntityTarget.TestField2);
            }

            [Test]
            public void ItShouldResetTheSourcePropertyByDefault()
            {
                _testEntitySource.TestField1.Should().Be(TestEntityTargetDefault + 1);
            }

            [Test]
            public void UpdateSourceValueShouldUpdateTheSourceProperty()
            {
                _testEntityTarget.TestField2 = TestEntityNewValue.ToString();
                _binding.UpdateSourceValue();
                _testEntitySource.TestField1.Should().Be(TestEntityNewValue);
            }

            [Test]
            public void UpdateTargetValueShouldUpdateTheTargetProperty()
            {
                _testEntitySource.TestField1 = TestEntityNewValue;
                _binding.UpdateTargetValue();
                _testEntityTarget.TestField2.Should().Be(TestEntityNewValue.ToString());
            }

            [Test]
            public void SettingTargetToEmptyStringAndUpdateSourceShouldSetTheSourcePropertyToZero()
            {
                _testEntityTarget.TestField2 = string.Empty;
                _binding.UpdateSourceValue();
                _testEntitySource.TestField1.Should().Be(0);
            }

            [Test]
            public void SettingTargetToNullAndUpdateSourceShouldSetTheSourcePropertyToZero()
            {
                _testEntityTarget.TestField2 = null;
                _binding.UpdateSourceValue();
                _testEntitySource.TestField1.Should().Be(0);
            }
        }

        public class WhenCreatingBindingWithList : PropertyBindingTests
        {
            private PropertyBinding _binding;

            protected override void Setup()
            {
                base.Setup();

                _binding = PropertyBinding.Create(
                    () => _testEntitySource.TestField3,
                    () => _testEntityTarget.TestField3);
            }

            [Test]
            public void ItShouldResetTheTargetPropertyByDefault()
            {
                _testEntitySource.TestField3.Should().ContainInOrder(TestEntityTargetListDefault);
            }

            [Test]
            public void ItShouldUpdateTheSourceProperty()
            {
                _testEntityTarget.TestField3.Clear();
                Array.ForEach(TestEntityNewList, value => _testEntityTarget.TestField3.Add(value));
                _binding.UpdateSourceValue();
                _testEntitySource.TestField3.Should().ContainInOrder(TestEntityNewList);
            }

            [Test]
            public void UpdateTargetValueShouldUpdateTheTargetProperty()
            {
                _testEntitySource.TestField3.Clear();
                Array.ForEach(TestEntityNewList, value => _testEntitySource.TestField3.Add(value));
                _binding.UpdateTargetValue();
                _testEntityTarget.TestField3.Should().ContainInOrder(TestEntityNewList);
            }
        }

        public class TestEntity
        {
            public int TestField1 { get; set; }

            public string TestField2 { get; set; }

            public IList<int> TestField3 { get; } = new List<int>();
        }
    }
}
