// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.SqlArrayCreateScalarExpression
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects.Visitors;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.Azure.Cosmos.SqlObjects
{
  internal sealed class SqlArrayCreateScalarExpression : SqlScalarExpression
  {
    private static readonly SqlArrayCreateScalarExpression Empty = new SqlArrayCreateScalarExpression(ImmutableArray<SqlScalarExpression>.Empty);

    private SqlArrayCreateScalarExpression(ImmutableArray<SqlScalarExpression> items)
    {
      foreach (SqlObject sqlObject in items)
      {
        if (sqlObject == (SqlObject) null)
          throw new ArgumentException("item must not have null items.");
      }
      this.Items = items;
    }

    public ImmutableArray<SqlScalarExpression> Items { get; }

    public static SqlArrayCreateScalarExpression Create() => SqlArrayCreateScalarExpression.Empty;

    public static SqlArrayCreateScalarExpression Create(params SqlScalarExpression[] items) => new SqlArrayCreateScalarExpression(((IEnumerable<SqlScalarExpression>) items).ToImmutableArray<SqlScalarExpression>());

    public static SqlArrayCreateScalarExpression Create(ImmutableArray<SqlScalarExpression> items) => new SqlArrayCreateScalarExpression(items);

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
