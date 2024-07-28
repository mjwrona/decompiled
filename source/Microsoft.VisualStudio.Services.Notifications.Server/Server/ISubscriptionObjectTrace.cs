// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.ISubscriptionObjectTrace
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public interface ISubscriptionObjectTrace
  {
    bool NoisyEnabled { get; }

    void EvaluationTraceClause(
      bool result,
      string clause,
      EvaluationContext evaluationContext,
      [CallerMemberName] string methodName = null);

    void EvaluationTraceNoisy(string message, Subscription subscription, [CallerMemberName] string methodName = null);

    void LogSubscriptionError(string message, Subscription subscription);

    void LogSubscriptionWarning(string message, Subscription subscription);

    void TraceSubscriptionException(int tracepoint, Exception exception, Subscription subscription);

    void TraceSubscriptionException(
      int tracepoint,
      Exception exception,
      string message,
      Subscription subscription);
  }
}
