// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.TypeExtensionMethods
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Common
{
  public static class TypeExtensionMethods
  {
    public static bool IsAssignableOrConvertibleFrom(this Type type, object value)
    {
      if (value == null)
        return false;
      if (type.GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo()))
        return true;
      if (value is IConvertible)
      {
        try
        {
          ConvertUtility.ChangeType(value, type, (IFormatProvider) CultureInfo.CurrentCulture);
          return true;
        }
        catch (FormatException ex)
        {
        }
        catch (InvalidCastException ex)
        {
        }
        catch (OverflowException ex)
        {
        }
      }
      return false;
    }

    public static bool IsOfType(this Type type, Type t) => t.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) || type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == t || type.GetTypeInfo().ImplementedInterfaces.Any<Type>((Func<Type, bool>) (i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == t));

    public static bool IsDictionary(this Type type) => typeof (IDictionary).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) || type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof (IDictionary<,>) || type.GetTypeInfo().ImplementedInterfaces.Any<Type>((Func<Type, bool>) (i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof (IDictionary<,>)));

    public static bool IsList(this Type type) => typeof (IList).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) || type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof (IList<>) || type.GetTypeInfo().ImplementedInterfaces.Any<Type>((Func<Type, bool>) (i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof (IList<>)));

    public static Type GetMemberType(this Type type, string name)
    {
      type.GetTypeInfo();
      PropertyInfo instancePropertyInfo = TypeExtensionMethods.GetPublicInstancePropertyInfo(type, name);
      if (instancePropertyInfo != (PropertyInfo) null)
        return instancePropertyInfo.PropertyType;
      FieldInfo instanceFieldInfo = TypeExtensionMethods.GetPublicInstanceFieldInfo(type, name);
      return instanceFieldInfo != (FieldInfo) null ? instanceFieldInfo.FieldType : (Type) null;
    }

    public static object GetMemberValue(this Type type, string name, object obj)
    {
      PropertyInfo instancePropertyInfo = TypeExtensionMethods.GetPublicInstancePropertyInfo(type, name);
      if (instancePropertyInfo != (PropertyInfo) null)
        return instancePropertyInfo.GetValue(obj);
      FieldInfo instanceFieldInfo = TypeExtensionMethods.GetPublicInstanceFieldInfo(type, name);
      return instanceFieldInfo != (FieldInfo) null ? instanceFieldInfo.GetValue(obj) : (object) null;
    }

    public static void SetMemberValue(this Type type, string name, object obj, object value)
    {
      PropertyInfo instancePropertyInfo = TypeExtensionMethods.GetPublicInstancePropertyInfo(type, name);
      if (instancePropertyInfo != (PropertyInfo) null)
      {
        if (!instancePropertyInfo.SetMethod.IsPublic)
          throw new ArgumentException("Property set method not public.");
        instancePropertyInfo.SetValue(obj, value);
      }
      else
      {
        FieldInfo instanceFieldInfo = TypeExtensionMethods.GetPublicInstanceFieldInfo(type, name);
        if (!(instanceFieldInfo != (FieldInfo) null))
          return;
        instanceFieldInfo.SetValue(obj, value);
      }
    }

    private static PropertyInfo GetPublicInstancePropertyInfo(Type type, string name)
    {
      Type type1 = type;
      PropertyInfo instancePropertyInfo;
      TypeInfo typeInfo;
      for (instancePropertyInfo = (PropertyInfo) null; instancePropertyInfo == (PropertyInfo) null && type1 != (Type) null; type1 = typeInfo.BaseType)
      {
        typeInfo = type1.GetTypeInfo();
        instancePropertyInfo = typeInfo.DeclaredProperties.FirstOrDefault<PropertyInfo>((Func<PropertyInfo, bool>) (p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && p.GetMethod.Attributes.HasFlag((Enum) MethodAttributes.Public) && !p.GetMethod.Attributes.HasFlag((Enum) MethodAttributes.Static)));
      }
      return instancePropertyInfo;
    }

    private static FieldInfo GetPublicInstanceFieldInfo(Type type, string name)
    {
      Type type1 = type;
      FieldInfo instanceFieldInfo;
      TypeInfo typeInfo;
      for (instanceFieldInfo = (FieldInfo) null; instanceFieldInfo == (FieldInfo) null && type1 != (Type) null; type1 = typeInfo.BaseType)
      {
        typeInfo = type1.GetTypeInfo();
        instanceFieldInfo = typeInfo.DeclaredFields.FirstOrDefault<FieldInfo>((Func<FieldInfo, bool>) (f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && f.IsPublic && !f.IsStatic));
      }
      return instanceFieldInfo;
    }
  }
}
