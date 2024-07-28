// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Serialization.TypedEdmStructuredObject
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.OData.Edm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.AspNet.OData.Formatter.Serialization
{
  internal abstract class TypedEdmStructuredObject : IEdmStructuredObject, IEdmObject
  {
    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, Func<object, object>>> _propertyGetterCache = new ConcurrentDictionary<Type, ConcurrentDictionary<string, Func<object, object>>>();
    private IEdmStructuredTypeReference _edmType;
    private Type _type;
    private ConcurrentDictionary<string, Func<object, object>> _typePropertyGetterCache;

    protected TypedEdmStructuredObject(
      object instance,
      IEdmStructuredTypeReference edmType,
      IEdmModel edmModel)
    {
      this.Instance = instance;
      this._edmType = edmType;
      this._type = instance == null ? (Type) null : instance.GetType();
      this.Model = edmModel;
    }

    public object Instance { get; private set; }

    public IEdmModel Model { get; private set; }

    public IEdmTypeReference GetEdmType() => (IEdmTypeReference) this._edmType;

    public bool TryGetPropertyValue(string propertyName, out object value)
    {
      if (this.Instance == null)
      {
        value = (object) null;
        return false;
      }
      Func<object, object> propertyGetter = TypedEdmStructuredObject.GetOrCreatePropertyGetter(this._type, propertyName, this._edmType, this.Model, ref this._typePropertyGetterCache);
      if (propertyGetter == null)
      {
        value = (object) null;
        return false;
      }
      value = propertyGetter(this.Instance);
      return true;
    }

    internal static Func<object, object> GetOrCreatePropertyGetter(
      Type type,
      string propertyName,
      IEdmStructuredTypeReference edmType,
      IEdmModel model,
      ref ConcurrentDictionary<string, Func<object, object>> propertyGetterCache)
    {
      TypedEdmStructuredObject.EnsurePropertyGettingCachePopulated(type, edmType, model, ref propertyGetterCache);
      return propertyGetterCache.GetOrAdd(propertyName, (Func<string, Func<object, object>>) (name =>
      {
        IEdmProperty property = edmType.FindProperty(name);
        if (property != null && model != null)
          name = EdmLibHelpers.GetClrPropertyName(property, model) ?? name;
        return TypedEdmStructuredObject.CreatePropertyGetter(type, name);
      }));
    }

    private static void EnsurePropertyGettingCachePopulated(
      Type type,
      IEdmStructuredTypeReference edmType,
      IEdmModel model,
      ref ConcurrentDictionary<string, Func<object, object>> propertyGetterCache)
    {
      if (propertyGetterCache != null)
        return;
      propertyGetterCache = TypedEdmStructuredObject._propertyGetterCache.GetOrAdd(type, (Func<Type, ConcurrentDictionary<string, Func<object, object>>>) (t =>
      {
        List<IEdmProperty> list = edmType.StructuredDefinition().Properties().ToList<IEdmProperty>();
        ConcurrentDictionary<string, Func<object, object>> concurrentDictionary = new ConcurrentDictionary<string, Func<object, object>>(4 * Environment.ProcessorCount, list.Count);
        foreach (IEdmProperty edmProperty in list)
        {
          string propertyName = EdmLibHelpers.GetClrPropertyName(edmProperty, model) ?? edmProperty.Name;
          concurrentDictionary.TryAdd(edmProperty.Name, TypedEdmStructuredObject.CreatePropertyGetter(type, propertyName));
        }
        return concurrentDictionary;
      }));
    }

    internal static Func<object, object> GetOrCreatePropertyGetter(
      Type type,
      string propertyName,
      IEdmStructuredTypeReference edmType,
      IEdmModel model)
    {
      ConcurrentDictionary<string, Func<object, object>> propertyGetterCache = (ConcurrentDictionary<string, Func<object, object>>) null;
      return TypedEdmStructuredObject.GetOrCreatePropertyGetter(type, propertyName, edmType, model, ref propertyGetterCache);
    }

    private static Func<object, object> CreatePropertyGetter(Type type, string propertyName)
    {
      string[] strArray = propertyName.Split('\\');
      Func<object, object> propertyGetter = (Func<object, object>) null;
      foreach (string name in strArray)
      {
        PropertyInfo property = type.GetProperty(name);
        if (property == (PropertyInfo) null)
          return (Func<object, object>) null;
        PropertyHelper helper = new PropertyHelper(property);
        type = property.PropertyType;
        if (propertyGetter == null)
        {
          propertyGetter = new Func<object, object>(helper.GetValue);
        }
        else
        {
          Func<object, object> f = propertyGetter;
          propertyGetter = (Func<object, object>) (o => helper.GetValue(f(o)));
        }
      }
      return propertyGetter;
    }

    public void SetModel(IEdmModel model)
    {
    }
  }
}
