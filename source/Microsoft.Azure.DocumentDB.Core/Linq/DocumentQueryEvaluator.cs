// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.DocumentQueryEvaluator
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Globalization;
using System.Linq.Expressions;

namespace Microsoft.Azure.Documents.Linq
{
  internal static class DocumentQueryEvaluator
  {
    private const string SQLMethod = "AsSQL";

    public static SqlQuerySpec Evaluate(Expression expression)
    {
      switch (expression.NodeType)
      {
        case ExpressionType.Call:
          return DocumentQueryEvaluator.HandleMethodCallExpression((MethodCallExpression) expression);
        case ExpressionType.Constant:
          return DocumentQueryEvaluator.HandleEmptyQuery((ConstantExpression) expression);
        default:
          throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ClientResources.BadQuery_InvalidExpression, (object) expression.ToString()));
      }
    }

    public static bool IsTransformExpression(Expression expression) => expression is MethodCallExpression methodCallExpression && (object) methodCallExpression.Method.DeclaringType == (object) typeof (DocumentQueryable) && methodCallExpression.Method.Name == "AsSQL";

    private static SqlQuerySpec HandleEmptyQuery(ConstantExpression expression)
    {
      Type type = expression.Value != null ? expression.Value.GetType() : throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ClientResources.BadQuery_InvalidExpression, (object) expression.ToString()));
      if (!type.IsGenericType() || (object) type.GetGenericTypeDefinition() != (object) typeof (DocumentQuery<bool>).GetGenericTypeDefinition())
        throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ClientResources.BadQuery_InvalidExpression, (object) expression.ToString()));
      return (SqlQuerySpec) null;
    }

    private static SqlQuerySpec HandleMethodCallExpression(MethodCallExpression expression)
    {
      if (!DocumentQueryEvaluator.IsTransformExpression((Expression) expression))
        return SqlTranslator.TranslateQuery((Expression) expression);
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
      if ((object) value.GetType() == (object) typeof (SqlQuerySpec))
        return (SqlQuerySpec) value;
      return (object) value.GetType() == (object) typeof (string) ? new SqlQuerySpec((string) value) : throw new DocumentQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, ClientResources.BadQuery_InvalidExpression, value));
    }
  }
}
