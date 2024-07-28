// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IWorkItemServiceHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface IWorkItemServiceHelper
  {
    IList<ProjectWorkItemStateColors> GetWorkItemStateColors(string[] projectNames);

    IList<WorkItem> GetWorkItems(
      IList<int> ids,
      IList<string> fields,
      WorkItemErrorPolicy errorPolicy);

    IList<WorkItem> GetWorkItems(
      Guid projectId,
      IList<int> ids,
      IList<string> fields,
      WorkItemExpand expand,
      WorkItemErrorPolicy errorPolicy);

    IList<WorkItem> GetWorkItems(
      IList<int> ids,
      WorkItemExpand expand,
      WorkItemErrorPolicy errorPolicy);

    WorkItemQueryResult QueryByWiql(Guid projectId, string wiqlQuery, int? top = null);

    WorkItemQueryResult QueryByWiql(string wiqlQuery);

    void LinkArtifactToWorkItems(
      string artifactUri,
      Dictionary<string, object> attributes,
      IList<WorkItem> workItems);

    void LinkArtifactsToWorkItem(
      List<string> artifactUris,
      Dictionary<string, object> attributes,
      int workItemId);

    void UnLinkArtifactsFromWorkItem(List<int> deletedIndices, int workItemId);

    ArtifactUriQueryResult GetLinkedWorkItemIds(IList<string> artifactUris);

    ArtifactUriQueryResult GetLinkedWorkItemIds(Guid projectId, IList<string> artifactUris);

    ArtifactUriQueryResult GetLinkedWorkItemIdsReadReplica(
      Guid projectId,
      IList<string> artifactUris);

    WorkItemTypeCategory GetWorkItemTypeCategory(Guid projectId, string category);

    Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference GetWorkItemRepresentation(
      int workItemId);

    Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference GetWorkItemShallowReference(
      int workItemId,
      string title);

    TeamFieldValues GetTeamFieldValues(TeamContext teamContext);

    WorkItemField GetField(Guid projectId, string fieldRefName);
  }
}
