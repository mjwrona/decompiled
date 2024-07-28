// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.CssNodes.ICommonStructureServiceProvider
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Microsoft.Azure.Boards.CssNodes
{
  [InheritedExport]
  public interface ICommonStructureServiceProvider
  {
    CommonStructureNodeInfo GetNode(IVssRequestContext requestContext, Guid nodeId);

    CommonStructureNodeInfo GetNode(IVssRequestContext requestContext, string nodePath);

    CommonStructureNodeInfo GetNodeWithDescendants(
      IVssRequestContext requestContext,
      Guid nodeId,
      out Dictionary<string, List<CommonStructureNodeInfo>> parents);

    CommonStructureNodeInfo[] GetRootNodes(IVssRequestContext requestContext, Guid projectId);

    bool TryGetNodeSecurityToken(
      IVssRequestContext requestContext,
      Guid nodeId,
      out string securityToken);

    List<ChangedNodeInfo> GetChangedNode(
      IVssRequestContext requestContext,
      int startSequenceId,
      bool includeDeletedProject,
      out int endSequenceId,
      out List<ChangedProjectInfo> deletedProjects);

    List<DeletedNodeInfo> GetDeletedNodes(
      IVssRequestContext requestContext,
      Guid projectId,
      DateTime since);

    void CreateNodes(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<CommonStructureNode> nodes);

    void DeleteNodes(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Guid> nodeIds,
      Guid reclassifyNodeId);

    void RenameNode(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid nodeId,
      string newName);

    void MoveNode(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid nodeId,
      Guid newParentId);

    void UpdateIterationDates(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid nodeId,
      DateTime? newStartDate,
      DateTime? newFinishDate);
  }
}
