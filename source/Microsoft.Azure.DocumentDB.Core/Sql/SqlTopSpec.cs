// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlTopSpec
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Linq;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlTopSpec : SqlObject
  {
    private const int PremadeTopIndex = 256;
    private static readonly SqlTopSpec[] PremadeTopSpecs = Enumerable.Range(0, 256).Select<int, SqlTopSpec>((Func<int, SqlTopSpec>) (top => new SqlTopSpec((long) top))).ToArray<SqlTopSpec>();

    private SqlTopSpec(long count)
      : base(SqlObjectKind.TopSpec)
    {
      this.Count = count;
    }

    public long Count { get; }

    public static SqlTopSpec Create(long value) => value < 256L && value >= 0L ? SqlTopSpec.PremadeTopSpecs[value] : new SqlTopSpec(value);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
