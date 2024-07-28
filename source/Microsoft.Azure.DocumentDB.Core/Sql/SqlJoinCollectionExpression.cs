// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlJoinCollectionExpression
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlJoinCollectionExpression : SqlCollectionExpression
  {
    private SqlJoinCollectionExpression(
      SqlCollectionExpression leftExpression,
      SqlCollectionExpression rightExpression)
      : base(SqlObjectKind.JoinCollectionExpression)
    {
      this.LeftExpression = leftExpression;
      this.RightExpression = rightExpression;
    }

    public SqlCollectionExpression LeftExpression { get; }

    public SqlCollectionExpression RightExpression { get; }

    public static SqlJoinCollectionExpression Create(
      SqlCollectionExpression leftExpression,
      SqlCollectionExpression rightExpression)
    {
      return new SqlJoinCollectionExpression(leftExpression, rightExpression);
    }

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);

    public override void Accept(SqlCollectionExpressionVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlCollectionExpressionVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(
      SqlCollectionExpressionVisitor<T, TResult> visitor,
      T input)
    {
      return visitor.Visit(this, input);
    }
  }
}
