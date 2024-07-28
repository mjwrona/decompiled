// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.ServiceSharedTraceExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public static class ServiceSharedTraceExtensions
  {
    public static IDisposable Enter(
      this IVssRequestContext context,
      EnterLeaveTracePoint tracepoint,
      [CallerMemberName] string methodName = null)
    {
      return tracepoint.Enter(context, methodName);
    }

    public static void TraceInfo(
      this IVssRequestContext context,
      SingleLocationTracePoint tracepoint,
      string messageFormat,
      params object[] args)
    {
      ServiceSharedTraceExtensions.Trace(context, tracepoint, TraceLevel.Info, messageFormat, args);
    }

    public static void TraceError(
      this IVssRequestContext context,
      SingleLocationTracePoint tracepoint,
      string message)
    {
      ServiceSharedTraceExtensions.Trace(context, tracepoint, TraceLevel.Error, message);
    }

    public static void TraceWarning(
      this IVssRequestContext context,
      SingleLocationTracePoint tracepoint,
      string messageFormat,
      params object[] args)
    {
      ServiceSharedTraceExtensions.Trace(context, tracepoint, TraceLevel.Warning, messageFormat, args);
    }

    public static void TraceAlways(
      this IVssRequestContext context,
      SingleLocationTracePoint tracepoint,
      string messageFormat,
      params object[] args)
    {
      ServiceSharedTraceExtensions.TraceAlways(context, tracepoint, TraceLevel.Info, messageFormat, args);
    }

    public static void TraceException(
      this IVssRequestContext context,
      SingleLocationTracePoint tracepoint,
      Exception exception)
    {
      if (context != null)
        context.TraceException(tracepoint.TracePoint, TraceLevel.Error, tracepoint.Area, tracepoint.Layer, exception);
      else
        TeamFoundationTracingService.TraceRaw(tracepoint.TracePoint, TraceLevel.Error, tracepoint.Area, tracepoint.Layer, exception.Message);
    }

    private static void Trace(
      IVssRequestContext context,
      SingleLocationTracePoint tracepoint,
      TraceLevel level,
      string messageFormat,
      params object[] args)
    {
      if (context != null)
        VssRequestContextExtensions.Trace(context, tracepoint.TracePoint, level, tracepoint.Area, tracepoint.Layer, messageFormat, args);
      else
        TeamFoundationTracingService.TraceRaw(tracepoint.TracePoint, level, tracepoint.Area, tracepoint.Layer, messageFormat, args);
    }

    private static void TraceAlways(
      IVssRequestContext context,
      SingleLocationTracePoint tracepoint,
      TraceLevel level,
      string messageFormat,
      params object[] args)
    {
      if (context != null)
        context.TraceAlways(tracepoint.TracePoint, level, tracepoint.Area, tracepoint.Layer, messageFormat, args);
      else
        TeamFoundationTracingService.TraceRaw(tracepoint.TracePoint, level, tracepoint.Area, tracepoint.Layer, messageFormat, args);
    }

    public static Task TraceInfoAsync(
      this VssRequestPump.Processor processor,
      SingleLocationTracePoint tracepoint,
      string messageFormat,
      params object[] args)
    {
      return processor.ExecuteWorkAsync((Action<IVssRequestContext>) (context => context.TraceInfo(tracepoint, messageFormat, args)));
    }

    public static Task TraceErrorAsync(
      this VssRequestPump.Processor processor,
      SingleLocationTracePoint tracepoint,
      string message)
    {
      return processor.ExecuteWorkAsync((Action<IVssRequestContext>) (context => context.TraceError(tracepoint, message)));
    }

    public static Task TraceWarningAsync(
      this VssRequestPump.Processor processor,
      SingleLocationTracePoint tracepoint,
      string messageFormat,
      params object[] args)
    {
      return processor.ExecuteWorkAsync((Action<IVssRequestContext>) (context => context.TraceWarning(tracepoint, messageFormat, args)));
    }

    public static Task TraceAlwaysAsync(
      this VssRequestPump.Processor processor,
      SingleLocationTracePoint tracepoint,
      string messageFormat,
      params object[] args)
    {
      return processor.ExecuteWorkAsync((Action<IVssRequestContext>) (context => context.TraceAlways(tracepoint, messageFormat, args)));
    }

    public static Task TraceExceptionAsync(
      this VssRequestPump.Processor processor,
      SingleLocationTracePoint tracepoint,
      Exception exception)
    {
      return processor.ExecuteWorkAsync((Action<IVssRequestContext>) (context => context.TraceException(tracepoint, exception)));
    }
  }
}
