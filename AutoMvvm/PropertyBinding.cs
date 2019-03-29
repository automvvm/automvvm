// --------------------------------------------------------------------------------
// <copyright file="PropertyBinding.cs" company="AutoMvvm Development Team">
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
using System.Collections;
using System.Linq.Expressions;
using AutoMvvm.Reflection;

namespace AutoMvvm
{
    /// <summary>
    /// Defines a property binding.
    /// </summary>
    public abstract class PropertyBinding
    {
        /// <summary>
        /// Gets the source property details.
        /// </summary>
        public PropertyDetails SourceProperty { get; }

        /// <summary>
        /// Gets the target property details.
        /// </summary>
        public PropertyDetails TargetProperty { get; }

        /// <summary>
        /// Gets the direction of the property binding.
        /// </summary>
        public PropertyBindingDirection Direction { get; }

        /// <summary>
        /// Gets whether we can write to the target property.
        /// </summary>
        protected bool CanWriteToTarget
        {
            get
            {
                var direction = GetDirection();
                return direction == PropertyBindingDirection.TwoWayResetSource || direction == PropertyBindingDirection.TwoWayResetTarget || direction == PropertyBindingDirection.OneWayToTarget;
            }
        }

        /// <summary>
        /// Gets whether we can write to the source property.
        /// </summary>
        protected bool CanWriteToSource
        {
            get
            {
                var direction = GetDirection();
                return direction == PropertyBindingDirection.TwoWayResetSource || direction == PropertyBindingDirection.TwoWayResetTarget || direction == PropertyBindingDirection.OneWayToSource;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBinding"/> class.
        /// </summary>
        /// <param name="sourceProperty">The source property details.</param>
        /// <param name="targetProperty">The target property details.</param>
        public PropertyBinding(PropertyDetails sourceProperty, PropertyDetails targetProperty)
            : this(sourceProperty, targetProperty, PropertyBindingDirection.Default)
        {
        }

        /// <summary>
        /// Creates a property binding with the given source property and target property.
        /// </summary>
        /// <param name="sourceProperty">The source property.</param>
        /// <param name="targetProperty">The target property.</param>
        /// <returns>A generated property binding.</returns>
        public static PropertyBinding Create(PropertyDetails sourceProperty, PropertyDetails targetProperty)
        {
            var propertyBindingType = typeof(PropertyBinding<,>).GetGenericType(sourceProperty.Type, targetProperty.Type);
            return (PropertyBinding)propertyBindingType.CreateInstance(sourceProperty, targetProperty);
        }

        /// <summary>
        /// Creates a property binding with the given source property and target property.
        /// </summary>
        /// <param name="sourceProperty">The source property.</param>
        /// <param name="targetProperty">The target property.</param>
        /// <returns>A generated property binding.</returns>
        public static PropertyBinding Create(PropertyDetails sourceProperty, PropertyDetails targetProperty, PropertyBindingDirection direction)
        {
            var propertyBindingType = typeof(PropertyBinding<,>).MakeGenericType(sourceProperty.Type, targetProperty.Type);
            return (PropertyBinding)propertyBindingType.CreateInstance(sourceProperty, targetProperty, direction);
        }

        /// <summary>
        /// Creates a property binding with the given source property and target property.
        /// </summary>
        /// <param name="sourceProperty">The source property.</param>
        /// <param name="targetProperty">The target property.</param>
        /// <param name="direction">The direction of the property binding.</param>
        /// <returns>A generated property binding.</returns>
        public static PropertyBinding Create<TSource, TTarget>(PropertyDetails<TSource> sourceProperty, PropertyDetails<TTarget> targetProperty, PropertyBindingDirection direction) =>
            new PropertyBinding<TSource, TTarget>(sourceProperty, targetProperty, direction);

        /// <summary>
        /// Creates a property binding with the given source property and target property.
        /// </summary>
        /// <param name="sourceProperty">The source property.</param>
        /// <param name="targetProperty">The target property.</param>
        /// <returns>A generated property binding.</returns>
        public static PropertyBinding Create<TSource, TTarget>(Expression<Func<TSource>> sourceProperty, Expression<Func<TTarget>> targetProperty) =>
            new PropertyBinding<TSource, TTarget>(sourceProperty, targetProperty);

        /// <summary>
        /// Creates a property binding with the given source property and target property.
        /// </summary>
        /// <param name="sourceProperty">The source property.</param>
        /// <param name="targetProperty">The target property.</param>
        /// <param name="direction">The direction of the property binding.</param>
        /// <returns>A generated property binding.</returns>
        public static PropertyBinding Create<TSource, TTarget>(Expression<Func<TSource>> sourceProperty, Expression<Func<TTarget>> targetProperty, PropertyBindingDirection direction) =>
            new PropertyBinding<TSource, TTarget>(sourceProperty, targetProperty, direction);

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBinding"/> class.
        /// </summary>
        /// <param name="sourceProperty">The source property details.</param>
        /// <param name="targetProperty">The target property details.</param>
        /// <param name="direction">The direction of the binding.</param>
        public PropertyBinding(PropertyDetails sourceProperty, PropertyDetails targetProperty, PropertyBindingDirection direction)
        {
            SourceProperty = sourceProperty;
            TargetProperty = targetProperty;
            Direction = direction;
            UpdateFromDefault();
        }

        /// <summary>
        /// Updates the property based on the initial binding update direction.
        /// </summary>
        /// <returns><c>true</c> if the property was updated.</returns>
        public bool UpdateFromDefault()
        {
            switch (GetDirection())
            {
                case PropertyBindingDirection.TwoWayResetSource:
                case PropertyBindingDirection.OneWayToSource:
                    return UpdateSourceValue();

                case PropertyBindingDirection.TwoWayResetTarget:
                case PropertyBindingDirection.OneWayToTarget:
                    return UpdateTargetValue();

                default:
                    throw new InvalidOperationException("The specified binding direction is invalid.");
            }
        }

        /// <summary>
        /// Updates the property bound to the given property.
        /// </summary>
        /// <param name="property">The property the update came from.</param>
        /// <returns><c>true</c> if the property was updated.</returns>
        public bool UpdateFrom(PropertyDetails property)
        {
            // If this property doesn't match fail update now.
            if (SourceProperty != property && TargetProperty != property)
                return false;

            return (SourceProperty == property) ? UpdateTargetValue() : UpdateSourceValue();
        }

        /// <summary>
        /// Updates the target with the value in the source.
        /// </summary>
        /// <returns><c>true</c> if the property was updated.</returns>
        public abstract bool UpdateTargetValue();

        /// <summary>
        /// Updates the source with the value in the target.
        /// </summary>
        /// <returns><c>true</c> if the property was updated.</returns>
        public abstract bool UpdateSourceValue();

        private PropertyBindingDirection GetDirection()
        {
            if (Direction != PropertyBindingDirection.Default)
                return Direction;

            if ((TargetProperty.CanGet || TargetProperty.CanGetCollection) && (TargetProperty.CanSet || TargetProperty.CanChangeCollection) && (SourceProperty.CanGet || SourceProperty.CanGetCollection) && (SourceProperty.CanSet || SourceProperty.CanChangeCollection))
                return PropertyBindingDirection.TwoWayResetSource;

            if ((TargetProperty.CanGet || TargetProperty.CanGetCollection) && !TargetProperty.CanSet && !TargetProperty.CanChangeCollection && (SourceProperty.CanSet || SourceProperty.CanChangeCollection))
                return PropertyBindingDirection.OneWayToSource;

            if ((SourceProperty.CanGet || TargetProperty.CanGetCollection) && !SourceProperty.CanSet && !SourceProperty.CanChangeCollection && (TargetProperty.CanSet || TargetProperty.CanChangeCollection))
                return PropertyBindingDirection.OneWayToTarget;

            throw new InvalidOperationException("No update direction could be determined for the given properties.");
        }
    }

    /// <summary>
    /// Defines a property binding with a source type of <typeparamref name="TSource"/> and target
    /// type of <typeparamref name="TTarget"/>.
    /// </summary>
    /// <typeparam name="TSource">The source property type.</typeparam>
    /// <typeparam name="TTarget">The target property type.</typeparam>
    public class PropertyBinding<TSource, TTarget> : PropertyBinding
    {
        private bool _updating;

        /// <summary>
        /// Gets the source property details.
        /// </summary>
        public new PropertyDetails<TSource> SourceProperty => (PropertyDetails<TSource>)base.SourceProperty;

        /// <summary>
        /// Gets the target property details.
        /// </summary>
        public new PropertyDetails<TTarget> TargetProperty => (PropertyDetails<TTarget>)base.TargetProperty;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBinding{TSource, TTarget}"/> class.
        /// </summary>
        /// <param name="sourceProperty">The source property details.</param>
        /// <param name="targetProperty">The target property details.</param>
        public PropertyBinding(PropertyDetails<TSource> sourceProperty, PropertyDetails<TTarget> targetProperty)
            : base(sourceProperty, targetProperty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBinding{TSource, TTarget}"/> class.
        /// </summary>
        /// <param name="sourceProperty">The source property details.</param>
        /// <param name="targetProperty">The target property details.</param>
        /// <param name="direction">The direction of the binding.</param>
        public PropertyBinding(PropertyDetails<TSource> sourceProperty, PropertyDetails<TTarget> targetProperty, PropertyBindingDirection direction)
            : base(sourceProperty, targetProperty, direction)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBinding{TSource, TTarget}"/> class.
        /// </summary>
        /// <param name="sourceProperty">The source property access expression.</param>
        /// <param name="targetProperty">The target property access expression.</param>
        public PropertyBinding(Expression<Func<TSource>> sourceProperty, Expression<Func<TTarget>> targetProperty)
            : this(new PropertyDetails<TSource>(sourceProperty), new PropertyDetails<TTarget>(targetProperty))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBinding{TSource, TTarget}"/> class.
        /// </summary>
        /// <param name="sourceProperty">The source property access expression.</param>
        /// <param name="targetProperty">The target property access expression.</param>
        /// <param name="direction">The direction of the binding.</param>
        public PropertyBinding(Expression<Func<TSource>> sourceProperty, Expression<Func<TTarget>> targetProperty, PropertyBindingDirection direction)
            : this(new PropertyDetails<TSource>(sourceProperty), new PropertyDetails<TTarget>(targetProperty), direction)
        {
        }

        /// <summary>
        /// Updates the target with the value in the source.
        /// </summary>
        /// <returns><c>true</c> if the property was updated.</returns>
        public override bool UpdateTargetValue()
        {
            var sourceObject = SourceProperty.Source;
            var targetObject = TargetProperty.Source;
            if (!CanWriteToTarget)
                return false;

            return UpdateProperty(TargetProperty, SourceProperty);
        }

        /// <summary>
        /// Updates the source with the value in the target.
        /// </summary>
        /// <returns><c>true</c> if the property was updated.</returns>
        public override bool UpdateSourceValue()
        {
            var sourceObject = SourceProperty.Source;
            var targetObject = TargetProperty.Source;
            if (!CanWriteToSource)
                return false;

            return UpdateProperty(SourceProperty, TargetProperty);
        }

        /// <summary>
        /// Updates the target property with the value in the source.
        /// </summary>
        /// <param name="assignProperty">The property to assign a value.</param>
        /// <param name="valueProperty">The property to provide a value.</param>
        /// <returns><c>true</c> if the property was updated.</returns>
        private bool UpdateProperty<TAssign, TValue>(PropertyDetails<TAssign> assignProperty, PropertyDetails<TValue> valueProperty)
        {
            // Keep track of whether we are already updating to ovoid an update circular reference.
            if (_updating)
                return false;

            try
            {
                _updating = true;
                var valueObject = valueProperty.Source;
                var assignObject = assignProperty.Source;
                if (valueObject == null || assignObject == null)
                    return false;

                if (assignProperty.CanChangeCollection && valueProperty.CanGetCollection)
                {
                    assignProperty.Clear();
                    foreach (var item in valueProperty.GetValue() as IList)
                        assignProperty.Add(Converter.Convert(item, assignProperty.ElementType));
                }

                var value = valueProperty.GetValue();
                var convertedValue = Converter<TAssign>.Convert(value);
                assignProperty.SetValue(convertedValue);
            }
            finally
            {
                _updating = false;
            }

            return true;
        }
    }
}
