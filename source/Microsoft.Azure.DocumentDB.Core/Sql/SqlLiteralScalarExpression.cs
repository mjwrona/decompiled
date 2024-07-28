// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlLiteralScalarExpression
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlLiteralScalarExpression : SqlScalarExpression
  {
    public static readonly SqlLiteralScalarExpression SqlNullLiteralScalarExpression = new SqlLiteralScalarExpression((SqlLiteral) SqlNullLiteral.Create());
    public static readonly SqlLiteralScalarExpression SqlTrueLiteralScalarExpression = new SqlLiteralScalarExpression((SqlLiteral) SqlBooleanLiteral.True);
    public static readonly SqlLiteralScalarExpression SqlFalseLiteralScalarExpression = new SqlLiteralScalarExpression((SqlLiteral) SqlBooleanLiteral.False);
    public static readonly SqlLiteralScalarExpression SqlUndefinedLiteralScalarExpression = new SqlLiteralScalarExpression((SqlLiteral) SqlUndefinedLiteral.Singleton);

    private SqlLiteralScalarExpression(SqlLiteral literal)
      : base(SqlObjectKind.LiteralScalarExpression)
    {
      this.Literal = literal != null ? literal : throw new ArgumentNullException(nameof (literal));
    }

    public SqlLiteral Literal { get; }

    public static SqlLiteralScalarExpression Create(SqlLiteral sqlLiteral) => sqlLiteral != SqlBooleanLiteral.True ? (sqlLiteral != SqlBooleanLiteral.False ? (sqlLiteral != SqlNullLiteral.Singleton ? (sqlLiteral != SqlUndefinedLiteral.Singleton ? new SqlLiteralScalarExpression(sqlLiteral) : SqlLiteralScalarExpression.SqlUndefinedLiteralScalarExpression) : SqlLiteralScalarExpression.SqlNullLiteralScalarExpression) : SqlLiteralScalarExpression.SqlFalseLiteralScalarExpression) : SqlLiteralScalarExpression.SqlTrueLiteralScalarExpression;

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
