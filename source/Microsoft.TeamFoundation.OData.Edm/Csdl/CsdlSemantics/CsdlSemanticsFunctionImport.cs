// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsFunctionImport
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;
using Microsoft.OData.Edm.Vocabularies;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsFunctionImport : 
    CsdlSemanticsOperationImport,
    IEdmFunctionImport,
    IEdmOperationImport,
    IEdmEntityContainerElement,
    IEdmNamedElement,
    IEdmElement,
    IEdmVocabularyAnnotatable
  {
    private readonly CsdlFunctionImport functionImport;
    private readonly CsdlSemanticsSchema csdlSchema;

    public CsdlSemanticsFunctionImport(
      CsdlSemanticsEntityContainer container,
      CsdlFunctionImport functionImport,
      IEdmFunction backingfunction)
      : base(container, (CsdlOperationImport) functionImport, (IEdmOperation) backingfunction)
    {
      this.csdlSchema = container.Context;
      this.functionImport = functionImport;
    }

    public IEdmFunction Function => (IEdmFunction) this.Operation;

    public bool IncludeInServiceDocument => this.functionImport.IncludeInServiceDocument;

    public override EdmContainerElementKind ContainerElementKind => EdmContainerElementKind.FunctionImport;
  }
}
