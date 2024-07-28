// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.DocumentQueryEvaluator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal static class DocumentQueryEvaluator
  {
    private const string SQLMethod = "AsSQL";

    public static SqlQuerySpec Evaluate(
      Expression expression,
      CosmosLinqSerializerOptions linqSerializerOptions = null,
      IDictionary<object, string> parameters = null)
    {
      switch (expression.NodeType)
      {
        case ExpressionType.Call:
          return DocumentQueryEvaluator.HandleMethodCallExpression((MethodCallExpression) expression, parameters, linqSerializerOptions);
        case ExpressionType.Constant:
          return DocumentQueryEvaluator.HandleEmptyQuery((ConstantExpression) expression);
        default:
          throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ClientResources.BadQuery_InvalidExpression, (object) expression.ToString()));
      }
    }

    public static bool IsTransformExpression(Expression expression) => expression is MethodCallExpression methodCallExpression && methodCallExpression.Method.DeclaringType == typeof (DocumentQueryable) && methodCallExpression.Method.Name == "AsSQL";

    private static SqlQuerySpec HandleEmptyQuery(ConstantExpression expression)
    {
      Type type = expression.Value != null ? expression.Value.GetType() : throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ClientResources.BadQuery_InvalidExpression, (object) expression.ToString()));
      if (!type.IsGenericType || !(type.GetGenericTypeDefinition() == typeof (DocumentQuery<bool>).GetGenericTypeDefinition()) && !(type.GetGenericTypeDefinition() == typeof (CosmosLinqQuery<bool>).GetGenericTypeDefinition()))
        throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ClientResources.BadQuery_InvalidExpression, (object) expression.ToString()));
      return (SqlQuerySpec) null;
    }

    private static SqlQuerySpec HandleMethodCallExpression(
      MethodCallExpression expression,
      IDictionary<object, string> parameters,
      CosmosLinqSerializerOptions linqSerializerOptions = null)
    {
      if (!DocumentQueryEvaluator.IsTransformExpression((Expression) expression))
        return SqlTranslator.TranslateQuery((Expression) expression, linqSerializerOptions, parameters);
      if (string.Compare(expression.Method.Name, "AsSQL", StringComparison.Ordinal) == 0)
        return DocumentQueryEvaluator.HandleAsSqlTransformExpression(expression);
      throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ClientResources.BadQuery_InvalidExpression, (object) expression.ToString()));
    }

    private static SqlQuerySpec HandleAsSqlTransformExpression(MethodCallExpression expression)
    {
      Expression body = expression.Arguments[1];
      if (body.NodeType == ExpressionType.Lambda)
        return DocumentQueryEvaluator.GetSqlQuerySpec(((LambdaExpression) body).Compile().DynamicInvoke((object[]) null));
      return body.NodeType == ExpressionType.Constant ? DocumentQueryEvaluator.GetSqlQuerySpec(((ConstantExpression) body).Value) : DocumentQueryEvaluator.GetSqlQuerySpec(Expression.Lambda(body).Compile().DynamicInvoke((object[]) null));
    }

    private static SqlQuerySpec GetSqlQuerySpec(object value)
    {
      if (value == null)
        throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ClientResources.BadQuery_InvalidExpression, value));
      if (value.GetType() == typeof (SqlQuerySpec))
        return (SqlQuerySpec) value;
      return value.GetType() == typeof (string) ? new SqlQuerySpec((string) value) : throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ClientResources.BadQuery_InvalidExpression, value));
    }
  }
}
