// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlQuery
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Documents.Sql
{
  [JsonConverter(typeof (SqlQueryJsonConverter))]
  internal class SqlQuery : SqlObject
  {
    protected SqlQuery(
      SqlSelectClause selectClause,
      SqlFromClause fromClause,
      SqlWhereClause whereClause,
      SqlGroupByClause groupByClause,
      SqlOrderbyClause orderbyClause,
      SqlOffsetLimitClause offsetLimitClause)
      : base(SqlObjectKind.Query)
    {
      this.SelectClause = selectClause != null ? selectClause : throw new ArgumentNullException("selectClause must not be null.");
      this.FromClause = fromClause;
      this.WhereClause = whereClause;
      this.GroupByClause = groupByClause;
      this.OrderbyClause = orderbyClause;
      this.OffsetLimitClause = offsetLimitClause;
    }

    public SqlSelectClause SelectClause { get; }

    public SqlFromClause FromClause { get; }

    public SqlWhereClause WhereClause { get; }

    public SqlGroupByClause GroupByClause { get; }

    public SqlOrderbyClause OrderbyClause { get; }

    public SqlOffsetLimitClause OffsetLimitClause { get; }

    public static SqlQuery Create(
      SqlSelectClause selectClause,
      SqlFromClause fromClause,
      SqlWhereClause whereClause,
      SqlGroupByClause groupByClause,
      SqlOrderbyClause orderByClause,
      SqlOffsetLimitClause offsetLimitClause)
    {
      return new SqlQuery(selectClause, fromClause, whereClause, groupByClause, orderByClause, offsetLimitClause);
    }

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
