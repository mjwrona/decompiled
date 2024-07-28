// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common.SmartRouterBase
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common
{
  public abstract class SmartRouterBase
  {
    protected SmartRouterBase(SmartRouterBase.TraceLayer traceLayer) => this.Tracer = new SmartRouterBase.TraceRequestHelper("SmartRouter", traceLayer.ToString());

    protected SmartRouterBase.TraceRequestHelper Tracer { get; }

    protected enum TracePoint
    {
      Start = 34005000, // 0x0206E008
      ShouldNotRouteRequest = 34005004, // 0x0206E00C
      NoTargetServer = 34005005, // 0x0206E00D
      TargetServer = 34005008, // 0x0206E010
      BackEndTimerTick = 34005014, // 0x0206E016
      BackEndTimerException = 34005015, // 0x0206E017
      DiscoverServersNone = 34005016, // 0x0206E018
      DiscoverServersFailed = 34005017, // 0x0206E019
      PublishServer = 34005021, // 0x0206E01D
      PublisherNoIpAddressForHost = 34005022, // 0x0206E01E
      DiscoverServersSucceeded = 34005023, // 0x0206E01F
      BeginRequestException = 34005028, // 0x0206E024
      ProbeHealthJob = 34005029, // 0x0206E025
      ProbeHealthJobException = 34005030, // 0x0206E026
      GetServerHealth = 34005032, // 0x0206E028
      GetServerHealthExpired = 34005033, // 0x0206E029
      ProbeHealthJobCanceled = 34005034, // 0x0206E02A
      GetInstanceNetworkMetadataException = 34005035, // 0x0206E02B
      GetLocalhostIpAddress = 34005036, // 0x0206E02C
      GetLocalhostIpAddressNone = 34005037, // 0x0206E02D
      HealthProbeNoDiscoveredServers = 34005039, // 0x0206E02F
      GetServerHealthUnknown = 34005040, // 0x0206E030
      DiscoveryRefreshException = 34005041, // 0x0206E031
      ConsistentHashPolicyServerCount = 34005042, // 0x0206E032
      PolicyNotEnabled = 34005043, // 0x0206E033
      PolicyTryRouteNoServers = 34005044, // 0x0206E034
      PolicyTryRouteSuccess = 34005045, // 0x0206E035
      NotEnoughServers = 34005046, // 0x0206E036
      NoReverseProxyToSelf = 34005048, // 0x0206E038
      ProbeHealthSocketException = 34005049, // 0x0206E039
      InvalidIpAddress = 34005050, // 0x0206E03A
    }

    protected enum TraceLayer
    {
      BackEnd,
      Common,
      Http,
      Routing,
      Settings,
    }

    protected struct TraceRequestHelper
    {
      private readonly string m_area;
      private readonly string m_layer;

      public TraceRequestHelper(string area, string layer)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(area, nameof (area));
        ArgumentUtility.CheckStringForNullOrEmpty(layer, nameof (layer));
        this.m_area = area;
        this.m_layer = layer;
      }

      public void TraceInfo(
        IVssRequestContext requestContext,
        SmartRouterBase.TracePoint tracePoint,
        string message,
        params object?[] args)
      {
        VssRequestContextExtensions.Trace(requestContext, (int) tracePoint, TraceLevel.Info, this.m_area, this.m_layer, message, args);
      }

      public void TraceWarning(
        IVssRequestContext requestContext,
        SmartRouterBase.TracePoint tracePoint,
        string message,
        params object?[] args)
      {
        VssRequestContextExtensions.Trace(requestContext, (int) tracePoint, TraceLevel.Warning, this.m_area, this.m_layer, message, args);
      }

      public void TraceVerbose(
        IVssRequestContext requestContext,
        SmartRouterBase.TracePoint tracePoint,
        string message,
        params object?[] args)
      {
        VssRequestContextExtensions.Trace(requestContext, (int) tracePoint, TraceLevel.Verbose, this.m_area, this.m_layer, message, args);
      }

      public void TraceException(
        IVssRequestContext requestContext,
        SmartRouterBase.TracePoint tracePoint,
        Exception ex,
        string? message = null,
        params object?[] args)
      {
        if (string.IsNullOrEmpty(message))
          requestContext.TraceException((int) tracePoint, TraceLevel.Error, this.m_area, this.m_layer, ex);
        else
          requestContext.TraceException((int) tracePoint, TraceLevel.Error, this.m_area, this.m_layer, ex, message, args);
      }

      public void TraceException(
        SmartRouterBase.TracePoint tracePoint,
        Exception ex,
        string? message = null,
        params object?[] args)
      {
        if (string.IsNullOrEmpty(message))
          TeamFoundationTracingService.TraceExceptionRaw((int) tracePoint, TraceLevel.Error, this.m_area, this.m_layer, ex);
        else
          TeamFoundationTracingService.TraceExceptionRaw((int) tracePoint, TraceLevel.Error, this.m_area, this.m_layer, ex, message, args);
      }

      public Lazy<string> GetLazyStackTrace(Exception ex) => new Lazy<string>((Func<string>) (() => ex.ToReadableStackTrace()));
    }
  }
}
