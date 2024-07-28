// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.SqlArrayIteratorCollectionExpression
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects.Visitors;
using System;

namespace Microsoft.Azure.Cosmos.SqlObjects
{
  internal sealed class SqlArrayIteratorCollectionExpression : SqlCollectionExpression
  {
    private SqlArrayIteratorCollectionExpression(SqlIdentifier identifier, SqlCollection collection)
    {
      this.Identifier = identifier ?? throw new ArgumentNullException(nameof (identifier));
      this.Collection = collection ?? throw new ArgumentNullException(nameof (collection));
    }

    public SqlIdentifier Identifier { get; }

    public SqlCollection Collection { get; }

    public static SqlArrayIteratorCollectionExpression Create(
      SqlIdentifier identifier,
      SqlCollection collection)
    {
      return new SqlArrayIteratorCollectionExpression(identifier, collection);
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
