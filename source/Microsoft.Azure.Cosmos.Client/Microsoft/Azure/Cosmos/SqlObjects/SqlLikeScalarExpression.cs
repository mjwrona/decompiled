// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.SqlLikeScalarExpression
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects.Visitors;
using System;

namespace Microsoft.Azure.Cosmos.SqlObjects
{
  internal sealed class SqlLikeScalarExpression : SqlScalarExpression
  {
    private SqlLikeScalarExpression(
      SqlScalarExpression expression,
      SqlScalarExpression pattern,
      bool not,
      SqlStringLiteral escapeSequence = null)
    {
      this.Expression = expression ?? throw new ArgumentNullException(nameof (expression));
      this.Pattern = pattern ?? throw new ArgumentNullException(nameof (pattern));
      this.Not = not;
      this.EscapeSequence = escapeSequence;
    }

    public SqlScalarExpression Expression { get; }

    public SqlScalarExpression Pattern { get; }

    public bool Not { get; }

    public SqlStringLiteral EscapeSequence { get; }

    public static SqlLikeScalarExpression Create(
      SqlScalarExpression expression,
      SqlScalarExpression pattern,
      bool not,
      SqlStringLiteral escapeSequence = null)
    {
      return new SqlLikeScalarExpression(expression, pattern, not, escapeSequence);
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
