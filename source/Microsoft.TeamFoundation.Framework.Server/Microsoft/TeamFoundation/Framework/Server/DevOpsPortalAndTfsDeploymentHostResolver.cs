// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DevOpsPortalAndTfsDeploymentHostResolver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [ExtensionPriority(50)]
  [ExtensionStrategy("Hosted")]
  internal class DevOpsPortalAndTfsDeploymentHostResolver : IUrlHostResolver
  {
    private static readonly IReadOnlyCollection<Guid> s_allowedInstanceTypes = (IReadOnlyCollection<Guid>) new HashSet<Guid>()
    {
      new Guid("0000004D-0000-8888-8000-000000000000"),
      ServiceInstanceTypes.TFS
    };
    private static readonly IReadOnlyCollection<string> s_allowedVirtualPaths = (IReadOnlyCollection<string>) new List<string>()
    {
      "/_apis/resourceareas",
      "/_public/_MsalRedirect",
      "/_public/_MsalSignedIn",
      "/_public/_MsalJsSignout"
    };
    private static readonly IReadOnlyCollection<string> s_allowedVirtualPathPrefixes = (IReadOnlyCollection<string>) new List<string>()
    {
      "/_apis/resourceareas/",
      "/_static/3rdParty/_scripts/msal-browser"
    };
    private string m_serviceMappingDomain;
    private string m_serviceMappingVirtualPath;
    private string[] m_allowedVirtualPaths;
    private string[] m_allowedVirtualPathPrefixes;

    public string Name => "DevOpsPortalAndTfsDeployment";

    public void Initialize(IVssRequestContext requestContext)
    {
      if (!DevOpsPortalAndTfsDeploymentHostResolver.s_allowedInstanceTypes.Contains<Guid>(requestContext.ServiceInstanceType()))
        return;
      AccessMapping accessMapping = requestContext.GetService<ILocationService>().GetAccessMapping(requestContext, AccessMappingConstants.ServicePathMappingMoniker);
      if (accessMapping == null)
        return;
      Uri uri = new Uri(accessMapping.AccessPoint);
      this.m_serviceMappingDomain = uri.Host;
      this.m_serviceMappingVirtualPath = VirtualPathUtility.RemoveTrailingSlash(uri.AbsolutePath);
      List<string> stringList = new List<string>()
      {
        this.m_serviceMappingVirtualPath
      };
      stringList.AddRange(this.Normalize(this.m_serviceMappingVirtualPath, (IEnumerable<string>) DevOpsPortalAndTfsDeploymentHostResolver.s_allowedVirtualPaths));
      this.m_allowedVirtualPaths = stringList.ToArray();
      this.m_allowedVirtualPathPrefixes = this.Normalize(this.m_serviceMappingVirtualPath, (IEnumerable<string>) DevOpsPortalAndTfsDeploymentHostResolver.s_allowedVirtualPathPrefixes).ToArray<string>();
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
      if (this.m_serviceMappingDomain == null || !StringComparer.OrdinalIgnoreCase.Equals(requestUri.Host, this.m_serviceMappingDomain) || !this.IsAllowedVirtualPath(requestUri.AbsolutePath))
      {
        routeContext = (HostRouteContext) null;
        return false;
      }
      ref HostRouteContext local = ref routeContext;
      DevOpsOrgAndCollectionHostResolver.ServicePathHostRouteContext hostRouteContext = new DevOpsOrgAndCollectionHostResolver.ServicePathHostRouteContext();
      hostRouteContext.HostId = requestContext.ServiceHost.InstanceId;
      hostRouteContext.VirtualPath = this.m_serviceMappingVirtualPath;
      hostRouteContext.WebApplicationPath = this.m_serviceMappingVirtualPath;
      hostRouteContext.RouteFlags = RouteFlags.DeploymentHost;
      local = (HostRouteContext) hostRouteContext;
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

    private IEnumerable<string> Normalize(string basePath, IEnumerable<string> paths)
    {
      foreach (string path in paths)
        yield return basePath == "/" ? path : basePath + path;
    }

    private bool IsAllowedVirtualPath(string virtualPath)
    {
      virtualPath = VirtualPathUtility.RemoveTrailingSlash(virtualPath);
      foreach (string allowedVirtualPath in this.m_allowedVirtualPaths)
      {
        if (string.Equals(virtualPath, allowedVirtualPath, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      foreach (string virtualPathPrefix in this.m_allowedVirtualPathPrefixes)
      {
        if (virtualPath.StartsWith(virtualPathPrefix, StringComparison.OrdinalIgnoreCase))
          return true;
      }
      return false;
    }
  }
}
