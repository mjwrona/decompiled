// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlSelectItem
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlSelectItem : SqlObject
  {
    private SqlSelectItem(SqlScalarExpression expression, SqlIdentifier alias)
      : base(SqlObjectKind.SelectItem)
    {
      this.Expression = expression != null ? expression : throw new ArgumentNullException(nameof (expression));
      this.Alias = alias;
    }

    public SqlScalarExpression Expression { get; }

    public SqlIdentifier Alias { get; }

    public static SqlSelectItem Create(SqlScalarExpression expression, SqlIdentifier alias = null) => new SqlSelectItem(expression, alias);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
