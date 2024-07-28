// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.BaseProjectService
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Types
{
  public abstract class BaseProjectService : VssBaseService, IProjectService, IVssFrameworkService
  {
    private const string c_lazyProjectRefreshFeature = "Project.Creation.EnableLazyCacheRefresh";
    private static readonly string s_area = "Project";
    private static readonly string s_layer = nameof (BaseProjectService);

    public abstract void ServiceStart(IVssRequestContext systemRequestContext);

    public abstract void ServiceEnd(IVssRequestContext systemRequestContext);

    public abstract ProjectInfo CreateProject(
      IVssRequestContext requestContext,
      Guid projectGuid,
      string projectName,
      ProjectVisibility projectVisibility,
      string projectAbbreviation = null,
      string projectDescription = null);

    public abstract void DeleteProject(IVssRequestContext requestContext, Guid projectId);

    public abstract void DeleteReservedProject(
      IVssRequestContext requestContext,
      Guid pendingProjectGuid);

    public ProjectInfo GetProject(
      IVssRequestContext requestContext,
      string projectName,
      bool includeHistory = false)
    {
      return this.GetProject(requestContext, projectName, includeHistory, true) ?? throw new ProjectDoesNotExistWithNameException(projectName);
    }

    public bool TryGetProject(
      IVssRequestContext requestContext,
      string projectName,
      out ProjectInfo projectInfo,
      bool includeHistory = false)
    {
      projectInfo = this.GetProject(requestContext, projectName, includeHistory, false);
      return projectInfo != null;
    }

    public ProjectInfo GetProject(IVssRequestContext requestContext, Guid projectId) => this.GetProject(requestContext, projectId, true) ?? throw new ProjectDoesNotExistException(projectId.ToString());

    public bool TryGetProject(
      IVssRequestContext requestContext,
      Guid projectId,
      out ProjectInfo projectInfo)
    {
      projectInfo = this.GetProject(requestContext, projectId, false);
      return projectInfo != null;
    }

    public IList<ProjectInfo> GetProjectHistory(
      IVssRequestContext requestContext,
      Guid projectId,
      long minRevision = 0)
    {
      requestContext.TraceEnter(5500206, this.Area, this.Layer, nameof (GetProjectHistory));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        this.CheckProjectPermission(requestContext, ProjectInfo.GetProjectUri(projectId), TeamProjectPermissions.GenericRead);
        IList<ProjectInfo> historyFromSource = this.GetProjectHistoryFromSource(requestContext, projectId, minRevision);
        return historyFromSource != null && historyFromSource.Count != 0 ? historyFromSource : throw new ProjectDoesNotExistException(projectId.ToString());
      }
      finally
      {
        requestContext.TraceLeave(5500207, this.Area, this.Layer, nameof (GetProjectHistory));
      }
    }

    public IList<ProjectInfo> GetProjectHistory(IVssRequestContext requestContext, long minRevision = 0)
    {
      requestContext.TraceEnter(5500208, this.Area, this.Layer, nameof (GetProjectHistory));
      try
      {
        IList<ProjectInfo> historyFromSource = this.GetProjectHistoryFromSource(requestContext, minRevision);
        return (IList<ProjectInfo>) this.GetAuthorizedProjects(requestContext, (IEnumerable<ProjectInfo>) historyFromSource, true).ToList<ProjectInfo>();
      }
      finally
      {
        requestContext.TraceLeave(5500209, this.Area, this.Layer, nameof (GetProjectHistory));
      }
    }

    public Guid GetProjectId(
      IVssRequestContext requestContext,
      string projectName,
      bool includeHistory = false)
    {
      Guid projectId = this.GetProjectId(requestContext, projectName, includeHistory, true);
      return !(projectId == Guid.Empty) ? projectId : throw new ProjectDoesNotExistWithNameException(projectName);
    }

    public bool TryGetProjectId(
      IVssRequestContext requestContext,
      string projectName,
      out Guid projectId,
      bool includeHistory = false)
    {
      projectId = this.GetProjectId(requestContext, projectName, includeHistory, false);
      return projectId != Guid.Empty;
    }

    public string GetProjectName(IVssRequestContext requestContext, Guid projectId)
    {
      string projectName = this.GetProjectName(requestContext, projectId, true);
      return !string.IsNullOrEmpty(projectName) ? projectName : throw new ProjectDoesNotExistException(projectId.ToString());
    }

    public bool TryGetProjectName(
      IVssRequestContext requestContext,
      Guid projectId,
      out string projectName)
    {
      projectName = this.GetProjectName(requestContext, projectId, false);
      return !string.IsNullOrEmpty(projectName);
    }

    public IEnumerable<ProjectInfo> GetProjects(
      IVssRequestContext requestContext,
      ProjectState state = ProjectState.All)
    {
      return this.GetProjects(requestContext, state, false);
    }

    public string GetSecurityToken(IVssRequestContext requestContext, string projectUri) => TeamProjectSecurityConstants.GetToken(projectUri);

    public abstract Guid ReserveProject(
      IVssRequestContext requestContext,
      string projectName,
      Guid? desiredProjectGuid = null);

    public Guid UpdateProject(
      IVssRequestContext requestContext,
      ProjectInfo project,
      out ProjectInfo updatedProject)
    {
      ProjectUtility.CheckOrganizationInReadOnlyMode(requestContext);
      if (!project.IsProjectForUpdate)
        throw new ArgumentException("The project passed in to update project was not created by calling ProjectInfo.GetProjectToUpdate.");
      return project.Properties == null ? this.UpdateProjectImpl(requestContext, project, out updatedProject) : throw new InvalidOperationException("Use SetProjectProperties to update project properties.");
    }

    public virtual IEnumerable<ProjectProperty> GetProjectProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> projectPropertyFilters)
    {
      return this.GetProjectProperties(requestContext, projectId, projectPropertyFilters, false);
    }

    public virtual IEnumerable<ProjectProperties> GetProjectsProperties(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds,
      IEnumerable<string> projectPropertyFilters)
    {
      return this.GetProjectsProperties(requestContext, projectIds, projectPropertyFilters, false);
    }

    public void SetProjectProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<ProjectProperty> projectProperties)
    {
      this.SetProjectProperties(requestContext, projectId, projectProperties, false);
    }

    public virtual ProjectVisibility GetProjectVisibility(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return this.GetProject(requestContext, projectId).Visibility;
    }

    protected abstract ProjectInfo GetProjectFromSource(
      IVssRequestContext requestContext,
      string projectName,
      bool refreshStateIfNecessary);

    protected abstract ProjectInfo GetProjectFromSource(
      IVssRequestContext requestContext,
      Guid projectId,
      bool refreshStateIfNecessary);

    protected abstract IList<ProjectInfo> GetProjectsFromSource(
      IVssRequestContext requestContext,
      ProjectState state);

    protected abstract IList<ProjectInfo> GetProjectHistoryFromSource(
      IVssRequestContext requestContext,
      Guid projectId,
      long minRevision);

    protected abstract IList<ProjectInfo> GetProjectHistoryFromSource(
      IVssRequestContext requestContext,
      long minRevision = 0);

    protected abstract Guid UpdateProjectImpl(
      IVssRequestContext requestContext,
      ProjectInfo project,
      out ProjectInfo updatedProject);

    protected abstract IEnumerable<ProjectProperty> GetProjectPropertiesImpl(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> projectPropertyFilters);

    protected abstract IEnumerable<ProjectProperties> GetProjectsPropertiesImpl(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds,
      IEnumerable<string> projectPropertyFilters);

    protected abstract void SetProjectPropertiesImpl(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<ProjectProperty> projectProperties);

    protected virtual string NormalizeProjectName(
      IVssRequestContext requestContext,
      string projectName,
      string parameterName,
      bool allowGuid = false,
      bool checkValidity = false)
    {
      try
      {
        return ProjectInfo.NormalizeProjectName(projectName, parameterName, allowGuid, checkValidity);
      }
      catch (Exception ex) when (ex is ArgumentException || ex is ArgumentNullException)
      {
        ex.Expected(requestContext.ServiceName);
        throw;
      }
    }

    protected IEnumerable<ProjectInfo> GetProjects(
      IVssRequestContext requestContext,
      ProjectState state,
      bool includeSystemPrivateProject)
    {
      requestContext.TraceEnter(5500204, this.Area, this.Layer, nameof (GetProjects));
      try
      {
        IList<ProjectInfo> projectsFromSource = this.GetProjectsFromSource(requestContext, state);
        return this.FilterProjectsBasedOnPermissions(requestContext, (IEnumerable<ProjectInfo>) projectsFromSource, includeSystemPrivateProject);
      }
      finally
      {
        requestContext.TraceLeave(5500205, this.Area, this.Layer, nameof (GetProjects));
      }
    }

    protected IEnumerable<ProjectProperty> GetProjectProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> projectPropertyFilters,
      bool skipProjectCheck)
    {
      if (!skipProjectCheck)
        this.CheckProjectExists(requestContext, projectId);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) projectPropertyFilters, nameof (projectPropertyFilters));
      ProjectProperty.NormalizePropertyNameFilters(ref projectPropertyFilters, nameof (projectPropertyFilters));
      this.CheckProjectPermission(requestContext, ProjectInfo.GetProjectUri(projectId), TeamProjectPermissions.GenericRead);
      return this.GetProjectPropertiesImpl(requestContext, projectId, projectPropertyFilters);
    }

    protected IEnumerable<ProjectProperties> GetProjectsProperties(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds,
      IEnumerable<string> projectPropertyFilters,
      bool skipProjectCheck)
    {
      if (!(projectIds is ICollection<Guid>))
        projectIds = (IEnumerable<Guid>) projectIds.ToList<Guid>();
      if (!skipProjectCheck)
      {
        foreach (Guid projectId in projectIds)
          this.CheckProjectExists(requestContext, projectId);
      }
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) projectPropertyFilters, nameof (projectPropertyFilters));
      ProjectProperty.NormalizePropertyNameFilters(ref projectPropertyFilters, nameof (projectPropertyFilters));
      foreach (Guid projectId in projectIds)
        this.CheckProjectPermission(requestContext, ProjectInfo.GetProjectUri(projectId), TeamProjectPermissions.GenericRead);
      return this.GetProjectsPropertiesImpl(requestContext, projectIds, projectPropertyFilters);
    }

    protected void SetProjectProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<ProjectProperty> projectProperties,
      bool skipProjectCheck)
    {
      ProjectUtility.CheckOrganizationInReadOnlyMode(requestContext);
      if (!skipProjectCheck)
        this.CheckProjectExists(requestContext, projectId);
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) projectProperties, nameof (projectProperties));
      IList<ProjectProperty> list = (IList<ProjectProperty>) projectProperties.ToList<ProjectProperty>();
      ProjectProperty.NormalizeProperties(ref list, "projectPropertyList");
      string projectUri = ProjectInfo.GetProjectUri(projectId);
      foreach (bool flag in projectProperties.GroupBy<ProjectProperty, bool>((Func<ProjectProperty, bool>) (p => TFStringComparer.TeamProjectPropertyName.StartsWith(p.Name, "System."))).Select<IGrouping<bool, ProjectProperty>, bool>((Func<IGrouping<bool, ProjectProperty>, bool>) (p => p.Key)))
        this.CheckProjectPermission(requestContext, projectUri, flag ? TeamProjectPermissions.ManageSystemProperties : TeamProjectPermissions.ManageProperties, !flag);
      this.SetProjectPropertiesImpl(requestContext, projectId, projectProperties);
    }

    internal IEnumerable<ProjectInfo> FilterProjectsBasedOnPermissions(
      IVssRequestContext requestContext,
      IEnumerable<ProjectInfo> projects,
      bool includeSystemPrivateProject = false)
    {
      bool allowOrgAccess;
      bool allowPublicAccess;
      this.GetCollectionAccessPolicies(requestContext, out allowOrgAccess, out allowPublicAccess);
      foreach (ProjectInfo authorizedProject in this.GetAuthorizedProjects(requestContext, projects, includeSystemPrivateProject))
        yield return this.CloneAndClearProperties(authorizedProject, allowOrgAccess, allowPublicAccess);
    }

    private ProjectInfo GetProjectFromSource(
      IVssRequestContext requestContext,
      string projectName,
      bool includeHistory,
      bool refreshStateIfNecessary)
    {
      ProjectInfo projectFromSource = this.GetProjectFromSource(requestContext, projectName, refreshStateIfNecessary);
      if (!includeHistory && projectFromSource != null && !TFStringComparer.TeamProjectName.Equals(projectFromSource.Name, projectName))
        projectFromSource = (ProjectInfo) null;
      return projectFromSource;
    }

    private static bool IsProjectUserVisible(ProjectInfo project, bool includeSystemPrivateProject = false) => includeSystemPrivateProject || project.Visibility != ProjectVisibility.SystemPrivate;

    private IEnumerable<ProjectInfo> GetAuthorizedProjects(
      IVssRequestContext requestContext,
      IEnumerable<ProjectInfo> projects,
      bool includeSystemPrivateProject)
    {
      requestContext.TraceEnter(5500222, this.Area, this.Layer, nameof (GetAuthorizedProjects));
      try
      {
        includeSystemPrivateProject |= requestContext.Items.GetCastedValueOrDefault<string, bool>("IncludeSoftDeletedProjects");
        IVssSecurityNamespace securityNamespace = this.GetProjectSecurityNamespace(requestContext);
        Dictionary<Guid, bool> authzStatus = new Dictionary<Guid, bool>();
        foreach (ProjectInfo project in projects ?? Enumerable.Empty<ProjectInfo>())
        {
          bool flag;
          if (!authzStatus.TryGetValue(project.Id, out flag))
          {
            flag = BaseProjectService.IsProjectUserVisible(project, includeSystemPrivateProject) && this.IsProjectAuthorized(requestContext, securityNamespace, project.Uri);
            authzStatus.Add(project.Id, flag);
          }
          if (flag)
            yield return project;
        }
        securityNamespace = (IVssSecurityNamespace) null;
        authzStatus = (Dictionary<Guid, bool>) null;
      }
      finally
      {
        requestContext.TraceLeave(5500223, this.Area, this.Layer, nameof (GetAuthorizedProjects));
      }
    }

    private IVssSecurityNamespace GetProjectSecurityNamespace(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(5500224, this.Area, this.Layer, nameof (GetProjectSecurityNamespace));
      try
      {
        return requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TeamProjectNamespaceId) ?? throw new ConfigurationErrorsException("Could not find the team project security namespace. Please register a remote team project security namespace pointer for this service host.");
      }
      finally
      {
        requestContext.TraceLeave(5500225, this.Area, this.Layer, nameof (GetProjectSecurityNamespace));
      }
    }

    private bool IsProjectAuthorized(
      IVssRequestContext requestContext,
      IVssSecurityNamespace securityNamespace,
      string projectUri)
    {
      requestContext.TraceEnter(5500226, this.Area, this.Layer, nameof (IsProjectAuthorized));
      try
      {
        return securityNamespace.HasPermission(requestContext, this.GetSecurityToken(requestContext, projectUri), securityNamespace.Description.ReadPermission);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5500250, this.Area, this.Layer, ex);
        return false;
      }
      finally
      {
        requestContext.TraceLeave(5500227, this.Area, this.Layer, nameof (IsProjectAuthorized));
      }
    }

    protected ProjectInfo GetProject(
      IVssRequestContext requestContext,
      string projectName,
      bool includeHistory,
      bool throwOnNoAccess)
    {
      requestContext.TraceEnter(5500200, this.Area, this.Layer, nameof (GetProject));
      try
      {
        projectName = this.NormalizeProjectName(requestContext, projectName, nameof (projectName), true);
        bool refreshStateIfNecessary = !requestContext.IsFeatureEnabled("Project.Creation.EnableLazyCacheRefresh");
        ProjectInfo projectFromSource = this.GetProjectFromSource(requestContext, projectName, includeHistory, refreshStateIfNecessary);
        if (projectFromSource == null || !BaseProjectService.IsProjectUserVisible(projectFromSource) || !this.DetermineProjectPermission(requestContext, projectFromSource.Uri, TeamProjectPermissions.GenericRead, throwOnNoAccess))
          return (ProjectInfo) null;
        bool allowOrgAccess;
        bool allowPublicAccess;
        this.GetCollectionAccessPolicies(requestContext, out allowOrgAccess, out allowPublicAccess);
        return this.CloneAndClearProperties(projectFromSource, allowOrgAccess, allowPublicAccess);
      }
      finally
      {
        requestContext.TraceLeave(5500201, this.Area, this.Layer, nameof (GetProject));
      }
    }

    protected ProjectInfo GetProject(
      IVssRequestContext requestContext,
      Guid projectId,
      bool throwOnNoAccess,
      bool includeSystemPrivateProject = false)
    {
      requestContext.TraceEnter(5500202, this.Area, this.Layer, nameof (GetProject));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        requestContext.Trace(5069160, TraceLevel.Info, this.Area, this.Layer, string.Format("GetProject projectId={0}", (object) projectId));
        if (this.DetermineProjectPermission(requestContext, ProjectInfo.GetProjectUri(projectId), TeamProjectPermissions.GenericRead, throwOnNoAccess))
        {
          requestContext.Trace(5069160, TraceLevel.Info, this.Area, this.Layer, "Permission check succeeded.");
          includeSystemPrivateProject |= requestContext.Items.GetCastedValueOrDefault<string, bool>("IncludeSoftDeletedProjects");
          bool refreshStateIfNecessary = !requestContext.IsFeatureEnabled("Project.Creation.EnableLazyCacheRefresh");
          ProjectInfo projectFromSource = this.GetProjectFromSource(requestContext, projectId, refreshStateIfNecessary);
          if (projectFromSource != null && BaseProjectService.IsProjectUserVisible(projectFromSource, includeSystemPrivateProject))
          {
            bool allowOrgAccess;
            bool allowPublicAccess;
            this.GetCollectionAccessPolicies(requestContext, out allowOrgAccess, out allowPublicAccess);
            return this.CloneAndClearProperties(projectFromSource, allowOrgAccess, allowPublicAccess);
          }
        }
        return (ProjectInfo) null;
      }
      finally
      {
        requestContext.TraceLeave(5500203, this.Area, this.Layer, nameof (GetProject));
      }
    }

    protected Guid GetProjectId(
      IVssRequestContext requestContext,
      string projectName,
      bool includeHistory,
      bool throwOnNoAccess)
    {
      requestContext.TraceEnter(5500210, this.Area, this.Layer, nameof (GetProjectId));
      try
      {
        projectName = this.NormalizeProjectName(requestContext, projectName, nameof (projectName), true);
        ProjectInfo projectFromSource = this.GetProjectFromSource(requestContext, projectName, includeHistory, false);
        return projectFromSource != null && BaseProjectService.IsProjectUserVisible(projectFromSource) && this.DetermineProjectPermission(requestContext, projectFromSource.Uri, TeamProjectPermissions.GenericRead, throwOnNoAccess) ? projectFromSource.Id : Guid.Empty;
      }
      finally
      {
        requestContext.TraceLeave(5500211, this.Area, this.Layer, nameof (GetProjectId));
      }
    }

    protected string GetProjectName(
      IVssRequestContext requestContext,
      Guid projectId,
      bool throwOnNoAccess,
      bool includeSystemPrivateProject = false)
    {
      requestContext.TraceEnter(5500212, this.Area, this.Layer, nameof (GetProjectName));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        if (this.DetermineProjectPermission(requestContext, ProjectInfo.GetProjectUri(projectId), TeamProjectPermissions.GenericRead, throwOnNoAccess))
        {
          ProjectInfo projectFromSource = this.GetProjectFromSource(requestContext, projectId, false);
          includeSystemPrivateProject |= requestContext.Items.GetCastedValueOrDefault<string, bool>("IncludeSoftDeletedProjects");
          if (projectFromSource != null && BaseProjectService.IsProjectUserVisible(projectFromSource, includeSystemPrivateProject))
            return projectFromSource.Name;
        }
        return string.Empty;
      }
      finally
      {
        requestContext.TraceLeave(5500213, this.Area, this.Layer, nameof (GetProjectName));
      }
    }

    private ProjectInfo CloneAndClearProperties(
      ProjectInfo project,
      bool allowOrgAccess,
      bool allowPublicProject)
    {
      project = project.Clone();
      project.Properties = (IList<ProjectProperty>) null;
      project.Visibility = this.GetProjectRuntimeVisibility(project, allowOrgAccess, allowPublicProject);
      return project;
    }

    private void GetCollectionAccessPolicies(
      IVssRequestContext requestContext,
      out bool allowOrgAccess,
      out bool allowPublicAccess)
    {
      allowOrgAccess = false;
      allowPublicAccess = false;
      if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      IOrganizationPolicyService service = requestContext.GetService<IOrganizationPolicyService>();
      allowOrgAccess = service.GetPolicy<bool>(requestContext.Elevate(), "Policy.AllowOrgAccess", true).EffectiveValue;
      allowPublicAccess = service.GetPolicy<bool>(requestContext.Elevate(), "Policy.AllowAnonymousAccess", false).EffectiveValue;
    }

    private ProjectVisibility GetProjectRuntimeVisibility(
      ProjectInfo project,
      bool allowOrgAccess,
      bool allowAnonymousAccess)
    {
      if (!allowOrgAccess && project.Visibility == ProjectVisibility.Organization)
        return ProjectVisibility.Private;
      if (allowAnonymousAccess || project.Visibility != ProjectVisibility.Public)
        return project.Visibility;
      return !allowOrgAccess ? ProjectVisibility.Private : ProjectVisibility.Organization;
    }

    protected virtual string Area => BaseProjectService.s_area;

    protected virtual string Layer => BaseProjectService.s_layer;
  }
}
