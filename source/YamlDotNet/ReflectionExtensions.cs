// Decompiled with JetBrains decompiler
// Type: YamlDotNet.ReflectionExtensions
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace YamlDotNet
{
  internal static class ReflectionExtensions
  {
    private static readonly FieldInfo remoteStackTraceField = typeof (Exception).GetField("_remoteStackTraceString", BindingFlags.Instance | BindingFlags.NonPublic);

    public static Type BaseType(this Type type) => type.BaseType;

    public static bool IsValueType(this Type type) => type.IsValueType;

    public static bool IsGenericType(this Type type) => type.IsGenericType;

    public static bool IsInterface(this Type type) => type.IsInterface;

    public static bool IsEnum(this Type type) => type.IsEnum;

    public static bool IsDbNull(this object value) => value is DBNull;

    public static bool HasDefaultConstructor(this Type type) => type.IsValueType || type.GetConstructor(BindingFlags.Instance | BindingFlags.Public, (Binder) null, Type.EmptyTypes, (ParameterModifier[]) null) != null;

    public static TypeCode GetTypeCode(this Type type) => Type.GetTypeCode(type);

    public static PropertyInfo GetPublicProperty(this Type type, string name) => type.GetProperty(name);

    public static IEnumerable<PropertyInfo> GetPublicProperties(this Type type)
    {
      BindingFlags instancePublic = BindingFlags.Instance | BindingFlags.Public;
      if (!type.IsInterface)
        return (IEnumerable<PropertyInfo>) type.GetProperties(instancePublic);
      return ((IEnumerable<Type>) new Type[1]{ type }).Concat<Type>((IEnumerable<Type>) type.GetInterfaces()).SelectMany<Type, PropertyInfo>((Func<Type, IEnumerable<PropertyInfo>>) (i => (IEnumerable<PropertyInfo>) i.GetProperties(instancePublic)));
    }

    public static IEnumerable<MethodInfo> GetPublicStaticMethods(this Type type) => (IEnumerable<MethodInfo>) type.GetMethods(BindingFlags.Static | BindingFlags.Public);

    public static MethodInfo GetPublicStaticMethod(
      this Type type,
      string name,
      params Type[] parameterTypes)
    {
      return type.GetMethod(name, BindingFlags.Static | BindingFlags.Public, (Binder) null, parameterTypes, (ParameterModifier[]) null);
    }

    public static MethodInfo GetPublicInstanceMethod(this Type type, string name) => type.GetMethod(name, BindingFlags.Instance | BindingFlags.Public);

    public static Exception Unwrap(this TargetInvocationException ex)
    {
      Exception innerException = ex.InnerException;
      if ((object) ReflectionExtensions.remoteStackTraceField == null)
        return innerException;
      ReflectionExtensions.remoteStackTraceField.SetValue((object) ex.InnerException, (object) (ex.InnerException.StackTrace + "\r\n"));
      return innerException;
    }

    public static bool IsInstanceOf(this Type type, object o) => type.IsInstanceOfType(o);
  }
}
