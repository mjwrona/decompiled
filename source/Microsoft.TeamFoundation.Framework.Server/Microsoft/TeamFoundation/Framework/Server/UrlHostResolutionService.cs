// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UrlHostResolutionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.NameResolution;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Hosting;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class UrlHostResolutionService : 
    IInternalUrlHostResolutionService,
    IUrlHostResolutionService,
    IVssFrameworkService
  {
    private static string s_applicationVirtualPath;
    private IDisposableReadOnlyList<IUrlHostResolver> m_resolvers;
    private static readonly string s_area = "HostResolution";
    private static readonly string s_layer = nameof (UrlHostResolutionService);

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      if (!HostingEnvironment.IsHosted)
        UrlHostResolutionService.ApplicationVirtualPath = WebApiConfiguration.GetVirtualPathRoot(systemRequestContext);
      string strategy = systemRequestContext.ExecutionEnvironment.IsHostedDeployment ? "Hosted" : "OnPrem";
      this.m_resolvers = systemRequestContext.GetExtensions<IUrlHostResolver>(strategy: strategy);
      foreach (IUrlHostResolver resolver in (IEnumerable<IUrlHostResolver>) this.m_resolvers)
        resolver.Initialize(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_resolvers == null)
        return;
      foreach (IUrlHostResolver resolver in (IEnumerable<IUrlHostResolver>) this.m_resolvers)
        resolver.Shutdown(systemRequestContext);
      this.m_resolvers.Dispose();
      this.m_resolvers = (IDisposableReadOnlyList<IUrlHostResolver>) null;
    }

    public HostRouteContext ResolveHost(IVssRequestContext requestContext, Uri requestUri)
    {
      requestContext.CheckDeploymentRequestContext();
      foreach (IUrlHostResolver resolver in (IEnumerable<IUrlHostResolver>) this.m_resolvers)
      {
        HostRouteContext routeContext;
        if (resolver.TryResolveHost(requestContext, requestUri, UrlHostResolutionService.ApplicationVirtualPath, out routeContext))
        {
          requestContext.Trace(497985, TraceLevel.Info, UrlHostResolutionService.s_area, UrlHostResolutionService.s_layer, "Host resolved using: {0}", (object) resolver.Name);
          return routeContext;
        }
      }
      requestContext.Trace(497986, TraceLevel.Info, UrlHostResolutionService.s_area, UrlHostResolutionService.s_layer, string.Format("Host:'{0}' was not resolved by any resolver.", (object) requestUri));
      return (HostRouteContext) null;
    }

    public virtual IHostUriData ResolveUriData(IVssRequestContext requestContext, Guid hostId)
    {
      requestContext.CheckDeploymentRequestContext();
      foreach (IUrlHostResolver resolver in (IEnumerable<IUrlHostResolver>) this.m_resolvers)
      {
        IHostUriData uriData;
        if (resolver.TryResolveUriData(requestContext, hostId, out uriData))
          return uriData;
      }
      return (IHostUriData) null;
    }

    public Uri GetHostUri(IVssRequestContext requestContext, Guid hostId, Guid serviceIdentifier = default (Guid)) => this.GetHostUri(requestContext, hostId, true, serviceIdentifier);

    public Uri GetHostUri(
      IVssRequestContext requestContext,
      Guid hostId,
      bool throwOnMissingService,
      Guid serviceIdentifier = default (Guid))
    {
      return this.ResolveUriData(requestContext, hostId)?.BuildUri(requestContext, serviceIdentifier, throwOnMissingService);
    }

    public void AddEntriesForHost(IVssRequestContext requestContext, HostProperties hostProperties)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && hostProperties.HostType != TeamFoundationHostType.Application && ServiceHostNameHelper.IsPGuid(hostProperties.Name))
        return;
      IList<NameResolutionEntry> entriesForHost = this.ComputeEntriesForHost(requestContext, hostProperties);
      if (entriesForHost.Count > 0)
        requestContext.GetService<NameResolutionStore>().SetEntries(requestContext, (IEnumerable<NameResolutionEntry>) entriesForHost, false);
      else
        requestContext.Trace(497985, TraceLevel.Error, UrlHostResolutionService.s_area, UrlHostResolutionService.s_layer, string.Format("No name resolution records found for host {0}", (object) hostProperties));
    }

    public void RemoveEntriesForHost(IVssRequestContext requestContext, Guid hostId)
    {
      NameResolutionStore service = requestContext.GetService<NameResolutionStore>();
      IList<NameResolutionEntry> entries = service.QueryEntriesForValue(requestContext, hostId);
      if (entries.Count <= 0)
        return;
      service.DeleteEntries(requestContext, (IEnumerable<NameResolutionEntry>) entries);
    }

    internal IList<NameResolutionEntry> ComputeEntriesForHost(
      IVssRequestContext requestContext,
      HostProperties hostProperties)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        if (hostProperties.HostType != TeamFoundationHostType.ProjectCollection)
          return (IList<NameResolutionEntry>) Array.Empty<NameResolutionEntry>();
        return (IList<NameResolutionEntry>) new NameResolutionEntry[1]
        {
          new NameResolutionEntry()
          {
            Namespace = "Collection",
            Name = "/" + hostProperties.Name + "/",
            Value = hostProperties.Id,
            IsPrimary = true,
            IsEnabled = true
          }
        };
      }
      IList<NameResolutionEntry> source = requestContext.GetService<IInternalNameResolutionService>().QueryEntriesForValue(requestContext, hostProperties.Id, QueryOptions.None);
      if (source != null && source.Count > 0)
      {
        source = (IList<NameResolutionEntry>) source.Where<NameResolutionEntry>((Func<NameResolutionEntry, bool>) (x => UrlHostResolutionService.IsServiceHostNamespace(x.Namespace))).ToList<NameResolutionEntry>();
        foreach (NameResolutionEntry nameResolutionEntry in (IEnumerable<NameResolutionEntry>) source)
        {
          if (VssStringComparer.Hostname.Equals(nameResolutionEntry.Name, hostProperties.Name))
            nameResolutionEntry.ExpiresOn = new DateTime?();
        }
      }
      return source;
    }

    private static bool IsServiceHostNamespace(string @namespace) => @namespace == "Organization" || @namespace == "Collection" || @namespace == "GlobalCollection";

    string IUrlHostResolutionService.ApplicationVirtualPath
    {
      get => UrlHostResolutionService.ApplicationVirtualPath;
      set => UrlHostResolutionService.ApplicationVirtualPath = value;
    }

    internal static string ApplicationVirtualPath
    {
      get
      {
        if (UrlHostResolutionService.s_applicationVirtualPath == null)
          UrlHostResolutionService.s_applicationVirtualPath = !HostingEnvironment.IsHosted ? "/" : HostingEnvironment.ApplicationVirtualPath;
        return UrlHostResolutionService.s_applicationVirtualPath;
      }
      set => UrlHostResolutionService.s_applicationVirtualPath = value;
    }
  }
}
