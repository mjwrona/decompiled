// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.PredictRewrite
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query.Expressions;
using Microsoft.VisualStudio.Services.Analytics.Model;
using Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal static class PredictRewrite
  {
    public static IQueryable Rewrite(IQueryable query, AnalyticsComponent component)
    {
      Expression expression1 = query.Expression;
      Expression expression2 = new PredictRewrite.PredictExpressionVisitor(component).Visit(expression1);
      return query.Provider.CreateQuery(expression2);
    }

    private class PredictExpressionVisitor : ExpressionVisitor
    {
      private AnalyticsComponent _component;
      private static readonly Dictionary<PredictModel, PredictRewrite.PredictFunctionDefinition> _predictFunctions = new Dictionary<PredictModel, PredictRewrite.PredictFunctionDefinition>()
      {
        {
          PredictModel.WorkItemCompletedTime,
          new PredictRewrite.PredictFunctionDefinition()
          {
            FunctionName = "PredictWorkItemCompletedTime",
            Parameters = new string[5]
            {
              "PartitionId",
              "WorkItemId",
              "Revision",
              "State",
              "WorkItemType"
            }
          }
        }
      };

      public PredictExpressionVisitor(AnalyticsComponent component) => this._component = component;

      protected override Expression VisitMethodCall(MethodCallExpression node)
      {
        if (node.Method.Name == "Predict")
        {
          object parameterizedConstant = ExpressionBinderBase.ExtractParameterizedConstant(node.Arguments[0]);
          PredictRewrite.PredictFunctionDefinition functionDefinition;
          if (parameterizedConstant != null && parameterizedConstant is PredictModel key && PredictRewrite.PredictExpressionVisitor._predictFunctions.TryGetValue(key, out functionDefinition))
            return (Expression) Expression.Call((Expression) null, SqlFunctions.GeCustomMethods(functionDefinition.FunctionName).First<MethodInfo>(), (IEnumerable<Expression>) ((IEnumerable<string>) functionDefinition.Parameters).Select<string, MemberExpression>((Func<string, MemberExpression>) (f => Expression.Property(node.Object, f))));
        }
        else if (node.Method.Name == "PredictTags" && this._component != null)
          return (Expression) Expression.Call((Expression) Expression.Constant((object) this._component.Context), SqlFunctions.GeCustomMethods(typeof (AnalyticsContext), "PredictTags").First<MethodInfo>(), (Expression) Expression.Property(node.Object, "PartitionId"), (Expression) Expression.Property(node.Object, "WorkItemRevisionSK"));
        return base.VisitMethodCall(node);
      }
    }

    private class PredictFunctionDefinition
    {
      public string FunctionName { get; set; }

      public string[] Parameters { get; set; }
    }
  }
}
