// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.EdmDeltaDeletedEntityObject
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData;
using Microsoft.OData.Edm;

namespace Microsoft.AspNet.OData
{
  [NonValidatingParameterBinding]
  public class EdmDeltaDeletedEntityObject : 
    EdmEntityObject,
    IEdmDeltaDeletedEntityObject,
    IEdmChangedObject,
    IEdmStructuredObject,
    IEdmObject
  {
    private string _id;
    private DeltaDeletedEntryReason _reason;
    private EdmDeltaType _edmType;
    private IEdmNavigationSource _navigationSource;

    public EdmDeltaDeletedEntityObject(IEdmEntityType entityType)
      : this(entityType, false)
    {
    }

    public EdmDeltaDeletedEntityObject(IEdmEntityTypeReference entityTypeReference)
      : this(entityTypeReference.EntityDefinition(), entityTypeReference.IsNullable)
    {
    }

    public EdmDeltaDeletedEntityObject(IEdmEntityType entityType, bool isNullable)
      : base(entityType, isNullable)
    {
      this._edmType = new EdmDeltaType(entityType, EdmDeltaEntityKind.DeletedEntry);
    }

    public string Id
    {
      get => this._id;
      set => this._id = value;
    }

    public DeltaDeletedEntryReason Reason
    {
      get => this._reason;
      set => this._reason = value;
    }

    public EdmDeltaEntityKind DeltaKind => this._edmType.DeltaKind;

    public IEdmNavigationSource NavigationSource
    {
      get => this._navigationSource;
      set => this._navigationSource = value;
    }
  }
}
