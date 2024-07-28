// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsTypeExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal abstract class CsdlSemanticsTypeExpression : 
    CsdlSemanticsElement,
    IEdmTypeReference,
    IEdmElement
  {
    private readonly CsdlExpressionTypeReference expressionUsage;
    private readonly CsdlSemanticsTypeDefinition type;

    protected CsdlSemanticsTypeExpression(
      CsdlExpressionTypeReference expressionUsage,
      CsdlSemanticsTypeDefinition type)
      : base((CsdlElement) expressionUsage)
    {
      this.expressionUsage = expressionUsage;
      this.type = type;
    }

    public IEdmType Definition => (IEdmType) this.type;

    public bool IsNullable => this.expressionUsage.IsNullable;

    public override CsdlSemanticsModel Model => this.type.Model;

    public override CsdlElement Element => (CsdlElement) this.expressionUsage;

    public override string ToString() => this.ToTraceString();
  }
}
