// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.EdmStructuredObject
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData
{
  [NonValidatingParameterBinding]
  public abstract class EdmStructuredObject : Delta, IEdmStructuredObject, IEdmObject
  {
    private Dictionary<string, object> _container = new Dictionary<string, object>();
    private HashSet<string> _setProperties = new HashSet<string>();
    private IEdmStructuredType _expectedEdmType;
    private IEdmStructuredType _actualEdmType;

    protected EdmStructuredObject(IEdmStructuredType edmType)
      : this(edmType, false)
    {
    }

    protected EdmStructuredObject(IEdmStructuredTypeReference edmType)
      : this(edmType.StructuredDefinition(), edmType.IsNullable)
    {
    }

    protected EdmStructuredObject(IEdmStructuredType edmType, bool isNullable)
    {
      this._expectedEdmType = edmType != null ? edmType : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmType));
      this._actualEdmType = edmType;
      this.IsNullable = isNullable;
    }

    public IEdmStructuredType ExpectedEdmType
    {
      get => this._expectedEdmType;
      set
      {
        if (value == null)
          throw Microsoft.AspNet.OData.Common.Error.PropertyNull();
        this._expectedEdmType = this._actualEdmType.IsOrInheritsFrom((IEdmType) value) ? value : throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.DeltaEntityTypeNotAssignable, (object) this._actualEdmType.ToTraceString(), (object) value.ToTraceString());
      }
    }

    public IEdmStructuredType ActualEdmType
    {
      get => this._actualEdmType;
      set
      {
        if (value == null)
          throw Microsoft.AspNet.OData.Common.Error.PropertyNull();
        this._actualEdmType = value.IsOrInheritsFrom((IEdmType) this._expectedEdmType) ? value : throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.DeltaEntityTypeNotAssignable, (object) value.ToTraceString(), (object) this._expectedEdmType.ToTraceString());
      }
    }

    public bool IsNullable { get; set; }

    public override void Clear()
    {
      this._container.Clear();
      this._setProperties.Clear();
    }

    public override bool TrySetPropertyValue(string name, object value)
    {
      if (this._actualEdmType.FindProperty(name) == null && !this._actualEdmType.IsOpen)
        return false;
      this._setProperties.Add(name);
      this._container[name] = value;
      return true;
    }

    public override bool TryGetPropertyValue(string name, out object value)
    {
      IEdmProperty property = this._actualEdmType.FindProperty(name);
      if (property != null || this._actualEdmType.IsOpen)
      {
        if (this._container.ContainsKey(name))
        {
          value = this._container[name];
          return true;
        }
        value = EdmStructuredObject.GetDefaultValue(property.Type);
        this._container[name] = value;
        return true;
      }
      value = (object) null;
      return false;
    }

    public override bool TryGetPropertyType(string name, out Type type)
    {
      IEdmProperty property = this._actualEdmType.FindProperty(name);
      if (property != null)
      {
        type = EdmStructuredObject.GetClrTypeForUntypedDelta(property.Type);
        return true;
      }
      if (this._actualEdmType.IsOpen && this._container.ContainsKey(name))
      {
        type = this._container[name].GetType();
        return true;
      }
      type = (Type) null;
      return false;
    }

    public Dictionary<string, object> TryGetDynamicProperties() => !this._actualEdmType.IsOpen ? new Dictionary<string, object>() : this._container.Where<KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, bool>) (p => this._actualEdmType.FindProperty(p.Key) == null)).ToDictionary<KeyValuePair<string, object>, string, object>((Func<KeyValuePair<string, object>, string>) (property => property.Key), (Func<KeyValuePair<string, object>, object>) (property => property.Value));

    public override IEnumerable<string> GetChangedPropertyNames() => (IEnumerable<string>) this._setProperties;

    public override IEnumerable<string> GetUnchangedPropertyNames() => this._actualEdmType.Properties().Select<IEdmProperty, string>((Func<IEdmProperty, string>) (p => p.Name)).Except<string>(this.GetChangedPropertyNames());

    public IEdmTypeReference GetEdmType() => this._actualEdmType.ToEdmTypeReference(this.IsNullable);

    internal static object GetDefaultValue(IEdmTypeReference propertyType)
    {
      bool flag = propertyType.IsCollection();
      if (!(!propertyType.IsNullable | flag))
        return (object) null;
      Type typeForUntypedDelta = EdmStructuredObject.GetClrTypeForUntypedDelta(propertyType);
      if (propertyType.IsPrimitive() || flag && propertyType.AsCollection().ElementType().IsPrimitive())
        return Activator.CreateInstance(typeForUntypedDelta);
      return Activator.CreateInstance(typeForUntypedDelta, (object) propertyType);
    }

    internal static Type GetClrTypeForUntypedDelta(IEdmTypeReference edmType)
    {
      switch (edmType.TypeKind())
      {
        case EdmTypeKind.Primitive:
          return EdmLibHelpers.GetClrType((IEdmTypeReference) edmType.AsPrimitive(), (IEdmModel) EdmCoreModel.Instance);
        case EdmTypeKind.Entity:
          return typeof (EdmEntityObject);
        case EdmTypeKind.Complex:
          return typeof (EdmComplexObject);
        case EdmTypeKind.Collection:
          IEdmTypeReference edmTypeReference = edmType.AsCollection().ElementType();
          if (edmTypeReference.IsPrimitive())
            return typeof (List<>).MakeGenericType(EdmStructuredObject.GetClrTypeForUntypedDelta(edmTypeReference));
          if (edmTypeReference.IsComplex())
            return typeof (EdmComplexObjectCollection);
          if (edmTypeReference.IsEntity())
            return typeof (EdmEntityObjectCollection);
          if (edmTypeReference.IsEnum())
            return typeof (EdmEnumObjectCollection);
          break;
        case EdmTypeKind.Enum:
          return typeof (EdmEnumObject);
      }
      throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.UnsupportedEdmType, (object) edmType.ToTraceString(), (object) edmType.TypeKind());
    }

    public void SetModel(IEdmModel model)
    {
    }
  }
}
