// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Tracing.EuiiTracingService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server.Tracing
{
  public class EuiiTracingService : IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool TraceEuii(
      IVssRequestContext requestContext,
      string area,
      string feature,
      string method,
      string properties,
      string uri = "",
      string component = "",
      string message = "",
      string exceptionType = "",
      TraceLevel level = TraceLevel.Info)
    {
      DateTime utcNow = DateTime.UtcNow;
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      TeamFoundationHostType hostType = this.GetHostType(requestContext);
      string userAgent = requestContext.UserAgent;
      Guid parentHostId = requestContext.ServiceHost.ParentServiceHost != null ? requestContext.ServiceHost.ParentServiceHost.InstanceId : Guid.Empty;
      Guid uniqueIdentifier = requestContext.UniqueIdentifier;
      string anonymousIdentifier = requestContext.GetAnonymousIdentifier();
      Guid e2Eid = requestContext.RootContext.E2EId;
      Guid userId = requestContext.RootContext.GetUserId();
      IdentityTracingItems identityTracingItems = requestContext.RootContext.GetUserIdentityTracingItems();
      return requestContext.TracingService().TraceEuii(properties, uniqueIdentifier, anonymousIdentifier, instanceId, parentHostId, (byte) hostType, userId, area, feature, userAgent, identityTracingItems != null ? identityTracingItems.Cuid : Guid.Empty, method, uri, component, message, exceptionType, e2Eid, identityTracingItems != null ? identityTracingItems.TenantId : Guid.Empty, identityTracingItems != null ? identityTracingItems.ProviderId : Guid.Empty, level, utcNow);
    }

    private TeamFoundationHostType GetHostType(IVssRequestContext requestContext)
    {
      TeamFoundationHostType hostType = requestContext.ServiceHost.HostType;
      if ((hostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment)
        hostType = TeamFoundationHostType.Deployment;
      return hostType;
    }
  }
}
