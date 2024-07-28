// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.SqlJoinCollectionExpression
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects.Visitors;

namespace Microsoft.Azure.Cosmos.SqlObjects
{
  internal sealed class SqlJoinCollectionExpression : SqlCollectionExpression
  {
    private SqlJoinCollectionExpression(SqlCollectionExpression left, SqlCollectionExpression right)
    {
      this.Left = left;
      this.Right = right;
    }

    public SqlCollectionExpression Left { get; }

    public SqlCollectionExpression Right { get; }

    public static SqlJoinCollectionExpression Create(
      SqlCollectionExpression left,
      SqlCollectionExpression right)
    {
      return new SqlJoinCollectionExpression(left, right);
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
