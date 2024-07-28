// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmSingleton
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm
{
  public class EdmSingleton : 
    EdmNavigationSource,
    IEdmSingleton,
    IEdmEntityContainerElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IEdmNavigationSource
  {
    private readonly IEdmEntityContainer container;
    private readonly IEdmEntityType entityType;
    private IEdmPathExpression path;

    public EdmSingleton(IEdmEntityContainer container, string name, IEdmEntityType entityType)
      : base(name)
    {
      EdmUtil.CheckArgumentNull<IEdmEntityContainer>(container, nameof (container));
      EdmUtil.CheckArgumentNull<IEdmEntityType>(entityType, nameof (entityType));
      this.container = container;
      this.entityType = entityType;
      this.path = (IEdmPathExpression) new EdmPathExpression(name);
    }

    public EdmContainerElementKind ContainerElementKind => EdmContainerElementKind.Singleton;

    public IEdmEntityContainer Container => this.container;

    public override IEdmType Type => (IEdmType) this.entityType;

    public override IEdmPathExpression Path => this.path;
  }
}
