// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.FastPropertyAccessor`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Formatter.Deserialization;
using Microsoft.OData.Edm;
using System;
using System.Reflection;

namespace Microsoft.AspNet.OData
{
  internal class FastPropertyAccessor<TStructuralType> : PropertyAccessor<TStructuralType> where TStructuralType : class
  {
    private bool _isCollection;
    private PropertyInfo _property;
    private Action<TStructuralType, object> _setter;
    private Func<object, object> _getter;

    public FastPropertyAccessor(PropertyInfo property)
      : base(property)
    {
      this._property = property;
      this._isCollection = TypeHelper.IsCollection(property.PropertyType);
      if (!this._isCollection)
        this._setter = PropertyHelper.MakeFastPropertySetter<TStructuralType>(property);
      this._getter = PropertyHelper.MakeFastPropertyGetter(property);
    }

    public override object GetValue(TStructuralType instance)
    {
      if ((object) instance == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (instance));
      return this._getter((object) instance);
    }

    public override void SetValue(TStructuralType instance, object value)
    {
      if ((object) instance == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (instance));
      if (this._isCollection)
        DeserializationHelpers.SetCollectionProperty((object) instance, this._property.Name, (IEdmCollectionTypeReference) null, value, true);
      else
        this._setter(instance, value);
    }
  }
}
