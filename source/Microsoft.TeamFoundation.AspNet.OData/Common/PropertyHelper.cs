// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Common.PropertyHelper
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.OData.Common
{
  internal class PropertyHelper
  {
    private static ConcurrentDictionary<Type, PropertyHelper[]> _reflectionCache = new ConcurrentDictionary<Type, PropertyHelper[]>();
    private Func<object, object> _valueGetter;
    private static readonly MethodInfo _callPropertyGetterOpenGenericMethod = typeof (PropertyHelper).GetMethod("CallPropertyGetter", BindingFlags.Static | BindingFlags.NonPublic);
    private static readonly MethodInfo _callPropertyGetterByReferenceOpenGenericMethod = typeof (PropertyHelper).GetMethod("CallPropertyGetterByReference", BindingFlags.Static | BindingFlags.NonPublic);
    private static readonly MethodInfo _callPropertySetterOpenGenericMethod = typeof (PropertyHelper).GetMethod("CallPropertySetter", BindingFlags.Static | BindingFlags.NonPublic);

    public PropertyHelper(PropertyInfo property)
    {
      this.Name = property.Name;
      this._valueGetter = PropertyHelper.MakeFastPropertyGetter(property);
    }

    public static Action<TDeclaringType, object> MakeFastPropertySetter<TDeclaringType>(
      PropertyInfo propertyInfo)
      where TDeclaringType : class
    {
      MethodInfo setMethod = propertyInfo.GetSetMethod();
      Type reflectedType = TypeHelper.GetReflectedType((MemberInfo) propertyInfo);
      Type parameterType = setMethod.GetParameters()[0].ParameterType;
      Delegate target = setMethod.CreateDelegate(typeof (Action<,>).MakeGenericType(reflectedType, parameterType));
      return (Action<TDeclaringType, object>) PropertyHelper._callPropertySetterOpenGenericMethod.MakeGenericMethod(reflectedType, parameterType).CreateDelegate(typeof (Action<TDeclaringType, object>), (object) target);
    }

    public virtual string Name { get; protected set; }

    public object GetValue(object instance) => instance != null ? this._valueGetter(instance) : (object) null;

    public static PropertyHelper[] GetProperties(object instance) => PropertyHelper.GetProperties(instance, new Func<PropertyInfo, PropertyHelper>(PropertyHelper.CreateInstance), PropertyHelper._reflectionCache);

    public static Func<object, object> MakeFastPropertyGetter(PropertyInfo propertyInfo)
    {
      MethodInfo getMethod = propertyInfo.GetGetMethod();
      Type reflectedType = TypeHelper.GetReflectedType((MemberInfo) getMethod);
      Type returnType = getMethod.ReturnType;
      Delegate @delegate;
      if (TypeHelper.IsValueType(reflectedType))
      {
        Delegate target = getMethod.CreateDelegate(typeof (PropertyHelper.ByRefFunc<,>).MakeGenericType(reflectedType, returnType));
        @delegate = PropertyHelper._callPropertyGetterByReferenceOpenGenericMethod.MakeGenericMethod(reflectedType, returnType).CreateDelegate(typeof (Func<object, object>), (object) target);
      }
      else
      {
        Delegate target = getMethod.CreateDelegate(typeof (Func<,>).MakeGenericType(reflectedType, returnType));
        @delegate = PropertyHelper._callPropertyGetterOpenGenericMethod.MakeGenericMethod(reflectedType, returnType).CreateDelegate(typeof (Func<object, object>), (object) target);
      }
      return (Func<object, object>) @delegate;
    }

    private static PropertyHelper CreateInstance(PropertyInfo property) => new PropertyHelper(property);

    private static object CallPropertyGetter<TDeclaringType, TValue>(
      Func<TDeclaringType, TValue> getter,
      object @this)
    {
      return (object) getter((TDeclaringType) @this);
    }

    private static object CallPropertyGetterByReference<TDeclaringType, TValue>(
      PropertyHelper.ByRefFunc<TDeclaringType, TValue> getter,
      object @this)
    {
      TDeclaringType declaringType = (TDeclaringType) @this;
      return (object) getter(ref declaringType);
    }

    private static void CallPropertySetter<TDeclaringType, TValue>(
      Action<TDeclaringType, TValue> setter,
      object @this,
      object value)
    {
      setter((TDeclaringType) @this, (TValue) value);
    }

    protected static PropertyHelper[] GetProperties(
      object instance,
      Func<PropertyInfo, PropertyHelper> createPropertyHelper,
      ConcurrentDictionary<Type, PropertyHelper[]> cache)
    {
      Type type = instance.GetType();
      PropertyHelper[] array;
      if (!cache.TryGetValue(type, out array))
      {
        IEnumerable<PropertyInfo> propertyInfos = ((IEnumerable<PropertyInfo>) type.GetProperties(BindingFlags.Instance | BindingFlags.Public)).Where<PropertyInfo>((Func<PropertyInfo, bool>) (prop => prop.GetIndexParameters().Length == 0 && prop.GetMethod != (MethodInfo) null));
        List<PropertyHelper> propertyHelperList = new List<PropertyHelper>();
        foreach (PropertyInfo propertyInfo in propertyInfos)
        {
          PropertyHelper propertyHelper = createPropertyHelper(propertyInfo);
          propertyHelperList.Add(propertyHelper);
        }
        array = propertyHelperList.ToArray();
        cache.TryAdd(type, array);
      }
      return array;
    }

    private delegate TValue ByRefFunc<TDeclaringType, TValue>(ref TDeclaringType arg);
  }
}
