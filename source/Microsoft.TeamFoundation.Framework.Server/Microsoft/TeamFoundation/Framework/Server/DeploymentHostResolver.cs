// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DeploymentHostResolver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.NameResolution;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [ExtensionPriority(200)]
  [ExtensionStrategy("Hosted")]
  internal class DeploymentHostResolver : IUrlHostResolver
  {
    private DeploymentHostResolver.AccessMappingRef[] m_deploymentAccessPoints;
    private string m_hostedServiceName;
    private string m_virtualPath;
    private static readonly RegistryQuery s_hostedServiceNameQuery = (RegistryQuery) "/Configuration/Settings/HostedServiceName";

    public virtual string Name => "Deployment";

    public virtual void Initialize(IVssRequestContext requestContext)
    {
      requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, "Default", SqlNotificationEventClasses.LocationDataChanged, new SqlNotificationCallback(this.OnDeploymentAccessPointsChanged), true);
      Interlocked.CompareExchange<DeploymentHostResolver.AccessMappingRef[]>(ref this.m_deploymentAccessPoints, DeploymentHostResolver.GetDeploymentAccessPoints(requestContext), (DeploymentHostResolver.AccessMappingRef[]) null);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      this.m_hostedServiceName = service.GetValue(requestContext, in DeploymentHostResolver.s_hostedServiceNameQuery);
      string str = service.GetValue(requestContext, in ConfigurationConstants.TestEnvironmentQuery);
      this.m_virtualPath = DeploymentHostResolver.NormalizeVirtualPath(string.IsNullOrEmpty(str) ? "/" : "/" + str);
    }

    public void Shutdown(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, "Default", SqlNotificationEventClasses.LocationDataChanged, new SqlNotificationCallback(this.OnDeploymentAccessPointsChanged), false);

    public virtual bool TryResolveHost(
      IVssRequestContext requestContext,
      Uri requestUri,
      string applicationVirtualPath,
      out HostRouteContext routeContext)
    {
      string host = requestUri.Host;
      if (IPAddress.TryParse(host, out IPAddress _) || host.EndsWith(".cloudapp.net", StringComparison.OrdinalIgnoreCase) || host.EndsWith(".cloudapp.azure.com", StringComparison.OrdinalIgnoreCase) || host.Equals("localhost", StringComparison.OrdinalIgnoreCase) && requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        routeContext = new HostRouteContext()
        {
          HostId = requestContext.ServiceHost.InstanceId,
          VirtualPath = this.m_virtualPath,
          RouteFlags = RouteFlags.DeploymentHost
        };
        return true;
      }
      if (this.TryResolveHostFromAGuid(requestContext, requestUri, applicationVirtualPath, out routeContext))
        return true;
      DeploymentHostResolver.AccessMappingRef accessMappingRef = this.MatchAccessMapping(host);
      if (accessMappingRef.Matched)
      {
        if (accessMappingRef.SupportsAffinity)
        {
          routeContext = this.ResolveAffinitizedMultiInstance(requestContext, accessMappingRef.VirtualPath);
        }
        else
        {
          if (this.TryResolveHostFromAGuid(requestContext, requestUri, accessMappingRef.VirtualPath, out routeContext))
            return true;
          routeContext = new HostRouteContext()
          {
            HostId = requestContext.ServiceHost.InstanceId,
            VirtualPath = accessMappingRef.VirtualPath,
            WebApplicationPath = accessMappingRef.VirtualPath,
            RouteFlags = RouteFlags.DeploymentHost,
            RouteKeyVersion = new int?(0),
            ExpectedRouteKey = host
          };
        }
        routeContext.AccessMappingMonikers = accessMappingRef.Moniker;
        return true;
      }
      routeContext = (HostRouteContext) null;
      return false;
    }

    private HostRouteContext ResolveAffinitizedMultiInstance(
      IVssRequestContext requestContext,
      string virtualPath)
    {
      HttpContext context = HttpContext.Current;
      string name = context.Request.Cookies["X-VSS-DeploymentAffinity"]?.Value;
      if (string.IsNullOrEmpty(name))
        return SetAffinityCookieAndRouteLocally();
      if (name == this.m_hostedServiceName)
        return new HostRouteContext()
        {
          HostId = requestContext.ServiceHost.InstanceId,
          VirtualPath = virtualPath,
          WebApplicationPath = virtualPath,
          RouteFlags = RouteFlags.DeploymentHost,
          RouteKeyVersion = new int?(2),
          ExpectedRouteKey = this.m_hostedServiceName
        };
      if (!string.IsNullOrEmpty(context.Request.Headers["X-FD-RouteKeyApplicationEndpointList"]))
        return SetAffinityCookieAndRouteLocally();
      NameResolutionEntry nameResolutionEntry = requestContext.GetService<INameResolutionService>().QueryEntry(requestContext, "Deployment", name);
      if (nameResolutionEntry == null)
        return SetAffinityCookieAndRouteLocally();
      return new HostRouteContext()
      {
        HostId = nameResolutionEntry.Value,
        VirtualPath = virtualPath,
        WebApplicationPath = virtualPath,
        RouteFlags = RouteFlags.DeploymentHost,
        RouteKeyVersion = new int?(2),
        ExpectedRouteKey = name
      };

      HostRouteContext SetAffinityCookieAndRouteLocally()
      {
        HttpCookie cookie = new HttpCookie("X-VSS-DeploymentAffinity")
        {
          Value = this.m_hostedServiceName,
          Path = virtualPath,
          HttpOnly = true,
          Secure = requestContext.ExecutionEnvironment.IsSslOnly
        };
        context.Response.Cookies.Add(cookie);
        return new HostRouteContext()
        {
          HostId = requestContext.ServiceHost.InstanceId,
          VirtualPath = virtualPath,
          WebApplicationPath = virtualPath,
          RouteFlags = RouteFlags.DeploymentHost
        };
      }
    }

    public bool TryResolveUriData(
      IVssRequestContext requestContext,
      Guid hostId,
      out IHostUriData uriData)
    {
      uriData = (IHostUriData) null;
      return false;
    }

    public Uri BuildHostUri(
      IVssRequestContext requestContext,
      IHostUriData uriData,
      Guid serviceIdentifier = default (Guid))
    {
      return (Uri) null;
    }

    private void OnDeploymentAccessPointsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      Interlocked.Exchange<DeploymentHostResolver.AccessMappingRef[]>(ref this.m_deploymentAccessPoints, DeploymentHostResolver.GetDeploymentAccessPoints(requestContext));
    }

    private static DeploymentHostResolver.AccessMappingRef[] GetDeploymentAccessPoints(
      IVssRequestContext requestContext)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return requestContext.GetService<ILocationService>().GetAccessMappings(requestContext).Where<AccessMapping>(DeploymentHostResolver.\u003C\u003EO.\u003C0\u003E__FilterAccessMapping ?? (DeploymentHostResolver.\u003C\u003EO.\u003C0\u003E__FilterAccessMapping = new Func<AccessMapping, bool>(DeploymentHostResolver.FilterAccessMapping))).Select<AccessMapping, DeploymentHostResolver.AccessMappingRef>((Func<AccessMapping, DeploymentHostResolver.AccessMappingRef>) (x => new DeploymentHostResolver.AccessMappingRef(x))).ToArray<DeploymentHostResolver.AccessMappingRef>();
    }

    private static bool FilterAccessMapping(AccessMapping accessMapping) => !VssStringComparer.AccessMappingMoniker.Equals(accessMapping.Moniker, AccessMappingConstants.RootDomainMappingMoniker) && !VssStringComparer.AccessMappingMoniker.Equals(accessMapping.Moniker, AccessMappingConstants.ServicePathMappingMoniker) && !VssStringComparer.AccessMappingMoniker.Equals(accessMapping.Moniker, AccessMappingConstants.ServiceDomainMappingMoniker);

    private static string NormalizeVirtualPath(string virtualPath)
    {
      if (UrlHostResolutionService.ApplicationVirtualPath.Length > 1 && !virtualPath.StartsWith(UrlHostResolutionService.ApplicationVirtualPath, StringComparison.OrdinalIgnoreCase))
        virtualPath = VirtualPathUtility.RemoveTrailingSlash(UrlHostResolutionService.ApplicationVirtualPath + virtualPath);
      return virtualPath;
    }

    private bool TryResolveHostFromAGuid(
      IVssRequestContext requestContext,
      Uri requestUri,
      string applicationVirtualPath,
      out HostRouteContext routeContext)
    {
      string absoluteVirtualPath;
      string firstPathSegment = UrlHostResolutionHelper.GetFirstPathSegment(requestContext, requestUri, applicationVirtualPath, out absoluteVirtualPath);
      Guid hostId;
      if (firstPathSegment != null && UrlHostResolutionHelper.ParseHostIdFromPathSegment(firstPathSegment, out hostId))
      {
        ref HostRouteContext local = ref routeContext;
        DeploymentHostResolver.HostGuidRouteContext guidRouteContext = new DeploymentHostResolver.HostGuidRouteContext();
        guidRouteContext.HostId = hostId;
        guidRouteContext.VirtualPath = absoluteVirtualPath;
        guidRouteContext.WebApplicationPath = applicationVirtualPath;
        guidRouteContext.RouteFlags = RouteFlags.AnyHost;
        guidRouteContext.RouteKeyVersion = new int?(0);
        guidRouteContext.ExpectedRouteKey = requestUri.Host;
        local = (HostRouteContext) guidRouteContext;
        return true;
      }
      routeContext = (HostRouteContext) null;
      return false;
    }

    private DeploymentHostResolver.AccessMappingRef MatchAccessMapping(string domain)
    {
      foreach (DeploymentHostResolver.AccessMappingRef deploymentAccessPoint in this.m_deploymentAccessPoints)
      {
        if (StringComparer.OrdinalIgnoreCase.Equals(domain, deploymentAccessPoint.Host))
          return deploymentAccessPoint;
      }
      return new DeploymentHostResolver.AccessMappingRef();
    }

    private struct AccessMappingRef
    {
      public AccessMappingRef(AccessMapping accessMapping)
      {
        Uri uri = accessMapping.AccessPoint.AsUri();
        this.Host = uri.Host;
        this.VirtualPath = DeploymentHostResolver.NormalizeVirtualPath(uri.AbsolutePath);
        this.SupportsAffinity = VssStringComparer.AccessMappingMoniker.Equals(accessMapping.Moniker, AccessMappingConstants.AffinitizedMultiInstanceAccessMappingMoniker);
        this.Moniker = accessMapping.Moniker;
      }

      public string Host { get; }

      public string VirtualPath { get; }

      public bool SupportsAffinity { get; }

      public string Moniker { get; }

      public bool Matched => this.Host != null || this.VirtualPath != null;
    }

    internal class HostGuidRouteContext : HostRouteContext
    {
      private static readonly string c_accessMappingMonikers = AccessMappingConstants.HostGuidAccessMappingMoniker;

      public HostGuidRouteContext() => this.AccessMappingMonikers = DeploymentHostResolver.HostGuidRouteContext.c_accessMappingMonikers;
    }
  }
}
