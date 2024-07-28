// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VstsDomainHostResolver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.NameResolution;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [ExtensionPriority(300)]
  [ExtensionStrategy("Hosted")]
  internal class VstsDomainHostResolver : IUrlHostResolver
  {
    protected Uri m_rootUri;
    private VstsDomainHostResolver.DomainEndpointMapping[] m_domainEndpointMappings;

    public virtual string Name => "VstsDomain";

    public virtual void Initialize(IVssRequestContext requestContext)
    {
      UriBuilder uriBuilder = new UriBuilder((requestContext.GetService<ILocationService>().GetAccessMapping(requestContext, AccessMappingConstants.RootDomainMappingMoniker) ?? throw new AccessMappingNotRegisteredException(AccessMappingConstants.RootDomainMappingMoniker)).AccessPoint);
      if (UrlHostResolutionService.ApplicationVirtualPath.Length > 1 && !uriBuilder.Path.StartsWith(UrlHostResolutionService.ApplicationVirtualPath, StringComparison.OrdinalIgnoreCase))
        uriBuilder.Path = VirtualPathUtility.RemoveTrailingSlash(UrlHostResolutionService.ApplicationVirtualPath + uriBuilder.Path);
      this.m_rootUri = uriBuilder.Uri;
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnDomainEndpointMappingEntriesChanged), in UrlHostResolutionConstants.DomainEndpointMappingEntriesQuery);
      Interlocked.CompareExchange<VstsDomainHostResolver.DomainEndpointMapping[]>(ref this.m_domainEndpointMappings, VstsDomainHostResolver.GetDomainEndpointMappings(requestContext), (VstsDomainHostResolver.DomainEndpointMapping[]) null);
    }

    public virtual void Shutdown(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnDomainEndpointMappingEntriesChanged));

    public virtual bool TryResolveHost(
      IVssRequestContext requestContext,
      Uri requestUri,
      string applicationVirtualPath,
      out HostRouteContext routeContext)
    {
      string host = requestUri.Host;
      string targetNameFromDomain = this.GetTargetNameFromDomain(requestContext, host);
      if (targetNameFromDomain == null)
      {
        routeContext = (HostRouteContext) null;
        return false;
      }
      RouteFlags routeFlags;
      NameResolutionEntry domainNameEntry = this.GetDomainNameEntry(requestContext, targetNameFromDomain, out routeFlags);
      if (domainNameEntry == null || !domainNameEntry.IsEnabled)
      {
        routeContext = (HostRouteContext) null;
        return true;
      }
      if (domainNameEntry.Value == requestContext.ServiceHost.InstanceId)
      {
        requestContext.Trace(409291500, TraceLevel.Error, "UrlHostResolutionService", "IVssFrameworkService", "Configuration error: DomainEndpointMapping exists for '" + host + "', but there is no corresponding AccessMapping.");
        routeContext = (HostRouteContext) null;
        return true;
      }
      string absoluteVirtualPath;
      string firstPathSegment = UrlHostResolutionHelper.GetFirstPathSegment(requestContext, requestUri, this.m_rootUri.AbsolutePath, out absoluteVirtualPath);
      if (firstPathSegment != null && StringComparer.OrdinalIgnoreCase.Equals(firstPathSegment, "DefaultCollection"))
      {
        ref HostRouteContext local = ref routeContext;
        VstsDomainHostResolver.VstsDomainRouteContext domainRouteContext = new VstsDomainHostResolver.VstsDomainRouteContext();
        domainRouteContext.HostId = domainNameEntry.Value;
        domainRouteContext.VirtualPath = absoluteVirtualPath;
        domainRouteContext.WebApplicationPath = this.m_rootUri.AbsolutePath;
        domainRouteContext.RouteFlags = routeFlags;
        domainRouteContext.RouteKeyVersion = new int?(0);
        domainRouteContext.ExpectedRouteKey = host;
        local = (HostRouteContext) domainRouteContext;
        return true;
      }
      ref HostRouteContext local1 = ref routeContext;
      VstsDomainHostResolver.VstsDomainRouteContext domainRouteContext1 = new VstsDomainHostResolver.VstsDomainRouteContext();
      domainRouteContext1.HostId = domainNameEntry.Value;
      domainRouteContext1.VirtualPath = this.m_rootUri.AbsolutePath;
      domainRouteContext1.WebApplicationPath = this.m_rootUri.AbsolutePath;
      domainRouteContext1.RouteFlags = routeFlags;
      domainRouteContext1.RouteKeyVersion = new int?(0);
      domainRouteContext1.ExpectedRouteKey = host;
      local1 = (HostRouteContext) domainRouteContext1;
      return true;
    }

    public bool TryResolveUriData(
      IVssRequestContext requestContext,
      Guid hostId,
      out IHostUriData uriData)
    {
      NameResolutionEntry primaryEntryForValue = requestContext.GetService<INameResolutionService>().GetPrimaryEntryForValue(requestContext, hostId);
      if (primaryEntryForValue != null && primaryEntryForValue.IsEnabled && primaryEntryForValue.Namespace == "Collection")
      {
        uriData = (IHostUriData) new RootMappingHostUriData(primaryEntryForValue.Name);
        return true;
      }
      uriData = (IHostUriData) null;
      return false;
    }

    internal static AccessMapping GetAccessMapping(
      IVssRequestContext requestContext,
      IList<NameResolutionEntry> entries)
    {
      NameResolutionEntry nameResolutionEntry = entries.FirstOrDefault<NameResolutionEntry>((Func<NameResolutionEntry, bool>) (x => x.Namespace == "Collection" && x.IsEnabled));
      return nameResolutionEntry == null ? (AccessMapping) null : new RootMappingHostUriData(nameResolutionEntry.Name).BuildAccessMapping(requestContext, AccessMappingConstants.VstsAccessMapping, "VSTS Access Mapping", false);
    }

    private string GetTargetNameFromDomain(IVssRequestContext requestContext, string domain)
    {
      foreach (VstsDomainHostResolver.DomainEndpointMapping domainEndpointMapping in this.m_domainEndpointMappings)
      {
        if (string.Equals(domain, domainEndpointMapping.Domain, StringComparison.OrdinalIgnoreCase))
          return domainEndpointMapping.EndpointName;
      }
      int length = domain.IndexOf("." + this.m_rootUri.Host, StringComparison.OrdinalIgnoreCase);
      return length < 0 ? (string) null : domain.Substring(0, length);
    }

    private NameResolutionEntry GetDomainNameEntry(
      IVssRequestContext requestContext,
      string domainPrefix,
      out RouteFlags routeFlags)
    {
      NameResolutionEntry domainNameEntry = requestContext.GetService<IInternalNameResolutionService>().QueryFirstEntry(requestContext, (IReadOnlyList<string>) new string[2]
      {
        "Collection",
        "Deployment"
      }, domainPrefix, (Predicate<NameResolutionEntry>) (x => x.IsEnabled));
      if (domainNameEntry != null)
      {
        routeFlags = domainNameEntry.Namespace == "Collection" ? RouteFlags.CollectionHost : RouteFlags.DeploymentHost;
        return domainNameEntry;
      }
      routeFlags = RouteFlags.None;
      return (NameResolutionEntry) null;
    }

    private void OnDomainEndpointMappingEntriesChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.m_domainEndpointMappings = VstsDomainHostResolver.GetDomainEndpointMappings(requestContext);
    }

    private static VstsDomainHostResolver.DomainEndpointMapping[] GetDomainEndpointMappings(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, UrlHostResolutionConstants.DomainEndpointMappingEntriesQuery).Select<RegistryEntry, VstsDomainHostResolver.DomainEndpointMapping>((Func<RegistryEntry, VstsDomainHostResolver.DomainEndpointMapping>) (e => new VstsDomainHostResolver.DomainEndpointMapping(e.Name, e.Value))).ToArray<VstsDomainHostResolver.DomainEndpointMapping>();
    }

    private struct DomainEndpointMapping
    {
      public string Domain;
      public string EndpointName;

      public DomainEndpointMapping(string domain, string endpointName)
      {
        this.Domain = domain;
        this.EndpointName = endpointName;
      }
    }

    internal class VstsDomainRouteContext : HostRouteContext
    {
      private static readonly string c_accessMappingMonikers = AccessMappingConstants.VstsAccessMapping + ";" + AccessMappingConstants.RootDomainMappingMoniker;

      public VstsDomainRouteContext() => this.AccessMappingMonikers = VstsDomainHostResolver.VstsDomainRouteContext.c_accessMappingMonikers;
    }
  }
}
