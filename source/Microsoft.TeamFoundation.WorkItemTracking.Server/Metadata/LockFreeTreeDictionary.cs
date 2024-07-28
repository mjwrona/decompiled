// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.LockFreeTreeDictionary
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class LockFreeTreeDictionary : ITreeDictionary
  {
    private IVssRequestContext m_requestContext;
    private Lazy<WorkItemTrackingTreeService> m_lazyTreeDictionary;
    private ITreeDictionary m_snapshot;

    public LockFreeTreeDictionary(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.m_lazyTreeDictionary = new Lazy<WorkItemTrackingTreeService>((Func<WorkItemTrackingTreeService>) (() => this.m_requestContext.GetService<WorkItemTrackingTreeService>()));
    }

    public int GetTreeNodeCount(Guid projectId, TreeStructureType type) => throw new NotImplementedException();

    public TreeNode GetTreeNode(Guid projectId, Guid nodeId)
    {
      TreeNode node;
      if (!this.TryGetTreeNode(projectId, nodeId, out node))
        throw new WorkItemTrackingTreeNodeNotFoundException(nodeId);
      return node;
    }

    public virtual bool TryGetTreeNode(Guid projectId, Guid nodeId, out TreeNode node)
    {
      if (this.CurrentSnapshot.TryGetTreeNode(projectId, nodeId, out node))
        return true;
      this.RefreshSnapshot();
      return this.CurrentSnapshot.TryGetTreeNode(projectId, nodeId, out node);
    }

    public TreeNode GetTreeNode(Guid projectId, int nodeId)
    {
      TreeNode node;
      if (!this.TryGetTreeNode(projectId, nodeId, out node))
        throw new WorkItemTrackingTreeNodeNotFoundException(nodeId);
      return node;
    }

    public bool TryGetTreeNode(Guid projectId, int nodeId, out TreeNode node)
    {
      if (this.CurrentSnapshot.TryGetTreeNode(projectId, nodeId, out node))
        return true;
      this.RefreshSnapshot();
      return this.CurrentSnapshot.TryGetTreeNode(projectId, nodeId, out node);
    }

    public bool TryGetTreeNode(
      Guid projectId,
      string relativePath,
      TreeStructureType type,
      out TreeNode node)
    {
      if (this.CurrentSnapshot.TryGetTreeNode(projectId, relativePath, type, out node))
        return true;
      this.RefreshSnapshot();
      return this.CurrentSnapshot.TryGetTreeNode(projectId, relativePath, type, out node);
    }

    public TreeNode GetTreeNode(Guid projectId, string relativePath, TreeStructureType type)
    {
      TreeNode node;
      if (!this.TryGetTreeNode(projectId, relativePath, type, out node))
        throw new WorkItemTrackingTreeNodeNotFoundException(relativePath);
      return node;
    }

    public IEnumerable<TreeNode> GetRootTreeNodes(Guid projectId)
    {
      IEnumerable<TreeNode> nodes;
      if (!this.TryGetRootTreeNodes(projectId, out nodes))
        throw new WorkItemTrackingTreeNodeNotFoundException(projectId);
      return nodes;
    }

    public bool TryGetRootTreeNodes(Guid projectId, out IEnumerable<TreeNode> nodes)
    {
      if (this.CurrentSnapshot.TryGetRootTreeNodes(projectId, out nodes))
        return true;
      this.RefreshSnapshot();
      return this.CurrentSnapshot.TryGetRootTreeNodes(projectId, out nodes);
    }

    public bool TryFindProjectId(int nodeId, out Guid projectId)
    {
      if (!this.CurrentSnapshot.TryFindProjectId(nodeId, out projectId))
      {
        this.RefreshSnapshot();
        if (!this.CurrentSnapshot.TryFindProjectId(nodeId, out projectId))
          return false;
      }
      return true;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public TreeNode LegacyGetTreeNode(Guid nodeId)
    {
      TreeNode node;
      if (!this.LegacyTryGetTreeNode(nodeId, out node))
        throw new WorkItemTrackingTreeNodeNotFoundException(nodeId);
      return node;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool LegacyTryGetTreeNode(Guid nodeId, out TreeNode node)
    {
      if (this.CurrentSnapshot.LegacyTryGetTreeNode(nodeId, out node))
        return true;
      this.RefreshSnapshot();
      return this.CurrentSnapshot.LegacyTryGetTreeNode(nodeId, out node);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public TreeNode LegacyGetTreeNode(int nodeId)
    {
      TreeNode node;
      if (!this.LegacyTryGetTreeNode(nodeId, out node))
        throw new WorkItemTrackingTreeNodeNotFoundException(nodeId);
      return node;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual bool LegacyTryGetTreeNode(int nodeId, out TreeNode node)
    {
      if (this.CurrentSnapshot.LegacyTryGetTreeNode(nodeId, out node))
        return true;
      this.RefreshSnapshot();
      return this.CurrentSnapshot.LegacyTryGetTreeNode(nodeId, out node);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public int LegacyGetTreeNodeIdFromPath(
      IVssRequestContext requestContext,
      string absolutePath,
      TreeStructureType type)
    {
      int treeNodeIdFromPath = this.CurrentSnapshot.LegacyGetTreeNodeIdFromPath(requestContext, absolutePath, type);
      if (treeNodeIdFromPath > 0)
        return treeNodeIdFromPath;
      this.RefreshSnapshot();
      return this.CurrentSnapshot.LegacyGetTreeNodeIdFromPath(requestContext, absolutePath, type);
    }

    private void RefreshSnapshot() => this.m_snapshot = this.m_lazyTreeDictionary.Value.GetSnapshot(this.m_requestContext);

    public bool TryGetNodeFromPath(
      IVssRequestContext requestContext,
      string absolutePath,
      TreeStructureType type,
      out TreeNode node)
    {
      if (this.CurrentSnapshot.TryGetNodeFromPath(requestContext, absolutePath, type, out node))
        return true;
      this.RefreshSnapshot();
      return this.CurrentSnapshot.TryGetNodeFromPath(requestContext, absolutePath, type, out node);
    }

    private ITreeDictionary CurrentSnapshot
    {
      get
      {
        if (this.m_snapshot == null)
          this.m_snapshot = this.m_lazyTreeDictionary.Value.GetSnapshot(this.m_requestContext);
        return this.m_snapshot;
      }
    }

    private TreeStructureType ResolveStructureType(
      CommonStructureNodeType type,
      ITreeDictionary snapshot,
      Guid projectId,
      Guid parentId)
    {
      if (type == CommonStructureNodeType.Area)
        return TreeStructureType.Area;
      if (type == CommonStructureNodeType.Iteration)
        return TreeStructureType.Iteration;
      TreeNode node;
      return snapshot.TryGetTreeNode(projectId, parentId, out node) ? node.Type : TreeStructureType.None;
    }
  }
}
