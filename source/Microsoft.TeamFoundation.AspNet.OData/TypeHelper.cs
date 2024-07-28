// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.TypeHelper
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Microsoft.AspNet.OData
{
  internal static class TypeHelper
  {
    private static readonly Dictionary<Type, HashSet<Type>> _typePromoMap = new Dictionary<Type, HashSet<Type>>()
    {
      {
        typeof (byte),
        new HashSet<Type>()
        {
          typeof (int),
          typeof (int),
          typeof (long),
          typeof (Decimal),
          typeof (double),
          typeof (float)
        }
      },
      {
        typeof (short),
        new HashSet<Type>()
        {
          typeof (int),
          typeof (long),
          typeof (Decimal),
          typeof (double),
          typeof (float)
        }
      },
      {
        typeof (int),
        new HashSet<Type>()
        {
          typeof (long),
          typeof (Decimal),
          typeof (double),
          typeof (float)
        }
      },
      {
        typeof (long),
        new HashSet<Type>()
        {
          typeof (Decimal),
          typeof (double),
          typeof (float)
        }
      }
    };

    public static MemberInfo AsMemberInfo(Type clrType) => (MemberInfo) clrType;

    public static Type AsType(MemberInfo memberInfo) => memberInfo as Type;

    public static Assembly GetAssembly(Type clrType) => clrType.Assembly;

    public static Type GetBaseType(Type clrType) => clrType.BaseType;

    public static string GetQualifiedName(MemberInfo memberInfo)
    {
      Type type = memberInfo as Type;
      return !(type != (Type) null) ? memberInfo.Name : type.Namespace + "." + type.Name;
    }

    public static Type GetReflectedType(MemberInfo memberInfo) => memberInfo.ReflectedType;

    public static bool IsAbstract(Type clrType) => clrType.IsAbstract;

    public static bool IsClass(Type clrType) => clrType.IsClass;

    public static bool IsGenericType(this Type clrType) => clrType.IsGenericType;

    public static bool IsGenericTypeDefinition(this Type clrType) => clrType.IsGenericTypeDefinition;

    public static bool IsInterface(Type clrType) => clrType.IsInterface;

    public static bool IsNullable(Type clrType)
    {
      if (!TypeHelper.IsValueType(clrType))
        return true;
      return clrType.IsGenericType() && clrType.GetGenericTypeDefinition() == typeof (Nullable<>);
    }

    public static bool IsPublic(Type clrType) => clrType.IsPublic;

    public static bool IsPrimitive(Type clrType) => clrType.IsPrimitive;

    public static bool IsTypeAssignableFrom(Type clrType, Type fromType) => clrType.IsAssignableFrom(fromType);

    public static bool IsValueType(Type clrType) => clrType.IsValueType;

    public static bool IsVisible(Type clrType) => clrType.IsVisible;

    public static Type ToNullable(Type clrType)
    {
      if (TypeHelper.IsNullable(clrType))
        return clrType;
      return typeof (Nullable<>).MakeGenericType(clrType);
    }

    public static Type GetInnerElementType(Type clrType)
    {
      Type elementType;
      TypeHelper.IsCollection(clrType, out elementType);
      return elementType;
    }

    public static bool IsCollection(Type clrType) => TypeHelper.IsCollection(clrType, out Type _);

    public static bool IsCollection(Type clrType, out Type elementType)
    {
      elementType = !(clrType == (Type) null) ? clrType : throw Error.ArgumentNull(nameof (clrType));
      if (clrType == typeof (string))
        return false;
      Type type = ((IEnumerable<Type>) clrType.GetInterfaces()).Union<Type>((IEnumerable<Type>) new Type[1]
      {
        clrType
      }).FirstOrDefault<Type>((Func<Type, bool>) (t => t.IsGenericType() && t.GetGenericTypeDefinition() == typeof (IEnumerable<>)));
      if (!(type != (Type) null))
        return false;
      elementType = ((IEnumerable<Type>) type.GetGenericArguments()).Single<Type>();
      return true;
    }

    public static Type GetUnderlyingTypeOrSelf(Type type)
    {
      Type underlyingType = Nullable.GetUnderlyingType(type);
      return (object) underlyingType != null ? underlyingType : type;
    }

    public static bool IsEnum(Type clrType) => TypeHelper.GetUnderlyingTypeOrSelf(clrType).IsEnum;

    public static bool IsDateTime(Type clrType) => Type.GetTypeCode(TypeHelper.GetUnderlyingTypeOrSelf(clrType)) == TypeCode.DateTime;

    public static bool IsTimeSpan(Type clrType) => TypeHelper.GetUnderlyingTypeOrSelf(clrType) == typeof (TimeSpan);

    internal static bool IsIQueryable(Type type)
    {
      if (type == typeof (IQueryable))
        return true;
      return type != (Type) null && type.IsGenericType() && type.GetGenericTypeDefinition() == typeof (IQueryable<>);
    }

    internal static bool IsQueryPrimitiveType(Type type)
    {
      type = TypeHelper.GetInnerMostElementType(type);
      return TypeHelper.IsEnum(type) || TypeHelper.IsPrimitive(type) || type == typeof (Uri) || EdmLibHelpers.GetEdmPrimitiveTypeOrNull(type) != null;
    }

    internal static Type GetInnerMostElementType(Type type)
    {
      while (true)
      {
        Type underlyingType = Nullable.GetUnderlyingType(type);
        if (underlyingType != (Type) null)
          type = underlyingType;
        else if (type.HasElementType)
          type = type.GetElementType();
        else
          break;
      }
      return type;
    }

    internal static Type GetImplementedIEnumerableType(Type type)
    {
      if (type.IsGenericType() && type.GetGenericTypeDefinition() == typeof (Task<>))
        type = ((IEnumerable<Type>) type.GetGenericArguments()).First<Type>();
      if (type.IsGenericType() && TypeHelper.IsInterface(type) && (type.GetGenericTypeDefinition() == typeof (IEnumerable<>) || type.GetGenericTypeDefinition() == typeof (IQueryable<>)))
        return TypeHelper.GetInnerGenericType(type);
      foreach (Type type1 in type.GetInterfaces())
      {
        if (type1.IsGenericType() && (type1.GetGenericTypeDefinition() == typeof (IEnumerable<>) || type1.GetGenericTypeDefinition() == typeof (IQueryable<>)))
          return TypeHelper.GetInnerGenericType(type1);
      }
      return (Type) null;
    }

    internal static IEnumerable<Type> GetLoadedTypes(IWebApiAssembliesResolver assembliesResolver)
    {
      List<Type> loadedTypes = new List<Type>();
      if (assembliesResolver == null)
        return (IEnumerable<Type>) loadedTypes;
      foreach (Assembly assembly in assembliesResolver.Assemblies)
      {
        if (!(assembly == (Assembly) null))
        {
          if (!assembly.IsDynamic)
          {
            Type[] types;
            try
            {
              types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
              types = ex.Types;
            }
            catch
            {
              continue;
            }
            if (types != null)
              loadedTypes.AddRange(((IEnumerable<Type>) types).Where<Type>((Func<Type, bool>) (t => t != (Type) null && TypeHelper.IsVisible(t))));
          }
        }
      }
      return (IEnumerable<Type>) loadedTypes;
    }

    internal static Type GetTaskInnerTypeOrSelf(Type type) => type.IsGenericType() && type.GetGenericTypeDefinition() == typeof (Task<>) ? ((IEnumerable<Type>) type.GetGenericArguments()).First<Type>() : type;

    private static Type GetInnerGenericType(Type interfaceType)
    {
      Type[] genericArguments = interfaceType.GetGenericArguments();
      return genericArguments.Length == 1 ? genericArguments[0] : (Type) null;
    }

    public static bool CanPromoteValueTypeTo(this Type from, Type to)
    {
      HashSet<Type> typeSet;
      return from.IsValueType && to.IsValueType && !from.IsEnum && !to.IsEnum && TypeHelper._typePromoMap.TryGetValue(from, out typeSet) && typeSet.Contains(to);
    }
  }
}
