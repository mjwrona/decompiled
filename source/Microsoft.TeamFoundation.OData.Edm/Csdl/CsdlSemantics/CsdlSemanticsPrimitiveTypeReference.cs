// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsPrimitiveTypeReference
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal class CsdlSemanticsPrimitiveTypeReference : 
    CsdlSemanticsElement,
    IEdmPrimitiveTypeReference,
    IEdmTypeReference,
    IEdmElement
  {
    internal readonly CsdlPrimitiveTypeReference Reference;
    private readonly CsdlSemanticsSchema schema;
    private readonly IEdmPrimitiveType definition;

    public CsdlSemanticsPrimitiveTypeReference(
      CsdlSemanticsSchema schema,
      CsdlPrimitiveTypeReference reference)
      : base((CsdlElement) reference)
    {
      this.schema = schema;
      this.Reference = reference;
      this.definition = EdmCoreModel.Instance.GetPrimitiveType(this.Reference.Kind);
    }

    public bool IsNullable => this.Reference.IsNullable;

    public IEdmType Definition => (IEdmType) this.definition;

    public override CsdlSemanticsModel Model => this.schema.Model;

    public override CsdlElement Element => (CsdlElement) this.Reference;

    public override string ToString() => this.ToTraceString();
  }
}
