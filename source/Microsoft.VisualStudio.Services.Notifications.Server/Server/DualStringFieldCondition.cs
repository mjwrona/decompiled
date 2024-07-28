// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.DualStringFieldCondition
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class DualStringFieldCondition : Condition
  {
    private StringFieldCondition m_legacyCondition;
    private Condition m_optimizedCondition;
    private bool m_haveLogged;
    internal DualStringFieldCondition.ConditionMismatch OnConditionMismatch;

    public DualStringFieldCondition(
      StringFieldCondition legacyCondition,
      Condition optimizedCondition)
    {
      this.m_legacyCondition = legacyCondition;
      this.m_optimizedCondition = optimizedCondition;
      this.OnConditionMismatch = new DualStringFieldCondition.ConditionMismatch(this.LogErrorOnMismatch);
    }

    public override bool Evaluate(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext)
    {
      bool legacyResult = this.m_legacyCondition.Evaluate(requestContext, evaluationContext);
      bool optimizedResult = this.m_optimizedCondition.Evaluate(requestContext, evaluationContext);
      if (legacyResult != optimizedResult)
      {
        DualStringFieldCondition.ConditionMismatch conditionMismatch = this.OnConditionMismatch;
        if (conditionMismatch != null)
          conditionMismatch(requestContext, evaluationContext, legacyResult, optimizedResult);
      }
      return legacyResult;
    }

    public override void Write(IndentedTextWriter writer) => this.m_legacyCondition.Write(writer);

    private void LogErrorOnMismatch(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext,
      bool legacyResult,
      bool optimizedResult)
    {
      if (this.m_haveLogged)
        return;
      this.m_haveLogged = true;
      try
      {
        if (this.m_optimizedCondition is StringFieldCondition optimizedCondition)
          requestContext.Trace(1002014, TraceLevel.Error, "Notifications", "Condition", string.Format("Condition eval mismatch for subs: {0} ev: {1} legacy: {2} optimized: {3} expr: {4} evalTarget: {5}", (object) evaluationContext.Subscription.ID, (object) evaluationContext.Event.Id, (object) legacyResult, (object) optimizedResult, (object) optimizedCondition.ToString(), (object) optimizedCondition.Target.EvaluateToken(evaluationContext.MacrosLookup)));
        else
          requestContext.TraceSerializedConditionally(1002014, TraceLevel.Error, "Notifications", "Condition", string.Format("Condition eval mismatch for subs: {0} ev: {1} legacy: {2} optimized: {3} expr: {4}", (object) evaluationContext.Subscription.ID, (object) evaluationContext.Event.Id, (object) legacyResult, (object) optimizedResult, (object) this.m_optimizedCondition.ToString()));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1002014, "Notifications", "Condition", ex);
      }
    }

    internal delegate void ConditionMismatch(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext,
      bool legacyResult,
      bool optimizedResult);
  }
}
