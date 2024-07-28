// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmLabeledExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmLabeledExpression : 
    EdmElement,
    IEdmLabeledExpression,
    IEdmNamedElement,
    IEdmElement,
    IEdmExpression
  {
    private readonly string name;
    private readonly IEdmExpression expression;

    public EdmLabeledExpression(string name, IEdmExpression expression)
    {
      EdmUtil.CheckArgumentNull<string>(name, nameof (name));
      EdmUtil.CheckArgumentNull<IEdmExpression>(expression, nameof (expression));
      this.name = name;
      this.expression = expression;
    }

    public string Name => this.name;

    public IEdmExpression Expression => this.expression;

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.Labeled;
  }
}
