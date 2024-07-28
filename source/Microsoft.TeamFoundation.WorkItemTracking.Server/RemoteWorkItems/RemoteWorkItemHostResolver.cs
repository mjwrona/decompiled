// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.RemoteWorkItems.RemoteWorkItemHostResolver
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.NameResolution;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.RemoteWorkItems
{
  public static class RemoteWorkItemHostResolver
  {
    public static string GetRemoteHostUrl(IVssRequestContext requestContext, Guid remoteHostId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Uri hostUri = vssRequestContext.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext, remoteHostId, ServiceInstanceTypes.TFS);
      if (!(hostUri != (Uri) null))
        return (string) null;
      return hostUri.AbsoluteUri.TrimEnd('/');
    }

    public static IDictionary<Guid, RemoteHostContext> GetRemoteHostUrls(
      IVssRequestContext requestContext,
      IEnumerable<Guid> remoteHostIds,
      bool resolveHostName = false)
    {
      Dictionary<Guid, RemoteHostContext> remoteHostUrls = new Dictionary<Guid, RemoteHostContext>();
      if (remoteHostIds.Any<Guid>())
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IUrlHostResolutionService service = vssRequestContext.GetService<IUrlHostResolutionService>();
        INameResolutionService resolutionService = (INameResolutionService) null;
        if (resolveHostName)
          resolutionService = vssRequestContext.GetService<INameResolutionService>();
        foreach (Guid guid in remoteHostIds.Distinct<Guid>())
        {
          Uri hostUri = service.GetHostUri(vssRequestContext, guid, ServiceInstanceTypes.TFS);
          if (hostUri != (Uri) null)
          {
            NameResolutionEntry primaryEntryForValue = resolutionService?.GetPrimaryEntryForValue(vssRequestContext, guid);
            remoteHostUrls.Add(guid, new RemoteHostContext()
            {
              Url = hostUri.AbsoluteUri.TrimEnd('/'),
              Name = primaryEntryForValue?.Name
            });
          }
        }
      }
      return (IDictionary<Guid, RemoteHostContext>) remoteHostUrls;
    }

    public static string GetOrganizationName(
      IVssRequestContext deploymentRequestContext,
      Guid hostId)
    {
      deploymentRequestContext.CheckServiceHostType(TeamFoundationHostType.Deployment);
      return deploymentRequestContext.GetService<INameResolutionService>().GetPrimaryEntryForValue(deploymentRequestContext, hostId)?.Name;
    }
  }
}
