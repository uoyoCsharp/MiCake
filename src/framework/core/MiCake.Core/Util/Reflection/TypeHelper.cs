using System.Reflection;
using System.Runtime.CompilerServices;

namespace MiCake.Core.Util.Reflection
{
    public static class TypeHelper
    {
        public static bool IsFunc(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var type = obj.GetType();
            if (!type.GetTypeInfo().IsGenericType)
            {
                return false;
            }

            return type.GetGenericTypeDefinition() == typeof(Func<>);
        }

        public static bool IsFunc<TReturn>(object obj)
        {
            return obj != null && obj.GetType() == typeof(Func<TReturn>);
        }

        public static bool IsPrimitiveExtended(Type type, bool includeNullables = true, bool includeEnums = false)
        {
            if (IsPrimitiveExtendedInternal(type, includeEnums))
            {
                return true;
            }

            if (includeNullables &&
                type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return IsPrimitiveExtendedInternal(type.GenericTypeArguments[0], includeEnums);
            }

            return false;
        }

        public static Type? GetFirstGenericArgumentIfNullable(Type t)
        {
            if (t.GetGenericArguments().Length > 0 && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return t.GetGenericArguments().FirstOrDefault();
            }

            return t;
        }

        /// <summary>
        /// Get all parameters of the generic interface inherited by the type
        /// </summary>
        /// <param name="type">Inherited types</param>
        /// <param name="genericType">generic interface type</param>
        /// <returns>interface generic arguments</returns>
        public static Type[] GetGenericArguments(Type type, Type genericType)
        {
            return type.GetInterfaces()
                            .Where(i => IsGenericType(i))
                            .SelectMany(i => i.GetGenericArguments())
                            .ToArray();

            bool IsGenericType(Type type1)
                => type1.IsGenericType && type1.GetGenericTypeDefinition() == genericType;
        }

        /// <summary>
        /// Get generic interface inherited by the type
        /// </summary>
        /// <param name="type">Inherited types</param>
        /// <param name="genericType">generic interface type</param>
        /// <returns>Generic interface information for specific types</returns>
        public static Type? GetGenericInterface(Type type, Type genericType)
        {
            return type.GetInterfaces()
                            .Where(i => IsGenericType(i))
                            .FirstOrDefault();

            bool IsGenericType(Type type1)
                => type1.IsGenericType && type1.GetGenericTypeDefinition() == genericType;
        }

        public static bool IsImplementedGenericInterface(Type type, Type generic)
        {
            return type.GetInterfaces().Any(x => generic == (x.IsGenericType ? x.GetGenericTypeDefinition() : x));
        }

        public static bool IsOpenGeneric(Type type)
        {
            return type.GetTypeInfo().IsGenericTypeDefinition || type.GetTypeInfo().ContainsGenericParameters;
        }

        public static Type UnwrapNullableType(Type type) => Nullable.GetUnderlyingType(type) ?? type;

        public static bool IsNullableType(Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return !typeInfo.IsValueType
                   || typeInfo.IsGenericType
                   && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type MakeNullable(Type type, bool nullable = true)
            => IsNullableType(type) == nullable
                ? type
                : nullable
                    ? typeof(Nullable<>).MakeGenericType(type)
                    : UnwrapNullableType(type);

        public static bool IsNumeric(Type type)
        {
            type = UnwrapNullableType(type);

            return IsInteger(type)
                   || type == typeof(decimal)
                   || type == typeof(float)
                   || type == typeof(double);
        }

        public static bool IsInteger(Type type)
        {
            type = UnwrapNullableType(type);

            return type == typeof(int)
                   || type == typeof(long)
                   || type == typeof(short)
                   || type == typeof(byte)
                   || type == typeof(uint)
                   || type == typeof(ulong)
                   || type == typeof(ushort)
                   || type == typeof(sbyte)
                   || type == typeof(char);
        }

        public static bool IsSignedInteger(Type type)
            => type == typeof(int)
               || type == typeof(long)
               || type == typeof(short)
               || type == typeof(sbyte);

        public static bool IsAnonymousType(Type type)
            => type.Name.StartsWith("<>", StringComparison.Ordinal)
               && type.GetCustomAttributes(typeof(CompilerGeneratedAttribute), inherit: false).Length > 0
               && type.Name.Contains("AnonymousType");

        public static PropertyInfo? GetAnyProperty(Type type, string name)
        {
            var props = type.GetRuntimeProperties().Where(p => p.Name == name).ToList();
            if (props.Count > 1)
            {
                throw new AmbiguousMatchException();
            }

            return props.SingleOrDefault();
        }

        public static bool IsInstantiable(Type type) => IsInstantiable(type.GetTypeInfo());

        private static bool IsInstantiable(TypeInfo type)
            => !type.IsAbstract
               && !type.IsInterface
               && (!type.IsGenericType || !type.IsGenericTypeDefinition);

        public static Type UnwrapEnumType(Type type)
        {
            var isNullable = IsNullableType(type);
            var underlyingNonNullableType = isNullable ? UnwrapNullableType(type) : type;
            if (!underlyingNonNullableType.GetTypeInfo().IsEnum)
            {
                return type;
            }

            var underlyingEnumType = Enum.GetUnderlyingType(underlyingNonNullableType);
            return isNullable ? MakeNullable(underlyingEnumType) : underlyingEnumType;
        }

        public static Type? TryGetSequenceType(Type type)
            => TryGetElementType(type, typeof(IEnumerable<>))
               ?? TryGetElementType(type, typeof(IAsyncEnumerable<>));

        public static Type? TryGetElementType(Type type, Type interfaceOrBaseType)
        {
            if (type.GetTypeInfo().IsGenericTypeDefinition)
            {
                return null;
            }

            var types = GetGenericTypeImplementations(type, interfaceOrBaseType);

            Type? singleImplementation = null;
            foreach (var implementation in types)
            {
                if (singleImplementation == null)
                {
                    singleImplementation = implementation;
                }
                else
                {
                    singleImplementation = null;
                    break;
                }
            }

            return singleImplementation?.GetTypeInfo().GenericTypeArguments.FirstOrDefault();
        }

        public static IEnumerable<Type> GetGenericTypeImplementations(Type type, Type interfaceOrBaseType)
        {
            var typeInfo = type.GetTypeInfo();
            if (!typeInfo.IsGenericTypeDefinition)
            {
                var baseTypes = interfaceOrBaseType.GetTypeInfo().IsInterface
                    ? typeInfo.ImplementedInterfaces
                    : GetBaseTypes(type);
                foreach (var baseType in baseTypes)
                {
                    if (baseType.GetTypeInfo().IsGenericType
                        && baseType.GetGenericTypeDefinition() == interfaceOrBaseType)
                    {
                        yield return baseType;
                    }
                }

                if (type.GetTypeInfo().IsGenericType
                    && type.GetGenericTypeDefinition() == interfaceOrBaseType)
                {
                    yield return type;
                }
            }
        }

        public static IEnumerable<Type> GetBaseTypes(Type type)
        {
            type = type.GetTypeInfo().BaseType!;

            while (type != null)
            {
                yield return type;

                type = type.GetTypeInfo().BaseType!;
            }
        }

        public static IEnumerable<Type> GetTypesInHierarchy(Type type)
        {
            while (type != null)
            {
                yield return type;

                type = type.GetTypeInfo().BaseType!;
            }
        }

        public static ConstructorInfo? GetDeclaredConstructor(Type type, Type[] types)
        {
            types ??= Array.Empty<Type>();

            return type.GetTypeInfo().DeclaredConstructors
                .SingleOrDefault(
                    c => !c.IsStatic
                         && c.GetParameters().Select(p => p.ParameterType).SequenceEqual(types));
        }

        public static IEnumerable<PropertyInfo> GetPropertiesInHierarchy(Type type, string name)
        {
            do
            {
                var typeInfo = type.GetTypeInfo();
                foreach (var propertyInfo in typeInfo.DeclaredProperties)
                {
                    if (propertyInfo.Name.Equals(name, StringComparison.Ordinal)
                        && !(propertyInfo.GetMethod ?? propertyInfo.SetMethod)!.IsStatic)
                    {
                        yield return propertyInfo;
                    }
                }

                type = typeInfo.BaseType!;
            }
            while (type != null);
        }

        // Looking up the members through the whole hierarchy allows to find inherited private members.
        public static IEnumerable<MemberInfo> GetMembersInHierarchy(Type type)
        {
            do
            {
                // Do the whole hierarchy for properties first since looking for fields is slower.
                foreach (var propertyInfo in type.GetRuntimeProperties().Where(pi => !(pi.GetMethod ?? pi.SetMethod)!.IsStatic))
                {
                    yield return propertyInfo;
                }

                foreach (var fieldInfo in type.GetRuntimeFields().Where(f => !f.IsStatic))
                {
                    yield return fieldInfo;
                }

                type = type.BaseType!;
            }
            while (type != null);
        }

        public static IEnumerable<MemberInfo> GetMembersInHierarchy(Type type, string name)
            => GetMembersInHierarchy(type).Where(m => m.Name == name);

        public static object? GetDefaultValue(Type type)
        {
            if (!type.GetTypeInfo().IsValueType)
            {
                return null;
            }

            // A bit of perf code to avoid calling Activator.CreateInstance for common types and
            // to avoid boxing on every call. This is about 50% faster than just calling CreateInstance
            // for all value types.
            return _commonTypeDictionary.TryGetValue(type, out var value)
                ? value
                : Activator.CreateInstance(type);
        }

        public static IEnumerable<TypeInfo> GetConstructibleTypes(this Assembly assembly)
            => assembly.GetLoadableDefinedTypes().Where(
                t => !t.IsAbstract
                     && !t.IsGenericTypeDefinition);

        public static IEnumerable<TypeInfo> GetLoadableDefinedTypes(this Assembly assembly)
        {
            try
            {
                return assembly.DefinedTypes;
            }
            catch (ReflectionTypeLoadException ex)
            {
                return ex.Types.Where(t => t != null).Select(IntrospectionExtensions.GetTypeInfo!);
            }
        }

        private static readonly Dictionary<Type, object> _commonTypeDictionary = new()
        {
            { typeof(int), default(int) },
            { typeof(Guid), default(Guid) },
            { typeof(DateTime), default(DateTime) },
            { typeof(DateTimeOffset), default(DateTimeOffset) },
            { typeof(long), default(long) },
            { typeof(bool), default(bool) },
            { typeof(double), default(double) },
            { typeof(short), default(short) },
            { typeof(float), default(float) },
            { typeof(byte), default(byte) },
            { typeof(char), default(char) },
            { typeof(uint), default(uint) },
            { typeof(ushort), default(ushort) },
            { typeof(ulong), default(ulong) },
            { typeof(sbyte), default(sbyte) }
        };

        private static bool IsPrimitiveExtendedInternal(Type type, bool includeEnums)
        {
            if (type.IsPrimitive)
            {
                return true;
            }

            if (includeEnums && type.IsEnum)
            {
                return true;
            }

            return type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(Guid);
        }
    }
}
