// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.PlatformServices.PlatformWorkItemArtifactUriQueryService
// Assembly: Microsoft.Azure.Devops.Work.PlatformServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7C8E511A-CB9A-4327-9803-A1164853E0F0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Work.PlatformServices.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters.ArtifactLinkFactories;
using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Devops.Work.PlatformServices
{
  public class PlatformWorkItemArtifactUriQueryService : 
    IWorkItemArtifactUriQueryRemotableService,
    IVssFrameworkService
  {
    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.ArtifactUriQueryResult QueryWorkItemsForArtifactUris(
      IVssRequestContext requestContext,
      ArtifactUriQuery artifactUriQuery)
    {
      ArgumentUtility.CheckForNull<ArtifactUriQuery>(artifactUriQuery, nameof (artifactUriQuery));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(artifactUriQuery.ArtifactUris, "ArtifactUris");
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.ArtifactUriQueryResult> idsForArtifactUris = requestContext.GetService<ITeamFoundationWorkItemService>().GetWorkItemIdsForArtifactUris(requestContext, artifactUriQuery.ArtifactUris);
      return ArtifactUriQueryResultFactory.Create(requestContext.WitContext(), idsForArtifactUris);
    }

    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.ArtifactUriQueryResult QueryWorkItemsForArtifactUris(
      IVssRequestContext requestContext,
      ArtifactUriQuery artifactUriQuery,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<ArtifactUriQuery>(artifactUriQuery, nameof (artifactUriQuery));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(artifactUriQuery.ArtifactUris, "ArtifactUris");
      ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<string> artifactUris = artifactUriQuery.ArtifactUris;
      Guid? nullable = new Guid?(projectId);
      DateTime? asOfDate = new DateTime?();
      Guid? filterUnderProjectId = nullable;
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.ArtifactUriQueryResult> idsForArtifactUris = service.GetWorkItemIdsForArtifactUris(requestContext1, artifactUris, asOfDate, filterUnderProjectId);
      return ArtifactUriQueryResultFactory.Create(requestContext.WitContext(), idsForArtifactUris, new Guid?(projectId));
    }

    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.ArtifactUriQueryResult QueryWorkItemsForArtifactUris(
      IVssRequestContext requestContext,
      ArtifactUriQuery artifactUriQuery,
      string projectName)
    {
      ArgumentUtility.CheckForNull<ArtifactUriQuery>(artifactUriQuery, nameof (artifactUriQuery));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(artifactUriQuery.ArtifactUris, "ArtifactUris");
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      ITeamFoundationWorkItemService service = requestContext.GetService<ITeamFoundationWorkItemService>();
      Guid projectId = requestContext.GetService<IProjectService>().GetProjectId(requestContext, projectName);
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<string> artifactUris = artifactUriQuery.ArtifactUris;
      Guid? nullable = new Guid?(projectId);
      DateTime? asOfDate = new DateTime?();
      Guid? filterUnderProjectId = nullable;
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.ArtifactUriQueryResult> idsForArtifactUris = service.GetWorkItemIdsForArtifactUris(requestContext1, artifactUris, asOfDate, filterUnderProjectId);
      return ArtifactUriQueryResultFactory.Create(requestContext.WitContext(), idsForArtifactUris, new Guid?(projectId));
    }
  }
}
