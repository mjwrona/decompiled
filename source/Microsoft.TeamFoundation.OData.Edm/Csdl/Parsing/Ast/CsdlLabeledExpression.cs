// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlLabeledExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlLabeledExpression : CsdlExpressionBase
  {
    private readonly string label;
    private readonly CsdlExpressionBase element;

    public CsdlLabeledExpression(string label, CsdlExpressionBase element, CsdlLocation location)
      : base(location)
    {
      this.label = label;
      this.element = element;
    }

    public override EdmExpressionKind ExpressionKind => EdmExpressionKind.Labeled;

    public string Label => this.label;

    public CsdlExpressionBase Element => this.element;
  }
}
