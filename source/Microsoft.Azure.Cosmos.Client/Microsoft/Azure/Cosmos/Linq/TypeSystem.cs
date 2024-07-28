// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.TypeSystem
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal static class TypeSystem
  {
    public static Type GetElementType(Type type) => TypeSystem.GetElementType(type, new HashSet<Type>());

    public static string GetMemberName(
      this MemberInfo memberInfo,
      CosmosLinqSerializerOptions linqSerializerOptions = null)
    {
      string name = (string) null;
      JsonPropertyAttribute customAttribute1 = memberInfo.GetCustomAttribute<JsonPropertyAttribute>(true);
      if (customAttribute1 != null && !string.IsNullOrEmpty(customAttribute1.PropertyName))
        name = customAttribute1.PropertyName;
      else if (memberInfo.DeclaringType.GetCustomAttribute<DataContractAttribute>(true) != null)
      {
        DataMemberAttribute customAttribute2 = memberInfo.GetCustomAttribute<DataMemberAttribute>(true);
        if (customAttribute2 != null && !string.IsNullOrEmpty(customAttribute2.Name))
          name = customAttribute2.Name;
      }
      if (name == null)
        name = memberInfo.Name;
      if (linqSerializerOptions != null)
        name = CosmosSerializationUtil.GetStringWithPropertyNamingPolicy(linqSerializerOptions, name);
      return name;
    }

    private static Type GetElementType(Type type, HashSet<Type> visitedSet)
    {
      if (visitedSet.Contains(type))
        return (Type) null;
      visitedSet.Add(type);
      if (type.IsArray)
        return type.GetElementType();
      Type left = (Type) null;
      if (type.IsInterface() && type.IsGenericType() && type.GetGenericTypeDefinition() == typeof (IEnumerable<>))
        left = TypeSystem.GetMoreSpecificType(left, type.GetGenericArguments()[0]);
      foreach (Type type1 in type.GetInterfaces())
        left = TypeSystem.GetMoreSpecificType(left, TypeSystem.GetElementType(type1, visitedSet));
      if (type.GetBaseType() != (Type) null && type.GetBaseType() != typeof (object))
        left = TypeSystem.GetMoreSpecificType(left, TypeSystem.GetElementType(type.GetBaseType(), visitedSet));
      return left;
    }

    private static Type GetMoreSpecificType(Type left, Type right)
    {
      if (left != (Type) null && right != (Type) null)
        return right.IsAssignableFrom(left) || !left.IsAssignableFrom(right) ? left : right;
      Type type = left;
      return (object) type != null ? type : right;
    }

    public static bool IsAnonymousType(this Type type) => ((IEnumerable<object>) type.GetCustomAttributes(typeof (CompilerGeneratedAttribute), false)).Any<object>() & type.FullName.Contains("AnonymousType");

    public static bool IsEnumerable(this Type type) => type == typeof (Enumerable) || type.IsGenericType() && type.GetGenericTypeDefinition() == typeof (IEnumerable<>) || ((IEnumerable<Type>) type.GetInterfaces()).Where<Type>((Func<Type, bool>) (interfaceType => interfaceType.IsGenericType() && interfaceType.GetGenericTypeDefinition() == typeof (IEnumerable<>))).FirstOrDefault<Type>() != (Type) null;

    public static bool IsExtensionMethod(this MethodInfo methodInfo) => methodInfo.GetCustomAttribute(typeof (ExtensionAttribute)) != null;

    public static bool IsNullable(this Type type) => type.IsGenericType() && type.GetGenericTypeDefinition() == typeof (Nullable<>);

    public static Type NullableUnderlyingType(this Type type) => type.IsNullable() ? type.GetGenericArguments()[0] : type;
  }
}
