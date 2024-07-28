// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlOffsetLimitClause
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlOffsetLimitClause : SqlObject
  {
    private SqlOffsetLimitClause(SqlOffsetSpec offsetSpec, SqlLimitSpec limitSpec)
      : base(SqlObjectKind.OffsetLimitClause)
    {
      if (offsetSpec == null)
        throw new ArgumentNullException(nameof (offsetSpec));
      if (limitSpec == null)
        throw new ArgumentNullException(nameof (limitSpec));
      this.OffsetSpec = offsetSpec;
      this.LimitSpec = limitSpec;
    }

    public SqlOffsetSpec OffsetSpec { get; }

    public SqlLimitSpec LimitSpec { get; }

    public static SqlOffsetLimitClause Create(SqlOffsetSpec offsetSpec, SqlLimitSpec limitSpec) => new SqlOffsetLimitClause(offsetSpec, limitSpec);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
