// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.SubscriptionDiagnosticsExtensions
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public static class SubscriptionDiagnosticsExtensions
  {
    private static readonly TimeSpan s_defaultResultTracingTimeSpan = new TimeSpan(1, 0, 0);
    private static readonly int s_defaultResultTracingLimit = 100;
    private static readonly TimeSpan s_defaultEvaluationTracingTimeSpan = new TimeSpan(1, 0, 0);
    private static readonly int s_defaultEvaluationTracingLimit = 25;
    private static readonly TimeSpan s_defaultDeliveryTracingTimeSpan = new TimeSpan(1, 0, 0);
    private static readonly int s_defaultDeliveryTracingLimit = 25;

    public static void RefreshExpirations(this SubscriptionDiagnostics diagnostics)
    {
      SubscriptionDiagnosticsExtensions.CheckExpiration(diagnostics.DeliveryResults);
      SubscriptionDiagnosticsExtensions.CheckExpiration(diagnostics.EvaluationTracing);
      SubscriptionDiagnosticsExtensions.CheckExpiration(diagnostics.DeliveryTracing);
    }

    public static bool DeliveryResultAdded(this SubscriptionTracing tracing) => SubscriptionDiagnosticsExtensions.NextTraceAllowed(tracing);

    public static bool StartEvaluationTrace(this SubscriptionTracing tracing) => SubscriptionDiagnosticsExtensions.NextTraceAllowed(tracing);

    public static bool StartDeliveryTrace(this SubscriptionTracing tracing) => SubscriptionDiagnosticsExtensions.NextTraceAllowed(tracing);

    private static bool NextTraceAllowed(SubscriptionTracing tracing)
    {
      bool flag = false;
      if (tracing != null)
      {
        SubscriptionDiagnosticsExtensions.CheckExpiration(tracing);
        if (tracing.Enabled)
        {
          ++tracing.TracedEntries;
          flag = true;
        }
      }
      return flag;
    }

    public static SubscriptionDiagnostics Clone(this SubscriptionDiagnostics diagnostics)
    {
      SubscriptionDiagnostics subscriptionDiagnostics = new SubscriptionDiagnostics();
      SubscriptionTracing deliveryResults = diagnostics.DeliveryResults;
      subscriptionDiagnostics.DeliveryResults = deliveryResults != null ? deliveryResults.Clone() : (SubscriptionTracing) null;
      return subscriptionDiagnostics;
    }

    public static void UpdateDiagnostics(
      this Subscription subscription,
      UpdateSubscripitonDiagnosticsParameters updateParameters)
    {
      if (updateParameters.DeliveryResults != null && !subscription.TraceDeliveryResultsAllowed)
        throw new ArgumentException(CoreRes.InvalidUpdateTracingParametersDeliveryResultsNotSupported());
      subscription.Diagnostics.UpdateParameters(updateParameters);
    }

    public static void UpdateParameters(
      this SubscriptionDiagnostics diagnostics,
      UpdateSubscripitonDiagnosticsParameters updateParameters)
    {
      diagnostics.DeliveryResults = SubscriptionDiagnosticsExtensions.UpdateTracingParameters(diagnostics.DeliveryResults, updateParameters.DeliveryResults, SubscriptionDiagnosticsExtensions.s_defaultResultTracingTimeSpan, SubscriptionDiagnosticsExtensions.s_defaultResultTracingLimit);
      diagnostics.EvaluationTracing = SubscriptionDiagnosticsExtensions.UpdateTracingParameters(diagnostics.EvaluationTracing, updateParameters.EvaluationTracing, SubscriptionDiagnosticsExtensions.s_defaultEvaluationTracingTimeSpan, SubscriptionDiagnosticsExtensions.s_defaultEvaluationTracingLimit);
      diagnostics.DeliveryTracing = SubscriptionDiagnosticsExtensions.UpdateTracingParameters(diagnostics.DeliveryTracing, updateParameters.DeliveryTracing, SubscriptionDiagnosticsExtensions.s_defaultDeliveryTracingTimeSpan, SubscriptionDiagnosticsExtensions.s_defaultDeliveryTracingLimit);
    }

    private static SubscriptionTracing UpdateTracingParameters(
      SubscriptionTracing currentTracing,
      UpdateSubscripitonTracingParameters parameters,
      TimeSpan duration,
      int count)
    {
      SubscriptionTracing tracing = currentTracing;
      if (parameters != null)
      {
        if (parameters.Enabled)
          tracing = SubscriptionDiagnosticsExtensions.Start(duration, count);
        else
          SubscriptionDiagnosticsExtensions.Stop(tracing);
      }
      return tracing;
    }

    public static SubscriptionTracing Clone(this SubscriptionTracing tracing) => new SubscriptionTracing()
    {
      StartDate = tracing.StartDate,
      EndDate = tracing.EndDate,
      MaxTracedEntries = tracing.MaxTracedEntries,
      TracedEntries = tracing.TracedEntries
    };

    private static SubscriptionTracing Start(TimeSpan duration, int count)
    {
      DateTime utcNow = DateTime.UtcNow;
      return new SubscriptionTracing()
      {
        StartDate = utcNow,
        EndDate = utcNow + duration,
        MaxTracedEntries = count,
        TracedEntries = 0,
        Enabled = true
      };
    }

    private static void Stop(SubscriptionTracing tracing)
    {
      if (tracing == null || !tracing.Enabled)
        return;
      tracing.EndDate = DateTime.UtcNow;
      tracing.Enabled = false;
    }

    public static void CheckExpiration(SubscriptionTracing tracing)
    {
      if (tracing == null || !tracing.Enabled)
        return;
      bool flag = (tracing.EndDate == DateTime.MinValue ? 1 : (tracing.EndDate < DateTime.UtcNow ? 1 : 0)) == 0 && tracing.TracedEntries < tracing.MaxTracedEntries;
      tracing.Enabled = flag;
    }

    public static void UpdateFromPrevious(
      this SubscriptionDiagnostics currentDiagnostics,
      SubscriptionDiagnostics previousDiagnostics)
    {
      currentDiagnostics.DeliveryResults.UpdateFromPrevious(previousDiagnostics.DeliveryResults);
      currentDiagnostics.EvaluationTracing.UpdateFromPrevious(previousDiagnostics.EvaluationTracing);
      currentDiagnostics.DeliveryTracing.UpdateFromPrevious(previousDiagnostics.DeliveryTracing);
    }

    public static void UpdateFromPrevious(
      this SubscriptionTracing currentTracing,
      SubscriptionTracing previousTracing)
    {
      if (currentTracing == null || previousTracing == null || !(currentTracing.StartDate == previousTracing.StartDate))
        return;
      currentTracing.TracedEntries = previousTracing.TracedEntries;
      SubscriptionDiagnosticsExtensions.CheckExpiration(currentTracing);
    }

    public static bool IsNewer(this SubscriptionTracing tracing, SubscriptionTracing other) => tracing.StartDate < other.StartDate;

    public enum SubscriptionTracingSetting
    {
      DeliveryResults,
      EvaluationTracing,
      DeliveryTracing,
    }
  }
}
