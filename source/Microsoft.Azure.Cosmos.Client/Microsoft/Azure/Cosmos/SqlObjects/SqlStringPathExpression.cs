// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.SqlStringPathExpression
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects.Visitors;
using System;

namespace Microsoft.Azure.Cosmos.SqlObjects
{
  internal sealed class SqlStringPathExpression : SqlPathExpression
  {
    private SqlStringPathExpression(SqlPathExpression parentPath, SqlStringLiteral value)
      : base(parentPath)
    {
      this.Value = value ?? throw new ArgumentNullException(nameof (value));
    }

    public SqlStringLiteral Value { get; }

    public static SqlStringPathExpression Create(
      SqlPathExpression parentPath,
      SqlStringLiteral value)
    {
      return new SqlStringPathExpression(parentPath, value);
    }

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);

    public override void Accept(SqlPathExpressionVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlPathExpressionVisitor<TResult> visitor) => visitor.Visit(this);
  }
}
