// --------------------------------------------------------------------------------
// <copyright file="MethodInfoExtensions.cs" company="AutoMvvm Development Team">
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
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AutoMvvm.Reflection
{
    /// <summary>
    /// Extension methods for <see cref="MethodInfo"/> delegate generation and caching.
    /// </summary>
    public static class MethodInfoExtensions
    {
        /// <summary>
        /// Gets an <see cref="Action{object, T}"/> delegate for the specified type and method name, with
        /// the method target unbound if found.
        /// </summary>
        /// <typeparam name="T">The type of the first parameter.</typeparam>
        /// <param name="methodName">The name of the method to create an action for.</param>
        /// <returns>
        /// An <see cref="Action{object, T}"/> delegate for the specified method, or <c>null</c>
        /// if none found.
        /// </returns>
        public static Action<object, T> GetAction<T>(this Type type, string methodName)
        {
            var method = GetBestMethodMatch(type, methodName, typeof(T));
            if (method == null)
                return null;

            return method.GetAction<T>();
        }

        /// <summary>
        /// Gets an <see cref="Action{object}"/> delegate for the specified method information, with
        /// the method target unbound.
        /// </summary>
        /// <typeparam name="T">The type of the first parameter.</typeparam>
        /// <param name="methodInfo">The reflected method information to create an action for.</param>
        /// <returns>
        /// An <see cref="Action{object}"/> delegate for the specified method.
        /// </returns>
        public static Action<object> GetAction(this MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));

            var type = methodInfo.DeclaringType;
            var parameter = Expression.Parameter(typeof(object));
            var instance = Expression.TypeAs(parameter, type);
            var methodCall = Expression.Call(instance, methodInfo);
            return Expression.Lambda<Action<object>>(methodCall, parameter).Compile();
        }

        /// <summary>
        /// Gets an <see cref="Action{object, T}"/> delegate for the specified method information, with
        /// the method target unbound.
        /// </summary>
        /// <typeparam name="T">The type of the first parameter.</typeparam>
        /// <param name="methodInfo">The reflected method information to create an action for.</param>
        /// <returns>
        /// An <see cref="Action{object, T}"/> delegate for the specified method.
        /// </returns>
        public static Action<object, T> GetAction<T>(this MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));

            var type = methodInfo.DeclaringType;
            var parameter1 = Expression.Parameter(typeof(object));
            var parameter2 = Expression.Parameter(typeof(T));
            var instance = Expression.TypeAs(parameter1, type);
            var methodParameters = methodInfo.GetParameters();
            var castParameter2 = Expression.Convert(parameter2, methodParameters[0].ParameterType);
            var methodCall = Expression.Call(instance, methodInfo, castParameter2);
            return Expression.Lambda<Action<object, T>>(methodCall, parameter1, parameter2).Compile();
        }

        /// <summary>
        /// Gets an <see cref="Action{object, T1, T2}"/> delegate for the specified method information, with
        /// the method target unbound.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <param name="methodInfo">The reflected method information to create an action for.</param>
        /// <returns>
        /// An <see cref="Action{object, T1, T2}"/> delegate for the specified method.
        /// </returns>
        public static Action<object, T1, T2> GetAction<T1, T2>(this MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));

            var type = methodInfo.DeclaringType;
            var parameter1 = Expression.Parameter(typeof(object));
            var parameter2 = Expression.Parameter(typeof(T1));
            var parameter3 = Expression.Parameter(typeof(T2));
            var instance = Expression.TypeAs(parameter1, type);
            var methodParameters = methodInfo.GetParameters();
            var castParameter2 = Expression.Convert(parameter2, methodParameters[0].ParameterType);
            var castParameter3 = Expression.Convert(parameter3, methodParameters[1].ParameterType);
            var methodCall = Expression.Call(instance, methodInfo, castParameter2, castParameter3);
            return Expression.Lambda<Action<object, T1, T2>>(methodCall, parameter1, parameter2, parameter3).Compile();
        }

        /// <summary>
        /// Gets an <see cref="Action{object, T1, T2, T3}"/> delegate for the specified method information, with
        /// the method target unbound.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter.</typeparam>
        /// <typeparam name="T2">The type of the second parameter.</typeparam>
        /// <typeparam name="T3">The type of the third parameter.</typeparam>
        /// <param name="methodInfo">The reflected method information to create an action for.</param>
        /// <returns>
        /// An <see cref="Action{object, T1, T2, T3}"/> delegate for the specified method.
        /// </returns>
        public static Action<object, T1, T2, T3> GetAction<T1, T2, T3>(this MethodInfo methodInfo)
        {
            if (methodInfo == null)
                throw new ArgumentNullException(nameof(methodInfo));

            var type = methodInfo.DeclaringType;
            var parameter1 = Expression.Parameter(typeof(object));
            var parameter2 = Expression.Parameter(typeof(T1));
            var parameter3 = Expression.Parameter(typeof(T2));
            var parameter4 = Expression.Parameter(typeof(T3));
            var instance = Expression.TypeAs(parameter1, type);
            var methodParameters = methodInfo.GetParameters();
            var castParameter2 = Expression.Convert(parameter2, methodParameters[0].ParameterType);
            var castParameter3 = Expression.Convert(parameter3, methodParameters[1].ParameterType);
            var castParameter4 = Expression.Convert(parameter4, methodParameters[2].ParameterType);
            var methodCall = Expression.Call(instance, methodInfo, castParameter2, castParameter3, castParameter4);
            return Expression.Lambda<Action<object, T1, T2, T3>>(methodCall, parameter1, parameter2, parameter3, parameter4).Compile();
        }

        private static MethodInfo GetBestMethodMatch(Type type, string methodName, params Type[] parameterTypes)
        {
            var methods = type.GetRuntimeMethods();
            MethodInfo matchingMethod = null;
            MethodInfo fallBackMethod = null;
            foreach (var method in methods.Where(method => method.Name == methodName && method.IsPublic && !method.IsAbstract))
            {
                var matchType = CheckParametersMatch(method, parameterTypes);
                if (matchType == MatchType.Exact)
                    return method;

                if (matchType == MatchType.Compatible)
                    matchingMethod = method;

                if (matchType == MatchType.None)
                    continue;

                fallBackMethod = method;
            }

            return matchingMethod ?? fallBackMethod;
        }

        private static MatchType CheckParametersMatch(MethodInfo method, params Type[] parameterTypes)
        {
            var methodParameters = method.GetParameters();
            var parameterIndex = 0;
            if (methodParameters.Length != parameterTypes.Length)
                return MatchType.None;

            var matchType = MatchType.Exact;
            foreach (var methodParameter in methodParameters)
            {
                // This method parameter is an exact match.
                if (parameterTypes[parameterIndex] == methodParameter.ParameterType)
                    continue;

                // This method parameter is a compatible match.
                if (matchType == MatchType.Exact && methodParameter.ParameterType.GetTypeInfo().IsAssignableFrom(parameterTypes[parameterIndex].GetTypeInfo()))
                {
                    matchType = MatchType.Compatible;
                    continue;
                }

                // This method parameter is a non-ideal castable match.
                if ((matchType == MatchType.Exact || matchType == MatchType.Compatible) && parameterTypes[parameterIndex].GetTypeInfo().IsAssignableFrom(methodParameter.ParameterType.GetTypeInfo()))
                {
                    matchType = MatchType.Castable;
                    continue;
                }

                // This method is not a valid match.
                return MatchType.None;
            }

            return matchType;
        }

        private enum MatchType
        {
            None,
            Exact,
            Compatible,
            Castable
        }

        private static class TypeCache
        {
            private static readonly ConditionalWeakTable<object, TypeDetails> _typeCache = new ConditionalWeakTable<object, TypeDetails>();

            /// <summary>
            /// Gets the <see cref="TypeDetails"/> of the given object instance.
            /// </summary>
            /// <param name="instance">The object to get type details for.</param>
            /// <returns>A <see cref="TypeDetails"/> instance containing all the reflected details of the given object.</returns>
            public static TypeDetails GetTypeDetails(object instance)
            {
                if (instance == null)
                    throw new ArgumentNullException(nameof(instance));

                return _typeCache.GetValue(instance, instance => new TypeDetails(instance.GetType()));
            }
        }

        public class TypeDetails
        {
            public TypeDetails(Type type)
            {
                Type = type;
            }

            public Type Type { get; }
        }
    }
}
