// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmIfExpression
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmIfExpression : EdmElement, IEdmIfExpression, IEdmExpression, IEdmElement
  {
    private readonly IEdmExpression testExpression;
    private readonly IEdmExpression trueExpression;
    private readonly IEdmExpression falseExpression;

    public EdmIfExpression(
      IEdmExpression testExpression,
      IEdmExpression trueExpression,
      IEdmExpression falseExpression)
    {
      EdmUtil.CheckArgumentNull<IEdmExpression>(testExpression, nameof (testExpression));
      EdmUtil.CheckArgumentNull<IEdmExpression>(trueExpression, nameof (trueExpression));
      EdmUtil.CheckArgumentNull<IEdmExpression>(falseExpression, nameof (falseExpression));
      this.testExpression = testExpression;
      this.trueExpression = trueExpression;
      this.falseExpression = falseExpression;
    }

    public IEdmExpression TestExpression => this.testExpression;

    public IEdmExpression TrueExpression => this.trueExpression;

    public IEdmExpression FalseExpression => this.falseExpression;

    public EdmExpressionKind ExpressionKind => EdmExpressionKind.If;
  }
}
