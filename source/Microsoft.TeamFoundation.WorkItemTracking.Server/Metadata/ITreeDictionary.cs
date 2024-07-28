// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ITreeDictionary
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public interface ITreeDictionary
  {
    TreeNode GetTreeNode(Guid projectId, Guid nodeId);

    bool TryGetTreeNode(Guid projectId, Guid nodeId, out TreeNode node);

    int GetTreeNodeCount(Guid projectId, TreeStructureType type);

    TreeNode GetTreeNode(Guid projectId, int nodeId);

    bool TryGetTreeNode(Guid projectId, int nodeId, out TreeNode node);

    TreeNode GetTreeNode(Guid projectId, string relativePath, TreeStructureType type);

    bool TryGetTreeNode(
      Guid projectId,
      string relativePath,
      TreeStructureType type,
      out TreeNode node);

    IEnumerable<TreeNode> GetRootTreeNodes(Guid projectId);

    bool TryGetRootTreeNodes(Guid projectId, out IEnumerable<TreeNode> nodes);

    bool TryGetNodeFromPath(
      IVssRequestContext requestContext,
      string absolutePath,
      TreeStructureType type,
      out TreeNode treeNode);

    bool TryFindProjectId(int nodeId, out Guid projectId);

    [EditorBrowsable(EditorBrowsableState.Never)]
    TreeNode LegacyGetTreeNode(Guid nodeId);

    [EditorBrowsable(EditorBrowsableState.Never)]
    bool LegacyTryGetTreeNode(Guid nodeId, out TreeNode node);

    [EditorBrowsable(EditorBrowsableState.Never)]
    TreeNode LegacyGetTreeNode(int nodeId);

    [EditorBrowsable(EditorBrowsableState.Never)]
    bool LegacyTryGetTreeNode(int nodeId, out TreeNode node);

    [EditorBrowsable(EditorBrowsableState.Never)]
    int LegacyGetTreeNodeIdFromPath(
      IVssRequestContext requestContext,
      string absolutePath,
      TreeStructureType type);
  }
}
