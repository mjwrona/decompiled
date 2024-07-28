// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ODataPathExtensions
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.UriParser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public static class ODataPathExtensions
  {
    public static Expression ApplySegment(this Expression source, KeySegment segment)
    {
      ParameterExpression parameterExpression = Expression.Parameter(source.Type.GetGenericArguments()[0]);
      Expression expression1 = (Expression) null;
      foreach (KeyValuePair<string, object> key in segment.Keys)
      {
        Expression expression2 = (Expression) Expression.Property((Expression) parameterExpression, key.Key);
        if (EdmTypeUtils.IsNullable(expression2.Type))
          expression2 = (Expression) Expression.Convert(expression2, Nullable.GetUnderlyingType(expression2.Type));
        BinaryExpression right = Expression.Equal(expression2, (Expression) Expression.Constant(key.Value));
        expression1 = expression1 == null ? (Expression) right : (Expression) Expression.AndAlso(expression1, (Expression) right);
      }
      LambdaExpression predicate = Expression.Lambda(expression1, parameterExpression);
      return ExpressionHelpers.FirstOrDefault(source, (Expression) predicate);
    }

    public static Expression ApplySegment(this Expression source, NavigationPropertySegment segment)
    {
      source = (Expression) Expression.Property(source, segment.Identifier);
      if (typeof (IEnumerable).IsAssignableFrom(source.Type))
        source = ExpressionHelpers.AsQueryable(source);
      return source;
    }
  }
}
