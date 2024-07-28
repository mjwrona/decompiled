// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SqlObjects.SqlScalarExpression
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects.Visitors;

namespace Microsoft.Azure.Cosmos.SqlObjects
{
  internal abstract class SqlScalarExpression : SqlObject
  {
    public abstract void Accept(SqlScalarExpressionVisitor visitor);

    public abstract TResult Accept<TResult>(SqlScalarExpressionVisitor<TResult> visitor);

    public abstract TResult Accept<T, TResult>(
      SqlScalarExpressionVisitor<T, TResult> visitor,
      T input);
  }
}
