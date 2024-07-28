// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence.ClientTraceService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebPlatform;
using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence
{
  public class ClientTraceService : IVssFrameworkService
  {
    private bool m_isTracingSupported;
    private const string s_liveIdIdentifierSuffix = "@live.com";
    private const string s_liveIdIdentityType = "WLID";
    private static readonly string s_area = "Telemetry";
    private static readonly string s_layer = nameof (ClientTraceService);

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_isTracingSupported = systemRequestContext.ExecutionEnvironment.IsHostedDeployment;

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      ClientTraceData properties,
      Level level = Level.Info,
      string method = "",
      string component = "",
      string message = "",
      string exceptionType = "")
    {
      this.Publish(requestContext, requestContext.ServiceHost.InstanceId, area, feature, method, component, message, exceptionType, level, properties);
    }

    public virtual void Publish(
      IVssRequestContext requestContext,
      Guid hostId,
      string area,
      string feature,
      string method,
      string component,
      string message,
      string exceptionType,
      Level level,
      ClientTraceData properties)
    {
      Guid userId = requestContext.RootContext.GetUserId();
      IdentityTracingItems identityTracingItems = requestContext.RootContext.GetUserIdentityTracingItems();
      this.Publish(requestContext, hostId, userId, identityTracingItems != null ? identityTracingItems.Cuid : Guid.Empty, DateTime.UtcNow, area, feature, method, component, message, exceptionType, identityTracingItems != null ? identityTracingItems.TenantId : Guid.Empty, identityTracingItems != null ? identityTracingItems.ProviderId : Guid.Empty, level, properties);
    }

    public virtual void Publish(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid vsid,
      Guid cuid,
      DateTime timeStamp,
      string area,
      string feature,
      string method,
      string component,
      string message,
      string exceptionType,
      Guid TenantId,
      Guid providerId,
      Level level,
      ClientTraceData properties)
    {
      this.PublishCTEvents(requestContext, properties, hostId, vsid, area, feature, cuid, timeStamp, method, component, message, exceptionType, TenantId, providerId, level);
    }

    private void PublishCTEvents(
      IVssRequestContext requestContext,
      ClientTraceData properties,
      Guid hostId,
      Guid vsid,
      string area,
      string feature,
      Guid cuid,
      DateTime timeStamp,
      string method,
      string component,
      string message,
      string exceptionType,
      Guid tenantId,
      Guid providerId,
      Level level)
    {
      requestContext.TraceEnter(522210, ClientTraceService.s_area, ClientTraceService.s_layer, nameof (PublishCTEvents));
      if (!this.IsTracingEnabled(requestContext))
        return;
      TeamFoundationHostType hostType = this.GetHostType(requestContext);
      string userAgent = requestContext.UserAgent;
      Guid parentHostId = requestContext.ServiceHost.ParentServiceHost != null ? requestContext.ServiceHost.ParentServiceHost.InstanceId : Guid.Empty;
      Guid uniqueIdentifier = requestContext.UniqueIdentifier;
      string anonymousIdentifier = requestContext.GetAnonymousIdentifier();
      Guid e2Eid = requestContext.RootContext.E2EId;
      if (properties == null)
        properties = new ClientTraceData();
      try
      {
        requestContext.TracingService().TraceClientTrace(this.ToJson(properties), uniqueIdentifier, anonymousIdentifier, hostId, parentHostId, (byte) hostType, vsid, area, feature, userAgent, cuid, method, component, message, exceptionType, e2Eid, tenantId, providerId, level, timeStamp);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(522214, TraceLevel.Warning, ClientTraceService.s_area, ClientTraceService.s_layer, ex);
      }
      finally
      {
        requestContext.TraceLeave(522220, ClientTraceService.s_area, ClientTraceService.s_layer, nameof (PublishCTEvents));
      }
    }

    private TeamFoundationHostType GetHostType(IVssRequestContext requestContext)
    {
      TeamFoundationHostType hostType = requestContext.ServiceHost.HostType;
      if ((hostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment)
        hostType = TeamFoundationHostType.Deployment;
      return hostType;
    }

    public bool IsTracingEnabled(IVssRequestContext requestContext) => this.m_isTracingSupported && requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.ClientTrace");

    private string ToJson(ClientTraceData properties) => JsonConvert.SerializeObject((object) properties.GetData());
  }
}
