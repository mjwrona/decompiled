// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.BadEntitySet
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
  internal class BadEntitySet : 
    BadElement,
    IEdmEntitySet,
    IEdmEntitySetBase,
    IEdmNavigationSource,
    IEdmNamedElement,
    IEdmElement,
    IEdmEntityContainerElement,
    IEdmVocabularyAnnotatable
  {
    private readonly string name;
    private readonly IEdmEntityContainer container;

    public BadEntitySet(string name, IEdmEntityContainer container, IEnumerable<EdmError> errors)
      : base(errors)
    {
      this.name = name ?? string.Empty;
      this.container = container;
    }

    public string Name => this.name;

    public EdmContainerElementKind ContainerElementKind => EdmContainerElementKind.EntitySet;

    public IEdmEntityContainer Container => this.container;

    public IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings => Enumerable.Empty<IEdmNavigationPropertyBinding>();

    public IEdmPathExpression Path => (IEdmPathExpression) null;

    public IEdmType Type => (IEdmType) new EdmCollectionType((IEdmTypeReference) new EdmEntityTypeReference((IEdmEntityType) new BadEntityType(string.Empty, this.Errors), false));

    public bool IncludeInServiceDocument => true;

    public IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty property) => (IEdmNavigationSource) null;

    public IEdmNavigationSource FindNavigationTarget(
      IEdmNavigationProperty navigationProperty,
      IEdmPathExpression bindingPath)
    {
      return (IEdmNavigationSource) null;
    }

    public IEnumerable<IEdmNavigationPropertyBinding> FindNavigationPropertyBindings(
      IEdmNavigationProperty navigationProperty)
    {
      return (IEnumerable<IEdmNavigationPropertyBinding>) null;
    }
  }
}
