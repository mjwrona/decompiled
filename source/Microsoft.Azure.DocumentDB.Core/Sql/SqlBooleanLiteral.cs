// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlBooleanLiteral
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlBooleanLiteral : SqlLiteral
  {
    public static readonly SqlBooleanLiteral True = new SqlBooleanLiteral(true);
    public static readonly SqlBooleanLiteral False = new SqlBooleanLiteral(false);

    private SqlBooleanLiteral(bool value)
      : base(SqlObjectKind.BooleanLiteral)
    {
      this.Value = value;
    }

    public bool Value { get; }

    public static SqlBooleanLiteral Create(bool value) => !value ? SqlBooleanLiteral.False : SqlBooleanLiteral.True;

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);

    public override void Accept(SqlLiteralVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlLiteralVisitor<TResult> visitor) => visitor.Visit(this);
  }
}
