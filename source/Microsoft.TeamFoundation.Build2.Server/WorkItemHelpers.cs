// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.WorkItemHelpers
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class WorkItemHelpers
  {
    private static readonly LinkFilter[] s_queryLinkFilter = new LinkFilter[1]
    {
      new LinkFilter()
      {
        FilterType = FilterType.ToolType,
        FilterValues = new string[1]{ "WorkItemTracking" }
      }
    };

    internal static IReadOnlyCollection<int> QueryWorkItemIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Uri> artifactUris,
      int maxItems)
    {
      using (requestContext.TraceScope("Service", nameof (QueryWorkItemIds)))
      {
        IWorkItemArtifactUriQueryRemotableService service = requestContext.GetService<IWorkItemArtifactUriQueryRemotableService>();
        ArtifactUriQuery artifactUriQuery = new ArtifactUriQuery()
        {
          ArtifactUris = (IEnumerable<string>) artifactUris.Select<Uri, string>((Func<Uri, string>) (uri => uri.ToString())).ToList<string>()
        };
        return (IReadOnlyCollection<int>) new HashSet<int>((IEnumerable<int>) (projectId == Guid.Empty ? service.QueryWorkItemsForArtifactUris(requestContext, artifactUriQuery).ArtifactUrisQueryResult : service.QueryWorkItemsForArtifactUris(requestContext, artifactUriQuery, projectId).ArtifactUrisQueryResult).Values.SelectMany<IEnumerable<WorkItemReference>, int>((Func<IEnumerable<WorkItemReference>, IEnumerable<int>>) (resultsList => resultsList.Select<WorkItemReference, int>((Func<WorkItemReference, int>) (wi => wi.Id)))).ToList<int>()).Take<int>(maxItems).ToList<int>();
      }
    }

    internal static IEnumerable<ResourceRef> ToWorkItemResourceRefs(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      ISecuredObject securedObject)
    {
      using (requestContext.TraceScope("Service", nameof (ToWorkItemResourceRefs)))
      {
        ILocationService locationService = requestContext.GetService<ILocationService>();
        return !workItemIds.Any<int>() ? (IEnumerable<ResourceRef>) new List<ResourceRef>() : workItemIds.Select<int, ResourceRef>((Func<int, ResourceRef>) (x => new ResourceRef(securedObject)
        {
          Id = x.ToString((IFormatProvider) CultureInfo.InvariantCulture),
          Url = WITHelper.GetWorkItemUrlById(requestContext, x, locationService)
        }));
      }
    }
  }
}
