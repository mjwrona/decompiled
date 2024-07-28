// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsSingleton
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;
using System;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsSingleton : 
    CsdlSemanticsNavigationSource,
    IEdmSingleton,
    IEdmEntityContainerElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable,
    IEdmNavigationSource
  {
    public CsdlSemanticsSingleton(CsdlSemanticsEntityContainer container, CsdlSingleton singleton)
      : base(container, (CsdlAbstractNavigationSource) singleton)
    {
    }

    public override IEdmType Type => (IEdmType) this.typeCache.GetValue((CsdlSemanticsNavigationSource) this, CsdlSemanticsNavigationSource.ComputeElementTypeFunc, (Func<CsdlSemanticsNavigationSource, IEdmEntityType>) null);

    public override EdmContainerElementKind ContainerElementKind => EdmContainerElementKind.Singleton;

    protected override IEdmEntityType ComputeElementType()
    {
      string type1 = ((CsdlSingleton) this.navigationSource).Type;
      return this.container.Context.FindType(type1) is IEdmEntityType type2 ? type2 : (IEdmEntityType) new UnresolvedEntityType(type1, this.Location);
    }
  }
}
