// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.SubscriptionObjectTracer
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class SubscriptionObjectTracer : ISubscriptionObjectTrace
  {
    private IVssRequestContext m_requestContext;
    private string m_area;
    private string m_layer;
    private bool m_noisy;

    public SubscriptionObjectTracer(IVssRequestContext requestContext, string layer = "SubscriptionObjectTracer", string area = "Notifications")
    {
      this.m_requestContext = requestContext;
      this.m_layer = layer;
      this.m_area = area;
      this.m_noisy = this.m_requestContext.IsTracing(1002699, TraceLevel.Verbose, this.m_area, this.m_layer);
    }

    public NotificationJobDiagnosticLog JobLog { get; set; }

    public bool NoisyEnabled => this.m_noisy;

    public void EvaluationTraceClause(
      bool result,
      string clause,
      EvaluationContext evaluationContext,
      [CallerMemberName] string methodName = null)
    {
      Subscription subscription = evaluationContext.Subscription;
      if ((subscription != null ? (subscription.SubscriptionTracingEnabled ? 1 : 0) : 0) == 0)
        return;
      evaluationContext.Subscription.TraceLog.AddClause(evaluationContext.Event, evaluationContext.User, result, clause);
    }

    public void EvaluationTraceNoisy(string message, Subscription subscription, [CallerMemberName] string methodName = null)
    {
      if (!this.m_noisy || !subscription.SubscriptionTracingEnabled)
        return;
      this.EvaluationTraceNoisyImpl(subscription, methodName, message);
    }

    public void LogSubscriptionError(string message, Subscription subscription) => this.LogSubscriptionMessage(TraceLevel.Error, message, subscription);

    public void LogSubscriptionWarning(string message, Subscription subscription) => this.LogSubscriptionMessage(TraceLevel.Warning, message, subscription);

    private void LogSubscriptionMessage(
      TraceLevel level,
      string message,
      Subscription subscription)
    {
      SubscriptionTraceEventProcessingLogInternal traceLog = subscription.TraceLog;
      if (traceLog != null)
        traceLog.AddMessage(level, message);
      if (this.JobLog != null)
        this.JobLog.AddMessage(level, "Subs " + subscription.SubscriptionId + ": " + message);
      else
        this.m_requestContext.Trace(1002698, level, this.m_area, this.m_layer, message);
    }

    public void TraceSubscriptionException(
      int tracepoint,
      Exception exception,
      Subscription subscription)
    {
      this.TraceSubscriptionException(tracepoint, exception, string.Empty, subscription);
    }

    public void TraceSubscriptionException(
      int tracepoint,
      Exception exception,
      string message,
      Subscription subscription)
    {
      bool flag = string.IsNullOrEmpty(message);
      if (subscription != null)
      {
        if (subscription.TraceLog != null)
          subscription.TraceLog.AddMessage(TraceLevel.Error, flag ? string.Format("TP:{0}: {1}", (object) tracepoint, (object) ExceptionUtil.FormatException(exception)) : string.Format("TP:{0}: {1}\r\n{2}", (object) tracepoint, (object) message, (object) ExceptionUtil.FormatException(exception)));
        if (this.JobLog != null)
        {
          NotificationJobDiagnosticLog jobLog = this.JobLog;
          string message1;
          if (!flag)
            message1 = string.Format("TP:{0}, Subs:{1}: {2}\r\n{3}", (object) tracepoint, (object) subscription.SubscriptionId, (object) message, (object) ExceptionUtil.FormatException(exception));
          else
            message1 = string.Format("TP:{0}, Subs:{1}: {2}", (object) tracepoint, (object) subscription.SubscriptionId, (object) ExceptionUtil.FormatException(exception));
          jobLog.AddMessage(TraceLevel.Error, message1);
        }
      }
      else if (this.JobLog != null)
        this.JobLog.AddMessage(TraceLevel.Error, flag ? string.Format("TP:{0}, {1}", (object) tracepoint, (object) ExceptionUtil.FormatException(exception)) : string.Format("TP:{0}, {1}\r\n{2}", (object) tracepoint, (object) message, (object) ExceptionUtil.FormatException(exception)));
      if (flag)
        this.m_requestContext.TraceExceptionMsg(1002698, this.m_area, this.m_layer, exception, message);
      else
        this.m_requestContext.TraceException(1002698, this.m_area, this.m_layer, exception);
    }

    private void EvaluationTraceNoisyImpl(
      Subscription subscription,
      string methodName,
      string message)
    {
      subscription.TraceLog.AddMessage(TraceLevel.Verbose, methodName + ":" + message);
    }

    private void EvaluationTraceNoisyImpl(
      Subscription subscription,
      string methodName,
      string format,
      params object[] args)
    {
      this.EvaluationTraceNoisyImpl(subscription, methodName, string.Format(format, args));
    }
  }
}
