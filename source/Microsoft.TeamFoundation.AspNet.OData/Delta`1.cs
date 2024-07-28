// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Delta`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Microsoft.AspNet.OData
{
  [NonValidatingParameterBinding]
  public class Delta<TStructuralType> : TypedDelta, IDelta where TStructuralType : class
  {
    private static ConcurrentDictionary<Type, Dictionary<string, PropertyAccessor<TStructuralType>>> _propertyCache = new ConcurrentDictionary<Type, Dictionary<string, PropertyAccessor<TStructuralType>>>();
    private Dictionary<string, PropertyAccessor<TStructuralType>> _allProperties;
    private HashSet<string> _updatableProperties;
    private HashSet<string> _changedProperties;
    private IDictionary<string, object> _deltaNestedResources;
    private TStructuralType _instance;
    private Type _structuredType;
    private PropertyInfo _dynamicDictionaryPropertyinfo;
    private HashSet<string> _changedDynamicProperties;
    private IDictionary<string, object> _dynamicDictionaryCache;

    public Delta()
      : this(typeof (TStructuralType))
    {
    }

    public Delta(Type structuralType)
      : this(structuralType, (IEnumerable<string>) null)
    {
    }

    public Delta(Type structuralType, IEnumerable<string> updatableProperties)
      : this(structuralType, updatableProperties, (PropertyInfo) null)
    {
    }

    public Delta(
      Type structuralType,
      IEnumerable<string> updatableProperties,
      PropertyInfo dynamicDictionaryPropertyInfo)
    {
      this._dynamicDictionaryPropertyinfo = dynamicDictionaryPropertyInfo;
      this.Reset(structuralType);
      this.InitializeProperties(updatableProperties);
    }

    public override Type StructuredType => this._structuredType;

    public override Type ExpectedClrType => typeof (TStructuralType);

    public override void Clear() => this.Reset(this._structuredType);

    public override bool TrySetPropertyValue(string name, object value) => value is IDelta ? this.TrySetNestedResourceInternal(name, value) : this.TrySetPropertyValueInternal(name, value);

    public override bool TryGetPropertyValue(string name, out object value)
    {
      if (name == null)
        throw Error.ArgumentNull(nameof (name));
      if (this._dynamicDictionaryPropertyinfo != (PropertyInfo) null)
      {
        if (this._dynamicDictionaryCache == null)
          this._dynamicDictionaryCache = Delta<TStructuralType>.GetDynamicPropertyDictionary(this._dynamicDictionaryPropertyinfo, this._instance, false);
        if (this._dynamicDictionaryCache != null && this._dynamicDictionaryCache.TryGetValue(name, out value))
          return true;
      }
      if (this._deltaNestedResources.ContainsKey(name))
      {
        object deltaNestedResource = this._deltaNestedResources[name];
        FieldInfo field = deltaNestedResource.GetType().GetField("_instance", BindingFlags.Instance | BindingFlags.NonPublic);
        value = field.GetValue(deltaNestedResource);
        return true;
      }
      PropertyAccessor<TStructuralType> propertyAccessor;
      if (this._allProperties.TryGetValue(name, out propertyAccessor))
      {
        value = propertyAccessor.GetValue(this._instance);
        return true;
      }
      value = (object) null;
      return false;
    }

    public override bool TryGetPropertyType(string name, out Type type)
    {
      if (name == null)
        throw Error.ArgumentNull(nameof (name));
      if (this._dynamicDictionaryPropertyinfo != (PropertyInfo) null)
      {
        if (this._dynamicDictionaryCache == null)
          this._dynamicDictionaryCache = Delta<TStructuralType>.GetDynamicPropertyDictionary(this._dynamicDictionaryPropertyinfo, this._instance, false);
        object obj;
        if (this._dynamicDictionaryCache != null && this._dynamicDictionaryCache.TryGetValue(name, out obj))
        {
          if (obj == null)
          {
            type = (Type) null;
            return false;
          }
          type = obj.GetType();
          return true;
        }
      }
      PropertyAccessor<TStructuralType> propertyAccessor;
      if (this._allProperties.TryGetValue(name, out propertyAccessor))
      {
        type = propertyAccessor.Property.PropertyType;
        return true;
      }
      type = (Type) null;
      return false;
    }

    public TStructuralType GetInstance() => this._instance;

    public override IEnumerable<string> GetChangedPropertyNames() => this._changedProperties.Concat<string>((IEnumerable<string>) this._deltaNestedResources.Keys);

    public override IEnumerable<string> GetUnchangedPropertyNames() => this._updatableProperties.Except<string>(this.GetChangedPropertyNames());

    public void CopyChangedValues(TStructuralType original)
    {
      if ((object) original == null)
        throw Error.ArgumentNull(nameof (original));
      if (!this._structuredType.IsAssignableFrom(original.GetType()))
        throw Error.Argument(nameof (original), SRResources.DeltaTypeMismatch, (object) this._structuredType, (object) original.GetType());
      RuntimeHelpers.EnsureSufficientExecutionStack();
      foreach (PropertyAccessor<TStructuralType> propertyAccessor in this._changedProperties.Select<string, PropertyAccessor<TStructuralType>>((Func<string, PropertyAccessor<TStructuralType>>) (s => this._allProperties[s])).ToArray<PropertyAccessor<TStructuralType>>())
        propertyAccessor.Copy(this._instance, original);
      this.CopyChangedDynamicValues(original);
      foreach (string key in (IEnumerable<string>) this._deltaNestedResources.Keys)
      {
        object deltaNestedResource1 = this._deltaNestedResources[key];
        object propertyRef = (object) null;
        if (!Delta<TStructuralType>.TryGetPropertyRef(original, key, out propertyRef))
          throw Error.Argument(key, SRResources.DeltaNestedResourceNameNotFound, (object) key, (object) original.GetType());
        // ISSUE: reference to a compiler-generated field
        if (Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (Delta<TStructuralType>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target1 = Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__1.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p1 = Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__1;
        // ISSUE: reference to a compiler-generated field
        if (Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (Delta<TStructuralType>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj1 = Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__0.Target((CallSite) Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__0, propertyRef, (object) null);
        if (target1((CallSite) p1, obj1))
        {
          object deltaNestedResource2 = this._deltaNestedResources[key];
          // ISSUE: reference to a compiler-generated field
          if (Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__2 == null)
          {
            // ISSUE: reference to a compiler-generated field
            Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "GetInstance", (IEnumerable<Type>) null, typeof (Delta<TStructuralType>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj2 = Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__2.Target((CallSite) Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__2, deltaNestedResource2);
          // ISSUE: reference to a compiler-generated field
          if (Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__3 == null)
          {
            // ISSUE: reference to a compiler-generated field
            Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__3 = CallSite<Action<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, nameof (CopyChangedValues), (IEnumerable<Type>) null, typeof (Delta<TStructuralType>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__3.Target((CallSite) Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__3, deltaNestedResource2, obj2);
          // ISSUE: reference to a compiler-generated field
          if (Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__4 == null)
          {
            // ISSUE: reference to a compiler-generated field
            Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__4 = CallSite<Action<CallSite, PropertyAccessor<TStructuralType>, TStructuralType, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "SetValue", (IEnumerable<Type>) null, typeof (Delta<TStructuralType>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__4.Target((CallSite) Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__4, this._allProperties[key], original, obj2);
        }
        else
        {
          // ISSUE: reference to a compiler-generated field
          if (Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__7 == null)
          {
            // ISSUE: reference to a compiler-generated field
            Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__7 = CallSite<Func<CallSite, object, bool>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof (bool), typeof (Delta<TStructuralType>)));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, object, bool> target2 = Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__7.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, object, bool>> p7 = Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__7;
          // ISSUE: reference to a compiler-generated field
          if (Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__6 == null)
          {
            // ISSUE: reference to a compiler-generated field
            Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__6 = CallSite<Func<CallSite, Type, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "IsDeltaOfT", (IEnumerable<Type>) null, typeof (Delta<TStructuralType>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          Func<CallSite, Type, object, object> target3 = Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__6.Target;
          // ISSUE: reference to a compiler-generated field
          CallSite<Func<CallSite, Type, object, object>> p6 = Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__6;
          Type type = typeof (TypedDelta);
          // ISSUE: reference to a compiler-generated field
          if (Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__5 == null)
          {
            // ISSUE: reference to a compiler-generated field
            Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "GetType", (IEnumerable<Type>) null, typeof (Delta<TStructuralType>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          object obj3 = Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__5.Target((CallSite) Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__5, deltaNestedResource1);
          object obj4 = target3((CallSite) p6, type, obj3);
          int num = target2((CallSite) p7, obj4) ? 1 : 0;
          // ISSUE: reference to a compiler-generated field
          if (Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__8 == null)
          {
            // ISSUE: reference to a compiler-generated field
            Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__8 = CallSite<Action<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, nameof (CopyChangedValues), (IEnumerable<Type>) null, typeof (Delta<TStructuralType>), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
            {
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
              CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
            }));
          }
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__8.Target((CallSite) Delta<TStructuralType>.\u003C\u003Eo__25.\u003C\u003Ep__8, deltaNestedResource1, propertyRef);
        }
      }
    }

    public void CopyUnchangedValues(TStructuralType original)
    {
      if ((object) original == null)
        throw Error.ArgumentNull(nameof (original));
      if (!this._structuredType.IsInstanceOfType((object) original))
        throw Error.Argument(nameof (original), SRResources.DeltaTypeMismatch, (object) this._structuredType, (object) original.GetType());
      foreach (PropertyAccessor<TStructuralType> propertyAccessor in this.GetUnchangedPropertyNames().Select<string, PropertyAccessor<TStructuralType>>((Func<string, PropertyAccessor<TStructuralType>>) (s => this._allProperties[s])))
        propertyAccessor.Copy(this._instance, original);
      this.CopyUnchangedDynamicValues(original);
    }

    public void Patch(TStructuralType original) => this.CopyChangedValues(original);

    public void Put(TStructuralType original)
    {
      this.CopyChangedValues(original);
      this.CopyUnchangedValues(original);
    }

    private static void CopyDynamicPropertyDictionary(
      IDictionary<string, object> source,
      IDictionary<string, object> dest,
      PropertyInfo dynamicPropertyInfo,
      TStructuralType targetEntity)
    {
      if (source.Count == 0)
      {
        dest?.Clear();
      }
      else
      {
        if (dest == null)
          dest = Delta<TStructuralType>.GetDynamicPropertyDictionary(dynamicPropertyInfo, targetEntity, true);
        else
          dest.Clear();
        foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) source)
          dest.Add(keyValuePair);
      }
    }

    private static IDictionary<string, object> GetDynamicPropertyDictionary(
      PropertyInfo propertyInfo,
      TStructuralType entity,
      bool create)
    {
      if (propertyInfo == (PropertyInfo) null)
        throw Error.ArgumentNull(nameof (propertyInfo));
      object propertyDictionary1 = (object) entity != null ? propertyInfo.GetValue((object) entity) : throw Error.ArgumentNull(nameof (entity));
      if (propertyDictionary1 != null)
        return (IDictionary<string, object>) propertyDictionary1;
      if (!create)
        return (IDictionary<string, object>) null;
      if (!propertyInfo.CanWrite)
        throw Error.InvalidOperation(SRResources.CannotSetDynamicPropertyDictionary, (object) propertyInfo.Name, (object) entity.GetType().FullName);
      IDictionary<string, object> propertyDictionary2 = (IDictionary<string, object>) new Dictionary<string, object>();
      propertyInfo.SetValue((object) entity, (object) propertyDictionary2);
      return propertyDictionary2;
    }

    private static bool TryGetPropertyRef(
      TStructuralType structural,
      string propertyName,
      out object propertyRef)
    {
      propertyRef = (object) null;
      PropertyInfo property = structural.GetType().GetProperty(propertyName);
      if (!(property != (PropertyInfo) null))
        return false;
      propertyRef = property.GetValue((object) structural, (object[]) null);
      return true;
    }

    private void Reset(Type structuralType)
    {
      if (structuralType == (Type) null)
        throw Error.ArgumentNull(nameof (structuralType));
      this._instance = typeof (TStructuralType).IsAssignableFrom(structuralType) ? Activator.CreateInstance(structuralType) as TStructuralType : throw Error.InvalidOperation(SRResources.DeltaEntityTypeNotAssignable, (object) structuralType, (object) typeof (TStructuralType));
      this._changedProperties = new HashSet<string>();
      this._deltaNestedResources = (IDictionary<string, object>) new Dictionary<string, object>();
      this._structuredType = structuralType;
      this._changedDynamicProperties = new HashSet<string>();
      this._dynamicDictionaryCache = (IDictionary<string, object>) null;
    }

    private void InitializeProperties(IEnumerable<string> updatableProperties)
    {
      this._allProperties = Delta<TStructuralType>._propertyCache.GetOrAdd(this._structuredType, (Func<Type, Dictionary<string, PropertyAccessor<TStructuralType>>>) (backingType => ((IEnumerable<PropertyInfo>) backingType.GetProperties(BindingFlags.Instance | BindingFlags.Public)).Where<PropertyInfo>((Func<PropertyInfo, bool>) (p => (p.GetSetMethod() != (MethodInfo) null || TypeHelper.IsCollection(p.PropertyType)) && p.GetGetMethod() != (MethodInfo) null)).Select<PropertyInfo, PropertyAccessor<TStructuralType>>((Func<PropertyInfo, PropertyAccessor<TStructuralType>>) (p => (PropertyAccessor<TStructuralType>) new FastPropertyAccessor<TStructuralType>(p))).ToDictionary<PropertyAccessor<TStructuralType>, string>((Func<PropertyAccessor<TStructuralType>, string>) (p => p.Property.Name))));
      if (updatableProperties != null)
      {
        this._updatableProperties = new HashSet<string>(updatableProperties);
        this._updatableProperties.IntersectWith((IEnumerable<string>) this._allProperties.Keys);
      }
      else
        this._updatableProperties = new HashSet<string>((IEnumerable<string>) this._allProperties.Keys);
      if (!(this._dynamicDictionaryPropertyinfo != (PropertyInfo) null))
        return;
      this._updatableProperties.Remove(this._dynamicDictionaryPropertyinfo.Name);
    }

    private void CopyChangedDynamicValues(TStructuralType targetEntity)
    {
      if (this._dynamicDictionaryPropertyinfo == (PropertyInfo) null)
        return;
      if (this._dynamicDictionaryCache == null)
        this._dynamicDictionaryCache = Delta<TStructuralType>.GetDynamicPropertyDictionary(this._dynamicDictionaryPropertyinfo, this._instance, false);
      IDictionary<string, object> dynamicDictionaryCache = this._dynamicDictionaryCache;
      if (dynamicDictionaryCache == null)
        return;
      IDictionary<string, object> propertyDictionary = Delta<TStructuralType>.GetDynamicPropertyDictionary(this._dynamicDictionaryPropertyinfo, targetEntity, false);
      IDictionary<string, object> source = propertyDictionary != null ? (IDictionary<string, object>) new Dictionary<string, object>(propertyDictionary) : (IDictionary<string, object>) new Dictionary<string, object>();
      foreach (string changedDynamicProperty in this._changedDynamicProperties)
      {
        object obj = dynamicDictionaryCache[changedDynamicProperty];
        if (obj == null)
          source.Remove(changedDynamicProperty);
        else
          source[changedDynamicProperty] = obj;
      }
      Delta<TStructuralType>.CopyDynamicPropertyDictionary(source, propertyDictionary, this._dynamicDictionaryPropertyinfo, targetEntity);
    }

    private void CopyUnchangedDynamicValues(TStructuralType targetEntity)
    {
      if (this._dynamicDictionaryPropertyinfo == (PropertyInfo) null)
        return;
      if (this._dynamicDictionaryCache == null)
        this._dynamicDictionaryCache = Delta<TStructuralType>.GetDynamicPropertyDictionary(this._dynamicDictionaryPropertyinfo, this._instance, false);
      IDictionary<string, object> propertyDictionary = Delta<TStructuralType>.GetDynamicPropertyDictionary(this._dynamicDictionaryPropertyinfo, targetEntity, false);
      if (this._dynamicDictionaryCache == null)
      {
        propertyDictionary?.Clear();
      }
      else
      {
        IDictionary<string, object> source = propertyDictionary != null ? (IDictionary<string, object>) new Dictionary<string, object>(propertyDictionary) : (IDictionary<string, object>) new Dictionary<string, object>();
        foreach (string key in source.Keys.Except<string>((IEnumerable<string>) this._changedDynamicProperties).ToList<string>())
          source.Remove(key);
        Delta<TStructuralType>.CopyDynamicPropertyDictionary(source, propertyDictionary, this._dynamicDictionaryPropertyinfo, targetEntity);
      }
    }

    private bool TrySetPropertyValueInternal(string name, object value)
    {
      if (name == null)
        throw Error.ArgumentNull(nameof (name));
      if (this._dynamicDictionaryPropertyinfo != (PropertyInfo) null && (name == this._dynamicDictionaryPropertyinfo.Name || !this._allProperties.ContainsKey(name)))
      {
        if (this._dynamicDictionaryCache == null)
          this._dynamicDictionaryCache = Delta<TStructuralType>.GetDynamicPropertyDictionary(this._dynamicDictionaryPropertyinfo, this._instance, true);
        this._dynamicDictionaryCache[name] = value;
        this._changedDynamicProperties.Add(name);
        return true;
      }
      if (!this._updatableProperties.Contains(name))
        return false;
      PropertyAccessor<TStructuralType> allProperty = this._allProperties[name];
      if (value == null && !EdmLibHelpers.IsNullable(allProperty.Property.PropertyType))
        return false;
      Type propertyType = allProperty.Property.PropertyType;
      if (value != null && !TypeHelper.IsCollection(propertyType) && !propertyType.IsAssignableFrom(value.GetType()))
        return false;
      allProperty.SetValue(this._instance, value);
      this._changedProperties.Add(name);
      return true;
    }

    private bool TrySetNestedResourceInternal(string name, object deltaNestedResource)
    {
      if (name == null)
        throw Error.ArgumentNull(nameof (name));
      if (!this._updatableProperties.Contains(name) || this._deltaNestedResources.ContainsKey(name))
        return false;
      this._deltaNestedResources[name] = deltaNestedResource;
      return true;
    }
  }
}
