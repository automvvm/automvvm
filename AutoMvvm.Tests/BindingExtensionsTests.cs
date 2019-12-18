// --------------------------------------------------------------------------------
// <copyright file="ViewModelExtensionsTests.cs" company="AutoMvvm Development Team">
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

namespace AutoMvvm.Tests
{
    [TestFixture]
    public abstract class BindingExtensionsTests
    {
        private BaseSource _source;

        [SetUp]
        protected virtual void Setup()
        {
        }

        [TestFixture]
        public class GetTargetFromBaseSource : BindingExtensionsTests
        {
            protected override void Setup()
            {
                base.Setup();
                _source = new BaseSource();
            }

            [Test]
            public void ItShouldGiveViewModelInstance()
            {
                var target = _source.GetTarget();
                target.Should().BeOfType<BaseTarget>();
            }

            [Test]
            public void ItShouldGiveSameViewModelForSameViewTwice()
            {
                var target = _source.GetTarget();
                var target2 = _source.GetTarget();
                target.Should().BeSameAs(target2);
            }

            [Test]
            public void ItShouldGiveDifferentViewModelForDifferentView()
            {
                var source2 = new BaseSource();
                var target = _source.GetTarget();
                var target2 = source2.GetTarget();
                target.Should().NotBeSameAs(target2);
            }
        }

        [TestFixture]
        public class GetTargetFromTypeResolverSource : BindingExtensionsTests
        {
            protected override void Setup()
            {
                base.Setup();
                _source = new TypeResolverSource();
            }

            [Test]
            public void ItShouldGiveViewModelInstance()
            {
                var viewModel = _source.GetTarget();
                viewModel.Should().BeOfType<ExtendedTarget>();
            }
        }

        [TestFixture]
        public class GetTargetFromFactoryTypeResolverSource : BindingExtensionsTests
        {
            protected override void Setup()
            {
                base.Setup();
                _source = new FactoryTypeResolverSource();
            }

            [Test]
            public void ItShouldGiveViewModelInstance()
            {
                var target = _source.GetTarget();
                target.Should().BeOfType<SpecialTarget>();
            }

            [Test]
            public void ItShouldGiveInstanceWithParameterSet()
            {
                var target = (SpecialTarget)_source.GetTarget();
                target.SpecialParameter.Should().Be(7);
            }
        }
    }

    public class BaseSource : IBinding<BaseTarget>
    {
    }

    public class TypeResolverSource : BaseSource, ITypeResolver
    {
        public Type ResolveType(Type type)
        {
            if (type == typeof(BaseTarget))
                return typeof(ExtendedTarget);

            return type;
        }
    }

    public class FactoryTypeResolverSource : BaseSource, ITypeResolver, IFactoryProvider
    {
        private readonly int _specialParameter = 7;

        public Type ResolveType(Type type)
        {
            if (type == typeof(BaseTarget))
                return typeof(SpecialTarget);

            return type;
        }

        public Func<Type, object> GetFactory() => Create;
        
        private object Create(Type type)
        {
            if (type == typeof(SpecialTarget))
                return new SpecialTarget(_specialParameter);

            return Activator.CreateInstance(type);
        }
    }

    public class BaseTarget
    {
    }

    public class ExtendedTarget : BaseTarget
    {
    }

    public class SpecialTarget : BaseTarget
    {
        public SpecialTarget(int specialParameter)
        {
            SpecialParameter = specialParameter;
        }

        public int SpecialParameter { get; }
    }
}
