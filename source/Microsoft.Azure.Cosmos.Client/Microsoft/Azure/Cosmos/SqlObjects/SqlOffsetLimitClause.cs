// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.SqlOffsetLimitClause
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects.Visitors;
using System;

namespace Microsoft.Azure.Cosmos.SqlObjects
{
  internal sealed class SqlOffsetLimitClause : SqlObject
  {
    private SqlOffsetLimitClause(SqlOffsetSpec offsetSpec, SqlLimitSpec limitSpec)
    {
      this.OffsetSpec = offsetSpec ?? throw new ArgumentNullException(nameof (offsetSpec));
      this.LimitSpec = limitSpec ?? throw new ArgumentNullException(nameof (limitSpec));
    }

    public SqlOffsetSpec OffsetSpec { get; }

    public SqlLimitSpec LimitSpec { get; }

    public static SqlOffsetLimitClause Create(SqlOffsetSpec offsetSpec, SqlLimitSpec limitSpec) => new SqlOffsetLimitClause(offsetSpec, limitSpec);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
