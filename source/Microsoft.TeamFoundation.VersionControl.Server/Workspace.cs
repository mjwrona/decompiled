// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.Workspace
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [RequiredClientService("VersionControlServer")]
  [CallOnDeserialization("AfterDeserialize")]
  public class Workspace : IValidatable, IRecordable
  {
    private WorkspaceInternal m_core;
    private string m_ownerName;
    private string m_ownerDisplayName;
    private List<string> m_ownerAliases = new List<string>();
    private Microsoft.VisualStudio.Services.Identity.Identity m_owner;
    private int m_effectivePermissions;

    public string LocalToServerItem(
      IVssRequestContext requestContext,
      string localItem,
      bool honorCloaks)
    {
      return this.LocalToServerItemPathPair(requestContext, localItem, honorCloaks).ProjectNamePath;
    }

    internal ItemPathPair LocalToServerItemPathPair(
      IVssRequestContext requestContext,
      string localItem,
      bool honorCloaks)
    {
      bool isCloaked;
      ItemPathPair serverItem = this.TranslateLocalItemToServerItem(requestContext, localItem, out isCloaked, out int _);
      if (serverItem.ProjectNamePath == null)
        throw new ItemNotMappedException(localItem);
      if (isCloaked & honorCloaks)
        throw new ItemCloakedException(localItem);
      return serverItem;
    }

    public string ServerToLocalItem(
      IVssRequestContext requestContext,
      string serverItem,
      bool honorCloaks)
    {
      bool isCloaked;
      string localItem = this.TranslateServerItemToLocalItem(requestContext, serverItem, out isCloaked, out int _);
      if (localItem == null)
      {
        if (!isCloaked)
          throw new ItemNotMappedException(serverItem);
        if (honorCloaks)
          throw new ItemCloakedException(serverItem);
      }
      return localItem;
    }

    public WorkingFolder LocalItemToWorkingFolder(
      IVssRequestContext requestContext,
      string localItem)
    {
      bool isCloaked;
      int depth;
      ItemPathPair serverItem = this.TranslateLocalItemToServerItem(requestContext, localItem, out isCloaked, out depth);
      if (serverItem.ProjectNamePath == null)
        throw new ItemNotMappedException(localItem);
      return isCloaked ? new WorkingFolder(serverItem, (string) null, WorkingFolderType.Cloak, 120) : new WorkingFolder(serverItem, localItem, WorkingFolderType.Map, depth);
    }

    public WorkingFolder ServerItemToWorkingFolder(
      IVssRequestContext requestContext,
      string serverItem)
    {
      bool isCloaked;
      int depth;
      string localItem = this.TranslateServerItemToLocalItem(requestContext, serverItem, out isCloaked, out depth);
      if (localItem != null)
        return new WorkingFolder(serverItem, localItem, WorkingFolderType.Map, depth);
      if (isCloaked)
        return new WorkingFolder(serverItem, (string) null, WorkingFolderType.Cloak, 120);
      throw new ItemNotMappedException(serverItem);
    }

    private ItemPathPair TranslateLocalItemToServerItem(
      IVssRequestContext requestContext,
      string localItem,
      out bool isCloaked,
      out int depth)
    {
      isCloaked = false;
      depth = 0;
      WorkingFolder workingFolder = (WorkingFolder) null;
      int num1 = 0;
      int num2 = -1;
      string projectNamePath = (string) null;
      string projectGuidPath = (string) null;
      foreach (WorkingFolder upToDateFolder in this.GetUpToDateFolders(requestContext))
      {
        if (upToDateFolder != null && upToDateFolder.Type != WorkingFolderType.Cloak && FileSpec.IsSubItem(localItem, upToDateFolder.LocalItem) && upToDateFolder.LocalItem.Length > num1)
        {
          workingFolder = upToDateFolder;
          num1 = upToDateFolder.LocalItem.Length;
          depth = workingFolder.Depth;
          if (upToDateFolder.Depth == 1)
          {
            int folderDepth = FileSpec.GetFolderDepth(upToDateFolder.LocalItem);
            if (num2 < 0)
              num2 = FileSpec.GetFolderDepth(localItem);
            if (folderDepth + 1 == num2)
              depth = 0;
            else if (folderDepth != num2)
              workingFolder = (WorkingFolder) null;
          }
        }
      }
      if (workingFolder != null)
      {
        PathLength serverPathLength = requestContext.GetVersionControlRequestContext().MaxSupportedServerPathLength;
        string serverItem = workingFolder.ServerItem;
        int relativeStartIndex = FileSpec.GetRelativeStartIndex(localItem, workingFolder.LocalItem);
        if (relativeStartIndex >= 0)
        {
          StringBuilder stringBuilder1 = new StringBuilder(workingFolder.ServerItem, workingFolder.ServerItem.Length + 1 + localItem.Length - relativeStartIndex);
          if (stringBuilder1.Length > 2)
            stringBuilder1.Append('/');
          stringBuilder1.Append(localItem, relativeStartIndex, localItem.Length - relativeStartIndex);
          stringBuilder1.Replace(Path.DirectorySeparatorChar, '/', workingFolder.ServerItem.Length, stringBuilder1.Length - workingFolder.ServerItem.Length);
          serverItem = stringBuilder1.ToString();
          if (workingFolder.ItemPathPair.ProjectGuidPath != null && workingFolder.ItemPathPair.ProjectGuidPath.Length > "$/".Length)
          {
            StringBuilder stringBuilder2 = new StringBuilder(workingFolder.ItemPathPair.ProjectGuidPath, workingFolder.ItemPathPair.ProjectGuidPath.Length + 1 + localItem.Length - relativeStartIndex);
            if (stringBuilder2.Length > 2)
              stringBuilder2.Append('/');
            stringBuilder2.Append(localItem, relativeStartIndex, localItem.Length - relativeStartIndex);
            stringBuilder2.Replace(Path.DirectorySeparatorChar, '/', workingFolder.ItemPathPair.ProjectGuidPath.Length, stringBuilder2.Length - workingFolder.ItemPathPair.ProjectGuidPath.Length);
            projectGuidPath = stringBuilder2.ToString();
          }
        }
        projectNamePath = VersionControlPath.GetFullPath(serverItem, serverPathLength);
        projectGuidPath = projectGuidPath != null ? VersionControlPath.GetFullPath(projectGuidPath, serverPathLength + 35) : (string) null;
        int length = workingFolder.ServerItem.Length;
        foreach (WorkingFolder upToDateFolder in this.GetUpToDateFolders(requestContext))
        {
          if (upToDateFolder != null && upToDateFolder.Type != WorkingFolderType.Map && upToDateFolder.ServerItem.Length > length && VersionControlPath.IsSubItem(projectNamePath, upToDateFolder.ServerItem))
          {
            isCloaked = true;
            break;
          }
        }
      }
      return new ItemPathPair(projectNamePath, projectGuidPath);
    }

    private string TranslateServerItemToLocalItem(
      IVssRequestContext requestContext,
      string serverItem,
      out bool isCloaked,
      out int depth)
    {
      int num1 = -1;
      int num2 = 0;
      WorkingFolder workingFolder = (WorkingFolder) null;
      string localItem1 = (string) null;
      depth = 120;
      foreach (WorkingFolder upToDateFolder in this.GetUpToDateFolders(requestContext))
      {
        if (upToDateFolder != null && VersionControlPath.IsSubItem(serverItem, upToDateFolder.ServerItem) && upToDateFolder.ServerItem.Length > num2)
        {
          workingFolder = upToDateFolder;
          num2 = upToDateFolder.ServerItem.Length;
          depth = upToDateFolder.Depth;
          if (upToDateFolder.Depth == 1)
          {
            int folderDepth = VersionControlPath.GetFolderDepth(upToDateFolder.ServerItem);
            if (num1 < 0)
              num1 = VersionControlPath.GetFolderDepth(serverItem);
            if (folderDepth + 1 == num1)
              depth = 0;
            else if (folderDepth != num1)
              workingFolder = (WorkingFolder) null;
          }
        }
      }
      isCloaked = workingFolder != null && workingFolder.Type == WorkingFolderType.Cloak;
      if (workingFolder != null && !isCloaked)
      {
        string localItem2 = workingFolder.LocalItem;
        int relativeStartIndex = VersionControlPath.GetRelativeStartIndex(serverItem, workingFolder.ServerItem);
        if (relativeStartIndex >= 0)
        {
          StringBuilder stringBuilder = new StringBuilder(workingFolder.LocalItem, workingFolder.LocalItem.Length + 1 + serverItem.Length - relativeStartIndex);
          if ((int) stringBuilder[stringBuilder.Length - 1] != (int) Path.DirectorySeparatorChar)
            stringBuilder.Append(Path.DirectorySeparatorChar);
          stringBuilder.Append(serverItem, relativeStartIndex, serverItem.Length - relativeStartIndex);
          stringBuilder.Replace('/', Path.DirectorySeparatorChar, workingFolder.LocalItem.Length, stringBuilder.Length - workingFolder.LocalItem.Length);
          localItem2 = stringBuilder.ToString();
        }
        localItem1 = FileSpec.GetFullPath(localItem2);
      }
      return localItem1;
    }

    public Workspace() => this.m_core = new WorkspaceInternal();

    internal Workspace(
      VersionControlRequestContext versionControlRequestContext,
      WorkspaceInternal wsInternal,
      Microsoft.VisualStudio.Services.Identity.Identity workspaceOwner = null,
      bool throwOnUnrecognizedVSID = true)
    {
      this.m_core = wsInternal;
      if (workspaceOwner == null)
        workspaceOwner = TfvcIdentityHelper.FindIdentity(versionControlRequestContext.RequestContext, this.OwnerId, throwOnUnrecognizedVSID);
      this.m_owner = workspaceOwner;
      this.m_ownerName = this.m_owner.Id.ToString();
      this.m_ownerDisplayName = this.m_owner.DisplayName;
      string domainUserName = IdentityHelper.GetDomainUserName(this.m_owner);
      if (!string.IsNullOrEmpty(domainUserName))
        this.m_ownerAliases.Add(domainUserName);
      string property = this.m_owner.GetProperty<string>("Account", (string) null);
      if (!string.IsNullOrEmpty(property))
        this.m_ownerAliases.Add(property);
      this.m_ownerAliases.Add(this.m_ownerDisplayName);
    }

    [XmlAttribute("computer")]
    [ClientProperty(ClientVisibility.Private)]
    public string Computer
    {
      get => this.m_core.Computer;
      set => this.m_core.Computer = value;
    }

    [ClientProperty(ClientVisibility.Private)]
    public string Comment
    {
      get => this.m_core.Comment;
      set => this.m_core.Comment = value;
    }

    [XmlAttribute("islocal")]
    [ClientProperty(ClientVisibility.Private, PropertyName = "IsLocalWorkspace")]
    public bool IsLocal
    {
      get => this.m_core.IsLocal;
      set => this.m_core.IsLocal = value;
    }

    [ClientProperty(ClientVisibility.Private)]
    public List<WorkingFolder> Folders
    {
      get => this.m_core.InternalFolders;
      set => this.m_core.InternalFolders = value;
    }

    public List<WorkingFolder> GetUpToDateFolders(IVssRequestContext requestContext) => this.m_core.GetUpToDateFolders(requestContext);

    [ClientProperty(ClientVisibility.Private)]
    public List<string> OwnerAliases => this.m_ownerAliases;

    [XmlAttribute("name")]
    [ClientProperty(ClientVisibility.Private)]
    public string Name
    {
      get => this.m_core.Name;
      set => this.m_core.Name = value;
    }

    [XmlAttribute("owner")]
    [ClientProperty(ClientVisibility.Private)]
    public string OwnerName
    {
      get => this.m_ownerName;
      set => this.m_ownerName = value;
    }

    [XmlAttribute("ownerdisp")]
    [ClientProperty(ClientVisibility.Private)]
    public string OwnerDisplayName
    {
      get => this.m_ownerDisplayName;
      set => this.m_ownerDisplayName = value;
    }

    [XmlAttribute("owneruniq")]
    [ClientProperty(ClientVisibility.Private)]
    public string OwnerUniqueName
    {
      get => this.m_ownerName;
      set
      {
        if (!string.IsNullOrEmpty(this.m_ownerName))
          return;
        this.m_ownerName = value;
      }
    }

    [XmlAttribute("permissions")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "EffectivePermissions", Direction = ClientPropertySerialization.ServerToClientOnly)]
    public int EffectivePermissions
    {
      get => this.m_effectivePermissions;
      set => this.m_effectivePermissions = value;
    }

    [XmlAttribute("securitytoken")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "SecurityToken", Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string SecurityToken
    {
      get => this.m_core.SecurityToken;
      set
      {
      }
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime LastAccessDate
    {
      get => this.m_core.LastAccessDate;
      set => this.m_core.LastAccessDate = value;
    }

    [XmlAttribute("ownertype")]
    public string OwnerIdentityType
    {
      get => this.m_owner == null ? (string) null : this.m_owner.Descriptor.IdentityType;
      set
      {
      }
    }

    [XmlAttribute("ownerid")]
    public string OwnerIdentifier
    {
      get => this.m_owner == null ? (string) null : this.m_owner.Descriptor.Identifier;
      set
      {
      }
    }

    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public int Options
    {
      get => (int) this.m_core.WorkspaceOptions;
      set => this.m_core.WorkspaceOptions = (WorkspaceOptions) value;
    }

    internal ItemPathPair LocalToCommittedServerItem(
      VersionControlRequestContext versionControlRequestContext,
      string localItem,
      bool honorCloaks)
    {
      using (VersionedItemComponent versionedItemComponent = versionControlRequestContext.VersionControlService.GetVersionedItemComponent(versionControlRequestContext))
        return versionedItemComponent.MapLocalToServerItemWithoutMappingRenames(this.m_core, localItem, honorCloaks, versionControlRequestContext.MaxSupportedServerPathLength);
    }

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      versionControlRequestContext.Validation.checkIdentity(ref this.m_ownerName, "OwnerName", false);
      versionControlRequestContext.Validation.check((IValidatable) this.m_core, parameterName, false);
    }

    internal Guid OwnerId => this.m_core.OwnerId;

    internal void ResolveOwner(
      VersionControlRequestContext versionControlRequestContext)
    {
      this.m_owner = TfvcIdentityHelper.FindIdentity(versionControlRequestContext.RequestContext, this.OwnerName);
      this.m_core.OwnerId = this.m_owner.Id;
    }

    internal static Workspace CreateWorkspace(
      VersionControlRequestContext versionControlRequestContext,
      Workspace workspace)
    {
      SecurityManager securityWrapper = versionControlRequestContext.VersionControlService.SecurityWrapper;
      securityWrapper.CheckGlobalPermission(versionControlRequestContext, GlobalPermissions.CreateWorkspace);
      IdentityDescriptor userContext = versionControlRequestContext.RequestContext.UserContext;
      workspace.ResolveOwner(versionControlRequestContext);
      if (!IdentityDescriptorComparer.Instance.Equals(workspace.Owner.Descriptor, userContext))
      {
        versionControlRequestContext.RequestContext.Trace(700223, TraceLevel.Verbose, TraceArea.CreateWorkspace, TraceLayer.BusinessLogic, "CheckGlobalPermission for AdminWorkspaces. owner:{0} id:{1} callerId:{2}", (object) workspace.Owner, (object) workspace.Owner.Descriptor.Identifier, (object) userContext.Identifier);
        securityWrapper.CheckGlobalPermission(versionControlRequestContext, GlobalPermissions.AdminWorkspaces);
      }
      ClientTraceData ctData = new ClientTraceData();
      ctData.Add("Name", (object) workspace?.Name);
      ctData.Add("Owner", (object) workspace?.OwnerId);
      ClientTrace.Publish(versionControlRequestContext.RequestContext, nameof (CreateWorkspace), ctData);
      WorkspaceNotification notificationEvent = new WorkspaceNotification(versionControlRequestContext.RequestContext.GetUserIdentity(), workspace.Owner, workspace.Name, workspace.Computer);
      ITeamFoundationEventService service = versionControlRequestContext.RequestContext.GetService<ITeamFoundationEventService>();
      service.PublishDecisionPoint(versionControlRequestContext.RequestContext, (object) notificationEvent);
      try
      {
        workspace.m_core.Create(versionControlRequestContext);
      }
      catch (WorkspaceExistsException ex)
      {
        string computer = Workspace.FindWorkspace(versionControlRequestContext, workspace.OwnerName, workspace.Name, false).Computer;
        ex.ComputerName = computer;
        throw;
      }
      versionControlRequestContext.VerifiedWorkspaces.Add(workspace.m_core);
      versionControlRequestContext.GetWorkspaceSecurity().OnDataChanged(versionControlRequestContext.RequestContext);
      service.PublishNotification(versionControlRequestContext.RequestContext, (object) notificationEvent);
      return Workspace.FindWorkspaceWithPermissions(versionControlRequestContext, workspace.Owner, workspace.Name, false);
    }

    internal static Workspace UpdateWorkspace(
      VersionControlRequestContext versionControlRequestContext,
      string oldWorkspaceName,
      string oldOwnerName,
      Workspace newWorkspace,
      SupportedFeatures clientFeatures)
    {
      Workspace workspace1 = !Guid.TryParse(oldOwnerName, out Guid _) ? Workspace.QueryWorkspace(versionControlRequestContext, oldWorkspaceName, oldOwnerName) : Workspace.QueryWorkspace(versionControlRequestContext, oldWorkspaceName, oldOwnerName, true, false, false, false);
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckWorkspacePermission(versionControlRequestContext, 8, workspace1);
      newWorkspace.ResolveOwner(versionControlRequestContext);
      ClientTraceData ctData = new ClientTraceData();
      ctData.Add("Name", (object) newWorkspace?.Name);
      ctData.Add("Owner", (object) newWorkspace?.OwnerId);
      ClientTrace.Publish(versionControlRequestContext.RequestContext, nameof (UpdateWorkspace), ctData);
      if ((clientFeatures & SupportedFeatures.OneLevelMapping) == SupportedFeatures.None)
      {
        foreach (WorkingFolder folder in newWorkspace.Folders)
        {
          foreach (WorkingFolder upToDateFolder in workspace1.GetUpToDateFolders(versionControlRequestContext.RequestContext))
          {
            if (folder.ServerItem == upToDateFolder.ServerItem && folder.LocalItem == upToDateFolder.LocalItem)
              folder.Depth = upToDateFolder.Depth;
          }
        }
      }
      newWorkspace.m_core.OptimizeMappings();
      WorkspaceNotification notificationEvent = new WorkspaceNotification(versionControlRequestContext.RequestContext.GetUserIdentity(), workspace1.Owner, workspace1.Name, workspace1.Computer, newWorkspace.Name, newWorkspace.Computer, newWorkspace.Owner);
      ITeamFoundationEventService service = versionControlRequestContext.RequestContext.GetService<ITeamFoundationEventService>();
      service.PublishDecisionPoint(versionControlRequestContext.RequestContext, (object) notificationEvent);
      try
      {
        using (WorkspaceComponent workspaceComponent = versionControlRequestContext.VersionControlService.GetWorkspaceComponent(versionControlRequestContext))
          workspaceComponent.UpdateWorkspace(workspace1.OwnerId, oldWorkspaceName, newWorkspace.OwnerId, newWorkspace.Name, newWorkspace.Comment, newWorkspace.Computer, newWorkspace.Folders, workspace1.SecurityToken, newWorkspace.SecurityToken, (WorkspaceOptions) newWorkspace.Options, newWorkspace.m_core.IsLocal);
      }
      catch (WorkspaceExistsException ex)
      {
        versionControlRequestContext.RequestContext.TraceException(700213, TraceLevel.Info, TraceArea.UpdateWorkspace, TraceLayer.BusinessLogic, (Exception) ex);
        Workspace workspace2 = Workspace.FindWorkspace(versionControlRequestContext, newWorkspace.OwnerName, newWorkspace.Name, false);
        ex.ComputerName = workspace2.Computer;
        throw;
      }
      versionControlRequestContext.GetWorkspaceSecurity().OnDataChanged(versionControlRequestContext.RequestContext);
      Workspace.RemoveFromCache(versionControlRequestContext, workspace1.OwnerId, oldWorkspaceName);
      service.PublishNotification(versionControlRequestContext.RequestContext, (object) notificationEvent);
      return Workspace.FindWorkspaceWithPermissions(versionControlRequestContext, newWorkspace.OwnerName, newWorkspace.Name, false);
    }

    internal static Workspace QueryWorkspace(
      VersionControlRequestContext versionControlRequestContext,
      string workspaceName,
      string workspaceOwner)
    {
      return Workspace.QueryWorkspace(versionControlRequestContext, workspaceName, workspaceOwner, true, false, false);
    }

    internal static Workspace QueryWorkspace(
      VersionControlRequestContext versionControlRequestContext,
      string workspaceName,
      string workspaceOwner,
      bool useCache)
    {
      return Workspace.QueryWorkspace(versionControlRequestContext, workspaceName, workspaceOwner, useCache, false, false);
    }

    internal static Workspace QueryWorkspace(
      VersionControlRequestContext versionControlRequestContext,
      string workspaceName,
      string workspaceOwner,
      bool useCache,
      bool hideLocalWorkspaces,
      bool hideWorkspaceWithOptions,
      bool throwOnUnrecognizedVSID = true)
    {
      versionControlRequestContext.RequestContext.Trace(700218, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "Workspace.QueryWorkspace: workspaceName:{0}, workspaceOwner:{1}, useCache:{2}, hideLocalWorkspaces:{3}, hideWorkspaceWithOptions:{4}", (object) workspaceName, (object) workspaceOwner, (object) useCache, (object) hideLocalWorkspaces, (object) hideWorkspaceWithOptions);
      Workspace workspace = (Workspace) null;
      if (!string.IsNullOrEmpty(workspaceName))
      {
        if (!string.IsNullOrEmpty(workspaceOwner))
        {
          try
          {
            workspace = Workspace.FindWorkspaceWithPermissions(versionControlRequestContext, workspaceOwner, workspaceName, useCache, throwOnUnrecognizedVSID);
            versionControlRequestContext.RequestContext.Trace(700218, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "Found workspace with permissions: {0}", (object) workspace);
            workspace.OwnerName = workspaceOwner;
            if ((!hideLocalWorkspaces || !workspace.IsLocal) && (workspace.EffectivePermissions & 1) != 0)
            {
              if (hideWorkspaceWithOptions)
              {
                if (workspace.Options == 0)
                  goto label_7;
              }
              else
                goto label_7;
            }
            versionControlRequestContext.RequestContext.Trace(700218, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "User {0} does not have permissions to workspace {1}. Throwing WorkspaceNotFoundException.", (object) workspaceOwner, (object) workspaceName);
            throw new WorkspaceNotFoundException(workspaceName, workspaceOwner);
          }
          catch (IdentityNotFoundException ex)
          {
            versionControlRequestContext.RequestContext.TraceException(700218, TraceLevel.Info, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, (Exception) ex);
            versionControlRequestContext.RequestContext.Trace(700218, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "User {0} is not in Version Control. Workspace {1}. Throwing WorkspaceNotFoundException.", (object) workspaceOwner, (object) workspaceName);
            throw new WorkspaceNotFoundException(workspaceName, workspaceOwner);
          }
        }
      }
label_7:
      versionControlRequestContext.RequestContext.Trace(700218, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "Found workspace: {0}", (object) workspace);
      return workspace;
    }

    internal static List<Workspace> QueryWorkspaces(
      VersionControlRequestContext versionControlRequestContext,
      string ownerName,
      string computer,
      int permissionsFilter)
    {
      versionControlRequestContext.RequestContext.Trace(700236, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "Workspace.QueryWorkspaces: ownerName:{0}, computer:{1}, permissionsFilter:{2}", (object) ownerName, (object) computer, (object) permissionsFilter);
      Guid ownerId = Guid.Empty;
      List<Workspace> values = new List<Workspace>();
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      if (ownerName != null)
      {
        try
        {
          identity = TfvcIdentityHelper.FindIdentity(versionControlRequestContext.RequestContext, ownerName);
          ownerId = identity.Id;
          versionControlRequestContext.RequestContext.Trace(700237, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "Workspace.QueryWorkspaces: owner:{0}", (object) identity);
        }
        catch (AuthorizationException ex)
        {
          versionControlRequestContext.RequestContext.TraceException(700215, TraceLevel.Info, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, (Exception) ex);
          return values;
        }
      }
      if (permissionsFilter > 0 && identity != null && IdentityDescriptorComparer.Instance.Equals(identity.Descriptor, versionControlRequestContext.RequestContext.UserContext))
      {
        ownerName = (string) null;
        ownerId = Guid.Empty;
        versionControlRequestContext.RequestContext.Trace(700238, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "Nulling out the owner");
      }
      List<WorkspaceInternal> items1;
      List<WorkingFolder> items2;
      using (WorkspaceComponent workspaceComponent = versionControlRequestContext.VersionControlService.GetWorkspaceComponent(versionControlRequestContext))
      {
        ResultCollection resultCollection = workspaceComponent.QueryWorkspaces(ownerId, (string) null, computer);
        items1 = resultCollection.GetCurrent<WorkspaceInternal>().Items;
        resultCollection.NextResult();
        items2 = resultCollection.GetCurrent<WorkingFolder>().Items;
      }
      IdentityService service = versionControlRequestContext.RequestContext.GetService<IdentityService>();
      int num1 = 1;
      permissionsFilter |= 1;
      if (versionControlRequestContext.RequestContext.IsSystemContext)
        num1 |= 15;
      else if (service.IsMember(versionControlRequestContext.RequestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, versionControlRequestContext.RequestContext.UserContext))
        num1 |= 8;
      Guid userId = versionControlRequestContext.RequestContext.GetUserId(true);
      IVssSecurityNamespace workspaceSecurity = versionControlRequestContext.GetWorkspaceSecurity();
      versionControlRequestContext.RequestContext.Trace(700240, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "callerId: {0}", (object) userId);
      foreach (WorkspaceInternal wsInternal in items1)
      {
        versionControlRequestContext.RequestContext.Trace(700239, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "Considering found workspace: {0}", (object) wsInternal);
        int num2 = num1;
        if (userId == wsInternal.OwnerId)
          num2 |= 15;
        if (num2 != 15)
        {
          num2 |= workspaceSecurity.QueryEffectivePermissions(versionControlRequestContext.RequestContext, wsInternal.SecurityToken, (EvaluationPrincipal) versionControlRequestContext.RequestContext.UserContext);
          if (permissionsFilter != (num2 & permissionsFilter))
          {
            versionControlRequestContext.RequestContext.Trace(700241, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "User doesn't have requested permissions for this workspace: {0}, permissionsFilter: {1}, effectivePermissions: {2}", (object) wsInternal, (object) permissionsFilter, (object) num2);
            continue;
          }
        }
        values.Add(new Workspace(versionControlRequestContext, wsInternal, throwOnUnrecognizedVSID: false)
        {
          EffectivePermissions = num2
        });
        versionControlRequestContext.VerifiedWorkspaces.Add(wsInternal);
      }
      int index = 0;
      foreach (WorkingFolder workingFolder in items2)
      {
        Workspace workspace1 = (Workspace) null;
        for (; index < values.Count; ++index)
        {
          Workspace workspace2 = values[index];
          if (workspace2.Id == workingFolder.workspaceId)
          {
            workspace1 = workspace2;
            break;
          }
          if (workspace2.Id > workingFolder.workspaceId)
            break;
        }
        workspace1?.Folders.Add(workingFolder);
      }
      if (versionControlRequestContext.RequestContext.IsTracing(700242, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic))
        versionControlRequestContext.RequestContext.Trace(700242, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "Visible workspaces: {0}", (object) string.Join<Workspace>(";", (IEnumerable<Workspace>) values));
      return values;
    }

    internal static Guid QueryPendingChangeSignature(
      VersionControlRequestContext vcRequestContext,
      string workspaceName,
      string workspaceOwner)
    {
      Guid result;
      Microsoft.VisualStudio.Services.Identity.Identity identity = !Guid.TryParse(workspaceOwner, out result) ? TfvcIdentityHelper.FindIdentity(vcRequestContext.RequestContext, workspaceOwner) : TfvcIdentityHelper.FindIdentity(vcRequestContext.RequestContext, result, false);
      using (WorkspaceComponent workspaceComponent = vcRequestContext.VersionControlService.GetWorkspaceComponent(vcRequestContext))
      {
        if (workspaceComponent is WorkspaceComponent4)
          return ((WorkspaceComponent4) workspaceComponent).QueryPendingChangeSignature(workspaceName, identity.Id);
        Workspace workspace = Workspace.QueryWorkspace(vcRequestContext, workspaceName, workspaceOwner, false);
        return workspace.IsLocal ? workspace.PendingChangeSignature : throw new LocalWorkspaceRequiredException(workspaceName, workspaceOwner);
      }
    }

    internal static Workspace FindWorkspaceWithPermissions(
      VersionControlRequestContext versionControlRequestContext,
      string workspaceOwnerName,
      string workspaceName,
      bool useCache,
      bool throwOnUnrecognizedVSID = true)
    {
      Workspace workspace = Workspace.FindWorkspace(versionControlRequestContext, workspaceOwnerName, workspaceName, useCache, throwOnUnrecognizedVSID);
      Workspace.PopulateEffectivePermissionsForUser(workspace, versionControlRequestContext);
      return workspace;
    }

    internal static Workspace FindWorkspaceWithPermissions(
      VersionControlRequestContext versionControlRequestContext,
      Microsoft.VisualStudio.Services.Identity.Identity workspaceOwner,
      string workspaceName,
      bool useCache)
    {
      Workspace workspace = Workspace.FindWorkspace(versionControlRequestContext, workspaceOwner, workspaceName, useCache);
      Workspace.PopulateEffectivePermissionsForUser(workspace, versionControlRequestContext);
      return workspace;
    }

    internal static void PopulateEffectivePermissionsForUser(
      Workspace workspace,
      VersionControlRequestContext versionControlRequestContext)
    {
      if (workspace == null)
        return;
      IVssRequestContext requestContext = versionControlRequestContext.RequestContext;
      IdentityService service = requestContext.GetService<IdentityService>();
      versionControlRequestContext.RequestContext.Trace(700232, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "workspace.Owner.Descriptor: {0}, requestContext.UserContext: {1}, workspace.SecurityToken: {2}", (object) workspace.Owner.Descriptor, (object) requestContext.UserContext, (object) workspace.SecurityToken);
      if (requestContext.IsSystemContext || IdentityDescriptorComparer.Instance.Equals(workspace.Owner.Descriptor, requestContext.UserContext))
      {
        workspace.EffectivePermissions = 15;
        versionControlRequestContext.RequestContext.Trace(700231, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "IsSystemContext or is the owner. Permissions: {0}", (object) workspace.EffectivePermissions);
      }
      else
      {
        IVssSecurityNamespace workspaceSecurity = versionControlRequestContext.GetWorkspaceSecurity();
        workspace.EffectivePermissions = 1 | workspaceSecurity.QueryEffectivePermissions(requestContext, workspace.SecurityToken, (EvaluationPrincipal) requestContext.UserContext);
        if ((8 & workspace.EffectivePermissions) == 0 && service.IsMember(requestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, requestContext.UserContext))
          workspace.EffectivePermissions |= 8;
      }
      versionControlRequestContext.RequestContext.Trace(700233, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "EffectivePermissions: {0}", (object) workspace.EffectivePermissions);
    }

    internal static Workspace FindWorkspace(
      VersionControlRequestContext versionControlRequestContext,
      string workspaceOwnerName,
      string workspaceName,
      bool useCache,
      bool throwOnUnrecognizedVSID = true)
    {
      Guid result;
      Microsoft.VisualStudio.Services.Identity.Identity workspaceOwner = throwOnUnrecognizedVSID || !Guid.TryParse(workspaceOwnerName, out result) ? TfvcIdentityHelper.FindIdentity(versionControlRequestContext.RequestContext, workspaceOwnerName) : TfvcIdentityHelper.FindIdentity(versionControlRequestContext.RequestContext, result, false);
      return Workspace.FindWorkspace(versionControlRequestContext, workspaceOwner, workspaceName, useCache);
    }

    internal static Workspace FindWorkspace(
      VersionControlRequestContext versionControlRequestContext,
      Microsoft.VisualStudio.Services.Identity.Identity workspaceOwner,
      string workspaceName,
      bool useCache)
    {
      versionControlRequestContext.RequestContext.Trace(700224, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "FindWorkspace. Found owner: {0}", (object) workspaceOwner);
      WorkspaceInternal workspace = WorkspaceInternal.FindWorkspace(versionControlRequestContext, workspaceOwner.Id, workspaceName, useCache);
      if (workspace == null)
      {
        versionControlRequestContext.RequestContext.Trace(700225, TraceLevel.Verbose, TraceArea.QueryWorkspace, TraceLayer.BusinessLogic, "Workspace is null, throwing WorkspaceNotFoundException. Owner:{0}. Name:{1}", (object) workspaceOwner, (object) workspaceName);
        throw new WorkspaceNotFoundException(workspaceName, workspaceOwner.DisplayName);
      }
      return new Workspace(versionControlRequestContext, workspace, workspaceOwner);
    }

    internal int Id => this.m_core.Id;

    internal Microsoft.VisualStudio.Services.Identity.Identity Owner => this.m_owner;

    internal Guid PendingChangeSignature => this.m_core.PendingChangeSignature;

    internal static void RemoveFromCache(
      VersionControlRequestContext versionControlRequestContext,
      Guid ownerId,
      string workspaceName)
    {
      WorkspaceInternal.RemoveFromCache(versionControlRequestContext, ownerId, workspaceName);
    }

    internal void DeleteWorkspace(
      VersionControlRequestContext versionControlRequestContext)
    {
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckWorkspacePermission(versionControlRequestContext, 8, this);
      WorkspaceNotification notificationEvent = new WorkspaceNotification(versionControlRequestContext.RequestContext.GetUserIdentity(), this.Owner, this.Name, this.Computer);
      ClientTraceData ctData = new ClientTraceData();
      ctData.Add("Name", (object) this.Name);
      ctData.Add("Owner", (object) this.OwnerId);
      ClientTrace.Publish(versionControlRequestContext.RequestContext, nameof (DeleteWorkspace), ctData);
      ITeamFoundationEventService service = versionControlRequestContext.RequestContext.GetService<ITeamFoundationEventService>();
      service.PublishDecisionPoint(versionControlRequestContext.RequestContext, (object) notificationEvent);
      using (WorkspaceComponent workspaceComponent = versionControlRequestContext.VersionControlService.GetWorkspaceComponent(versionControlRequestContext))
        workspaceComponent.DeleteWorkspace(this.OwnerId, this.Name, this.SecurityToken);
      Workspace.RemoveFromCache(versionControlRequestContext, this.OwnerId, this.Name);
      versionControlRequestContext.GetWorkspaceSecurity().OnDataChanged(versionControlRequestContext.RequestContext);
      service.PublishNotification(versionControlRequestContext.RequestContext, (object) notificationEvent);
    }

    public void RecordInformation(MethodInformation methodInformation, int paramIndex)
    {
      string str = "workspace[" + (object) paramIndex + "].";
      methodInformation.AddParameter(str + "Name", (object) this.Name);
      methodInformation.AddParameter(str + "OwnerName", (object) this.OwnerName);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Workspace (Owner: {0}, OwnerUniqueName: {1}, OwnerName: {2}, OwnerDisplayName: {3}, OwnerId: {4}, Name: {5}, Id: {6}, Comment: {7}, Computer: {8}, IsLocal: {9}, SecurityToken: {10}, EffectivePermissions: {11}, LastAccessDate: {12}, Options: {13}, PendingChangeSignature: {14})", (object) this.Owner, (object) this.OwnerUniqueName, (object) this.OwnerName, (object) this.OwnerDisplayName, (object) this.OwnerId, (object) this.Name, (object) this.Id, (object) this.Comment, (object) this.Computer, (object) this.IsLocal, (object) this.SecurityToken, (object) this.EffectivePermissions, (object) this.LastAccessDate, (object) this.Options, (object) this.PendingChangeSignature);
  }
}
