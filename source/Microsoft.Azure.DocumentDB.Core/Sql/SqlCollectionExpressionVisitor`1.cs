// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlCollectionExpressionVisitor`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents.Sql
{
  internal abstract class SqlCollectionExpressionVisitor<TResult>
  {
    public abstract TResult Visit(
      SqlAliasedCollectionExpression collectionExpression);

    public abstract TResult Visit(
      SqlArrayIteratorCollectionExpression collectionExpression);

    public abstract TResult Visit(SqlJoinCollectionExpression collectionExpression);
  }
}
