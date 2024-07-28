// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.RemoteServices.ClassificationNodesFrameworkService
// Assembly: Microsoft.Azure.Devops.Work.RemoteServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C97796CA-4166-42B2-B96F-9A166B07FF72
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Devops.Work.RemoteServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Azure.Devops.Work.RemoteServices
{
  internal class ClassificationNodesFrameworkService : 
    IClassificationNodesRemotableService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<WorkItemClassificationNode> GetRootNodes(
      IVssRequestContext requestContext,
      string projectName,
      int? depth = 0)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      return (IEnumerable<WorkItemClassificationNode>) requestContext.TraceBlock<List<WorkItemClassificationNode>>(919600, 919601, 919602, "FrameworkServices", "WorkItemFrameworkService", nameof (GetRootNodes), (Func<List<WorkItemClassificationNode>>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().GetRootNodesAsync(projectName, depth).Result));
    }

    public IEnumerable<WorkItemClassificationNode> GetRootNodes(
      IVssRequestContext requestContext,
      Guid projectId,
      int? depth = 0)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return (IEnumerable<WorkItemClassificationNode>) requestContext.TraceBlock<List<WorkItemClassificationNode>>(919600, 919601, 919602, "FrameworkServices", "WorkItemFrameworkService", nameof (GetRootNodes), (Func<List<WorkItemClassificationNode>>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().GetRootNodesAsync(projectId, depth).Result));
    }

    public WorkItemClassificationNode GetClassificationNode(
      IVssRequestContext requestContext,
      Guid ProjectId,
      TreeStructureGroup treeStructureGroup,
      string path = null,
      int depth = 0)
    {
      ArgumentUtility.CheckForEmptyGuid(ProjectId, nameof (ProjectId));
      return requestContext.TraceBlock<WorkItemClassificationNode>(919600, 919601, 919602, "FrameworkServices", "WorkItemFrameworkService", nameof (GetClassificationNode), (Func<WorkItemClassificationNode>) (() =>
      {
        WorkItemTrackingHttpClient client = requestContext.GetClient<WorkItemTrackingHttpClient>();
        Guid project = ProjectId;
        int? nullable = new int?(depth);
        int structureGroup = (int) treeStructureGroup;
        string path1 = path;
        int? depth1 = nullable;
        CancellationToken cancellationToken = new CancellationToken();
        return client.GetClassificationNodeAsync(project, (TreeStructureGroup) structureGroup, path1, depth1, cancellationToken: cancellationToken).Result;
      }));
    }

    public WorkItemClassificationNode GetClassificationNode(
      IVssRequestContext requestContext,
      string projectName,
      TreeStructureGroup treeStructureGroup,
      string path = null,
      int depth = 0)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      return requestContext.TraceBlock<WorkItemClassificationNode>(919600, 919601, 919602, "FrameworkServices", "WorkItemFrameworkService", nameof (GetClassificationNode), (Func<WorkItemClassificationNode>) (() =>
      {
        WorkItemTrackingHttpClient client = requestContext.GetClient<WorkItemTrackingHttpClient>();
        string project = projectName;
        int? nullable = new int?(depth);
        int structureGroup = (int) treeStructureGroup;
        string path1 = path;
        int? depth1 = nullable;
        CancellationToken cancellationToken = new CancellationToken();
        return client.GetClassificationNodeAsync(project, (TreeStructureGroup) structureGroup, path1, depth1, cancellationToken: cancellationToken).Result;
      }));
    }

    public IEnumerable<WorkItemClassificationNode> GetClassificationNodes(
      IVssRequestContext requestContext,
      string projectName,
      IEnumerable<Guid> nodeIds)
    {
      throw new NotImplementedException();
    }

    public IEnumerable<WorkItemClassificationNode> GetClassificationNodes(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Guid> nodeIds)
    {
      throw new NotImplementedException();
    }
  }
}
