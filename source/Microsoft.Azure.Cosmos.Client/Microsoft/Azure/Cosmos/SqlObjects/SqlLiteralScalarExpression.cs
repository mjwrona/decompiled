// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.SqlLiteralScalarExpression
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects.Visitors;
using System;

namespace Microsoft.Azure.Cosmos.SqlObjects
{
  internal sealed class SqlLiteralScalarExpression : SqlScalarExpression
  {
    public static readonly SqlLiteralScalarExpression SqlNullLiteralScalarExpression = new SqlLiteralScalarExpression((SqlLiteral) SqlNullLiteral.Create());
    public static readonly SqlLiteralScalarExpression SqlTrueLiteralScalarExpression = new SqlLiteralScalarExpression((SqlLiteral) SqlBooleanLiteral.True);
    public static readonly SqlLiteralScalarExpression SqlFalseLiteralScalarExpression = new SqlLiteralScalarExpression((SqlLiteral) SqlBooleanLiteral.False);
    public static readonly SqlLiteralScalarExpression SqlUndefinedLiteralScalarExpression = new SqlLiteralScalarExpression((SqlLiteral) SqlUndefinedLiteral.Create());

    private SqlLiteralScalarExpression(SqlLiteral literal) => this.Literal = literal ?? throw new ArgumentNullException(nameof (literal));

    public SqlLiteral Literal { get; }

    public static SqlLiteralScalarExpression Create(SqlLiteral sqlLiteral) => !((SqlObject) sqlLiteral == (SqlObject) SqlBooleanLiteral.True) ? (!((SqlObject) sqlLiteral == (SqlObject) SqlBooleanLiteral.False) ? (!((SqlObject) sqlLiteral == (SqlObject) SqlNullLiteral.Singleton) ? (!((SqlObject) sqlLiteral == (SqlObject) SqlUndefinedLiteral.Create()) ? new SqlLiteralScalarExpression(sqlLiteral) : SqlLiteralScalarExpression.SqlUndefinedLiteralScalarExpression) : SqlLiteralScalarExpression.SqlNullLiteralScalarExpression) : SqlLiteralScalarExpression.SqlFalseLiteralScalarExpression) : SqlLiteralScalarExpression.SqlTrueLiteralScalarExpression;

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);

    public override void Accept(SqlScalarExpressionVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlScalarExpressionVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(
      SqlScalarExpressionVisitor<T, TResult> visitor,
      T input)
    {
      return visitor.Visit(this, input);
    }
  }
}
