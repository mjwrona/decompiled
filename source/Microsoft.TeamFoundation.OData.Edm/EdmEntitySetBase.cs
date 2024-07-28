// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmEntitySetBase
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm
{
  public abstract class EdmEntitySetBase : 
    EdmNavigationSource,
    IEdmEntitySetBase,
    IEdmNavigationSource,
    IEdmNamedElement,
    IEdmElement
  {
    private readonly IEdmCollectionType type;

    protected EdmEntitySetBase(string name, IEdmEntityType elementType)
      : base(name)
    {
      EdmUtil.CheckArgumentNull<IEdmEntityType>(elementType, nameof (elementType));
      this.type = (IEdmCollectionType) new EdmCollectionType((IEdmTypeReference) new EdmEntityTypeReference(elementType, false));
    }

    public override IEdmType Type => (IEdmType) this.type;
  }
}
