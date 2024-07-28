// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.RemoteServices.WorkItemArtifactUriQueryFrameworkService
// Assembly: Microsoft.Azure.Devops.Work.RemoteServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C97796CA-4166-42B2-B96F-9A166B07FF72
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Devops.Work.RemoteServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Devops.Work.RemoteServices
{
  public class WorkItemArtifactUriQueryFrameworkService : 
    IWorkItemArtifactUriQueryRemotableService,
    IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public ArtifactUriQueryResult QueryWorkItemsForArtifactUris(
      IVssRequestContext requestContext,
      ArtifactUriQuery artifactUriQuery,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<ArtifactUriQuery>(artifactUriQuery, nameof (artifactUriQuery));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(artifactUriQuery.ArtifactUris, "ArtifactUris");
      return requestContext.TraceBlock<ArtifactUriQueryResult>(919600, 919601, 919602, "FrameworkServices", "WorkItemFrameworkService", nameof (QueryWorkItemsForArtifactUris), (Func<ArtifactUriQueryResult>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().QueryWorkItemsForArtifactUrisAsync(artifactUriQuery, projectId.ToString()).Result));
    }

    public ArtifactUriQueryResult QueryWorkItemsForArtifactUris(
      IVssRequestContext requestContext,
      ArtifactUriQuery artifactUriQuery,
      string projectName)
    {
      ArgumentUtility.CheckForNull<ArtifactUriQuery>(artifactUriQuery, nameof (artifactUriQuery));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(artifactUriQuery.ArtifactUris, "ArtifactUris");
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      return requestContext.TraceBlock<ArtifactUriQueryResult>(919600, 919601, 919602, "FrameworkServices", "WorkItemFrameworkService", nameof (QueryWorkItemsForArtifactUris), (Func<ArtifactUriQueryResult>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().QueryWorkItemsForArtifactUrisAsync(artifactUriQuery, projectName).Result));
    }

    public ArtifactUriQueryResult QueryWorkItemsForArtifactUris(
      IVssRequestContext requestContext,
      ArtifactUriQuery artifactUriQuery)
    {
      ArgumentUtility.CheckForNull<ArtifactUriQuery>(artifactUriQuery, nameof (artifactUriQuery));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(artifactUriQuery.ArtifactUris, "ArtifactUris");
      return requestContext.TraceBlock<ArtifactUriQueryResult>(919600, 919601, 919602, "FrameworkServices", "WorkItemFrameworkService", nameof (QueryWorkItemsForArtifactUris), (Func<ArtifactUriQueryResult>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().QueryWorkItemsForArtifactUrisAsync(artifactUriQuery).Result));
    }
  }
}
