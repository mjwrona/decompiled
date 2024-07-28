// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Proxy.CommonStructureService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Server;
using System;
using System.Collections;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.Proxy
{
  internal class CommonStructureService : ICommonStructureService
  {
    private Hashtable _projectInfoCache = new Hashtable();
    protected Classification _proxy;
    private static object m_locker = new object();

    protected CommonStructureService()
    {
    }

    internal CommonStructureService(TfsTeamProjectCollection tfsObject, string url) => this._proxy = new Classification(tfsObject, url);

    public Microsoft.TeamFoundation.Server.ProjectInfo[] ListProjects()
    {
      try
      {
        return this._proxy.ListProjects();
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public Microsoft.TeamFoundation.Server.ProjectInfo[] ListAllProjects()
    {
      try
      {
        return this._proxy.ListAllProjects();
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public NodeInfo[] ListStructures(string projectUri)
    {
      try
      {
        return this._proxy.ListStructures(projectUri);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public XmlElement GetNodesXml(string[] nodeUris, bool childNodes)
    {
      try
      {
        return (XmlElement) this._proxy.GetNodesXml(nodeUris, childNodes);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public XmlElement GetDeletedNodesXml(string projectUri, DateTime since)
    {
      try
      {
        return (XmlElement) this._proxy.GetDeletedNodesXml(projectUri, since);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public Microsoft.TeamFoundation.Server.ProjectInfo GetProject(string projectUri)
    {
      lock (CommonStructureService.m_locker)
      {
        if (this._projectInfoCache.ContainsKey((object) projectUri))
          return (Microsoft.TeamFoundation.Server.ProjectInfo) this._projectInfoCache[(object) projectUri];
        try
        {
          Microsoft.TeamFoundation.Server.ProjectInfo project = this._proxy.GetProject(projectUri);
          if (project != null)
            this._projectInfoCache[(object) projectUri] = (object) project;
          return project;
        }
        catch (SoapException ex)
        {
          throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
        }
      }
    }

    public NodeInfo GetNode(string nodeUri)
    {
      try
      {
        return this._proxy.GetNode(nodeUri);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public NodeInfo GetNodeFromPath(string nodePath)
    {
      try
      {
        return this._proxy.GetNodeFromPath(nodePath);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public Microsoft.TeamFoundation.Server.ProjectInfo GetProjectFromName(string projectName)
    {
      try
      {
        Microsoft.TeamFoundation.Server.ProjectInfo projectFromName = (Microsoft.TeamFoundation.Server.ProjectInfo) null;
        lock (CommonStructureService.m_locker)
          projectFromName = this.GetProjectInfoFromNameFromCache(projectName);
        if (projectFromName == null)
        {
          projectFromName = this._proxy.GetProjectFromName(projectName);
          if (projectFromName != null)
          {
            lock (CommonStructureService.m_locker)
              this._projectInfoCache[(object) projectFromName.Uri] = (object) projectFromName;
          }
        }
        return projectFromName;
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public Microsoft.TeamFoundation.Server.ProjectInfo CreateProject(
      string projectName,
      XmlElement structure)
    {
      try
      {
        Microsoft.TeamFoundation.Server.ProjectInfo project = this._proxy.CreateProject(projectName, (XmlNode) structure);
        lock (CommonStructureService.m_locker)
        {
          Microsoft.TeamFoundation.Server.ProjectInfo fromNameFromCache = this.GetProjectInfoFromNameFromCache(projectName);
          if (fromNameFromCache != null)
            this._projectInfoCache.Remove((object) fromNameFromCache.Uri);
          this._projectInfoCache[(object) project.Uri] = (object) project;
        }
        return project;
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public void ClearProjectInfoCache()
    {
      lock (CommonStructureService.m_locker)
        this._projectInfoCache.Clear();
    }

    public void DeleteProject(string projectUri)
    {
      try
      {
        lock (CommonStructureService.m_locker)
          this._projectInfoCache.Remove((object) projectUri);
        this._proxy.DeleteProject(projectUri);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public string CreateNode(string nodeName, string parentNodeUri)
    {
      try
      {
        return this._proxy.CreateNode(nodeName, parentNodeUri);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public void RenameNode(string nodeUri, string newNodeName)
    {
      try
      {
        this._proxy.RenameNode(nodeUri, newNodeName);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public void MoveBranch(string nodeUri, string newParentNodeUri)
    {
      try
      {
        this._proxy.MoveBranch(nodeUri, newParentNodeUri);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public void ReorderNode(string nodeUri, int moveBy)
    {
      try
      {
        this._proxy.ReorderNode(nodeUri, moveBy);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public void DeleteBranches(string[] nodeUris, string reclassifyUri)
    {
      try
      {
        this._proxy.DeleteBranches(nodeUris, reclassifyUri);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public void GetProjectProperties(
      string projectUri,
      out string name,
      out string state,
      out int templateId,
      out Microsoft.TeamFoundation.Server.ProjectProperty[] properties)
    {
      try
      {
        this._proxy.GetProjectProperties(projectUri, out name, out state, out templateId, out properties);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public void UpdateProjectProperties(
      string projectUri,
      string state,
      Microsoft.TeamFoundation.Server.ProjectProperty[] properties)
    {
      try
      {
        this._proxy.UpdateProjectProperties(projectUri, state, properties);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public string GetChangedNodes(int firstSequenceId)
    {
      try
      {
        return this._proxy.GetChangedNodes(firstSequenceId);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    private Microsoft.TeamFoundation.Server.ProjectInfo GetProjectInfoFromNameFromCache(
      string projectName)
    {
      foreach (Microsoft.TeamFoundation.Server.ProjectInfo fromNameFromCache in (IEnumerable) this._projectInfoCache.Values)
      {
        if (TFStringComparer.TeamProjectName.Equals(fromNameFromCache.Name, projectName))
          return fromNameFromCache;
      }
      return (Microsoft.TeamFoundation.Server.ProjectInfo) null;
    }
  }
}
