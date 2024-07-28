// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.SqlOffsetSpec
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects.Visitors;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.Azure.Cosmos.SqlObjects
{
  internal sealed class SqlOffsetSpec : SqlObject
  {
    private const int PremadeOffsetIndex = 256;
    private static readonly ImmutableArray<SqlOffsetSpec> PremadeOffsetSpecs = Enumerable.Range(0, 256).Select<int, SqlOffsetSpec>((Func<int, SqlOffsetSpec>) (offset => new SqlOffsetSpec((SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlNumberLiteral.Create((Number64) (long) offset))))).ToImmutableArray<SqlOffsetSpec>();

    private SqlOffsetSpec(SqlScalarExpression offsetExpression) => this.OffsetExpression = offsetExpression ?? throw new ArgumentNullException(nameof (offsetExpression));

    public SqlScalarExpression OffsetExpression { get; }

    public static SqlOffsetSpec Create(SqlNumberLiteral sqlNumberLiteral)
    {
      if ((SqlObject) sqlNumberLiteral == (SqlObject) null)
        throw new ArgumentNullException(nameof (sqlNumberLiteral));
      if (!sqlNumberLiteral.Value.IsInteger)
        throw new ArgumentOutOfRangeException("Expected sqlNumberLiteral to be an integer.");
      long index = Number64.ToLong(sqlNumberLiteral.Value);
      return index < 256L && index >= 0L ? SqlOffsetSpec.PremadeOffsetSpecs[(int) index] : new SqlOffsetSpec((SqlScalarExpression) SqlLiteralScalarExpression.Create((SqlLiteral) SqlNumberLiteral.Create((Number64) index)));
    }

    public static SqlOffsetSpec Create(SqlParameter sqlParameter) => !((SqlObject) sqlParameter == (SqlObject) null) ? new SqlOffsetSpec((SqlScalarExpression) SqlParameterRefScalarExpression.Create(sqlParameter)) : throw new ArgumentNullException(nameof (sqlParameter));

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
