// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.Utilities
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal static class Utilities
  {
    public static string SqlQuoteString(string toQuote)
    {
      toQuote = toQuote.Replace("'", "\\'").Replace("\"", "\\\"");
      toQuote = "\"" + toQuote + "\"";
      return toQuote;
    }

    public static LambdaExpression GetLambda(Expression expr)
    {
      while (expr.NodeType == ExpressionType.Quote)
        expr = ((UnaryExpression) expr).Operand;
      return expr.NodeType == ExpressionType.Lambda ? expr as LambdaExpression : throw new ArgumentException("Expected a lambda expression");
    }

    public static ParameterExpression NewParameter(
      string prefix,
      Type type,
      HashSet<ParameterExpression> inScope)
    {
      int num = 0;
      ParameterExpression parameterExpression;
      while (true)
      {
        string name = prefix + num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        parameterExpression = Expression.Parameter(type, name);
        if (inScope.Any<ParameterExpression>((Func<ParameterExpression, bool>) (p => p.Name.Equals(name))))
          ++num;
        else
          break;
      }
      inScope.Add(parameterExpression);
      return parameterExpression;
    }
  }
}
