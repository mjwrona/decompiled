// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EvaluationContextEvaluate
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class EvaluationContextEvaluate : IDisposable
  {
    private EvaluationContext m_evaluationContext;

    public EvaluationContextEvaluate(
      EvaluationContext evaluationContext,
      Subscription subscription,
      TeamFoundationEvent ev,
      Microsoft.VisualStudio.Services.Identity.Identity user,
      Dictionary<string, string> macrosLookup = null)
    {
      this.m_evaluationContext = evaluationContext;
      this.m_evaluationContext.Subscription = subscription;
      this.m_evaluationContext.Event = ev;
      this.m_evaluationContext.User = user;
      this.m_evaluationContext.MacrosLookup = macrosLookup;
    }

    public void Dispose()
    {
      this.m_evaluationContext.Subscription = (Subscription) null;
      this.m_evaluationContext.Event = (TeamFoundationEvent) null;
      this.m_evaluationContext.User = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      this.m_evaluationContext.MacrosLookup = (Dictionary<string, string>) null;
    }
  }
}
