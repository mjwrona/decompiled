// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.Logger
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.Table
{
  internal static class Logger
  {
    private static TraceSource TraceSourceInternal = new TraceSource("Microsoft.Azure.Cosmos.Table");

    static Logger()
    {
      Trace.UseGlobalLock = false;
      SourceSwitch sourceSwitch = new SourceSwitch("ClientSwitch", SourceLevels.Off.ToString());
      Logger.TraceSourceInternal.Switch = sourceSwitch;
    }

    public static void SetupTableSdkTraceSource(
      TraceListener[] traceListeners,
      SourceSwitch sourceSwitch)
    {
      Logger.TraceSourceInternal.Switch = sourceSwitch;
      Logger.TraceSourceInternal.Listeners.Remove("Default");
      if (traceListeners == null)
        return;
      Logger.TraceSourceInternal.Listeners.AddRange(traceListeners);
    }

    internal static void LogError(
      OperationContext operationContext,
      string format,
      params object[] args)
    {
      if ((Logger.TraceSourceInternal.Switch.Level & (SourceLevels) 2) == SourceLevels.Off)
        return;
      Logger.TraceSourceInternal.TraceEvent(TraceEventType.Error, 0, Logger.FormatLine(operationContext, format, args));
    }

    internal static void LogWarning(
      OperationContext operationContext,
      string format,
      params object[] args)
    {
      if ((Logger.TraceSourceInternal.Switch.Level & (SourceLevels) 4) == SourceLevels.Off)
        return;
      Logger.TraceSourceInternal.TraceEvent(TraceEventType.Warning, 0, Logger.FormatLine(operationContext, format, args));
    }

    internal static void LogInformational(
      OperationContext operationContext,
      string format,
      params object[] args)
    {
      if ((Logger.TraceSourceInternal.Switch.Level & (SourceLevels) 8) == SourceLevels.Off)
        return;
      Logger.TraceSourceInternal.TraceEvent(TraceEventType.Information, 0, Logger.FormatLine(operationContext, format, args));
    }

    internal static void LogVerbose(
      OperationContext operationContext,
      string format,
      params object[] args)
    {
      if ((Logger.TraceSourceInternal.Switch.Level & (SourceLevels) 16) == SourceLevels.Off)
        return;
      Logger.TraceSourceInternal.TraceEvent(TraceEventType.Verbose, 0, Logger.FormatLine(operationContext, format, args));
    }

    private static string FormatLine(
      OperationContext operationContext,
      string format,
      object[] args)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: {1}", operationContext == null ? (object) "*" : (object) operationContext.ClientRequestID, args == null ? (object) format : (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args).Replace('\n', '.'));
    }
  }
}
