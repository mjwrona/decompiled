// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.BinaryOperatorToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser
{
  public sealed class BinaryOperatorToken : QueryToken
  {
    private readonly BinaryOperatorKind operatorKind;
    private readonly QueryToken left;
    private readonly QueryToken right;

    public BinaryOperatorToken(BinaryOperatorKind operatorKind, QueryToken left, QueryToken right)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryToken>(left, nameof (left));
      ExceptionUtils.CheckArgumentNotNull<QueryToken>(right, nameof (right));
      this.operatorKind = operatorKind;
      this.left = left;
      this.right = right;
    }

    public override QueryTokenKind Kind => QueryTokenKind.BinaryOperator;

    public BinaryOperatorKind OperatorKind => this.operatorKind;

    public QueryToken Left => this.left;

    public QueryToken Right => this.right;

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
  }
}
