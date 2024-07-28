// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlArrayCreateScalarExpression
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlArrayCreateScalarExpression : SqlScalarExpression
  {
    private static readonly SqlArrayCreateScalarExpression Empty = new SqlArrayCreateScalarExpression((IReadOnlyList<SqlScalarExpression>) new List<SqlScalarExpression>());

    private SqlArrayCreateScalarExpression(IReadOnlyList<SqlScalarExpression> items)
      : base(SqlObjectKind.ArrayCreateScalarExpression)
    {
      if (items == null)
        throw new ArgumentNullException("items must not be null.");
      foreach (SqlScalarExpression scalarExpression in (IEnumerable<SqlScalarExpression>) items)
      {
        if (scalarExpression == null)
          throw new ArgumentException("item must not have null items.");
      }
      this.Items = (IReadOnlyList<SqlScalarExpression>) new List<SqlScalarExpression>((IEnumerable<SqlScalarExpression>) items);
    }

    public IReadOnlyList<SqlScalarExpression> Items { get; }

    public static SqlArrayCreateScalarExpression Create() => SqlArrayCreateScalarExpression.Empty;

    public static SqlArrayCreateScalarExpression Create(params SqlScalarExpression[] items) => new SqlArrayCreateScalarExpression((IReadOnlyList<SqlScalarExpression>) items);

    public static SqlArrayCreateScalarExpression Create(IReadOnlyList<SqlScalarExpression> items) => new SqlArrayCreateScalarExpression(items);

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
