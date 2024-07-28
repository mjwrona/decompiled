// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.ExpressionSimplifier
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal abstract class ExpressionSimplifier
  {
    private static ConcurrentDictionary<Type, ExpressionSimplifier> cached = new ConcurrentDictionary<Type, ExpressionSimplifier>();

    public abstract object EvalBoxed(Expression expr);

    public static object Evaluate(Expression expr)
    {
      ExpressionSimplifier instance;
      if (ExpressionSimplifier.cached.ContainsKey(expr.Type))
      {
        instance = ExpressionSimplifier.cached[expr.Type];
      }
      else
      {
        instance = (ExpressionSimplifier) Activator.CreateInstance(typeof (ExpressionSimplifier<>).MakeGenericType(expr.Type));
        ExpressionSimplifier.cached.TryAdd(expr.Type, instance);
      }
      return instance.EvalBoxed(expr);
    }

    public static Expression EvaluateToExpression(Expression expr) => (Expression) Expression.Constant(ExpressionSimplifier.Evaluate(expr), expr.Type);
  }
}
