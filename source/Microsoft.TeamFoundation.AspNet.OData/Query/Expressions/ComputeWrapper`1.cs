// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.ComputeWrapper`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNet.OData.Formatter.Serialization;
using Microsoft.OData.Edm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  internal class ComputeWrapper<T> : 
    GroupByWrapper,
    IEdmEntityObject,
    IEdmStructuredObject,
    IEdmObject,
    ISelectExpandWrapper
  {
    private bool _merged;
    private TypedEdmEntityObject _typedEdmEntityObject;

    public T Instance { get; set; }

    public IEdmModel Model { get; set; }

    protected override void EnsureValues()
    {
      base.EnsureValues();
      if (this._merged)
        return;
      if (this.Instance is DynamicTypeWrapper instance)
      {
        this._values.MergeWithReplace<string, object>(instance.Values);
      }
      else
      {
        IEdmEntityTypeReference edmType = this.GetEdmType() as IEdmEntityTypeReference;
        this._typedEdmEntityObject = this._typedEdmEntityObject ?? new TypedEdmEntityObject((object) this.Instance, edmType, this.GetModel());
        foreach (string str in edmType.DeclaredStructuralProperties().Where<IEdmStructuralProperty>((Func<IEdmStructuralProperty, bool>) (p => p.Type.IsPrimitive() || p.Type.IsEnum())).Select<IEdmStructuralProperty, string>((Func<IEdmStructuralProperty, string>) (p => p.Name)))
        {
          object obj;
          if (this._typedEdmEntityObject.TryGetPropertyValue(str, out obj))
            this._values[str] = obj;
        }
      }
      this._merged = true;
    }

    private IEdmModel GetModel() => this.Model;

    public void SetModel(IEdmModel model) => this.Model = model;

    public IEdmTypeReference GetEdmType() => this.GetModel().GetEdmTypeReference(typeof (T));

    public IDictionary<string, object> ToDictionary() => (IDictionary<string, object>) this.Values;

    public IDictionary<string, object> ToDictionary(
      Func<IEdmModel, IEdmStructuredType, IPropertyMapper> propertyMapperProvider)
    {
      throw new NotImplementedException();
    }
  }
}
