// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.PlatformServices.PlatformClassificationNodesService
// Assembly: Microsoft.Azure.Devops.Work.PlatformServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7C8E511A-CB9A-4327-9803-A1164853E0F0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Work.PlatformServices.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.Azure.Boards.WebApi.Common.Helpers;
using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Devops.Work.PlatformServices
{
  public class PlatformClassificationNodesService : 
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
      int? depth)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      Guid projectId = requestContext.GetService<IProjectService>().GetProjectId(requestContext, projectName);
      return (IEnumerable<WorkItemClassificationNode>) requestContext.WitContext().TreeService.GetRootTreeNodes(projectId).OrderBy<TreeNode, TreeStructureType>((Func<TreeNode, TreeStructureType>) (x => x.Type)).Select<TreeNode, WorkItemClassificationNode>((Func<TreeNode, WorkItemClassificationNode>) (x => WorkItemClassificationNodeFactory.Create(requestContext, x, depth.GetValueOrDefault(), false))).ToList<WorkItemClassificationNode>();
    }

    public IEnumerable<WorkItemClassificationNode> GetRootNodes(
      IVssRequestContext requestContext,
      Guid projectId,
      int? depth)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return (IEnumerable<WorkItemClassificationNode>) requestContext.WitContext().TreeService.GetRootTreeNodes(projectId).OrderBy<TreeNode, TreeStructureType>((Func<TreeNode, TreeStructureType>) (x => x.Type)).Select<TreeNode, WorkItemClassificationNode>((Func<TreeNode, WorkItemClassificationNode>) (x => WorkItemClassificationNodeFactory.Create(requestContext, x, depth.GetValueOrDefault(), false))).ToList<WorkItemClassificationNode>();
    }

    public WorkItemClassificationNode GetClassificationNode(
      IVssRequestContext requestContext,
      Guid projectId,
      TreeStructureGroup treeStructureGroup,
      string path = null,
      int depth = 0)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      path = WorkItemClassificationNodeHelper.FixPath(path);
      TreeNode treeNode = requestContext.WitContext().TreeService.GetTreeNode(projectId, path, WorkItemClassificationNodeHelper.ToInternalTreeStructureType(treeStructureGroup));
      return WorkItemClassificationNodeFactory.Create(requestContext, treeNode, depth, true);
    }

    public WorkItemClassificationNode GetClassificationNode(
      IVssRequestContext requestContext,
      string projectName,
      TreeStructureGroup treeStructureGroup,
      string path = null,
      int depth = 0)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      Guid projectId = requestContext.GetService<IProjectService>().GetProjectId(requestContext, projectName);
      path = WorkItemClassificationNodeHelper.FixPath(path);
      TreeNode treeNode = requestContext.WitContext().TreeService.GetTreeNode(projectId, path, WorkItemClassificationNodeHelper.ToInternalTreeStructureType(treeStructureGroup));
      return WorkItemClassificationNodeFactory.Create(requestContext, treeNode, depth, true);
    }

    public IEnumerable<WorkItemClassificationNode> GetClassificationNodes(
      IVssRequestContext requestContext,
      string projectName,
      IEnumerable<Guid> nodeIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(nodeIds, nameof (nodeIds));
      Guid projectId = requestContext.GetService<IProjectService>().GetProjectId(requestContext, projectName);
      List<WorkItemClassificationNode> classificationNodes = new List<WorkItemClassificationNode>();
      ITreeDictionary treeService = requestContext.WitContext().TreeService;
      foreach (Guid nodeId in nodeIds)
      {
        TreeNode treeNode = treeService.GetTreeNode(projectId, nodeId);
        classificationNodes.Add(WorkItemClassificationNodeFactory.Create(requestContext, treeNode, 0, true));
      }
      return (IEnumerable<WorkItemClassificationNode>) classificationNodes;
    }

    public IEnumerable<WorkItemClassificationNode> GetClassificationNodes(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Guid> nodeIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(nodeIds, nameof (nodeIds));
      List<WorkItemClassificationNode> classificationNodes = new List<WorkItemClassificationNode>();
      ITreeDictionary treeService = requestContext.WitContext().TreeService;
      foreach (Guid nodeId in nodeIds)
      {
        TreeNode treeNode = treeService.GetTreeNode(projectId, nodeId);
        classificationNodes.Add(WorkItemClassificationNodeFactory.Create(requestContext, treeNode, 0, true));
      }
      return (IEnumerable<WorkItemClassificationNode>) classificationNodes;
    }
  }
}
