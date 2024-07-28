// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ICommonStructureServiceHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public interface ICommonStructureServiceHelper
  {
    List<TcmCommonStructureNodeInfo> GetNodes(List<string> uris);

    string GetWorkItemPathFromUri(string uri);

    TcmCommonStructureNodeInfo GetNodeFromAreaPath(string path);

    TcmCommonStructureNodeInfo GetNodeFromIterationPath(string path);

    List<TcmCommonStructureNodeInfo> GetRootNodes(string projectUri);

    TcmCommonStructureNodeInfo GetRootNode(string projectUri, string structureTypeFilter);

    string TCMToWorkItemPath(string tcmPath, Guid projectId, string projectName);

    string WorkItemToTCMPath(string wiPath, Guid projectId);

    string WorkItemToTCMPath(string wiPath);

    string ExtractProjectNameFromNodePath(string wiPath);

    string CssNodeToTCMPath(TcmCommonStructureNodeInfo cssNode);

    TCMAndWorkItemPath GetTCMAndWorkItemPath(string wiNodePath, Guid projectId);

    string GetAreaRootName(string path);

    string GetIterationRootName(string path);
  }
}
