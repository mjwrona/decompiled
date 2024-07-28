// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlWhereClause
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlWhereClause : SqlObject
  {
    private SqlWhereClause(SqlScalarExpression filterExpression)
      : base(SqlObjectKind.WhereClause)
    {
      this.FilterExpression = filterExpression != null ? filterExpression : throw new ArgumentNullException(nameof (filterExpression));
    }

    public SqlScalarExpression FilterExpression { get; }

    public static SqlWhereClause Create(SqlScalarExpression filterExpression) => new SqlWhereClause(filterExpression);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
