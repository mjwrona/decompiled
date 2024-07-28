// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.ExpressionSimplifier
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace Microsoft.Azure.Documents.Linq
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
