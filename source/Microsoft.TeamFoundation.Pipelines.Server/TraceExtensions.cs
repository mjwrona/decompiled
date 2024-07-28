// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.TraceExtensions
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public static class TraceExtensions
  {
    public static void TraceInfo(
      this IVssRequestContext context,
      int tracepoint,
      string layer,
      string messageFormat,
      params object[] args)
    {
      TraceExtensions.Trace(context, tracepoint, TraceLevel.Info, layer, messageFormat, args);
    }

    public static void TraceError(
      this IVssRequestContext context,
      int tracepoint,
      string layer,
      string message)
    {
      TraceExtensions.Trace(context, tracepoint, TraceLevel.Error, layer, message);
    }

    public static void TraceWarning(
      this IVssRequestContext context,
      int tracepoint,
      string layer,
      string messageFormat,
      params object[] args)
    {
      TraceExtensions.Trace(context, tracepoint, TraceLevel.Warning, layer, messageFormat, args);
    }

    public static void TraceException(
      this IVssRequestContext context,
      int tracepoint,
      string layer,
      Exception exception)
    {
      if (context != null)
        context.TraceException(tracepoint, TraceLevel.Error, TracePoints.Area, layer, exception);
      else
        TeamFoundationTracingService.TraceRaw(tracepoint, TraceLevel.Error, TracePoints.Area, layer, exception.Message);
    }

    private static void Trace(
      IVssRequestContext context,
      int tracepoint,
      TraceLevel level,
      string layer,
      string messageFormat,
      params object[] args)
    {
      if (context != null)
        VssRequestContextExtensions.Trace(context, tracepoint, level, TracePoints.Area, layer, messageFormat, args);
      else
        TeamFoundationTracingService.TraceRaw(tracepoint, level, TracePoints.Area, layer, messageFormat, args);
    }
  }
}
