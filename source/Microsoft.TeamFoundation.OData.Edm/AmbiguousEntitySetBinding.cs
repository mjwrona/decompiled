// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.AmbiguousEntitySetBinding
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Vocabularies;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.OData.Edm
{
  internal class AmbiguousEntitySetBinding : 
    AmbiguousBinding<IEdmEntitySet>,
    IEdmEntitySet,
    IEdmEntitySetBase,
    IEdmNavigationSource,
    IEdmNamedElement,
    IEdmElement,
    IEdmEntityContainerElement,
    IEdmVocabularyAnnotatable
  {
    public AmbiguousEntitySetBinding(IEdmEntitySet first, IEdmEntitySet second)
      : base(first, second)
    {
    }

    public EdmContainerElementKind ContainerElementKind => EdmContainerElementKind.EntitySet;

    public IEdmEntityContainer Container => this.Bindings.FirstOrDefault<IEdmEntitySet>()?.Container;

    public IEdmPathExpression Path => (IEdmPathExpression) null;

    public IEdmType Type => (IEdmType) new EdmCollectionType((IEdmTypeReference) new EdmEntityTypeReference((IEdmEntityType) new BadEntityType(string.Empty, this.Errors), false));

    public bool IncludeInServiceDocument => true;

    public IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings => Enumerable.Empty<IEdmNavigationPropertyBinding>();

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
