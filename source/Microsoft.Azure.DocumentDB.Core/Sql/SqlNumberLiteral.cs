// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlNumberLiteral
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlNumberLiteral : SqlLiteral
  {
    private const int Capacity = 256;
    private static readonly Dictionary<long, SqlNumberLiteral> FrequentLongs = Enumerable.Range(-256, 256).ToDictionary<int, long, SqlNumberLiteral>((Func<int, long>) (x => (long) x), (Func<int, SqlNumberLiteral>) (x => new SqlNumberLiteral((Number64) (long) x)));
    private static readonly Dictionary<double, SqlNumberLiteral> FrequentDoubles = Enumerable.Range(-256, 256).ToDictionary<int, double, SqlNumberLiteral>((Func<int, double>) (x => (double) x), (Func<int, SqlNumberLiteral>) (x => new SqlNumberLiteral((Number64) (double) x)));

    private SqlNumberLiteral(Number64 value)
      : base(SqlObjectKind.NumberLiteral)
    {
      this.Value = value;
    }

    public Number64 Value { get; }

    public static SqlNumberLiteral Create(Decimal number) => !(number >= -9223372036854775808M) || !(number <= 9223372036854775807M) || !(number % 1M == 0M) ? SqlNumberLiteral.Create(Convert.ToDouble(number)) : SqlNumberLiteral.Create(Convert.ToInt64(number));

    public static SqlNumberLiteral Create(double number)
    {
      SqlNumberLiteral sqlNumberLiteral;
      if (!SqlNumberLiteral.FrequentDoubles.TryGetValue(number, out sqlNumberLiteral))
        sqlNumberLiteral = new SqlNumberLiteral((Number64) number);
      return sqlNumberLiteral;
    }

    public static SqlNumberLiteral Create(long number)
    {
      SqlNumberLiteral sqlNumberLiteral;
      if (!SqlNumberLiteral.FrequentLongs.TryGetValue(number, out sqlNumberLiteral))
        sqlNumberLiteral = new SqlNumberLiteral((Number64) number);
      return sqlNumberLiteral;
    }

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);

    public override void Accept(SqlLiteralVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlLiteralVisitor<TResult> visitor) => visitor.Visit(this);
  }
}
