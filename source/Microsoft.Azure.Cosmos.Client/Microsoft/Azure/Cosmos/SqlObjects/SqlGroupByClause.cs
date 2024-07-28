// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.SqlGroupByClause
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects.Visitors;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.Azure.Cosmos.SqlObjects
{
  internal sealed class SqlGroupByClause : SqlObject
  {
    private SqlGroupByClause(ImmutableArray<SqlScalarExpression> expressions)
    {
      foreach (SqlObject expression in expressions)
      {
        if (expression == (SqlObject) null)
          throw new ArgumentException("expressions must not have null items.");
      }
      this.Expressions = expressions;
    }

    public ImmutableArray<SqlScalarExpression> Expressions { get; }

    public static SqlGroupByClause Create(params SqlScalarExpression[] expressions) => new SqlGroupByClause(((IEnumerable<SqlScalarExpression>) expressions).ToImmutableArray<SqlScalarExpression>());

    public static SqlGroupByClause Create(ImmutableArray<SqlScalarExpression> expressions) => new SqlGroupByClause(expressions);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
