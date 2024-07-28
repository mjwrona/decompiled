// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlSelectClause
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlSelectClause : SqlObject
  {
    public static readonly SqlSelectClause SelectStar = new SqlSelectClause((SqlSelectSpec) SqlSelectStarSpec.Singleton);

    private SqlSelectClause(SqlSelectSpec selectSpec, SqlTopSpec topSpec = null, bool hasDistinct = false)
      : base(SqlObjectKind.SelectClause)
    {
      this.SelectSpec = selectSpec != null ? selectSpec : throw new ArgumentNullException(nameof (selectSpec));
      this.TopSpec = topSpec;
      this.HasDistinct = hasDistinct;
    }

    public SqlSelectSpec SelectSpec { get; }

    public SqlTopSpec TopSpec { get; }

    public bool HasDistinct { get; }

    public static SqlSelectClause Create(
      SqlSelectSpec selectSpec,
      SqlTopSpec topSpec = null,
      bool hasDistinct = false)
    {
      return new SqlSelectClause(selectSpec, topSpec, hasDistinct);
    }

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
