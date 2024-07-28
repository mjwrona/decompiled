// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlObjectLiteral
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlObjectLiteral : SqlLiteral
  {
    public readonly bool isValueSerialized;

    public object Value { get; private set; }

    private SqlObjectLiteral(object value, bool isValueSerialized)
      : base(SqlObjectKind.ObjectLiteral)
    {
      this.Value = value != null ? value : throw new ArgumentNullException(nameof (value));
      this.isValueSerialized = isValueSerialized;
    }

    public static SqlObjectLiteral Create(object value, bool isValueSerialized) => new SqlObjectLiteral(value, isValueSerialized);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);

    public override void Accept(SqlLiteralVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlLiteralVisitor<TResult> visitor) => visitor.Visit(this);
  }
}
