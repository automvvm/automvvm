// --------------------------------------------------------------------------------
// <copyright file="PropertyDetails.cs" company="AutoMvvm Development Team">
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
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMvvm.Design;

namespace AutoMvvm.Reflection
{
    /// <summary>
    /// Represents the property details of an instance member.
    /// </summary>
    public abstract class PropertyDetails : IEquatable<PropertyDetails>
    {
        private WeakReference<object> _source;

        /// <summary>
        /// Gets the source object.
        /// </summary>
        public object Source => _source.TryGetTarget(out var result) ? result : null;

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the type of the property.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the type of the elements in the collection (if applicable).
        /// </summary>
        public Type ElementType { get; }

        /// <summary>
        /// Gets a value indicating whether this property supports adding values to its collection.
        /// </summary>
        public abstract bool CanGetCollection { get; }

        /// <summary>
        /// Gets a value indicating whether this property supports adding values to its collection.
        /// </summary>
        public abstract bool CanChangeCollection { get; }

        /// <summary>
        /// Gets a value indicating whether this property supports getting values.
        /// </summary>
        public abstract bool CanGet { get; }

        /// <summary>
        /// Gets a value indicating whether this property supports setting values.
        /// </summary>
        public abstract bool CanSet { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDetails"/> class.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="type"></param>
        /// <param name="elementType">The type of the elements in the collection (if applicable).</param>
        protected PropertyDetails(object source, string name, Type type, Type elementType)
        {
            _source = new WeakReference<object>(source);
            Name = name;
            Type = type;
            ElementType = elementType;
        }

        /// <summary>
        /// Creates an instance of the <see cref="PropertyDetails"/> with the given source and property details.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="property">The property info.</param>
        /// <returns>An instance of the <see cref="PropertyDetails"/>.</returns>
        public static PropertyDetails Create(object source, PropertyInfo property)
        {
            var propertyDetailsType = typeof(PropertyDetails<>).GetGenericType(property.PropertyType);
            return (PropertyDetails)propertyDetailsType.CreateInstance(source, property);
        }

        /// <summary>
        /// Creates an instance of the <see cref="PropertyDetails{T}"/> with the given source and property details.
        /// </summary>
        /// <typeparam name="T">The type of the property value.</typeparam>
        /// <param name="property">The property expression.</param>
        /// <returns>An instance of the <see cref="PropertyDetails{T}"/>.</returns>
        public static PropertyDetails<T> Create<T>(Expression<Func<T>> property) => new PropertyDetails<T>(property);

        /// <summary>
        /// Indicates whether the left object is equal to the right.
        /// </summary>
        /// <param name="left">The left object to compare.</param>
        /// <param name="right">The right object to compare.</param>
        /// <returns><c>true</c> if the left object is equal to the right; otherwise, <c>false</c>.</returns>
        public static bool operator ==(PropertyDetails left, PropertyDetails right) =>
            (left == null && right == null) || left?.Equals(right) == true;

        /// <summary>
        /// Indicates whether the left object is not equal to the right.
        /// </summary>
        /// <param name="left">The left object to compare.</param>
        /// <param name="right">The right object to compare.</param>
        /// <returns><c>true</c> if the left object is not equal to the right; otherwise, <c>false</c>.</returns>
        public static bool operator !=(PropertyDetails left, PropertyDetails right) =>
            !(left == right);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj) => Equals(obj as PropertyDetails);

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>.</returns>
        public bool Equals(PropertyDetails other)
        {
            var source = Source;
            if (source == null)
                return false;

            if (!ReferenceEquals(other?.Source, source))
                return false;

            return other.Name == Name;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            var source = Source;
            var hash = (source != null) ? source.GetType().GetHashCode() : 17;
            return Hash.Combine(hash, Name.GetHashCode());
        }

        /// <summary>
        /// Gets the element type for a given collection type.
        /// </summary>
        /// <param name="type">A collection type.</param>
        /// <returns>The element type, or <c>null</c> if it isn't a collection.</returns>
        protected static Type GetElementType(Type type)
        {
            if (type?.IsGenericType == true && typeof(IEnumerable<>) == type.GetGenericTypeDefinition())
                return type.GetGenericArguments().FirstOrDefault();

            if (typeof(string).IsAssignableFrom(type))
                return null;

            if (typeof(IEnumerable).IsAssignableFrom(type))
                return typeof(object);

            return null;
        }
    }

    /// <summary>
    /// Represents the property details of an instance member.
    /// </summary>
    /// <typeparam name="T">The type of data for this property.</typeparam>
    public class PropertyDetails<T> : PropertyDetails
    {
        /// <summary>
        /// Gets a value indicating whether this property supports adding values to its collection.
        /// </summary>
        public override bool CanGetCollection => CanGet && (GetValue() as IEnumerable) != null;

        /// <summary>
        /// Gets a value indicating whether this property supports adding values to its collection.
        /// </summary>
        public override bool CanChangeCollection
        {
            get
            {
                if (!CanGet)
                    return false;

                var value = GetValue();
                if (value is IList list)
                    return !list.IsReadOnly;

                if (ElementType == null)
                    return false;

                var valueType = value?.GetType();
                var collectionType = typeof(ICollection<>).MakeGenericType(ElementType);
                return collectionType.IsAssignableFrom(valueType) && !(bool)collectionType.GetProperty(nameof(ICollection<int>.IsReadOnly)).GetValue(value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this property supports getting values.
        /// </summary>
        public override bool CanGet => GetMethod != null;

        /// <summary>
        /// Gets a value indicating whether this property supports setting values.
        /// </summary>
        public override bool CanSet => SetMethod != null;

        /// <summary>
        /// Gets the property info of the property.
        /// </summary>
        protected Func<object, T> GetMethod { get; }

        /// <summary>
        /// Gets the property info of the property.
        /// </summary>
        protected Action<object, T> SetMethod { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDetails"/> class.
        /// </summary>
        /// <param name="property">The property to get the details of.</param>
        public PropertyDetails(Expression<Func<T>> property)
            : this(GetSource(property), GetPropertyInfo(property))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDetails"/> class.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="propertyInfo">The property info.</param>
        public PropertyDetails(object source, PropertyInfo propertyInfo)
            : this(source, propertyInfo?.Name, GetElementType(propertyInfo?.PropertyType), WrapGetMethod(propertyInfo?.GetGetMethod()), WrapSetMethod(propertyInfo?.GetSetMethod()))
        {
            if (!propertyInfo.DeclaringType.IsAssignableFrom(source.GetType()))
                throw new InvalidCastException("The given source entity does not declare this property.");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDetails"/> class.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="type"></param>
        /// <param name="elementType">The type of the elements in the collection (if applicable).</param>
        /// <param name="getMethod">The method for getting the value of the property.</param>
        /// <param name="setMethod">The method for setting the value of the property.</param>
        public PropertyDetails(object source, string name, Type elementType, Func<object, T> getMethod, Action<object, T> setMethod)
            : base(source, name, typeof(T), elementType)
        {
            GetMethod = getMethod;
            SetMethod = setMethod;
        }

        /// <summary>
        /// Gets the value of this property.
        /// </summary>
        /// <returns>The current value of this property.</returns>
        public T GetValue() => GetMethod.Invoke(Source);

        /// <summary>
        /// Sets the value of this property.
        /// </summary>
        /// <param name="value">The new value of this property.</param>
        public void SetValue(T value) => SetMethod?.Invoke(Source, value);

        /// <summary>
        /// Adds the given value to the collection.
        /// </summary>
        /// <param name="value">The item to add.</param>
        public void Add(object value)
        {
            if (!CanChangeCollection)
                throw new InvalidOperationException("This types does not support collection modification.");

            var collection = GetValue() as IList;
            collection.Add(value);
        }

        /// <summary>
        /// Removes the value from the given index.
        /// </summary>
        /// <param name="value">The item to remove.</param>
        public void Remove(object value)
        {
            if (!CanChangeCollection)
                throw new InvalidOperationException("This types does not support collection modification.");

            var collection = GetValue() as IList;
            collection?.Remove(value);
        }

        /// <summary>
        /// Removes the value from the given index.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            if (!CanChangeCollection)
                throw new InvalidOperationException("This types does not support collection modification.");

            var collection = GetValue() as IList;
            collection?.RemoveAt(index);
        }

        /// <summary>
        /// Clears the list of items in the collection.
        /// </summary>
        public void Clear()
        {
            if (!CanChangeCollection)
                throw new InvalidOperationException("This types does not support collection modification.");

            var collection = GetValue() as IList;
            collection?.Clear();
        }

        private static Func<object, T> WrapGetMethod(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                return null;

            var sourceType = methodInfo.DeclaringType;
            var valueType = methodInfo.ReturnType;
            var sourceParameter = Expression.Parameter(typeof(object));
            var getMethod = Expression.Call(Expression.Convert(sourceParameter, sourceType), methodInfo);
            return Expression.Lambda<Func<object, T>>(getMethod, sourceParameter).Compile();
        }

        private static Action<object, T> WrapSetMethod(MethodInfo methodInfo)
        {
            if (methodInfo == null)
                return null;

            var sourceType = methodInfo.DeclaringType;
            var valueParameters = methodInfo.GetParameters();
            var sourceParameter = Expression.Parameter(typeof(object));
            var valueParameter = Expression.Parameter(typeof(T));
            var setMethod = Expression.Call(Expression.Convert(sourceParameter, sourceType), methodInfo, valueParameter);
            return Expression.Lambda<Action<object, T>>(setMethod, sourceParameter, valueParameter).Compile();
        }

        private static object GetSource(Expression<Func<T>> property)
        {
            var memberExpression = GetMemberExpression(property);
            if (memberExpression == null)
                return null;

            return Expression.Lambda<Func<object>>(memberExpression.Expression).Compile()?.Invoke();
        }

        private static PropertyInfo GetPropertyInfo(Expression<Func<T>> property)
        {
            var memberExpression = GetMemberExpression(property);
            if (memberExpression == null)
                return null;

            return (PropertyInfo)memberExpression.Member;
        }

        private static MemberExpression GetMemberExpression(Expression<Func<T>> property)
        {
            // Unwrap the cast expressions.
            var methodBody = property.Body;
            if (methodBody.NodeType == ExpressionType.TypeAs)
            {
                methodBody = ((UnaryExpression)methodBody).Operand;
            }

            if (methodBody.NodeType == ExpressionType.Convert)
            {
                methodBody = ((UnaryExpression)methodBody).Operand;
            }

            return methodBody as MemberExpression;
        }
    }
}
