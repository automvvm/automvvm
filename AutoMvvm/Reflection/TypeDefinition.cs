// --------------------------------------------------------------------------------
// <copyright file="TypeDefinition.cs" company="AutoMvvm Development Team">
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
using System.Linq;
using AutoMvvm.Design;

namespace AutoMvvm.Reflection
{
    /// <summary>
    /// Defines a complete type definition including generic parameters (if applicable).
    /// </summary>
    public class TypeDefinition : IEquatable<TypeDefinition>
    {
        private int _hashCode;

        /// <summary>
        /// Gets the base type or type definition.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the generic parameters.
        /// </summary>
        public IList<Type> GenericParameters { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeDefinition"/> class.
        /// </summary>
        /// <param name="type">The contracted type.</param>
        public TypeDefinition(Type type)
            : this(type, new Type[] { })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeDefinition"/> class.
        /// </summary>
        /// <param name="type">The type or generic type definition.</param>
        /// <param name="genericParameters">The generic parameters for constructing the type (if applicable).</param>
        public TypeDefinition(Type type, params Type[] genericParameters)
        {
            Type = type;
            GenericParameters = Array.AsReadOnly(genericParameters);
        }

        /// <summary>
        /// Creates the generic type from the type definition.
        /// </summary>
        /// <returns>The new generic type.</returns>
        public Type MakeGenericType() => Type.MakeGenericType(GenericParameters.ToArray());

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
        public bool Equals(TypeDefinition other)
        {
            // Null is always not equal.
            if (other == null)
                return false;

            // Compare the reference equality first.
            if (ReferenceEquals(this, other))
                return true;

            // If the type isn't equal or the number of generic parameters don't match, return false.
            var parameterCount = GenericParameters.Count;
            if (other.Type != Type || parameterCount != other.GenericParameters.Count)
                return false;

            // Compare the generic parameters.
            foreach (var index in Enumerable.Range(0, parameterCount - 1))
            {
                if (GenericParameters[index] != other.GenericParameters[index])
                    return false;
            }

            // The types are equal.
            return true;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            if (_hashCode != 0)
                return _hashCode;

            var hash = 17;
            hash = Hash.Combine(hash, Type.GetHashCode());
            foreach (var parameter in GenericParameters)
                hash = Hash.Combine(hash, parameter.GetHashCode());

            _hashCode = hash;
            return hash;
        }
    }
}