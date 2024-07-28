// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.DiagnosticLogUtil
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal static class DiagnosticLogUtil
  {
    private const string c_guardParam = "!guard!";

    internal static DiagnosticIdentity CreateDiagnosticIdentity(Guid vsid, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      DiagnosticIdentity diagnosticIdentity = new DiagnosticIdentity()
      {
        Id = vsid
      };
      if (identity != null)
        diagnosticIdentity.DisplayName = identity.DisplayName;
      return diagnosticIdentity;
    }

    internal static ProcessingDiagnosticIdentity ToProcessingDiagnosticIdentity(
      this Microsoft.VisualStudio.Services.Identity.Identity identity,
      NotificationSubscriber subscriber = null,
      string message = null)
    {
      ProcessingDiagnosticIdentity diagnosticIdentity = new ProcessingDiagnosticIdentity();
      diagnosticIdentity.Id = identity.Id;
      diagnosticIdentity.DisplayName = identity.DisplayName;
      diagnosticIdentity.IsActive = identity.IsActive;
      diagnosticIdentity.IsGroup = identity.IsContainer;
      diagnosticIdentity.DeliveryPreference = subscriber?.DeliveryPreference.ToString();
      diagnosticIdentity.Message = message;
      return diagnosticIdentity;
    }

    internal static void Append(
      this List<NotificationDiagnosticLogMessage> messages,
      TraceLevel level,
      string message)
    {
      NotificationDiagnosticLogMessage diagnosticLogMessage = new NotificationDiagnosticLogMessage()
      {
        Level = (int) level,
        Time = DateTime.UtcNow.TimeOfDay,
        Message = message
      };
      messages.Add(diagnosticLogMessage);
    }

    internal static void AddMessage(
      this NotificationDiagnosticLog log,
      TraceLevel level,
      string message)
    {
      switch (level)
      {
        case TraceLevel.Error:
          ++log.Errors;
          break;
        case TraceLevel.Warning:
          ++log.Warnings;
          break;
      }
      log.Messages.Append(level, message);
    }

    internal static ProcessedEvent GetProcessedEvent(
      this SubscriptionTraceEventProcessingLogInternal log,
      TeamFoundationEvent ev)
    {
      int key = ev != null ? ev.Id : -1;
      ProcessedEvent pe;
      if (!log.ProcessedEvents.TryGetValue(key, out pe) && log.SubscriptionTracing.Enabled)
      {
        pe = new ProcessedEvent() { EventId = key };
        ev?.Actors.ForEach((Action<EventActor>) (e => pe.Actors.Add(e)));
        log.ProcessedEvents[key] = pe;
        log.SubscriptionTracing.StartEvaluationTrace();
        if (ev != null)
        {
          pe.ArtifactUri = ev?.ArtifactUri;
          if (ev.AllowedChannels != null)
            pe.AllowedChannels = ev.AllowedChannels.Count > 0 ? string.Join<string>(",", (IEnumerable<string>) ev.AllowedChannels) : "none";
        }
      }
      return pe;
    }

    internal static SubscriptionEvaluation GetSubscriptionEvaluation(
      this ProcessedEvent pe,
      Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      Guid guid = identity != null ? identity.Id : Guid.Empty;
      SubscriptionEvaluation subscriptionEvaluation;
      if (!pe.Evaluations.TryGetValue(guid, out subscriptionEvaluation))
      {
        subscriptionEvaluation = new SubscriptionEvaluation()
        {
          User = DiagnosticLogUtil.CreateDiagnosticIdentity(guid, identity)
        };
        pe.Evaluations[guid] = subscriptionEvaluation;
      }
      return subscriptionEvaluation;
    }

    internal static void AddClause(this SubscriptionEvaluation se, bool result, string clause)
    {
      SubscriptionEvaluationClause evaluationClause = new SubscriptionEvaluationClause()
      {
        Order = se.Clauses.Count<SubscriptionEvaluationClause>(),
        Clause = clause,
        Result = result
      };
      se.Clauses.Add(evaluationClause);
    }

    internal static void AddClause(
      this ProcessedEvent pe,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      bool result,
      string clause)
    {
      SubscriptionEvaluation subscriptionEvaluation = pe.GetSubscriptionEvaluation(identity);
      if (subscriptionEvaluation == null)
        return;
      subscriptionEvaluation.AddClause(result, clause);
    }

    internal static void AddClause(
      this SubscriptionTraceEventProcessingLogInternal log,
      TeamFoundationEvent ev,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      bool result,
      string clause)
    {
      ProcessedEvent processedEvent = log.GetProcessedEvent(ev);
      if (processedEvent == null)
        return;
      processedEvent.AddClause(identity, result, clause);
    }

    internal static void EvaluationTraceClause(
      this ISubscriptionObjectTrace tracer,
      bool result,
      string clauseFormat,
      object arg1,
      EvaluationContext evaluationContext,
      [CallerMemberName] string methodName = null)
    {
      if (!evaluationContext.Subscription.SubscriptionTracingEnabled)
        return;
      tracer.EvaluationTraceClause(result, string.Format(clauseFormat, arg1, (object) "!guard!", (object) "!guard!"), evaluationContext, methodName);
    }

    internal static void EvaluationTraceClause(
      this ISubscriptionObjectTrace tracer,
      bool result,
      string clauseFormat,
      object arg1,
      object arg2,
      EvaluationContext evaluationContext,
      [CallerMemberName] string methodName = null)
    {
      if (!evaluationContext.Subscription.SubscriptionTracingEnabled)
        return;
      tracer.EvaluationTraceClause((result ? 1 : 0) != 0, string.Format(clauseFormat, arg1, arg2, (object) "!guard!", (object) "!guard!"), evaluationContext, methodName);
    }

    internal static void EvaluationTraceClause(
      this ISubscriptionObjectTrace tracer,
      bool result,
      string clauseFormat,
      object arg1,
      object arg2,
      object arg3,
      EvaluationContext evaluationContext,
      [CallerMemberName] string methodName = null)
    {
      if (!evaluationContext.Subscription.SubscriptionTracingEnabled)
        return;
      tracer.EvaluationTraceClause((result ? 1 : 0) != 0, string.Format(clauseFormat, arg1, arg2, arg3, (object) "!guard!", (object) "!guard!"), evaluationContext, methodName);
    }

    internal static void EvaluationTraceClause(
      this ISubscriptionObjectTrace tracer,
      bool result,
      string clauseFormat,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      EvaluationContext evaluationContext,
      [CallerMemberName] string methodName = null)
    {
      if (!evaluationContext.Subscription.SubscriptionTracingEnabled)
        return;
      tracer.EvaluationTraceClause((result ? 1 : 0) != 0, string.Format(clauseFormat, arg1, arg2, arg3, arg4, (object) "!guard!", (object) "!guard!"), evaluationContext, methodName);
    }

    internal static void EvaluationTraceClause(
      this ISubscriptionObjectTrace tracer,
      bool result,
      string clauseFormat,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      EvaluationContext evaluationContext,
      [CallerMemberName] string methodName = null)
    {
      if (!evaluationContext.Subscription.SubscriptionTracingEnabled)
        return;
      tracer.EvaluationTraceClause((result ? 1 : 0) != 0, string.Format(clauseFormat, arg1, arg2, arg3, arg4, arg5, (object) "!guard!", (object) "!guard!"), evaluationContext, methodName);
    }

    internal static void EvaluationTraceNoisy(
      this ISubscriptionObjectTrace tracer,
      string format,
      object arg1,
      Subscription subscription,
      [CallerMemberName] string methodName = null)
    {
      if (!tracer.NoisyEnabled || !subscription.SubscriptionTracingEnabled)
        return;
      tracer.EvaluationTraceNoisy(string.Format(format, arg1, (object) "!guard!", (object) "!guard!"), subscription, methodName);
    }

    internal static void EvaluationTraceNoisy(
      this ISubscriptionObjectTrace tracer,
      string format,
      object arg1,
      object arg2,
      Subscription subscription,
      [CallerMemberName] string methodName = null)
    {
      if (!tracer.NoisyEnabled || !subscription.SubscriptionTracingEnabled)
        return;
      tracer.EvaluationTraceNoisy(string.Format(format, arg1, arg2, (object) "!guard!", (object) "!guard!"), subscription, methodName);
    }

    internal static void EvaluationTraceNoisy(
      this ISubscriptionObjectTrace tracer,
      string format,
      object arg1,
      object arg2,
      object arg3,
      Subscription subscription,
      [CallerMemberName] string methodName = null)
    {
      if (!tracer.NoisyEnabled || !subscription.SubscriptionTracingEnabled)
        return;
      tracer.EvaluationTraceNoisy(string.Format(format, arg1, arg2, arg3, (object) "!guard!", (object) "!guard!"), subscription, methodName);
    }

    internal static void EvaluationTraceNoisy(
      this ISubscriptionObjectTrace tracer,
      string format,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      Subscription subscription,
      [CallerMemberName] string methodName = null)
    {
      if (!tracer.NoisyEnabled || !subscription.SubscriptionTracingEnabled)
        return;
      tracer.EvaluationTraceNoisy(string.Format(format, arg1, arg2, arg3, arg4, (object) "!guard!", (object) "!guard!"), subscription, methodName);
    }

    internal static void EvaluationTraceNoisy(
      this ISubscriptionObjectTrace tracer,
      string format,
      object arg1,
      object arg2,
      object arg3,
      object arg4,
      object arg5,
      Subscription subscription,
      [CallerMemberName] string methodName = null)
    {
      if (!tracer.NoisyEnabled || !subscription.SubscriptionTracingEnabled)
        return;
      tracer.EvaluationTraceNoisy(string.Format(format, arg1, arg2, arg3, arg4, arg5, (object) "!guard!", (object) "!guard!"), subscription, methodName);
    }
  }
}
