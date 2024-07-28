// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlIsTypeExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlIsTypeExpression : CsdlExpressionBase
  {
    private readonly CsdlTypeReference type;
    private readonly CsdlExpressionBase operand;

    public CsdlIsTypeExpression(
      CsdlTypeReference type,
      CsdlExpressionBase operand,
      CsdlLocation location)
      : base(location)
    {
      this.type = type;
      this.operand = operand;
    }

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.IsType;

    public CsdlTypeReference Type => this.type;

    public CsdlExpressionBase Operand => this.operand;
  }
}
