// Decompiled with JetBrains decompiler
// Type: Nest.TypeExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Nest
{
  internal static class TypeExtensions
  {
    internal static readonly MethodInfo GetActivatorMethodInfo = typeof (TypeExtensions).GetMethod("GetActivator", BindingFlags.Static | BindingFlags.NonPublic);
    private static readonly ConcurrentDictionary<string, TypeExtensions.ObjectActivator<object>> CachedActivators = new ConcurrentDictionary<string, TypeExtensions.ObjectActivator<object>>();
    private static readonly ConcurrentDictionary<string, Type> CachedGenericClosedTypes = new ConcurrentDictionary<string, Type>();
    private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> CachedTypePropertyInfos = new ConcurrentDictionary<Type, List<PropertyInfo>>();
    private static readonly Assembly NestAssembly = typeof (TypeExtensions).Assembly;

    internal static object CreateGenericInstance(this Type t, Type closeOver, params object[] args) => t.CreateGenericInstance(new Type[1]
    {
      closeOver
    }, args);

    internal static object CreateGenericInstance(
      this Type t,
      Type[] closeOver,
      params object[] args)
    {
      string key = ((IEnumerable<Type>) closeOver).Aggregate<Type, StringBuilder, string>(new StringBuilder(t.FullName), (Func<StringBuilder, Type, StringBuilder>) ((sb, gt) =>
      {
        sb.Append("--");
        return sb.Append(gt.FullName);
      }), (Func<StringBuilder, string>) (sb => sb.ToString()));
      Type t1;
      if (!TypeExtensions.CachedGenericClosedTypes.TryGetValue(key, out t1))
      {
        t1 = t.MakeGenericType(closeOver);
        TypeExtensions.CachedGenericClosedTypes.TryAdd(key, t1);
      }
      return t1.CreateInstance(args);
    }

    internal static T CreateInstance<T>(this Type t, params object[] args) => (T) t.CreateInstance(args);

    internal static object CreateInstance(this Type t, params object[] args)
    {
      string key = t.FullName;
      int length = args.Length;
      if (args.Length != 0)
        key = length.ToString() + "--" + key;
      TypeExtensions.ObjectActivator<object> objectActivator1;
      if (TypeExtensions.CachedActivators.TryGetValue(key, out objectActivator1))
        return objectActivator1(args);
      MethodInfo methodInfo = TypeExtensions.GetActivatorMethodInfo.MakeGenericMethod(t);
      ConstructorInfo constructorInfo = ((IEnumerable<ConstructorInfo>) t.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)).Select(c => new
      {
        c = c,
        p = c.GetParameters()
      }).Where(_param1 => _param1.p.Length == args.Length).Select(_param1 => _param1.c).FirstOrDefault<ConstructorInfo>();
      TypeExtensions.ObjectActivator<object> objectActivator2 = !(constructorInfo == (ConstructorInfo) null) ? (TypeExtensions.ObjectActivator<object>) methodInfo.Invoke((object) null, new object[1]
      {
        (object) constructorInfo
      }) : throw new Exception(string.Format("Cannot create an instance of {0} because it has no constructor taking {1} arguments", (object) t.FullName, (object) args.Length));
      TypeExtensions.CachedActivators.TryAdd(key, objectActivator2);
      return objectActivator2(args);
    }

    internal static TypeExtensions.ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor)
    {
      ParameterInfo[] parameters = ctor.GetParameters();
      ParameterExpression array = Expression.Parameter(typeof (object[]), "args");
      Expression[] expressionArray = new Expression[parameters.Length];
      for (int index1 = 0; index1 < parameters.Length; ++index1)
      {
        ConstantExpression index2 = Expression.Constant((object) index1);
        Type parameterType = parameters[index1].ParameterType;
        UnaryExpression unaryExpression = Expression.Convert((Expression) Expression.ArrayIndex((Expression) array, (Expression) index2), parameterType);
        expressionArray[index1] = (Expression) unaryExpression;
      }
      return (TypeExtensions.ObjectActivator<T>) Expression.Lambda(typeof (TypeExtensions.ObjectActivator<T>), (Expression) Expression.New(ctor, expressionArray), array).Compile();
    }

    internal static List<PropertyInfo> GetAllProperties(this Type type)
    {
      List<PropertyInfo> allProperties;
      if (TypeExtensions.CachedTypePropertyInfos.TryGetValue(type, out allProperties))
        return allProperties;
      Dictionary<string, PropertyInfo> collectedProperties = new Dictionary<string, PropertyInfo>();
      TypeExtensions.GetAllPropertiesCore(type, collectedProperties);
      List<PropertyInfo> list = collectedProperties.Values.ToList<PropertyInfo>();
      TypeExtensions.CachedTypePropertyInfos.TryAdd(type, list);
      return list;
    }

    private static void GetAllPropertiesCore(
      Type type,
      Dictionary<string, PropertyInfo> collectedProperties)
    {
      foreach (PropertyInfo property in type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public))
      {
        PropertyInfo propertyInfo;
        if (collectedProperties.TryGetValue(property.Name, out propertyInfo))
        {
          if (TypeExtensions.IsHidingMember(property))
            collectedProperties[property.Name] = property;
          else if (!type.IsInterface && property.IsVirtual())
          {
            Type declaringType = property.GetDeclaringType();
            if (!propertyInfo.IsVirtual() || !propertyInfo.GetDeclaringType().IsAssignableFrom(declaringType))
              collectedProperties[property.Name] = property;
          }
        }
        else
          collectedProperties.Add(property.Name, property);
      }
      if (!(type.BaseType != (Type) null))
        return;
      TypeExtensions.GetAllPropertiesCore(type.BaseType, collectedProperties);
    }

    private static bool IsHidingMember(PropertyInfo propertyInfo) => !(propertyInfo.DeclaringType?.BaseType?.GetProperty(propertyInfo.Name) == (PropertyInfo) null) && propertyInfo.GetBaseDefinition()?.ReturnType != propertyInfo.PropertyType;

    private static Type GetDeclaringType(this PropertyInfo propertyInfo)
    {
      Type declaringType = propertyInfo.GetBaseDefinition()?.DeclaringType;
      return (object) declaringType != null ? declaringType : propertyInfo.DeclaringType;
    }

    private static MethodInfo GetBaseDefinition(this PropertyInfo propertyInfo)
    {
      MethodInfo getMethod = propertyInfo.GetMethod;
      if (getMethod != (MethodInfo) null)
        return getMethod.GetBaseDefinition();
      return propertyInfo.SetMethod?.GetBaseDefinition();
    }

    private static bool IsVirtual(this PropertyInfo propertyInfo)
    {
      MethodInfo getMethod = propertyInfo.GetMethod;
      if (getMethod != (MethodInfo) null && getMethod.IsVirtual)
        return true;
      MethodInfo setMethod = propertyInfo.SetMethod;
      return setMethod != (MethodInfo) null && setMethod.IsVirtual;
    }

    public static bool IsNestType(this Type type) => type.Assembly == TypeExtensions.NestAssembly;

    internal delegate T ObjectActivator<out T>(params object[] args);
  }
}
