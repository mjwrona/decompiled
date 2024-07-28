// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.AndFilterCondition
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class AndFilterCondition : LogicalOperationFilterCondition
  {
    public AndFilterCondition(
      IFilterCondition left,
      IFilterCondition right,
      string logicalOperator = null)
      : base(left, right, logicalOperator)
    {
    }

    public override bool Evaluate(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext)
    {
      return this.Right.Evaluate(requestContext, evaluationContext) && this.Left.Evaluate(requestContext, evaluationContext);
    }
  }
}
