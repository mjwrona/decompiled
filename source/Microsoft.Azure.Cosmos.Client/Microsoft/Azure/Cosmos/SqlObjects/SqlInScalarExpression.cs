// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.SqlInScalarExpression
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects.Visitors;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.Azure.Cosmos.SqlObjects
{
  internal sealed class SqlInScalarExpression : SqlScalarExpression
  {
    private SqlInScalarExpression(
      SqlScalarExpression needle,
      bool not,
      ImmutableArray<SqlScalarExpression> haystack)
    {
      if (haystack.IsEmpty)
        throw new ArgumentException("items can't be empty.");
      foreach (SqlObject sqlObject in haystack)
      {
        if (sqlObject == (SqlObject) null)
          throw new ArgumentException("items can't have a null item.");
      }
      this.Needle = needle ?? throw new ArgumentNullException(nameof (needle));
      this.Not = not;
      this.Haystack = haystack;
    }

    public SqlScalarExpression Needle { get; }

    public bool Not { get; }

    public ImmutableArray<SqlScalarExpression> Haystack { get; }

    public static SqlInScalarExpression Create(
      SqlScalarExpression needle,
      bool not,
      params SqlScalarExpression[] haystack)
    {
      return new SqlInScalarExpression(needle, not, ((IEnumerable<SqlScalarExpression>) haystack).ToImmutableArray<SqlScalarExpression>());
    }

    public static SqlInScalarExpression Create(
      SqlScalarExpression needle,
      bool not,
      ImmutableArray<SqlScalarExpression> haystack)
    {
      return new SqlInScalarExpression(needle, not, haystack);
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
