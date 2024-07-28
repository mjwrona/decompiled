// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.LegacyTfsImplementations.LegacyTfsCssUtilsService
// Assembly: Microsoft.Tfs.WorkItemTracking.LegacyTfsImplementations, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6D9A1E77-52F6-4366-807D-D0FABA8CDE81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Tfs.WorkItemTracking.LegacyTfsImplementations.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.LegacyTfsImplementations
{
  public class LegacyTfsCssUtilsService : ILegacyCssUtilsService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public object GetTreeValues(
      IVssRequestContext requestContext,
      string projectName,
      TreeStructureType nodeType,
      out int nodeCount)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      CommonStructureNodeInfo rootNode;
      Dictionary<string, List<CommonStructureNodeInfo>> treeNodes = this.GetTreeNodes(requestContext, projectName, nodeType, out rootNode);
      nodeCount = treeNodes.Count;
      return (object) new List<object>()
      {
        this.ProcessNode(projectName, rootNode, treeNodes)
      };
    }

    public List<CommonStructureNodeInfo> GetNodes(
      IVssRequestContext requestContext,
      IEnumerable<Guid> nodeIds)
    {
      List<string> nodesUris = new List<string>();
      foreach (Guid nodeId in nodeIds)
        nodesUris.Add(CommonStructureUtils.GetNodeUri(nodeId));
      return requestContext.GetService<ICommonStructureService>().GetNodes(requestContext, nodesUris);
    }

    private CommonStructureNodeInfo GetRootNode(
      IVssRequestContext requestContext,
      string projectName,
      TreeStructureType nodeType)
    {
      string nodeTypeString = nodeType == TreeStructureType.Iteration ? "ProjectLifecycle" : "ProjectModelHierarchy";
      CommonStructureService service = requestContext.GetService<CommonStructureService>();
      string uri = service.GetProjectFromName(requestContext, projectName).Uri;
      return ((IEnumerable<CommonStructureNodeInfo>) service.GetRootNodes(requestContext, uri)).First<CommonStructureNodeInfo>((Func<CommonStructureNodeInfo, bool>) (ni => TFStringComparer.CssStructureType.Equals(ni.StructureType, nodeTypeString)));
    }

    private Dictionary<string, List<CommonStructureNodeInfo>> GetTreeNodes(
      IVssRequestContext requestContext,
      string projectName,
      TreeStructureType nodeType,
      out CommonStructureNodeInfo rootNode)
    {
      rootNode = this.GetRootNode(requestContext, projectName, nodeType);
      Dictionary<string, List<CommonStructureNodeInfo>> parents;
      requestContext.GetService<CommonStructureService>().GetNodes(requestContext, rootNode.Uri, out rootNode, out parents);
      return parents;
    }

    public object CreateNode(
      string nodeId,
      string nodeName,
      string parentId,
      ICollection<object> values,
      IEnumerable<object> children)
    {
      return (object) new
      {
        id = nodeId,
        parentId = parentId,
        text = nodeName,
        values = values,
        children = children
      };
    }

    private object ProcessNode(
      string projectName,
      CommonStructureNodeInfo node,
      Dictionary<string, List<CommonStructureNodeInfo>> parents)
    {
      node.Name = projectName;
      return this.BuildTree(node, parents);
    }

    private object BuildTree(
      CommonStructureNodeInfo node,
      Dictionary<string, List<CommonStructureNodeInfo>> parents)
    {
      ArtifactId artifactId = LinkingUtilities.DecodeUri(node.Uri);
      string parentId = (string) null;
      if (node.ParentUri != null)
        parentId = LinkingUtilities.DecodeUri(node.ParentUri).ToolSpecificId;
      List<object> values = new List<object>(3);
      values.Add((object) node.Name);
      if (TFStringComparer.CssStructureType.Equals("ProjectLifecycle", node.StructureType))
      {
        values.Add((object) node.StartDate);
        values.Add((object) node.FinishDate);
      }
      return this.CreateNode(artifactId.ToolSpecificId, node.Name, parentId, (ICollection<object>) values, parents[node.Uri].Select<CommonStructureNodeInfo, object>((Func<CommonStructureNodeInfo, object>) (n => this.BuildTree(n, parents))));
    }
  }
}
