// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.TypeUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;

namespace Microsoft.OData
{
  internal static class TypeUtils
  {
    internal static bool IsNullableType(Type type) => type.IsGenericType() && type.GetGenericTypeDefinition() == typeof (Nullable<>);

    internal static Type GetNonNullableType(Type type)
    {
      Type underlyingType = Nullable.GetUnderlyingType(type);
      return (object) underlyingType != null ? underlyingType : type;
    }

    internal static Type GetNullableType(Type type)
    {
      if (!TypeUtils.TypeAllowsNull(type))
        type = typeof (Nullable<>).MakeGenericType(type);
      return type;
    }

    internal static bool TypeAllowsNull(Type type) => !type.IsValueType() || TypeUtils.IsNullableType(type);

    internal static bool AreTypesEquivalent(Type typeA, Type typeB) => !(typeA == (Type) null) && !(typeB == (Type) null) && typeA == typeB;

    internal static void ParseQualifiedTypeName(
      string qualifiedTypeName,
      out string namespaceName,
      out string typeName,
      out bool isCollection)
    {
      isCollection = qualifiedTypeName.StartsWith("Collection(", StringComparison.Ordinal);
      if (isCollection)
        qualifiedTypeName = qualifiedTypeName.Substring("Collection".Length + 1).TrimEnd(')');
      int length = qualifiedTypeName.LastIndexOf(".", StringComparison.Ordinal);
      namespaceName = qualifiedTypeName.Substring(0, length);
      typeName = qualifiedTypeName.Substring(length == 0 ? 0 : length + 1);
    }
  }
}
