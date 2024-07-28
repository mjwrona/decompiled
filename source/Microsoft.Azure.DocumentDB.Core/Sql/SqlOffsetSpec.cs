// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlOffsetSpec
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Linq;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlOffsetSpec : SqlObject
  {
    private const int PremadeOffsetIndex = 256;
    private static readonly SqlOffsetSpec[] PremadeOffsetSpecs = Enumerable.Range(0, 256).Select<int, SqlOffsetSpec>((Func<int, SqlOffsetSpec>) (offset => new SqlOffsetSpec((long) offset))).ToArray<SqlOffsetSpec>();

    private SqlOffsetSpec(long offset)
      : base(SqlObjectKind.OffsetSpec)
    {
      this.Offset = offset;
    }

    public long Offset { get; }

    public static SqlOffsetSpec Create(long value) => value < 256L && value >= 0L ? SqlOffsetSpec.PremadeOffsetSpecs[value] : new SqlOffsetSpec(value);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
