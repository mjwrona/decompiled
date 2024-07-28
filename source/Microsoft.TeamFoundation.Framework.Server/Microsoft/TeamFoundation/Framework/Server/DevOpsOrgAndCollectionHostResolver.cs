// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DevOpsOrgAndCollectionHostResolver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.NameResolution;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [ExtensionPriority(100)]
  [ExtensionStrategy("Hosted")]
  internal class DevOpsOrgAndCollectionHostResolver : IUrlHostResolver
  {
    private string m_serviceMappingDomain;
    private string m_serviceMappingVirtualPath;
    private bool m_resolvesCollections;
    private bool m_resolvesOrgs;

    public string Name => "DevOpsOrgAndCollection";

    public void Initialize(IVssRequestContext requestContext)
    {
      AccessMapping accessMapping = requestContext.GetService<ILocationService>().GetAccessMapping(requestContext, AccessMappingConstants.ServicePathMappingMoniker);
      if (accessMapping == null)
        return;
      Uri uri = new Uri(accessMapping.AccessPoint);
      this.m_serviceMappingDomain = uri.Host;
      this.m_serviceMappingVirtualPath = uri.AbsolutePath;
      if (UrlHostResolutionService.ApplicationVirtualPath.Length > 1 && !this.m_serviceMappingVirtualPath.StartsWith(UrlHostResolutionService.ApplicationVirtualPath, StringComparison.OrdinalIgnoreCase))
        this.m_serviceMappingVirtualPath = VirtualPathUtility.RemoveTrailingSlash(UrlHostResolutionService.ApplicationVirtualPath + this.m_serviceMappingVirtualPath);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      this.m_resolvesCollections = service.GetValue<bool>(requestContext, (RegistryQuery) "/Configuration/UrlHostResolution/PathRouting/ResolvesCollections", false, false);
      this.m_resolvesOrgs = service.GetValue<bool>(requestContext, (RegistryQuery) "/Configuration/UrlHostResolution/PathRouting/ResolvesOrganizations", false, false);
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
      if (this.m_serviceMappingDomain == null || !StringComparer.Ordinal.Equals(host, this.m_serviceMappingDomain))
      {
        routeContext = (HostRouteContext) null;
        return false;
      }
      string absoluteVirtualPath;
      string firstPathSegment1 = UrlHostResolutionHelper.GetFirstPathSegment(requestContext, requestUri, this.m_serviceMappingVirtualPath, out absoluteVirtualPath);
      if ("e".Equals(firstPathSegment1, StringComparison.OrdinalIgnoreCase))
      {
        string firstPathSegment2 = UrlHostResolutionHelper.GetFirstPathSegment(requestContext, requestUri, absoluteVirtualPath, out absoluteVirtualPath);
        routeContext = this.ResolveOrganization(requestContext, host, firstPathSegment2, absoluteVirtualPath);
        return true;
      }
      if (this.m_resolvesCollections && !string.IsNullOrWhiteSpace(firstPathSegment1) && !DevOpsS2SAndDeploymentHostResolver.IsReservedFirstPathSegment(firstPathSegment1))
      {
        routeContext = this.ResolveCollection(requestContext, host, firstPathSegment1, absoluteVirtualPath);
        return true;
      }
      routeContext = (HostRouteContext) null;
      return false;
    }

    private HostRouteContext ResolveOrganization(
      IVssRequestContext requestContext,
      string domain,
      string organizationName,
      string absoluteVirtualPath)
    {
      if (!this.m_resolvesOrgs || string.IsNullOrEmpty(organizationName))
        return (HostRouteContext) null;
      NameResolutionEntry nameResolutionEntry = requestContext.GetService<INameResolutionService>().QueryEntry(requestContext, "Organization", organizationName);
      if (nameResolutionEntry == null || !nameResolutionEntry.IsEnabled)
        return (HostRouteContext) null;
      DevOpsOrgAndCollectionHostResolver.ServicePathHostRouteContext hostRouteContext = new DevOpsOrgAndCollectionHostResolver.ServicePathHostRouteContext();
      hostRouteContext.HostId = nameResolutionEntry.Value;
      hostRouteContext.VirtualPath = absoluteVirtualPath;
      hostRouteContext.WebApplicationPath = absoluteVirtualPath;
      hostRouteContext.RouteFlags = RouteFlags.OrganizationHost;
      hostRouteContext.RouteKeyVersion = new int?(2);
      hostRouteContext.ExpectedRouteKey = domain + "/e/" + organizationName;
      return (HostRouteContext) hostRouteContext;
    }

    private HostRouteContext ResolveCollection(
      IVssRequestContext requestContext,
      string domain,
      string collectionName,
      string absoluteVirtualPath)
    {
      if (!this.m_resolvesCollections)
        return (HostRouteContext) null;
      NameResolutionEntry nameResolutionEntry = requestContext.GetService<INameResolutionService>().QueryEntry(requestContext, "GlobalCollection", collectionName);
      if (nameResolutionEntry == null || !nameResolutionEntry.IsEnabled)
        return (HostRouteContext) null;
      DevOpsOrgAndCollectionHostResolver.ServicePathHostRouteContext hostRouteContext = new DevOpsOrgAndCollectionHostResolver.ServicePathHostRouteContext();
      hostRouteContext.HostId = nameResolutionEntry.Value;
      hostRouteContext.VirtualPath = absoluteVirtualPath;
      hostRouteContext.WebApplicationPath = absoluteVirtualPath;
      hostRouteContext.RouteFlags = RouteFlags.CollectionHost;
      hostRouteContext.RouteKeyVersion = new int?(2);
      hostRouteContext.ExpectedRouteKey = domain + "/" + collectionName;
      return (HostRouteContext) hostRouteContext;
    }

    public bool TryResolveUriData(
      IVssRequestContext requestContext,
      Guid hostId,
      out IHostUriData uriData)
    {
      if (string.IsNullOrEmpty(this.m_serviceMappingDomain))
      {
        uriData = (IHostUriData) null;
        return false;
      }
      NameResolutionEntry primaryEntryForValue = requestContext.GetService<INameResolutionService>().GetPrimaryEntryForValue(requestContext, hostId);
      if (primaryEntryForValue != null && primaryEntryForValue.IsEnabled)
      {
        if (primaryEntryForValue.Namespace == "GlobalCollection")
        {
          uriData = (IHostUriData) new DevOpsCollectionHostUriData(primaryEntryForValue.Name);
          return true;
        }
        if (primaryEntryForValue.Namespace == "Organization")
        {
          uriData = (IHostUriData) new DevOpsOrganizationHostUriData(primaryEntryForValue.Name);
          return true;
        }
      }
      uriData = (IHostUriData) null;
      return false;
    }

    internal static AccessMapping GetAccessMapping(
      IVssRequestContext requestContext,
      IList<NameResolutionEntry> entries)
    {
      HostUriData hostUriData = (HostUriData) null;
      NameResolutionEntry nameResolutionEntry1 = entries.FirstOrDefault<NameResolutionEntry>((Func<NameResolutionEntry, bool>) (x => x.Namespace == "GlobalCollection" && x.IsEnabled));
      if (nameResolutionEntry1 != null)
      {
        hostUriData = (HostUriData) new DevOpsCollectionHostUriData(nameResolutionEntry1.Name);
      }
      else
      {
        NameResolutionEntry nameResolutionEntry2 = entries.FirstOrDefault<NameResolutionEntry>((Func<NameResolutionEntry, bool>) (x => x.Namespace == "Organization" && x.IsEnabled));
        if (nameResolutionEntry2 != null)
          hostUriData = (HostUriData) new DevOpsOrganizationHostUriData(nameResolutionEntry2.Name);
      }
      return hostUriData?.BuildAccessMapping(requestContext, AccessMappingConstants.DevOpsAccessMapping, "Codex Access Mapping", false);
    }

    internal class ServicePathHostRouteContext : HostRouteContext
    {
      private static readonly string c_accessMappingMonikers = AccessMappingConstants.DevOpsAccessMapping + ";" + AccessMappingConstants.ServicePathMappingMoniker;

      public ServicePathHostRouteContext() => this.AccessMappingMonikers = DevOpsOrgAndCollectionHostResolver.ServicePathHostRouteContext.c_accessMappingMonikers;
    }
  }
}
