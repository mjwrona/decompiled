// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.ICommonStructureService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Xml;

namespace Microsoft.TeamFoundation.Server
{
  public interface ICommonStructureService
  {
    ProjectInfo[] ListProjects();

    ProjectInfo[] ListAllProjects();

    NodeInfo[] ListStructures(string projectUri);

    XmlElement GetNodesXml(string[] nodeUris, bool childNodes);

    NodeInfo GetNode(string nodeUri);

    NodeInfo GetNodeFromPath(string nodePath);

    ProjectInfo GetProject(string projectUri);

    ProjectInfo GetProjectFromName(string projectName);

    void ClearProjectInfoCache();

    XmlElement GetDeletedNodesXml(string projectUri, DateTime since);

    ProjectInfo CreateProject(string projectName, XmlElement projectStructure);

    void DeleteProject(string projectUri);

    string CreateNode(string nodeName, string parentNodeUri);

    void RenameNode(string nodeUri, string newNodeName);

    void MoveBranch(string nodeUri, string newParentNodeUri);

    void ReorderNode(string nodeUri, int moveBy);

    void DeleteBranches(string[] nodeUris, string reclassifyUri);

    void GetProjectProperties(
      string projectUri,
      out string name,
      out string state,
      out int templateId,
      out ProjectProperty[] properties);

    void UpdateProjectProperties(string projectUri, string state, ProjectProperty[] properties);

    string GetChangedNodes(int startSequenceId);
  }
}
