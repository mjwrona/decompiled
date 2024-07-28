// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.SelectExpandWrapper
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  internal abstract class SelectExpandWrapper : 
    IEdmEntityObject,
    IEdmStructuredObject,
    IEdmObject,
    ISelectExpandWrapper
  {
    private static readonly IPropertyMapper DefaultPropertyMapper = (IPropertyMapper) new IdentityPropertyMapper();
    private static readonly Func<IEdmModel, IEdmStructuredType, IPropertyMapper> _mapperProvider = (Func<IEdmModel, IEdmStructuredType, IPropertyMapper>) ((m, t) => SelectExpandWrapper.DefaultPropertyMapper);
    private Dictionary<string, object> _containerDict;
    private TypedEdmStructuredObject _typedEdmStructuredObject;

    public PropertyContainer Container { get; set; }

    public string InstanceType { get; set; }

    public object UntypedInstance { get; set; }

    public bool UseInstanceForProperties { get; set; }

    public IEdmModel Model { get; set; }

    public IEdmTypeReference GetEdmType()
    {
      IEdmModel model = this.GetModel();
      if (this.InstanceType != null)
      {
        IEdmStructuredType type = model.FindType(this.InstanceType) as IEdmStructuredType;
        return type is IEdmEntityType edmType ? edmType.ToEdmTypeReference(true) : type.ToEdmTypeReference(true);
      }
      Type elementType = this.GetElementType();
      return model.GetEdmTypeReference(elementType);
    }

    public bool TryGetPropertyValue(string propertyName, out object value)
    {
      if (this.Container != null)
      {
        this._containerDict = this._containerDict ?? this.Container.ToDictionary(SelectExpandWrapper.DefaultPropertyMapper);
        if (this._containerDict.TryGetValue(propertyName, out value))
          return true;
      }
      if (this.UseInstanceForProperties && this.UntypedInstance != null)
      {
        this._typedEdmStructuredObject = !(this.GetEdmType() is IEdmComplexTypeReference) ? this._typedEdmStructuredObject ?? (TypedEdmStructuredObject) new TypedEdmEntityObject(this.UntypedInstance, this.GetEdmType() as IEdmEntityTypeReference, this.GetModel()) : this._typedEdmStructuredObject ?? (TypedEdmStructuredObject) new TypedEdmComplexObject(this.UntypedInstance, this.GetEdmType() as IEdmComplexTypeReference, this.GetModel());
        return this._typedEdmStructuredObject.TryGetPropertyValue(propertyName, out value);
      }
      value = (object) null;
      return false;
    }

    public IDictionary<string, object> ToDictionary() => this.ToDictionary(SelectExpandWrapper._mapperProvider);

    public IDictionary<string, object> ToDictionary(
      Func<IEdmModel, IEdmStructuredType, IPropertyMapper> mapperProvider)
    {
      if (mapperProvider == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (mapperProvider));
      Dictionary<string, object> dictionary = new Dictionary<string, object>();
      IEdmStructuredType type = this.GetEdmType().AsStructured().StructuredDefinition();
      IPropertyMapper propertyMapper = mapperProvider(this.GetModel(), type);
      if (propertyMapper == null)
        throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.InvalidPropertyMapper, (object) typeof (IPropertyMapper).FullName, (object) type.FullTypeName());
      if (this.Container != null)
        dictionary = this.Container.ToDictionary(propertyMapper, false);
      if (this.UseInstanceForProperties && this.UntypedInstance != null)
      {
        foreach (IEdmStructuralProperty structuralProperty in type.StructuralProperties())
        {
          object obj;
          if (this.TryGetPropertyValue(structuralProperty.Name, out obj))
          {
            string key = propertyMapper.MapProperty(structuralProperty.Name);
            if (string.IsNullOrWhiteSpace(key))
              throw Microsoft.AspNet.OData.Common.Error.InvalidOperation(SRResources.InvalidPropertyMapping, (object) structuralProperty.Name);
            dictionary[key] = obj;
          }
        }
      }
      return (IDictionary<string, object>) dictionary;
    }

    protected abstract Type GetElementType();

    private IEdmModel GetModel() => this.Model;

    public void SetModel(IEdmModel model) => this.Model = model;
  }
}
