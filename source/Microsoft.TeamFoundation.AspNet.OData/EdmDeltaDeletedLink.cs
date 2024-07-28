// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.EdmDeltaDeletedLink
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System;

namespace Microsoft.AspNet.OData
{
  [NonValidatingParameterBinding]
  public class EdmDeltaDeletedLink : 
    EdmEntityObject,
    IEdmDeltaDeletedLink,
    IEdmDeltaLinkBase,
    IEdmChangedObject,
    IEdmStructuredObject,
    IEdmObject
  {
    private Uri _source;
    private Uri _target;
    private string _relationship;
    private EdmDeltaType _edmType;

    public EdmDeltaDeletedLink(IEdmEntityType entityType)
      : this(entityType, false)
    {
    }

    public EdmDeltaDeletedLink(IEdmEntityTypeReference entityTypeReference)
      : this(entityTypeReference.EntityDefinition(), entityTypeReference.IsNullable)
    {
    }

    public EdmDeltaDeletedLink(IEdmEntityType entityType, bool isNullable)
      : base(entityType, isNullable)
    {
      this._edmType = new EdmDeltaType(entityType, EdmDeltaEntityKind.DeletedLinkEntry);
    }

    public Uri Source
    {
      get => this._source;
      set => this._source = value;
    }

    public Uri Target
    {
      get => this._target;
      set => this._target = value;
    }

    public string Relationship
    {
      get => this._relationship;
      set => this._relationship = value;
    }

    public EdmDeltaEntityKind DeltaKind => this._edmType.DeltaKind;
  }
}
