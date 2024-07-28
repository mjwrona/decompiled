// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.FilterCondition
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class FilterCondition : IFilterCondition
  {
    public FilterCondition(ExpressionFilterClause filterClause)
    {
      this.FilterClause = filterClause;
      this.LogicalOperator = filterClause.LogicalOperator;
    }

    public string LogicalOperator { get; set; }

    public ExpressionFilterClause FilterClause { get; private set; }

    public bool Evaluate(IVssRequestContext requestContext, EvaluationContext evaluationContext)
    {
      byte rawOperator = SubscriptionFilterOperators.GetRawOperator(this.FilterClause.Operator);
      NotificationEventField fieldByName = evaluationContext.Subscription.GetFieldProvider(requestContext).GetFieldByName(this.FilterClause.FieldName, true);
      FieldResult fieldResult = evaluationContext.Event.GetFieldResult(fieldByName.Path);
      bool result = fieldResult.CompareResult(rawOperator, evaluationContext, (object) this.FilterClause.Value, fieldByName.FieldType.SubscriptionFieldType);
      fieldResult.TraceEvaluationString(evaluationContext, result, rawOperator, (object) this.FilterClause.Value);
      return result;
    }
  }
}
