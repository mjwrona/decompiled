// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlProgram
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlProgram : SqlObject
  {
    private SqlProgram(SqlQuery query)
      : base(SqlObjectKind.Program)
    {
      this.Query = query != null ? query : throw new ArgumentNullException("query must not be null.");
    }

    public SqlQuery Query { get; }

    public static SqlProgram Create(SqlQuery query) => new SqlProgram(query);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
