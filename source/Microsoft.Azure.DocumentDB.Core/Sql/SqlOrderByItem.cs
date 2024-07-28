// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlOrderByItem
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlOrderByItem : SqlObject
  {
    private SqlOrderByItem(SqlScalarExpression expression, bool isDescending)
      : base(SqlObjectKind.OrderByItem)
    {
      this.Expression = expression != null ? expression : throw new ArgumentNullException(nameof (expression));
      this.IsDescending = isDescending;
    }

    public SqlScalarExpression Expression { get; }

    public bool IsDescending { get; }

    public static SqlOrderByItem Create(SqlScalarExpression expression, bool isDescending) => new SqlOrderByItem(expression, isDescending);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
