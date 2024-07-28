// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.RemoteServices.FrameworkProjectService
// Assembly: Microsoft.TeamFoundation.Server.RemoteServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 836F660A-A756-49A7-82F0-68378533B43C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.RemoteServices.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Server.RemoteServices
{
  public class FrameworkProjectService : BaseProjectService
  {
    private const string OptOutOfConcurrencyConsolidatorForFetchProjects = "VisualStudio.Services.Project.OptOutOfConcurrencyConsolidator";
    private const string IncrementalCacheDisabled = "Project.FrameworkProjectService.IncrementalCacheDisabled";
    private readonly Guid fetchProjectsConsolidatorKey = new Guid("00677C28-DADB-4A28-BD68-0D12EADE393A");
    private readonly IConcurrencyConsolidator<Guid, IList<ProjectInfo>> fetchProjectsConsolidator;
    private object m_incrementalCacheLock = new object();
    private FrameworkProjectService.FrameworkProjectCachePerformanceProvider m_cacheProvider;
    private ProjectCache m_projectCache;
    private bool m_incrementalCacheEnabled;
    private TeamFoundationTask m_expiryTask;
    private ILockName m_expiryLock;
    private const string c_expiryRegistryKey = "/Configuration/Caching/FrameworkProjectCache/ExpiryInterval";
    private static readonly TimeSpan s_defaultExpiry = TimeSpan.FromDays(1.0);
    private static readonly string s_area = "Project";
    private static readonly string s_layer = nameof (FrameworkProjectService);

    public FrameworkProjectService() => this.fetchProjectsConsolidator = (IConcurrencyConsolidator<Guid, IList<ProjectInfo>>) new ConcurrencyConsolidator<Guid, IList<ProjectInfo>>(false, 2);

    internal FrameworkProjectService(
      IConcurrencyConsolidator<Guid, IList<ProjectInfo>> concurrencyConsolidator)
    {
      this.fetchProjectsConsolidator = concurrencyConsolidator;
    }

    public override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      this.m_cacheProvider = new FrameworkProjectService.FrameworkProjectCachePerformanceProvider();
      this.m_expiryLock = this.CreateLockName(systemRequestContext, "ExpiryLock");
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), true, "/Configuration/Caching/FrameworkProjectCache/ExpiryInterval");
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(systemRequestContext, "Default", FrameworkProjectNotification.ProjectUpdated, new SqlNotificationHandler(this.OnProjectChanged), false);
      this.ResetExpiryTask(systemRequestContext, false);
    }

    public override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(systemRequestContext, "Default", FrameworkProjectNotification.ProjectUpdated, new SqlNotificationHandler(this.OnProjectChanged), false);
      systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));
      this.ResetExpiryTask(systemRequestContext, true);
    }

    public override ProjectInfo CreateProject(
      IVssRequestContext requestContext,
      Guid projectGuid,
      string projectName,
      ProjectVisibility projectVisibility,
      string projectAbbreviation = null,
      string projectDescription = null)
    {
      throw new NotImplementedException();
    }

    public override void DeleteProject(IVssRequestContext requestContext, Guid projectId) => throw new NotImplementedException();

    public override void DeleteReservedProject(
      IVssRequestContext requestContext,
      Guid pendingProjectGuid)
    {
      throw new NotImplementedException();
    }

    public override Guid ReserveProject(
      IVssRequestContext requestContext,
      string projectName,
      Guid? desiredProjectGuid = null)
    {
      throw new NotImplementedException();
    }

    protected override Guid UpdateProjectImpl(
      IVssRequestContext requestContext,
      ProjectInfo project,
      out ProjectInfo updatedProject)
    {
      throw new NotImplementedException();
    }

    protected override ProjectInfo GetProjectFromSource(
      IVssRequestContext requestContext,
      string projectName,
      bool refreshStateIfNecessary)
    {
      requestContext.TraceEnter(5500400, this.Area, this.Layer, nameof (GetProjectFromSource));
      try
      {
        return this.GetProjectData(requestContext, projectName);
      }
      finally
      {
        requestContext.TraceLeave(5500401, this.Area, this.Layer, nameof (GetProjectFromSource));
      }
    }

    protected override ProjectInfo GetProjectFromSource(
      IVssRequestContext requestContext,
      Guid projectId,
      bool refreshStateIfNecessary)
    {
      requestContext.TraceEnter(5500402, this.Area, this.Layer, nameof (GetProjectFromSource));
      try
      {
        return this.GetProjectData(requestContext, projectId.ToString());
      }
      finally
      {
        requestContext.TraceLeave(5500403, this.Area, this.Layer, nameof (GetProjectFromSource));
      }
    }

    protected override IList<ProjectInfo> GetProjectsFromSource(
      IVssRequestContext requestContext,
      ProjectState state)
    {
      requestContext.TraceEnter(5500404, this.Area, this.Layer, nameof (GetProjectsFromSource));
      try
      {
        IList<ProjectInfo> projectsData = this.GetProjectsData(requestContext);
        return state == ProjectState.All || state == ProjectState.Unchanged ? projectsData : (IList<ProjectInfo>) projectsData.Where<ProjectInfo>((Func<ProjectInfo, bool>) (p => p.State == state)).ToList<ProjectInfo>();
      }
      finally
      {
        requestContext.TraceLeave(5500405, this.Area, this.Layer, nameof (GetProjectsFromSource));
      }
    }

    protected override IList<ProjectInfo> GetProjectHistoryFromSource(
      IVssRequestContext requestContext,
      Guid projectId,
      long minRevision = 0)
    {
      throw new NotImplementedException();
    }

    protected override IList<ProjectInfo> GetProjectHistoryFromSource(
      IVssRequestContext requestContext,
      long minRevision = 0)
    {
      requestContext.TraceEnter(5500408, this.Area, this.Layer, nameof (GetProjectHistoryFromSource));
      try
      {
        return (IList<ProjectInfo>) requestContext.Elevate().GetClient<ProjectHttpClient>().GetProjectHistoryEntriesAsync(new long?(minRevision)).SyncResult<List<ProjectInfo>>();
      }
      finally
      {
        requestContext.TraceLeave(5500409, this.Area, this.Layer, nameof (GetProjectHistoryFromSource));
      }
    }

    protected override IEnumerable<ProjectProperty> GetProjectPropertiesImpl(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> projectPropertyFilters)
    {
      return requestContext.GetService<FrameworkProjectPropertyService>().GetProperties(requestContext, projectId, projectPropertyFilters);
    }

    protected override IEnumerable<ProjectProperties> GetProjectsPropertiesImpl(
      IVssRequestContext requestContext,
      IEnumerable<Guid> projectIds,
      IEnumerable<string> projectPropertyFilters)
    {
      return requestContext.GetService<FrameworkProjectPropertyService>().GetProjectsProperties(requestContext, projectIds, projectPropertyFilters);
    }

    protected override void SetProjectPropertiesImpl(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<ProjectProperty> projectProperties)
    {
      requestContext.GetService<FrameworkProjectPropertyService>().SetProperties(requestContext, projectId, projectProperties);
    }

    private void OnProjectChanged(IVssRequestContext requestContext, NotificationEventArgs args)
    {
      ProjectInfo project;
      if (this.IsIncrementalCacheEnabled(requestContext) && FrameworkProjectUtilities.TryDeserialize(args.Data, out project))
        this.InvalidateSpecificProjectInCache(requestContext, project);
      else
        this.InvalidateCache();
    }

    private bool IsIncrementalCacheEnabled(IVssRequestContext requestContext) => !requestContext.IsFeatureEnabled("Project.FrameworkProjectService.IncrementalCacheDisabled");

    private void InvalidateSpecificProjectInCache(
      IVssRequestContext requestContext,
      ProjectInfo projectInfo)
    {
      ProjectCache projectCache = this.m_projectCache;
      if (projectCache == null)
        return;
      if (!projectInfo.KnownNames.Any<string>())
        projectInfo.KnownNames = (IList<string>) new List<string>()
        {
          projectInfo.Name
        };
      if (projectInfo.Visibility == ProjectVisibility.SystemPrivate || projectInfo.State == ProjectState.Deleted)
      {
        projectCache.Remove(projectInfo.Id);
        this.m_cacheProvider.NotifyCacheItemsRemoved(1, new MemoryCacheOperationStatistics(-1, 0L));
      }
      else
      {
        projectCache.Update(projectInfo);
        this.m_cacheProvider.NotifyCacheItemsReplaced(1, new MemoryCacheOperationStatistics());
      }
    }

    private void InvalidateCache()
    {
      this.m_projectCache = (ProjectCache) null;
      this.m_cacheProvider.NotifyCacheReset();
    }

    private IList<ProjectInfo> GetProjectsData(IVssRequestContext requestContext)
    {
      if (this.IsCacheDisabled(requestContext))
      {
        this.m_cacheProvider.LogCacheLookup(false);
        return this.FetchProjectsData(requestContext);
      }
      bool cacheMiss;
      ProjectCache projectCache = this.EnsureCache(requestContext, out cacheMiss);
      this.m_cacheProvider.LogCacheLookup(!cacheMiss);
      return (IList<ProjectInfo>) projectCache.GetValues(ProjectState.All).ToList<ProjectInfo>();
    }

    private ProjectCache InitializeProjectCache(IVssRequestContext requestContext)
    {
      IList<ProjectInfo> projects = this.FetchProjectsData(requestContext);
      return new ProjectCache(requestContext, VssCachePerformanceProvider.NoProvider, (IEnumerable<ProjectInfo>) projects);
    }

    private ProjectInfo GetProjectData(IVssRequestContext requestContext, string project)
    {
      if (this.IsCacheDisabled(requestContext))
      {
        this.m_cacheProvider.LogCacheLookup(false);
        return this.FetchProjectData(requestContext, project);
      }
      bool cacheMiss;
      ProjectCache projectCache = this.EnsureCache(requestContext, out cacheMiss);
      Guid result;
      ProjectInfo projectInfo;
      if ((!Guid.TryParse(project, out result) || !projectCache.TryGetValue(result, out projectInfo)) && !projectCache.TryGetValue(project, out projectInfo))
      {
        projectInfo = this.FetchProjectData(requestContext, project);
        if (projectInfo != null)
          projectCache.Update(projectInfo);
        cacheMiss = true;
      }
      this.m_cacheProvider.LogCacheLookup(!cacheMiss);
      return projectInfo;
    }

    private ProjectCache EnsureCache(IVssRequestContext requestContext, out bool cacheMiss)
    {
      this.CheckIncrementalCacheFeatureFlagChange(requestContext);
      ProjectCache projectCache = this.m_projectCache;
      if (projectCache == null)
      {
        projectCache = this.InitializeProjectCache(requestContext);
        this.m_projectCache = projectCache;
        cacheMiss = true;
      }
      else
        cacheMiss = false;
      return projectCache;
    }

    private void CheckIncrementalCacheFeatureFlagChange(IVssRequestContext requestContext)
    {
      bool flag = this.IsIncrementalCacheEnabled(requestContext);
      if (this.m_incrementalCacheEnabled == flag)
        return;
      lock (this.m_incrementalCacheLock)
      {
        if (this.m_incrementalCacheEnabled == flag)
          return;
        if (this.m_projectCache != null)
          this.InvalidateCache();
        this.m_incrementalCacheEnabled = flag;
      }
    }

    private IList<ProjectInfo> FetchProjectsData(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled("VisualStudio.Services.Project.OptOutOfConcurrencyConsolidator") ? requestContext.RunSynchronously<IList<ProjectInfo>>((Func<Task<IList<ProjectInfo>>>) (() => this.FetchProjectsDataInternalAsync(requestContext))) : requestContext.RunSynchronously<IList<ProjectInfo>>((Func<Task<IList<ProjectInfo>>>) (() => this.fetchProjectsConsolidator.RunOnceAsync(this.fetchProjectsConsolidatorKey, (Func<Task<IList<ProjectInfo>>>) (() => this.FetchProjectsDataInternalAsync(requestContext)))));

    private async Task<IList<ProjectInfo>> FetchProjectsDataInternalAsync(
      IVssRequestContext requestContext)
    {
      FrameworkProjectService frameworkProjectService = this;
      requestContext.TraceEnter(5500420, frameworkProjectService.Area, frameworkProjectService.Layer, nameof (FetchProjectsDataInternalAsync));
      IList<ProjectInfo> projectInfoList;
      try
      {
        ProjectHttpClient projectClient = requestContext.Elevate().GetClient<ProjectHttpClient>();
        List<ProjectInfo> projectInfos = new List<ProjectInfo>();
        string str1 = (string) null;
        do
        {
          ProjectHttpClient projectHttpClient = projectClient;
          ProjectState? stateFilter = new ProjectState?(ProjectState.All);
          string str2 = str1;
          int? top = new int?(int.MaxValue);
          int? skip = new int?();
          string continuationToken = str2;
          bool? getDefaultTeamImageUrl = new bool?();
          IPagedList<TeamProjectReference> projects = await projectHttpClient.GetProjects(stateFilter, top, skip, continuationToken: continuationToken, getDefaultTeamImageUrl: getDefaultTeamImageUrl);
          // ISSUE: reference to a compiler-generated method
          projectInfos.AddRange(projects.Where<TeamProjectReference>((Func<TeamProjectReference, bool>) (project => project != null)).Select<TeamProjectReference, ProjectInfo>(new Func<TeamProjectReference, ProjectInfo>(frameworkProjectService.\u003CFetchProjectsDataInternalAsync\u003Eb__32_1)));
          str1 = projects.ContinuationToken;
        }
        while (str1 != null);
        projectInfoList = (IList<ProjectInfo>) projectInfos;
      }
      finally
      {
        requestContext.TraceLeave(5500421, frameworkProjectService.Area, frameworkProjectService.Layer, nameof (FetchProjectsDataInternalAsync));
      }
      return projectInfoList;
    }

    private ProjectInfo FetchProjectData(IVssRequestContext requestContext, string projectName)
    {
      requestContext.TraceEnter(5500422, this.Area, this.Layer, nameof (FetchProjectData));
      try
      {
        ProjectHttpClient client = requestContext.Elevate().GetClient<ProjectHttpClient>();
        try
        {
          return this.ToProjectInfo((TeamProjectReference) client.GetProject(projectName, includeHistory: true).SyncResult<TeamProject>());
        }
        catch (Exception ex) when (ex is ProjectDoesNotExistException || ex is ProjectDoesNotExistWithNameException)
        {
          return (ProjectInfo) null;
        }
      }
      finally
      {
        requestContext.TraceLeave(5500423, this.Area, this.Layer, nameof (FetchProjectData));
      }
    }

    private void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.ResetExpiryTask(requestContext, false);
    }

    private ProjectInfo ToProjectInfo(TeamProjectReference project) => new ProjectInfo(project.Id, project.Name, project.State, project.Visibility, project.Abbreviation, project.Description)
    {
      Revision = project.Revision
    };

    internal bool IsCacheDisabled(IVssRequestContext requestContext) => this.m_expiryTask == null;

    internal void ResetExpiryTask(IVssRequestContext requestContext, bool stopOnly)
    {
      ITeamFoundationTaskService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<ITeamFoundationTaskService>();
      TimeSpan timeSpan = requestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(requestContext, (RegistryQuery) "/Configuration/Caching/FrameworkProjectCache/ExpiryInterval", true, FrameworkProjectService.s_defaultExpiry);
      Action resetCache = (Action) (() =>
      {
        this.m_projectCache?.Clear();
        this.m_projectCache = (ProjectCache) null;
        this.m_cacheProvider.NotifyCacheReset();
      });
      if (timeSpan == TimeSpan.Zero)
        stopOnly = true;
      using (requestContext.Lock(this.m_expiryLock))
      {
        if (this.m_expiryTask != null)
        {
          service.RemoveTask(requestContext, this.m_expiryTask);
          this.m_expiryTask = (TeamFoundationTask) null;
        }
        if (this.m_expiryTask == null)
        {
          if (!stopOnly)
          {
            this.m_expiryTask = new TeamFoundationTask((TeamFoundationTaskCallback) ((rq, state) => resetCache()), (object) null, (int) timeSpan.TotalMilliseconds);
            service.AddTask(requestContext, this.m_expiryTask);
          }
        }
      }
      if (!stopOnly)
        return;
      resetCache();
    }

    protected override string Area => FrameworkProjectService.s_area;

    protected override string Layer => FrameworkProjectService.s_layer;

    private class FrameworkProjectCachePerformanceProvider : VssCachePerformanceProvider
    {
      public FrameworkProjectCachePerformanceProvider()
        : base(nameof (FrameworkProjectService))
      {
      }

      public override void NotifyCacheLookupSucceeded()
      {
      }

      public override void NotifyCacheLookupFailed()
      {
      }

      public void LogCacheLookup(bool foundInCache)
      {
        if (foundInCache)
          base.NotifyCacheLookupSucceeded();
        else
          base.NotifyCacheLookupFailed();
      }
    }
  }
}
