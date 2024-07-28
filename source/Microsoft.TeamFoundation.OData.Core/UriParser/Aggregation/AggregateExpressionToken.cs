// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.Aggregation.AggregateExpressionToken
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser.Aggregation
{
  public sealed class AggregateExpressionToken : AggregateTokenBase
  {
    private readonly QueryToken expression;
    private readonly AggregationMethod method;
    private readonly AggregationMethodDefinition methodDefinition;
    private readonly string alias;

    public AggregateExpressionToken(QueryToken expression, AggregationMethod method, string alias)
    {
      ExceptionUtils.CheckArgumentNotNull<QueryToken>(expression, nameof (expression));
      ExceptionUtils.CheckArgumentNotNull<string>(alias, nameof (alias));
      this.expression = expression;
      this.method = method;
      this.alias = alias;
    }

    public AggregateExpressionToken(
      QueryToken expression,
      AggregationMethodDefinition methodDefinition,
      string alias)
      : this(expression, methodDefinition.MethodKind, alias)
    {
      this.methodDefinition = methodDefinition;
    }

    public override QueryTokenKind Kind => QueryTokenKind.AggregateExpression;

    public AggregationMethod Method => this.method;

    public QueryToken Expression => this.expression;

    public AggregationMethodDefinition MethodDefinition => this.methodDefinition;

    public string Alias => this.alias;

    public override T Accept<T>(ISyntacticTreeVisitor<T> visitor) => visitor.Visit(this);
  }
}
