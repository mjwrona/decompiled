// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Diagnostics.TraceUtility
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common.Diagnostics;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.ServiceModel.Channels;

namespace Microsoft.Azure.NotificationHubs.Diagnostics
{
  internal class TraceUtility
  {
    private static bool shouldPropagateActivity;

    public static bool PropagateUserActivity => TraceUtility.ShouldPropagateActivity && TraceUtility.PropagateUserActivityCore;

    internal static bool ShouldPropagateActivity => TraceUtility.shouldPropagateActivity;

    private static bool PropagateUserActivityCore => false;

    internal static void AddAmbientActivityToMessage(Message message)
    {
    }

    internal static Exception ThrowHelperError(Exception exception, Message message) => exception;

    internal static Exception ThrowHelperError(Exception exception, Guid activityId, object source) => exception;

    internal static Exception ThrowHelperWarning(Exception exception, Message message) => exception;

    internal static void TraceEvent(TraceEventType severity, TraceCode traceCode, object source) => TraceUtility.TraceEvent(severity, traceCode, (TraceRecord) null, source, (Exception) null);

    internal static void TraceEvent(
      TraceEventType severity,
      TraceCode traceCode,
      object source,
      Exception exception)
    {
      TraceUtility.TraceEvent(severity, traceCode, (TraceRecord) null, source, exception);
    }

    internal static void TraceEvent(TraceEventType severity, TraceCode traceCode, Message message)
    {
      if (message == null)
        TraceUtility.TraceEvent(severity, traceCode, (object) null, (Exception) null);
      else
        TraceUtility.TraceEvent(severity, traceCode, (object) message, message);
    }

    internal static void TraceEvent(
      TraceEventType severity,
      TraceCode traceCode,
      object source,
      Message message)
    {
      Guid empty = Guid.Empty;
      if (!DiagnosticUtility.ShouldTrace(severity))
        return;
      DiagnosticUtility.DiagnosticTrace.TraceEvent(severity, traceCode, TraceUtility.Description(traceCode), (TraceRecord) new MessageTraceRecord(message), (Exception) null, empty, (object) message);
    }

    internal static void TraceEvent(
      TraceEventType severity,
      TraceCode traceCode,
      Exception exception,
      Message message)
    {
      Guid empty = Guid.Empty;
      if (!DiagnosticUtility.ShouldTrace(severity))
        return;
      DiagnosticUtility.DiagnosticTrace.TraceEvent(severity, traceCode, TraceUtility.Description(traceCode), (TraceRecord) new MessageTraceRecord(message), exception, empty, (object) null);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void TraceEvent(
      TraceEventType severity,
      TraceCode traceCode,
      TraceRecord extendedData,
      object source,
      Exception exception)
    {
      if (!DiagnosticUtility.ShouldTrace(severity))
        return;
      DiagnosticUtility.DiagnosticTrace.TraceEvent(severity, traceCode, TraceUtility.Description(traceCode), extendedData, exception, source);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static void TraceEvent(
      TraceEventType severity,
      TraceCode traceCode,
      TraceRecord extendedData,
      object source,
      Exception exception,
      Message message)
    {
      Guid empty = Guid.Empty;
      if (!DiagnosticUtility.ShouldTrace(severity))
        return;
      DiagnosticUtility.DiagnosticTrace.TraceEvent(severity, traceCode, TraceUtility.Description(traceCode), extendedData, exception, empty, source);
    }

    private static string Description(TraceCode traceCode) => Microsoft.Azure.NotificationHubs.SR.GetString("TraceCode" + DiagnosticTrace.CodeToString(traceCode));

    internal class TracingAsyncCallbackState
    {
      private object innerState;
      private Guid activityId;

      internal TracingAsyncCallbackState(object innerState)
      {
        this.innerState = innerState;
        this.activityId = DiagnosticTrace.ActivityId;
      }

      internal object InnerState => this.innerState;

      internal Guid ActivityId => this.activityId;
    }
  }
}
