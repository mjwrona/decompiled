// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Sql.SqlObjectCreateScalarExpression
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents.Sql
{
  internal sealed class SqlObjectCreateScalarExpression : SqlScalarExpression
  {
    private SqlObjectCreateScalarExpression(IEnumerable<SqlObjectProperty> properties)
      : base(SqlObjectKind.ObjectCreateScalarExpression)
    {
      if (properties == null)
        throw new ArgumentNullException("properties must not be null.");
      foreach (SqlObjectProperty property in properties)
      {
        if (property == null)
          throw new ArgumentException("properties must not have null items.");
      }
      this.Properties = (IEnumerable<SqlObjectProperty>) new List<SqlObjectProperty>(properties);
    }

    public IEnumerable<SqlObjectProperty> Properties { get; }

    public static SqlObjectCreateScalarExpression Create(params SqlObjectProperty[] properties) => new SqlObjectCreateScalarExpression((IEnumerable<SqlObjectProperty>) properties);

    public static SqlObjectCreateScalarExpression Create(IEnumerable<SqlObjectProperty> properties) => new SqlObjectCreateScalarExpression(properties);

    public override void Accept(SqlObjectVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlObjectVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(SqlObjectVisitor<T, TResult> visitor, T input) => visitor.Visit(this, input);

    public override void Accept(SqlScalarExpressionVisitor visitor) => visitor.Visit(this);

    public override TResult Accept<TResult>(SqlScalarExpressionVisitor<TResult> visitor) => visitor.Visit(this);

    public override TResult Accept<T, TResult>(
      SqlScalarExpressionVisitor<T, TResult> visitor,
      T input)
    {
      return visitor.Visit(this, input);
    }
  }
}
