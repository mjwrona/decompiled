// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.WorkspaceInternal
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class WorkspaceInternal : IValidatable
  {
    private DateTime m_lastMappingsUpdate;
    private List<WorkingFolder> m_folders;
    private string m_computer;
    private bool m_isLocal;
    private Guid m_pendingChangeSignature = Guid.Empty;
    private string m_comment;
    private string m_name;
    private string m_securityToken;
    private DateTime m_lastAccessDate;
    private Guid m_ownerId = Guid.Empty;
    private int m_id;
    private object m_foldersUpdateLock = new object();

    internal WorkspaceInternal()
    {
    }

    internal string Computer
    {
      get => this.m_computer;
      set => this.m_computer = value;
    }

    internal bool IsLocal
    {
      get => this.m_isLocal;
      set => this.m_isLocal = value;
    }

    internal Guid PendingChangeSignature
    {
      get => this.m_pendingChangeSignature;
      set => this.m_pendingChangeSignature = value;
    }

    internal string Comment
    {
      get => this.m_comment;
      set => this.m_comment = value;
    }

    internal List<WorkingFolder> InternalFolders
    {
      get
      {
        if (this.m_folders == null)
          this.m_folders = new List<WorkingFolder>();
        return this.m_folders;
      }
      set => this.m_folders = value;
    }

    internal List<WorkingFolder> GetUpToDateFolders(IVssRequestContext requestContext)
    {
      if (this.m_folders == null)
      {
        this.m_folders = new List<WorkingFolder>();
        return this.m_folders;
      }
      this.EnsureFoldersUpToDate(requestContext);
      return this.m_folders;
    }

    private void EnsureFoldersUpToDate(IVssRequestContext requestContext)
    {
      HashSet<WorkspaceInternal> verifiedWorkspaces = requestContext.GetVerifiedWorkspaces();
      if (this.m_id == 0 || verifiedWorkspaces.Contains(this))
        return;
      lock (this.m_foldersUpdateLock)
      {
        if (verifiedWorkspaces.Contains(this))
          return;
        using (WorkspaceComponent workspaceComponent = requestContext.GetService<TeamFoundationVersionControlService>().GetWorkspaceComponent(requestContext.GetVersionControlRequestContext()))
        {
          ResultCollection resultCollection = workspaceComponent.QueryWorkspaceMappingsIfUpdated(this.Id, ref this.m_lastMappingsUpdate);
          if (resultCollection != null)
            this.m_folders = resultCollection.GetCurrent<WorkingFolder>().Items;
        }
        verifiedWorkspaces.Add(this);
      }
    }

    internal string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    internal string SecurityToken
    {
      get
      {
        if (this.m_securityToken == null)
          this.m_securityToken = "/" + this.m_name + ";" + this.m_ownerId.ToString();
        return this.m_securityToken;
      }
    }

    internal DateTime LastAccessDate
    {
      get => this.m_lastAccessDate;
      set => this.m_lastAccessDate = value;
    }

    internal DateTime LastMappingsUpdate
    {
      get => this.m_lastMappingsUpdate;
      set => this.m_lastMappingsUpdate = value;
    }

    internal Guid OwnerId
    {
      get => this.m_ownerId;
      set => this.m_ownerId = value;
    }

    internal WorkingFolder FindParentMapping(string serverPath, int maxIndex)
    {
      for (int index = maxIndex - 1; index >= 0; --index)
      {
        WorkingFolder folder = this.m_folders[index];
        if (VersionControlPath.IsSubItem(serverPath, folder.ServerItem))
          return folder;
      }
      return (WorkingFolder) null;
    }

    internal void OptimizeMappings()
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.m_folders.Sort(WorkspaceInternal.\u003C\u003EO.\u003C0\u003E__CompareDepth ?? (WorkspaceInternal.\u003C\u003EO.\u003C0\u003E__CompareDepth = new Comparison<WorkingFolder>(Mapping.CompareDepth)));
      for (int index1 = 0; index1 < this.m_folders.Count; ++index1)
      {
        WorkingFolder folder = this.m_folders[index1];
        WorkingFolder parentMapping = this.FindParentMapping(folder.ServerItem, index1);
        if (parentMapping != null)
        {
          if (Mapping.RedundantMapping((Mapping) folder, (Mapping) parentMapping))
          {
            this.m_folders.RemoveAt(index1);
            --index1;
          }
          else if (folder.Type == WorkingFolderType.Map && parentMapping.Type != WorkingFolderType.Cloak && folder.Depth != 1 && parentMapping.Depth != 1)
          {
            string relative = VersionControlPath.MakeRelative(folder.ServerItem, parentMapping.ServerItem);
            if (parentMapping.LocalItem.Length + relative.Length <= 259 && FileSpec.Equals(FileSpec.Combine(parentMapping.LocalItem, relative), folder.LocalItem))
            {
              bool flag = false;
              for (int index2 = 0; index2 < this.m_folders.Count; ++index2)
              {
                if (this.m_folders[index2].LocalItem != null && !this.m_folders[index2].Equals((object) parentMapping) && !this.m_folders[index2].Equals((object) folder) && FileSpec.IsSubItem(folder.LocalItem, this.m_folders[index2].LocalItem) && FileSpec.IsSubItem(this.m_folders[index2].LocalItem, parentMapping.LocalItem))
                {
                  flag = true;
                  break;
                }
              }
              if (!flag)
              {
                this.m_folders.RemoveAt(index1);
                --index1;
              }
            }
          }
        }
      }
    }

    internal static WorkspaceInternal FindWorkspace(
      VersionControlRequestContext versionControlRequestContext,
      Guid ownerId,
      string workspaceName,
      bool useCache)
    {
      WorkspaceInternal cached = useCache ? WorkspaceInternal.FindCached(versionControlRequestContext, ownerId, workspaceName) : (WorkspaceInternal) null;
      if (cached == null)
      {
        versionControlRequestContext.RequestContext.Trace(700227, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "FindWorkspace. Cache Miss. Looking in DB");
        List<WorkspaceInternal> items1;
        List<WorkingFolder> items2;
        using (WorkspaceComponent workspaceComponent = versionControlRequestContext.VersionControlService.GetWorkspaceComponent(versionControlRequestContext))
        {
          ResultCollection resultCollection = workspaceComponent.QueryWorkspaces(ownerId, workspaceName, (string) null);
          items1 = resultCollection.GetCurrent<WorkspaceInternal>().Items;
          resultCollection.NextResult();
          items2 = resultCollection.GetCurrent<WorkingFolder>().Items;
        }
        if (items1.Count > 0)
        {
          cached = items1[0];
          cached.InternalFolders = items2;
          versionControlRequestContext.VerifiedWorkspaces.Add(cached);
          WorkspaceInternal.Cache(versionControlRequestContext, cached);
        }
        if (versionControlRequestContext.RequestContext.IsTracing(700228, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic))
          versionControlRequestContext.RequestContext.Trace(700228, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "FindWorkspace. Found in DB: {0}", (object) string.Join<WorkspaceInternal>(";", (IEnumerable<WorkspaceInternal>) items1));
      }
      versionControlRequestContext.RequestContext.Trace(700229, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "FindWorkspace. Found workspace: {0}", (object) cached);
      return cached;
    }

    private static void Cache(
      VersionControlRequestContext vcRequestContext,
      WorkspaceInternal workspace)
    {
      vcRequestContext.VersionControlService.GetWorkspaceCache(vcRequestContext)?.Insert(WorkspaceInternal.GetCacheKey(workspace.OwnerId, workspace.Name), workspace, DateTime.Now.AddMinutes(5.0), System.Web.Caching.Cache.NoSlidingExpiration);
    }

    private static string GetCacheKey(Guid ownerID, string workspaceName) => "ws:" + ownerID.ToString() + ":" + workspaceName;

    private static WorkspaceInternal FindCached(
      VersionControlRequestContext versionControlRequestContext,
      Guid ownerId,
      string workspaceName)
    {
      return versionControlRequestContext.VersionControlService.GetWorkspaceCache(versionControlRequestContext)?.Get(WorkspaceInternal.GetCacheKey(ownerId, workspaceName));
    }

    internal static void RemoveFromCache(
      VersionControlRequestContext versionControlRequestContext,
      Guid ownerId,
      string workspaceName)
    {
      if (versionControlRequestContext.VersionControlService.GetWorkspaceCache(versionControlRequestContext) == null)
        return;
      versionControlRequestContext.VersionControlService.GetWorkspaceCache(versionControlRequestContext).Remove(WorkspaceInternal.GetCacheKey(ownerId, workspaceName));
    }

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      versionControlRequestContext.Validation.checkWorkspaceName(this.m_name, "Name", false);
      versionControlRequestContext.Validation.checkComputerName(this.m_computer, "Computer", false);
      versionControlRequestContext.Validation.nullToEmpty(ref this.m_comment);
      versionControlRequestContext.Validation.checkComment(this.m_comment, "Comment", true, 1073741823);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      this.InternalFolders.Sort(WorkspaceInternal.\u003C\u003EO.\u003C0\u003E__CompareDepth ?? (WorkspaceInternal.\u003C\u003EO.\u003C0\u003E__CompareDepth = new Comparison<WorkingFolder>(Mapping.CompareDepth)));
label_21:
      for (int index = 0; index < this.InternalFolders.Count; ++index)
      {
        WorkingFolder internalFolder = this.InternalFolders[index];
        try
        {
          internalFolder.validate(versionControlRequestContext);
        }
        catch (WildcardNotAllowedException ex)
        {
          versionControlRequestContext.RequestContext.TraceException(700216, TraceLevel.Info, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, (Exception) ex);
          throw new WildcardNotAllowedException("WorkingFolderWildcard", Array.Empty<object>());
        }
        if (VersionControlPath.Equals(internalFolder.ServerItem, "$/") && internalFolder.Type == WorkingFolderType.Cloak)
          throw new WorkingFolderException("CannotCloakRoot", (Mapping) internalFolder);
        WorkingFolder parentMapping1 = this.FindParentMapping(internalFolder.ServerItem, index);
        if (parentMapping1 == null)
        {
          if (internalFolder.Type == WorkingFolderType.Cloak)
            throw new WorkingFolderException("CloakWithNoParent", (Mapping) internalFolder);
        }
        else
        {
          if (VersionControlPath.Equals(parentMapping1.ServerItem, internalFolder.ServerItem))
            throw new WorkingFolderException("FolderMappedTwice", (Mapping) internalFolder);
          if (parentMapping1.Type == WorkingFolderType.Cloak && internalFolder.Type == WorkingFolderType.Cloak)
            throw new WorkingFolderException("CloakHasChildren", (Mapping) parentMapping1);
          if (internalFolder.Type == WorkingFolderType.Map)
          {
            string folderName = VersionControlPath.GetFolderName(internalFolder.ServerItem);
            while (true)
            {
              WorkingFolder parentMapping2 = this.FindParentMapping(folderName, index);
              if (parentMapping2 != null)
              {
                if (parentMapping2.Type != WorkingFolderType.Map || !FileSpec.IsSubItem(parentMapping2.LocalItem, internalFolder.LocalItem))
                {
                  if (!(folderName == "$/"))
                    folderName = VersionControlPath.GetFolderName(folderName);
                  else
                    goto label_21;
                }
                else
                  break;
              }
              else
                goto label_21;
            }
            throw new WorkingFolderException("CollapsedMapping", (Mapping) internalFolder);
          }
        }
      }
    }

    internal void Create(
      VersionControlRequestContext versionControlRequestContext)
    {
      using (WorkspaceComponent workspaceComponent = versionControlRequestContext.VersionControlService.GetWorkspaceComponent(versionControlRequestContext))
        this.m_id = workspaceComponent.CreateWorkspace(this);
      this.OptimizeMappings();
    }

    internal int Id
    {
      get => this.m_id;
      set => this.m_id = value;
    }

    internal WorkspaceOptions WorkspaceOptions { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "WorkspaceInternal (OwnerId: {0}, Name: {1}, SecurityToken: {2}, Id: {3}, Computer: {4}, LastAccessDate: {5}, Comment: {6}, IsLocal: {7}, LastMappingsUpdate: {8}, PendingChangeSignature: {9}, WorkspaceOptions: {10}", (object) this.OwnerId, (object) this.Name, (object) this.SecurityToken, (object) this.Id, (object) this.Computer, (object) this.LastAccessDate, (object) this.Comment, (object) this.IsLocal, (object) this.LastMappingsUpdate, (object) this.PendingChangeSignature, (object) this.WorkspaceOptions);
  }
}
