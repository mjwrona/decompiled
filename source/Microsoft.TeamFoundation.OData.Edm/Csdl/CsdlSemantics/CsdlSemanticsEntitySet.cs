// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsEntitySet
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsEntitySet : 
    CsdlSemanticsNavigationSource,
    IEdmEntitySet,
    IEdmEntitySetBase,
    IEdmNavigationSource,
    IEdmNamedElement,
    IEdmElement,
    IEdmEntityContainerElement,
    IEdmVocabularyAnnotatable
  {
    public CsdlSemanticsEntitySet(CsdlSemanticsEntityContainer container, CsdlEntitySet entitySet)
      : base(container, (CsdlAbstractNavigationSource) entitySet)
    {
    }

    public override IEdmType Type => (IEdmType) new EdmCollectionType((IEdmTypeReference) new EdmEntityTypeReference(this.typeCache.GetValue((CsdlSemanticsNavigationSource) this, CsdlSemanticsNavigationSource.ComputeElementTypeFunc, (Func<CsdlSemanticsNavigationSource, IEdmEntityType>) null), false));

    public override EdmContainerElementKind ContainerElementKind => EdmContainerElementKind.EntitySet;

    public bool IncludeInServiceDocument => ((CsdlEntitySet) this.navigationSource).IncludeInServiceDocument;

    protected override IEdmEntityType ComputeElementType()
    {
      string elementType = ((CsdlEntitySet) this.navigationSource).ElementType;
      return this.container.Context.FindType(elementType) is IEdmEntityType type ? type : (IEdmEntityType) new UnresolvedEntityType(elementType, this.Location);
    }
  }
}
