// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostResolution.OnPremHostResolver
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.NameResolution;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using System;

namespace Microsoft.TeamFoundation.Framework.Server.HostResolution
{
  [ExtensionPriority(400)]
  [ExtensionStrategy("OnPrem")]
  internal class OnPremHostResolver : IUrlHostResolver
  {
    public string Name => "OnPrem";

    public void Initialize(IVssRequestContext requestContext)
    {
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
      if (!requestContext.ServiceHost.HasDatabaseAccess)
      {
        routeContext = new HostRouteContext()
        {
          HostId = requestContext.ServiceHost.InstanceId,
          VirtualPath = applicationVirtualPath,
          RouteFlags = RouteFlags.DeploymentHost
        };
        return true;
      }
      string absoluteVirtualPath;
      string firstPathSegment = UrlHostResolutionHelper.GetFirstPathSegment(requestContext, requestUri, applicationVirtualPath, out absoluteVirtualPath);
      NameResolutionEntry nameResolutionEntry = (NameResolutionEntry) null;
      if (firstPathSegment != null)
        nameResolutionEntry = requestContext.GetService<INameResolutionService>().QueryEntry(requestContext, "Collection", "/" + firstPathSegment + "/");
      if (nameResolutionEntry != null && nameResolutionEntry.IsEnabled)
      {
        routeContext = new HostRouteContext()
        {
          HostId = nameResolutionEntry.Value,
          VirtualPath = absoluteVirtualPath,
          RouteFlags = RouteFlags.CollectionHost
        };
        return true;
      }
      routeContext = new HostRouteContext()
      {
        HostId = requestContext.ServiceHost.InstanceId,
        VirtualPath = applicationVirtualPath,
        RouteFlags = RouteFlags.DeploymentHost
      };
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
        uriData = (IHostUriData) new HostUriData((string) null, primaryEntryForValue.Name, AccessMappingConstants.PublicAccessMappingMoniker);
        return true;
      }
      uriData = (IHostUriData) null;
      return false;
    }
  }
}
