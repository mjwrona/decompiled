// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlLimitSpec
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Linq;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlLimitSpec : SqlObject
  {
    private const int PremadeLimitIndex = 256;
    private static readonly SqlLimitSpec[] PremadeLimitSpecs = Enumerable.Range(0, 256).Select<int, SqlLimitSpec>((Func<int, SqlLimitSpec>) (limit => new SqlLimitSpec((long) limit))).ToArray<SqlLimitSpec>();

    private SqlLimitSpec(long limit)
      : base(SqlObjectKind.LimitSpec)
    {
      this.Limit = limit;
    }

    public long Limit { get; }

    public static SqlLimitSpec Create(long value) => value < 256L && value >= 0L ? SqlLimitSpec.PremadeLimitSpecs[value] : new SqlLimitSpec(value);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
