// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Queryable.ProjectionQueryOptionExpression
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Table.Queryable
{
  internal class ProjectionQueryOptionExpression : QueryOptionExpression
  {
    private readonly LambdaExpression lambda;
    private readonly List<string> paths;
    internal static readonly LambdaExpression DefaultLambda = Expression.Lambda((Expression) Expression.Constant((object) 0), (ParameterExpression[]) null);

    internal ProjectionQueryOptionExpression(
      Type type,
      LambdaExpression lambda,
      List<string> paths)
      : base((ExpressionType) 10008, type)
    {
      this.lambda = lambda;
      this.paths = paths;
    }

    internal LambdaExpression Selector => this.lambda;

    internal List<string> Paths => this.paths;
  }
}
