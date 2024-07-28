// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.TypeSystem
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal static class TypeSystem
  {
    private static readonly Dictionary<MethodInfo, string> StaticExpressionMethodMap = new Dictionary<MethodInfo, string>((IEqualityComparer<MethodInfo>) EqualityComparer<MethodInfo>.Default);
    private static readonly Dictionary<string, string> StaticExpressionVBMethodMap;
    private static readonly Dictionary<PropertyInfo, MethodInfo> StaticPropertiesAsMethodsMap;
    private const string VisualBasicAssemblyFullName = "Microsoft.VisualBasic, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

    static TypeSystem()
    {
      TypeSystem.StaticExpressionMethodMap.Add(typeof (string).GetMethod("Contains", new Type[1]
      {
        typeof (string)
      }), "substringof");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (string).GetMethod("EndsWith", new Type[1]
      {
        typeof (string)
      }), "endswith");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (string).GetMethod("StartsWith", new Type[1]
      {
        typeof (string)
      }), "startswith");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (string).GetMethod("IndexOf", new Type[1]
      {
        typeof (string)
      }), "indexof");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (string).GetMethod("Replace", new Type[2]
      {
        typeof (string),
        typeof (string)
      }), "replace");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (string).GetMethod("Substring", new Type[1]
      {
        typeof (int)
      }), "substring");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (string).GetMethod("Substring", new Type[2]
      {
        typeof (int),
        typeof (int)
      }), "substring");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (string).GetMethod("ToLower", Type.EmptyTypes), "tolower");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (string).GetMethod("ToUpper", Type.EmptyTypes), "toupper");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (string).GetMethod("Trim", Type.EmptyTypes), "trim");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (string).GetMethod("Concat", new Type[2]
      {
        typeof (string),
        typeof (string)
      }, (ParameterModifier[]) null), "concat");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (string).GetProperty("Length", typeof (int)).GetGetMethod(), "length");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (DateTime).GetProperty("Day", typeof (int)).GetGetMethod(), "day");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (DateTime).GetProperty("Hour", typeof (int)).GetGetMethod(), "hour");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (DateTime).GetProperty("Month", typeof (int)).GetGetMethod(), "month");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (DateTime).GetProperty("Minute", typeof (int)).GetGetMethod(), "minute");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (DateTime).GetProperty("Second", typeof (int)).GetGetMethod(), "second");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (DateTime).GetProperty("Year", typeof (int)).GetGetMethod(), "year");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (Math).GetMethod("Round", new Type[1]
      {
        typeof (double)
      }), "round");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (Math).GetMethod("Round", new Type[1]
      {
        typeof (Decimal)
      }), "round");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (Math).GetMethod("Floor", new Type[1]
      {
        typeof (double)
      }), "floor");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (Math).GetMethod("Floor", new Type[1]
      {
        typeof (Decimal)
      }), "floor");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (Math).GetMethod("Ceiling", new Type[1]
      {
        typeof (double)
      }), "ceiling");
      TypeSystem.StaticExpressionMethodMap.Add(typeof (Math).GetMethod("Ceiling", new Type[1]
      {
        typeof (Decimal)
      }), "ceiling");
      TypeSystem.StaticExpressionVBMethodMap = new Dictionary<string, string>((IEqualityComparer<string>) EqualityComparer<string>.Default);
      TypeSystem.StaticExpressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.Trim", "trim");
      TypeSystem.StaticExpressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.Len", "length");
      TypeSystem.StaticExpressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.Mid", "substring");
      TypeSystem.StaticExpressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.UCase", "toupper");
      TypeSystem.StaticExpressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.LCase", "tolower");
      TypeSystem.StaticExpressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Year", "year");
      TypeSystem.StaticExpressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Month", "month");
      TypeSystem.StaticExpressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Day", "day");
      TypeSystem.StaticExpressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Hour", "hour");
      TypeSystem.StaticExpressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Minute", "minute");
      TypeSystem.StaticExpressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Second", "second");
      TypeSystem.StaticPropertiesAsMethodsMap = new Dictionary<PropertyInfo, MethodInfo>((IEqualityComparer<PropertyInfo>) EqualityComparer<PropertyInfo>.Default);
      TypeSystem.StaticPropertiesAsMethodsMap.Add(typeof (string).GetProperty("Length", typeof (int)), typeof (string).GetProperty("Length", typeof (int)).GetGetMethod());
      TypeSystem.StaticPropertiesAsMethodsMap.Add(typeof (DateTime).GetProperty("Day", typeof (int)), typeof (DateTime).GetProperty("Day", typeof (int)).GetGetMethod());
      TypeSystem.StaticPropertiesAsMethodsMap.Add(typeof (DateTime).GetProperty("Hour", typeof (int)), typeof (DateTime).GetProperty("Hour", typeof (int)).GetGetMethod());
      TypeSystem.StaticPropertiesAsMethodsMap.Add(typeof (DateTime).GetProperty("Minute", typeof (int)), typeof (DateTime).GetProperty("Minute", typeof (int)).GetGetMethod());
      TypeSystem.StaticPropertiesAsMethodsMap.Add(typeof (DateTime).GetProperty("Second", typeof (int)), typeof (DateTime).GetProperty("Second", typeof (int)).GetGetMethod());
      TypeSystem.StaticPropertiesAsMethodsMap.Add(typeof (DateTime).GetProperty("Month", typeof (int)), typeof (DateTime).GetProperty("Month", typeof (int)).GetGetMethod());
      TypeSystem.StaticPropertiesAsMethodsMap.Add(typeof (DateTime).GetProperty("Year", typeof (int)), typeof (DateTime).GetProperty("Year", typeof (int)).GetGetMethod());
    }

    internal static bool TryGetQueryOptionMethod(MethodInfo mi, out string methodName)
    {
      if (TypeSystem.StaticExpressionMethodMap.TryGetValue(mi, out methodName))
        return true;
      return mi.DeclaringType.Assembly.FullName == "Microsoft.VisualBasic, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a" && TypeSystem.StaticExpressionVBMethodMap.TryGetValue(mi.DeclaringType.FullName + "." + mi.Name, out methodName);
    }

    internal static bool TryGetPropertyAsMethod(PropertyInfo pi, out MethodInfo mi) => TypeSystem.StaticPropertiesAsMethodsMap.TryGetValue(pi, out mi);

    internal static Type GetElementType(Type seqType)
    {
      Type ienumerable = TypeSystem.FindIEnumerable(seqType);
      return ienumerable == (Type) null ? seqType : ienumerable.GetGenericArguments()[0];
    }

    internal static bool IsPrivate(PropertyInfo pi)
    {
      MethodInfo methodInfo1 = pi.GetGetMethod();
      if ((object) methodInfo1 == null)
        methodInfo1 = pi.GetSetMethod();
      MethodInfo methodInfo2 = methodInfo1;
      return !(methodInfo2 != (MethodInfo) null) || methodInfo2.IsPrivate;
    }

    internal static Type FindIEnumerable(Type seqType)
    {
      if (seqType == (Type) null || seqType == typeof (string))
        return (Type) null;
      if (seqType.IsArray)
        return typeof (IEnumerable<>).MakeGenericType(seqType.GetElementType());
      if (seqType.IsGenericType)
      {
        foreach (Type genericArgument in seqType.GetGenericArguments())
        {
          Type ienumerable = typeof (IEnumerable<>).MakeGenericType(genericArgument);
          if (ienumerable.IsAssignableFrom(seqType))
            return ienumerable;
        }
      }
      Type[] interfaces = seqType.GetInterfaces();
      if (interfaces != null && interfaces.Length != 0)
      {
        foreach (Type seqType1 in interfaces)
        {
          Type ienumerable = TypeSystem.FindIEnumerable(seqType1);
          if (ienumerable != (Type) null)
            return ienumerable;
        }
      }
      return seqType.BaseType != (Type) null && seqType.BaseType != typeof (object) ? TypeSystem.FindIEnumerable(seqType.BaseType) : (Type) null;
    }
  }
}
