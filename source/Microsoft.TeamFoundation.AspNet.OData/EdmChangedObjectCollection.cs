// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.EdmChangedObjectCollection
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Microsoft.AspNet.OData
{
  [NonValidatingParameterBinding]
  public class EdmChangedObjectCollection : Collection<IEdmChangedObject>, IEdmObject
  {
    private IEdmEntityType _entityType;
    private EdmDeltaCollectionType _edmType;
    private IEdmCollectionTypeReference _edmTypeReference;

    public EdmChangedObjectCollection(IEdmEntityType entityType)
      : base((IList<IEdmChangedObject>) Enumerable.Empty<IEdmChangedObject>().ToList<IEdmChangedObject>())
    {
      this.Initialize(entityType);
    }

    public EdmChangedObjectCollection(
      IEdmEntityType entityType,
      IList<IEdmChangedObject> changedObjectList)
      : base(changedObjectList)
    {
      this.Initialize(entityType);
    }

    public IEdmTypeReference GetEdmType() => (IEdmTypeReference) this._edmTypeReference;

    private void Initialize(IEdmEntityType entityType)
    {
      this._entityType = entityType != null ? entityType : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (entityType));
      this._edmType = new EdmDeltaCollectionType((IEdmTypeReference) new EdmEntityTypeReference(this._entityType, true));
      this._edmTypeReference = (IEdmCollectionTypeReference) new EdmCollectionTypeReference((IEdmCollectionType) this._edmType);
    }
  }
}
