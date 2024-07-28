// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.SqlBooleanLiteral
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects.Visitors;

namespace Microsoft.Azure.Cosmos.SqlObjects
{
  internal sealed class SqlBooleanLiteral : SqlLiteral
  {
    public static readonly SqlBooleanLiteral True = new SqlBooleanLiteral(true);
    public static readonly SqlBooleanLiteral False = new SqlBooleanLiteral(false);

    private SqlBooleanLiteral(bool value) => this.Value = value;

    public bool Value { get; }

    public static SqlBooleanLiteral Create(bool value) => !value ? SqlBooleanLiteral.False : SqlBooleanLiteral.True;

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);

    public override void Accept(SqlLiteralVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlLiteralVisitor<TResult> visitor) => visitor.Visit(this);
  }
}
