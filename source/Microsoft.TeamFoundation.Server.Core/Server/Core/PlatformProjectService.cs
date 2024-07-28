// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.PlatformProjectService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Core.WebApi.Team;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.TeamFoundation.Server.Core.Audit;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class PlatformProjectService : BaseProjectService
  {
    private const string c_disableFetchPreCreatedProjectFromDb = "Project.PreCreation.DisableFetchPreCreatedProjectFromDb";
    private const string c_disableHidePreCreatedProjectFeature = "Project.PreCreation.DisableHidePreCreatedProject";
    private static readonly string[] s_hostedReservedNames = new string[2]
    {
      "DefaultCollection",
      "Web"
    };
    internal static readonly int s_defaultSecondsUntilDeleteProjectIsAllowed = 300;
    private static readonly string s_area = "Project";
    private static readonly string s_layer = nameof (PlatformProjectService);
    private static readonly string s_secondsUntilDeleteProjectIsAllowedRegistryPath = FrameworkServerConstants.CollectionsRoot + "/SecondsUntilDeleteProjectIsAllowed";
    private static readonly TimeSpan s_collectionVisibilityLeaseTime = TimeSpan.FromSeconds(60.0);

    public override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
    }

    public override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    protected override ProjectInfo GetProjectFromSource(
      IVssRequestContext requestContext,
      string projectName,
      bool refreshStateIfNecessary)
    {
      requestContext.TraceEnter(5500300, this.Area, this.Layer, nameof (GetProjectFromSource));
      try
      {
        ProjectCacheService service = requestContext.GetService<ProjectCacheService>();
        ProjectInfo projectFromSource = service.GetProject(requestContext, projectName);
        if (((projectFromSource == null ? 0 : (projectFromSource.State != ProjectState.WellFormed ? 1 : (projectFromSource.Id == service.PreCreatedProjectId ? 1 : 0))) & (refreshStateIfNecessary ? 1 : 0)) != 0)
          projectFromSource = service.RefreshProject(requestContext, projectFromSource.Id);
        return projectFromSource;
      }
      finally
      {
        requestContext.TraceLeave(5500301, this.Area, this.Layer, nameof (GetProjectFromSource));
      }
    }

    protected override ProjectInfo GetProjectFromSource(
      IVssRequestContext requestContext,
      Guid projectId,
      bool refreshStateIfNecessary)
    {
      requestContext.TraceEnter(5500302, this.Area, this.Layer, nameof (GetProjectFromSource));
      try
      {
        ProjectCacheService service = requestContext.GetService<ProjectCacheService>();
        ProjectInfo projectFromSource = service.GetProject(requestContext, projectId);
        IVssRequestContext requestContext1 = requestContext;
        string area1 = this.Area;
        string layer1 = this.Layer;
        // ISSUE: variable of a boxed type
        __Boxed<Guid> local1 = (ValueType) projectId;
        ProjectState state;
        string str1;
        if (projectFromSource != null)
        {
          state = projectFromSource.State;
          str1 = state.ToString();
        }
        else
          str1 = "NULL";
        string message1 = string.Format("Retrieved project from cache. projectId={0}, state={1}.", (object) local1, (object) str1);
        requestContext1.Trace(5069160, TraceLevel.Info, area1, layer1, message1);
        if (((projectFromSource == null ? 0 : (projectFromSource.State != ProjectState.WellFormed ? 1 : (projectFromSource.Id == service.PreCreatedProjectId ? 1 : 0))) & (refreshStateIfNecessary ? 1 : 0)) != 0)
        {
          projectFromSource = service.RefreshProject(requestContext, projectFromSource.Id);
          IVssRequestContext requestContext2 = requestContext;
          string area2 = PlatformProjectService.s_area;
          string layer2 = PlatformProjectService.s_layer;
          // ISSUE: variable of a boxed type
          __Boxed<Guid> local2 = (ValueType) projectId;
          string str2;
          if (projectFromSource != null)
          {
            state = projectFromSource.State;
            str2 = state.ToString();
          }
          else
            str2 = "NULL";
          string message2 = string.Format("Cached refreshed. Retrieved project from db. projectId={0}, state={1}.", (object) local2, (object) str2);
          requestContext2.Trace(5069160, TraceLevel.Info, area2, layer2, message2);
        }
        return projectFromSource;
      }
      finally
      {
        requestContext.TraceLeave(5500303, this.Area, this.Layer, nameof (GetProjectFromSource));
      }
    }

    protected override IList<ProjectInfo> GetProjectsFromSource(
      IVssRequestContext requestContext,
      ProjectState state)
    {
      requestContext.TraceEnter(5500304, this.Area, this.Layer, nameof (GetProjectsFromSource));
      try
      {
        ProjectCacheService service = requestContext.GetService<ProjectCacheService>();
        IList<ProjectInfo> projects = service.GetProjects(requestContext, state);
        if (projects.Count == 1 && projects[0].Id == service.PreCreatedProjectId && !requestContext.IsFeatureEnabled("Project.PreCreation.DisableFetchPreCreatedProjectFromDb"))
        {
          ProjectInfo projectInfo = service.RefreshProject(requestContext, projects[0].Id);
          if (projectInfo == null)
            projects.RemoveAt(0);
          else if (projectInfo.Name != projects[0].Name)
            projects[0] = projectInfo;
          else if (!requestContext.IsSystemContext && !requestContext.IsFeatureEnabled("Project.PreCreation.DisableHidePreCreatedProject"))
            projects.RemoveAt(0);
        }
        return projects;
      }
      finally
      {
        requestContext.TraceLeave(5500305, this.Area, this.Layer, nameof (GetProjectsFromSource));
      }
    }

    protected override IList<ProjectInfo> GetProjectHistoryFromSource(
      IVssRequestContext requestContext,
      Guid projectId,
      long minRevision = 0)
    {
      requestContext.TraceEnter(5500306, this.Area, this.Layer, nameof (GetProjectHistoryFromSource));
      try
      {
        using (ProjectComponent component = requestContext.CreateComponent<ProjectComponent>())
          return (IList<ProjectInfo>) component.GetProjectHistory(requestContext, projectId, minRevision);
      }
      finally
      {
        requestContext.TraceLeave(5500307, this.Area, this.Layer, nameof (GetProjectHistoryFromSource));
      }
    }

    protected override IList<ProjectInfo> GetProjectHistoryFromSource(
      IVssRequestContext requestContext,
      long minRevision = 0)
    {
      requestContext.TraceEnter(5500308, this.Area, this.Layer, nameof (GetProjectHistoryFromSource));
      try
      {
        using (ProjectComponent component = requestContext.CreateComponent<ProjectComponent>())
          return component.GetProjectHistory(requestContext, minRevision);
      }
      finally
      {
        requestContext.TraceLeave(5500309, this.Area, this.Layer, nameof (GetProjectHistoryFromSource));
      }
    }

    internal new virtual string NormalizeProjectName(
      IVssRequestContext requestContext,
      string projectName,
      string parameterName,
      bool allowGuid = false,
      bool checkValidity = false)
    {
      projectName = base.NormalizeProjectName(requestContext, projectName, parameterName, allowGuid, checkValidity);
      if (checkValidity)
      {
        foreach (string hostedReservedName in PlatformProjectService.s_hostedReservedNames)
        {
          if (TFStringComparer.TeamProjectName.Equals(projectName, hostedReservedName))
            throw new InvalidProjectNameException(projectName);
        }
      }
      return projectName;
    }

    protected override IEnumerable<ProjectProperty> GetProjectPropertiesImpl(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> projectPropertyFilters)
    {
      return requestContext.GetService<PlatformProjectPropertyService>().GetProperties(requestContext, projectId, projectPropertyFilters);
    }

    protected override IEnumerable<ProjectProperties> GetProjectsPropertiesImpl(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds,
      IEnumerable<string> projectPropertyFilters)
    {
      return this.GetProjectProperties(requestContext, projectIds, projectPropertyFilters).Select<KeyValuePair<Guid, IEnumerable<ProjectProperty>>, ProjectProperties>((Func<KeyValuePair<Guid, IEnumerable<ProjectProperty>>, ProjectProperties>) (kvp => new ProjectProperties()
      {
        ProjectId = kvp.Key,
        Properties = kvp.Value
      }));
    }

    protected override void SetProjectPropertiesImpl(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<ProjectProperty> projectProperties)
    {
      requestContext.GetService<PlatformProjectPropertyService>().SetProperties(requestContext, projectId, projectProperties);
    }

    public override ProjectInfo CreateProject(
      IVssRequestContext requestContext,
      Guid projectId,
      string projectName,
      ProjectVisibility projectVisibility,
      string projectAbbreviation = null,
      string projectDescription = null)
    {
      requestContext.TraceEnter(5500310, this.Area, this.Layer, nameof (CreateProject));
      try
      {
        ProjectUtility.CheckOrganizationInReadOnlyMode(requestContext);
        string projectUri = ProjectInfo.GetProjectUri(projectId);
        try
        {
          projectName = this.NormalizeProjectName(requestContext, projectName, nameof (projectName), false, true);
          projectAbbreviation = ProjectInfo.NormalizeProjectAbbreviation(projectAbbreviation, nameof (projectAbbreviation));
          ProjectsUtility.CheckProjectDescription(requestContext, projectDescription);
          ProjectsUtility.CheckProjectVisibility(requestContext, projectVisibility);
          if (projectVisibility == ProjectVisibility.Unchanged || projectVisibility == ProjectVisibility.SystemPrivate)
            throw new ArgumentException(Resources.InvalidCreateProjectVisibilityLevel((object) projectVisibility), nameof (projectVisibility));
          this.CheckGlobalPermission(requestContext, TeamProjectCollectionPermissions.CreateProjects);
          this.AssertProjectNameIsAvailable(requestContext, new Guid?(projectId), projectName, ProjectNameReservationType.Create);
          bool flag = ServiceHostNameHelper.IsPGuid(requestContext.ServiceHost.Name);
          int num = flag ? 1 : 0;
          Guid userId = requestContext.GetUserId();
          Func<KeyValuePair<ProjectOperation, ProjectInfo>> callback = (Func<KeyValuePair<ProjectOperation, ProjectInfo>>) (() =>
          {
            ProjectOperation project;
            using (ProjectComponent component = requestContext.CreateComponent<ProjectComponent>())
              project = component.CreateProject(projectId, projectName, projectAbbreviation, projectDescription, projectId, userId, projectVisibility);
            ProjectInfo projectInfo = new ProjectInfo(projectId, projectName, ProjectState.New, projectVisibility, projectAbbreviation, projectDescription)
            {
              Revision = project.Revision
            };
            return new KeyValuePair<ProjectOperation, ProjectInfo>(project, projectInfo);
          });
          ProjectOperation key;
          ProjectInfo project1;
          try
          {
            KeyValuePair<ProjectOperation, ProjectInfo> keyValuePair = requestContext.GetService<ProjectCacheService>().Synchronize(requestContext, callback);
            key = keyValuePair.Key;
            project1 = keyValuePair.Value;
          }
          catch (ProjectWorkPendingException ex)
          {
            this.ProcessProjectOperations(requestContext, true);
            throw;
          }
          requestContext.GetService<DataspaceService>().CreateDataspaces(requestContext, new string[1]
          {
            "Default"
          }, projectId);
          IdentityDescriptor descriptor = this.EnsureProjectScopeCreated(requestContext, projectUri, projectName);
          LocalSecurityNamespace securityNamespace = requestContext.GetService<LocalSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
          securityNamespace.SetAccessControlEntry(requestContext, securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, (IVssSecurityNamespace) securityNamespace, projectUri), (IAccessControlEntry) new Microsoft.TeamFoundation.Framework.Server.AccessControlEntry(descriptor, TeamProjectPermissions.AllPermissions, 0), false);
          requestContext.GetService<LocalSecurityInvalidationService>().InvalidateSystemStore(requestContext, nameof (PlatformProjectService));
          key.Properties.Add(new ProjectOperationProperty("IsPreCreate", (object) flag));
          this.PublishProjectChanges(requestContext, key, !flag);
          return project1;
        }
        catch
        {
          this.DeleteImsScope(requestContext, projectUri);
          throw;
        }
      }
      finally
      {
        requestContext.TraceLeave(5500311, this.Area, this.Layer, nameof (CreateProject));
      }
    }

    public override void DeleteProject(IVssRequestContext requestContext, Guid projectId) => this.SoftDeleteProject(requestContext, projectId, out ProjectInfo _);

    public void HardDeleteProject(IVssRequestContext requestContext, Guid projectId)
    {
      requestContext.TraceEnter(5500312, this.Area, this.Layer, nameof (HardDeleteProject));
      try
      {
        ProjectUtility.CheckOrganizationInReadOnlyMode(requestContext);
        string projectUri = ProjectInfo.GetProjectUri(projectId);
        ProjectCacheService service1 = requestContext.GetService<ProjectCacheService>();
        try
        {
          this.CheckProjectPermission(requestContext, projectUri, TeamProjectPermissions.Delete);
        }
        catch
        {
          ProjectInfo project = service1.GetProject(requestContext, projectId);
          if (project != null && project.State != ProjectState.WellFormed)
            this.CheckGlobalPermission(requestContext, TeamProjectCollectionPermissions.CreateProjects);
          else
            throw;
        }
        TeamFoundationEventService service2 = requestContext.GetService<TeamFoundationEventService>();
        service2.PublishDecisionPoint(requestContext, (object) new PreProjectDeletionNotification(projectUri));
        LocalSecurityService service3 = requestContext.GetService<LocalSecurityService>();
        LocalSecurityNamespace securityNamespace = service3.GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId);
        try
        {
          securityNamespace.RemoveAccessControlLists(requestContext, (IEnumerable<string>) new string[1]
          {
            securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, (IVssSecurityNamespace) securityNamespace, projectUri)
          }, true);
        }
        catch (DataspaceNotFoundException ex)
        {
        }
        service3.RemoveDataspacedACEs(requestContext, projectId);
        this.DeleteImsScope(requestContext, projectUri);
        requestContext.GetService<PlatformProjectPropertyService>().DeleteAllProperties(requestContext, projectId);
        Guid userId = requestContext.GetUserId();
        Func<KeyValuePair<ProjectOperation, ProjectInfo>> callback = (Func<KeyValuePair<ProjectOperation, ProjectInfo>>) (() =>
        {
          using (ProjectComponent component = requestContext.CreateComponent<ProjectComponent>())
            return new KeyValuePair<ProjectOperation, ProjectInfo>(component.DeleteProject(projectId, userId), (ProjectInfo) null);
        });
        ProjectOperation key = service1.Synchronize(requestContext, callback).Key;
        service2.PublishNotification(requestContext, (object) new ProjectDeletedEvent(projectUri));
        requestContext.GetService<LocalSecurityInvalidationService>().InvalidateSystemStore(requestContext, nameof (PlatformProjectService));
        key.Properties.Add(new ProjectOperationProperty("ShouldInvalidateSystemStore", (object) true));
        this.PublishProjectChanges(requestContext, key, true);
      }
      finally
      {
        requestContext.TraceLeave(5500313, this.Area, this.Layer, nameof (HardDeleteProject));
      }
    }

    public override void DeleteReservedProject(
      IVssRequestContext requestContext,
      Guid pendingProjectGuid)
    {
      requestContext.TraceEnter(5500314, this.Area, this.Layer, nameof (DeleteReservedProject));
      try
      {
        ProjectUtility.CheckOrganizationInReadOnlyMode(requestContext);
        ArgumentUtility.CheckForEmptyGuid(pendingProjectGuid, nameof (pendingProjectGuid));
        this.CheckGlobalPermission(requestContext, TeamProjectCollectionPermissions.CreateProjects);
        Guid userId = requestContext.GetUserId();
        Func<KeyValuePair<ProjectOperation, ProjectInfo>> callback = (Func<KeyValuePair<ProjectOperation, ProjectInfo>>) (() =>
        {
          using (ProjectComponent component = requestContext.CreateComponent<ProjectComponent>())
            return new KeyValuePair<ProjectOperation, ProjectInfo>(component.DeleteReservedProject(pendingProjectGuid, userId), (ProjectInfo) null);
        });
        requestContext.GetService<ProjectCacheService>().Synchronize(requestContext, callback);
      }
      finally
      {
        requestContext.TraceLeave(5500315, this.Area, this.Layer, nameof (DeleteReservedProject));
      }
    }

    public override Guid ReserveProject(
      IVssRequestContext requestContext,
      string projectName,
      Guid? desiredProjectGuid = null)
    {
      requestContext.TraceEnter(5500316, this.Area, this.Layer, nameof (ReserveProject));
      try
      {
        ProjectUtility.CheckOrganizationInReadOnlyMode(requestContext);
        projectName = this.NormalizeProjectName(requestContext, projectName, nameof (projectName), false, true);
        this.CheckGlobalPermission(requestContext, TeamProjectCollectionPermissions.CreateProjects);
        this.AssertProjectNameIsAvailable(requestContext, new Guid?(), projectName, ProjectNameReservationType.Reserve);
        Guid userId = requestContext.GetUserId();
        Guid pendingProjectGuid = desiredProjectGuid ?? Guid.NewGuid();
        Func<KeyValuePair<ProjectOperation, ProjectInfo>> callback = (Func<KeyValuePair<ProjectOperation, ProjectInfo>>) (() =>
        {
          ProjectOperation key;
          using (ProjectComponent component = requestContext.CreateComponent<ProjectComponent>())
            key = component.ReserveProject(pendingProjectGuid, projectName, userId);
          ProjectInfo projectInfo = new ProjectInfo(pendingProjectGuid, projectName, ProjectState.CreatePending)
          {
            Revision = key.Revision
          };
          return new KeyValuePair<ProjectOperation, ProjectInfo>(key, projectInfo);
        });
        requestContext.GetService<ProjectCacheService>().Synchronize(requestContext, callback);
        return pendingProjectGuid;
      }
      finally
      {
        requestContext.TraceLeave(5500317, this.Area, this.Layer, nameof (ReserveProject));
      }
    }

    protected override Guid UpdateProjectImpl(
      IVssRequestContext requestContext,
      ProjectInfo project,
      out ProjectInfo updatedProject)
    {
      requestContext.TraceEnter(5500318, this.Area, this.Layer, nameof (UpdateProjectImpl));
      try
      {
        bool flag1 = project.Name != null;
        bool flag2 = project.Visibility != ProjectVisibility.Unchanged;
        int permission = 0;
        if (project.State == ProjectState.Deleting)
        {
          permission = TeamProjectPermissions.Delete;
        }
        else
        {
          if (flag1 || project.Abbreviation != null)
            permission = TeamProjectPermissions.Rename;
          if (flag2)
            permission |= TeamProjectPermissions.UpdateVisibility;
          if (project.State != ProjectState.Unchanged || project.Description != null || project.Properties != null)
            permission |= TeamProjectPermissions.GenericWrite;
        }
        if (permission == 0)
          throw new InvalidOperationException("The project update is empty and not allowed.");
        this.CheckProjectPermission(requestContext, project.Uri, permission);
        return this.UpdateProject(requestContext, project, false, out updatedProject);
      }
      finally
      {
        requestContext.TraceLeave(5500319, this.Area, this.Layer, nameof (UpdateProjectImpl));
      }
    }

    internal bool CanDeleteProject(IVssRequestContext requestContext, ProjectInfo project)
    {
      if (project.State == ProjectState.WellFormed)
        return true;
      TimeSpan timeSpan = TimeSpan.FromSeconds((double) requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) PlatformProjectService.s_secondsUntilDeleteProjectIsAllowedRegistryPath, PlatformProjectService.s_defaultSecondsUntilDeleteProjectIsAllowed));
      return DateTime.UtcNow - project.LastUpdateTime > timeSpan;
    }

    public virtual Guid AssignProject(
      IVssRequestContext requestContext,
      ProjectInfo project,
      string featuresEnabled = null)
    {
      requestContext.TraceEnter(5500345, this.Area, this.Layer, nameof (AssignProject));
      try
      {
        ProjectUtility.CheckOrganizationInReadOnlyMode(requestContext);
        this.CheckGlobalPermission(requestContext, TeamProjectCollectionPermissions.CreateProjects);
        if (!string.IsNullOrEmpty(featuresEnabled))
          ProjectsUtility.SetProjectFeatures(requestContext.Elevate(), project.Id, featuresEnabled);
        this.RenameDefaultTeam(requestContext, project);
        Guid guid = this.UpdateProject(requestContext, project, true, out ProjectInfo _);
        this.SetProjectProperties(requestContext.Elevate(), project.Id, (IEnumerable<ProjectProperty>) new ProjectProperty[1]
        {
          new ProjectProperty("System.ProjectPreCreated", (object) null)
        });
        return guid;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5500347, this.Area, this.Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5500346, this.Area, this.Layer, nameof (AssignProject));
      }
    }

    internal Guid SoftDeleteProject(
      IVssRequestContext requestContext,
      Guid projectId,
      out ProjectInfo softDeletedProject)
    {
      requestContext.TraceEnter(5500342, this.Area, this.Layer, nameof (SoftDeleteProject));
      try
      {
        ProjectUtility.CheckOrganizationInReadOnlyMode(requestContext);
        TeamProjectUtil.CheckAcquireProjectLease(requestContext, projectId);
        this.CheckProjectPermission(requestContext, ProjectInfo.GetProjectUri(projectId), TeamProjectPermissions.Delete);
        string projectName = this.GetProjectName(requestContext, projectId);
        IEnumerable<ProjectProperty> projectProperties = this.GetProjectProperties(requestContext, projectId, (IEnumerable<string>) new string[2]
        {
          "System.SoftDeletedProjectName",
          "System.SoftDeletedTimestamp"
        }, true);
        List<ProjectProperty> list = projectProperties != null ? projectProperties.ToList<ProjectProperty>() : (List<ProjectProperty>) null;
        if (list != null && list.Count == 2 && (list[0] != null || list[1] != null))
          requestContext.Trace(5500348, TraceLevel.Error, this.Area, this.Layer, "Found unexpected soft deleted properties for project {0}. SoftDeletedProjectName: {1}; SoftDeletedTimestamp: {2}", (object) projectId, (object) (list[0]?.ToString() ?? "NULL"), (object) (list[1]?.ToString() ?? "NULL"));
        ProjectInfo projectToUpdate = ProjectInfo.GetProjectToUpdate(projectId);
        projectToUpdate.Visibility = ProjectVisibility.SystemPrivate;
        projectToUpdate.Name = "D" + projectId.ToString("D");
        Guid guid = this.UpdateProject(requestContext, projectToUpdate, false, out softDeletedProject);
        this.SetProjectProperties(requestContext.Elevate(), projectToUpdate.Id, (IEnumerable<ProjectProperty>) new ProjectProperty[2]
        {
          new ProjectProperty("System.SoftDeletedProjectName", (object) projectName),
          new ProjectProperty("System.SoftDeletedTimestamp", (object) DateTime.UtcNow)
        }, true);
        return guid;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5500344, this.Area, this.Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5500343, this.Area, this.Layer, nameof (SoftDeleteProject));
      }
    }

    public Guid RecoverProject(
      IVssRequestContext requestContext,
      Guid projectId,
      TeamProject projectUpdate,
      out ProjectInfo recoveredProject)
    {
      requestContext.TraceEnter(5500445, this.Area, this.Layer, nameof (RecoverProject));
      try
      {
        ProjectUtility.CheckOrganizationInReadOnlyMode(requestContext);
        this.CheckProjectPermission(requestContext, ProjectInfo.GetProjectUri(projectId), TeamProjectPermissions.Delete);
        ProjectInfo project = this.GetProject(requestContext, projectId, true);
        if (!project.IsSoftDeleted)
          throw new ProjectDoesNotExistException(Resources.UnableToFindSoftDeletedProject((object) projectId), (Exception) null);
        this.RestoreImsScope(requestContext, project.Uri);
        ProjectInfo projectToUpdate = ProjectInfo.GetProjectToUpdate(projectId);
        projectToUpdate.Visibility = projectUpdate.Visibility == ProjectVisibility.Unchanged ? ProjectVisibility.Private : projectUpdate.Visibility;
        projectToUpdate.Name = string.IsNullOrEmpty(projectUpdate.Name) ? this.GetSoftDeletedProjectOldName(requestContext, projectId) : projectUpdate.Name;
        projectToUpdate.Abbreviation = projectUpdate.Abbreviation;
        projectToUpdate.Description = projectUpdate.Description;
        Guid guid = this.UpdateProject(requestContext, projectToUpdate, false, out recoveredProject);
        this.SetProjectProperties(requestContext.Elevate(), projectToUpdate.Id, (IEnumerable<ProjectProperty>) new ProjectProperty[2]
        {
          new ProjectProperty("System.SoftDeletedProjectName", (object) null),
          new ProjectProperty("System.SoftDeletedTimestamp", (object) null)
        }, true);
        return guid;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5500447, this.Area, this.Layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5500446, this.Area, this.Layer, nameof (RecoverProject));
      }
    }

    public Guid GetPreCreatedProjectId(IVssRequestContext requestContext) => requestContext.GetService<ProjectCacheService>().PreCreatedProjectId;

    internal virtual IReadOnlyDictionary<Guid, IEnumerable<ProjectProperty>> GetProjectProperties(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds,
      IEnumerable<string> projectPropertyFilters)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) projectPropertyFilters, nameof (projectPropertyFilters));
      ProjectProperty.NormalizePropertyNameFilters(ref projectPropertyFilters, nameof (projectPropertyFilters));
      return requestContext.GetService<PlatformProjectPropertyService>().GetProperties(requestContext, projectIds, projectPropertyFilters);
    }

    internal void SetProjectPropertiesInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<ProjectProperty> properties)
    {
      ProjectUtility.CheckOrganizationInReadOnlyMode(requestContext);
      ProjectInfo updatedProject = ProjectInfo.GetProjectToUpdate(projectId);
      updatedProject.Properties = (IList<ProjectProperty>) properties.ToList<ProjectProperty>();
      this.UpdateProjectImpl(requestContext, updatedProject, out updatedProject);
    }

    internal virtual IDictionary<Guid, long> GetProjectWatermarks(IVssRequestContext requestContext)
    {
      using (ProjectComponent component = requestContext.CreateComponent<ProjectComponent>())
        return component.GetProjectWatermarks(requestContext, Guid.Empty);
    }

    internal virtual long GetProjectWatermark(IVssRequestContext requestContext, Guid projectId)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      using (ProjectComponent component = requestContext.CreateComponent<ProjectComponent>())
      {
        long num;
        return component.GetProjectWatermarks(requestContext, projectId).TryGetValue(projectId, out num) ? num : -1L;
      }
    }

    internal virtual void UpdateProjectWatermarks(
      IVssRequestContext requestContext,
      IDictionary<Guid, long> projectRevisions)
    {
      ProjectUtility.CheckOrganizationInReadOnlyMode(requestContext);
      using (ProjectComponent component = requestContext.CreateComponent<ProjectComponent>())
        component.UpdateProjectWatermarks(projectRevisions);
    }

    internal static TeamFoundationJobDefinition CreateWorkerJob(
      ProjectOperation projectOperation,
      JobPriorityClass priority = JobPriorityClass.High)
    {
      return new TeamFoundationJobDefinition(projectOperation.OperationId, string.Format("ProjectUpdateWorker for [{0}, {1}]", (object) projectOperation.ProjectId, (object) projectOperation.Revision), "Microsoft.TeamFoundation.JobService.Extensions.Core.ProjectUpdateJobWorker", TeamFoundationSerializationUtility.SerializeToXml((object) projectOperation), TeamFoundationJobEnabledState.Enabled, false, priority);
    }

    internal bool IsHighPriorityProjectUpdate(ProjectInfo project) => project.Name != null || project.State != ProjectState.Unchanged;

    internal void ProcessProjectOperations(IVssRequestContext requestContext, bool highPriority)
    {
      JobPriorityLevel priorityLevel = highPriority ? JobPriorityLevel.Highest : JobPriorityLevel.Idle;
      ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
      try
      {
        service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          ProjectNotifications.ProjectUpdateJob
        }, priorityLevel);
      }
      catch (JobDefinitionNotFoundException ex)
      {
      }
    }

    internal void PublishProjectChanges(
      IVssRequestContext requestContext,
      ProjectOperation projectOperation,
      bool highPriority,
      bool preCreateWorker = false)
    {
      if (preCreateWorker)
        requestContext.GetService<ITeamFoundationJobService>().UpdateJobDefinitions(requestContext, (IEnumerable<TeamFoundationJobDefinition>) new TeamFoundationJobDefinition[1]
        {
          PlatformProjectService.CreateWorkerJob(projectOperation)
        });
      this.ProcessProjectOperations(requestContext, highPriority);
    }

    internal virtual ChangedProjects GetChangedProjects(
      IVssRequestContext requestContext,
      string revision = null)
    {
      requestContext.TraceEnter(5500320, this.Area, this.Layer, nameof (GetChangedProjects));
      try
      {
        ProjectRevision revision1 = ProjectRevision.FromString(revision);
        IList<ProjectInfo> modifiedProjects;
        IList<ProjectInfo> deletedProjects;
        using (ProjectComponent component = requestContext.CreateComponent<ProjectComponent>())
          component.GetChangedProjects(requestContext, revision1.Modified, revision1.Deleted, out modifiedProjects, out deletedProjects);
        return new ChangedProjects(modifiedProjects, deletedProjects, revision1);
      }
      finally
      {
        requestContext.TraceLeave(5500321, this.Area, this.Layer, nameof (GetChangedProjects));
      }
    }

    internal virtual IDictionary<Guid, IEnumerable<ProjectOperation>> GetUnpublishedProjectChanges(
      IVssRequestContext requestContext)
    {
      using (ProjectComponent component = requestContext.CreateComponent<ProjectComponent>())
        return (IDictionary<Guid, IEnumerable<ProjectOperation>>) component.GetUnpublishedProjectChanges();
    }

    internal void CheckGlobalPermission(IVssRequestContext requestContext, int permission)
    {
      requestContext.TraceEnter(5500322, this.Area, this.Layer, nameof (CheckGlobalPermission));
      try
      {
        IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectCollectionNamespaceId);
        string token = securityNamespace.NamespaceExtension.HandleIncomingToken(requestContext, securityNamespace, FrameworkSecurity.TeamProjectCollectionNamespaceToken);
        securityNamespace.CheckPermission(requestContext, token, permission);
      }
      finally
      {
        requestContext.TraceLeave(5500323, this.Area, this.Layer, nameof (CheckGlobalPermission));
      }
    }

    internal IdentityDescriptor EnsureProjectScopeCreated(
      IVssRequestContext requestContext,
      string projectUri,
      string projectName)
    {
      ProjectUtility.CheckOrganizationInReadOnlyMode(requestContext);
      ITeamFoundationIdentityService service = requestContext.GetService<ITeamFoundationIdentityService>();
      IdentityDescriptor administrators;
      try
      {
        administrators = service.CreateScope(requestContext.Elevate(), projectUri, projectName, Microsoft.TeamFoundation.Framework.Server.ServerResources.GSS_PROJECT_ADMINISTRATORS(), Microsoft.Azure.Boards.CssNodes.ServerResources.GSS_CAN_MANAGE_PROJECT());
      }
      catch (GroupScopeCreationException ex)
      {
        service.GetScopeInfo(requestContext.Elevate(), projectUri, out string _, out administrators, out bool _);
      }
      return administrators;
    }

    internal virtual void TakeDownPublicProjects(
      IVssRequestContext requestContext,
      Guid sourceProjectId)
    {
      ProjectUtility.CheckOrganizationInReadOnlyMode(requestContext);
      requestContext.CheckProjectCollectionRequestContext();
      this.DetermineProjectPermission(requestContext, (string) null, TeamProjectPermissions.UpdateVisibility, true);
      requestContext.GetService<IVssRegistryService>().SetValue<bool>(requestContext, FrameworkServerConstants.AllowPublicProjectsRegistryPath, false);
      List<ProjectInfo> list = this.GetProjectsFromDatabase(requestContext).Where<ProjectInfo>((Func<ProjectInfo, bool>) (x => x.Visibility == ProjectVisibility.Public)).ToList<ProjectInfo>();
      foreach (ProjectInfo projectInfo in list)
      {
        ProjectInfo projectToUpdate = ProjectInfo.GetProjectToUpdate(projectInfo.Id);
        projectToUpdate.Visibility = ProjectVisibility.Private;
        this.UpdateProject(requestContext, projectToUpdate, out ProjectInfo _);
      }
      if (list.Count <= 0)
        return;
      this.SendEmailNotification(requestContext, sourceProjectId);
    }

    internal virtual ProjectInfo GetProject(
      IVssRequestContext requestContext,
      Guid projectId,
      bool includeSystemPrivateProject)
    {
      return this.GetProject(requestContext, projectId, true, includeSystemPrivateProject) ?? throw new ProjectDoesNotExistException(projectId.ToString());
    }

    internal virtual IEnumerable<ProjectInfo> GetSoftDeletedProjects(
      IVssRequestContext requestContext)
    {
      return this.GetProjects(requestContext, ProjectState.All, true).Where<ProjectInfo>((Func<ProjectInfo, bool>) (p => p.IsSoftDeleted));
    }

    private void AssertProjectNameIsAvailable(
      IVssRequestContext requestContext,
      Guid? projectId,
      string projectName,
      ProjectNameReservationType reservationType)
    {
      requestContext.TraceEnter(5500324, this.Area, this.Layer, nameof (AssertProjectNameIsAvailable));
      try
      {
        ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName));
        ProjectInfo projectFromSource = this.GetProjectFromSource(requestContext.Elevate(), projectName, true);
        if (projectFromSource != null && TFStringComparer.TeamProjectName.Equals(projectFromSource.Name, projectName))
        {
          Guid id = projectFromSource.Id;
          Guid? nullable = projectId;
          if ((nullable.HasValue ? (id != nullable.GetValueOrDefault() ? 1 : 0) : 1) != 0)
            throw new ProjectAlreadyExistsException(projectName);
        }
        bool flag;
        using (ProjectComponent component = requestContext.CreateComponent<ProjectComponent>())
          flag = component.HasUnacknowledgedProjectChanges(projectName);
        if (flag)
        {
          if (reservationType == ProjectNameReservationType.Create)
          {
            requestContext.Trace(5500366, TraceLevel.Error, this.Area, this.Layer, "Project ID: {0}", (object) projectFromSource?.Id);
            requestContext.Trace(5500366, TraceLevel.Error, this.Area, this.Layer, "Project State: {0}", (object) projectFromSource?.State);
            foreach (KeyValuePair<Guid, IEnumerable<ProjectOperation>> unpublishedProjectChange in (IEnumerable<KeyValuePair<Guid, IEnumerable<ProjectOperation>>>) this.GetUnpublishedProjectChanges(requestContext))
            {
              foreach (ProjectOperation projectOperation in unpublishedProjectChange.Value)
                requestContext.Trace(5500366, TraceLevel.Error, this.Area, this.Layer, projectOperation.ToString());
            }
          }
          this.ProcessProjectOperations(requestContext, true);
          throw new ProjectWorkPendingException(projectName);
        }
        requestContext.GetService<TeamFoundationEventService>().PublishDecisionPoint(requestContext, (object) new PreProjectNameReservationNotification(projectName, reservationType, projectId));
      }
      finally
      {
        requestContext.TraceLeave(5500325, this.Area, this.Layer, nameof (AssertProjectNameIsAvailable));
      }
    }

    internal void DeleteImsScope(IVssRequestContext requestContext, string projectUri)
    {
      TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
      try
      {
        service.DeleteScope(requestContext.Elevate(), projectUri);
      }
      catch (GroupScopeDoesNotExistException ex)
      {
      }
      catch (AccessCheckException ex)
      {
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(5500122, this.Area, this.Layer, ex);
      }
    }

    private void RestoreImsScope(IVssRequestContext requestContext, string projectUri)
    {
      TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
      try
      {
        service.RestoreScope(requestContext.Elevate(), projectUri);
      }
      catch (RestoreGroupScopeValidationException ex)
      {
      }
      catch (AccessCheckException ex)
      {
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(5500124, this.Area, this.Layer, ex);
      }
    }

    private void RenameDefaultTeam(IVssRequestContext requestContext, ProjectInfo project)
    {
      try
      {
        ITeamService service = requestContext.GetService<ITeamService>();
        WebApiTeam defaultTeam = service.GetDefaultTeam(requestContext, project.Id);
        if (defaultTeam == null)
          return;
        string str = FrameworkResources.ProjectDefaultTeam((object) project.Name);
        service.UpdateTeam(requestContext, project.Id, defaultTeam.Id, new UpdateTeam()
        {
          Name = str,
          Description = defaultTeam.Description
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceCatch(5500125, this.Area, this.Layer, ex);
      }
    }

    private void FireVisibilityChangeCI(
      IVssRequestContext requestContext,
      ProjectVisibility previousVisibility,
      ProjectVisibility newVisibility,
      Guid projectId,
      string projectName)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(CustomerIntelligenceProperty.Action, "UpdateProject");
      properties.Add("ProjectVisibility", (object) newVisibility);
      properties.Add("PreviousProjectVisibility", (object) previousVisibility);
      properties.Add("ProjectName", projectName);
      properties.Add("ProjectId", projectId.ToString());
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, this.Area, CustomerIntelligenceFeature.ProjectEdit, properties);
    }

    private IList<ProjectInfo> GetProjectsFromDatabase(IVssRequestContext requestContext)
    {
      using (ProjectComponent component = requestContext.CreateComponent<ProjectComponent>())
        return component.GetProjects();
    }

    private void SendEmailNotification(IVssRequestContext requestContext, Guid sourceProjectId)
    {
      string urlForNotification = ProjectsUtility.GetPublicUrlForNotification(requestContext);
      string relativePath = (string) null;
      string str = (string) null;
      DateTime? nullable = new DateTime?();
      ProjectInfo projectInfo;
      if (sourceProjectId != Guid.Empty && this.TryGetProject(requestContext, sourceProjectId, out projectInfo))
      {
        relativePath = projectInfo.Name;
        str = UriUtility.Combine(urlForNotification, relativePath, false).ToString();
        nullable = new DateTime?(projectInfo.LastUpdateTime);
      }
      VssNotificationEvent theEvent = new VssNotificationEvent()
      {
        EventType = "ms.vss-tfs.projects-locked-event",
        Data = (object) new ProjectsLockedEvent()
        {
          CollectionUrl = urlForNotification,
          ProjectName = relativePath,
          ProjectUrl = (str ?? urlForNotification)
        },
        ItemId = sourceProjectId.ToString()
      };
      theEvent.SourceEventCreatedTime = nullable;
      Guid owner = requestContext.GetService<ICollectionService>().GetCollection(requestContext, (IEnumerable<string>) null).Owner;
      theEvent.AddActor("receivers", owner);
      Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup
      }, QueryMembership.None, (IEnumerable<string>) null).SingleOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (identity != null)
        theEvent.AddActor("receivers", identity.Id);
      requestContext.GetService<NotificationEventService>().PublishSystemEvent(requestContext, theEvent);
    }

    private Guid UpdateProject(
      IVssRequestContext requestContext,
      ProjectInfo project,
      bool isAssignProject,
      out ProjectInfo updatedProject)
    {
      DateTime utcNow = DateTime.UtcNow;
      project = project.Clone();
      bool flag1 = project.Name != null;
      bool flag2 = isAssignProject ? project.Visibility != 0 : project.Visibility != ProjectVisibility.Unchanged;
      project.Name = flag1 ? this.NormalizeProjectName(requestContext, project.Name, "projectName", false, true) : (string) null;
      project.Abbreviation = ProjectInfo.NormalizeProjectAbbreviation(project.Abbreviation, "projectAbbreviation");
      ProjectsUtility.CheckProjectDescription(requestContext, project.Description);
      ProjectsUtility.CheckProjectVisibility(requestContext, project.Visibility);
      if (project.State == ProjectState.All)
        throw new ArgumentException("Project state cannot be All when updating a project");
      ProjectInfo project1 = this.GetProject(requestContext.Elevate(), project.Id, true);
      bool flag3 = flag1 & flag2 && project.IsSoftDeleted;
      bool flag4 = flag1 & flag2 && project1.IsSoftDeleted;
      string str = string.Empty;
      if (flag1)
      {
        str = project1.Name;
        try
        {
          this.AssertProjectNameIsAvailable(requestContext, new Guid?(project.Id), project.Name, isAssignProject ? ProjectNameReservationType.Create : ProjectNameReservationType.Update);
        }
        catch (Exception ex)
        {
          TeamProjectKpiUtil.PublishProjectRenameData(requestContext, utcNow, DateTime.UtcNow - utcNow, (object) ex);
          throw;
        }
      }
      ProjectVisibility previousVisibility = ProjectVisibility.Unchanged;
      if (flag2)
      {
        previousVisibility = project1.Visibility;
        this.FireVisibilityChangeCI(requestContext, previousVisibility, project.Visibility, project.Id, project.Name ?? str);
        requestContext.GetService<LocalSecurityInvalidationService>().InvalidateSystemStore(requestContext, nameof (PlatformProjectService));
      }
      try
      {
        Guid userId = requestContext.GetUserId();
        Func<KeyValuePair<ProjectOperation, ProjectInfo>> callback = (Func<KeyValuePair<ProjectOperation, ProjectInfo>>) (() =>
        {
          using (ProjectComponent component = requestContext.CreateComponent<ProjectComponent>())
          {
            ProjectInfo updatedProject1;
            return new KeyValuePair<ProjectOperation, ProjectInfo>(component.UpdateProject(project, userId, out updatedProject1), updatedProject1);
          }
        });
        ProjectCacheService service = requestContext.GetService<ProjectCacheService>();
        ProjectOperation key;
        try
        {
          KeyValuePair<ProjectOperation, ProjectInfo> keyValuePair = service.Synchronize(requestContext, callback);
          key = keyValuePair.Key;
          updatedProject = keyValuePair.Value;
        }
        catch (ProjectWorkPendingException ex)
        {
          this.ProcessProjectOperations(requestContext, true);
          throw;
        }
        key.Properties.Add(new ProjectOperationProperty("IsRename", (object) flag1));
        if (flag4)
        {
          key.Properties.Add(new ProjectOperationProperty("IsProjectRecovery", (object) flag4));
          IVssRequestContext requestContext1 = requestContext;
          string actionId = AuditActionState.Queued(ProjectAuditConstants.Restore);
          Dictionary<string, object> data = ProjectAuditData.Restore(updatedProject.Name);
          Guid id = updatedProject.Id;
          Guid targetHostId = new Guid();
          Guid projectId = id;
          requestContext1.LogAuditEvent(actionId, data, targetHostId, projectId);
        }
        else if (flag3)
        {
          key.Properties.Add(new ProjectOperationProperty("PreviousName", (object) str));
          key.Properties.Add(new ProjectOperationProperty("IsProjectSoftDelete", (object) flag3));
          IVssRequestContext requestContext2 = requestContext;
          string actionId = AuditActionState.Queued(ProjectAuditConstants.SoftDelete);
          Dictionary<string, object> data = ProjectAuditData.SoftDelete(str);
          Guid id = updatedProject.Id;
          Guid targetHostId = new Guid();
          Guid projectId = id;
          requestContext2.LogAuditEvent(actionId, data, targetHostId, projectId);
        }
        else if (isAssignProject)
        {
          key.Properties.Add(new ProjectOperationProperty("IsAssignProject", (object) isAssignProject));
        }
        else
        {
          if (flag1)
          {
            key.Properties.Add(new ProjectOperationProperty("PreviousName", (object) str));
            IVssRequestContext requestContext3 = requestContext;
            string actionId = AuditActionState.Queued(ProjectAuditConstants.Rename);
            Dictionary<string, object> data = ProjectAuditData.Rename(updatedProject.Name, (object) str);
            Guid id = updatedProject.Id;
            Guid targetHostId = new Guid();
            Guid projectId = id;
            requestContext3.LogAuditEvent(actionId, data, targetHostId, projectId);
          }
          if (flag2)
          {
            key.Properties.Add(new ProjectOperationProperty("PreviousProjectVisibility", (object) previousVisibility));
            key.Properties.Add(new ProjectOperationProperty("ProjectVisibility", (object) project.Visibility));
            IVssRequestContext requestContext4 = requestContext;
            string actionId = AuditActionState.Queued(ProjectAuditConstants.Visibility);
            Dictionary<string, object> data = ProjectAuditData.Visibility(updatedProject.Name, (object) previousVisibility, (object) project.Visibility);
            Guid id = updatedProject.Id;
            Guid targetHostId = new Guid();
            Guid projectId = id;
            requestContext4.LogAuditEvent(actionId, data, targetHostId, projectId);
          }
        }
        key.Properties.Add(new ProjectOperationProperty("ShouldInvalidateSystemStore", (object) (bool) (flag2 ? 1 : (project.State == ProjectState.WellFormed ? 1 : 0))));
        key.SetCorrelationId(requestContext);
        if (project.State != ProjectState.Unchanged)
        {
          ProjectUpdatedEvent projectUpdatedEvent = new ProjectUpdatedEvent(updatedProject.Uri, updatedProject.Name, updatedProject.State, updatedProject.UserId, updatedProject.LastUpdateTime, updatedProject.Revision);
          ProjectCatalog.Update(requestContext, projectUpdatedEvent);
        }
        IVssRequestContext requestContext5 = requestContext;
        ProjectOperation projectOperation = key;
        int num1 = this.IsHighPriorityProjectUpdate(project) ? 1 : 0;
        ProjectInfo projectInfo = updatedProject;
        int num2 = projectInfo != null ? (projectInfo.State == ProjectState.WellFormed ? 1 : 0) : 0;
        this.PublishProjectChanges(requestContext5, projectOperation, num1 != 0, num2 != 0);
        return key.OperationId;
      }
      catch (Exception ex)
      {
        if (flag1)
          TeamProjectKpiUtil.PublishProjectRenameData(requestContext, utcNow, DateTime.UtcNow - utcNow, failures: (object) ex);
        throw;
      }
    }

    private string GetSoftDeletedProjectOldName(IVssRequestContext requestContext, Guid projectId)
    {
      IEnumerable<ProjectProperty> projectProperties = this.GetProjectProperties(requestContext, projectId, (IEnumerable<string>) new string[1]
      {
        "System.SoftDeletedProjectName"
      }, true);
      return (projectProperties != null ? projectProperties.ToList<ProjectProperty>() : (List<ProjectProperty>) null)[0].Value.ToString();
    }

    public int GetProjectCount(IVssRequestContext requestContext) => this.GetProjectsFromSource(requestContext, ProjectState.WellFormed).Where<ProjectInfo>((Func<ProjectInfo, bool>) (project => !project.IsSoftDeleted)).ToList<ProjectInfo>().Count;

    protected override string Area => PlatformProjectService.s_area;

    protected override string Layer => PlatformProjectService.s_layer;
  }
}
