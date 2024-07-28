// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.ICommonStructureService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Microsoft.TeamFoundation.Integration.Server
{
  [DefaultServiceImplementation(typeof (CommonStructureService))]
  public interface ICommonStructureService : IVssFrameworkService
  {
    CatalogNode QueryProjectCatalogNode(IVssRequestContext requestContext, string projectUri);

    CommonStructureProjectInfo[] GetProjects(IVssRequestContext requestContext);

    CommonStructureProjectInfo[] GetWellFormedProjects(IVssRequestContext requestContext);

    CommonStructureProjectInfo GetProject(IVssRequestContext requestContext, string projectUri);

    CommonStructureProjectInfo GetProject(IVssRequestContext requestContext, Guid projectId);

    CommonStructureProjectInfo GetProjectFromName(
      IVssRequestContext requestContext,
      string projectName);

    CommonStructureProjectInfo CreateProject(
      IVssRequestContext requestContext,
      string projectName,
      XmlElement structure,
      Guid? pendingProjectGuid = null);

    CommonStructureProjectInfo CreateProject(
      IVssRequestContext requestContext,
      string projectName,
      XmlNode[] nodes,
      Guid? pendingProjectGuid = null,
      string projectDescription = null,
      ProjectVisibility projectVisibility = ProjectVisibility.Private);

    void EnsureTeamProjectsExistsInCatalog(
      IVssRequestContext requestContext,
      IEnumerable<CommonStructureProjectInfo> projects);

    void HardDeleteProject(IVssRequestContext requestContext, string projectUri);

    void TransformProjectMappingProperty(
      string xsltVersionSpecifier,
      CommonStructureProjectProperty[] properties);

    CommonStructureNodeInfo GetNode(IVssRequestContext requestContext, string nodeUri);

    List<CommonStructureNodeInfo> GetNodes(
      IVssRequestContext requestContext,
      List<string> nodesUris);

    bool TryGetNodeFromPath(
      IVssRequestContext requestContext,
      string nodePath,
      out CommonStructureNodeInfo node);

    CommonStructureNodeInfo GetNodeFromPath(IVssRequestContext requestContext, string nodePath);

    XmlElement GetNodesXml(
      IVssRequestContext requestContext,
      string[] nodeUris,
      bool includeChildren);

    void GetNodes(
      IVssRequestContext requestContext,
      string nodeUri,
      out CommonStructureNodeInfo nodeInfo,
      out Dictionary<string, List<CommonStructureNodeInfo>> parents);

    CommonStructureNodeInfo[] GetRootNodes(IVssRequestContext requestContext, string projectUri);

    XmlElement GetDeletedNodes(
      IVssRequestContext requestContext,
      string projectUri,
      DateTime since);

    string GetChangedNodes(IVssRequestContext requestContext, int startSequenceId);

    string GetChangedNodesAndProjects(IVssRequestContext requestContext, int startSequenceId);

    string CreateNode(
      IVssRequestContext requestContext,
      string nodeName,
      string parentUri,
      DateTime? startDate,
      DateTime? finishDate);

    string CreateNode(IVssRequestContext requestContext, string nodeName, string parentUri);

    void DeleteBranches(IVssRequestContext requestContext, string[] nodeUris, string reclassifyUri);

    void MoveBranch(IVssRequestContext requestContext, string nodeUri, string newParentUri);

    void RenameNode(IVssRequestContext requestContext, string nodeUri, string newName);

    void SetIterationDates(
      IVssRequestContext requestContext,
      string nodeUri,
      DateTime? startDate,
      DateTime? finishDate);

    void ReorderNode(IVssRequestContext requestContext, string nodeUri, int moveBy);
  }
}
