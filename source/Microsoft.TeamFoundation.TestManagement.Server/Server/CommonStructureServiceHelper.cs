// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CommonStructureServiceHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class CommonStructureServiceHelper : ICommonStructureServiceHelper
  {
    private IVssRequestContext m_requestContext;
    private ICommonStructureService m_cssService;

    public CommonStructureServiceHelper(IVssRequestContext context) => this.m_requestContext = context.Elevate();

    public virtual List<TcmCommonStructureNodeInfo> GetNodes(List<string> uris)
    {
      List<CommonStructureNodeInfo> nodes = this.CSSService.GetNodes(this.RequestContext, uris);
      nodes.ForEach((Action<CommonStructureNodeInfo>) (n => n.Path = Replicator.CssToWorkItemPath(n.Path)));
      return nodes.Select<CommonStructureNodeInfo, TcmCommonStructureNodeInfo>((Func<CommonStructureNodeInfo, TcmCommonStructureNodeInfo>) (n => n.ToTcmCommonStructureNodeInfo())).ToList<TcmCommonStructureNodeInfo>();
    }

    public virtual string GetWorkItemPathFromUri(string uri)
    {
      List<TcmCommonStructureNodeInfo> nodes = this.GetNodes(new List<string>()
      {
        uri
      });
      return nodes != null && nodes.Count > 0 ? nodes[0].Path : string.Empty;
    }

    public virtual TcmCommonStructureNodeInfo GetNodeFromAreaPath(string path)
    {
      try
      {
        this.RequestContext.TraceEnter("Framework", "CommonStructureServiceHelper.GetNodeFromAreaPath");
        CommonStructureNodeInfo nodeFromPath = this.CSSService.GetNodeFromPath(this.RequestContext, Replicator.WorkItemToCssPath(path, CssCacheType.Area, this.GetAreaRootName(path)));
        return nodeFromPath != null ? nodeFromPath.ToTcmCommonStructureNodeInfo() : (TcmCommonStructureNodeInfo) null;
      }
      finally
      {
        this.RequestContext.TraceLeave("Framework", "CommonStructureServiceHelper.GetNodeFromAreaPath");
      }
    }

    public virtual TcmCommonStructureNodeInfo GetNodeFromIterationPath(string path)
    {
      try
      {
        this.RequestContext.TraceEnter("Framework", "CommonStructureServiceHelper.GetNodeFromIterationPath");
        CommonStructureNodeInfo nodeFromPath = this.CSSService.GetNodeFromPath(this.RequestContext, Replicator.WorkItemToCssPath(path, CssCacheType.Iteration, this.GetIterationRootName(path)));
        return nodeFromPath != null ? nodeFromPath.ToTcmCommonStructureNodeInfo() : (TcmCommonStructureNodeInfo) null;
      }
      finally
      {
        this.RequestContext.TraceLeave("Framework", "CommonStructureServiceHelper.GetNodeFromIterationPath");
      }
    }

    public virtual List<TcmCommonStructureNodeInfo> GetRootNodes(string projectUri)
    {
      List<CommonStructureNodeInfo> list = ((IEnumerable<CommonStructureNodeInfo>) this.CSSService.GetRootNodes(this.RequestContext, projectUri)).ToList<CommonStructureNodeInfo>();
      list.ForEach((Action<CommonStructureNodeInfo>) (n => n.Path = Replicator.CssToWorkItemPath(n.Path)));
      return list.Select<CommonStructureNodeInfo, TcmCommonStructureNodeInfo>((Func<CommonStructureNodeInfo, TcmCommonStructureNodeInfo>) (n => n.ToTcmCommonStructureNodeInfo())).ToList<TcmCommonStructureNodeInfo>();
    }

    public virtual TcmCommonStructureNodeInfo GetRootNode(
      string projectUri,
      string structureTypeFilter)
    {
      CommonStructureNodeInfo csInfo = (CommonStructureNodeInfo) null;
      foreach (CommonStructureNodeInfo structureNodeInfo in ((IEnumerable<CommonStructureNodeInfo>) this.CSSService.GetRootNodes(this.RequestContext, projectUri)).ToList<CommonStructureNodeInfo>())
      {
        if (TFStringComparer.StructureType.Equals(structureNodeInfo.StructureType, structureTypeFilter))
        {
          structureNodeInfo.Path = Replicator.CssToWorkItemPath(structureNodeInfo.Path);
          csInfo = structureNodeInfo;
          break;
        }
      }
      return csInfo == null ? (TcmCommonStructureNodeInfo) null : csInfo.ToTcmCommonStructureNodeInfo();
    }

    public virtual string TCMToWorkItemPath(string tcmPath, Guid projectGuid, string projectName) => new Regex(projectGuid.ToString(), RegexOptions.IgnoreCase).Replace(tcmPath, projectName, 1);

    public virtual string WorkItemToTCMPath(string wiPath, Guid projectGuid)
    {
      string tcmPath = string.Empty;
      if (!string.IsNullOrWhiteSpace(wiPath))
      {
        if (wiPath.IndexOf('\\') < 0)
        {
          tcmPath = projectGuid.ToString();
        }
        else
        {
          int startIndex = wiPath.IndexOf('\\');
          string str = wiPath.Substring(startIndex);
          tcmPath = projectGuid.ToString() + str;
        }
      }
      return tcmPath;
    }

    public virtual string WorkItemToTCMPath(string wiPath)
    {
      string nameFromNodePath = this.ExtractProjectNameFromNodePath(wiPath);
      return !string.IsNullOrEmpty(nameFromNodePath) ? this.WorkItemToTCMPath(wiPath, this.GetProjectGuid(nameFromNodePath)) : string.Empty;
    }

    public virtual string ExtractProjectNameFromNodePath(string wiPath)
    {
      string nameFromNodePath = string.Empty;
      if (!string.IsNullOrWhiteSpace(wiPath))
      {
        if (wiPath.IndexOf('\\') < 0)
        {
          nameFromNodePath = wiPath;
        }
        else
        {
          int length = wiPath.IndexOf('\\');
          nameFromNodePath = wiPath.Substring(0, length);
        }
      }
      return nameFromNodePath;
    }

    public virtual string CssNodeToTCMPath(TcmCommonStructureNodeInfo cssNode)
    {
      string tcmPath = string.Empty;
      string path = cssNode.Path;
      if (!string.IsNullOrWhiteSpace(path) && path[0] == '\\')
      {
        int num = path.IndexOf('\\', 1);
        if (num > 0)
        {
          int startIndex = path.IndexOf('\\', num + 1);
          string str1 = cssNode.ProjectUri.Substring(cssNode.ProjectUri.Length - 36);
          if (startIndex > 0)
          {
            string str2 = path.Substring(startIndex);
            tcmPath = str1 + str2;
          }
          else
            tcmPath = str1;
        }
      }
      return tcmPath;
    }

    public virtual TCMAndWorkItemPath GetTCMAndWorkItemPath(string wiNodePath, Guid projectGuid) => new TCMAndWorkItemPath(this.WorkItemToTCMPath(wiNodePath, projectGuid), wiNodePath);

    public virtual IVssRequestContext RequestContext => this.m_requestContext;

    public virtual ICommonStructureService CSSService
    {
      get
      {
        if (this.m_cssService == null)
          this.m_cssService = this.RequestContext.GetService<ICommonStructureService>();
        return this.m_cssService;
      }
    }

    public string GetAreaRootName(string path)
    {
      string rootName = this.GetRootName(path, TreeStructureType.Area);
      this.RequestContext.TraceVerbose("Cache", "area rootName received {0} for path {1}", (object) rootName, (object) path);
      return rootName;
    }

    public string GetIterationRootName(string path)
    {
      string rootName = this.GetRootName(path, TreeStructureType.Iteration);
      this.RequestContext.TraceVerbose("Cache", "iteration rootName received {0} for path {1}", (object) rootName, (object) path);
      return rootName;
    }

    private string GetRootName(string path, TreeStructureType structureType)
    {
      this.RequestContext.TraceVerbose("Cache", "{0} is either Null or Empty and path passed is {1}", (object) structureType.ToString(), (object) path);
      string nameFromNodePath = this.ExtractProjectNameFromNodePath(path);
      this.RequestContext.TraceVerbose("Cache", "{0} project extracted from {1} path", (object) nameFromNodePath, (object) path);
      Guid projectGuid = this.GetProjectGuid(nameFromNodePath);
      return this.RequestContext.WitContext().TreeService.GetRootTreeNodes(projectGuid).First<TreeNode>((Func<TreeNode, bool>) (a => a.Type == structureType)).GetName(this.RequestContext);
    }

    private Guid GetProjectGuid(string projectName) => this.RequestContext.GetService<IProjectService>().GetProjectId(this.RequestContext, projectName);
  }
}
