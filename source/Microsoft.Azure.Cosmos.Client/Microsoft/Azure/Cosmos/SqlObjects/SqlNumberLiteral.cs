// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.SqlNumberLiteral
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects.Visitors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Cosmos.SqlObjects
{
  internal sealed class SqlNumberLiteral : SqlLiteral
  {
    private const int Capacity = 256;
    private static readonly Dictionary<long, SqlNumberLiteral> FrequentLongs = Enumerable.Range(-256, 256).ToDictionary<int, long, SqlNumberLiteral>((Func<int, long>) (x => (long) x), (Func<int, SqlNumberLiteral>) (x => new SqlNumberLiteral((Number64) (long) x)));
    private static readonly Dictionary<double, SqlNumberLiteral> FrequentDoubles = Enumerable.Range(-256, 256).ToDictionary<int, double, SqlNumberLiteral>((Func<int, double>) (x => (double) x), (Func<int, SqlNumberLiteral>) (x => new SqlNumberLiteral((Number64) (double) x)));

    private SqlNumberLiteral(Number64 value) => this.Value = value;

    public Number64 Value { get; }

    public static SqlNumberLiteral Create(Number64 number64)
    {
      SqlNumberLiteral sqlNumberLiteral;
      if (number64.IsDouble)
      {
        if (!SqlNumberLiteral.FrequentDoubles.TryGetValue(Number64.ToDouble(number64), out sqlNumberLiteral))
          sqlNumberLiteral = new SqlNumberLiteral(number64);
      }
      else if (!SqlNumberLiteral.FrequentLongs.TryGetValue(Number64.ToLong(number64), out sqlNumberLiteral))
        sqlNumberLiteral = new SqlNumberLiteral(number64);
      return sqlNumberLiteral;
    }

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);

    public override void Accept(SqlLiteralVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlLiteralVisitor<TResult> visitor) => visitor.Visit(this);
  }
}
