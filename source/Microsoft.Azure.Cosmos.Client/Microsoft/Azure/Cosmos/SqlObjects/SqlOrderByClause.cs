// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.SqlOrderByClause
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects.Visitors;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.Azure.Cosmos.SqlObjects
{
  internal sealed class SqlOrderByClause : SqlObject
  {
    private SqlOrderByClause(ImmutableArray<SqlOrderByItem> orderByItems)
    {
      foreach (SqlObject orderByItem in orderByItems)
      {
        if (orderByItem == (SqlObject) null)
          throw new ArgumentException("sqlOrderbyItem must have have null items.");
      }
      this.OrderByItems = orderByItems;
    }

    public ImmutableArray<SqlOrderByItem> OrderByItems { get; }

    public static SqlOrderByClause Create(params SqlOrderByItem[] orderByItems) => new SqlOrderByClause(((IEnumerable<SqlOrderByItem>) orderByItems).ToImmutableArray<SqlOrderByItem>());

    public static SqlOrderByClause Create(ImmutableArray<SqlOrderByItem> orderByItems) => new SqlOrderByClause(orderByItems);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
