// Decompiled with JetBrains decompiler
// Type: Tomlyn.Model.Accessors.ReflectionObjectInfo
// Assembly: Tomlyn.Signed, Version=0.16.0.0, Culture=neutral, PublicKeyToken=925c35bbd70fa682
// MVID: 8155B7FF-9444-4ADB-B8A6-687FA556DA39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Tomlyn.Signed.dll

using System;
using System.Collections.Generic;


#nullable enable
namespace Tomlyn.Model.Accessors
{
  internal readonly struct ReflectionObjectInfo
  {
    public readonly ReflectionObjectKind Kind;
    public readonly Type? GenericArgument1;
    public readonly Type? GenericArgument2;

    public ReflectionObjectInfo(ReflectionObjectKind kind)
    {
      this.Kind = kind;
      this.GenericArgument1 = (Type) null;
      this.GenericArgument2 = (Type) null;
    }

    public ReflectionObjectInfo(
      ReflectionObjectKind kind,
      Type genericArgument1,
      Type? genericArgument2 = null)
    {
      this.Kind = kind;
      this.GenericArgument1 = genericArgument1;
      this.GenericArgument2 = genericArgument2;
    }

    public static ReflectionObjectInfo Get(Type type)
    {
      if (type == typeof (string) || type.IsPrimitive || type == typeof (TomlDateTime) || type == typeof (DateTime) || type == typeof (DateTimeOffset))
        return new ReflectionObjectInfo(ReflectionObjectKind.Primitive);
      if (type.IsValueType)
      {
        Type underlyingType = Nullable.GetUnderlyingType(type);
        return (object) underlyingType == null ? new ReflectionObjectInfo(ReflectionObjectKind.Struct) : new ReflectionObjectInfo(underlyingType.IsPrimitive ? ReflectionObjectKind.NullablePrimitive : ReflectionObjectKind.NullableStruct, underlyingType);
      }
      Type[] interfaces = type.GetInterfaces();
      Type type1 = (Type) null;
      foreach (Type type2 in interfaces)
      {
        if (type2.IsGenericType)
        {
          Type genericTypeDefinition = type2.GetGenericTypeDefinition();
          if (genericTypeDefinition == typeof (IDictionary<,>))
          {
            Type[] genericArguments = type2.GetGenericArguments();
            return new ReflectionObjectInfo(ReflectionObjectKind.Dictionary, genericArguments[0], genericArguments[1]);
          }
          if ((object) type1 == null && genericTypeDefinition == typeof (ICollection<>))
            type1 = type2;
        }
      }
      return (object) type1 != null ? new ReflectionObjectInfo(ReflectionObjectKind.Collection, type1.GetGenericArguments()[0]) : new ReflectionObjectInfo(ReflectionObjectKind.Object);
    }
  }
}
