// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlSelectValueSpec
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlSelectValueSpec : SqlSelectSpec
  {
    private SqlSelectValueSpec(SqlScalarExpression expression)
      : base(SqlObjectKind.SelectValueSpec)
    {
      this.Expression = expression != null ? expression : throw new ArgumentNullException(nameof (expression));
    }

    public SqlScalarExpression Expression { get; }

    public static SqlSelectValueSpec Create(SqlScalarExpression expression) => new SqlSelectValueSpec(expression);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);

    public override void Accept(SqlSelectSpecVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlSelectSpecVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlSelectSpecVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
