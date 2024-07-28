// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.Logger
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.Azure.Storage.Core
{
  internal static class Logger
  {
    private static TraceSource traceSource = new TraceSource("Microsoft.Azure.Storage");
    private static volatile bool isClosed = false;
    private const string TraceFormat = "{0}: {1}";

    static Logger()
    {
      AppDomain.CurrentDomain.DomainUnload += new EventHandler(Logger.AppDomainUnloadEvent);
      AppDomain.CurrentDomain.ProcessExit += new EventHandler(Logger.ProcessExitEvent);
    }

    private static void Close()
    {
      Logger.isClosed = true;
      TraceSource traceSource = Logger.traceSource;
      if (traceSource == null)
        return;
      Logger.traceSource = (TraceSource) null;
      traceSource.Close();
    }

    private static void ProcessExitEvent(object sender, EventArgs e) => Logger.Close();

    private static void AppDomainUnloadEvent(object sender, EventArgs e) => Logger.Close();

    internal static void LogError(
      OperationContext operationContext,
      string format,
      params object[] args)
    {
      if (Logger.isClosed || !Logger.traceSource.Switch.ShouldTrace(TraceEventType.Error))
        return;
      Logger.ShouldLog(LogLevel.Error, operationContext);
    }

    internal static void LogWarning(
      OperationContext operationContext,
      string format,
      params object[] args)
    {
      if (Logger.isClosed || !Logger.traceSource.Switch.ShouldTrace(TraceEventType.Warning))
        return;
      Logger.ShouldLog(LogLevel.Warning, operationContext);
    }

    internal static void LogInformational(
      OperationContext operationContext,
      string format,
      params object[] args)
    {
      if (Logger.isClosed || !Logger.traceSource.Switch.ShouldTrace(TraceEventType.Information))
        return;
      Logger.ShouldLog(LogLevel.Informational, operationContext);
    }

    internal static void LogVerbose(
      OperationContext operationContext,
      string format,
      params object[] args)
    {
      if (Logger.isClosed || !Logger.traceSource.Switch.ShouldTrace(TraceEventType.Verbose))
        return;
      Logger.ShouldLog(LogLevel.Verbose, operationContext);
    }

    private static string FormatLine(
      OperationContext operationContext,
      string format,
      object[] args)
    {
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: {1}", operationContext == null ? (object) "*" : (object) operationContext.ClientRequestID, args == null ? (object) format : (object) string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args).Replace('\n', '.'));
    }

    private static bool ShouldLog(LogLevel level, OperationContext operationContext) => operationContext == null || level <= operationContext.LogLevel;
  }
}
