// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.UnaryOperatorToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser
{
  public sealed class UnaryOperatorToken : QueryToken
  {
    private readonly UnaryOperatorKind operatorKind;
    private readonly QueryToken operand;

    public UnaryOperatorToken(UnaryOperatorKind operatorKind, QueryToken operand)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryToken>(operand, nameof (operand));
      this.operatorKind = operatorKind;
      this.operand = operand;
    }

    public override QueryTokenKind Kind => QueryTokenKind.UnaryOperator;

    public UnaryOperatorKind OperatorKind => this.operatorKind;

    public QueryToken Operand => this.operand;

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
  }
}
