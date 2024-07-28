// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.KanbanUtils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class KanbanUtils
  {
    private const int DefaultMemberLimit = 5;
    private const int DefaultReconcileTimeoutInMilliseconds = 25;
    internal const int CreationReconcileTimeoutInMilliseconds = 1000;
    private const string PathSeparator = "\\";
    private static KanbanUtils m_helper;

    protected KanbanUtils()
    {
    }

    public static KanbanUtils Instance
    {
      get
      {
        if (KanbanUtils.m_helper == null)
          KanbanUtils.m_helper = new KanbanUtils();
        return KanbanUtils.m_helper;
      }
      set => KanbanUtils.m_helper = value;
    }

    public void ProvisionExtensionRank(
      IVssRequestContext requestContext,
      BoardSettings board,
      BacklogLevelConfiguration backlogLevel,
      WebApiTeam team,
      string projectUri)
    {
      IWorkItemTypeExtensionService service = requestContext.GetService<IWorkItemTypeExtensionService>();
      WorkItemTypeExtension extension = service.GetExtension(requestContext, board.ExtensionId.Value);
      if (extension == null)
      {
        requestContext.Trace(290421, TraceLevel.Warning, "WebAccess", TfsTraceLayers.BusinessLogic, "Extension was not found or may have been deleted when attempting to update extension rank. BoardId: {0}, ExtensionId: {1}", (object) board.Id, (object) board.ExtensionId.Value);
      }
      else
      {
        if (extension.Rank != 0)
          return;
        ITeamSettings teamSettings = requestContext.GetService<ITeamConfigurationService>().GetTeamSettings(requestContext, team, false, false, true);
        ProjectProcessConfiguration processConfiguration = requestContext.GetProjectProcessConfiguration(projectUri, false);
        ReconcileRequestResult reconcileRequestResult = ReconcileRequestResult.CompletedSynchronously;
        service.UpdateExtension(requestContext, extension.Id, extension.ProjectId, extension.OwnerId, (string) null, (string) null, (IEnumerable<WorkItemTypeExtensionFieldDeclaration>) this.CreateExtensionFields(requestContext, backlogLevel), extension.Predicate, extension.FieldRules, new int?(this.GetExtensionRank(processConfiguration.IsTeamFieldAreaPath(), teamSettings.TeamFieldConfig)), (string) null, (IEnumerable<WorkItemFieldRule>) null, 0, out reconcileRequestResult, true);
      }
    }

    public void ProvisionKanbanDoneFieldAndRules(
      IVssRequestContext requestContext,
      BoardSettings board,
      BacklogLevelConfiguration backlogLevel)
    {
      IWorkItemTypeExtensionService service = requestContext.GetService<IWorkItemTypeExtensionService>();
      WorkItemTypeExtension extension = service.GetExtension(requestContext, board.ExtensionId.Value);
      if (extension.Fields.Where<WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeExtensionFieldEntry, bool>) (f => f.LocalReferenceName == "Kanban.Column.Done")).Any<WorkItemTypeExtensionFieldEntry>())
        return;
      ReconcileRequestResult reconcileRequestResult = ReconcileRequestResult.CompletedSynchronously;
      service.UpdateExtension(requestContext, extension.Id, extension.ProjectId, extension.OwnerId, (string) null, (string) null, (IEnumerable<WorkItemTypeExtensionFieldDeclaration>) this.CreateExtensionFields(requestContext, backlogLevel), extension.Predicate, extension.FieldRules.Concat<WorkItemFieldRule>((IEnumerable<WorkItemFieldRule>) new WorkItemFieldRule[1]
      {
        this.CreateKanbanDoneFieldRule()
      }), new int?(), (string) null, (IEnumerable<WorkItemFieldRule>) null, 0, out reconcileRequestResult, true);
    }

    public void ProvisionKanbanLaneFieldAndRules(
      IVssRequestContext requestContext,
      BoardSettings board,
      BacklogLevelConfiguration backlogLevel)
    {
      IWorkItemTypeExtensionService service = requestContext.GetService<IWorkItemTypeExtensionService>();
      WorkItemTypeExtension extension = service.GetExtension(requestContext, board.ExtensionId.Value);
      if (extension.Fields.Where<WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeExtensionFieldEntry, bool>) (f => f.LocalReferenceName == "Kanban.Lane")).Any<WorkItemTypeExtensionFieldEntry>())
        return;
      ReconcileRequestResult reconcileRequestResult = ReconcileRequestResult.CompletedSynchronously;
      service.UpdateExtension(requestContext, extension.Id, extension.ProjectId, extension.OwnerId, (string) null, (string) null, (IEnumerable<WorkItemTypeExtensionFieldDeclaration>) this.CreateExtensionFields(requestContext, backlogLevel), extension.Predicate, extension.FieldRules.Concat<WorkItemFieldRule>((IEnumerable<WorkItemFieldRule>) new WorkItemFieldRule[1]
      {
        this.CreateKanbanLaneFieldRule(board.Columns.First<BoardColumn>((Func<BoardColumn, bool>) (c => c.ColumnType == BoardColumnType.Incoming)).Name, board.Columns.First<BoardColumn>((Func<BoardColumn, bool>) (c => c.ColumnType == BoardColumnType.Outgoing)).Name, this.GetDefaultRowName(board.Rows), this.GetBoardRowNames(board))
      }), new int?(), (string) null, (IEnumerable<WorkItemFieldRule>) null, 0, out reconcileRequestResult, true);
    }

    public virtual void UpdateExtension(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      WebApiTeam team,
      BacklogLevelConfiguration backlogLevel,
      BoardSettings boardSettings,
      bool updatePredicate,
      bool updateFieldRules,
      int reconcileTimeout,
      ITeamSettings teamSettings,
      IEnumerable<BoardColumn> deletedColumns = null,
      BoardSettings oldBoardSettings = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<CommonStructureProjectInfo>(project, nameof (project));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<BoardSettings>(boardSettings, nameof (boardSettings));
      ArgumentUtility.CheckForNull<BacklogLevelConfiguration>(backlogLevel, nameof (backlogLevel));
      IEnumerable<WorkItemFieldRule> reconciliationScopeRules = (IEnumerable<WorkItemFieldRule>) null;
      IWorkItemTypeExtensionService service = requestContext.GetService<IWorkItemTypeExtensionService>();
      WorkItemTypeExtension extension = service.GetExtension(requestContext, boardSettings.ExtensionId.Value);
      WorkItemExtensionPredicate predicate = extension.Predicate;
      if (updatePredicate)
        predicate = this.CreateExtensionPredicate(requestContext, project, team, backlogLevel, teamSettings);
      IEnumerable<WorkItemTypeExtensionFieldDeclaration> toAddIfNecessary = this.GetExtensionFieldsToAddIfNecessary(requestContext, extension, backlogLevel);
      if (toAddIfNecessary.Any<WorkItemTypeExtensionFieldDeclaration>())
        updateFieldRules = true;
      IEnumerable<WorkItemFieldRule> fieldRules = extension.FieldRules;
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration = requestContext.GetService<IBacklogConfigurationService>().GetProjectBacklogConfiguration(requestContext, project.GetId());
      bool bugsOnBacklogFeatureAvailable = this.BugsOnBacklogFeatureAvailable(backlogConfiguration, backlogLevel);
      WorkItemFieldRule workItemFieldRule = extension.FieldRules.Where<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (r => r.Field == "[Kanban.Column]")).First<WorkItemFieldRule>();
      if (updateFieldRules)
        fieldRules = this.CreateRules(requestContext, project, team, backlogLevel, boardSettings, teamSettings, deletedColumns, backlogConfiguration, bugsOnBacklogFeatureAvailable, workItemFieldRule, out reconciliationScopeRules, oldBoardSettings);
      else if (bugsOnBacklogFeatureAvailable)
        this.ValidateAndUpdateWorkItemRules(requestContext, project, team, backlogConfiguration, backlogLevel, workItemFieldRule);
      requestContext.Trace(240407, TraceLevel.Verbose, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Updating Kanban extension for team: {0}, backlog level: {1}", (object) team.Name, (object) backlogLevel.Id);
      ProjectProcessConfiguration processConfiguration = requestContext.GetProjectProcessConfiguration(project.Uri);
      service.UpdateExtension(requestContext, extension.Id, extension.ProjectId, extension.OwnerId, (string) null, (string) null, toAddIfNecessary.Any<WorkItemTypeExtensionFieldDeclaration>() ? toAddIfNecessary : (IEnumerable<WorkItemTypeExtensionFieldDeclaration>) null, predicate, fieldRules, new int?(this.GetExtensionRank(processConfiguration.IsTeamFieldAreaPath(), teamSettings.TeamFieldConfig)), reconciliationScopeRules, reconcileTimeout, out ReconcileRequestResult _);
      requestContext.Trace(240408, TraceLevel.Verbose, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Successfully updated Kanban extension for team: {0}, backlog level: {1}", (object) team.Name, (object) backlogLevel.Id);
    }

    public virtual BoardRecord GetBoardRecordById(
      BoardService boardService,
      IVssRequestContext requestContext,
      Guid teamID,
      Guid boardId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return boardService.GetTeamBoards(requestContext, teamID).Where<BoardRecord>((Func<BoardRecord, bool>) (b => b.Id == boardId)).FirstOrDefault<BoardRecord>();
    }

    internal virtual BoardCardSettings GetBoardCardSettings(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      BoardCardSettings.ScopeType scope,
      Guid scopeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<CommonStructureProjectInfo>(project, nameof (project));
      return requestContext.GetService<BoardService>().GetBoardCardSettings(requestContext, project.GetId(), scope, scopeId);
    }

    internal virtual List<CardRule> GetBoardCardRules(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      BoardCardSettings.ScopeType scope,
      Guid scopeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<CommonStructureProjectInfo>(project, nameof (project));
      return requestContext.GetService<BoardService>().GetBoardCardRules(requestContext, project.GetId(), scope, scopeId);
    }

    public virtual BoardCardSettings GetOrCreateBoardCardSettings(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      BoardCardSettings.ScopeType scope,
      Guid scopeId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<CommonStructureProjectInfo>(project, nameof (project));
      return this.GetBoardCardSettings(requestContext, project, scope, scopeId) ?? new BoardCardSettings(scope, scopeId);
    }

    public virtual BoardSettings GetBoardSettings(
      IVssRequestContext requestContext,
      WebApiTeam team,
      BacklogLevelConfiguration backlogLevel,
      CommonStructureProjectInfo project,
      bool validateTeamSettings = false,
      bool bypassCache = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<BacklogLevelConfiguration>(backlogLevel, nameof (backlogLevel));
      PerformanceScenarioHelper scenarioHelper = new PerformanceScenarioHelper(requestContext, "Agile", nameof (GetBoardSettings));
      BoardService service = requestContext.GetService<BoardService>();
      BoardSettings boardSettings = (BoardSettings) null;
      using (scenarioHelper.Measure("GetBoard"))
        boardSettings = service.GetBoard(requestContext, project.GetId(), team.Id, backlogLevel.Id, bypassCache);
      if (boardSettings != null)
        this.UpdateExtensionsAndPopulateStateMappings(requestContext, boardSettings, team, backlogLevel, project, scenarioHelper, validateTeamSettings);
      scenarioHelper.EndScenario();
      return boardSettings;
    }

    private BoardSettings RestoreBoardSettings(
      IVssRequestContext requestContext,
      WebApiTeam team,
      BacklogLevelConfiguration backlogLevel,
      CommonStructureProjectInfo project,
      bool validateTeamSettings = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<BacklogLevelConfiguration>(backlogLevel, nameof (backlogLevel));
      PerformanceScenarioHelper scenarioHelper = new PerformanceScenarioHelper(requestContext, "Agile", nameof (RestoreBoardSettings));
      BoardService service = requestContext.GetService<BoardService>();
      BoardSettings boardSettings = (BoardSettings) null;
      using (scenarioHelper.Measure("GetBoard"))
        boardSettings = service.RestoreBoard(requestContext, project.GetId(), team.Id, backlogLevel.Id);
      if (boardSettings != null)
        this.UpdateExtensionsAndPopulateStateMappings(requestContext, boardSettings, team, backlogLevel, project, scenarioHelper, validateTeamSettings);
      scenarioHelper.Add("Restored", (object) (boardSettings != null));
      scenarioHelper.EndScenario();
      return boardSettings;
    }

    private void UpdateExtensionsAndPopulateStateMappings(
      IVssRequestContext requestContext,
      BoardSettings boardSettings,
      WebApiTeam team,
      BacklogLevelConfiguration backlogLevel,
      CommonStructureProjectInfo project,
      PerformanceScenarioHelper scenarioHelper,
      bool validateTeamSettings = false)
    {
      WorkItemTypeExtension extension = (WorkItemTypeExtension) null;
      using (scenarioHelper.Measure("GetExtension"))
        extension = requestContext.GetService<IWorkItemTypeExtensionService>().GetExtension(requestContext, boardSettings.ExtensionId.Value, true);
      if (extension == null)
      {
        requestContext.Trace(240415, TraceLevel.Error, "WebAccess", TfsTraceLayers.BusinessLogic, "Extension was null when attempting to retrieve Board. BoardId: {0}, (Old)ExtensionId: {1}", (object) boardSettings.Id, (object) boardSettings.ExtensionId);
        extension = this.CreateExtension(requestContext, project, backlogLevel, team, 0, validateTeamSettings);
        if (extension == null)
        {
          requestContext.Trace(240416, TraceLevel.Error, "WebAccess", TfsTraceLayers.BusinessLogic, "Attempted to create extension for board but it failed. BoardId: {0}", (object) boardSettings.Id);
        }
        else
        {
          requestContext.Trace(240417, TraceLevel.Info, "WebAccess", TfsTraceLayers.BusinessLogic, "Extension for Board was updated. BoardId: {0}, (New)ExtensionId: {1}", (object) boardSettings.Id, (object) extension.Id);
          requestContext.GetService<BoardService>().UpdateBoardExtension(requestContext, project.GetId(), boardSettings, extension.Id);
        }
      }
      using (scenarioHelper.Measure("PopulateStateMappings"))
        boardSettings.PopulateStateMappings(requestContext, project, team, backlogLevel, extension);
    }

    internal virtual BoardSettings GetOrCreateBoardSettings(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      WebApiTeam team,
      BacklogLevelConfiguration backlogLevel,
      int reconcileTimeout,
      bool validateTeamSettings = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      ArgumentUtility.CheckForNull<BacklogLevelConfiguration>(backlogLevel, nameof (backlogLevel));
      BoardSettings boardSettings = this.GetBoardSettings(requestContext, team, backlogLevel, project, validateTeamSettings) ?? this.RestoreBoardSettings(requestContext, team, backlogLevel, project);
      if (boardSettings == null)
      {
        try
        {
          boardSettings = this.CreateBoardSettings(requestContext, project, team, backlogLevel, reconcileTimeout);
        }
        catch (BoardExistsException ex)
        {
          requestContext.Trace(240419, TraceLevel.Error, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Kanban sync failed for team {0}. Exception detail: {1}.", (object) (team.Name ?? "<null team name>"), (object) ex.ToString());
          boardSettings = this.GetBoardSettings(requestContext, team, backlogLevel, project);
        }
      }
      return boardSettings != null ? boardSettings : throw new TeamFoundationServiceException(Resources.BoardNotFoundException_Message);
    }

    internal virtual BoardSettings CreateBoardSettings(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      WebApiTeam team,
      BacklogLevelConfiguration backlogLevel,
      int reconcileTimeout)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<CommonStructureProjectInfo>(project, nameof (project));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      WorkItemTypeExtension extension = this.CreateExtension(requestContext, project, backlogLevel, team, reconcileTimeout, skipWITChangeDateUpdate: true);
      BoardSettings board;
      try
      {
        BoardSettings boardSettingsModel = this.CreateBoardSettingsModel(requestContext, project.GetId(), team, backlogLevel);
        boardSettingsModel.ExtensionId = new Guid?(extension.Id);
        boardSettingsModel.ExtensionLastChangeDate = extension.LastChangedDate;
        boardSettingsModel.BacklogLevelId = backlogLevel.Id;
        board = requestContext.GetService<BoardService>().CreateBoard(requestContext, project.GetId(), boardSettingsModel);
        board.PopulateStateMappings(requestContext, project, team, backlogLevel, extension);
        this.SetDefaultCFDSettings(requestContext, team, backlogLevel);
      }
      catch (TeamFoundationServerException ex)
      {
        requestContext.Trace(290007, TraceLevel.Error, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Kanban sync failed for team {0}. Exception detail: {1}.", (object) (team.Name ?? "<null team name>"), (object) ex.ToString());
        requestContext.GetService<IWorkItemTypeExtensionService>().DeleteExtensions(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          extension.Id
        });
        throw;
      }
      return board;
    }

    private void SetDefaultCFDSettings(
      IVssRequestContext requestContext,
      WebApiTeam team,
      BacklogLevelConfiguration backlogLevel)
    {
      ITeamConfigurationService service = requestContext.GetService<ITeamConfigurationService>();
      ITeamSettings teamSettings = service.GetTeamSettings(requestContext, team, false, false, true);
      if (teamSettings.CumulativeFlowDiagramSettings == null)
        teamSettings.CumulativeFlowDiagramSettings = (IDictionary<string, CumulativeFlowDiagramSettings>) new Dictionary<string, CumulativeFlowDiagramSettings>();
      CumulativeFlowDiagramSettings flowDiagramSettings;
      if (!teamSettings.CumulativeFlowDiagramSettings.TryGetValue(backlogLevel.Id, out flowDiagramSettings))
      {
        flowDiagramSettings = new CumulativeFlowDiagramSettings();
        teamSettings.CumulativeFlowDiagramSettings[backlogLevel.Id] = flowDiagramSettings;
      }
      service.SetCumulativeFlowDiagramSettings(requestContext.Elevate(), team, teamSettings.CumulativeFlowDiagramSettings);
    }

    private string GetDefaultRowName(IEnumerable<BoardRow> boardRows) => boardRows.FirstOrDefault<BoardRow>((Func<BoardRow, bool>) (r => r.IsDefault))?.Name;

    internal IEnumerable<WorkItemFieldRule> CreateExtensionDefaultFieldRules(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      BacklogLevelConfiguration backlogLevel)
    {
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration = requestContext.GetService<IBacklogConfigurationService>().GetProjectBacklogConfiguration(requestContext, project.GetId());
      bool flag = this.BugsOnBacklogFeatureAvailable(backlogConfiguration, backlogLevel);
      IReadOnlyCollection<string> orderedByCategory = backlogConfiguration.GetWorkItemStatesOrderedByCategory(backlogLevel, flag);
      string incomingColumnName = orderedByCategory.FirstOrDefault<string>();
      string outgoingColumnName = orderedByCategory.LastOrDefault<string>();
      WorkItemFieldRule workItemFieldRule = new WorkItemFieldRule()
      {
        Field = "[Kanban.Column]"
      };
      IReadOnlyCollection<string> backlogWorkItemTypes = this.GetEffectiveBacklogWorkItemTypes(backlogConfiguration, backlogLevel, flag);
      IEnumerable<BoardColumn> defaultBoardColumns = this.GetDefaultBoardColumns(requestContext, project.GetId(), backlogLevel);
      foreach (string workItemTypeName in (IEnumerable<string>) backlogWorkItemTypes)
        workItemFieldRule.AddRule<WhenRule>(this.CreateDefaultWhenRuleForWorkItemTypeFromBoardColumns(requestContext, project, backlogConfiguration, defaultBoardColumns, workItemTypeName));
      WorkItemFieldRule kanbanDoneFieldRule = this.CreateKanbanDoneFieldRule();
      WorkItemFieldRule kanbanLaneFieldRule = this.CreateKanbanLaneFieldRule(incomingColumnName, outgoingColumnName, (string) null, (string[]) null);
      return (IEnumerable<WorkItemFieldRule>) new List<WorkItemFieldRule>((IEnumerable<WorkItemFieldRule>) new WorkItemFieldRule[3]
      {
        workItemFieldRule,
        kanbanDoneFieldRule,
        kanbanLaneFieldRule
      });
    }

    public WhenRule CreateDefaultWhenRuleForWorkItemTypeByState(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      IEnumerable<BoardColumn> columns,
      IEnumerable<Tuple<string, WorkItemStateCategory>> states,
      string workItemTypeName)
    {
      WhenRule whenRule1 = new WhenRule();
      whenRule1.Field = CoreFieldReferenceNames.WorkItemType;
      whenRule1.Value = workItemTypeName;
      WhenRule workItemTypeByState = whenRule1;
      WhenRule whenRule2 = workItemTypeByState;
      MapRule rule = new MapRule();
      rule.Field = CoreFieldReferenceNames.State;
      rule.Else = new MapValues() { Default = string.Empty };
      MapRule mapRule = whenRule2.AddRule<MapRule>(rule);
      string str = requestContext.GetService<IWorkItemTypeService>().GetWorkItemTypeByReferenceName(requestContext, project.GetId(), workItemTypeName).GetAdditionalProperties(requestContext).Transitions[string.Empty].First<string>();
      List<string> list1 = states.Where<Tuple<string, WorkItemStateCategory>>((Func<Tuple<string, WorkItemStateCategory>, bool>) (s => s.Item2 == WorkItemStateCategory.Proposed || s.Item2 == WorkItemStateCategory.InProgress || s.Item2 == WorkItemStateCategory.Resolved)).Select<Tuple<string, WorkItemStateCategory>, string>((Func<Tuple<string, WorkItemStateCategory>, string>) (s => s.Item1)).ToList<string>();
      List<string> list2 = states.Where<Tuple<string, WorkItemStateCategory>>((Func<Tuple<string, WorkItemStateCategory>, bool>) (s => s.Item2 == WorkItemStateCategory.Completed)).Select<Tuple<string, WorkItemStateCategory>, string>((Func<Tuple<string, WorkItemStateCategory>, string>) (s => s.Item1)).ToList<string>();
      List<MapCase> mapCaseList1 = new List<MapCase>();
      foreach (BoardColumn column in columns)
      {
        string columnName = column.Name;
        BoardColumnType columnType = column.ColumnType;
        if (columnType == BoardColumnType.Incoming)
        {
          List<MapCase> mapCaseList2 = mapCaseList1;
          MapCase mapCase1 = new MapCase();
          mapCase1.Value = str;
          mapCase1.Values = new string[1]{ columnName };
          mapCase1.Default = columnName;
          MapCase mapCase2 = mapCase1;
          mapCaseList2.Add(mapCase2);
        }
        else
        {
          List<string> source = columnType == BoardColumnType.InProgress ? list1 : list2;
          string bestMatchCandidateState = source.Where<string>((Func<string, bool>) (s => TFStringComparer.BoardColumnName.Equals(columnName, s))).FirstOrDefault<string>();
          if (bestMatchCandidateState == null)
            bestMatchCandidateState = source.First<string>();
          if (mapCaseList1.Exists((Predicate<MapCase>) (m => TFStringComparer.BoardColumnName.Equals(bestMatchCandidateState, m.Value))))
          {
            MapCase mapCase = mapCaseList1.Find((Predicate<MapCase>) (m => TFStringComparer.BoardColumnName.Equals(bestMatchCandidateState, m.Value)));
            mapCase.Values = ((IEnumerable<string>) mapCase.Values).Union<string>((IEnumerable<string>) new string[1]
            {
              columnName
            }).ToArray<string>();
          }
          else
          {
            List<MapCase> mapCaseList3 = mapCaseList1;
            MapCase mapCase3 = new MapCase();
            mapCase3.Value = bestMatchCandidateState;
            mapCase3.Values = new string[1]{ columnName };
            mapCase3.Default = columnName;
            MapCase mapCase4 = mapCase3;
            mapCaseList3.Add(mapCase4);
          }
        }
      }
      mapRule.Cases = mapCaseList1.ToArray();
      return workItemTypeByState;
    }

    public WhenRule CreateDefaultWhenRuleForCustomWorkItemType(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      IEnumerable<BoardColumn> columns,
      IReadOnlyCollection<WorkItemStateDefinition> stateDefinitions,
      string workItemTypeName)
    {
      List<Tuple<string, WorkItemStateCategory>> states = new List<Tuple<string, WorkItemStateCategory>>();
      foreach (WorkItemStateDefinition stateDefinition in (IEnumerable<WorkItemStateDefinition>) stateDefinitions)
        states.Add(new Tuple<string, WorkItemStateCategory>(stateDefinition.Name, stateDefinition.StateCategory));
      return this.CreateDefaultWhenRuleForWorkItemTypeByState(requestContext, project, columns, (IEnumerable<Tuple<string, WorkItemStateCategory>>) states, workItemTypeName);
    }

    public WhenRule CreateDefaultWhenRuleForWorkItemTypeFromBoardColumns(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration,
      IEnumerable<BoardColumn> columns,
      string workItemTypeName)
    {
      IEnumerable<Tuple<string, WorkItemStateCategory>> categoryForWorkItem = backlogConfiguration.GetWorkItemStatesWithCategoryForWorkItem(workItemTypeName);
      return this.CreateDefaultWhenRuleForWorkItemTypeByState(requestContext, project, columns, categoryForWorkItem, workItemTypeName);
    }

    public Dictionary<string, string> GetAllBoardFieldNames(
      IVssRequestContext requestContext,
      Guid extensionId)
    {
      Dictionary<string, string> allBoardFieldNames = new Dictionary<string, string>();
      WorkItemTypeExtension extension = requestContext.GetService<IWorkItemTypeExtensionService>().GetExtension(requestContext, extensionId);
      if (extension != null)
      {
        allBoardFieldNames.Add("RowField", this.GetFieldReferenceName(requestContext, extension, "Kanban.Lane"));
        allBoardFieldNames.Add("ColumnField", this.GetFieldReferenceName(requestContext, extension, "Kanban.Column"));
        allBoardFieldNames.Add("DoneField", this.GetFieldReferenceName(requestContext, extension, "Kanban.Column.Done"));
      }
      return allBoardFieldNames;
    }

    public FieldEntry GetMarkerField(IVssRequestContext requestContext, Guid extensionId) => requestContext.GetService<IWorkItemTypeExtensionService>().GetExtension(requestContext, extensionId).MarkerField.Field;

    public virtual string GetKanbanLaneFieldReferenceName(
      IVssRequestContext requestContext,
      Guid extensionId)
    {
      WorkItemTypeExtension extension = requestContext.GetService<IWorkItemTypeExtensionService>().GetExtension(requestContext, extensionId);
      return this.GetFieldReferenceName(requestContext, extension, "Kanban.Lane");
    }

    public virtual string GetKanbanColumnFieldReferenceName(
      IVssRequestContext requestContext,
      Guid extensionId)
    {
      WorkItemTypeExtension extension = requestContext.GetService<IWorkItemTypeExtensionService>().GetExtension(requestContext, extensionId);
      return this.GetFieldReferenceName(requestContext, extension, "Kanban.Column");
    }

    public virtual string GetKanbanDoneFieldReferenceName(
      IVssRequestContext requestContext,
      Guid extensionId)
    {
      WorkItemTypeExtension extension = requestContext.GetService<IWorkItemTypeExtensionService>().GetExtension(requestContext, extensionId);
      return this.GetFieldReferenceName(requestContext, extension, "Kanban.Column.Done");
    }

    public string GetFieldReferenceName(
      IVssRequestContext requestContext,
      WorkItemTypeExtension extension,
      string extensionScopedFieldRefName)
    {
      string fieldReferenceName = (string) null;
      if (extension != null)
      {
        WorkItemTypeExtensionFieldEntry extensionFieldEntry = extension.Fields.FirstOrDefault<WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeExtensionFieldEntry, bool>) (f => TFStringComparer.WorkItemFieldReferenceName.Equals(f.LocalReferenceName, extensionScopedFieldRefName)));
        if (extensionFieldEntry != null)
          fieldReferenceName = extensionFieldEntry.Field.ReferenceName;
      }
      return fieldReferenceName;
    }

    public void EnsureKanbanForTeam(IVssRequestContext requestContext, WebApiTeam team)
    {
      ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, team.ProjectId);
      this.EnsureKanbanForTeam(requestContext, CommonStructureProjectInfo.ConvertProjectInfo(project), team);
    }

    public void EnsureKanbanForTeam(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      WebApiTeam team)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<CommonStructureProjectInfo>(project, nameof (project));
      ArgumentUtility.CheckForNull<WebApiTeam>(team, nameof (team));
      try
      {
        ITeamSettings teamSettings = requestContext.GetService<ITeamConfigurationService>().GetTeamSettings(requestContext, team, false, true);
        if (!((IEnumerable<ITeamFieldValue>) teamSettings.TeamFieldConfig.TeamFieldValues).Any<ITeamFieldValue>())
          requestContext.Trace(40420, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, "Bypassing Kanban provisioning because team settings are not initialized");
        else if (teamSettings.BacklogIterationId == Guid.Empty)
        {
          requestContext.Trace(240423, TraceLevel.Error, "Agile", TfsTraceLayers.BusinessLogic, "Empty backlog iteration id for team {0}", (object) team.ToString());
        }
        else
        {
          Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration = requestContext.GetService<IBacklogConfigurationService>().GetProjectBacklogConfiguration(requestContext, project.GetId());
          foreach (BacklogLevelConfiguration portfolioBacklog in (IEnumerable<BacklogLevelConfiguration>) backlogConfiguration.PortfolioBacklogs)
            KanbanUtils.TryUpdateKanbanExtension(requestContext, project, team, portfolioBacklog, teamSettings);
          KanbanUtils.TryUpdateKanbanExtension(requestContext, project, team, backlogConfiguration.RequirementBacklog, teamSettings);
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(290003, TraceLevel.Error, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Kanban sync failed for team {0}. Exception detail: {1}", (object) (team.Name ?? "<null team name>"), (object) ex.ToString());
        throw;
      }
    }

    public virtual IDictionary<string, IDictionary<string, string[]>> GetColumnTypeAllowedStateMappings(
      IVssRequestContext requestContext,
      string projectName,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration,
      BacklogLevelConfiguration backlogLevel,
      bool showBugsOnBacklog)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      DateTime now1 = DateTime.Now;
      IReadOnlyCollection<string> witNames = this.GetEffectiveBacklogWorkItemTypes(backlogConfiguration, backlogLevel, showBugsOnBacklog);
      IDictionary<string, IDictionary<string, string[]>> allowedStateMappings = (IDictionary<string, IDictionary<string, string[]>>) new Dictionary<string, IDictionary<string, string[]>>((IEqualityComparer<string>) TFStringComparer.BoardColumnName);
      allowedStateMappings[0.ToString()] = (IDictionary<string, string[]>) new Dictionary<string, string[]>((IEqualityComparer<string>) TFStringComparer.BoardColumnName);
      allowedStateMappings[1.ToString()] = (IDictionary<string, string[]>) new Dictionary<string, string[]>((IEqualityComparer<string>) TFStringComparer.BoardColumnName);
      allowedStateMappings[2.ToString()] = (IDictionary<string, string[]>) new Dictionary<string, string[]>((IEqualityComparer<string>) TFStringComparer.BoardColumnName);
      HashSet<string> second1 = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      HashSet<string> second2 = new HashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
      second1.UnionWith((IEnumerable<string>) backlogConfiguration.GetWorkItemStates(witNames, new WorkItemStateCategory[3]
      {
        WorkItemStateCategory.Proposed,
        WorkItemStateCategory.InProgress,
        WorkItemStateCategory.Resolved
      }));
      second2.UnionWith((IEnumerable<string>) backlogConfiguration.GetWorkItemStates(witNames, new WorkItemStateCategory[1]
      {
        WorkItemStateCategory.Completed
      }));
      WebAccessWorkItemService service = requestContext.GetService<WebAccessWorkItemService>();
      DateTime now2 = DateTime.Now;
      IEnumerable<IWorkItemType> workItemTypes = service.GetWorkItemTypes(requestContext, service.GetProjectId(requestContext, projectName)).Where<IWorkItemType>((Func<IWorkItemType, bool>) (w => witNames.Contains<string>(w.Name, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName)));
      properties.Add("GetWorkItemTypes", (DateTime.Now - now2).TotalMilliseconds);
      DateTime now3 = DateTime.Now;
      IDictionary<string, IList<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition>> itemTypeTransitions = service.GetWorkItemTypeTransitions(requestContext, workItemTypes);
      properties.Add("GetWorkItemTypeTransitions", (DateTime.Now - now3).TotalMilliseconds);
      Dictionary<string, string> initialStateMapping = this.GetWorkItemTypeInitialStateMapping(itemTypeTransitions);
      Dictionary<string, IList<string>> basedOnTransitions = this.GetAllowedValuesBasedOnTransitions(itemTypeTransitions);
      DateTime now4 = DateTime.Now;
      foreach (IWorkItemType workItemType in workItemTypes)
      {
        IList<string> first = basedOnTransitions[workItemType.Name];
        IDictionary<string, IDictionary<string, string[]>> dictionary1 = allowedStateMappings;
        int num = 0;
        string key1 = num.ToString();
        dictionary1[key1][workItemType.Name] = new string[1]
        {
          initialStateMapping[workItemType.Name]
        };
        IDictionary<string, IDictionary<string, string[]>> dictionary2 = allowedStateMappings;
        num = 1;
        string key2 = num.ToString();
        dictionary2[key2][workItemType.Name] = first.Intersect<string>((IEnumerable<string>) second1).ToArray<string>();
        IDictionary<string, IDictionary<string, string[]>> dictionary3 = allowedStateMappings;
        num = 2;
        string key3 = num.ToString();
        dictionary3[key3][workItemType.Name] = first.Intersect<string>((IEnumerable<string>) second2).ToArray<string>();
      }
      CustomerIntelligenceData intelligenceData = properties;
      TimeSpan timeSpan = DateTime.Now - now4;
      double totalMilliseconds1 = timeSpan.TotalMilliseconds;
      intelligenceData.Add("GetAllowedValues", totalMilliseconds1);
      timeSpan = DateTime.Now - now1;
      double totalMilliseconds2 = timeSpan.TotalMilliseconds;
      properties.Add("ElapsedTime", totalMilliseconds2);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Agile", nameof (GetColumnTypeAllowedStateMappings), properties);
      return allowedStateMappings;
    }

    public static string GetCumulativeFlowHideIncomingProperty(string backlogLevelId) => TeamConstants.CumulativeFlowDiagramHideIncoming + "." + backlogLevelId;

    private int GetExtensionRank(bool isAreaPath, ITeamFieldSettings teamFields)
    {
      if (!isAreaPath)
        return -10 * teamFields.TeamFieldValues.Length;
      int val1 = 0;
      foreach (ITeamFieldValue teamFieldValue in teamFields.TeamFieldValues)
      {
        int val2 = teamFieldValue.Value.Split(new string[1]
        {
          "\\"
        }, StringSplitOptions.None).Length * 10;
        val1 = Math.Max(val1, val2);
      }
      return val1;
    }

    private IEnumerable<WorkItemFieldRule> CreateRules(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      WebApiTeam team,
      BacklogLevelConfiguration backlogLevel,
      BoardSettings boardSettings,
      ITeamSettings teamSettings,
      IEnumerable<BoardColumn> deletedColumns,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfig,
      bool bugsOnBacklogFeatureAvailable,
      WorkItemFieldRule kanbanColumnExtensionFieldRule,
      out IEnumerable<WorkItemFieldRule> reconciliationScopeRules,
      BoardSettings oldBoardSettings = null)
    {
      IEnumerable<WorkItemFieldRule> extensionFieldRules = this.CreateExtensionFieldRules(requestContext, project, team, backlogLevel, boardSettings, out reconciliationScopeRules, oldBoardSettings);
      this.AddBugsOnBacklogRules(requestContext, project, teamSettings, deletedColumns, extensionFieldRules, backlogConfig, bugsOnBacklogFeatureAvailable, kanbanColumnExtensionFieldRule);
      return extensionFieldRules;
    }

    private string[] GetBoardRowNames(BoardSettings board) => !board.Rows.Any<BoardRow>() ? (string[]) null : board.Rows.Select<BoardRow, string>((Func<BoardRow, string>) (r => r.Name)).ToArray<string>();

    private void AddBugsOnBacklogRules(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      ITeamSettings teamSettings,
      IEnumerable<BoardColumn> deletedColumns,
      IEnumerable<WorkItemFieldRule> rules,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfig,
      bool bugsOnBacklogFeatureAvailable,
      WorkItemFieldRule kanbanColumnExtensionFieldRule)
    {
      if (!bugsOnBacklogFeatureAvailable || teamSettings.BugsBehavior == BugsBehavior.AsRequirements)
        return;
      IEnumerable<WhenRule> bugWorkItemRules = this.GetBugWorkItemRules(requestContext, project, backlogConfig, teamSettings, kanbanColumnExtensionFieldRule);
      if (deletedColumns != null && deletedColumns.Any<BoardColumn>() && bugWorkItemRules != null && bugWorkItemRules.Any<WhenRule>())
        this.CleanUpDeletedColumnsFromRules(bugWorkItemRules, deletedColumns);
      if (bugWorkItemRules == null || !bugWorkItemRules.Any<WhenRule>())
        return;
      WorkItemFieldRule workItemFieldRule = rules.First<WorkItemFieldRule>((Func<WorkItemFieldRule, bool>) (r => r.Field == "[Kanban.Column]"));
      workItemFieldRule.SubRules = (WorkItemRule[]) workItemFieldRule.SelectRules<WhenRule>().Union<WhenRule>(bugWorkItemRules).ToArray<WhenRule>();
    }

    private IEnumerable<WorkItemTypeExtensionFieldDeclaration> GetExtensionFieldsToAddIfNecessary(
      IVssRequestContext requestContext,
      WorkItemTypeExtension extension,
      BacklogLevelConfiguration backlogLevel)
    {
      List<WorkItemTypeExtensionFieldDeclaration> toAddIfNecessary = new List<WorkItemTypeExtensionFieldDeclaration>();
      if (!extension.Fields.Where<WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeExtensionFieldEntry, bool>) (f => f.LocalReferenceName == "Kanban.Column.Done")).Any<WorkItemTypeExtensionFieldEntry>())
        toAddIfNecessary.AddRange((IEnumerable<WorkItemTypeExtensionFieldDeclaration>) this.CreateExtensionFields(requestContext, backlogLevel));
      else if (!extension.Fields.Where<WorkItemTypeExtensionFieldEntry>((Func<WorkItemTypeExtensionFieldEntry, bool>) (f => f.LocalReferenceName == "Kanban.Lane")).Any<WorkItemTypeExtensionFieldEntry>())
        toAddIfNecessary.AddRange((IEnumerable<WorkItemTypeExtensionFieldDeclaration>) this.CreateExtensionFields(requestContext, backlogLevel));
      return (IEnumerable<WorkItemTypeExtensionFieldDeclaration>) toAddIfNecessary;
    }

    public IEnumerable<WhenRule> GetBugWorkItemRules(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfig,
      ITeamSettings teamSettings,
      WorkItemFieldRule extensionFieldRule)
    {
      return backlogConfig.BugWorkItemTypes == null ? (IEnumerable<WhenRule>) null : extensionFieldRule.SelectRules<WhenRule>().Where<WhenRule>((Func<WhenRule, bool>) (r => backlogConfig.BugWorkItemTypes.Contains<string>(r.Value, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName)));
    }

    public void CleanUpDeletedColumnsFromRules(
      IEnumerable<WhenRule> whenRules,
      IEnumerable<BoardColumn> deletedColumns)
    {
      foreach (RuleBlock whenRule in whenRules)
      {
        MapRule mapRule = whenRule.SelectRules<MapRule>().Where<MapRule>((Func<MapRule, bool>) (r => r.Field == CoreFieldReferenceNames.State)).First<MapRule>();
        List<MapCase> mapCaseList = new List<MapCase>();
        foreach (MapCase mapCase in mapRule.Cases)
        {
          IEnumerable<string> strings = deletedColumns.Select<BoardColumn, string>((Func<BoardColumn, string>) (c => c.Name));
          IEnumerable<string> second = ((IEnumerable<string>) mapCase.Values).Intersect<string>(strings, (IEqualityComparer<string>) TFStringComparer.BoardColumnName);
          IEnumerable<string> source = ((IEnumerable<string>) mapCase.Values).Except<string>(second, (IEqualityComparer<string>) TFStringComparer.BoardColumnName);
          if (!source.Any<string>())
          {
            mapCaseList.Add(mapCase);
          }
          else
          {
            mapCase.Values = source.ToArray<string>();
            if (strings.Contains<string>(mapCase.Default, (IEqualityComparer<string>) TFStringComparer.BoardColumnName))
              mapCase.Default = ((IEnumerable<string>) mapCase.Values).First<string>();
          }
        }
        mapRule.Cases = ((IEnumerable<MapCase>) mapRule.Cases).Except<MapCase>((IEnumerable<MapCase>) mapCaseList.ToArray()).ToArray<MapCase>();
      }
    }

    private void ValidateAndUpdateWorkItemRules(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      WebApiTeam team,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration,
      BacklogLevelConfiguration backlogLevel,
      WorkItemFieldRule extensionFieldRule)
    {
      BoardSettings boardSettings = KanbanUtils.Instance.GetBoardSettings(requestContext, team, backlogLevel, project);
      IReadOnlyCollection<string> backlogWorkItemTypeNames = this.GetEffectiveBacklogWorkItemTypes(backlogConfiguration, backlogLevel, true);
      IEnumerable<string> workItemTypesWithRules = extensionFieldRule.SelectRules<WhenRule>().Select<WhenRule, string>((Func<WhenRule, string>) (r => r.Value));
      IEnumerable<WhenRule> whenRules = extensionFieldRule.SelectRules<WhenRule>().Where<WhenRule>((Func<WhenRule, bool>) (r => !backlogWorkItemTypeNames.Contains<string>(r.Value, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName)));
      IEnumerable<string> strings = backlogWorkItemTypeNames.Where<string>((Func<string, bool>) (workItem => !workItemTypesWithRules.Contains<string>(workItem, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName)));
      List<WhenRule> whenRuleList = new List<WhenRule>();
      foreach (string workItemTypeName in strings)
        whenRuleList.Add(this.CreateDefaultWhenRuleForWorkItemTypeFromBoardColumns(requestContext, project, backlogConfiguration, boardSettings.Columns, workItemTypeName));
      if (!whenRules.Any<WhenRule>() && !whenRuleList.Any<WhenRule>())
        return;
      IEnumerable<WhenRule> source = extensionFieldRule.SelectRules<WhenRule>().Except<WhenRule>(whenRules).Union<WhenRule>((IEnumerable<WhenRule>) whenRuleList);
      extensionFieldRule.Field = extensionFieldRule.Field;
      extensionFieldRule.SubRules = (WorkItemRule[]) source.ToArray<WhenRule>();
    }

    internal virtual Dictionary<string, string> GetWorkItemTypeInitialStateMapping(
      IDictionary<string, IList<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition>> witTransitions)
    {
      Dictionary<string, string> initialStateMapping = new Dictionary<string, string>(witTransitions.Count, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      foreach (KeyValuePair<string, IList<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition>> witTransition in (IEnumerable<KeyValuePair<string, IList<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition>>>) witTransitions)
      {
        foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition itemTypeTransition in (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition>) witTransition.Value)
        {
          if (string.IsNullOrEmpty(itemTypeTransition.FromState))
          {
            initialStateMapping[witTransition.Key] = itemTypeTransition.ToState;
            break;
          }
        }
      }
      return initialStateMapping;
    }

    internal virtual Dictionary<string, IList<string>> GetAllowedValuesBasedOnTransitions(
      IDictionary<string, IList<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition>> witsTransitions)
    {
      Dictionary<string, IList<string>> basedOnTransitions = new Dictionary<string, IList<string>>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      foreach (KeyValuePair<string, IList<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition>> witsTransition in (IEnumerable<KeyValuePair<string, IList<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition>>>) witsTransitions)
      {
        List<string> list = witsTransition.Value.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition, string>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition, string>) (t => t.FromState)).Distinct<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName).Union<string>(witsTransition.Value.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition, string>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTypeTransition, string>) (t => t.ToState)).Distinct<string>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName)).Where<string>((Func<string, bool>) (state => !string.IsNullOrEmpty(state))).ToList<string>();
        basedOnTransitions[witsTransition.Key] = (IList<string>) list;
      }
      return basedOnTransitions;
    }

    private static bool TryUpdateKanbanExtension(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      WebApiTeam team,
      BacklogLevelConfiguration backlogLevel,
      ITeamSettings teamSettings)
    {
      try
      {
        BoardSettings boardSettings = KanbanUtils.Instance.GetBoardSettings(requestContext, team, backlogLevel, project);
        if (boardSettings == null)
          return false;
        KanbanUtils.Instance.UpdateExtension(requestContext, project, team, backlogLevel, boardSettings, true, false, 25, teamSettings);
        return true;
      }
      catch (Exception ex)
      {
        requestContext.Trace(240413, TraceLevel.Error, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Kanban extension update failed {0}. Exception detail: {1}", (object) (team.Name ?? "<null team name>"), (object) ex.ToString());
        return false;
      }
    }

    private static void EnsureBoard(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      WebApiTeam team,
      BacklogLevelConfiguration backlogLevel,
      ITeamSettings teamSettings)
    {
      try
      {
        requestContext.TraceEnter(240411, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (EnsureBoard));
        if (KanbanUtils.TryUpdateKanbanExtension(requestContext, project, team, backlogLevel, teamSettings))
          return;
        KanbanUtils.Instance.CreateBoardSettings(requestContext, project, team, backlogLevel, 1000);
      }
      catch (Exception ex)
      {
        requestContext.Trace(240414, TraceLevel.Error, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Kanban sync failed for team {0}. Exception detail: {1}.", (object) (team.Name ?? "<null team name>"), (object) ex.ToString());
        throw;
      }
      finally
      {
        requestContext.TraceLeave(240412, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (EnsureBoard));
      }
    }

    private WorkItemTypeExtension CreateExtension(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      BacklogLevelConfiguration backlogLevel,
      WebApiTeam team,
      int reconcileTimeout,
      bool validateTeamSettings = false,
      bool skipWITChangeDateUpdate = false)
    {
      WorkItemTypeExtensionFieldDeclaration[] extensionFields = this.CreateExtensionFields(requestContext, backlogLevel);
      ITeamSettings teamSettings = requestContext.GetService<ITeamConfigurationService>().GetTeamSettings(requestContext, team, validateTeamSettings, true);
      WorkItemExtensionPredicate extensionPredicate = this.CreateExtensionPredicate(requestContext, project, team, backlogLevel, teamSettings);
      IEnumerable<WorkItemFieldRule> defaultFieldRules = this.CreateExtensionDefaultFieldRules(requestContext, project, backlogLevel);
      string form = (string) null;
      PerformanceScenarioHelper performanceScenarioHelper = new PerformanceScenarioHelper(requestContext, "Agile", "CreateKanbanExtension");
      WorkItemTypeExtension extension;
      this.CreateKanbanExtension(requestContext, project, team, backlogLevel, reconcileTimeout, extensionFields, extensionPredicate, defaultFieldRules, form, out extension, skipWITChangeDateUpdate);
      performanceScenarioHelper.Add("success", (object) (extension != null));
      performanceScenarioHelper.EndScenario();
      return extension;
    }

    private WorkItemTypeExtensionFieldDeclaration[] CreateExtensionFields(
      IVssRequestContext requestContext,
      BacklogLevelConfiguration backlogLevel)
    {
      return new List<WorkItemTypeExtensionFieldDeclaration>()
      {
        this.CreateKanbanColumnField(backlogLevel),
        this.CreateKanbanDoneField(backlogLevel),
        this.CreateKanbanLaneField(backlogLevel)
      }.ToArray();
    }

    private WorkItemTypeExtensionFieldDeclaration CreateKanbanColumnField(
      BacklogLevelConfiguration backlogLevel)
    {
      string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.BoardFieldLocalName, (object) backlogLevel.Name);
      return new WorkItemTypeExtensionFieldDeclaration()
      {
        ExtensionScoped = true,
        Name = str,
        ParentFieldId = 90,
        ReferenceName = "Kanban.Column",
        ReportingType = FieldReportingType.Dimension,
        Type = FieldDBType.Keyword
      };
    }

    private WorkItemTypeExtensionFieldDeclaration CreateKanbanDoneField(
      BacklogLevelConfiguration backlogLevel)
    {
      string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.KanbanBoardDoneFieldLocalName, (object) backlogLevel.Name);
      return new WorkItemTypeExtensionFieldDeclaration()
      {
        ExtensionScoped = true,
        Name = str,
        ParentFieldId = 91,
        ReferenceName = "Kanban.Column.Done",
        ReportingType = FieldReportingType.Dimension,
        Type = FieldDBType.Bit
      };
    }

    private WorkItemTypeExtensionFieldDeclaration CreateKanbanLaneField(
      BacklogLevelConfiguration backlogLevel)
    {
      string str = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.KanbanBoardLaneFieldLocalName, (object) backlogLevel.Name);
      return new WorkItemTypeExtensionFieldDeclaration()
      {
        ExtensionScoped = true,
        Name = str,
        ParentFieldId = 92,
        ReferenceName = "Kanban.Lane",
        ReportingType = FieldReportingType.Dimension,
        Type = FieldDBType.Keyword
      };
    }

    private void CreateKanbanExtension(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      WebApiTeam team,
      BacklogLevelConfiguration backlogLevel,
      int reconcileTimeout,
      WorkItemTypeExtensionFieldDeclaration[] fields,
      WorkItemExtensionPredicate predicate,
      IEnumerable<WorkItemFieldRule> rules,
      string form,
      out WorkItemTypeExtension extension,
      bool skipWITChangeDateUpdate = false)
    {
      requestContext.Trace(240401, TraceLevel.Verbose, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Creating Kanban extension for team: {0}, backlog: {1}", (object) team.Name, (object) backlogLevel.Id);
      IWorkItemTypeExtensionService service = requestContext.GetService<IWorkItemTypeExtensionService>();
      ProjectProcessConfiguration processConfiguration = requestContext.GetProjectProcessConfiguration(project.Uri);
      ITeamSettings teamSettings = requestContext.GetService<ITeamConfigurationService>().GetTeamSettings(requestContext, team, false, true);
      extension = service.CreateExtension(requestContext, project.GetId(), team.Id, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.{2}", (object) Guid.NewGuid(), (object) "Kanban.Column", (object) backlogLevel.Id), string.Format("Work item type extension for backlog: {0}", (object) backlogLevel.Id), (IEnumerable<WorkItemTypeExtensionFieldDeclaration>) fields, predicate, rules, this.GetExtensionRank(processConfiguration.IsTeamFieldAreaPath(), teamSettings.TeamFieldConfig), form, reconcileTimeout, out ReconcileRequestResult _, skipWITChangeDateUpdate);
      requestContext.Trace(240402, TraceLevel.Verbose, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Successfully created Kanban extension for team: {0}, backlog: {1}", (object) team.Name, (object) backlogLevel.Id);
    }

    internal virtual WorkItemExtensionPredicate CreateExtensionPredicate(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      WebApiTeam team,
      BacklogLevelConfiguration backlogLevel,
      ITeamSettings teamSettings)
    {
      requestContext.Trace(240403, TraceLevel.Verbose, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Creating Kanban extension predicate for team: {0}", (object) team.Name);
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration = requestContext.GetService<IBacklogConfigurationService>().GetProjectBacklogConfiguration(requestContext, project.GetId());
      IReadOnlyCollection<string> backlogWorkItemTypes = this.GetEffectiveBacklogWorkItemTypes(backlogConfiguration, backlogLevel, teamSettings.BugsBehavior == BugsBehavior.AsRequirements);
      PredicateInOperator predicateInOperator1 = new PredicateInOperator();
      predicateInOperator1.Field = CoreFieldReferenceNames.WorkItemType;
      predicateInOperator1.Value = (object) backlogWorkItemTypes.ToArray<string>();
      IReadOnlyCollection<string> workItemStates = backlogConfiguration.GetWorkItemStates(backlogWorkItemTypes);
      PredicateInOperator predicateInOperator2 = new PredicateInOperator();
      predicateInOperator2.Field = CoreFieldReferenceNames.State;
      predicateInOperator2.Value = (object) workItemStates.ToArray<string>();
      ITreeDictionary snapshot = requestContext.GetService<WorkItemTrackingTreeService>().GetSnapshot(requestContext);
      string name = requestContext.GetProjectProcessConfiguration(project.Uri).TeamField.Name;
      bool flag = TFStringComparer.WorkItemFieldReferenceName.Equals(CoreFieldReferenceNames.AreaPath, name);
      List<PredicateOperator> source = new List<PredicateOperator>();
      List<PredicateOperator> predicateOperatorList1 = new List<PredicateOperator>();
      foreach (ITeamFieldValue teamFieldValue in teamSettings.TeamFieldConfig.TeamFieldValues)
      {
        if (flag)
        {
          PredicateFieldComparisonOperator comparisonOperator = !teamFieldValue.IncludeChildren ? (PredicateFieldComparisonOperator) new PredicateEqualsOperator() : (PredicateFieldComparisonOperator) new PredicateUnderOperator();
          comparisonOperator.Field = CoreFieldReferenceNames.AreaId;
          comparisonOperator.Value = (object) snapshot.LegacyGetTreeNodeIdFromPath(requestContext, teamFieldValue.Value, TreeStructureType.Area);
          predicateOperatorList1.Add((PredicateOperator) comparisonOperator);
        }
        else
        {
          PredicateEqualsOperator predicateEqualsOperator = new PredicateEqualsOperator();
          predicateEqualsOperator.Field = name;
          predicateEqualsOperator.Value = (object) teamFieldValue.Value;
          predicateOperatorList1.Add((PredicateOperator) predicateEqualsOperator);
        }
      }
      if (predicateOperatorList1.Count > 1)
      {
        List<PredicateOperator> predicateOperatorList2 = source;
        PredicateOrOperator predicateOrOperator = new PredicateOrOperator();
        predicateOrOperator.Operands = predicateOperatorList1.ToArray();
        predicateOperatorList2.Add((PredicateOperator) predicateOrOperator);
      }
      else
        source.Add(predicateOperatorList1[0]);
      Guid id = project.GetId();
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode = snapshot.GetTreeNode(id, teamSettings.BacklogIterationId);
      PredicateUnderOperator predicateUnderOperator = new PredicateUnderOperator();
      predicateUnderOperator.Field = CoreFieldReferenceNames.IterationId;
      predicateUnderOperator.Value = (object) snapshot.LegacyGetTreeNodeIdFromPath(requestContext, treeNode.GetPath(requestContext), TreeStructureType.Iteration);
      source.Add((PredicateOperator) predicateUnderOperator);
      PredicateOperator predicateOperator;
      if (source.Count > 1)
      {
        PredicateAndOperator predicateAndOperator = new PredicateAndOperator();
        predicateAndOperator.Operands = source.ToArray();
        predicateOperator = (PredicateOperator) predicateAndOperator;
      }
      else
        predicateOperator = source.First<PredicateOperator>();
      requestContext.Trace(240404, TraceLevel.Verbose, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Successfully created Kanban extension predicate for team: {0}", (object) team.Name);
      if (predicateInOperator1 != null)
      {
        WorkItemExtensionPredicate extensionPredicate1 = new WorkItemExtensionPredicate();
        WorkItemExtensionPredicate extensionPredicate2 = extensionPredicate1;
        PredicateAndOperator predicateAndOperator1 = new PredicateAndOperator();
        predicateAndOperator1.Operands = new PredicateOperator[3]
        {
          (PredicateOperator) predicateInOperator1,
          (PredicateOperator) predicateInOperator2,
          predicateOperator
        };
        PredicateAndOperator predicateAndOperator2 = predicateAndOperator1;
        extensionPredicate2.Operand = (PredicateOperator) predicateAndOperator2;
        return extensionPredicate1;
      }
      WorkItemExtensionPredicate extensionPredicate3 = new WorkItemExtensionPredicate();
      WorkItemExtensionPredicate extensionPredicate4 = extensionPredicate3;
      PredicateAndOperator predicateAndOperator3 = new PredicateAndOperator();
      predicateAndOperator3.Operands = new PredicateOperator[3]
      {
        (PredicateOperator) predicateInOperator1,
        (PredicateOperator) predicateInOperator2,
        predicateOperator
      };
      PredicateAndOperator predicateAndOperator4 = predicateAndOperator3;
      extensionPredicate4.Operand = (PredicateOperator) predicateAndOperator4;
      return extensionPredicate3;
    }

    private IEnumerable<WorkItemFieldRule> CreateExtensionFieldRules(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo project,
      WebApiTeam team,
      BacklogLevelConfiguration backlogLevel,
      BoardSettings newBoardSettings,
      out IEnumerable<WorkItemFieldRule> reconciliationScopeRules,
      BoardSettings passedInOldBoardSettings = null)
    {
      requestContext.Trace(240405, TraceLevel.Verbose, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Creating Kanban extension field rules for team: {0}", (object) team.Name);
      reconciliationScopeRules = (IEnumerable<WorkItemFieldRule>) null;
      BoardSettings oldBoardSettings = passedInOldBoardSettings ?? KanbanUtils.Instance.GetBoardSettings(requestContext, team, backlogLevel, project);
      Dictionary<string, Dictionary<string, List<string>>> dictionary1 = new Dictionary<string, Dictionary<string, List<string>>>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      foreach (BoardColumn column in newBoardSettings.Columns)
      {
        foreach (KeyValuePair<string, string> stateMapping in column.StateMappings)
        {
          string key1 = stateMapping.Key;
          string key2 = stateMapping.Value;
          Dictionary<string, List<string>> dictionary2;
          if (!dictionary1.TryGetValue(key1, out dictionary2))
          {
            dictionary2 = new Dictionary<string, List<string>>((IEqualityComparer<string>) TFStringComparer.WorkItemStateName);
            dictionary1[key1] = dictionary2;
          }
          List<string> stringList;
          if (!dictionary2.TryGetValue(key2, out stringList))
          {
            dictionary2[key2] = new List<string>();
            stringList = dictionary2[key2];
          }
          stringList.Add(column.Name.Trim());
        }
      }
      WorkItemFieldRule workItemFieldRule1 = new WorkItemFieldRule()
      {
        Field = "[Kanban.Column]"
      };
      foreach (KeyValuePair<string, Dictionary<string, List<string>>> keyValuePair1 in dictionary1)
      {
        string key3 = keyValuePair1.Key;
        Dictionary<string, List<string>> dictionary3 = keyValuePair1.Value;
        WorkItemFieldRule workItemFieldRule2 = workItemFieldRule1;
        WhenRule rule1 = new WhenRule();
        rule1.Field = CoreFieldReferenceNames.WorkItemType;
        rule1.Value = key3;
        WhenRule whenRule = workItemFieldRule2.AddRule<WhenRule>(rule1);
        IEnumerable<WorkItemFieldRule> renameAndDeleteRules = this.GenerateRenameAndDeleteRules(requestContext, newBoardSettings, oldBoardSettings);
        if (renameAndDeleteRules.Any<WorkItemFieldRule>())
          reconciliationScopeRules = renameAndDeleteRules;
        MapRule rule2 = new MapRule();
        rule2.Field = CoreFieldReferenceNames.State;
        rule2.Else = new MapValues()
        {
          Default = string.Empty
        };
        MapRule mapRule = whenRule.AddRule<MapRule>(rule2);
        List<MapCase> mapCaseList1 = new List<MapCase>();
        foreach (KeyValuePair<string, List<string>> keyValuePair2 in dictionary3)
        {
          string key4 = keyValuePair2.Key;
          List<string> source = keyValuePair2.Value;
          List<MapCase> mapCaseList2 = mapCaseList1;
          MapCase mapCase = new MapCase();
          mapCase.Value = key4;
          mapCase.Values = source.ToArray();
          mapCase.Default = source.First<string>();
          mapCaseList2.Add(mapCase);
        }
        mapRule.Cases = mapCaseList1.ToArray();
      }
      WorkItemFieldRule kanbanDoneFieldRule = this.CreateKanbanDoneFieldRule();
      WorkItemFieldRule kanbanLaneFieldRule = this.CreateKanbanLaneFieldRule(newBoardSettings.Columns.First<BoardColumn>((Func<BoardColumn, bool>) (c => c.ColumnType == BoardColumnType.Incoming)).Name, newBoardSettings.Columns.First<BoardColumn>((Func<BoardColumn, bool>) (c => c.ColumnType == BoardColumnType.Outgoing)).Name, this.GetDefaultRowName(newBoardSettings.Rows), this.GetBoardRowNames(newBoardSettings));
      List<WorkItemFieldRule> extensionFieldRules = new List<WorkItemFieldRule>((IEnumerable<WorkItemFieldRule>) new WorkItemFieldRule[3]
      {
        workItemFieldRule1,
        kanbanDoneFieldRule,
        kanbanLaneFieldRule
      });
      requestContext.Trace(240406, TraceLevel.Verbose, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Successfully created Kanban extension field rules for team: {0}", (object) team.Name);
      return (IEnumerable<WorkItemFieldRule>) extensionFieldRules;
    }

    private IEnumerable<WorkItemFieldRule> GenerateRenameAndDeleteRules(
      IVssRequestContext requestContext,
      BoardSettings newBoardSettings,
      BoardSettings oldBoardSettings)
    {
      List<WorkItemFieldRule> renameAndDeleteRules = new List<WorkItemFieldRule>();
      WorkItemFieldRule workItemFieldRule1 = new WorkItemFieldRule()
      {
        Field = "[Kanban.Column]"
      };
      WorkItemFieldRule workItemFieldRule2 = new WorkItemFieldRule()
      {
        Field = "[Kanban.Column.Done]"
      };
      foreach (BoardColumn column1 in newBoardSettings.Columns)
      {
        BoardColumn column = column1;
        if (column.Id.HasValue)
        {
          BoardColumn boardColumn = oldBoardSettings.Columns.FirstOrDefault<BoardColumn>((Func<BoardColumn, bool>) (c =>
          {
            Guid? id = c.Id;
            Guid guid = column.Id.Value;
            if (!id.HasValue)
              return false;
            return !id.HasValue || id.GetValueOrDefault() == guid;
          }));
          if (boardColumn != null && !TFStringComparer.BoardColumnNameCaseSensitive.Equals(boardColumn.Name, column.Name))
          {
            WorkItemFieldRule workItemFieldRule3 = workItemFieldRule1;
            WhenWasRule rule1 = new WhenWasRule();
            rule1.Field = "[Kanban.Column]";
            rule1.Value = boardColumn.Name;
            WhenWasRule whenWasRule1 = workItemFieldRule3.AddRule<WhenWasRule>(rule1);
            CopyRule rule2 = new CopyRule();
            rule2.Value = column.Name;
            whenWasRule1.AddRule<CopyRule>(rule2);
            WorkItemFieldRule workItemFieldRule4 = workItemFieldRule2;
            WhenWasRule rule3 = new WhenWasRule();
            rule3.Field = "[Kanban.Column]";
            rule3.Value = boardColumn.Name;
            WhenWasRule whenWasRule2 = workItemFieldRule4.AddRule<WhenWasRule>(rule3);
            CopyRule rule4 = new CopyRule();
            rule4.ValueFrom = RuleValueFrom.OriginalValue;
            whenWasRule2.AddRule<CopyRule>(rule4);
          }
        }
      }
      if (((IEnumerable<WorkItemRule>) workItemFieldRule1.SubRules).Any<WorkItemRule>())
      {
        renameAndDeleteRules.Add(workItemFieldRule1);
        renameAndDeleteRules.Add(workItemFieldRule2);
      }
      string name1 = oldBoardSettings.Columns.FirstOrDefault<BoardColumn>((Func<BoardColumn, bool>) (c => c.ColumnType == BoardColumnType.Incoming)).Name;
      string name2 = oldBoardSettings.Columns.FirstOrDefault<BoardColumn>((Func<BoardColumn, bool>) (c => c.ColumnType == BoardColumnType.Outgoing)).Name;
      WorkItemFieldRule laneReconcileRule = this.GetKanbanLaneReconcileRule(newBoardSettings.Rows, oldBoardSettings.Rows, name1, name2);
      if (((IEnumerable<WorkItemRule>) laneReconcileRule.SubRules).Any<WorkItemRule>())
        renameAndDeleteRules.Add(laneReconcileRule);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("OldBoardRows", (object) this.GetBoardRowNames(oldBoardSettings));
      properties.Add("NewBoardRows", (object) this.GetBoardRowNames(newBoardSettings));
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Agile", "GetKanbanLaneReconcileRule", properties);
      return (IEnumerable<WorkItemFieldRule>) renameAndDeleteRules;
    }

    public WorkItemFieldRule GetKanbanLaneReconcileRule(
      IEnumerable<BoardRow> newBoardSettingRows,
      IEnumerable<BoardRow> oldBoardSettingRows,
      string oldIncomingColumnName,
      string oldOutgoingColumnName)
    {
      WorkItemFieldRule laneReconcileRule = new WorkItemFieldRule()
      {
        Field = "[Kanban.Lane]"
      };
      foreach (BoardRow newBoardSettingRow in newBoardSettingRows)
      {
        BoardRow row = newBoardSettingRow;
        if (row.Id.HasValue)
        {
          BoardRow boardRow = oldBoardSettingRows.FirstOrDefault<BoardRow>((Func<BoardRow, bool>) (r =>
          {
            Guid? id = r.Id;
            Guid guid = row.Id.Value;
            if (!id.HasValue)
              return false;
            return !id.HasValue || id.GetValueOrDefault() == guid;
          }));
          if (boardRow != null && !TFStringComparer.BoardRowName.Equals(boardRow.Name, row.Name))
          {
            WorkItemFieldRule workItemFieldRule = laneReconcileRule;
            WhenRule rule1 = new WhenRule();
            rule1.Inverse = true;
            rule1.Field = "[Kanban.Column]";
            rule1.Value = oldIncomingColumnName;
            WhenRule whenRule1 = workItemFieldRule.AddRule<WhenRule>(rule1);
            WhenRule rule2 = new WhenRule();
            rule2.Inverse = true;
            rule2.Field = "[Kanban.Column]";
            rule2.Value = oldOutgoingColumnName;
            WhenRule whenRule2 = whenRule1.AddRule<WhenRule>(rule2);
            WhenRule rule3 = new WhenRule();
            rule3.Field = "[Kanban.Lane]";
            rule3.Value = boardRow.Name;
            WhenRule whenRule3 = whenRule2.AddRule<WhenRule>(rule3);
            CopyRule rule4 = new CopyRule();
            rule4.Value = row.Name;
            whenRule3.AddRule<CopyRule>(rule4);
          }
        }
      }
      IEnumerable<Guid?> deletedIds = oldBoardSettingRows.Select<BoardRow, Guid?>((Func<BoardRow, Guid?>) (r => r.Id)).Except<Guid?>(newBoardSettingRows.Select<BoardRow, Guid?>((Func<BoardRow, Guid?>) (r => r.Id)));
      IEnumerable<BoardRow> source = oldBoardSettingRows.Where<BoardRow>((Func<BoardRow, bool>) (r => deletedIds.Contains<Guid?>(r.Id)));
      if (source.Any<BoardRow>())
      {
        WorkItemFieldRule workItemFieldRule = laneReconcileRule;
        WhenRule rule5 = new WhenRule();
        rule5.Inverse = true;
        rule5.Field = "[Kanban.Column]";
        rule5.Value = oldIncomingColumnName;
        WhenRule whenRule4 = workItemFieldRule.AddRule<WhenRule>(rule5);
        WhenRule rule6 = new WhenRule();
        rule6.Inverse = true;
        rule6.Field = "[Kanban.Column]";
        rule6.Value = oldOutgoingColumnName;
        WhenRule whenRule5 = whenRule4.AddRule<WhenRule>(rule6);
        foreach (BoardRow boardRow in source)
        {
          WhenRule whenRule6 = whenRule5;
          WhenWasRule whenWasRule1 = new WhenWasRule();
          whenWasRule1.Field = "[Kanban.Lane]";
          whenWasRule1.Value = boardRow.Name;
          WhenWasRule whenWasRule2 = whenWasRule1;
          CopyRule[] copyRuleArray = new CopyRule[1];
          CopyRule copyRule = new CopyRule();
          copyRule.Value = this.GetDefaultRowName(newBoardSettingRows);
          copyRuleArray[0] = copyRule;
          WorkItemRule[] workItemRuleArray = (WorkItemRule[]) copyRuleArray;
          whenWasRule2.SubRules = workItemRuleArray;
          WhenWasRule rule7 = whenWasRule1;
          whenRule6.AddRule<WhenWasRule>(rule7);
        }
      }
      return laneReconcileRule;
    }

    private WorkItemFieldRule CreateKanbanDoneFieldRule()
    {
      WorkItemFieldRule kanbanDoneFieldRule = new WorkItemFieldRule();
      kanbanDoneFieldRule.Field = "[Kanban.Column.Done]";
      WorkItemFieldRule workItemFieldRule = kanbanDoneFieldRule;
      WhenChangedRule[] whenChangedRuleArray1 = new WhenChangedRule[1];
      WhenChangedRule whenChangedRule1 = new WhenChangedRule();
      whenChangedRule1.Field = "[Kanban.Column]";
      WhenChangedRule whenChangedRule2 = whenChangedRule1;
      WhenChangedRule[] whenChangedRuleArray2 = new WhenChangedRule[1];
      WhenChangedRule whenChangedRule3 = new WhenChangedRule();
      whenChangedRule3.Field = "[Kanban.Column.Done]";
      whenChangedRule3.Inverse = true;
      WhenChangedRule whenChangedRule4 = whenChangedRule3;
      CopyRule[] copyRuleArray = new CopyRule[1];
      CopyRule copyRule = new CopyRule();
      copyRule.ValueFrom = RuleValueFrom.Value;
      copyRule.Value = "false";
      copyRuleArray[0] = copyRule;
      WorkItemRule[] workItemRuleArray1 = (WorkItemRule[]) copyRuleArray;
      whenChangedRule4.SubRules = workItemRuleArray1;
      whenChangedRuleArray2[0] = whenChangedRule3;
      WorkItemRule[] workItemRuleArray2 = (WorkItemRule[]) whenChangedRuleArray2;
      whenChangedRule2.SubRules = workItemRuleArray2;
      whenChangedRuleArray1[0] = whenChangedRule1;
      WorkItemRule[] workItemRuleArray3 = (WorkItemRule[]) whenChangedRuleArray1;
      workItemFieldRule.SubRules = workItemRuleArray3;
      return kanbanDoneFieldRule;
    }

    public WorkItemFieldRule CreateKanbanLaneFieldRule(
      string incomingColumnName,
      string outgoingColumnName,
      string defaultLaneName,
      string[] rowNames)
    {
      WorkItemFieldRule workItemFieldRule1 = new WorkItemFieldRule();
      workItemFieldRule1.Field = "[Kanban.Lane]";
      WorkItemFieldRule workItemFieldRule2 = workItemFieldRule1;
      WorkItemRule[] workItemRuleArray1 = new WorkItemRule[2];
      WhenRule whenRule1 = new WhenRule();
      whenRule1.Field = "[Kanban.Column]";
      whenRule1.Value = incomingColumnName;
      WhenRule whenRule2 = whenRule1;
      CopyRule[] copyRuleArray1 = new CopyRule[1];
      CopyRule copyRule1 = new CopyRule();
      copyRule1.ValueFrom = RuleValueFrom.Value;
      copyRule1.Value = (string) null;
      copyRuleArray1[0] = copyRule1;
      WorkItemRule[] workItemRuleArray2 = (WorkItemRule[]) copyRuleArray1;
      whenRule2.SubRules = workItemRuleArray2;
      workItemRuleArray1[0] = (WorkItemRule) whenRule1;
      WhenChangedRule whenChangedRule1 = new WhenChangedRule();
      whenChangedRule1.Inverse = true;
      whenChangedRule1.Field = "[Kanban.Lane]";
      WhenChangedRule whenChangedRule2 = whenChangedRule1;
      WhenWasRule[] whenWasRuleArray = new WhenWasRule[1];
      WhenWasRule whenWasRule1 = new WhenWasRule();
      whenWasRule1.Field = "[Kanban.Column]";
      whenWasRule1.Value = incomingColumnName;
      WhenWasRule whenWasRule2 = whenWasRule1;
      WhenRule[] whenRuleArray1 = new WhenRule[1];
      WhenRule whenRule3 = new WhenRule();
      whenRule3.Inverse = true;
      whenRule3.Field = "[Kanban.Column]";
      whenRule3.Value = outgoingColumnName;
      WhenRule whenRule4 = whenRule3;
      CopyRule[] copyRuleArray2 = new CopyRule[1];
      CopyRule copyRule2 = new CopyRule();
      copyRule2.Value = defaultLaneName;
      copyRuleArray2[0] = copyRule2;
      WorkItemRule[] workItemRuleArray3 = (WorkItemRule[]) copyRuleArray2;
      whenRule4.SubRules = workItemRuleArray3;
      whenRuleArray1[0] = whenRule3;
      WorkItemRule[] workItemRuleArray4 = (WorkItemRule[]) whenRuleArray1;
      whenWasRule2.SubRules = workItemRuleArray4;
      whenWasRuleArray[0] = whenWasRule1;
      WorkItemRule[] workItemRuleArray5 = (WorkItemRule[]) whenWasRuleArray;
      whenChangedRule2.SubRules = workItemRuleArray5;
      workItemRuleArray1[1] = (WorkItemRule) whenChangedRule1;
      workItemFieldRule2.SubRules = workItemRuleArray1;
      WorkItemFieldRule kanbanLaneFieldRule1 = workItemFieldRule1;
      if (rowNames != null && ((IEnumerable<string>) rowNames).Any<string>())
      {
        WorkItemFieldRule workItemFieldRule3 = kanbanLaneFieldRule1;
        WhenRule whenRule5 = new WhenRule();
        whenRule5.Inverse = true;
        whenRule5.Field = "[Kanban.Column]";
        whenRule5.Value = outgoingColumnName;
        WhenRule whenRule6 = whenRule5;
        WhenRule[] whenRuleArray2 = new WhenRule[1];
        WhenRule whenRule7 = new WhenRule();
        whenRule7.Inverse = true;
        whenRule7.Field = "[Kanban.Column]";
        whenRule7.Value = incomingColumnName;
        WhenRule whenRule8 = whenRule7;
        IEnumerable<WorkItemRule> first = ((IEnumerable<string>) rowNames).Select<string, WorkItemRule>((Func<string, WorkItemRule>) (rowName =>
        {
          WhenRule kanbanLaneFieldRule2 = new WhenRule();
          kanbanLaneFieldRule2.Field = "[Kanban.Lane]";
          kanbanLaneFieldRule2.Value = rowName;
          WhenRule whenRule9 = kanbanLaneFieldRule2;
          CopyRule[] copyRuleArray3 = new CopyRule[1]
          {
            new CopyRule()
            {
              ValueFrom = RuleValueFrom.Value,
              Value = rowName
            }
          };
          WorkItemRule[] workItemRuleArray6 = (WorkItemRule[]) copyRuleArray3;
          whenRule9.SubRules = workItemRuleArray6;
          return (WorkItemRule) kanbanLaneFieldRule2;
        }));
        CopyRule[] second = new CopyRule[1];
        CopyRule copyRule3 = new CopyRule();
        copyRule3.ValueFrom = RuleValueFrom.Value;
        copyRule3.Value = defaultLaneName;
        second[0] = copyRule3;
        WorkItemRule[] array = first.Concat<WorkItemRule>((IEnumerable<WorkItemRule>) second).ToArray<WorkItemRule>();
        whenRule8.SubRules = array;
        whenRuleArray2[0] = whenRule7;
        WorkItemRule[] workItemRuleArray7 = (WorkItemRule[]) whenRuleArray2;
        whenRule6.SubRules = workItemRuleArray7;
        WhenRule rule = whenRule5;
        workItemFieldRule3.AddRule<WhenRule>(rule);
      }
      return kanbanLaneFieldRule1;
    }

    private BoardSettings CreateBoardSettingsModel(
      IVssRequestContext requestContext,
      Guid projectId,
      WebApiTeam team,
      BacklogLevelConfiguration backlogLevel)
    {
      requestContext.Trace(240303, TraceLevel.Verbose, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Creating BoardSettings for team: {0}, backlog: {1}", (object) team.Name, (object) backlogLevel.Id);
      IEnumerable<BoardColumn> defaultBoardColumns = this.GetDefaultBoardColumns(requestContext, projectId, backlogLevel);
      requestContext.Trace(240304, TraceLevel.Verbose, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Successfully created BoardSettings for team: {0}, backlog: {1}", (object) team.Name, (object) backlogLevel.Id);
      return new BoardSettings()
      {
        TeamId = team.Id,
        Columns = (IEnumerable<BoardColumn>) defaultBoardColumns.ToList<BoardColumn>()
      };
    }

    internal IEnumerable<BoardColumn> GetDefaultBoardColumns(
      IVssRequestContext requestContext,
      Guid projectId,
      BacklogLevelConfiguration backlogLevel)
    {
      string[] array = requestContext.GetService<IBacklogConfigurationService>().GetProjectBacklogConfiguration(requestContext, projectId).GetWorkItemStatesOrderedByCategory(backlogLevel, false).ToArray<string>();
      int count = array.Length;
      return ((IEnumerable<string>) array).Select<string, BoardColumn>((Func<string, int, BoardColumn>) ((state, i) =>
      {
        string str = state;
        BoardColumnType boardColumnType = BoardColumnType.InProgress;
        if (i == 0)
          boardColumnType = BoardColumnType.Incoming;
        else if (i == count - 1)
          boardColumnType = BoardColumnType.Outgoing;
        int num = boardColumnType == BoardColumnType.InProgress ? 5 : 0;
        return new BoardColumn()
        {
          Name = str,
          ColumnType = boardColumnType,
          ItemLimit = num,
          Order = i
        };
      }));
    }

    internal IReadOnlyCollection<string> GetEffectiveBacklogWorkItemTypes(
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfig,
      BacklogLevelConfiguration backlogLevel,
      bool includeBugTypes)
    {
      HashSet<string> backlogWorkItemTypes = new HashSet<string>((IEnumerable<string>) backlogLevel.WorkItemTypes, (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      if (includeBugTypes && backlogLevel.IsRequirementsBacklog && backlogConfig.IsBugsBehaviorConfigValid)
        backlogWorkItemTypes.UnionWith((IEnumerable<string>) backlogConfig.BugWorkItemTypes);
      return (IReadOnlyCollection<string>) backlogWorkItemTypes;
    }

    internal bool BugsOnBacklogFeatureAvailable(
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfig,
      BacklogLevelConfiguration backlogLevel)
    {
      return backlogConfig.IsBugsBehaviorConfigValid && backlogLevel.IsRequirementsBacklog;
    }
  }
}
