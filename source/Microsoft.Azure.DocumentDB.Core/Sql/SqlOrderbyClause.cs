// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlOrderbyClause
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlOrderbyClause : SqlObject
  {
    private SqlOrderbyClause(IReadOnlyList<SqlOrderByItem> orderbyItems)
      : base(SqlObjectKind.OrderByClause)
    {
      if (orderbyItems == null)
        throw new ArgumentNullException(nameof (orderbyItems));
      foreach (SqlOrderByItem orderbyItem in (IEnumerable<SqlOrderByItem>) orderbyItems)
      {
        if (orderbyItem == null)
          throw new ArgumentException("sqlOrderbyItem must have have null items.");
      }
      this.OrderbyItems = orderbyItems;
    }

    public IReadOnlyList<SqlOrderByItem> OrderbyItems { get; }

    public static SqlOrderbyClause Create(params SqlOrderByItem[] orderbyItems) => new SqlOrderbyClause((IReadOnlyList<SqlOrderByItem>) orderbyItems);

    public static SqlOrderbyClause Create(IReadOnlyList<SqlOrderByItem> orderbyItems) => new SqlOrderbyClause(orderbyItems);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
