// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostManagement.HostManagementControllerBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.HostManagement
{
  public abstract class HostManagementControllerBase : TfsApiController
  {
    internal static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (HostManagementArgumentException),
        HttpStatusCode.BadRequest
      },
      {
        typeof (ServiceHostDoesNotExistException),
        HttpStatusCode.NotFound
      }
    };
    private const string c_area = "HostManagement";
    private const string c_layer = "ServiceHostsController";

    [HttpGet]
    public ServiceHostProperties GetServiceHostProperties(Guid hostId)
    {
      this.TfsRequestContext.CheckDeploymentRequestContext();
      return HostManagementControllerBase.GetServiceHostProperties(this.TfsRequestContext, hostId);
    }

    protected static ServiceHostProperties GetServiceHostProperties(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      HostManagementControllerBase.CheckSecurity(requestContext);
      ServiceHostProperties serviceHost = HostManagementControllerBase.GetServiceHost(requestContext, hostId);
      if (serviceHost == null || serviceHost.HostType.HasFlag((System.Enum) ServiceHostType.Deployment))
        throw new ServiceHostDoesNotExistException(hostId);
      if (serviceHost.HostType == ServiceHostType.Application)
        serviceHost.ParentHostId = Guid.Empty;
      return serviceHost;
    }

    private static void CheckSecurity(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(818397552, "HostManagement", "ServiceHostsController", nameof (CheckSecurity));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      try
      {
        vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(vssRequestContext, FrameworkServerConstants.HostManagementToken, 1);
      }
      finally
      {
        requestContext.TraceLeave(232421988, "HostManagement", "ServiceHostsController", nameof (CheckSecurity));
      }
    }

    private static ServiceHostProperties GetServiceHost(
      IVssRequestContext requestContext,
      Guid hostId)
    {
      requestContext.TraceEnter(885354284, "HostManagement", "ServiceHostsController", nameof (GetServiceHost));
      ServiceHostProperties serviceHost = (ServiceHostProperties) null;
      try
      {
        if (requestContext.ServiceHost.InstanceId == hostId)
        {
          requestContext.CheckProjectCollectionOrOrganizationRequestContext();
          serviceHost = HostManagementControllerBase.GetServiceHost(requestContext);
        }
        else
        {
          requestContext.CheckDeploymentRequestContext();
          try
          {
            using (IVssRequestContext hostContext = requestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(requestContext, hostId, RequestContextType.ServicingContext))
              serviceHost = HostManagementControllerBase.GetServiceHost(hostContext);
          }
          catch (HostDoesNotExistException ex)
          {
            throw new ServiceHostDoesNotExistException(hostId);
          }
        }
        return serviceHost;
      }
      finally
      {
        requestContext.TraceLeave(442792954, "HostManagement", "ServiceHostsController", nameof (GetServiceHost));
      }
    }

    private static ServiceHostProperties GetServiceHost(IVssRequestContext hostContext)
    {
      IVssRequestContext vssRequestContext = hostContext.To(TeamFoundationHostType.Deployment);
      HostProperties hostProperties = (HostProperties) vssRequestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(vssRequestContext, hostContext.ServiceHost.InstanceId);
      if (hostProperties == null)
        return (ServiceHostProperties) null;
      ServiceHostProperties serviceHost = HostManagementUtil.Convert(hostProperties);
      serviceHost.Region = hostContext.GetService<IHostRegionService>().GetHostRegion(hostContext);
      return serviceHost;
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) HostManagementControllerBase.s_httpExceptions;

    public override string ActivityLogArea => "HostManagement";

    public override string TraceArea => "ServiceHostsController";
  }
}
