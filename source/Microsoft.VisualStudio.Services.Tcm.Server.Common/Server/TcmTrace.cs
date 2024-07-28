// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TcmTrace
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public static class TcmTrace
  {
    private static string m_traceArea = "TestManagement";

    public static string TraceArea
    {
      get => TcmTrace.m_traceArea;
      set => TcmTrace.m_traceArea = value;
    }

    public static void TraceVerbose(
      this TestManagementRequestContext context,
      string layer,
      string messageFormat,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(context.RequestContext, 0, TraceLevel.Verbose, TcmTrace.m_traceArea, layer, messageFormat, args);
    }

    public static void TraceVerbose(
      this IVssRequestContext context,
      string layer,
      string messageFormat,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(context, 0, TraceLevel.Verbose, TcmTrace.m_traceArea, layer, messageFormat, args);
    }

    public static void TraceInfo(
      this TestManagementRequestContext context,
      string layer,
      string messageFormat,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(context.RequestContext, 0, TraceLevel.Info, TcmTrace.m_traceArea, layer, messageFormat, args);
    }

    public static void TraceInfo(
      this IVssRequestContext context,
      string layer,
      string messageFormat,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(context, 0, TraceLevel.Info, TcmTrace.m_traceArea, layer, messageFormat, args);
    }

    public static void TraceWarning(
      this TestManagementRequestContext context,
      string layer,
      string messageFormat,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(context.RequestContext, 0, TraceLevel.Warning, TcmTrace.m_traceArea, layer, messageFormat, args);
    }

    public static void TraceError(
      this TestManagementRequestContext context,
      string layer,
      string messageFormat,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(context.RequestContext, 0, TraceLevel.Error, TcmTrace.m_traceArea, layer, messageFormat, args);
    }

    public static void TraceError(
      this IVssRequestContext context,
      string layer,
      string messageFormat,
      params object[] args)
    {
      VssRequestContextExtensions.Trace(context, 0, TraceLevel.Error, TcmTrace.m_traceArea, layer, messageFormat, args);
    }

    public static void TraceException(
      this TestManagementRequestContext context,
      string layer,
      Exception ex)
    {
      context.RequestContext.TraceException(0, TraceLevel.Error, TcmTrace.m_traceArea, layer, ex);
    }

    public static void TraceException(this IVssRequestContext context, string layer, Exception ex) => context.TraceException(0, TraceLevel.Error, TcmTrace.m_traceArea, layer, ex);

    public static bool IsTracing(
      this TestManagementRequestContext context,
      TraceLevel traceLevel,
      string layer)
    {
      return context.RequestContext.IsTracing(0, traceLevel, TcmTrace.m_traceArea, layer);
    }

    public static void TraceEnter(
      this TestManagementRequestContext context,
      string layer,
      string methodName)
    {
      context.RequestContext.TraceEnter(0, TcmTrace.m_traceArea, layer, methodName);
    }

    public static void TraceEnter(this IVssRequestContext context, string layer, string methodName) => context.TraceEnter(0, TcmTrace.m_traceArea, layer, methodName);

    public static void TraceLeave(
      this TestManagementRequestContext context,
      string layer,
      string methodName)
    {
      context.RequestContext.TraceLeave(0, TcmTrace.m_traceArea, layer, methodName);
    }

    public static void TraceLeave(this IVssRequestContext context, string layer, string methodName) => context.TraceLeave(0, TcmTrace.m_traceArea, layer, methodName);

    public static void TraceAndDebugAssert(
      this TestManagementRequestContext context,
      string layer,
      bool assertCondition,
      string message)
    {
      if (assertCondition)
        return;
      context.TraceError(layer, message);
    }

    public static void TraceAndDebugAssert(string layer, bool assertCondition, string message)
    {
      if (assertCondition)
        return;
      TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, TcmTrace.m_traceArea, layer, message);
    }

    public static void IfNullThenTraceAndDebugFail(
      this TestManagementRequestContext context,
      string layer,
      object parameter,
      string parameterName)
    {
      if (parameter != null)
        return;
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Parameter '{0}' is null", (object) parameterName);
      context.TraceAndDebugFail(layer, message);
    }

    public static void IfNullThenTraceAndDebugFail(
      this IVssRequestContext context,
      string layer,
      object parameter,
      string parameterName)
    {
      if (parameter != null)
        return;
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Parameter '{0}' is null", (object) parameterName);
      context.TraceAndDebugFail(layer, message);
    }

    public static void IfEmptyThenTraceAndDebugFail(
      this TestManagementRequestContext context,
      string layer,
      string parameter,
      string parameterName)
    {
      if (!string.IsNullOrWhiteSpace(parameter))
        return;
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Parameter '{0}' is null or empty string", (object) parameterName);
      context.TraceAndDebugFail(layer, message);
    }

    public static void IfNullThenTraceAndDebugFail(
      string layer,
      string parameter,
      string parameterName)
    {
      if (!string.IsNullOrWhiteSpace(parameter))
        return;
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Parameter '{0}' is null or empty string", (object) parameterName);
      TcmTrace.TraceAndDebugFail(layer, message);
    }

    public static void TraceAndDebugFail(
      this TestManagementRequestContext context,
      string layer,
      string message)
    {
      context.TraceError(layer, message);
    }

    public static void TraceAndDebugFail(
      this IVssRequestContext context,
      string layer,
      string message)
    {
      context.Trace(0, TraceLevel.Error, TcmTrace.m_traceArea, layer, message);
    }

    public static void TraceAndDebugFail(string layer, string message) => TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, TcmTrace.m_traceArea, layer, message);
  }
}
