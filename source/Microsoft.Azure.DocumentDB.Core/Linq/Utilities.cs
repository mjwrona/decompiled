// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.Utilities
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;

namespace Microsoft.Azure.Documents.Linq
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
