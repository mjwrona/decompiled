// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.DefaultCommonStructureServiceHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class DefaultCommonStructureServiceHelper : ICommonStructureServiceHelper
  {
    public DefaultCommonStructureServiceHelper(IVssRequestContext context)
    {
    }

    public virtual List<TcmCommonStructureNodeInfo> GetNodes(List<string> uris) => new List<TcmCommonStructureNodeInfo>();

    public virtual string GetWorkItemPathFromUri(string uri) => string.Empty;

    public virtual TcmCommonStructureNodeInfo GetNodeFromAreaPath(string path) => new TcmCommonStructureNodeInfo();

    public virtual TcmCommonStructureNodeInfo GetNodeFromIterationPath(string path) => new TcmCommonStructureNodeInfo();

    public virtual List<TcmCommonStructureNodeInfo> GetRootNodes(string projectUri) => new List<TcmCommonStructureNodeInfo>();

    public virtual TcmCommonStructureNodeInfo GetRootNode(
      string projectUri,
      string structureTypeFilter)
    {
      return new TcmCommonStructureNodeInfo();
    }

    public virtual string TCMToWorkItemPath(string tcmPath, Guid projectId, string projectName) => string.Empty;

    public virtual string WorkItemToTCMPath(string wiPath, Guid projectId) => string.Empty;

    public virtual string WorkItemToTCMPath(string wiPath) => string.Empty;

    public virtual string ExtractProjectNameFromNodePath(string wiPath) => string.Empty;

    public virtual string CssNodeToTCMPath(TcmCommonStructureNodeInfo cssNode) => string.Empty;

    public virtual TCMAndWorkItemPath GetTCMAndWorkItemPath(string wiNodePath, Guid projectId) => new TCMAndWorkItemPath();

    public string GetAreaRootName(string path) => string.Empty;

    public string GetIterationRootName(string path) => string.Empty;
  }
}
