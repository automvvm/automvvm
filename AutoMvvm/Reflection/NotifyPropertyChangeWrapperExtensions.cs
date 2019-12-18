// --------------------------------------------------------------------------------
// <copyright file="NotifyPropertyChangeWrapperExtensions.cs" company="AutoMvvm Development Team">
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
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CSharp;

namespace AutoMvvm.Reflection
{
    /// <summary>
    /// Extensions methods to build an object wrapper around any entity so changes can be hooked.
    /// </summary>
    /// <remarks>
    /// The only requirement is to make your properties virtual and class
    /// able to be inherited and you can hook any property change or method
    /// call.  It binds to and exposes all property changes through the
    /// <see cref="INotifyPropertyChanged"/> and <see cref="INotifyPropertyChanging"/>
    /// interfaces.
    /// </remarks>
    public static class NotifyPropertyChangeWrapperExtensions
    {
        private static readonly ConcurrentDictionary<Type, Type> _wrapperCache = new ConcurrentDictionary<Type, Type>();
        private static bool _supportsCodeDom = true;

        /// <summary>
        /// Wraps the given factory method with a notification wrapper factory method
        /// to automatically wrap the generated types with property change notifications.
        /// </summary>
        /// <param name="typeResolver">The factory to wrap.</param>
        /// <returns>A type resolver that auto-generates property change notifications.</returns>
        public static Func<Type, Type> WrapNotificationTypeResolver(this Func<Type, Type> typeResolver) =>
            new NotifyPropertyChangeTypeResolver(typeResolver).ResolveType;

        public static Type GetNotificationWrappedType(this Type sourceType)
        {
            if (sourceType == null)
                throw new NullReferenceException("The source object to wrap is null.");

            var wrapperType = _wrapperCache.GetOrAdd(sourceType, CreateType);
            return wrapperType ?? sourceType;
        }

        private static Type CreateType(Type sourceType)
        {
            ValidateSourceType(sourceType);

            // Make sure not to wrap a type that was already wrapped.
            var namespaceExtension = ".NotifyPropertyChange";
            if (sourceType.Namespace.EndsWith(namespaceExtension, StringComparison.Ordinal))
                return sourceType;

            var name = sourceType.Name;
            var customNamespace = $"{sourceType.Namespace}{namespaceExtension}";
            var code = GenerateTypeSource(sourceType, customNamespace);

            // No need to generate a type for this type, no properties to override.
            if (string.IsNullOrEmpty(code))
                return null;

            try
            {
                return CreateTypeNetFramework(sourceType, customNamespace, code);
            }
            catch (PlatformNotSupportedException)
            {
                _supportsCodeDom = false;
            }

            if (_supportsCodeDom)
                return null;

            return CreateTypeNetCore(sourceType, $"{customNamespace}.{name}", code);
        }

        /// <summary>
        /// Ensures the source type is valid for being wrapped.
        /// </summary>
        /// <param name="sourceType">The source type.</param>
        private static void ValidateSourceType(Type sourceType)
        {
            if (sourceType == null)
                throw new NullReferenceException("The source object to wrap is null.");

            var sourceTypeInfo = sourceType.GetTypeInfo();
            if (!sourceTypeInfo.IsClass || sourceTypeInfo.IsNotPublic || sourceTypeInfo.IsAbstract || sourceTypeInfo.IsInterface)
                throw new InvalidOperationException("The source class must be a class which supports public construction, private or abstract classes and interfaces are not supported.");

            if (sourceTypeInfo.IsCOMObject)
                throw new InvalidOperationException("COM objects are not supported for wrapping directly, you must build a .NET class around it if you want to do this.");

            if (sourceTypeInfo.IsGenericType && !sourceType.IsConstructedGenericType)
                throw new InvalidOperationException("Only a generic type with all type parameters satisfied are supported.  Please satisfy all generic parameters first.");

            if (sourceTypeInfo.IsSealed)
                throw new InvalidOperationException("The source must not be a sealed class, inheritance is required.");
        }

        private static Type CreateTypeNetFramework(Type sourceType, string customNamespace, string sourceCode)
        {
            var referencedAssemblies = sourceType.GetTypeInfo().Assembly.GetReferencedAssemblies();
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => referencedAssemblies.Any(referenceName => referenceName.FullName == assembly.FullName));
            var references = new[] { "mscorlib.dll", sourceType.Assembly.Location }
                .Concat(loadedAssemblies.Select(assembly => assembly.Location))
                .ToArray();

            using (var csc = new CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v4.0" } }))
            {
                var tempPath = Path.GetTempPath();
                var fileName = $"{customNamespace}.{sourceType.Name}{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.dll";
                var parameters = new CompilerParameters(references, Path.Combine(tempPath, fileName), false)
                {
                    GenerateExecutable = false,
                    GenerateInMemory = true
                };

                var results = csc.CompileAssemblyFromSource(parameters, sourceCode);
                if (results.Errors.Count > 0)
                {

                    var failureMessage = $"Failed to wrap an instance of {sourceType.FullName}:";
                    failureMessage += Environment.NewLine + string.Join(Environment.NewLine, results.Errors.Cast<CompilerError>().ToList().Select(error => error.ErrorText));
                    throw new InvalidOperationException(failureMessage);
                }
                var type = results.CompiledAssembly.GetType($"{customNamespace}.{sourceType.Name}");

                // Should be generating the file in memory, but if not this will ensure no files are left behind.
                results.TempFiles.KeepFiles = false;
                results.TempFiles.Delete();
                return type;
            }
        }

        private static Type CreateTypeNetCore(Type sourceType, string targetType, string sourceCode)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

            var assemblyName = Path.GetRandomFileName();
            var referencedAssemblies = sourceType.GetTypeInfo().Assembly.GetReferencedAssemblies();
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assembly => referencedAssemblies.Any(referenceName => referenceName.FullName == assembly.FullName));
            var referencedMetadata = loadedAssemblies.Select(assembly => MetadataReference.CreateFromFile(assembly.Location));
            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(INotifyPropertyChanging).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(sourceType.GetTypeInfo().Assembly.Location)
            }.Concat(referencedMetadata).ToArray();

            var compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    var failureMessage = $"Failed to wrap an instance of {sourceType.FullName}:";
                    failureMessage += Environment.NewLine + string.Join(Environment.NewLine, failures.Select(failure => failure.ToString()));
                    throw new InvalidOperationException(failureMessage);
                }

                ms.Seek(0, SeekOrigin.Begin);

                Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                return assembly.GetType(targetType);
            }
        }

        private static string GenerateTypeSource(Type sourceType, string customNamespace)
        {
            var implementedInterfaces = new HashSet<Type>(sourceType.GetInterfaces());
            var sourceImplementsPropertyChanging = implementedInterfaces.Contains(typeof(INotifyPropertyChanging));
            var sourceImplementsPropertyChanged = implementedInterfaces.Contains(typeof(INotifyPropertyChanged));
            var properties = sourceType.GetRuntimeProperties();
            var sourceCode = new StringBuilder();
            sourceCode.AppendLine("using System;");
            sourceCode.AppendLine("using System.ComponentModel;");
            sourceCode.AppendLine();
            sourceCode.AppendLine($"namespace {customNamespace}");
            sourceCode.AppendLine("{");
            sourceCode.AppendLine($"    public class {sourceType.Name} : {sourceType.FullName}, {nameof(INotifyPropertyChanging)}, {nameof(INotifyPropertyChanged)}");
            sourceCode.AppendLine("    {");

            // Implement property changing and property changed only if the source type hasn't implemented it already.
            if (!sourceImplementsPropertyChanging)
            {
                sourceCode.AppendLine($"        public event {nameof(PropertyChangingEventHandler)} {nameof(INotifyPropertyChanging.PropertyChanging)};");
                sourceCode.AppendLine();
            }

            if (!sourceImplementsPropertyChanged)
            {
                sourceCode.AppendLine($"        public event {nameof(PropertyChangedEventHandler)} {nameof(INotifyPropertyChanged.PropertyChanged)};");
                sourceCode.AppendLine();
            }

            // Keep track of whether we need to override this class.
            var propertyOveridden = false;
            foreach (var property in properties)
            {
                // Get whether the base get and set methods are available and virtual.
                var getMethodAvailable = property.CanRead && property.GetMethod?.IsVirtual == true;
                var setMethodAvailable = property.CanWrite && property.SetMethod?.IsVirtual == true;

                // If neither the get or set method is available, we don't want to generate a property override.
                if (!getMethodAvailable && !setMethodAvailable)
                    continue;

                // Signal that we are overriding at least one property.
                propertyOveridden = true;
                sourceCode.Append(GenerateProperty(property.Name, property.PropertyType, getMethodAvailable, setMethodAvailable));
            }

            // If we are not overriding any properties on this object, just return null to
            // indicate we don't need to override this type.
            if (!propertyOveridden)
                return string.Empty;

            if (!sourceImplementsPropertyChanging)
            {
                sourceCode.Append(GenerateEventCall(nameof(INotifyPropertyChanging.PropertyChanging), nameof(PropertyChangingEventArgs)));
            }

            if (!sourceImplementsPropertyChanged)
            {
                sourceCode.Append(GenerateEventCall(nameof(INotifyPropertyChanged.PropertyChanged), nameof(PropertyChangedEventArgs)));
            }

            sourceCode.AppendLine("    }");
            sourceCode.AppendLine("}");

            return sourceCode.ToString();
        }

        private static string GenerateProperty(string propertyName, Type propertyType, bool getMethodAvailable, bool setMethodAvailable)
        {
            var propertyString = new StringBuilder();

            // Define the property override.
            propertyString.AppendLine($"        public override {propertyType.FullName} {propertyName}");
            propertyString.AppendLine("        {");

            // If the get method is available, create a base property get reference to pass through the value from the base class.
            if (getMethodAvailable)
            {
                propertyString.AppendLine($"            get {{ return base.{propertyName}; }}");
            }

            // If the set method is available, create a base property set reference and wrap with property notification events.
            if (setMethodAvailable)
            {
                // Wrap the base changes in property change notifications.
                propertyString.AppendLine("            set");
                propertyString.AppendLine("            {");
                propertyString.AppendLine($"                On{nameof(INotifyPropertyChanging.PropertyChanging)}(new {nameof(PropertyChangingEventArgs)}(\"{propertyName}\"));");
                propertyString.AppendLine($"                base.{propertyName} = value;");
                propertyString.AppendLine($"                On{nameof(INotifyPropertyChanged.PropertyChanged)}(new {nameof(PropertyChangedEventArgs)}(\"{propertyName}\"));");
                propertyString.AppendLine("            }");
            }

            propertyString.AppendLine("        }");
            propertyString.AppendLine();
            return propertyString.ToString();
        }

        private static string GenerateEventCall(string eventName, string eventArgsName)
        {
            var eventString = new StringBuilder();
            eventString.AppendLine($"        protected virtual void On{eventName}({eventArgsName} e)");
            eventString.AppendLine("        {");
            eventString.AppendLine($"            var eventHandler = {eventName};");
            eventString.AppendLine("            if (eventHandler == null)");
            eventString.AppendLine("                return;");
            eventString.AppendLine();
            eventString.AppendLine("            eventHandler(this, e);");
            eventString.AppendLine("        }");
            eventString.AppendLine();
            return eventString.ToString();
        }
    }
}
