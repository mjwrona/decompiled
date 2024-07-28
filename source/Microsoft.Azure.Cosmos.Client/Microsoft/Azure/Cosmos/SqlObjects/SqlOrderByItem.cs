// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.SqlOrderByItem
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects.Visitors;
using System;

namespace Microsoft.Azure.Cosmos.SqlObjects
{
  internal sealed class SqlOrderByItem : SqlObject
  {
    private SqlOrderByItem(SqlScalarExpression expression, bool isDescending)
    {
      this.Expression = expression ?? throw new ArgumentNullException(nameof (expression));
      this.IsDescending = isDescending;
    }

    public SqlScalarExpression Expression { get; }

    public bool IsDescending { get; }

    public static SqlOrderByItem Create(SqlScalarExpression expression, bool isDescending) => new SqlOrderByItem(expression, isDescending);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);
  }
}
