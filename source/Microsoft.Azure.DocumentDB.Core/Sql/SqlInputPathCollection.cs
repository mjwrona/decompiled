// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlInputPathCollection
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlInputPathCollection : SqlCollection
  {
    private SqlInputPathCollection(SqlIdentifier input, SqlPathExpression relativePath)
      : base(SqlObjectKind.InputPathCollection)
    {
      this.Input = input != null ? input : throw new ArgumentNullException(nameof (input));
      this.RelativePath = relativePath;
    }

    public SqlIdentifier Input { get; }

    public SqlPathExpression RelativePath { get; }

    public static SqlInputPathCollection Create(SqlIdentifier input, SqlPathExpression relativePath) => new SqlInputPathCollection(input, relativePath);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);

    public override void Accept(SqlCollectionVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlCollectionVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlCollectionVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
