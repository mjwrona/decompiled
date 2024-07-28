// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.EdmEntityObject
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;

namespace Microsoft.AspNet.OData
{
  [NonValidatingParameterBinding]
  public class EdmEntityObject : 
    EdmStructuredObject,
    IEdmEntityObject,
    IEdmStructuredObject,
    IEdmObject
  {
    public EdmEntityObject(IEdmEntityType edmType)
      : this(edmType, false)
    {
    }

    public EdmEntityObject(IEdmEntityTypeReference edmType)
      : this(edmType.EntityDefinition(), edmType.IsNullable)
    {
    }

    public EdmEntityObject(IEdmEntityType edmType, bool isNullable)
      : base((IEdmStructuredType) edmType, isNullable)
    {
    }
  }
}
