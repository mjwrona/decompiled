// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ManyToManyCountRewrite
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal static class ManyToManyCountRewrite
  {
    public static IQueryable Rewrite(Type baseElement, IQueryable query)
    {
      if (baseElement != typeof (BoardLocation))
        return query;
      Expression expression = new ManyToManyCountRewrite.ManyToManyCountExpressionVisitor().Visit(query.Expression);
      return query.Provider.CreateQuery(expression);
    }

    private class ManyToManyCountExpressionVisitor : ExpressionVisitor
    {
      private static MethodInfo _asQueryableMethod = ((IEnumerable<MethodInfo>) typeof (Queryable).GetMethods()).Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name == "AsQueryable")).Where<MethodInfo>((Func<MethodInfo, bool>) (m => ((IEnumerable<ParameterInfo>) m.GetParameters()).First<ParameterInfo>().ParameterType.IsGenericType)).Single<MethodInfo>().MakeGenericMethod(typeof (WorkItem));
      private static MethodInfo _sumMethod = ((IEnumerable<MethodInfo>) typeof (Queryable).GetMethods()).Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.Name == "Sum")).Where<MethodInfo>((Func<MethodInfo, bool>) (m => ((IEnumerable<ParameterInfo>) m.GetParameters()).Count<ParameterInfo>() == 2)).Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.ReturnType == typeof (long))).Single<MethodInfo>().MakeGenericMethod(typeof (WorkItem));

      protected override Expression VisitMethodCall(MethodCallExpression node)
      {
        if (!(node.Method.Name == "LongCount") || !(((IEnumerable<Type>) node.Arguments[0].Type.GenericTypeArguments).Last<Type>() == typeof (WorkItem)))
          return base.VisitMethodCall(node);
        MethodCallExpression methodCallExpression = Expression.Call((Expression) null, ManyToManyCountRewrite.ManyToManyCountExpressionVisitor._asQueryableMethod, node.Arguments[0]);
        ParameterExpression parameterExpression = Expression.Parameter(typeof (WorkItem), "w");
        LambdaExpression lambdaExpression = Expression.Lambda((Expression) Expression.Property((Expression) parameterExpression, "Count"), parameterExpression);
        return (Expression) Expression.Call((Expression) null, ManyToManyCountRewrite.ManyToManyCountExpressionVisitor._sumMethod, (Expression) methodCallExpression, (Expression) lambdaExpression);
      }
    }
  }
}
