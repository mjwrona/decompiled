// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.EdmEnumObject
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;

namespace Microsoft.AspNet.OData
{
  [NonValidatingParameterBinding]
  public class EdmEnumObject : IEdmEnumObject, IEdmObject
  {
    private readonly IEdmType _edmType;

    public string Value { get; set; }

    public bool IsNullable { get; set; }

    public EdmEnumObject(IEdmEnumType edmType, string value)
      : this(edmType, value, false)
    {
    }

    public EdmEnumObject(IEdmEnumTypeReference edmType, string value)
      : this(edmType.EnumDefinition(), value, edmType.IsNullable)
    {
    }

    public EdmEnumObject(IEdmEnumType edmType, string value, bool isNullable)
    {
      this._edmType = edmType != null ? (IEdmType) edmType : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (edmType));
      this.Value = value;
      this.IsNullable = isNullable;
    }

    public IEdmTypeReference GetEdmType() => (IEdmTypeReference) new EdmEnumTypeReference(this._edmType as IEdmEnumType, this.IsNullable);
  }
}
