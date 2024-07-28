// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.RequestContextExtensions
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  public static class RequestContextExtensions
  {
    public static void TraceError(
      this IVssRequestContext requestContext,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, 0, TraceLevel.Error, "DevSecOps", layer, format, arguments);
    }

    public static void TraceError(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, tracepoint, TraceLevel.Error, "DevSecOps", layer, format, arguments);
    }

    public static void TraceException(
      this IVssRequestContext requestContext,
      string layer,
      Exception exception)
    {
      requestContext.TraceException(0, "DevSecOps", layer, exception);
    }

    public static void TraceException(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      Exception exception)
    {
      requestContext.TraceException(tracepoint, "DevSecOps", layer, exception);
    }

    public static void TraceInfo(
      this IVssRequestContext requestContext,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, 0, TraceLevel.Info, "DevSecOps", layer, format, arguments);
    }

    public static void TraceInfo(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, tracepoint, TraceLevel.Info, "DevSecOps", layer, format, arguments);
    }

    public static void TraceWarning(
      this IVssRequestContext requestContext,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, 0, TraceLevel.Warning, "DevSecOps", layer, format, arguments);
    }

    public static void TraceWarning(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, tracepoint, TraceLevel.Warning, "DevSecOps", layer, format, arguments);
    }

    public static void TraceVerbose(
      this IVssRequestContext requestContext,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, 0, TraceLevel.Verbose, "DevSecOps", layer, format, arguments);
    }

    public static void TraceVerbose(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      string format,
      params object[] arguments)
    {
      VssRequestContextExtensions.Trace(requestContext, tracepoint, TraceLevel.Verbose, "DevSecOps", layer, format, arguments);
    }

    public static void TraceEnter(
      this IVssRequestContext requestContext,
      string layer,
      [CallerMemberName] string method = null)
    {
      requestContext.TraceEnter(0, "DevSecOps", layer, method);
    }

    public static void TraceEnter(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      [CallerMemberName] string method = null)
    {
      requestContext.TraceEnter(tracepoint, "DevSecOps", layer, method);
    }

    public static void TraceLeave(
      this IVssRequestContext requestContext,
      string layer,
      [CallerMemberName] string method = null)
    {
      requestContext.TraceLeave(0, "DevSecOps", layer, method);
    }

    public static void TraceLeave(
      this IVssRequestContext requestContext,
      int tracepoint,
      string layer,
      [CallerMemberName] string method = null)
    {
      requestContext.TraceLeave(tracepoint, "DevSecOps", layer, method);
    }

    public static bool IsMicrosoftTenant(this IVssRequestContext requestContext) => requestContext.ExecutionEnvironment.IsDevFabricDeployment || requestContext.GetOrganizationAadTenantId() == new Guid("72f988bf-86f1-41af-91ab-2d7cd011db47") || requestContext.GetOrganizationAadTenantId() == new Guid("33e01921-4d64-4f8c-a055-5bdaffd5e33d") || requestContext.GetOrganizationAadTenantId() == new Guid("975f013f-7f24-47e8-a7d3-abc4752bf346") || requestContext.GetOrganizationAadTenantId() == new Guid("16b3c013-d300-468d-ac64-7eda0820b6d3") || requestContext.GetOrganizationAadTenantId() == new Guid("cdc5aeea-15c5-4db6-b079-fcadd2505dc2");
  }
}
