// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.TypeSystem
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Documents.Linq
{
  internal static class TypeSystem
  {
    public static Type GetElementType(Type type) => TypeSystem.GetElementType(type, new HashSet<Type>());

    public static string GetMemberName(this MemberInfo memberInfo)
    {
      JsonPropertyAttribute customAttribute1 = memberInfo.GetCustomAttribute<JsonPropertyAttribute>(true);
      if (customAttribute1 != null && !string.IsNullOrEmpty(customAttribute1.PropertyName))
        return customAttribute1.PropertyName;
      if (memberInfo.DeclaringType.GetCustomAttribute<DataContractAttribute>(true) != null)
      {
        DataMemberAttribute customAttribute2 = memberInfo.GetCustomAttribute<DataMemberAttribute>(true);
        if (customAttribute2 != null && !string.IsNullOrEmpty(customAttribute2.Name))
          return customAttribute2.Name;
      }
      return memberInfo.Name;
    }

    private static Type GetElementType(Type type, HashSet<Type> visitedSet)
    {
      if (visitedSet.Contains(type))
        return (Type) null;
      visitedSet.Add(type);
      if (type.IsArray)
        return type.GetElementType();
      Type left = (Type) null;
      if (type.IsInterface() && type.IsGenericType() && (object) type.GetGenericTypeDefinition() == (object) typeof (IEnumerable<>))
        left = TypeSystem.GetMoreSpecificType(left, CustomTypeExtensions.GetGenericArguments(type)[0]);
      foreach (Type type1 in CustomTypeExtensions.GetInterfaces(type))
        left = TypeSystem.GetMoreSpecificType(left, TypeSystem.GetElementType(type1, visitedSet));
      if ((object) type.GetBaseType() != null && (object) type.GetBaseType() != (object) typeof (object))
        left = TypeSystem.GetMoreSpecificType(left, TypeSystem.GetElementType(type.GetBaseType(), visitedSet));
      return left;
    }

    private static Type GetMoreSpecificType(Type left, Type right)
    {
      if ((object) left != null && (object) right != null)
        return CustomTypeExtensions.IsAssignableFrom(right, left) || !CustomTypeExtensions.IsAssignableFrom(left, right) ? left : right;
      Type type = left;
      return (object) type != null ? type : right;
    }

    public static bool IsAnonymousType(this Type type) => CustomTypeExtensions.GetCustomAttributes(type, typeof (CompilerGeneratedAttribute), false).Any<Attribute>() & type.FullName.Contains("AnonymousType");

    public static bool IsEnumerable(this Type type) => (object) type == (object) typeof (Enumerable) || type.IsGenericType() && (object) type.GetGenericTypeDefinition() == (object) typeof (IEnumerable<>) || ((IEnumerable<Type>) CustomTypeExtensions.GetInterfaces(type)).Where<Type>((Func<Type, bool>) (interfaceType => interfaceType.IsGenericType() && (object) interfaceType.GetGenericTypeDefinition() == (object) typeof (IEnumerable<>))).FirstOrDefault<Type>() != null;

    public static bool IsExtensionMethod(this MethodInfo methodInfo) => methodInfo.GetCustomAttribute(typeof (ExtensionAttribute)) != null;

    public static bool IsNullable(this Type type) => type.IsGenericType() && (object) type.GetGenericTypeDefinition() == (object) typeof (Nullable<>);

    public static Type NullableUnderlyingType(this Type type) => type.IsNullable() ? CustomTypeExtensions.GetGenericArguments(type)[0] : type;
  }
}
