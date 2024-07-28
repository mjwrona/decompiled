// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.SqlBetweenScalarExpression
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects.Visitors;

namespace Microsoft.Azure.Cosmos.SqlObjects
{
  internal sealed class SqlBetweenScalarExpression : SqlScalarExpression
  {
    private SqlBetweenScalarExpression(
      SqlScalarExpression expression,
      SqlScalarExpression startInclusive,
      SqlScalarExpression endInclusive,
      bool not)
    {
      this.Expression = expression;
      this.StartInclusive = startInclusive;
      this.EndInclusive = endInclusive;
      this.Not = not;
    }

    public SqlScalarExpression Expression { get; }

    public bool Not { get; }

    public SqlScalarExpression StartInclusive { get; }

    public SqlScalarExpression EndInclusive { get; }

    public static SqlBetweenScalarExpression Create(
      SqlScalarExpression expression,
      SqlScalarExpression startInclusive,
      SqlScalarExpression endInclusive,
      bool not)
    {
      return new SqlBetweenScalarExpression(expression, startInclusive, endInclusive, not);
    }

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);

    public override void Accept(SqlScalarExpressionVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlScalarExpressionVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(
      SqlScalarExpressionVisitor<T, TResult> visitor,
      T input)
    {
      return visitor.Visit(this, input);
    }
  }
}
