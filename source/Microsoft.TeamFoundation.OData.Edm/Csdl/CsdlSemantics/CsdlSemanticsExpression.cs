// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.CsdlSemantics.CsdlSemanticsExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Csdl.Parsing.Ast;

namespace Microsoft.OData.Edm.Csdl.CsdlSemantics
{
  internal abstract class CsdlSemanticsExpression : CsdlSemanticsElement, IEdmExpression, IEdmElement
  {
    private readonly CsdlSemanticsSchema schema;

    protected CsdlSemanticsExpression(CsdlSemanticsSchema schema, CsdlExpressionBase element)
      : base((CsdlElement) element)
    {
      this.schema = schema;
    }

    public abstract EdmExpressionKind ExpressionKind { get; }

    public CsdlSemanticsSchema Schema => this.schema;

    public override CsdlSemanticsModel Model => this.schema.Model;
  }
}
