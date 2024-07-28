// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemMetadataFacadeService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class WorkItemMetadataFacadeService : IWorkItemMetadataFacadeService, IVssFrameworkService
  {
    private static IReadOnlyCollection<StateTypeEnum> s_doingStateType = (IReadOnlyCollection<StateTypeEnum>) new List<StateTypeEnum>()
    {
      StateTypeEnum.Proposed,
      StateTypeEnum.InProgress,
      StateTypeEnum.Resolved,
      StateTypeEnum.Received,
      StateTypeEnum.Declined,
      StateTypeEnum.Requested,
      StateTypeEnum.Resolved,
      StateTypeEnum.Reviewed
    };
    private static readonly IReadOnlyCollection<StateTypeEnum> s_doneStateType = (IReadOnlyCollection<StateTypeEnum>) new List<StateTypeEnum>()
    {
      StateTypeEnum.Complete
    };
    private static readonly IReadOnlyCollection<WorkItemStateCategory> s_doingStateCategories = (IReadOnlyCollection<WorkItemStateCategory>) new List<WorkItemStateCategory>()
    {
      WorkItemStateCategory.Proposed,
      WorkItemStateCategory.InProgress,
      WorkItemStateCategory.Resolved
    };
    private static readonly IReadOnlyCollection<WorkItemStateCategory> s_doneStateCategories = (IReadOnlyCollection<WorkItemStateCategory>) new List<WorkItemStateCategory>()
    {
      WorkItemStateCategory.Completed
    };
    private const string s_area = "WebAccess.Settings";
    private const string s_layer = "WorkItemMetadataFacadeService";

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
    }

    public IReadOnlyCollection<string> GetWorkItemStates(
      IVssRequestContext requestContext,
      StateGroup stateGroup,
      bool includeUnmappedStatesWithDone = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.TraceBlock<IReadOnlyCollection<string>>(15163000, 15163001, "WebAccess.Settings", nameof (WorkItemMetadataFacadeService), nameof (GetWorkItemStates), (Func<IReadOnlyCollection<string>>) (() =>
      {
        List<ProjectInfo> list = requestContext.GetService<IProjectService>().GetProjects(requestContext, ProjectState.WellFormed).ToList<ProjectInfo>();
        List<IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>>> stateMaps = new List<IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>>>();
        foreach (ProjectInfo projectInfo in (IEnumerable<ProjectInfo>) list)
          stateMaps.Add(this.GetWorkItemStatesInternal(requestContext, projectInfo.Id));
        return WorkItemMetadataFacadeService.GetMergedStates(requestContext, (IReadOnlyCollection<IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>>>) stateMaps, stateGroup, includeUnmappedStatesWithDone);
      }));
    }

    public IReadOnlyCollection<string> GetWorkItemStates(
      IVssRequestContext requestContext,
      Guid projectId,
      StateGroup stateGroup,
      bool includeUnmappedStatesWithDone = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return requestContext.TraceBlock<IReadOnlyCollection<string>>(15163002, 15163003, "WebAccess.Settings", nameof (WorkItemMetadataFacadeService), nameof (GetWorkItemStates), (Func<IReadOnlyCollection<string>>) (() =>
      {
        IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>> itemStatesInternal = this.GetWorkItemStatesInternal(requestContext, projectId);
        return WorkItemMetadataFacadeService.GetMergedStates(requestContext, (IReadOnlyCollection<IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>>>) new List<IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>>>()
        {
          itemStatesInternal
        }, stateGroup, includeUnmappedStatesWithDone);
      }));
    }

    public IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>> GetWorkItemStateColors(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return requestContext.TraceBlock<IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>>>(15163004, 15163005, "WebAccess.Settings", nameof (WorkItemMetadataFacadeService), nameof (GetWorkItemStateColors), (Func<IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>>>) (() => requestContext.GetService<WorkItemStateColorCacheService>().GetWorkItemStateColorMap(requestContext, projectId, (Func<IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>>>) (() => this.IsStateServiceAvailableForProject(requestContext, projectId) ? this.GetWorkItemStateColorMapFromProcessDescriptor(requestContext, projectId) : this.GetWorkItemStateColorMapFromProcessConfig(requestContext, projectId)))));
    }

    public IReadOnlyCollection<WorkItemStateColor> GetWorkItemStateColors(
      IVssRequestContext requestContext,
      Guid projectId,
      string witName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<string>(witName, nameof (witName));
      return (IReadOnlyCollection<WorkItemStateColor>) requestContext.TraceBlock<List<WorkItemStateColor>>(15163006, 15163007, "WebAccess.Settings", nameof (WorkItemMetadataFacadeService), nameof (GetWorkItemStateColors), (Func<List<WorkItemStateColor>>) (() =>
      {
        IWITProcessReadPermissionCheckHelper permissionChecker = requestContext.WitContext().ProcessReadPermissionChecker;
        ProcessReadSecuredObject securedObject = (ProcessReadSecuredObject) null;
        Guid projectId1 = projectId;
        ref ProcessReadSecuredObject local = ref securedObject;
        if (!permissionChecker.HasProcessReadPermissionForProject(projectId1, out local))
        {
          requestContext.Trace(15114015, TraceLevel.Error, "Agile", "BusinessLogic", "You do not have permissions for obtaining state color for " + witName);
          return new List<WorkItemStateColor>();
        }
        IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>> workItemStateColors1 = this.GetWorkItemStateColors(requestContext, projectId);
        string key = workItemStateColors1.Keys.Where<string>((Func<string, bool>) (k => string.Compare(k, witName, StringComparison.OrdinalIgnoreCase) == 0)).FirstOrDefault<string>();
        if (key != null)
        {
          List<WorkItemStateColor> workItemStateColors2 = new List<WorkItemStateColor>(workItemStateColors1[key].Select<WorkItemStateColor, WorkItemStateColor>((Func<WorkItemStateColor, WorkItemStateColor>) (s => s.Clone())));
          foreach (ProcessReadSecuredObject readSecuredObject in workItemStateColors2)
            readSecuredObject.SetSecuredObjectProperties(securedObject);
          return workItemStateColors2;
        }
        requestContext.Trace(15114015, TraceLevel.Error, "Agile", "BusinessLogic", "WorkItem State color is not available for " + witName);
        return new List<WorkItemStateColor>();
      }));
    }

    public IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>>> GetWorkItemStateColorsByProjectName(
      IVssRequestContext requestContext,
      IReadOnlyCollection<string> projectNames)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IReadOnlyDictionary<Guid, string> projectIdToNames = this.GetValidProjectIdToNameDictionary(requestContext, (IReadOnlyCollection<string>) projectNames.Where<string>((Func<string, bool>) (p => !string.IsNullOrWhiteSpace(p))).ToList<string>());
      return (IReadOnlyDictionary<string, IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>>>) projectIdToNames.Keys.ToDictionary<Guid, string, IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>>>((Func<Guid, string>) (projectId => projectIdToNames[projectId]), (Func<Guid, IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>>>) (projectId => this.GetWorkItemStateColors(requestContext, projectId)), (IEqualityComparer<string>) TFStringComparer.TeamProjectName);
    }

    public bool TryGetWorkItemStateColorsFromCache(
      IVssRequestContext requestContext,
      Guid projectId,
      out IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>> workItemTypeStateColorMap)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return requestContext.GetService<WorkItemStateColorCacheService>().TryGetWorkItemStateColorMapFromCache(requestContext, projectId, out workItemTypeStateColorMap);
    }

    public string GetWorkItemStateColorETag(IVssRequestContext requestContext, Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      return requestContext.TraceBlock<string>(15163008, 15163009, "WebAccess.Settings", nameof (WorkItemMetadataFacadeService), nameof (GetWorkItemStateColorETag), (Func<string>) (() =>
      {
        if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext))
        {
          IWorkItemTrackingProcessService service1 = requestContext.GetService<IWorkItemTrackingProcessService>();
          WorkItemStateDefinitionService service2 = requestContext.GetService<WorkItemStateDefinitionService>();
          IVssRequestContext requestContext1 = requestContext;
          Guid projectId1 = projectId;
          ProcessDescriptor processDescriptor;
          ref ProcessDescriptor local = ref processDescriptor;
          return service1.TryGetProjectProcessDescriptor(requestContext1, projectId1, out local) ? service2.GetStateDefinitionHash(requestContext, processDescriptor.TypeId).ToString() : string.Empty;
        }
        if (requestContext.IsFeatureEnabled("WebAccess.WorkItemTracking.Safeguard.DisableWorkItemStateColors"))
          return string.Empty;
        ProjectProcessConfiguration processSettings = requestContext.GetService<IProjectConfigurationService>().GetProcessSettings(requestContext, ProjectInfo.GetProjectUri(projectId), false);
        return string.Join("-", processSettings.GetAllCategoryConfigurationsWithStates().OrderBy<CategoryConfiguration, string>((Func<CategoryConfiguration, string>) (c => c.CategoryReferenceName + c.SingularName)).Select<CategoryConfiguration, string>((Func<CategoryConfiguration, string>) (config => config.CategoryReferenceName + string.Join("-", ((IEnumerable<State>) config.States).OrderBy<State, string>((Func<State, string>) (s => s.Value)).Select<State, string>((Func<State, string>) (state => state.Value + state.Type.ToString())))))) + "-" + string.Join("-", new string[1]
        {
          processSettings.StateColorPropertyValue
        });
      }));
    }

    public IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemColor>> GetWorkItemTypeColorsByProjectNames(
      IVssRequestContext requestContext,
      IReadOnlyCollection<string> projectNames)
    {
      ArgumentUtility.CheckForNull<IReadOnlyCollection<string>>(projectNames, nameof (projectNames));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IReadOnlyDictionary<Guid, string> projectIdToNames = this.GetValidProjectIdToNameDictionary(requestContext, projectNames);
      return (IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemColor>>) this.GetWorkItemTypeColorsByProjectIds(requestContext, (IReadOnlyCollection<Guid>) projectIdToNames.Keys.ToList<Guid>()).ToDictionary<KeyValuePair<Guid, IReadOnlyCollection<WorkItemColor>>, string, IReadOnlyCollection<WorkItemColor>>((Func<KeyValuePair<Guid, IReadOnlyCollection<WorkItemColor>>, string>) (item => projectIdToNames[item.Key]), (Func<KeyValuePair<Guid, IReadOnlyCollection<WorkItemColor>>, IReadOnlyCollection<WorkItemColor>>) (item => item.Value), (IEqualityComparer<string>) TFStringComparer.TeamProjectName);
    }

    public IReadOnlyDictionary<Guid, IReadOnlyCollection<WorkItemColor>> GetWorkItemTypeColorsByProjectIds(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Guid> projectIds)
    {
      ArgumentUtility.CheckForNull<IReadOnlyCollection<Guid>>(projectIds, nameof (projectIds));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.TraceBlock<IReadOnlyDictionary<Guid, IReadOnlyCollection<WorkItemColor>>>(15163010, 15163011, "WebAccess.Settings", nameof (WorkItemMetadataFacadeService), nameof (GetWorkItemTypeColorsByProjectIds), (Func<IReadOnlyDictionary<Guid, IReadOnlyCollection<WorkItemColor>>>) (() =>
      {
        Dictionary<Guid, IReadOnlyCollection<WorkItemColor>> colorsByProjectIds = new Dictionary<Guid, IReadOnlyCollection<WorkItemColor>>();
        if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext))
          return this.GetWorkItemTypeColors(requestContext, projectIds);
        foreach (Guid projectId in (IEnumerable<Guid>) projectIds)
        {
          ProjectProcessConfiguration processSettings = requestContext.GetService<IProjectConfigurationService>().GetProcessSettings(requestContext, ProjectInfo.GetProjectUri(projectId), false);
          if (processSettings != null && !processSettings.IsDefault)
            colorsByProjectIds.Add(projectId, (IReadOnlyCollection<WorkItemColor>) ((IEnumerable<WorkItemColor>) processSettings.WorkItemColors).ToList<WorkItemColor>());
        }
        return (IReadOnlyDictionary<Guid, IReadOnlyCollection<WorkItemColor>>) colorsByProjectIds;
      }));
    }

    public IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemTypeColorAndIcon>> GetWorkItemTypeColorAndIconsByProjectNames(
      IVssRequestContext requestContext,
      IReadOnlyCollection<string> projectNames)
    {
      ArgumentUtility.CheckForNull<IReadOnlyCollection<string>>(projectNames, nameof (projectNames));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IReadOnlyDictionary<Guid, string> projectIdToNames = this.GetValidProjectIdToNameDictionary(requestContext, projectNames);
      return (IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemTypeColorAndIcon>>) this.GetWorkItemTypeColorAndIconsByProjectIds(requestContext, (IReadOnlyCollection<Guid>) projectIdToNames.Keys.ToList<Guid>()).ToDictionary<KeyValuePair<Guid, IReadOnlyCollection<WorkItemTypeColorAndIcon>>, string, IReadOnlyCollection<WorkItemTypeColorAndIcon>>((Func<KeyValuePair<Guid, IReadOnlyCollection<WorkItemTypeColorAndIcon>>, string>) (item => projectIdToNames[item.Key]), (Func<KeyValuePair<Guid, IReadOnlyCollection<WorkItemTypeColorAndIcon>>, IReadOnlyCollection<WorkItemTypeColorAndIcon>>) (item => item.Value), (IEqualityComparer<string>) TFStringComparer.TeamProjectName);
    }

    public IReadOnlyDictionary<Guid, IReadOnlyCollection<WorkItemTypeColorAndIcon>> GetWorkItemTypeColorAndIconsByProjectIds(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Guid> projectIds)
    {
      ArgumentUtility.CheckForNull<IReadOnlyCollection<Guid>>(projectIds, nameof (projectIds));
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return (IReadOnlyDictionary<Guid, IReadOnlyCollection<WorkItemTypeColorAndIcon>>) requestContext.TraceBlock<Dictionary<Guid, IReadOnlyCollection<WorkItemTypeColorAndIcon>>>(15163024, 15163025, "WebAccess.Settings", nameof (WorkItemMetadataFacadeService), nameof (GetWorkItemTypeColorAndIconsByProjectIds), (Func<Dictionary<Guid, IReadOnlyCollection<WorkItemTypeColorAndIcon>>>) (() =>
      {
        Dictionary<Guid, IReadOnlyCollection<WorkItemTypeColorAndIcon>> iconsByProjectIds = new Dictionary<Guid, IReadOnlyCollection<WorkItemTypeColorAndIcon>>();
        Dictionary<Guid, List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>> dictionary = requestContext.GetService<IWorkItemTypeService>().GetWorkItemTypes(requestContext, (IEnumerable<Guid>) projectIds).GroupBy<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType, Guid>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType, Guid>) (type => type.ProjectId)).ToDictionary<IGrouping<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>, Guid, List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>>((Func<IGrouping<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>, Guid>) (group => group.Key), (Func<IGrouping<Guid, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>, List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>>) (group => group.ToList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>()));
        foreach (Guid key in dictionary.Keys)
          iconsByProjectIds[key] = (IReadOnlyCollection<WorkItemTypeColorAndIcon>) this.GetWorkItemTypeColorAndIcon(requestContext, key, dictionary[key]);
        return iconsByProjectIds;
      }));
    }

    public IReadOnlyCollection<WorkItemTypeColorAndIcon> GetWorkItemTypeColorAndIconsByProjectId(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return (IReadOnlyCollection<WorkItemTypeColorAndIcon>) requestContext.TraceBlock<List<WorkItemTypeColorAndIcon>>(15162026, 15162027, "WebAccess.Settings", nameof (WorkItemMetadataFacadeService), nameof (GetWorkItemTypeColorAndIconsByProjectId), (Func<List<WorkItemTypeColorAndIcon>>) (() => this.GetWorkItemTypeColorAndIcon(requestContext, projectId, requestContext.GetService<IWorkItemTypeService>().GetWorkItemTypes(requestContext, projectId).ToList<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>())));
    }

    public void OnProjectSettingsChanged(
      IVssRequestContext requestContext,
      IEnumerable<ProjectInfo> projectInfos)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<ProjectInfo>>(projectInfos, nameof (projectInfos));
      WorkItemStateColorCacheService service1 = requestContext.GetService<WorkItemStateColorCacheService>();
      foreach (ProjectInfo projectInfo in projectInfos)
        service1.RemoveProjectStateColors(requestContext, projectInfo.Id);
      WorkItemStateCacheService service2 = requestContext.GetService<WorkItemStateCacheService>();
      foreach (ProjectInfo projectInfo in projectInfos)
        service2.RemoveWorkItemStates(requestContext, projectInfo.Id);
    }

    internal static IReadOnlyCollection<string> GetMergedStates(
      IVssRequestContext requestContext,
      IReadOnlyCollection<IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>>> stateMaps,
      StateGroup stateGroup,
      bool includeUnmappedStatesWithDone)
    {
      HashSet<string> doingStates = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      HashSet<string> doneStates = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      HashSet<string> other1 = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      foreach (IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>> stateMap in (IEnumerable<IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>>>) stateMaps)
      {
        IReadOnlyCollection<string> other2 = (IReadOnlyCollection<string>) null;
        if (stateMap.TryGetValue(StateGroupForCache.Doing, out other2))
          doingStates.UnionWith((IEnumerable<string>) other2);
        if (stateGroup == StateGroup.Done)
        {
          if (stateMap.TryGetValue(StateGroupForCache.Done, out other2))
            doneStates.UnionWith((IEnumerable<string>) other2);
          if (stateMap.TryGetValue(StateGroupForCache.Unmapped, out other2))
            other1.UnionWith((IEnumerable<string>) other2);
        }
      }
      other1.RemoveWhere((Predicate<string>) (state => doingStates.Contains(state) || doneStates.Contains(state)));
      if (includeUnmappedStatesWithDone)
        doneStates.UnionWith((IEnumerable<string>) other1);
      HashSet<string> values = doneStates;
      if (stateGroup == StateGroup.Doing)
        values = doingStates;
      requestContext.Trace(15114020, TraceLevel.Info, nameof (WorkItemMetadataFacadeService), TfsTraceLayers.BusinessLogic, string.Format("{0} : {1}", (object) stateGroup, (object) string.Join(",", (IEnumerable<string>) values)));
      return (IReadOnlyCollection<string>) values;
    }

    private IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>> GetWorkItemStatesInternal(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return requestContext.TraceBlock<IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>>>(15163012, 15163013, "WebAccess.Settings", nameof (WorkItemMetadataFacadeService), nameof (GetWorkItemStatesInternal), (Func<IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>>>) (() => requestContext.GetService<WorkItemStateCacheService>().GetWorkItemStatesMap(requestContext, projectId, (Func<IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>>>) (() =>
      {
        Dictionary<string, IEnumerable<WorkItemStateColor>> dictionary = new Dictionary<string, IEnumerable<WorkItemStateColor>>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
        return this.IsStateServiceAvailableForProject(requestContext, projectId) ? this.GetWorkItemStatesMapFromProcess(requestContext, projectId) : this.GetWorkItemStatesMapFromProjectConfig(requestContext, projectId);
      }))));
    }

    private IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>> GetWorkItemStatesMapFromProcess(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return (IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>>) requestContext.TraceBlock<Dictionary<StateGroupForCache, IReadOnlyCollection<string>>>(15163014, 15163015, "WebAccess.Settings", nameof (WorkItemMetadataFacadeService), nameof (GetWorkItemStatesMapFromProcess), (Func<Dictionary<StateGroupForCache, IReadOnlyCollection<string>>>) (() =>
      {
        Dictionary<StateGroupForCache, IReadOnlyCollection<string>> statesMapFromProcess = new Dictionary<StateGroupForCache, IReadOnlyCollection<string>>();
        ProcessDescriptor processDescriptor = (ProcessDescriptor) null;
        if (requestContext.GetService<IWorkItemTrackingProcessService>().TryGetLatestProjectProcessDescriptor(requestContext, projectId, out processDescriptor))
        {
          IWorkItemStateDefinitionService service = requestContext.GetService<IWorkItemStateDefinitionService>();
          HashSet<string> stateNames1 = service.GetStateNames(requestContext, processDescriptor.TypeId, WorkItemMetadataFacadeService.s_doingStateCategories);
          HashSet<string> stateNames2 = service.GetStateNames(requestContext, processDescriptor.TypeId, WorkItemMetadataFacadeService.s_doneStateCategories);
          statesMapFromProcess[StateGroupForCache.Doing] = (IReadOnlyCollection<string>) stateNames1;
          statesMapFromProcess[StateGroupForCache.Done] = (IReadOnlyCollection<string>) stateNames2;
        }
        else
          requestContext.Trace(15114015, TraceLevel.Error, "Agile", "BusinessLogic", "This method should not be called for phase 0/1");
        return statesMapFromProcess;
      }));
    }

    private IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>> GetWorkItemStatesMapFromProjectConfig(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return requestContext.TraceBlock<IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>>>(15163016, 15163017, "WebAccess.Settings", nameof (WorkItemMetadataFacadeService), nameof (GetWorkItemStatesMapFromProjectConfig), (Func<IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>>>) (() =>
      {
        ProjectProcessConfiguration processSettings = requestContext.GetService<IProjectConfigurationService>().GetProcessSettings(requestContext, ProjectInfo.GetProjectUri(projectId), false);
        string projectName = requestContext.GetService<IProjectService>().GetProjectName(requestContext, projectId);
        IEnumerable<string> allowedValues = requestContext.GetService<WebAccessWorkItemService>().GetAllowedValues(requestContext, 2, projectName);
        return WorkItemMetadataFacadeService.CalculateDoingDoneStatesFromProjectConfigInternal(processSettings, allowedValues);
      }));
    }

    internal static IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>> CalculateDoingDoneStatesFromProjectConfigInternal(
      ProjectProcessConfiguration projectProcessConfig,
      IEnumerable<string> allStates)
    {
      HashSet<string> stringSet1 = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      HashSet<string> stringSet2 = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      HashSet<string> mappedStates = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      IReadOnlyCollection<CategoryConfiguration> configurationsWithStates = projectProcessConfig.GetAllCategoryConfigurationsWithStates();
      stringSet1.UnionWith(configurationsWithStates.SelectMany<CategoryConfiguration, State>((Func<CategoryConfiguration, IEnumerable<State>>) (c => (IEnumerable<State>) c.States)).Where<State>((Func<State, bool>) (s => WorkItemMetadataFacadeService.s_doingStateType.Contains<StateTypeEnum>(s.Type))).Select<State, string>((Func<State, string>) (s => s.Value)));
      stringSet2.UnionWith(configurationsWithStates.SelectMany<CategoryConfiguration, State>((Func<CategoryConfiguration, IEnumerable<State>>) (c => (IEnumerable<State>) c.States)).Where<State>((Func<State, bool>) (s => WorkItemMetadataFacadeService.s_doneStateType.Contains<StateTypeEnum>(s.Type))).Select<State, string>((Func<State, string>) (s => s.Value)));
      mappedStates.UnionWith(configurationsWithStates.SelectMany<CategoryConfiguration, State>((Func<CategoryConfiguration, IEnumerable<State>>) (c => (IEnumerable<State>) c.States)).Select<State, string>((Func<State, string>) (s => s.Value)));
      HashSet<string> stringSet3 = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      stringSet3.UnionWith(allStates.Where<string>((Func<string, bool>) (s => !mappedStates.Contains(s))));
      return (IReadOnlyDictionary<StateGroupForCache, IReadOnlyCollection<string>>) new Dictionary<StateGroupForCache, IReadOnlyCollection<string>>()
      {
        [StateGroupForCache.Doing] = (IReadOnlyCollection<string>) stringSet1,
        [StateGroupForCache.Done] = (IReadOnlyCollection<string>) stringSet2,
        [StateGroupForCache.Unmapped] = (IReadOnlyCollection<string>) stringSet3
      };
    }

    private Dictionary<string, IReadOnlyCollection<WorkItemStateDefinition>> GetStateColorsFromProcess(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return requestContext.TraceBlock<Dictionary<string, IReadOnlyCollection<WorkItemStateDefinition>>>(15163018, 15163019, "WebAccess.Settings", nameof (WorkItemMetadataFacadeService), nameof (GetStateColorsFromProcess), (Func<Dictionary<string, IReadOnlyCollection<WorkItemStateDefinition>>>) (() =>
      {
        ProcessDescriptor processDescriptor = (ProcessDescriptor) null;
        IWorkItemTrackingProcessService service1 = requestContext.GetService<IWorkItemTrackingProcessService>();
        Dictionary<string, IReadOnlyCollection<WorkItemStateDefinition>> result = new Dictionary<string, IReadOnlyCollection<WorkItemStateDefinition>>();
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        ref ProcessDescriptor local = ref processDescriptor;
        if (!service1.TryGetLatestProjectProcessDescriptor(requestContext1, projectId1, out local))
        {
          requestContext.Trace(15114014, TraceLevel.Error, "Agile", "BusinessLogic", "This method should not be called for non-Phase2 projects.");
          return result;
        }
        IWorkItemTypeService service2 = requestContext.GetService<IWorkItemTypeService>();
        IWorkItemStateDefinitionService stateService = requestContext.GetService<IWorkItemStateDefinitionService>();
        IVssRequestContext requestContext2 = requestContext;
        Guid projectId2 = projectId;
        foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType workItemType in (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>) service2.GetWorkItemTypes(requestContext2, projectId2))
        {
          Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType witype = workItemType;
          List<WorkItemStateColor> workItemStateColorList = new List<WorkItemStateColor>();
          requestContext.TraceBlock(15114012, 15114013, 15114014, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, (Action) (() => result[witype.Name] = stateService.GetCombinedStateDefinitions(requestContext, processDescriptor.TypeId, witype.ReferenceName)), nameof (GetStateColorsFromProcess));
        }
        return result;
      }));
    }

    private IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>> GetWorkItemStateColorMapFromProcessConfig(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return requestContext.TraceBlock<IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>>>(15163020, 15163021, "WebAccess.Settings", nameof (WorkItemMetadataFacadeService), nameof (GetWorkItemStateColorMapFromProcessConfig), (Func<IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>>>) (() =>
      {
        ProjectProcessConfiguration processSettings = requestContext.GetService<IProjectConfigurationService>().GetProcessSettings(requestContext, ProjectInfo.GetProjectUri(projectId), false);
        IEnumerable<IWorkItemType> workItemTypes = (IEnumerable<IWorkItemType>) requestContext.GetService<WebAccessWorkItemService>().GetWorkItemTypes(requestContext, projectId);
        ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, projectId);
        IVssRequestContext requestContext1 = requestContext;
        IEnumerable<IWorkItemType> workItemTypesInProject = workItemTypes;
        string name = project.Name;
        return processSettings.GetWorkItemStateColors(requestContext1, workItemTypesInProject, name);
      }));
    }

    public static Dictionary<string, IReadOnlyCollection<WorkItemStateColor>> GetWorkItemStateColorMap(
      Dictionary<string, IReadOnlyCollection<WorkItemStateDefinition>> stateDefinitionsByWit)
    {
      ArgumentUtility.CheckForNull<Dictionary<string, IReadOnlyCollection<WorkItemStateDefinition>>>(stateDefinitionsByWit, nameof (stateDefinitionsByWit));
      Dictionary<string, IReadOnlyCollection<WorkItemStateColor>> itemStateColorMap = new Dictionary<string, IReadOnlyCollection<WorkItemStateColor>>();
      foreach (KeyValuePair<string, IReadOnlyCollection<WorkItemStateDefinition>> keyValuePair in stateDefinitionsByWit)
        itemStateColorMap[keyValuePair.Key] = (IReadOnlyCollection<WorkItemStateColor>) keyValuePair.Value.Select<WorkItemStateDefinition, WorkItemStateColor>((Func<WorkItemStateDefinition, WorkItemStateColor>) (stateDefinition => new WorkItemStateColor()
        {
          Name = stateDefinition.Name,
          Color = stateDefinition.Color,
          Category = stateDefinition.StateCategory.ToString()
        })).ToList<WorkItemStateColor>();
      return itemStateColorMap;
    }

    private IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>> GetWorkItemStateColorMapFromProcessDescriptor(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return (IReadOnlyDictionary<string, IReadOnlyCollection<WorkItemStateColor>>) requestContext.TraceBlock<Dictionary<string, IReadOnlyCollection<WorkItemStateColor>>>(15163022, 15163023, "WebAccess.Settings", nameof (WorkItemMetadataFacadeService), nameof (GetWorkItemStateColorMapFromProcessDescriptor), (Func<Dictionary<string, IReadOnlyCollection<WorkItemStateColor>>>) (() => WorkItemMetadataFacadeService.GetWorkItemStateColorMap(this.GetStateColorsFromProcess(requestContext, projectId))));
    }

    private bool IsStateServiceAvailableForProject(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      IWorkItemTrackingProcessService service = requestContext.GetService<IWorkItemTrackingProcessService>();
      ProcessDescriptor processDescriptor;
      return WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) && service.TryGetProjectProcessDescriptor(requestContext, projectId, out processDescriptor) && !processDescriptor.IsCustom;
    }

    private IReadOnlyDictionary<Guid, IReadOnlyCollection<WorkItemColor>> GetWorkItemTypeColors(
      IVssRequestContext requestContext,
      IReadOnlyCollection<Guid> projectIds)
    {
      Dictionary<Guid, List<WorkItemColor>> source = new Dictionary<Guid, List<WorkItemColor>>();
      if (projectIds.Any<Guid>())
      {
        projectIds = (IReadOnlyCollection<Guid>) projectIds.Distinct<Guid>().ToList<Guid>();
        requestContext.WitContext();
        IReadOnlyCollection<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType> workItemTypes = requestContext.GetService<IWorkItemTypeService>().GetWorkItemTypes(requestContext, (IEnumerable<Guid>) projectIds);
        ProcessReadSecuredObject processReadSecuredObject = (ProcessReadSecuredObject) null;
        Dictionary<Guid, string> dictionary = new Dictionary<Guid, string>();
        foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType workItemType in (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType>) workItemTypes)
        {
          Guid projectId = workItemType.ProjectId;
          if (requestContext.WitContext().ProcessReadPermissionChecker.HasProcessReadPermissionForProject(projectId, out processReadSecuredObject))
          {
            WorkItemColor workItemColor = new WorkItemColor()
            {
              WorkItemTypeName = workItemType.Name,
              PrimaryColor = workItemType.Color,
              SecondaryColor = workItemType.Color
            };
            workItemColor.SetSecuredObjectProperties(processReadSecuredObject);
            List<WorkItemColor> workItemColorList;
            if (source.TryGetValue(projectId, out workItemColorList))
              workItemColorList.Add(workItemColor);
            else
              source.Add(projectId, new List<WorkItemColor>()
              {
                workItemColor
              });
          }
        }
      }
      return (IReadOnlyDictionary<Guid, IReadOnlyCollection<WorkItemColor>>) source.ToDictionary<KeyValuePair<Guid, List<WorkItemColor>>, Guid, IReadOnlyCollection<WorkItemColor>>((Func<KeyValuePair<Guid, List<WorkItemColor>>, Guid>) (item => item.Key), (Func<KeyValuePair<Guid, List<WorkItemColor>>, IReadOnlyCollection<WorkItemColor>>) (item => (IReadOnlyCollection<WorkItemColor>) item.Value));
    }

    private List<WorkItemTypeColorAndIcon> GetWorkItemTypeColorAndIcon(
      IVssRequestContext requestContext,
      Guid projectId,
      List<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType> workItemTypes)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      try
      {
        IWITProcessReadPermissionCheckHelper permissionChecker = requestContext.WitContext().ProcessReadPermissionChecker;
        ProcessReadSecuredObject securedObject = (ProcessReadSecuredObject) null;
        Guid projectId1 = projectId;
        ref ProcessReadSecuredObject local = ref securedObject;
        if (!permissionChecker.HasProcessReadPermissionForProject(projectId1, out local))
          return new List<WorkItemTypeColorAndIcon>();
        ProcessDescriptor processDescriptor;
        requestContext.GetService<IWorkItemTrackingProcessService>().TryGetProjectProcessDescriptor(requestContext, projectId, out processDescriptor);
        properties.Add("ProjectId", (object) projectId);
        properties.Add("IsCustomProcess", processDescriptor == null || processDescriptor.IsCustom);
        List<WorkItemTypeColorAndIcon> typeColorAndIcon1 = new List<WorkItemTypeColorAndIcon>();
        if (processDescriptor != null)
        {
          foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType workItemType in workItemTypes)
          {
            WorkItemTypeColorAndIcon typeColorAndIcon2 = new WorkItemTypeColorAndIcon()
            {
              WorkItemTypeName = workItemType.Name,
              Color = workItemType.Color ?? CommonWITUtils.GetDefaultWorkItemTypeColor(),
              Icon = workItemType.Icon ?? WorkItemTypeIconUtils.GetDefaultIcon(),
              IsDisabled = workItemType.IsDisabled
            };
            typeColorAndIcon2.SetSecuredObjectProperties(securedObject);
            typeColorAndIcon1.Add(typeColorAndIcon2);
          }
        }
        else
        {
          ProjectProcessConfiguration processSettings = requestContext.GetService<IProjectConfigurationService>().GetProcessSettings(requestContext, ProjectInfo.GetProjectUri(projectId), false);
          if (processSettings != null && !processSettings.IsDefault)
          {
            foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType workItemType1 in workItemTypes)
            {
              Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType workItemType = workItemType1;
              string str;
              processSettings.WorkItemTypeIcons.TryGetValue(workItemType.Name, out str);
              string primaryColor = ((IEnumerable<WorkItemColor>) processSettings.WorkItemColors).FirstOrDefault<WorkItemColor>((Func<WorkItemColor, bool>) (color => string.Equals(workItemType.Name, color.WorkItemTypeName, StringComparison.CurrentCultureIgnoreCase)))?.PrimaryColor;
              WorkItemTypeColorAndIcon typeColorAndIcon3 = new WorkItemTypeColorAndIcon()
              {
                WorkItemTypeName = workItemType.Name,
                Color = primaryColor ?? CommonWITUtils.GetDefaultWorkItemTypeColor(),
                Icon = str ?? WorkItemTypeIconUtils.GetDefaultIcon(),
                IsDisabled = workItemType.IsDisabled
              };
              typeColorAndIcon3.SetSecuredObjectProperties(securedObject);
              typeColorAndIcon1.Add(typeColorAndIcon3);
            }
            properties.Add("WorkItemTypeIconCustomization", processSettings.WorkItemTypeIconPropertyValue);
          }
        }
        properties.Add("WorkItemTypeColorsAndIcons", (object) typeColorAndIcon1);
        return typeColorAndIcon1;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(599999, "WebAccess.Settings", nameof (WorkItemMetadataFacadeService), ex);
        properties.Add("HasException", true);
        return new List<WorkItemTypeColorAndIcon>();
      }
      finally
      {
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.WorkItemTracking, "GetWorkItemTypeIcon", properties);
      }
    }

    private IReadOnlyDictionary<Guid, string> GetValidProjectIdToNameDictionary(
      IVssRequestContext requestContext,
      IReadOnlyCollection<string> projectNames)
    {
      IProjectService service = requestContext.GetService<IProjectService>();
      Dictionary<Guid, string> toNameDictionary = new Dictionary<Guid, string>();
      foreach (string projectName in (IEnumerable<string>) projectNames)
      {
        Guid projectId = service.GetProjectId(requestContext, projectName);
        toNameDictionary.Add(projectId, projectName);
      }
      return (IReadOnlyDictionary<Guid, string>) toNameDictionary;
    }
  }
}
