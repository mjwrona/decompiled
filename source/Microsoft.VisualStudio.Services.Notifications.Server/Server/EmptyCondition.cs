// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EmptyCondition
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [Serializable]
  public class EmptyCondition : FieldCondition
  {
    public EmptyCondition() => this.FieldName = (Token) new ConstantToken();

    public override bool Evaluate(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext)
    {
      ++evaluationContext.EvaluationsCount;
      evaluationContext.Tracer.EvaluationTraceClause(true, "empty clause", evaluationContext, nameof (Evaluate));
      return true;
    }

    public override bool EvaluateOneValue(
      IVssRequestContext requestContext,
      EvaluationContext evaluationContext,
      object fieldValue)
    {
      ++evaluationContext.EvaluationsCount;
      evaluationContext.Tracer.EvaluationTraceClause(true, "empty({0})", (object) fieldValue?.ToString(), evaluationContext, nameof (EvaluateOneValue));
      return true;
    }

    public override bool Equals(object obj) => obj is EmptyCondition;

    public override int GetHashCode() => 0;

    public override string GetOperandString() => string.Empty;

    public override string ToString() => string.Empty;
  }
}
