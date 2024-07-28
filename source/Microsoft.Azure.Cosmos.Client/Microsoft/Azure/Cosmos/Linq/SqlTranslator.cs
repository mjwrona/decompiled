// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.SqlTranslator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core;
using Microsoft.Azure.Cosmos.SqlObjects;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal static class SqlTranslator
  {
    internal static string TranslateExpression(
      Expression inputExpression,
      CosmosLinqSerializerOptions linqSerializerOptions = null)
    {
      TranslationContext context = new TranslationContext(linqSerializerOptions);
      inputExpression = ConstantEvaluator.PartialEval(inputExpression);
      return ExpressionToSql.VisitNonSubqueryScalarExpression(inputExpression, context).ToString();
    }

    internal static string TranslateExpressionOld(
      Expression inputExpression,
      CosmosLinqSerializerOptions linqSerializerOptions = null)
    {
      TranslationContext context = new TranslationContext(linqSerializerOptions);
      inputExpression = ConstantFolding.Fold(inputExpression);
      return ExpressionToSql.VisitNonSubqueryScalarExpression(inputExpression, context).ToString();
    }

    internal static SqlQuerySpec TranslateQuery(
      Expression inputExpression,
      CosmosLinqSerializerOptions linqSerializerOptions,
      IDictionary<object, string> parameters)
    {
      inputExpression = ConstantEvaluator.PartialEval(inputExpression);
      SqlQuery sqlQuery = ExpressionToSql.TranslateQuery(inputExpression, parameters, linqSerializerOptions);
      SqlParameterCollection parameters1 = new SqlParameterCollection();
      if (parameters != null && parameters.Count > 0)
      {
        foreach (KeyValuePair<object, string> parameter in (IEnumerable<KeyValuePair<object, string>>) parameters)
          parameters1.Add(new Microsoft.Azure.Cosmos.Query.Core.SqlParameter(parameter.Value, parameter.Key));
      }
      return new SqlQuerySpec(sqlQuery.ToString(), parameters1);
    }
  }
}
