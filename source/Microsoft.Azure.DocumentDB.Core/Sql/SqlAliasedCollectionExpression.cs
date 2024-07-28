// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlAliasedCollectionExpression
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlAliasedCollectionExpression : SqlCollectionExpression
  {
    private SqlAliasedCollectionExpression(SqlCollection collection, SqlIdentifier alias)
      : base(SqlObjectKind.AliasedCollectionExpression)
    {
      this.Collection = collection != null ? collection : throw new ArgumentNullException(nameof (collection));
      this.Alias = alias;
    }

    public SqlCollection Collection { get; }

    public SqlIdentifier Alias { get; }

    public static SqlAliasedCollectionExpression Create(
      SqlCollection collection,
      SqlIdentifier alias)
    {
      return new SqlAliasedCollectionExpression(collection, alias);
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
