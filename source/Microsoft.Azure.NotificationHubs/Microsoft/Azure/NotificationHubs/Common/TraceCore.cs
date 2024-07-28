// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Common.TraceCore
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using Microsoft.Azure.NotificationHubs.Common.Diagnostics;
using System;
using System.Diagnostics.Eventing;
using System.Globalization;
using System.Resources;
using System.Security;

namespace Microsoft.Azure.NotificationHubs.Common
{
  internal class TraceCore
  {
    private static ResourceManager resourceManager;
    private static CultureInfo resourceCulture;
    [SecurityCritical]
    private static EventDescriptor[] eventDescriptors;

    private TraceCore()
    {
    }

    private static ResourceManager ResourceManager
    {
      get
      {
        if (TraceCore.resourceManager == null)
          TraceCore.resourceManager = new ResourceManager("Microsoft.Notifications.Messaging.TraceCore", typeof (TraceCore).Assembly);
        return TraceCore.resourceManager;
      }
    }

    internal static CultureInfo Culture
    {
      get => TraceCore.resourceCulture;
      set => TraceCore.resourceCulture = value;
    }

    internal static bool HandledExceptionIsEnabled(DiagnosticTrace trace) => trace.ShouldTrace(TraceEventLevel.Informational) || TraceCore.IsEtwEventEnabled(trace, 1);

    internal static void HandledException(DiagnosticTrace trace, Exception exception)
    {
      TracePayload serializedPayload = DiagnosticTrace.GetSerializedPayload((object) null, (TraceRecord) null, exception);
      if (TraceCore.IsEtwEventEnabled(trace, 1))
        TraceCore.WriteEtwEvent(trace, 1, serializedPayload.SerializedException, serializedPayload.AppDomainFriendlyName);
      if (!trace.ShouldTraceToTraceSource(TraceEventLevel.Informational))
        return;
      string description = string.Format((IFormatProvider) TraceCore.Culture, TraceCore.ResourceManager.GetString(nameof (HandledException), TraceCore.Culture));
      TraceCore.WriteTraceSource(trace, 1, description, serializedPayload);
    }

    internal static void UnhandledException(DiagnosticTrace trace, Exception exception)
    {
      TracePayload serializedPayload = DiagnosticTrace.GetSerializedPayload((object) null, (TraceRecord) null, exception);
      if (TraceCore.IsEtwEventEnabled(trace, 4))
        TraceCore.WriteEtwEvent(trace, 4, serializedPayload.SerializedException, serializedPayload.AppDomainFriendlyName);
      if (!trace.ShouldTraceToTraceSource(TraceEventLevel.Error))
        return;
      string description = string.Format((IFormatProvider) TraceCore.Culture, TraceCore.ResourceManager.GetString(nameof (UnhandledException), TraceCore.Culture));
      TraceCore.WriteTraceSource(trace, 4, description, serializedPayload);
    }

    internal static bool TraceCodeEventLogCriticalIsEnabled(DiagnosticTrace trace) => trace.ShouldTrace(TraceEventLevel.Critical) || TraceCore.IsEtwEventEnabled(trace, 6);

    internal static void TraceCodeEventLogCritical(DiagnosticTrace trace, TraceRecord traceRecord)
    {
      TracePayload serializedPayload = DiagnosticTrace.GetSerializedPayload((object) null, traceRecord, (Exception) null);
      if (TraceCore.IsEtwEventEnabled(trace, 6))
        TraceCore.WriteEtwEvent(trace, 6, serializedPayload.ExtendedData, serializedPayload.AppDomainFriendlyName);
      if (!trace.ShouldTraceToTraceSource(TraceEventLevel.Critical))
        return;
      string description = string.Format((IFormatProvider) TraceCore.Culture, TraceCore.ResourceManager.GetString(nameof (TraceCodeEventLogCritical), TraceCore.Culture));
      TraceCore.WriteTraceSource(trace, 6, description, serializedPayload);
    }

    internal static bool TraceCodeEventLogErrorIsEnabled(DiagnosticTrace trace) => trace.ShouldTrace(TraceEventLevel.Error) || TraceCore.IsEtwEventEnabled(trace, 7);

    internal static void TraceCodeEventLogError(DiagnosticTrace trace, TraceRecord traceRecord)
    {
      TracePayload serializedPayload = DiagnosticTrace.GetSerializedPayload((object) null, traceRecord, (Exception) null);
      if (TraceCore.IsEtwEventEnabled(trace, 7))
        TraceCore.WriteEtwEvent(trace, 7, serializedPayload.ExtendedData, serializedPayload.AppDomainFriendlyName);
      if (!trace.ShouldTraceToTraceSource(TraceEventLevel.Error))
        return;
      string description = string.Format((IFormatProvider) TraceCore.Culture, TraceCore.ResourceManager.GetString(nameof (TraceCodeEventLogError), TraceCore.Culture));
      TraceCore.WriteTraceSource(trace, 7, description, serializedPayload);
    }

    internal static bool TraceCodeEventLogInfoIsEnabled(DiagnosticTrace trace) => trace.ShouldTrace(TraceEventLevel.Informational) || TraceCore.IsEtwEventEnabled(trace, 8);

    internal static void TraceCodeEventLogInfo(DiagnosticTrace trace, TraceRecord traceRecord)
    {
      TracePayload serializedPayload = DiagnosticTrace.GetSerializedPayload((object) null, traceRecord, (Exception) null);
      if (TraceCore.IsEtwEventEnabled(trace, 8))
        TraceCore.WriteEtwEvent(trace, 8, serializedPayload.ExtendedData, serializedPayload.AppDomainFriendlyName);
      if (!trace.ShouldTraceToTraceSource(TraceEventLevel.Informational))
        return;
      string description = string.Format((IFormatProvider) TraceCore.Culture, TraceCore.ResourceManager.GetString(nameof (TraceCodeEventLogInfo), TraceCore.Culture));
      TraceCore.WriteTraceSource(trace, 8, description, serializedPayload);
    }

    internal static bool TraceCodeEventLogVerboseIsEnabled(DiagnosticTrace trace) => trace.ShouldTrace(TraceEventLevel.Verbose) || TraceCore.IsEtwEventEnabled(trace, 9);

    internal static void TraceCodeEventLogVerbose(DiagnosticTrace trace, TraceRecord traceRecord)
    {
      TracePayload serializedPayload = DiagnosticTrace.GetSerializedPayload((object) null, traceRecord, (Exception) null);
      if (TraceCore.IsEtwEventEnabled(trace, 9))
        TraceCore.WriteEtwEvent(trace, 9, serializedPayload.ExtendedData, serializedPayload.AppDomainFriendlyName);
      if (!trace.ShouldTraceToTraceSource(TraceEventLevel.Verbose))
        return;
      string description = string.Format((IFormatProvider) TraceCore.Culture, TraceCore.ResourceManager.GetString(nameof (TraceCodeEventLogVerbose), TraceCore.Culture));
      TraceCore.WriteTraceSource(trace, 9, description, serializedPayload);
    }

    internal static bool TraceCodeEventLogWarningIsEnabled(DiagnosticTrace trace) => trace.ShouldTrace(TraceEventLevel.Warning) || TraceCore.IsEtwEventEnabled(trace, 10);

    internal static void TraceCodeEventLogWarning(DiagnosticTrace trace, TraceRecord traceRecord)
    {
      TracePayload serializedPayload = DiagnosticTrace.GetSerializedPayload((object) null, traceRecord, (Exception) null);
      if (TraceCore.IsEtwEventEnabled(trace, 10))
        TraceCore.WriteEtwEvent(trace, 10, serializedPayload.ExtendedData, serializedPayload.AppDomainFriendlyName);
      if (!trace.ShouldTraceToTraceSource(TraceEventLevel.Warning))
        return;
      string description = string.Format((IFormatProvider) TraceCore.Culture, TraceCore.ResourceManager.GetString(nameof (TraceCodeEventLogWarning), TraceCore.Culture));
      TraceCore.WriteTraceSource(trace, 10, description, serializedPayload);
    }

    private static void EnsureEventDescriptors()
    {
      if (TraceCore.eventDescriptors != null)
        return;
      TraceCore.eventDescriptors = new EventDescriptor[12]
      {
        new EventDescriptor(30200, (byte) 0, (byte) 19, (byte) 4, (byte) 0, 0, 1152921504606846976L),
        new EventDescriptor(30201, (byte) 0, (byte) 19, (byte) 4, (byte) 0, 0, 1152921504606846976L),
        new EventDescriptor(30202, (byte) 0, (byte) 19, (byte) 2, (byte) 0, 0, 1152921504606846976L),
        new EventDescriptor(30203, (byte) 0, (byte) 19, (byte) 2, (byte) 0, 0, 1152921504606846976L),
        new EventDescriptor(30204, (byte) 0, (byte) 19, (byte) 2, (byte) 0, 0, 1152921504606846976L),
        new EventDescriptor(30205, (byte) 0, (byte) 19, (byte) 3, (byte) 0, 0, 1152921504606846976L),
        new EventDescriptor(30206, (byte) 0, (byte) 19, (byte) 1, (byte) 0, 0, 1152921504606846976L),
        new EventDescriptor(30207, (byte) 0, (byte) 19, (byte) 2, (byte) 0, 0, 1152921504606846976L),
        new EventDescriptor(30208, (byte) 0, (byte) 19, (byte) 4, (byte) 0, 0, 1152921504606846976L),
        new EventDescriptor(30209, (byte) 0, (byte) 19, (byte) 5, (byte) 0, 0, 1152921504606846976L),
        new EventDescriptor(30210, (byte) 0, (byte) 19, (byte) 3, (byte) 0, 0, 1152921504606846976L),
        new EventDescriptor(30211, (byte) 0, (byte) 19, (byte) 3, (byte) 0, 0, 1152921504606846976L)
      };
    }

    private static bool IsEtwEventEnabled(DiagnosticTrace trace, int eventIndex)
    {
      TraceCore.EnsureEventDescriptors();
      return trace.IsEtwEventEnabled(ref TraceCore.eventDescriptors[eventIndex]);
    }

    private static bool WriteEtwEvent(
      DiagnosticTrace trace,
      int eventIndex,
      string eventParam0,
      string eventParam1)
    {
      TraceCore.EnsureEventDescriptors();
      return trace.EtwProvider.WriteEvent(ref TraceCore.eventDescriptors[eventIndex], (object) eventParam0, (object) eventParam1);
    }

    private static void WriteTraceSource(
      DiagnosticTrace trace,
      int eventIndex,
      string description,
      TracePayload payload)
    {
      TraceCore.EnsureEventDescriptors();
      trace.WriteTraceSource(ref TraceCore.eventDescriptors[eventIndex], description, payload);
    }
  }
}
