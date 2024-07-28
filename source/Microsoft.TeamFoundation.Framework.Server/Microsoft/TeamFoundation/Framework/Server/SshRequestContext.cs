// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SshRequestContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authorization;
using Microsoft.VisualStudio.Services.Collection;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SshRequestContext : WebRequestContext
  {
    private static readonly string s_Area = "HostManagement";
    private static readonly string s_Layer = nameof (SshRequestContext);

    public SshRequestContext(
      IVssServiceHost serviceHost,
      RequestContextType requestContextType,
      string userAgent,
      HttpContextBase httpContext,
      LockHelper helper,
      TimeSpan timeout)
      : base(serviceHost, requestContextType, httpContext, helper, timeout)
    {
      this.ServiceName = "Ssh";
      this.m_userAgent = userAgent;
    }

    protected override void EndRequest()
    {
      base.EndRequest();
      IDisposableReadOnlyList<ITeamFoundationRequestFilter> requestFilters = this.ServiceHost.ServiceHostInternal().RequestFilters;
      if (requestFilters != null)
      {
        foreach (ITeamFoundationRequestFilter foundationRequestFilter in (IEnumerable<ITeamFoundationRequestFilter>) requestFilters)
        {
          try
          {
            foundationRequestFilter.EndRequest((IVssRequestContext) this);
          }
          catch (Exception ex)
          {
            TeamFoundationTracingService.TraceExceptionRaw(36200, TraceLevel.Error, SshRequestContext.s_Area, SshRequestContext.s_Layer, ex);
          }
        }
      }
      try
      {
        this.GetService<WebRequestLogger>().LogRequest((IVssRequestContext) this);
        if (!this.IsFeatureEnabled("VisualStudio.FrameworkService.ServiceHostExtended"))
          return;
        this.To(TeamFoundationHostType.Deployment).GetService<WebHostLogger>().LogRequest((IVssRequestContext) this);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(36201, TraceLevel.Error, SshRequestContext.s_Area, SshRequestContext.s_Layer, ex);
      }
    }

    public static IVssRequestContext CreateSshDeploymentRequestContext(
      IVssRequestContext systemRC,
      string userAgent,
      string clientAddress = "",
      Guid? e2eId = null)
    {
      systemRC.TraceEnter(36202, SshRequestContext.s_Area, SshRequestContext.s_Layer, "GetSshDeploymentRequestContext");
      try
      {
        IInternalTeamFoundationHostManagementService service = systemRC.GetService<IInternalTeamFoundationHostManagementService>();
        UrlHttpContext httpContext = SshRequestContext.CreateHttpContext(systemRC, SshRequestContext.GetDeploymentRCUri(systemRC), clientAddress, e2eId).httpContext;
        IVssRequestContext requestContext = systemRC;
        Guid instanceId = systemRC.ServiceHost.InstanceId;
        object[] objArray = new object[2]
        {
          (object) httpContext,
          (object) userAgent
        };
        IVssRequestContext deploymentRequestContext = service.BeginRequest(requestContext, instanceId, RequestContextType.SystemContext, true, true, (IReadOnlyList<IRequestActor>) null, HostRequestType.Ssh, objArray);
        deploymentRequestContext.Items[FrameworkServerConstants.UsePartitioningCache] = (object) true;
        return deploymentRequestContext;
      }
      catch (Exception ex)
      {
        systemRC.TraceException(36203, SshRequestContext.s_Area, SshRequestContext.s_Layer, ex);
        throw;
      }
      finally
      {
        systemRC.TraceLeave(36204, SshRequestContext.s_Area, SshRequestContext.s_Layer, "GetSshDeploymentRequestContext");
      }
    }

    public static IVssRequestContext CreateSshNonDeploymentRequestContext(
      IVssRequestContext systemRC,
      (string collection, string project, string repo) commandInfo,
      string clientAddress,
      Guid? e2eId = null)
    {
      (UrlHttpContext httpContext, HostRouteContext routeContext) = SshRequestContext.CreateHttpContext(systemRC, SshRequestContext.GetCollectionishRCUri(systemRC, commandInfo), clientAddress, e2eId);
      if (routeContext == null || (routeContext.RouteFlags & RouteFlags.CollectionHost) != RouteFlags.CollectionHost)
        throw new CollectionDoesNotExistException(commandInfo.collection);
      (IVssRequestContext requestContext, HostProxyData hostProxyData) = systemRC.GetService<IHostRoutingService>().BeginRequest(systemRC, (HttpContextBase) httpContext);
      try
      {
        if (requestContext == null && hostProxyData != null)
        {
          httpContext.Items[(object) HttpContextConstants.ServiceHostRouteContext] = (object) new HostRouteContext()
          {
            HostId = systemRC.ServiceHost.InstanceId,
            VirtualPath = UrlHostResolutionService.ApplicationVirtualPath,
            RouteFlags = RouteFlags.DeploymentHost
          };
          requestContext = systemRC.GetService<IInternalTeamFoundationHostManagementService>().BeginRequest(systemRC, systemRC.ServiceHost.InstanceId, RequestContextType.UserContext, true, true, (IReadOnlyList<IRequestActor>) null, HostRequestType.Ssh, (object) httpContext, (object) systemRC.UserAgent);
          requestContext.Items.Add(RequestContextItemsKeys.HostProxyData, (object) hostProxyData);
        }
        if (requestContext == null)
          throw new CollectionDoesNotExistException(commandInfo.collection);
        requestContext.Items[FrameworkServerConstants.UsePartitioningCache] = (object) true;
        requestContext.WebRequestContextInternal().RequestRestrictions = new RequestRestrictions(RequiredAuthentication.ValidatedUser, AllowedHandler.All, "SSH");
        System.Web.HttpContext.Current = new System.Web.HttpContext(new HttpRequest(httpContext.Request.FilePath, httpContext.Request.Url.ToString(), httpContext.Request.QueryString.ToString()), new HttpResponse(TextWriter.Null));
        IVssRequestContext deploymentRequestContext = requestContext;
        requestContext = (IVssRequestContext) null;
        return deploymentRequestContext;
      }
      finally
      {
        requestContext?.Dispose();
      }
    }

    private static (UrlHttpContext httpContext, HostRouteContext routeContext) CreateHttpContext(
      IVssRequestContext systemRC,
      Uri uri,
      string clientAddress,
      Guid? e2eId)
    {
      IUrlHostResolutionService service = systemRC.GetService<IUrlHostResolutionService>();
      string virtualPathRoot = WebApiConfiguration.GetVirtualPathRoot(systemRC);
      service.ApplicationVirtualPath = virtualPathRoot;
      UrlHttpContext urlHttpContext = new UrlHttpContext(virtualPathRoot, uri, clientAddress);
      HostRouteContext hostRouteContext = service.ResolveHost(systemRC, uri);
      urlHttpContext.Items[(object) HttpContextConstants.ServiceHostRouteContext] = (object) hostRouteContext;
      if (e2eId.HasValue)
        urlHttpContext.Request.Headers.Set("X-VSS-E2EID", e2eId.Value.ToString("D"));
      return (urlHttpContext, hostRouteContext);
    }

    private static Uri GetCollectionishRCUri(
      IVssRequestContext deploymentContext,
      (string collection, string project, string repo) commandInfo)
    {
      ArgumentUtility.CheckForNull<string>(commandInfo.collection, "commandInfo.collection");
      UriBuilder uriBuilder;
      if (deploymentContext.ExecutionEnvironment.IsHostedDeployment)
      {
        uriBuilder = new UriBuilder(LocationServiceHelper.GetServiceBaseUri(deploymentContext, AccessMappingConstants.ServicePathMappingMoniker));
        if (commandInfo.project != null)
          uriBuilder.Path = commandInfo.collection + "/" + commandInfo.project + "/_git/" + commandInfo.repo;
        else
          uriBuilder.Path = commandInfo.collection;
      }
      else
      {
        uriBuilder = new UriBuilder(new Uri(deploymentContext.GetService<ILocationService>().GetLocationServiceUrl(deploymentContext, Guid.Empty, AccessMappingConstants.PublicAccessMappingMoniker)));
        uriBuilder.Path = UriUtility.CombinePath(uriBuilder.Path, commandInfo.collection + "/" + commandInfo.project + "/_git/" + commandInfo.repo);
      }
      return uriBuilder.Uri;
    }

    private static Uri GetDeploymentRCUri(IVssRequestContext deploymentRC) => new Uri(deploymentRC.GetService<ILocationService>().GetLocationServiceUrl(deploymentRC, Guid.Empty, AccessMappingConstants.PublicAccessMappingMoniker));
  }
}
