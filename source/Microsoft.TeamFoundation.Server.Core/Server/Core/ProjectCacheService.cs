// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectCacheService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectCacheService : VssVersionedMemoryCacheService<Guid, ProjectInfo>
  {
    private static readonly Array s_allProjectStates = Enum.GetValues(typeof (ProjectState));
    private static readonly ProjectInfo Tombstone = new ProjectInfo()
    {
      Revision = long.MaxValue
    };
    private IVssMemoryCacheGrouping<Guid, ProjectInfo, string> m_nameMapping;
    private IVssMemoryCacheGrouping<Guid, ProjectInfo, ProjectState> m_stateMapping;
    private static readonly string s_area = "Project";
    private static readonly string s_layer = nameof (ProjectCacheService);
    private static readonly string s_projectStatsCIFeature = "ProjectStats";

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckProjectCollectionRequestContext();
      base.ServiceStart(systemRequestContext);
      this.m_nameMapping = VssMemoryCacheGroupingFactory.Create<Guid, ProjectInfo, string>(systemRequestContext, this.MemoryCache, (Func<Guid, ProjectInfo, IEnumerable<string>>) ((K, V) => !ProjectCacheService.IsReal(V) ? (IEnumerable<string>) null : (IEnumerable<string>) V.KnownNames), (IEqualityComparer<string>) TFStringComparer.TeamProjectCollectionName);
      this.m_stateMapping = VssMemoryCacheGroupingFactory.Create<Guid, ProjectInfo, ProjectState>(systemRequestContext, this.MemoryCache, (Func<Guid, ProjectInfo, IEnumerable<ProjectState>>) ((K, V) =>
      {
        if (!ProjectCacheService.IsReal(V))
          return (IEnumerable<ProjectState>) null;
        return (IEnumerable<ProjectState>) new ProjectState[1]
        {
          V.State
        };
      }));
      IList<ProjectInfo> projects;
      using (ProjectComponent component = systemRequestContext.CreateComponent<ProjectComponent>("Default"))
        projects = component.GetProjects();
      CustomerIntelligenceService service1 = systemRequestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("ProjectCount", (double) projects.Count);
      IVssRequestContext requestContext = systemRequestContext;
      string projectStatsCiFeature = ProjectCacheService.s_projectStatsCIFeature;
      CustomerIntelligenceData properties = intelligenceData;
      service1.Publish(requestContext, projectStatsCiFeature, nameof (ProjectCacheService), properties);
      foreach (ProjectInfo projectInfo in (IEnumerable<ProjectInfo>) projects)
        this.MemoryCache.Add(projectInfo.Id, projectInfo);
      if (projects.Count == 1)
        this.SetPreCreatedProjectId(systemRequestContext, projects[0].Id, true);
      ITeamFoundationSqlNotificationService service2 = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      service2.RegisterNotification(systemRequestContext, "Default", ProjectNotifications.TeamProjectChanged, new SqlNotificationCallback(this.OnTeamProjectChanged), false);
      service2.RegisterNotification(systemRequestContext, "Default", ProjectNotifications.TeamProjectDeleted, new SqlNotificationCallback(this.OnTeamProjectDeleted), false);
    }

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      ITeamFoundationSqlNotificationService service = systemRequestContext.GetService<ITeamFoundationSqlNotificationService>();
      service.UnregisterNotification(systemRequestContext, "Default", ProjectNotifications.TeamProjectChanged, new SqlNotificationCallback(this.OnTeamProjectChanged), true);
      service.UnregisterNotification(systemRequestContext, "Default", ProjectNotifications.TeamProjectDeleted, new SqlNotificationCallback(this.OnTeamProjectDeleted), true);
      base.ServiceEnd(systemRequestContext);
    }

    public ProjectInfo GetProject(IVssRequestContext requestContext, Guid projectId)
    {
      ProjectInfo projectInfo;
      return this.TryGetValue(requestContext, projectId, out projectInfo) && ProjectCacheService.IsReal(projectInfo) ? projectInfo.Clone() : (ProjectInfo) null;
    }

    public ProjectInfo GetProject(IVssRequestContext requestContext, string projectName)
    {
      ProjectInfo projectInfo = (ProjectInfo) null;
      IEnumerable<Guid> keys;
      if (this.m_nameMapping.TryGetKeys(projectName, out keys))
      {
        foreach (Guid projectId in keys)
        {
          ProjectInfo project = this.GetProject(requestContext, projectId);
          if (project != null && (projectInfo == null || projectInfo.Revision <= project.Revision))
            projectInfo = project;
        }
      }
      return projectInfo?.Clone();
    }

    public IList<ProjectInfo> GetProjects(
      IVssRequestContext requestContext,
      ProjectState projectState)
    {
      IEnumerable<Guid> guids = Enumerable.Empty<Guid>();
      if (projectState == ProjectState.All)
      {
        HashSet<Guid> guidSet = new HashSet<Guid>();
        foreach (ProjectState allProjectState in ProjectCacheService.s_allProjectStates)
        {
          IEnumerable<Guid> keys;
          if (this.m_stateMapping.TryGetKeys(allProjectState, out keys))
            guidSet.UnionWith(keys);
        }
        guids = (IEnumerable<Guid>) guidSet;
      }
      else
      {
        IEnumerable<Guid> keys;
        if (this.m_stateMapping.TryGetKeys(projectState, out keys))
          guids = keys;
      }
      List<ProjectInfo> projects = new List<ProjectInfo>();
      foreach (Guid projectId in guids)
      {
        ProjectInfo project = this.GetProject(requestContext, projectId);
        if (project != null)
          projects.Add(project.Clone());
      }
      return (IList<ProjectInfo>) projects;
    }

    public virtual ProjectInfo RefreshProject(IVssRequestContext requestContext, Guid projectId)
    {
      requestContext.TraceEnter(5500360, ProjectCacheService.s_area, ProjectCacheService.s_layer, nameof (RefreshProject));
      ProjectInfo projectInfo = (ProjectInfo) null;
      try
      {
        using (IVssVersionedCacheContext<Guid, ProjectInfo> versionedContext = this.CreateVersionedContext(requestContext, (IComparer<ProjectInfo>) ProjectCacheService.ProjectInfoComparer.Instance))
        {
          try
          {
            using (ProjectComponent component = requestContext.CreateComponent<ProjectComponent>())
              projectInfo = component.GetProject(projectId);
            int num = (int) versionedContext.TryUpdate(requestContext, projectId, projectInfo);
          }
          catch (ProjectDoesNotExistException ex)
          {
            requestContext.TraceException(5500355, ProjectCacheService.s_area, ProjectCacheService.s_layer, (Exception) ex);
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(5500369, ProjectCacheService.s_area, ProjectCacheService.s_layer, nameof (RefreshProject));
      }
      return projectInfo?.Clone();
    }

    public KeyValuePair<ProjectOperation, ProjectInfo> Synchronize(
      IVssRequestContext requestContext,
      Func<KeyValuePair<ProjectOperation, ProjectInfo>> callback)
    {
      KeyValuePair<ProjectOperation, ProjectInfo> keyValuePair;
      using (IVssVersionedCacheContext<Guid, ProjectInfo> versionedContext = this.CreateVersionedContext(requestContext, (IComparer<ProjectInfo>) ProjectCacheService.ProjectInfoComparer.Instance))
      {
        keyValuePair = callback();
        if (keyValuePair.Value != null)
        {
          int num1 = (int) versionedContext.TryUpdate(requestContext, keyValuePair.Key.ProjectId, keyValuePair.Value);
        }
        else
        {
          int num2 = (int) versionedContext.TryUpdate(requestContext, keyValuePair.Key.ProjectId, ProjectCacheService.Tombstone);
        }
      }
      return new KeyValuePair<ProjectOperation, ProjectInfo>(keyValuePair.Key, keyValuePair.Value?.Clone());
    }

    private void OnTeamProjectChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      Guid result;
      if (Guid.TryParse(eventData, out result))
      {
        this.RefreshProject(requestContext, result);
        this.SetPreCreatedProjectId(requestContext, result);
      }
      else
        requestContext.Trace(5500356, TraceLevel.Error, ProjectCacheService.s_area, ProjectCacheService.s_layer, "Invalid event payload: " + (eventData ?? string.Empty));
    }

    private void OnTeamProjectDeleted(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      Guid result;
      if (Guid.TryParse(eventData, out result))
      {
        ProjectInfo project = this.GetProject(requestContext, result);
        ProjectState projectState = project != null ? project.State : ProjectState.Deleting;
        using (IVssVersionedCacheContext<Guid, ProjectInfo> versionedContext = this.CreateVersionedContext(requestContext, (IComparer<ProjectInfo>) ProjectCacheService.ProjectInfoComparer.Instance))
        {
          int num = (int) versionedContext.TryUpdate(requestContext, result, ProjectCacheService.Tombstone);
        }
        if (projectState != ProjectState.CreatePending)
          requestContext.GetService<TeamFoundationEventService>().PublishNotification(requestContext, (object) new ProjectDeletedEvent(ProjectInfo.GetProjectUri(result)));
        if (!(result == this.PreCreatedProjectId))
          return;
        this.PreCreatedProjectId = Guid.Empty;
      }
      else
        requestContext.Trace(5500357, TraceLevel.Error, ProjectCacheService.s_area, ProjectCacheService.s_layer, "Invalid event payload: " + (eventData ?? string.Empty));
    }

    private void SetPreCreatedProjectId(
      IVssRequestContext requestContext,
      Guid projectId,
      bool isInitialSet = false)
    {
      if (!isInitialSet && !(projectId == this.PreCreatedProjectId))
        return;
      ProjectProperty projectProperty = requestContext.GetService<PlatformProjectPropertyService>().GetProperties(requestContext, projectId, (IEnumerable<string>) new string[1]
      {
        "System.ProjectPreCreated"
      }).SingleOrDefault<ProjectProperty>();
      bool result;
      if (((projectProperty == null ? 0 : (bool.TryParse(projectProperty.Value?.ToString(), out result) ? 1 : 0)) & (result ? 1 : 0)) != 0)
        this.PreCreatedProjectId = projectId;
      else
        this.PreCreatedProjectId = Guid.Empty;
    }

    private static bool IsReal(ProjectInfo projectInfo) => projectInfo != ProjectCacheService.Tombstone;

    internal Guid PreCreatedProjectId { get; private set; }

    private class ProjectInfoComparer : IComparer<ProjectInfo>
    {
      public static readonly ProjectCacheService.ProjectInfoComparer Instance = new ProjectCacheService.ProjectInfoComparer();

      public int Compare(ProjectInfo x, ProjectInfo y)
      {
        ArgumentUtility.CheckForNull<ProjectInfo>(x, nameof (x));
        ArgumentUtility.CheckForNull<ProjectInfo>(y, nameof (y));
        if (x.Revision == y.Revision)
          return 0;
        return x.Revision >= y.Revision ? 1 : -1;
      }
    }
  }
}
