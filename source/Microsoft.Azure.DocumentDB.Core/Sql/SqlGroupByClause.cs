// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlGroupByClause
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlGroupByClause : SqlObject
  {
    private SqlGroupByClause(IReadOnlyList<SqlScalarExpression> expressions)
      : base(SqlObjectKind.GroupByClause)
    {
      if (expressions == null)
        throw new ArgumentNullException(nameof (expressions));
      foreach (SqlScalarExpression expression in (IEnumerable<SqlScalarExpression>) expressions)
      {
        if (expression == null)
          throw new ArgumentException("expressions must not have null items.");
      }
      this.Expressions = expressions;
    }

    public IReadOnlyList<SqlScalarExpression> Expressions { get; }

    public static SqlGroupByClause Create(params SqlScalarExpression[] expressions) => new SqlGroupByClause((IReadOnlyList<SqlScalarExpression>) expressions);

    public static SqlGroupByClause Create(IReadOnlyList<SqlScalarExpression> expressions) => new SqlGroupByClause(expressions);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
