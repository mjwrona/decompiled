// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.EdmDeltaType
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData.Edm;

namespace Microsoft.AspNet.OData
{
  internal class EdmDeltaType : IEdmType, IEdmElement
  {
    private IEdmEntityType _entityType;
    private EdmDeltaEntityKind _deltaKind;

    internal EdmDeltaType(IEdmEntityType entityType, EdmDeltaEntityKind deltaKind)
    {
      this._entityType = entityType != null ? entityType : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (entityType));
      this._deltaKind = deltaKind;
    }

    public EdmTypeKind TypeKind => EdmTypeKind.Entity;

    public IEdmEntityType EntityType => this._entityType;

    public EdmDeltaEntityKind DeltaKind => this._deltaKind;
  }
}
