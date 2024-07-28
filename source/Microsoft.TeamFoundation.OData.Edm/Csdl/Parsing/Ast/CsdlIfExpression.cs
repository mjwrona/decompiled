// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlIfExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlIfExpression : CsdlExpressionBase
  {
    private readonly CsdlExpressionBase test;
    private readonly CsdlExpressionBase ifTrue;
    private readonly CsdlExpressionBase ifFalse;

    public CsdlIfExpression(
      CsdlExpressionBase test,
      CsdlExpressionBase ifTrue,
      CsdlExpressionBase ifFalse,
      CsdlLocation location)
      : base(location)
    {
      this.test = test;
      this.ifTrue = ifTrue;
      this.ifFalse = ifFalse;
    }

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.If;

    public CsdlExpressionBase Test => this.test;

    public CsdlExpressionBase IfTrue => this.ifTrue;

    public CsdlExpressionBase IfFalse => this.ifFalse;
  }
}
