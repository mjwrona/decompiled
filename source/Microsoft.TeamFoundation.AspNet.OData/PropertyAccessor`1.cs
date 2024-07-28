// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.PropertyAccessor`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System.Reflection;

namespace Microsoft.AspNet.OData
{
  internal abstract class PropertyAccessor<TStructuralType> where TStructuralType : class
  {
    protected PropertyAccessor(PropertyInfo property)
    {
      this.Property = !(property == (PropertyInfo) null) ? property : throw Error.ArgumentNull(nameof (property));
      if (this.Property.GetGetMethod() == (MethodInfo) null || !TypeHelper.IsCollection(property.PropertyType) && this.Property.GetSetMethod() == (MethodInfo) null)
        throw Error.Argument(nameof (property), SRResources.PropertyMustHavePublicGetterAndSetter);
    }

    public PropertyInfo Property { get; private set; }

    public void Copy(TStructuralType from, TStructuralType to)
    {
      if ((object) from == null)
        throw Error.ArgumentNull(nameof (from));
      if ((object) to == null)
        throw Error.ArgumentNull(nameof (to));
      this.SetValue(to, this.GetValue(from));
    }

    public abstract object GetValue(TStructuralType instance);

    public abstract void SetValue(TStructuralType instance, object value);
  }
}
