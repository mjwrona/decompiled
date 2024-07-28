// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmEntitySet
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
  public class EdmEntitySet : 
    EdmEntitySetBase,
    IEdmEntitySet,
    IEdmEntitySetBase,
    IEdmNavigationSource,
    IEdmNamedElement,
    IEdmElement,
    IEdmEntityContainerElement,
    IEdmVocabularyAnnotatable
  {
    private readonly IEdmEntityContainer container;
    private readonly IEdmCollectionType type;
    private IEdmPathExpression path;
    private bool includeInServiceDocument;

    public EdmEntitySet(IEdmEntityContainer container, string name, IEdmEntityType elementType)
      : this(container, name, elementType, true)
    {
      EdmUtil.CheckArgumentNull<IEdmEntityContainer>(container, nameof (container));
      this.container = container;
      this.type = (IEdmCollectionType) new EdmCollectionType((IEdmTypeReference) new EdmEntityTypeReference(elementType, false));
      this.path = (IEdmPathExpression) new EdmPathExpression(this.container.FullName() + "." + this.Name);
    }

    public EdmEntitySet(
      IEdmEntityContainer container,
      string name,
      IEdmEntityType elementType,
      bool includeInServiceDocument)
      : base(name, elementType)
    {
      this.includeInServiceDocument = includeInServiceDocument;
    }

    public EdmContainerElementKind ContainerElementKind => EdmContainerElementKind.EntitySet;

    public IEdmEntityContainer Container => this.container;

    public override IEdmType Type => (IEdmType) this.type;

    public override IEdmPathExpression Path => this.path;

    public bool IncludeInServiceDocument => this.includeInServiceDocument;
  }
}
