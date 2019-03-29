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
    public abstract class ViewModelExtensionsTests
    {
        private BaseView _view;

        [SetUp]
        protected virtual void Setup()
        {
        }

        [TestFixture]
        public class GetViewModelFromBaseView : ViewModelExtensionsTests
        {
            protected override void Setup()
            {
                base.Setup();
                _view = new BaseView();
            }

            [Test]
            public void ItShouldGiveViewModelInstance()
            {
                var viewModel = _view.GetViewModel();
                viewModel.Should().BeOfType<BaseViewModel>();
            }

            [Test]
            public void ItShouldGiveSameViewModelForSameViewTwice()
            {
                var viewModel = _view.GetViewModel();
                var viewModel2 = _view.GetViewModel();
                viewModel.Should().BeSameAs(viewModel2);
            }

            [Test]
            public void ItShouldGiveDifferentViewModelForDifferentView()
            {
                var view2 = new BaseView();
                var viewModel = _view.GetViewModel();
                var viewModel2 = view2.GetViewModel();
                viewModel.Should().NotBeSameAs(viewModel2);
            }
        }

        [TestFixture]
        public class GetViewModelFromTypeResolverView : ViewModelExtensionsTests
        {
            protected override void Setup()
            {
                base.Setup();
                _view = new TypeResolverView();
            }

            [Test]
            public void ItShouldGiveViewModelInstance()
            {
                var viewModel = _view.GetViewModel();
                viewModel.Should().BeOfType<ExtendedViewModel>();
            }
        }

        [TestFixture]
        public class GetViewModelFromFactoryTypeResolverView : ViewModelExtensionsTests
        {
            protected override void Setup()
            {
                base.Setup();
                _view = new FactoryTypeResolverView();
            }

            [Test]
            public void ItShouldGiveViewModelInstance()
            {
                var viewModel = _view.GetViewModel();
                viewModel.Should().BeOfType<SpecialViewModel>();
            }

            [Test]
            public void ItShouldGiveInstanceWithParameterSet()
            {
                var viewModel = (SpecialViewModel)_view.GetViewModel();
                viewModel.SpecialParameter.Should().Be(7);
            }
        }
    }

    public class BaseView : IWithViewModel<BaseViewModel>
    {
    }

    public class TypeResolverView : BaseView, ITypeResolver
    {
        public Type ResolveType(Type type)
        {
            if (type == typeof(BaseViewModel))
                return typeof(ExtendedViewModel);

            return type;
        }
    }

    public class FactoryTypeResolverView : BaseView, ITypeResolver, IFactory
    {
        private int _specialParameter = 7;
        
        public object Create(Type type)
        {
            if (type == typeof(SpecialViewModel))
                return new SpecialViewModel(_specialParameter);

            return Activator.CreateInstance(type);
        }

        public Type ResolveType(Type type)
        {
            if (type == typeof(BaseViewModel))
                return typeof(SpecialViewModel);

            return type;
        }
    }

    public class BaseViewModel
    {
    }

    public class ExtendedViewModel : BaseViewModel
    {
    }

    public class SpecialViewModel : BaseViewModel
    {
        public SpecialViewModel(int specialParameter)
        {
            SpecialParameter = specialParameter;
        }

        public int SpecialParameter { get; }
    }
}
