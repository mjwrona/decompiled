// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DevOpsS2SAndDeploymentHostResolver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.NameResolution;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [ExtensionPriority(150)]
  [ExtensionStrategy("Hosted")]
  internal class DevOpsS2SAndDeploymentHostResolver : IUrlHostResolver
  {
    private string m_accessMappingDomain;
    private string m_accessMappingVirtualPath;
    private const string c_area = "HostResolution";
    private const string c_layer = "DevOpsS2SAndDeploymentHostResolver";

    public string Name => "DevOpsS2SAndDeployment";

    public void Initialize(IVssRequestContext requestContext)
    {
      AccessMapping accessMapping = requestContext.GetService<ILocationService>().GetAccessMapping(requestContext, AccessMappingConstants.ServiceDomainMappingMoniker);
      if (accessMapping == null)
        return;
      Uri uri = new Uri(accessMapping.AccessPoint);
      this.m_accessMappingDomain = uri.Host;
      this.m_accessMappingVirtualPath = uri.AbsolutePath;
      if (!(this.m_accessMappingVirtualPath != "/") || !requestContext.ServiceHost.IsProduction)
        return;
      requestContext.Trace(497991, TraceLevel.Error, "HostResolution", nameof (DevOpsS2SAndDeploymentHostResolver), "Unexpected non-empty virtual path {0} in multi-instance mapping", (object) this.m_accessMappingVirtualPath);
    }

    public void Shutdown(IVssRequestContext requestContext)
    {
    }

    public bool TryResolveHost(
      IVssRequestContext requestContext,
      Uri requestUri,
      string applicationVirtualPath,
      out HostRouteContext routeContext)
    {
      string host = requestUri.Host;
      if (this.m_accessMappingDomain == null || !StringComparer.OrdinalIgnoreCase.Equals(this.m_accessMappingDomain, host))
      {
        routeContext = (HostRouteContext) null;
        return false;
      }
      string absoluteVirtualPath;
      string firstPathSegment1 = UrlHostResolutionHelper.GetFirstPathSegment(requestContext, requestUri, this.m_accessMappingVirtualPath, out absoluteVirtualPath);
      if ("serviceDeployments".Equals(firstPathSegment1, StringComparison.OrdinalIgnoreCase))
      {
        string firstPathSegment2 = UrlHostResolutionHelper.GetFirstPathSegment(requestContext, requestUri, absoluteVirtualPath, out absoluteVirtualPath);
        routeContext = this.ResolveDeployment(requestContext, firstPathSegment2, absoluteVirtualPath, host);
      }
      else if ("serviceHosts".Equals(firstPathSegment1, StringComparison.OrdinalIgnoreCase))
      {
        string firstPathSegment3 = UrlHostResolutionHelper.GetFirstPathSegment(requestContext, requestUri, absoluteVirtualPath, out absoluteVirtualPath);
        routeContext = this.ResolveHost(requestContext, firstPathSegment3, absoluteVirtualPath, host);
      }
      else
      {
        ref HostRouteContext local = ref routeContext;
        DevOpsS2SAndDeploymentHostResolver.ServiceDomainHostRouteContext hostRouteContext = new DevOpsS2SAndDeploymentHostResolver.ServiceDomainHostRouteContext();
        hostRouteContext.HostId = requestContext.ServiceHost.InstanceId;
        hostRouteContext.VirtualPath = this.m_accessMappingVirtualPath;
        hostRouteContext.WebApplicationPath = this.m_accessMappingVirtualPath;
        hostRouteContext.RouteFlags = RouteFlags.DeploymentHost;
        local = (HostRouteContext) hostRouteContext;
      }
      return true;
    }

    public bool TryResolveUriData(
      IVssRequestContext requestContext,
      Guid hostId,
      out IHostUriData uriData)
    {
      uriData = (IHostUriData) null;
      return false;
    }

    public HostRouteContext ResolveDeployment(
      IVssRequestContext requestContext,
      string deploymentName,
      string absoluteVirtualPath,
      string domain)
    {
      if (string.IsNullOrEmpty(deploymentName))
        return (HostRouteContext) null;
      NameResolutionEntry nameResolutionEntry = requestContext.GetService<INameResolutionService>().QueryEntry(requestContext, "Deployment", deploymentName);
      if (nameResolutionEntry == null || !nameResolutionEntry.IsEnabled)
        return (HostRouteContext) null;
      DevOpsS2SAndDeploymentHostResolver.ServiceDomainHostRouteContext hostRouteContext = new DevOpsS2SAndDeploymentHostResolver.ServiceDomainHostRouteContext();
      hostRouteContext.HostId = nameResolutionEntry.Value;
      hostRouteContext.VirtualPath = absoluteVirtualPath;
      hostRouteContext.WebApplicationPath = absoluteVirtualPath;
      hostRouteContext.RouteFlags = RouteFlags.DeploymentHost;
      hostRouteContext.RouteKeyVersion = new int?(2);
      hostRouteContext.ExpectedRouteKey = domain + "/serviceDeployments/" + deploymentName;
      return (HostRouteContext) hostRouteContext;
    }

    public HostRouteContext ResolveHost(
      IVssRequestContext requestContext,
      string hostIdString,
      string absoluteVirtualPath,
      string domain)
    {
      Guid result;
      if (!Guid.TryParse(hostIdString, out result))
        return (HostRouteContext) null;
      DevOpsS2SAndDeploymentHostResolver.ServiceDomainHostRouteContext hostRouteContext = new DevOpsS2SAndDeploymentHostResolver.ServiceDomainHostRouteContext(requestContext.IsFeatureEnabled("VisualStudio.Services.Location.UseDevOpsDomainForS2S") ? AccessMappingConstants.HostGuidAccessMappingMoniker : AccessMappingConstants.ServiceDomainMappingMoniker);
      hostRouteContext.HostId = result;
      hostRouteContext.VirtualPath = absoluteVirtualPath;
      hostRouteContext.WebApplicationPath = absoluteVirtualPath;
      hostRouteContext.RouteFlags = RouteFlags.AnyHost;
      hostRouteContext.RouteKeyVersion = new int?(2);
      hostRouteContext.ExpectedRouteKey = domain + "/serviceHosts/" + hostIdString;
      return (HostRouteContext) hostRouteContext;
    }

    internal static bool IsReservedFirstPathSegment(string firstPathSegment) => "serviceDeployments".Equals(firstPathSegment, StringComparison.OrdinalIgnoreCase) || "serviceHosts".Equals(firstPathSegment, StringComparison.OrdinalIgnoreCase);

    internal class ServiceDomainHostRouteContext : HostRouteContext
    {
      public ServiceDomainHostRouteContext()
        : this(AccessMappingConstants.ServiceDomainMappingMoniker)
      {
      }

      public ServiceDomainHostRouteContext(string moniker) => this.AccessMappingMonikers = moniker;
    }
  }
}
