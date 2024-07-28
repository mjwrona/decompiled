// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmEntityReferenceType
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm
{
  public class EdmEntityReferenceType : EdmType, IEdmEntityReferenceType, IEdmType, IEdmElement
  {
    private readonly IEdmEntityType entityType;

    public EdmEntityReferenceType(IEdmEntityType entityType)
    {
      EdmUtil.CheckArgumentNull<IEdmEntityType>(entityType, nameof (entityType));
      this.entityType = entityType;
    }

    public override EdmTypeKind TypeKind => EdmTypeKind.EntityReference;

    public IEdmEntityType EntityType => this.entityType;
  }
}
