// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlArrayScalarExpression
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlArrayScalarExpression : SqlScalarExpression
  {
    public SqlQuery SqlQuery { get; }

    private SqlArrayScalarExpression(SqlQuery sqlQuery)
      : base(SqlObjectKind.ArrayScalarExpression)
    {
      this.SqlQuery = sqlQuery != null ? sqlQuery : throw new ArgumentNullException("sqlQuery can not be null");
    }

    public static SqlArrayScalarExpression Create(SqlQuery sqlQuery) => new SqlArrayScalarExpression(sqlQuery);

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
