// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.ArtifactLinkFactories.ArtifactUriQueryResultFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters.ArtifactLinkFactories
{
  internal static class ArtifactUriQueryResultFactory
  {
    public static Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.ArtifactUriQueryResult Create(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.ArtifactUriQueryResult> serverResults,
      Guid? filterUnderProjectId = null)
    {
      Guid? projectIdInUrl = CommonWITUtils.HasCrossProjectQueryArtifactUriPermission(witRequestContext.RequestContext) ? new Guid?() : filterUnderProjectId;
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.ArtifactUriQueryResult artifactUriQueryResult = new Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.ArtifactUriQueryResult((ISecuredObject) new ArtifactQueryDualIntentSecuredObject(projectIdInUrl));
      artifactUriQueryResult.ArtifactUrisQueryResult = (IDictionary<string, IEnumerable<WorkItemReference>>) new Dictionary<string, IEnumerable<WorkItemReference>>();
      foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.ArtifactUriQueryResult serverResult in serverResults)
      {
        Microsoft.TeamFoundation.WorkItemTracking.Server.ArtifactUriQueryResult artifactResult = serverResult;
        artifactUriQueryResult.ArtifactUrisQueryResult[artifactResult.ArtifactUri] = (IEnumerable<WorkItemReference>) artifactResult.WorkItemIds.Select<int, WorkItemReference>((Func<int, WorkItemReference>) (id => WorkItemReferenceFactory.Create(witRequestContext, id, projectIdInUrl, artifactResult.GetTokenWorkItem(id)))).ToList<WorkItemReference>();
      }
      return artifactUriQueryResult;
    }
  }
}
