// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlStringPathExpression
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlStringPathExpression : SqlPathExpression
  {
    private SqlStringPathExpression(SqlPathExpression parentPath, SqlStringLiteral value)
      : base(SqlObjectKind.StringPathExpression, parentPath)
    {
      this.Value = value != null ? value : throw new ArgumentNullException(nameof (value));
    }

    public SqlStringLiteral Value { get; }

    public static SqlStringPathExpression Create(
      SqlPathExpression parentPath,
      SqlStringLiteral value)
    {
      return new SqlStringPathExpression(parentPath, value);
    }

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);

    public override void Accept(SqlPathExpressionVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlPathExpressionVisitor<TResult> visitor) => visitor.Visit(this);
  }
}
