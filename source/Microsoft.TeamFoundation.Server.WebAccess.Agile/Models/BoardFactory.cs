// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.BoardFactory
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Boards.Settings;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.GenericBoard.Filters;
using Microsoft.TeamFoundation.Server.WebAccess.Agile.Utility;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  internal class BoardFactory
  {
    private const string c_kanbanSettingsRootPath = "/Configuration/Application/Kanban";
    private const string c_pageSizeRelativePath = "/PageSize";
    private const string c_filterPageSizeRelativePath = "/FilterPageSize";
    private const int DefaultPageSize = 50;
    private const int DefaultFilterPageSize = 200;
    private static readonly string[] DefaultFilterFieldNames = new string[3]
    {
      CoreFieldReferenceNames.AssignedTo,
      CoreFieldReferenceNames.Title,
      CoreFieldReferenceNames.WorkItemType
    };

    public static HierarchyDataReader GetBoardWorkItems(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project,
      WebApiTeam team,
      BacklogContext backlogContext,
      IAgileSettings settings,
      IEnumerable<int> workItemIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Core.WebApi.ProjectInfo>(project, nameof (project));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<BacklogContext>(backlogContext, nameof (backlogContext));
      ArgumentUtility.CheckForNull<IAgileSettings>(settings, nameof (settings));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) workItemIds, nameof (workItemIds));
      BoardSettings backlogBoardSettings = BoardFactory.GetBacklogBoardSettings(requestContext, project, team, settings, backlogContext.CurrentLevelConfiguration);
      return BoardFactory.GetBacklogBoardItemSource(requestContext, project, team, backlogContext, settings, backlogBoardSettings).GetBoardWorkItems(workItemIds);
    }

    public static int GetMaxPageSize(IVssRequestContext teamFoundationRequestContext)
    {
      RegistryEntryCollection registryEntries = teamFoundationRequestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(teamFoundationRequestContext, (RegistryQuery) "/Configuration/Application/Kanban/*");
      return Math.Max(registryEntries.GetValueFromPath<int>("/Configuration/Application/Kanban/PageSize", 50), BoardFactory.GetFilterPageSize(teamFoundationRequestContext, registryEntries));
    }

    public static IBoard GetBoardFromBoardSettings(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project,
      WebApiTeam team,
      BacklogContext backlogContext,
      IAgileSettings settings,
      bool fetchItemSource,
      out BoardSettings boardSettings,
      bool pageIdentityRefs = false)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "BoardFactory.GetBoardFromBoardSettings"))
      {
        boardSettings = BoardFactory.GetBacklogBoardSettings(requestContext, project, team, settings, backlogContext.CurrentLevelConfiguration);
        Dictionary<string, string[]> dictionary = new Dictionary<string, string[]>();
        foreach (KeyValuePair<string, List<FieldSetting>> card in boardSettings.BoardCardSettings.Cards)
        {
          IEnumerable<string> source = card.Value.Select<FieldSetting, string>((System.Func<FieldSetting, string>) (fieldSettings => fieldSettings.FieldIdentifier)).Union<string>((IEnumerable<string>) BoardFactory.DefaultFilterFieldNames).Distinct<string>();
          dictionary.Add(card.Key, source.ToArray<string>());
        }
        Board fromBoardSettings = new Board((BoardNode) null)
        {
          Id = boardSettings.Id.Value,
          FieldTypes = (IDictionary<string, string>) ((IEnumerable<TypeField>) settings.Process.TypeFields).ToDictionary<TypeField, string, string>((System.Func<TypeField, string>) (tf => tf.Type.ToString()), (System.Func<TypeField, string>) (tf => tf.Name)),
          Membership = new FunctionReference(BoardFunction.TeamMembership),
          FilterableFieldNamesByItemType = dictionary
        };
        if (fetchItemSource)
          fromBoardSettings.ItemDataSource = (IItemSource) BoardFactory.GetBacklogBoardItemSource(requestContext, project, team, backlogContext, settings, boardSettings, pageIdentityRefs);
        RegistryEntryCollection registryEntries = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, (RegistryQuery) "/Configuration/Application/Kanban/*");
        fromBoardSettings.PageSize = registryEntries.GetValueFromPath<int>("/Configuration/Application/Kanban/PageSize", 50);
        fromBoardSettings.FilterPageSize = BoardFactory.GetFilterPageSize(requestContext, registryEntries);
        return (IBoard) fromBoardSettings;
      }
    }

    public static BoardSettings GetBacklogBoardSettings(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project,
      WebApiTeam team,
      IAgileSettings settings,
      BacklogLevelConfiguration backlogLevelConfiguration)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "BoardFactory." + AgileCustomerIntelligenceFeature.GetBacklogBoardSettings))
      {
        requestContext.GetService<BoardService>();
        ITeamService service = requestContext.GetService<ITeamService>();
        CommonStructureProjectInfo project1 = CommonStructureProjectInfo.ConvertProjectInfo(project);
        BoardSettings backlogBoardSettings = (BoardSettings) null;
        using (PerformanceTimer.StartMeasure(requestContext, "GetBacklogBoardSettings." + AgileCustomerIntelligencePropertyName.GetOrCreateBoardSettings))
          backlogBoardSettings = KanbanUtils.Instance.GetOrCreateBoardSettings(requestContext, project1, team, backlogLevelConfiguration, 0);
        IVssRequestContext requestContext1 = requestContext;
        Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project2 = project;
        WebApiTeam team1 = team;
        Guid? nullable = backlogBoardSettings.ExtensionId;
        Guid extensionId = nullable.Value;
        BacklogLevelConfiguration backlogLevelConfiguration1 = backlogLevelConfiguration;
        if (BoardFactory.RemoveInvalidWorkItemTypeRulesFromExtension(requestContext1, project2, team1, extensionId, backlogLevelConfiguration1))
          backlogBoardSettings = KanbanUtils.Instance.GetBoardSettings(requestContext, team, backlogLevelConfiguration, project1);
        backlogBoardSettings.AutoRefreshState = false;
        BoardUserSettings boardUserSettings = (BoardUserSettings) null;
        using (PerformanceTimer.StartMeasure(requestContext, "GetBacklogBoardSettings." + AgileCustomerIntelligencePropertyName.GetBoardUserSettings))
        {
          IBoardUserSettingsUtils instance = BoardUserSettingsUtils.Instance;
          IVssRequestContext requestContext2 = requestContext;
          Guid id = project.Id;
          WebApiTeam team2 = team;
          nullable = backlogBoardSettings.Id;
          Guid boardId = nullable.Value;
          boardUserSettings = instance.GetBoardUserSettings(requestContext2, id, team2, boardId);
        }
        if (boardUserSettings != null)
        {
          using (PerformanceTimer.StartMeasure(requestContext, "GetBacklogBoardSettings." + AgileCustomerIntelligencePropertyName.BoardFilterSettings))
          {
            BoardFilterSettingsManager filterSettingsManager = new BoardFilterSettingsManager();
            string boardId = backlogBoardSettings.Id.Value.ToString();
            string projectId = project.Id.ToString();
            Dictionary<string, WorkItemFilter> filter = filterSettingsManager.GetFilterValue(requestContext, projectId, boardId);
            if (filter == null)
            {
              BoardFilterSettingsModel wiqlandParentIds = filterSettingsManager.GetBoardFilterSettingsModelFromWiqlandParentIds(requestContext, boardUserSettings.CurrentBoardFilter, boardUserSettings.ParentWorkItemIds);
              filter = filterSettingsManager.TryToConvertBoardFilterSettingsModelToFilterValue(requestContext, wiqlandParentIds);
              filterSettingsManager.SaveFilterValue(requestContext, projectId, boardId, filter);
            }
            backlogBoardSettings.BoardFilterSettings = new BoardFilterSettings()
            {
              BoardId = backlogBoardSettings.Id.Value,
              InitialFilter = filter
            };
          }
          backlogBoardSettings.AutoRefreshState = boardUserSettings.AutoBoardRefreshState;
          using (PerformanceTimer.StartMeasure(requestContext, "GetBacklogBoardSettings.GetAllBoardFieldNames"))
            backlogBoardSettings.BoardFields = (IDictionary<string, string>) KanbanUtils.Instance.GetAllBoardFieldNames(requestContext, backlogBoardSettings.ExtensionId.Value);
          string name = ((IEnumerable<TypeField>) settings.Process.TypeFields).FirstOrDefault<TypeField>((System.Func<TypeField, bool>) (field => field.Type == FieldTypeEnum.Order)).Name;
          string fieldReferenceName = settings.GetClosedDateFieldReferenceName(requestContext);
          backlogBoardSettings.SortableFieldsByColumnType = (IDictionary<string, string>) new Dictionary<string, string>()
          {
            {
              "Incoming".ToLowerInvariant(),
              name
            },
            {
              "InProgress".ToLowerInvariant(),
              name
            },
            {
              "Outgoing".ToLowerInvariant(),
              fieldReferenceName
            }
          };
        }
        backlogBoardSettings.CanEdit = service.UserIsTeamAdmin(requestContext, team.Identity);
        if (backlogBoardSettings.BoardCardSettings == null)
          backlogBoardSettings.BoardCardSettings = new BoardCardSettings(BoardCardSettings.ScopeType.KANBAN, backlogBoardSettings.Id.Value);
        using (PerformanceTimer.StartMeasure(requestContext, AgileCustomerIntelligencePropertyName.ReconcileSettings))
          CardSettingsUtils.Instance.ValidateAndReconcileSettingsForGET(requestContext, backlogLevelConfiguration, settings, BoardCardSettings.ScopeType.KANBAN, backlogBoardSettings.Id.Value, backlogBoardSettings.BoardCardSettings);
        using (PerformanceTimer.StartMeasure(requestContext, "BoardFactory.TransformWiqls"))
          backlogBoardSettings.BoardCardSettings.Rules = CardSettingsUtils.Instance.TransformWiqls(requestContext, backlogBoardSettings.BoardCardSettings.Rules);
        using (ISettingsProvider webSettings = WebSettings.GetWebSettings(requestContext, project.Id, team, WebSettingsScope.Project))
        {
          string path1 = "TFSTests.Agile.Cards.TestAnnotation/Settings/TestPlan/{0}".Format((object) team.Id);
          string path2 = "MS.VS.TestManagement/TestOutcomeSettings/Team/{0}".Format((object) team.Id);
          string setting1 = webSettings.GetSetting<string>(path1, string.Empty);
          bool setting2 = webSettings.GetSetting<bool>(path2, false);
          backlogBoardSettings.BoardCardSettings.TestSettings = new BoardTestSettings()
          {
            SameTestOutcomes = setting2,
            TestPlanId = setting1
          };
        }
        backlogBoardSettings.AllowedMappings = KanbanUtils.Instance.GetColumnTypeAllowedStateMappings(requestContext, project.Name, settings.BacklogConfiguration, backlogLevelConfiguration, settings.TeamSettings.BugsBehavior == BugsBehavior.AsRequirements);
        return backlogBoardSettings;
      }
    }

    public static BoardCardSettings GetBoardCardSettings(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project,
      IAgileSettings settings,
      BacklogLevelConfiguration backlogLevel,
      BoardCardSettings.ScopeType boardScope,
      Guid boardScopeId)
    {
      BoardCardSettings boardCardSettings = KanbanUtils.Instance.GetOrCreateBoardCardSettings(requestContext, CommonStructureProjectInfo.ConvertProjectInfo(project), boardScope, boardScopeId);
      CardSettingsUtils.Instance.ValidateAndReconcileSettingsForGET(requestContext, backlogLevel, settings, boardScope, boardScopeId, boardCardSettings);
      return boardCardSettings;
    }

    public static List<CardRule> GetBoardCardRules(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project,
      BoardCardSettings.ScopeType boardScope,
      Guid boardScopeId)
    {
      return KanbanUtils.Instance.GetBoardCardRules(requestContext, CommonStructureProjectInfo.ConvertProjectInfo(project), boardScope, boardScopeId);
    }

    private static bool RemoveInvalidWorkItemTypeRulesFromExtension(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project,
      WebApiTeam team,
      Guid extensionId,
      BacklogLevelConfiguration backlogLevelConfiguration)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "BoardFactory.RemoveInvalidWorkItemTypeRulesFromExtension"))
      {
        IWorkItemTypeExtensionService service = requestContext.GetService<IWorkItemTypeExtensionService>();
        WorkItemTypeExtension extension = service.GetExtension(requestContext, extensionId);
        requestContext.GetService<IProjectConfigurationService>().GetProcessSettings(requestContext, project.Uri, false);
        IReadOnlyCollection<string> backlogWorkItemTypeNames = backlogLevelConfiguration.WorkItemTypes;
        IEnumerable<WorkItemFieldRule> fieldRules = extension.FieldRules;
        WorkItemFieldRule workItemFieldRule = fieldRules.Where<WorkItemFieldRule>((System.Func<WorkItemFieldRule, bool>) (r => r.Field == "[Kanban.Column]")).First<WorkItemFieldRule>();
        IEnumerable<WhenRule> whenRules = workItemFieldRule.SelectRules<WhenRule>().Where<WhenRule>((System.Func<WhenRule, bool>) (r => !backlogWorkItemTypeNames.Contains<string>(r.Value, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName)));
        if (!whenRules.Any<WhenRule>())
          return false;
        WhenRule[] array = workItemFieldRule.SelectRules<WhenRule>().Except<WhenRule>(whenRules).ToArray<WhenRule>();
        workItemFieldRule.SubRules = (WorkItemRule[]) array;
        ReconcileRequestResult reconcileRequestResult;
        service.UpdateExtension(requestContext, extension.Id, extension.ProjectId, extension.OwnerId, (string) null, (string) null, (IEnumerable<WorkItemTypeExtensionFieldDeclaration>) null, extension.Predicate, fieldRules, -1, out reconcileRequestResult);
        return reconcileRequestResult == ReconcileRequestResult.CompletedSynchronously;
      }
    }

    public static WorkItemSource GetBacklogBoardItemSource(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project,
      WebApiTeam team,
      BacklogContext backlogContext,
      IAgileSettings settings,
      BoardSettings boardSettings,
      bool pageIdentityRefs = false)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "BoardFactory.GetBacklogBoardItemSource"))
      {
        IDictionary defaultQueryContext = requestContext.GetDefaultQueryContext(project.Name);
        List<string> stringList1 = new List<string>();
        ProductBacklogQueryBuilder queryBuilder = (ProductBacklogQueryBuilder) null;
        string kanbanColumnMarkerFieldRefName = (string) null;
        using (PerformanceTimer.StartMeasure(requestContext, "GetBacklogBoardItemSource.GetFieldReferenceNames"))
        {
          string fieldReferenceName1 = KanbanUtils.Instance.GetKanbanColumnFieldReferenceName(requestContext, boardSettings.ExtensionId.Value);
          string fieldReferenceName2 = KanbanUtils.Instance.GetKanbanDoneFieldReferenceName(requestContext, boardSettings.ExtensionId.Value);
          kanbanColumnMarkerFieldRefName = KanbanUtils.Instance.GetMarkerField(requestContext, boardSettings.ExtensionId.Value).ReferenceName;
          string fieldReferenceName3 = KanbanUtils.Instance.GetKanbanLaneFieldReferenceName(requestContext, boardSettings.ExtensionId.Value);
          if (!string.IsNullOrEmpty(fieldReferenceName3))
            stringList1.Add(fieldReferenceName3);
          if (!string.IsNullOrEmpty(fieldReferenceName1))
            stringList1.Add(fieldReferenceName1);
          if (!string.IsNullOrEmpty(kanbanColumnMarkerFieldRefName))
            stringList1.Add(kanbanColumnMarkerFieldRefName);
          if (!string.IsNullOrEmpty(fieldReferenceName2))
            stringList1.Add(fieldReferenceName2);
          stringList1.AddRange(CardSettingsUtils.Instance.GetBoardFields(requestContext, boardSettings.BoardCardSettings));
          stringList1 = stringList1.Union<string>((IEnumerable<string>) new List<string>()
          {
            CoreFieldReferenceNames.Tags
          }).ToList<string>();
          queryBuilder = new ProductBacklogQueryBuilder(requestContext, settings, backlogContext, settings.TeamSettings.GetBacklogIterationNode(requestContext).GetPath(requestContext));
          List<string> stringList2 = new List<string>(queryBuilder.Fields.Union<string>((IEnumerable<string>) new string[4]
          {
            CoreFieldReferenceNames.Id,
            CoreFieldReferenceNames.AssignedTo,
            fieldReferenceName1,
            kanbanColumnMarkerFieldRefName
          }));
          if (!string.IsNullOrEmpty(fieldReferenceName3))
            stringList2.Add(fieldReferenceName3);
          queryBuilder.Fields = (IEnumerable<string>) stringList2.ToArray();
        }
        IDictionary<string, IDictionary<string, ISet<string>>> stateRuleTransitions = (IDictionary<string, IDictionary<string, ISet<string>>>) null;
        using (PerformanceTimer.StartMeasure(requestContext, "GetBacklogBoardItemSource.GetStateTransitions"))
          stateRuleTransitions = BoardFactory.GetStateTransitions(requestContext, settings, backlogContext.CurrentLevelConfiguration);
        IDictionary<string, IDictionary<string, ISet<string>>> transitions = (IDictionary<string, IDictionary<string, ISet<string>>>) null;
        using (PerformanceTimer.StartMeasure(requestContext, "GetBacklogBoardItemSource.GetKanbanColumnTransitions"))
          transitions = BoardFactory.GetKanbanColumnTransitions(requestContext, boardSettings.ExtensionId.Value, stateRuleTransitions);
        return new WorkItemSource(requestContext, settings, queryBuilder, defaultQueryContext, backlogContext, transitions, (IEnumerable<string>) stringList1, (System.Func<IDataRecord, bool>) (r => r[kanbanColumnMarkerFieldRefName] != null && (bool) r[kanbanColumnMarkerFieldRefName]), pageIdentityRefs);
      }
    }

    private static IDictionary<string, IDictionary<string, ISet<string>>> GetStateTransitions(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      BacklogLevelConfiguration backlogLevelConfiguration)
    {
      WebAccessWorkItemService service = requestContext.GetService<WebAccessWorkItemService>();
      IEnumerable<string> workItemTypes = (IEnumerable<string>) backlogLevelConfiguration.WorkItemTypes;
      IEnumerable<string> workItemStates = (IEnumerable<string>) settings.BacklogConfiguration.GetWorkItemStates(backlogLevelConfiguration);
      IVssRequestContext requestContext1 = requestContext;
      string projectName = settings.ProjectName;
      IEnumerable<string> childWorkItems = workItemTypes;
      IEnumerable<string> displayStates = workItemStates;
      return service.GetStateTransitions(requestContext1, projectName, childWorkItems, displayStates);
    }

    private static IDictionary<string, IDictionary<string, ISet<string>>> GetKanbanColumnTransitions(
      IVssRequestContext requestContext,
      Guid extensionId,
      IDictionary<string, IDictionary<string, ISet<string>>> stateRuleTransitions)
    {
      Dictionary<string, Dictionary<string, IEnumerable<string>>> stateColumnMapping = BoardFactory.GetWitStateColumnMapping(requestContext.GetService<IWorkItemTypeExtensionService>().GetExtensions(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        extensionId
      }).First<WorkItemTypeExtension>());
      Dictionary<string, IDictionary<string, ISet<string>>> columnTransitions = new Dictionary<string, IDictionary<string, ISet<string>>>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      foreach (KeyValuePair<string, IDictionary<string, ISet<string>>> stateRuleTransition in (IEnumerable<KeyValuePair<string, IDictionary<string, ISet<string>>>>) stateRuleTransitions)
      {
        string witName = stateRuleTransition.Key;
        columnTransitions[witName] = (IDictionary<string, ISet<string>>) new Dictionary<string, ISet<string>>();
        IDictionary<string, ISet<string>> dictionary1 = stateRuleTransition.Value;
        Dictionary<string, IEnumerable<string>> dictionary2 = stateColumnMapping.Where<KeyValuePair<string, Dictionary<string, IEnumerable<string>>>>((System.Func<KeyValuePair<string, Dictionary<string, IEnumerable<string>>>, bool>) (kvp => TFStringComparer.WorkItemTypeName.Equals(kvp.Key, witName))).FirstOrDefault<KeyValuePair<string, Dictionary<string, IEnumerable<string>>>>().Value;
        if (dictionary2 != null)
        {
          foreach (KeyValuePair<string, ISet<string>> keyValuePair in (IEnumerable<KeyValuePair<string, ISet<string>>>) dictionary1)
          {
            string key1 = keyValuePair.Key;
            ISet<string> stringSet = keyValuePair.Value;
            stringSet.Add(key1);
            IEnumerable<string> strings;
            if (dictionary2.TryGetValue(key1, out strings))
            {
              List<string> source = new List<string>();
              foreach (string key2 in (IEnumerable<string>) stringSet)
              {
                IEnumerable<string> collection;
                if (dictionary2.TryGetValue(key2, out collection))
                  source.AddRange(collection);
              }
              foreach (string str in strings)
              {
                string from = str;
                columnTransitions[witName][from] = (ISet<string>) new HashSet<string>(source.Where<string>((System.Func<string, bool>) (val => !TFStringComparer.BoardColumnName.Equals(val, from))), (IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
              }
            }
          }
        }
      }
      return (IDictionary<string, IDictionary<string, ISet<string>>>) columnTransitions;
    }

    private static Dictionary<string, Dictionary<string, IEnumerable<string>>> GetWitStateColumnMapping(
      WorkItemTypeExtension extension)
    {
      Dictionary<string, Dictionary<string, IEnumerable<string>>> stateColumnMapping = new Dictionary<string, Dictionary<string, IEnumerable<string>>>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      foreach (WhenRule selectRule in extension.FieldRules.FirstOrDefault<WorkItemFieldRule>().SelectRules<WhenRule>())
      {
        string key1 = selectRule.Value;
        Dictionary<string, IEnumerable<string>> dictionary;
        if (!stateColumnMapping.TryGetValue(key1, out dictionary))
        {
          dictionary = new Dictionary<string, IEnumerable<string>>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
          stateColumnMapping[key1] = dictionary;
        }
        foreach (MapCase mapCase in selectRule.SelectRules<MapRule>().FirstOrDefault<MapRule>().Cases)
        {
          string key2 = mapCase.Value;
          string[] values = mapCase.Values;
          dictionary[key2] = (IEnumerable<string>) values;
        }
      }
      return stateColumnMapping;
    }

    private static int GetFilterPageSize(
      IVssRequestContext requestContext,
      RegistryEntryCollection registryEntries)
    {
      int defaultValue = 200;
      if (requestContext.IsFeatureEnabled("WebAccess.Agile.KanbanBoard.IncreaseDefaultFilterPageSize"))
        defaultValue = 1000;
      return registryEntries.GetValueFromPath<int>("/Configuration/Application/Kanban/FilterPageSize", defaultValue);
    }
  }
}
