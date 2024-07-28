// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.SqlTranslator
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System.Linq.Expressions;

namespace Microsoft.Azure.Documents.Linq
{
  internal static class SqlTranslator
  {
    internal static string TranslateExpression(Expression inputExpression)
    {
      TranslationContext context = new TranslationContext();
      inputExpression = ConstantEvaluator.PartialEval(inputExpression);
      return ExpressionToSql.VisitNonSubqueryScalarExpression(inputExpression, context).ToString();
    }

    internal static string TranslateExpressionOld(Expression inputExpression)
    {
      TranslationContext context = new TranslationContext();
      inputExpression = ConstantFolding.Fold(inputExpression);
      return ExpressionToSql.VisitNonSubqueryScalarExpression(inputExpression, context).ToString();
    }

    internal static SqlQuerySpec TranslateQuery(Expression inputExpression)
    {
      inputExpression = ConstantEvaluator.PartialEval(inputExpression);
      return new SqlQuerySpec(ExpressionToSql.TranslateQuery(inputExpression).ToString());
    }
  }
}
