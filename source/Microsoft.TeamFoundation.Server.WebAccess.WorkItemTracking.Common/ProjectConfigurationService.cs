// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ProjectConfigurationService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess.Tvps;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Plugins;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Promote;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Settings.Legacy;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Utilities;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class ProjectConfigurationService : IVssFrameworkService, IProjectConfigurationService
  {
    internal ConcurrentDictionary<Guid, Tuple<ProjectProcessConfiguration, bool>> m_cache = new ConcurrentDictionary<Guid, Tuple<ProjectProcessConfiguration, bool>>();
    internal ConcurrentDictionary<Guid, string> m_teamFieldCache = new ConcurrentDictionary<Guid, string>();

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      TeamFoundationSqlNotificationService notificationService = requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? requestContext.GetService<TeamFoundationSqlNotificationService>() : throw new InvalidOperationException(FrameworkResources.UnexpectedHostType((object) requestContext.ServiceHost.HostType.ToString()));
      foreach (Guid key in ProcessConfigurationServiceNotifications.ProjectConfigurationServiceNotificationsMap.Keys)
        notificationService.RegisterNotification(requestContext, "Default", key, new SqlNotificationCallback(this.OnNotification), false);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
      TeamFoundationSqlNotificationService service = requestContext.GetService<TeamFoundationSqlNotificationService>();
      foreach (Guid key in ProcessConfigurationServiceNotifications.ProjectConfigurationServiceNotificationsMap.Keys)
        service.UnregisterNotification(requestContext, "Default", key, new SqlNotificationCallback(this.OnNotification), false);
    }

    public void SetProcessSettings(
      IVssRequestContext requestContext,
      string projectUri,
      ProjectProcessConfiguration settings)
    {
      this.SetProcessSettings(requestContext, projectUri, settings, false);
    }

    public void ValidateDerivedProcessSettings(
      IVssRequestContext requestContext,
      Guid processId,
      string projectUri,
      ProjectProcessConfiguration settings)
    {
      requestContext.GetService<IProcessWorkItemTypeService>().GetAllWorkItemTypes(requestContext, processId, true, true);
      Guid projectId = ProjectConfigurationService.GetProjectId(projectUri);
      this.AttachNewStatesToProcessSettings(requestContext, projectId, settings);
      ProcessSettingsValidator.Validate(requestContext, settings, projectUri, false);
    }

    public void SetProcessSettings(
      IVssRequestContext requestContext,
      string projectUri,
      ProjectProcessConfiguration settings,
      bool skipValidation)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(projectUri, "project");
      ArgumentUtility.CheckForNull<ProjectProcessConfiguration>(settings, nameof (settings));
      try
      {
        settings.Properties = BugsBehaviorTranslator.TranslateProperties(settings.Properties);
        requestContext.TraceEnter(240111, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (SetProcessSettings));
        Guid projectId = ProjectConfigurationService.GetProjectId(projectUri);
        if (!skipValidation)
          ProcessSettingsValidator.Validate(requestContext, settings, projectUri, false);
        using (ProjectProcessConfigurationComponent component = ProjectProcessConfigurationComponent.CreateComponent(requestContext))
          component.SetProjectProcessConfiguration(projectId, settings);
        this.RemoveProjectSettingsFromCache(projectId);
        this.AddProjectSettingsToCache(requestContext, projectId, settings, !skipValidation);
        requestContext.GetService<TeamFoundationSqlNotificationService>().SendNotification(requestContext, SqlNotificationEventClasses.ProjectSettingsChanged, projectUri);
      }
      finally
      {
        requestContext.TraceLeave(240119, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (SetProcessSettings));
      }
    }

    public ProjectProcessConfiguration GetProcessSettings(
      IVssRequestContext requestContext,
      string projectUri,
      bool validate)
    {
      return this.GetProcessSettings(requestContext, projectUri, validate, false);
    }

    public virtual ProjectProcessConfiguration GetProcessSettings(
      IVssRequestContext requestContext,
      string projectUri,
      bool validate,
      bool bypassCache)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectUri, nameof (projectUri));
      PerformanceScenarioHelper performanceScenarioHelper = (PerformanceScenarioHelper) null;
      try
      {
        requestContext.TraceEnter(240101, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (GetProcessSettings));
        Guid projectId = ProjectConfigurationService.GetProjectId(projectUri);
        ProjectProcessConfiguration settings;
        if (!bypassCache && this.TryGetSettingsFromCache(projectId, validate, out settings))
          return settings;
        performanceScenarioHelper = new PerformanceScenarioHelper(requestContext, "Agile", "ProjectConfigurationService_GetProcessSettings");
        performanceScenarioHelper.Add(nameof (validate), (object) validate);
        performanceScenarioHelper.Add(nameof (bypassCache), (object) bypassCache);
        using (ProjectProcessConfigurationComponent component = ProjectProcessConfigurationComponent.CreateComponent(requestContext))
        {
          settings = component.GetProjectProcessConfiguration(projectId);
          ProcessSettingsValidator.ValidateAndOrderPortfolioBacklogs(settings);
        }
        if (requestContext.IsFeatureEnabled("WebAccess.Agile.BypassProcessConfigValidation") || WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) && this.IsOutOfBoxProject(requestContext, projectId))
        {
          this.AddProjectSettingsToCache(requestContext, projectId, settings, true);
        }
        else
        {
          if (!settings.IsDefault)
            this.AttachNewStatesToProcessSettings(requestContext, projectId, settings);
          if (validate)
          {
            using (performanceScenarioHelper.Measure("Validate"))
              ProcessSettingsValidator.Validate(requestContext, settings, projectUri, true);
          }
          if (validate || !this.TryGetSettingsFromCache(projectId, true, out ProjectProcessConfiguration _))
            this.AddProjectSettingsToCache(requestContext, projectId, settings, validate);
        }
        return settings;
      }
      finally
      {
        performanceScenarioHelper?.EndScenario();
        requestContext.TraceLeave(240109, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (GetProcessSettings));
      }
    }

    public void DeleteProjectSettings(IVssRequestContext requestContext, string projectUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectUri, nameof (projectUri));
      try
      {
        requestContext.TraceEnter(240171, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (DeleteProjectSettings));
        Guid projectId = ProjectConfigurationService.GetProjectId(projectUri);
        using (ProjectProcessConfigurationComponent component = ProjectProcessConfigurationComponent.CreateComponent(requestContext))
          component.DeleteProjectProcessConfiguration(projectId);
      }
      finally
      {
        requestContext.TraceLeave(240179, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (DeleteProjectSettings));
      }
    }

    public IEnumerable<ProjectCategoryStateMap> GetCategoryStates(
      IVssRequestContext requestContext,
      WorkItemTypeEnum categoryType)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (ProjectProcessConfigurationComponent component = ProjectProcessConfigurationComponent.CreateComponent(requestContext))
        return component.GetCategoryStates(categoryType);
    }

    public void OnMigrateProjectsProcess(
      IVssRequestContext requestContext,
      IEnumerable<ProjectInfo> migratedProjects)
    {
      foreach (ProjectInfo migratedProject in migratedProjects)
        this.RemoveProjectSettingsFromCache(migratedProject.Id);
    }

    public IReadOnlyCollection<ProjectGuidWatermarkPair> GetChangedStateProjects(
      IVssRequestContext requestContext,
      int watermark)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.TraceBlock<IReadOnlyCollection<ProjectGuidWatermarkPair>>(290256, 290257, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (GetChangedStateProjects), (Func<IReadOnlyCollection<ProjectGuidWatermarkPair>>) (() =>
      {
        using (ProjectProcessConfigurationComponent component = ProjectProcessConfigurationComponent.CreateComponent(requestContext))
          return component.GetChangedStateProjectsSinceWatermark(watermark);
      }));
    }

    private void AttachNewStatesToProcessSettings(
      IVssRequestContext requestContext,
      Guid projectId,
      ProjectProcessConfiguration settings)
    {
      requestContext.TraceBlock(240190, 240191, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (AttachNewStatesToProcessSettings), (Action) (() =>
      {
        IWorkItemTrackingProcessService service = requestContext.GetService<IWorkItemTrackingProcessService>();
        ProcessDescriptor processDescriptor;
        if (!WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) || !service.TryGetLatestProjectProcessDescriptor(requestContext, projectId, out processDescriptor) || !processDescriptor.IsDerived)
          return;
        IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType> workItemTypes = requestContext.GetService<IWorkItemTypeService>().GetWorkItemTypes(requestContext, projectId);
        IEnumerable<WorkItemTypeCategory> itemTypeCategories = requestContext.GetService<IWorkItemTypeCategoryService>().GetWorkItemTypeCategories(requestContext, projectId);
        foreach (CategoryConfiguration configurationsWithState in (IEnumerable<CategoryConfiguration>) settings.GetAllCategoryConfigurationsWithStates())
        {
          CategoryConfiguration categoryConfig = configurationsWithState;
          WorkItemTypeCategory itemTypeCategory = itemTypeCategories.FirstOrDefault<WorkItemTypeCategory>((Func<WorkItemTypeCategory, bool>) (c => TFStringComparer.WorkItemCategoryReferenceName.Equals(c.ReferenceName, categoryConfig.CategoryReferenceName)));
          if (itemTypeCategory != null)
          {
            List<WorkItemStateDefinition> source = new List<WorkItemStateDefinition>();
            Dictionary<string, IReadOnlyCollection<WorkItemStateDefinition>> dictionary1 = new Dictionary<string, IReadOnlyCollection<WorkItemStateDefinition>>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
            foreach (string workItemTypeName in itemTypeCategory.WorkItemTypeNames)
            {
              string typeName = workItemTypeName;
              Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType workItemType = workItemTypes.FirstOrDefault<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType, bool>) (t => TFStringComparer.WorkItemTypeName.Equals(typeName, t.Name)));
              if (workItemType != null)
              {
                if (workItemType.IsDerived)
                {
                  IReadOnlyCollection<WorkItemStateDefinition> deltaStates = workItemType.InheritedWorkItemType.GetDeltaStates(requestContext);
                  source.AddRange((IEnumerable<WorkItemStateDefinition>) deltaStates);
                  dictionary1[workItemType.Name] = deltaStates;
                }
                else if (workItemType.IsCustomType)
                {
                  IReadOnlyCollection<WorkItemStateDefinition> states = workItemType.Source.GetStates(requestContext);
                  source.AddRange((IEnumerable<WorkItemStateDefinition>) states);
                  dictionary1[workItemType.Name] = states;
                }
              }
            }
            categoryConfig.ProcessWorkItemTypeStates = (IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateDefinition>>) dictionary1;
            if (source.Any<WorkItemStateDefinition>())
            {
              List<string> list1 = source.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => s.Hidden)).Select<WorkItemStateDefinition, string>((Func<WorkItemStateDefinition, string>) (s => s.Name)).ToList<string>();
              List<WorkItemStateDefinition> list2 = source.Where<WorkItemStateDefinition>((Func<WorkItemStateDefinition, bool>) (s => !s.Hidden)).ToList<WorkItemStateDefinition>();
              Dictionary<string, State> dictionary2 = ((IEnumerable<State>) categoryConfig.States).ToDictionary<State, string>((Func<State, string>) (s => s.Value), (IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
              foreach (string key in list1)
                dictionary2.Remove(key);
              foreach (WorkItemStateDefinition itemStateDefinition in list2)
              {
                if (itemStateDefinition.StateCategory != WorkItemStateCategory.Removed)
                  dictionary2[itemStateDefinition.Name] = new State()
                  {
                    Value = itemStateDefinition.Name,
                    Type = ProjectConfigurationService.ConvertStateCategoryToStateType(itemStateDefinition.StateCategory)
                  };
              }
              requestContext.Trace(290426, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, string.Join<State>(",", dictionary2.Select<KeyValuePair<string, State>, State>((Func<KeyValuePair<string, State>, State>) (s => s.Value))));
              categoryConfig.States = dictionary2.Values.ToArray<State>();
            }
          }
        }
      }));
    }

    private static StateTypeEnum ConvertStateCategoryToStateType(WorkItemStateCategory stateCategory)
    {
      switch (stateCategory)
      {
        case WorkItemStateCategory.Proposed:
          return StateTypeEnum.Proposed;
        case WorkItemStateCategory.InProgress:
          return StateTypeEnum.InProgress;
        case WorkItemStateCategory.Resolved:
          return StateTypeEnum.Resolved;
        case WorkItemStateCategory.Completed:
          return StateTypeEnum.Complete;
        default:
          return StateTypeEnum.Removed;
      }
    }

    private bool IsOutOfBoxProject(IVssRequestContext requestContext, Guid projectId)
    {
      try
      {
        ProcessDescriptor processDescriptor;
        requestContext.GetService<IWorkItemTrackingProcessService>().TryGetLatestProjectProcessDescriptor(requestContext, projectId, out processDescriptor);
        return processDescriptor != null && !TeamProjectUtilAzureBoards.IsTpcOptedOutOfPromote(requestContext) && TeamFoundationProjectPromoteService.IsOutOfBoxProcessTemplate(processDescriptor.TypeId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(599999, "Agile", TfsTraceLayers.BusinessLogic, ex);
      }
      return false;
    }

    protected bool TryGetSettingsFromCache(
      Guid projectId,
      bool requireValidation,
      out ProjectProcessConfiguration settings)
    {
      Tuple<ProjectProcessConfiguration, bool> tuple;
      if (this.m_cache.TryGetValue(projectId, out tuple) && (!requireValidation || requireValidation && tuple.Item2))
      {
        settings = tuple.Item1;
        return true;
      }
      settings = (ProjectProcessConfiguration) null;
      return false;
    }

    private void AddProjectSettingsToCache(
      IVssRequestContext requestContext,
      Guid projectId,
      ProjectProcessConfiguration settings,
      bool validated)
    {
      Tuple<ProjectProcessConfiguration, bool> newValue = new Tuple<ProjectProcessConfiguration, bool>(settings, validated);
      this.m_cache.AddOrUpdate(projectId, newValue, (Func<Guid, Tuple<ProjectProcessConfiguration, bool>, Tuple<ProjectProcessConfiguration, bool>>) ((k, v) => newValue));
      this.m_teamFieldCache.TryRemove(projectId, out string _);
    }

    private void RemoveProjectSettingsFromCache(Guid projectId)
    {
      this.m_cache.TryRemove(projectId, out Tuple<ProjectProcessConfiguration, bool> _);
      this.m_teamFieldCache.TryRemove(projectId, out string _);
    }

    private void ClearCache()
    {
      this.m_cache.Clear();
      this.m_teamFieldCache.Clear();
    }

    protected static Guid GetProjectId(string projectUri) => new Guid(CommonStructureUtils.ExtractProjectId(ref projectUri, nameof (projectUri)));

    protected static bool TryGetProjectId(string projectUri, out Guid projectId)
    {
      try
      {
        return Guid.TryParse(CommonStructureUtils.ExtractProjectId(ref projectUri, nameof (projectUri)), out projectId);
      }
      catch (ArgumentException ex)
      {
        projectId = Guid.Empty;
        return false;
      }
    }

    internal void OnNotification(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      bool flag = true;
      Guid projectId1 = Guid.Empty;
      if (eventClass == SqlNotificationEventClasses.ProjectSettingsChanged && ProjectConfigurationService.TryGetProjectId(eventData, out projectId1))
      {
        this.RemoveProjectSettingsFromCache(projectId1);
        flag = false;
      }
      else if (eventClass == SqlNotificationEventClasses.ProjectsProcessMigrated)
      {
        IEnumerable<string> projectSettingsChanged = AgileProcessTemplateExtension.GetProjectUrisFromProjectSettingsChanged(eventData);
        List<Guid> source = new List<Guid>();
        foreach (string projectUri in projectSettingsChanged)
        {
          if (ProjectConfigurationService.TryGetProjectId(projectUri, out projectId1))
            source.Add(projectId1);
        }
        foreach (Guid projectId2 in source)
          this.RemoveProjectSettingsFromCache(projectId2);
        flag = !source.Any<Guid>();
      }
      if (!flag)
        return;
      this.ClearCache();
    }

    internal virtual bool IsTeamFieldAreaPath(IVssRequestContext requestContext, string projectUri) => TFStringComparer.WorkItemFieldReferenceName.Equals("System.AreaPath", this.GetTeamFieldsForProjects(requestContext, (IEnumerable<string>) new string[1]
    {
      projectUri
    })[projectUri]);

    internal virtual IDictionary<string, string> GetTeamFieldsForProjects(
      IVssRequestContext requestContext,
      IEnumerable<string> projectUris)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(projectUris, nameof (projectUris));
      return requestContext.TraceBlock<IDictionary<string, string>>(290253, 290254, 290255, "Agile", TfsTraceLayers.BusinessLogic, (Func<IDictionary<string, string>>) (() =>
      {
        IDictionary<string, string> fieldsForProjects1 = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) TFStringComparer.ProjectUri);
        ISet<string> source = (ISet<string>) new HashSet<string>((IEqualityComparer<string>) TFStringComparer.ProjectUri);
        foreach (string projectUri in projectUris)
        {
          Guid projectId = ProjectConfigurationService.GetProjectId(projectUri);
          ProjectProcessConfiguration settings = (ProjectProcessConfiguration) null;
          string str = (string) null;
          if (this.TryGetSettingsFromCache(projectId, false, out settings))
            fieldsForProjects1[projectUri] = settings.TeamField.Name;
          else if (this.m_teamFieldCache.TryGetValue(projectId, out str))
            fieldsForProjects1[projectUri] = str;
          else
            source.Add(projectUri);
        }
        if (source.Any<string>())
        {
          using (ProjectProcessConfigurationComponent component = ProjectProcessConfigurationComponent.CreateComponent(requestContext))
          {
            Guid[] array = source.Select<string, Guid>((Func<string, Guid>) (projectUri => ProjectConfigurationService.GetProjectId(projectUri))).ToArray<Guid>();
            IDictionary<Guid, string> fieldsForProjects2 = component.GetTeamFieldsForProjects((IEnumerable<Guid>) array);
            foreach (string str in (IEnumerable<string>) source)
            {
              Guid projectId = ProjectConfigurationService.GetProjectId(str);
              string teamFieldRefName;
              if (!fieldsForProjects2.TryGetValue(projectId, out teamFieldRefName))
              {
                requestContext.Trace(290422, TraceLevel.Verbose, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "GetTeamFieldsForProject: Did not find team field setting for project {0}; defaulting to area path.", (object) projectId);
                teamFieldRefName = CoreFieldReferenceNames.AreaPath;
              }
              fieldsForProjects1.Add(str, teamFieldRefName);
              if (!this.m_cache.ContainsKey(projectId))
                this.m_teamFieldCache.AddOrUpdate(projectId, teamFieldRefName, (Func<Guid, string, string>) ((k, v) => teamFieldRefName));
            }
          }
        }
        return fieldsForProjects1;
      }), nameof (GetTeamFieldsForProjects));
    }
  }
}
