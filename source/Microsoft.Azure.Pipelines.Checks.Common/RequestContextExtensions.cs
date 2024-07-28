// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Common.RequestContextExtensions
// Assembly: Microsoft.Azure.Pipelines.Checks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C585FB3-01FB-4B82-B4E2-03BD94D0A581
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.Azure.Pipelines.Checks.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class RequestContextExtensions
  {
    public static void TraceError(
      this IVssRequestContext requestContext,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, 0, TraceLevel.Error, "PipelineChecks", layer, format, arguments);
    }

    public static void TraceError(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, tracepoint, TraceLevel.Error, "PipelineChecks", layer, format, arguments);
    }

    public static void TraceException(
      this IVssRequestContext requestContext,
      string layer,
      Exception exception)
    {
      requestContext.TraceException(0, "PipelineChecks", layer, exception);
    }

    public static void TraceException(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      Exception exception)
    {
      requestContext.TraceException(tracepoint, "PipelineChecks", layer, exception);
    }

    public static void TraceAlways(
      this IVssRequestContext requestContext,
      string layer,
      string format,
      params object[] arguments)
    {
      requestContext.TraceAlways(0, TraceLevel.Info, "PipelineChecks", layer, format, arguments);
    }

    public static void TraceAlways(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string format,
      params object[] arguments)
    {
      requestContext.TraceAlways(tracepoint, TraceLevel.Info, "PipelineChecks", layer, format, arguments);
    }

    public static void TraceInfo(
      this IVssRequestContext requestContext,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, 0, TraceLevel.Info, "PipelineChecks", layer, format, arguments);
    }

    public static void TraceInfo(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, tracepoint, TraceLevel.Info, "PipelineChecks", layer, format, arguments);
    }

    public static void TraceWarning(
      this IVssRequestContext requestContext,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, 0, TraceLevel.Warning, "PipelineChecks", layer, format, arguments);
    }

    public static void TraceWarning(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, tracepoint, TraceLevel.Warning, "PipelineChecks", layer, format, arguments);
    }

    public static void TraceVerbose(
      this IVssRequestContext requestContext,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, 0, TraceLevel.Verbose, "PipelineChecks", layer, format, arguments);
    }

    public static void TraceVerbose(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, tracepoint, TraceLevel.Verbose, "PipelineChecks", layer, format, arguments);
    }

    public static void TraceEnter(
      this IVssRequestContext requestContext,
      string layer,
      [CallerMemberName] string method = null)
    {
      requestContext.TraceEnter(0, "PipelineChecks", layer, method);
    }

    public static void TraceEnter(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      [CallerMemberName] string method = null)
    {
      requestContext.TraceEnter(tracepoint, "PipelineChecks", layer, method);
    }

    public static void TraceLeave(
      this IVssRequestContext requestContext,
      string layer,
      [CallerMemberName] string method = null)
    {
      requestContext.TraceLeave(0, "PipelineChecks", layer, method);
    }

    public static void TraceLeave(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      [CallerMemberName] string method = null)
    {
      requestContext.TraceLeave(tracepoint, "PipelineChecks", layer, method);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IDisposable TraceScope(
      this IVssRequestContext requestContext,
      string layer,
      [CallerMemberName] string method = null)
    {
      return (IDisposable) new MethodScope(requestContext, layer, method);
    }

    public static bool IsScalabilityComplianceCheckWarningFeatureEnabled(
      this IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("Pipelines.Checks.EnableChecksScalabilityPhase1Warnings");
    }

    public static bool IsScalabilityComplianceCheckErrorFeatureEnabled(
      this IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled("Pipelines.Checks.EnableChecksScalabilityPhase2Errors") || requestContext.IsFeatureEnabled("Pipelines.Checks.EnableChecksScalabilityPhase3FailCheckRuns");
    }

    public static bool IsScalabilityComplianceCheckFeatureEnabled(
      this IVssRequestContext requestContext)
    {
      return requestContext.IsScalabilityComplianceCheckWarningFeatureEnabled() || requestContext.IsScalabilityComplianceCheckErrorFeatureEnabled();
    }

    public static bool IsBypassCheckFeatureEnabled(this IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("Pipelines.Checks.EnableBypassTaskChecks") || requestContext.IsFeatureEnabled("Pipelines.Checks.EnableBypassApprovals");
  }
}
