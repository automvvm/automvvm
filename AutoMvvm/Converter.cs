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
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Numerics;
using AutoMvvm.Reflection;

namespace AutoMvvm
{
    /// <summary>
    /// Defines a general value converter.
    /// </summary>
    public static class Converter
    {
        private static readonly ConcurrentDictionary<Tuple<Type, Type>, Func<object, object>> _converters = new ConcurrentDictionary<Tuple<Type, Type>, Func<object, object>>();

        /// <summary>
        /// Converts the value given to the type <paramref name="convertType"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="convertType">The type to convert to.</param>
        /// <returns>The converted value.</returns>
        public static object Convert(object value, Type convertType)
        {
            if (convertType == null)
                return value;

            var valueType = value?.GetType() ?? typeof(object);
            var converter = GetConverter(valueType, convertType);
            return converter(value);
        }

        private static Func<object, object> GetConverter(Type valueType, Type convertType)
        {
            return _converters.GetOrAdd(
                Tuple.Create(convertType, valueType),
                (Tuple<Type, Type> key) =>
                {
                    var converterType = typeof(Converter<,>).GetGenericType(key.Item1, key.Item2);
                    var converterMethod = converterType.GetMethod(nameof(Converter<int, int>.Convert));
                    var valueParam = Expression.Parameter(typeof(object));
                    var converterCallExpression = Expression.Call(null, converterMethod, Expression.Convert(valueParam, key.Item2));
                    return Expression.Lambda<Func<object, object>>(Expression.Convert(converterCallExpression, typeof(object)), valueParam).Compile();
                });
        }
    }

    /// <summary>
    /// Defines a value converter to type <typeparamref name="TConvert"/>.
    /// </summary>
    /// <typeparam name="TConvert">The type to of the value convert to.</typeparam>
    public static class Converter<TConvert>
    {
        /// <summary>
        /// Converts the value given of type <typeparamref name="TValue"/> to the type <typeparamref name="TConvert"/>.
        /// </summary>
        /// <typeparam name="TValue">The type of the value to convert.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static TConvert Convert<TValue>(TValue value) => Converter<TConvert, TValue>.Convert(value);
    }

    /// <summary>
    /// Defines a value converter to type <typeparamref name="TConvert"/> from type <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TConvert">The type of the value to convert to.</typeparam>
    /// <typeparam name="TValue">The type of the value to convert.</typeparam>
    public static class Converter<TConvert, TValue>
    {
        private static readonly Func<TValue, TConvert> _converter;

        static Converter()
        {
            _converter = GetConverter();
        }

        /// <summary>
        /// Converts the value given of type <typeparamref name="TValue"/> to the type <typeparamref name="TConvert"/>.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static TConvert Convert(TValue value) => _converter(value);

        private static Func<TValue, TConvert> GetConverter()
        {
            if (typeof(TConvert).IsAssignableFrom(typeof(TValue)))
                return value => (TConvert)(object)value;

            if (typeof(TConvert).IsValueType && !typeof(string).IsAssignableFrom(typeof(TValue)) && !typeof(TValue).IsValueType)
                return value => (value as object == null) ? default : (TConvert)(object)value;

            // A string value should cast to the target types using their underlying parse call.
            if (typeof(string).IsAssignableFrom(typeof(TValue)))
            {
                if (typeof(TConvert) == typeof(byte))
                    return value => (TConvert)(object)(string.IsNullOrEmpty((string)(object)value) ? default : byte.Parse((string)(object)value));

                if (typeof(TConvert) == typeof(short))
                    return value => (TConvert)(object)(string.IsNullOrEmpty((string)(object)value) ? default : short.Parse((string)(object)value));

                if (typeof(TConvert) == typeof(int))
                    return value => (TConvert)(object)(string.IsNullOrEmpty((string)(object)value) ? default : int.Parse((string)(object)value));

                if (typeof(TConvert) == typeof(long))
                    return value => (TConvert)(object)(string.IsNullOrEmpty((string)(object)value) ? default : long.Parse((string)(object)value));

                if (typeof(TConvert) == typeof(float))
                    return value => (TConvert)(object)(string.IsNullOrEmpty((string)(object)value) ? default : float.Parse((string)(object)value));

                if (typeof(TConvert) == typeof(double))
                    return value => (TConvert)(object)(string.IsNullOrEmpty((string)(object)value) ? default : double.Parse((string)(object)value));

                if (typeof(TConvert) == typeof(decimal))
                    return value => (TConvert)(object)(string.IsNullOrEmpty((string)(object)value) ? default : decimal.Parse((string)(object)value));

                if (typeof(TConvert) == typeof(BigInteger))
                    return value => (TConvert)(object)(string.IsNullOrEmpty((string)(object)value) ? default : BigInteger.Parse((string)(object)value));

                if (typeof(TConvert) == typeof(Complex))
                {
                    return value =>
                    {
                        if (string.IsNullOrEmpty((string)(object)value))
                            return (TConvert)(object)default(Complex);

                        var coordinateStrings = ((string)(object)value).Trim('(', ')').Split(',');
                        if (coordinateStrings.Length < 2)
                            return (TConvert)(object)default(Complex);

                        return (TConvert)(object)Complex.FromPolarCoordinates(double.Parse(coordinateStrings[0]), double.Parse(coordinateStrings[1]));
                    };
                }
            }

            if (typeof(TConvert) == typeof(string))
                return value => (TConvert)(object)value.ToString();

            return value => (TConvert)(object)value;
        }
    }
}
