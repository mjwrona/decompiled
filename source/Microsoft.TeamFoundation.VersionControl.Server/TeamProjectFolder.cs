// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.TeamProjectFolder
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal sealed class TeamProjectFolder
  {
    private ConcurrentDictionary<Guid, TeamProjectFolder.TeamProjectInformation> m_tfvcTeamProjectCache = new ConcurrentDictionary<Guid, TeamProjectFolder.TeamProjectInformation>();
    private ReaderWriterLock m_rwLock = new ReaderWriterLock();
    private ITeamFoundationSqlNotificationService m_sqlNotificationService;
    private bool m_enableExclusiveCheckout = true;

    public TeamProjectFolder(
      VersionControlRequestContext vcSystemRequestContext)
    {
      ArgumentUtility.CheckForNull<VersionControlRequestContext>(vcSystemRequestContext, nameof (vcSystemRequestContext));
      this.RefreshTeamProjectCache(vcSystemRequestContext);
      this.m_sqlNotificationService = vcSystemRequestContext.Elevate().RequestContext.GetService<ITeamFoundationSqlNotificationService>();
      ArgumentUtility.CheckForNull<ITeamFoundationSqlNotificationService>(this.m_sqlNotificationService, "versionControlService.SystemRequestContext.RequestContext.GetService<ITeamFoundationSqlNotificationService>()");
      this.RegisterNotification(vcSystemRequestContext);
    }

    public void Unload(IVssRequestContext requestContext)
    {
      this.UnregisterNotification(requestContext);
      this.m_tfvcTeamProjectCache = (ConcurrentDictionary<Guid, TeamProjectFolder.TeamProjectInformation>) null;
    }

    internal void CreateProjectFolder(
      VersionControlRequestContext versionControlRequestContext,
      TeamProjectFolderOptions options)
    {
      versionControlRequestContext.RequestContext.TraceEnter(700177, TraceArea.TeamProjectCache, TraceLayer.BusinessLogic, nameof (CreateProjectFolder));
      if (string.IsNullOrEmpty(options.TeamProject))
        throw new ArgumentNullException(nameof (options));
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckGSSGlobalPermission(versionControlRequestContext, "CREATE_PROJECTS", Microsoft.TeamFoundation.Server.AuthorizationNamespacePermissions.CreateProjects);
      IVssRequestContext requestContext1 = versionControlRequestContext.RequestContext;
      if (VersionControlPath.IsServerItem(options.TeamProject))
        options.TeamProject = options.TeamProject.Substring("$/".Length);
      IProjectService service1 = requestContext1.GetService<IProjectService>();
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project = service1.GetProject(requestContext1, options.TeamProject, true);
      CatalogNode catalogNode = requestContext1.GetService<CommonStructureService>().QueryProjectCatalogNode(requestContext1, project.Uri);
      DataspaceService service2 = requestContext1.GetService<DataspaceService>();
      service2.CreateDataspaces(requestContext1, new string[2]
      {
        "VersionControl",
        "Git"
      }, project.Id);
      Guid dataspaceIdentifier = versionControlRequestContext.VersionControlService.DebugDataspace(versionControlRequestContext);
      if (dataspaceIdentifier != Guid.Empty)
        service2.CreateDataspaces(requestContext1, new string[1]
        {
          "VersionControl"
        }, dataspaceIdentifier);
      Microsoft.VisualStudio.Services.Identity.Identity identity = versionControlRequestContext.VersionControlService.SecurityWrapper.FindIdentity(versionControlRequestContext.RequestContext);
      if (options.Permissions != null)
        this.ResolveIdentities(versionControlRequestContext, options.Permissions);
      PathLength serverPathLength = versionControlRequestContext.MaxSupportedServerPathLength;
      string str1 = VersionControlPath.PrependRootIfNeeded(options.TeamProject, serverPathLength);
      string str2 = (string) null;
      versionControlRequestContext.Validation.checkServerItem(ref str1, "teamProjectPath", false, false, false, true, serverPathLength);
      if (VersionControlPath.Equals("$/", str1) || !VersionControlPath.Equals("$/", VersionControlPath.GetFolderName(str1)))
        throw new ArgumentException(Resources.Format("InvalidTeamProject", (object) str1));
      string token = (string) null;
      IVssSecurityNamespace securityNamespace = (IVssSecurityNamespace) null;
      IEnumerable<IAccessControlEntry> accessControlEntries = (IEnumerable<IAccessControlEntry>) null;
      if (options.KeepExistingPermissions)
      {
        ITeamFoundationSecurityService service3 = requestContext1.GetService<ITeamFoundationSecurityService>();
        token = VersionControlPath.Combine("$/", project.Id.ToString("D"));
        IVssRequestContext requestContext2 = requestContext1;
        Guid security2NamespaceGuid = SecurityConstants.RepositorySecurity2NamespaceGuid;
        securityNamespace = service3.GetSecurityNamespace(requestContext2, security2NamespaceGuid);
        if (securityNamespace != null)
          accessControlEntries = securityNamespace.QueryAccessControlList(requestContext1, token, (IEnumerable<IdentityDescriptor>) null, false).AccessControlEntries;
      }
      if (!string.IsNullOrEmpty(options.SourceProject))
      {
        str2 = VersionControlPath.PrependRootIfNeeded(options.SourceProject, serverPathLength);
        versionControlRequestContext.Validation.checkServerItem(ref str2, "sourceProjectPath", false, false, true, false, serverPathLength);
        this.m_rwLock.AcquireReaderLock(-1);
        try
        {
          Guid projectId;
          this.TryGetProjectId(requestContext1, str2, out projectId);
          if (!this.TeamProjectCacheContainsKey(versionControlRequestContext, projectId))
          {
            string teamProjectName = VersionControlPath.GetTeamProjectName(str2);
            requestContext1.Trace(700319, TraceLevel.Verbose, TraceArea.TeamProjectCache, TraceLayer.BusinessLogic, "Team project not found in cache: {0}", (object) teamProjectName);
            throw new TeamProjectNotFoundException(teamProjectName);
          }
        }
        finally
        {
          this.m_rwLock.ReleaseReaderLock();
        }
      }
      if (!this.IsValidTeamProject(versionControlRequestContext, str1))
      {
        this.m_rwLock.AcquireWriterLock(-1);
        try
        {
          using (TeamProjectFolderComponent projectFolderComponent = versionControlRequestContext.VersionControlService.GetTeamProjectFolderComponent(versionControlRequestContext))
          {
            projectFolderComponent.CreateProjectFolder(identity.Id, str1, str2, options.Comment, options.ExclusiveCheckout, options.GetLatestOnCheckout, options.CheckinNoteDefinition, options.Permissions, versionControlRequestContext.MaxSupportedServerPathLength);
            versionControlRequestContext.RequestContext.Trace(700179, TraceLevel.Verbose, TraceArea.TeamProjectCache, TraceLayer.BusinessLogic, "Adding team project to the cache: {0}", (object) str1);
            this.m_tfvcTeamProjectCache.TryAdd(project.Id, new TeamProjectFolder.TeamProjectInformation(str1, options.ExclusiveCheckout, options.GetLatestOnCheckout));
          }
          versionControlRequestContext.GetRepositorySecurity().OnDataChanged(versionControlRequestContext.RequestContext);
        }
        finally
        {
          this.m_rwLock.ReleaseWriterLock();
        }
        if (accessControlEntries != null)
          securityNamespace.SetAccessControlEntries(requestContext1, token, accessControlEntries, false, false);
      }
      Guid projectId1 = Microsoft.TeamFoundation.Core.WebApi.ProjectInfo.GetProjectId(project.Uri);
      IEnumerable<Microsoft.TeamFoundation.Core.WebApi.ProjectProperty> projectProperties = service1.GetProjectProperties(requestContext1, projectId1, "System.SourceControlCapabilityFlags");
      List<Microsoft.TeamFoundation.Core.WebApi.ProjectProperty> projectPropertyList = new List<Microsoft.TeamFoundation.Core.WebApi.ProjectProperty>();
      if (!projectProperties.Any<Microsoft.TeamFoundation.Core.WebApi.ProjectProperty>((Func<Microsoft.TeamFoundation.Core.WebApi.ProjectProperty, bool>) (x => x.Name.Equals("System.SourceControlCapabilityFlags"))))
        projectPropertyList.Add(new Microsoft.TeamFoundation.Core.WebApi.ProjectProperty("System.SourceControlCapabilityFlags", (object) RepositoryConstants.SourceControlCapabilityFlag.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      projectPropertyList.Add(new Microsoft.TeamFoundation.Core.WebApi.ProjectProperty("System.SourceControlTfvcEnabled", (object) bool.TrueString));
      service1.CheckProjectPermission(requestContext1, project.Uri, TeamProjectPermissions.GenericWrite);
      service1.SetProjectProperties(requestContext1.Elevate(), projectId1, (IEnumerable<Microsoft.TeamFoundation.Core.WebApi.ProjectProperty>) projectPropertyList);
      if (!catalogNode.Resource.Properties.TryGetValue("SourceControlCapabilityFlags", out string _))
        catalogNode.Resource.Properties["SourceControlCapabilityFlags"] = RepositoryConstants.SourceControlCapabilityFlag.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      catalogNode.Resource.Properties["SourceControlTfvcEnabled"] = bool.TrueString;
      IVssRequestContext vssRequestContext = requestContext1.To(TeamFoundationHostType.Application);
      CatalogTransactionContext transactionContext = vssRequestContext.GetService<ITeamFoundationCatalogService>().CreateTransactionContext();
      transactionContext.AttachResource(catalogNode.Resource);
      transactionContext.Save(vssRequestContext, false);
      versionControlRequestContext.RequestContext.TraceLeave(700180, TraceArea.TeamProjectCache, TraceLayer.BusinessLogic, nameof (CreateProjectFolder));
    }

    internal bool DeleteProjectFolder(
      VersionControlRequestContext versionControlRequestContext,
      string teamProject,
      string teamProjectUri,
      bool startCleanup,
      bool deleteWorkspaceState)
    {
      if (string.IsNullOrEmpty(teamProjectUri))
        throw new ArgumentNullException(nameof (teamProjectUri));
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckGSSProjectPermission(versionControlRequestContext, teamProjectUri, "DELETE", Microsoft.TeamFoundation.Server.AuthorizationProjectPermissions.Delete, false);
      PathLength serverPathLength = versionControlRequestContext.MaxSupportedServerPathLength;
      string path = VersionControlPath.PrependRootIfNeeded(teamProject, serverPathLength);
      this.m_rwLock.AcquireReaderLock(-1);
      try
      {
        Guid projectId;
        this.TryGetProjectId(versionControlRequestContext.RequestContext, path, out projectId);
        if (!this.TeamProjectCacheContainsKey(versionControlRequestContext, projectId))
        {
          versionControlRequestContext.RequestContext.Trace(700181, TraceLevel.Verbose, TraceArea.TeamProjectCache, TraceLayer.BusinessLogic, "Team project not found in cache: {0}", (object) path);
          return false;
        }
      }
      finally
      {
        this.m_rwLock.ReleaseReaderLock();
      }
      versionControlRequestContext.VersionControlService.SecurityWrapper.FindIdentity(versionControlRequestContext.RequestContext);
      DestroyFlags flags = DestroyFlags.Silent;
      if (startCleanup)
        flags |= DestroyFlags.StartCleanup;
      if (deleteWorkspaceState)
        flags |= DestroyFlags.DeleteWorkspaceState;
      Failure[] failures = Array.Empty<Failure>();
      VersionControlRequestContext versionControlRequestContext1 = versionControlRequestContext.RequestContext.IsSystemContext || versionControlRequestContext.RequestContext.IsServicingContext ? versionControlRequestContext : new VersionControlRequestContext(versionControlRequestContext.RequestContext.Elevate());
      using (versionControlRequestContext.RequestContext.AcquireExemptionLock())
      {
        DestroyRequest destroyRequest = new DestroyRequest(versionControlRequestContext1);
        try
        {
          destroyRequest.DestroyItems(new ItemSpec(path, RecursionType.Full), (VersionSpec) new LatestVersionSpec(), (VersionSpec) null, (int) flags, out failures, out StreamingCollection<PendingSet> _, out StreamingCollection<PendingSet> _);
        }
        catch (ItemNotFoundException ex)
        {
        }
      }
      return failures.Length == 0;
    }

    internal void RemoveProject(
      VersionControlRequestContext versionControlRequestContext,
      string teamProjectPath)
    {
      this.m_rwLock.AcquireWriterLock(-1);
      try
      {
        Guid projectId;
        this.TryGetProjectId(versionControlRequestContext.RequestContext, teamProjectPath, out projectId);
        this.m_tfvcTeamProjectCache.TryRemove(projectId, out TeamProjectFolder.TeamProjectInformation _);
      }
      finally
      {
        this.m_rwLock.ReleaseWriterLock();
      }
    }

    internal bool IsValidTeamProject(VersionControlRequestContext vcRequestContext, string path)
    {
      Guid projectId;
      if (!this.TryGetProjectId(vcRequestContext.RequestContext, path, out projectId))
        return false;
      this.m_rwLock.AcquireReaderLock(-1);
      try
      {
        return this.TeamProjectCacheContainsKey(vcRequestContext, projectId);
      }
      finally
      {
        this.m_rwLock.ReleaseReaderLock();
      }
    }

    internal void EnableExclusiveCheckout(bool enableExclusiveCheckout) => this.m_enableExclusiveCheckout = enableExclusiveCheckout;

    internal bool IsExclusiveCheckout(IVssRequestContext requestContext, string serverItem)
    {
      if (!this.m_enableExclusiveCheckout)
        return false;
      bool flag = false;
      this.m_rwLock.AcquireReaderLock(-1);
      try
      {
        Guid projectId;
        this.TryGetProjectId(requestContext, VersionControlPath.GetTeamProject(serverItem), out projectId);
        TeamProjectFolder.TeamProjectInformation projectInformation;
        if (this.m_tfvcTeamProjectCache.TryGetValue(projectId, out projectInformation))
        {
          requestContext.Trace(700182, TraceLevel.Verbose, TraceArea.TeamProjectCache, TraceLayer.BusinessLogic, "Team project found in cache: {0}", (object) projectInformation.m_teamProjectFolder);
          flag = projectInformation.m_exclusiveCheckout;
        }
      }
      finally
      {
        this.m_rwLock.ReleaseReaderLock();
      }
      return flag;
    }

    internal bool IsGetLatestOnCheckout(IVssRequestContext requestContext, string serverItem)
    {
      bool latestOnCheckout = false;
      this.m_rwLock.AcquireReaderLock(-1);
      try
      {
        Guid projectId;
        this.TryGetProjectId(requestContext, VersionControlPath.GetTeamProject(serverItem), out projectId);
        TeamProjectFolder.TeamProjectInformation projectInformation;
        if (this.m_tfvcTeamProjectCache.TryGetValue(projectId, out projectInformation))
        {
          requestContext.Trace(700183, TraceLevel.Verbose, TraceArea.TeamProjectCache, TraceLayer.BusinessLogic, "Team project found in cache: {0} {1}", (object) projectInformation.m_teamProjectFolder, (object) projectInformation.m_getLatestOnCheckout);
          latestOnCheckout = projectInformation.m_getLatestOnCheckout;
        }
      }
      finally
      {
        this.m_rwLock.ReleaseReaderLock();
      }
      return latestOnCheckout;
    }

    internal void ValidateChange(
      VersionControlRequestContext vcRequestContext,
      ItemType type,
      RequestType requestType,
      string serverItem)
    {
      if (VersionControlPath.IsRootFolder(serverItem))
        return;
      string teamProject = VersionControlPath.GetTeamProject(serverItem);
      this.m_rwLock.AcquireReaderLock(-1);
      try
      {
        Guid projectId;
        this.TryGetProjectId(vcRequestContext.RequestContext, teamProject, out projectId);
        if (VersionControlPath.Equals(teamProject, serverItem))
        {
          if (type == ItemType.File)
            throw new CannotCreateFilesInRootException();
          switch (requestType)
          {
            case RequestType.Add:
              if (this.TeamProjectCacheContainsKey(vcRequestContext, projectId))
                return;
              break;
            case RequestType.Lock:
              return;
          }
          vcRequestContext.RequestContext.Trace(700184, TraceLevel.Verbose, TraceArea.TeamProjectCache, TraceLayer.BusinessLogic, "Team project found in cache: {0}", (object) serverItem);
          throw new InvalidProjectPendingChangeException(teamProject);
        }
        if (!this.TeamProjectCacheContainsKey(vcRequestContext, projectId))
        {
          string teamProjectName = VersionControlPath.GetTeamProjectName(teamProject);
          vcRequestContext.RequestContext.Trace(700185, TraceLevel.Verbose, TraceArea.TeamProjectCache, TraceLayer.BusinessLogic, "Team project not found in cache: {0}", (object) teamProjectName);
          throw new TeamProjectNotFoundException(teamProjectName);
        }
      }
      finally
      {
        this.m_rwLock.ReleaseReaderLock();
      }
    }

    private void ResolveIdentities(
      VersionControlRequestContext versionControlRequestContext,
      TeamProjectFolderPermission[] permissions)
    {
      IVssRequestContext requestContext = versionControlRequestContext.RequestContext;
      versionControlRequestContext.GetRepositorySecurity2();
      foreach (TeamProjectFolderPermission permission in permissions)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = TfvcIdentityHelper.FindIdentity(versionControlRequestContext.RequestContext, permission.IdentityName);
        Microsoft.VisualStudio.Services.Identity.Identity identity2 = requestContext.GetService<LocalSecurityService>().EnsureIdentityIsKnown(requestContext.Elevate(), identity1.Descriptor);
        permission.IdentityId = identity2.Id;
        permission.allowBits = (VersionedItemPermissions) VersionControlUtil.TranslatePermission(typeof (VersionedItemPermissions), permission.AllowPermission, 15871);
        permission.denyBits = (VersionedItemPermissions) VersionControlUtil.TranslatePermission(typeof (VersionedItemPermissions), permission.DenyPermission, 15871);
      }
    }

    internal void RefreshTeamProjectCache(
      VersionControlRequestContext vcSystemRequestContext)
    {
      this.m_rwLock.AcquireWriterLock(-1);
      try
      {
        this.RefreshTeamProjectCacheHelper(vcSystemRequestContext);
      }
      finally
      {
        this.m_rwLock.ReleaseWriterLock();
      }
    }

    private bool TeamProjectCacheContainsKey(
      VersionControlRequestContext vcRequestContext,
      Guid projectId)
    {
      if (this.m_tfvcTeamProjectCache.ContainsKey(projectId))
        return true;
      this.m_rwLock.ReleaseReaderLock();
      try
      {
        this.RefreshTeamProjectCache(vcRequestContext);
      }
      finally
      {
        this.m_rwLock.AcquireReaderLock(-1);
      }
      return this.m_tfvcTeamProjectCache.ContainsKey(projectId);
    }

    private void RefreshTeamProjectCacheHelper(
      VersionControlRequestContext vcSystemRequestContext)
    {
      vcSystemRequestContext.RequestContext.TraceEnter(700186, TraceArea.TeamProjectCache, TraceLayer.BusinessLogic, nameof (RefreshTeamProjectCacheHelper));
      try
      {
        this.m_tfvcTeamProjectCache.Clear();
        foreach (Microsoft.TeamFoundation.Core.WebApi.ProjectInfo projectInfo in ProjectUtility.GetProjectsWithTfvcProperty(vcSystemRequestContext.RequestContext))
          this.m_tfvcTeamProjectCache.TryAdd(projectInfo.Id, new TeamProjectFolder.TeamProjectInformation("$/" + projectInfo.Name.ToString(), false, false));
        using (CommandGetAnnotationProperty annotationProperty = new CommandGetAnnotationProperty(vcSystemRequestContext.Elevate()))
        {
          annotationProperty.Execute("ExclusiveCheckout", "$/", 0, RecursionType.OneLevel);
          TeamProjectFolder.TeamProjectInformation projectInformation;
          foreach (Annotation annotation in annotationProperty.Result)
          {
            Guid projectId;
            this.TryGetProjectId(vcSystemRequestContext.RequestContext, annotation.AnnotatedItem, out projectId);
            if (this.m_tfvcTeamProjectCache.TryGetValue(projectId, out projectInformation))
              projectInformation.m_exclusiveCheckout = string.Equals(annotation.AnnotationValue, bool.TrueString, StringComparison.OrdinalIgnoreCase);
          }
          annotationProperty.Execute("GetLatestOnCheckout", "$/", 0, RecursionType.OneLevel);
          foreach (Annotation annotation in annotationProperty.Result)
          {
            Guid projectId;
            this.TryGetProjectId(vcSystemRequestContext.RequestContext, annotation.AnnotatedItem, out projectId);
            if (this.m_tfvcTeamProjectCache.TryGetValue(projectId, out projectInformation))
              projectInformation.m_getLatestOnCheckout = string.Equals(annotation.AnnotationValue, bool.TrueString, StringComparison.OrdinalIgnoreCase);
          }
        }
        if (vcSystemRequestContext.RequestContext.IsTracing(700187, TraceLevel.Verbose, TraceArea.TeamProjectCache, TraceLayer.BusinessLogic))
        {
          vcSystemRequestContext.RequestContext.Trace(700187, TraceLevel.Verbose, TraceArea.TeamProjectCache, TraceLayer.BusinessLogic, "Contents of the reloaded cache:");
          foreach (TeamProjectFolder.TeamProjectInformation projectInformation in (IEnumerable<TeamProjectFolder.TeamProjectInformation>) this.m_tfvcTeamProjectCache.Values)
            vcSystemRequestContext.RequestContext.Trace(700187, TraceLevel.Verbose, TraceArea.TeamProjectCache, TraceLayer.BusinessLogic, "{0} {1} {2}", (object) projectInformation.m_teamProjectFolder, (object) projectInformation.m_getLatestOnCheckout, (object) projectInformation.m_exclusiveCheckout);
        }
      }
      catch (Exception ex)
      {
        vcSystemRequestContext.RequestContext.TraceException(700190, TraceArea.TeamProjectCache, TraceLayer.BusinessLogic, ex);
        TeamFoundationEventLog.Default.LogException(Resources.Get("TeamProjectInitializeFailed"), ex);
        throw;
      }
      vcSystemRequestContext.RequestContext.TraceLeave(700191, TraceArea.TeamProjectCache, TraceLayer.BusinessLogic, nameof (RefreshTeamProjectCacheHelper));
    }

    private void RegisterNotification(
      VersionControlRequestContext vcSystemRequestContext)
    {
      this.m_sqlNotificationService.RegisterNotification(vcSystemRequestContext.RequestContext, DatabaseCategories.VersionControl, SqlNotificationEventClasses.TeamProjectChanged, new SqlNotificationCallback(this.OnTeamProjectChanged), true);
    }

    private void UnregisterNotification(IVssRequestContext requestContext)
    {
      if (this.m_sqlNotificationService == null)
        return;
      this.m_sqlNotificationService.UnregisterNotification(requestContext, DatabaseCategories.VersionControl, SqlNotificationEventClasses.TeamProjectChanged, new SqlNotificationCallback(this.OnTeamProjectChanged), false);
      this.m_sqlNotificationService = (ITeamFoundationSqlNotificationService) null;
    }

    private void OnTeamProjectChanged(
      IVssRequestContext requestContext,
      Guid projectId,
      string eventData)
    {
      requestContext.Trace(700192, TraceLevel.Verbose, TraceArea.TeamProjectCache, TraceLayer.BusinessLogic, "Entering {0}: {1} {2}", (object) nameof (OnTeamProjectChanged), (object) projectId, (object) eventData);
      this.m_tfvcTeamProjectCache.TryRemove(projectId, out TeamProjectFolder.TeamProjectInformation _);
      requestContext.TraceLeave(700193, TraceArea.TeamProjectCache, TraceLayer.BusinessLogic, nameof (OnTeamProjectChanged));
    }

    private bool TryGetProjectId(
      IVssRequestContext requestContext,
      string path,
      out Guid projectId)
    {
      return ProjectUtility.TryConvertToPathWithProjectId(requestContext, path, out string _, out projectId, out string _);
    }

    internal class TeamProjectInformation
    {
      internal string m_teamProjectFolder;
      internal bool m_exclusiveCheckout;
      internal bool m_getLatestOnCheckout;

      public TeamProjectInformation(
        string teamProjectFolder,
        bool exclusiveCheckout,
        bool getLatestOnCheckout)
      {
        this.m_teamProjectFolder = teamProjectFolder;
        this.m_exclusiveCheckout = exclusiveCheckout;
        this.m_getLatestOnCheckout = getLatestOnCheckout;
      }
    }
  }
}
