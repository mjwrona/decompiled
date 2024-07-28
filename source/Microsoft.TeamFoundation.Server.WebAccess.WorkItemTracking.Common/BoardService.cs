// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Events;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class BoardService : IVssFrameworkService
  {
    private readonly Guid CollectionKey = Guid.Empty;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      TeamFoundationSqlNotificationService notificationService = requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? requestContext.GetService<TeamFoundationSqlNotificationService>() : throw new InvalidOperationException(FrameworkResources.UnexpectedHostType((object) requestContext.ServiceHost.HostType.ToString()));
      notificationService.RegisterNotification(requestContext, "Default", SqlNotificationEventClasses.TeamBoardCardSettingsChanged, new SqlNotificationCallback(this.OnBoardCardSettingsChanged), false);
      notificationService.RegisterNotification(requestContext, "Default", SqlNotificationEventClasses.ProjectBoardColumnsChanged, new SqlNotificationCallback(this.OnProjectBoardColumnsChanged), false);
      notificationService.RegisterNotification(requestContext, "Default", SqlNotificationEventClasses.ProjectBoardRowsChanged, new SqlNotificationCallback(this.OnProjectBoardRowsChanged), false);
      notificationService.RegisterNotification(requestContext, "Default", SqlNotificationEventClasses.TeamBoardSettingsChanged, new SqlNotificationCallback(this.OnBoardSettingsChanged), false);
      notificationService.RegisterNotification(requestContext, "Default", SqlNotificationEventClasses.BoardsDeleted, new SqlNotificationCallback(this.OnBoardsDeleted), false);
      notificationService.RegisterNotification(requestContext, "Default", SqlNotificationEventClasses.ProjectBoardOptionsChanged, new SqlNotificationCallback(this.OnProjectBoardOptionsChanged), false);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      TeamFoundationSqlNotificationService service = requestContext.GetService<TeamFoundationSqlNotificationService>();
      service.UnregisterNotification(requestContext, "Default", SqlNotificationEventClasses.TeamBoardCardSettingsChanged, new SqlNotificationCallback(this.OnBoardCardSettingsChanged), false);
      service.UnregisterNotification(requestContext, "Default", SqlNotificationEventClasses.ProjectBoardColumnsChanged, new SqlNotificationCallback(this.OnProjectBoardColumnsChanged), false);
      service.UnregisterNotification(requestContext, "Default", SqlNotificationEventClasses.ProjectBoardRowsChanged, new SqlNotificationCallback(this.OnProjectBoardRowsChanged), false);
      service.UnregisterNotification(requestContext, "Default", SqlNotificationEventClasses.TeamBoardSettingsChanged, new SqlNotificationCallback(this.OnBoardSettingsChanged), false);
      service.UnregisterNotification(requestContext, "Default", SqlNotificationEventClasses.BoardsDeleted, new SqlNotificationCallback(this.OnBoardsDeleted), false);
      service.UnregisterNotification(requestContext, "Default", SqlNotificationEventClasses.ProjectBoardOptionsChanged, new SqlNotificationCallback(this.OnProjectBoardOptionsChanged), false);
    }

    public BoardSettings CreateBoard(
      IVssRequestContext requestContext,
      Guid projectId,
      BoardSettings boardSettings)
    {
      requestContext.Trace(240306, TraceLevel.Verbose, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Creating board for team with ID: {0}, backlog: {1}", (object) boardSettings.TeamId, (object) boardSettings.BacklogLevelId);
      try
      {
        BoardSettings board = (BoardSettings) null;
        BoardUserSettings boardUserSettings = (BoardUserSettings) null;
        using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
          board = BoardSettingsDTOConverter.BoardSettingsFromDTO(component.CreateBoard(projectId, boardSettings));
        if (board != null)
        {
          if (!requestContext.IsSystemContext)
          {
            WebApiTeam team = this.GetTeam(requestContext, projectId, boardSettings.TeamId);
            boardUserSettings = BoardUserSettingsUtils.Instance.GetBoardUserSettings(requestContext, projectId, team, board.Id.Value);
          }
          board.AutoRefreshState = boardUserSettings == null || boardUserSettings.AutoBoardRefreshState;
        }
        requestContext.Trace(240307, TraceLevel.Verbose, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, "Successfully created board for team with ID: {0}, backlog category: {1}", (object) boardSettings.TeamId, (object) boardSettings.BacklogLevelId);
        this.UpdateBoardIdMap(requestContext, boardSettings.Id.Value, boardSettings.TeamId, boardSettings.BacklogLevelId);
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        Guid? nullable = boardSettings.Id;
        Guid boardId = nullable.Value;
        nullable = new Guid?();
        Guid? workItemTypeExtensionId = nullable;
        this.PublishTeamBoardSettingsChangedEvent(requestContext1, projectId1, boardId, KanbanBoardChangeType.Create, workItemTypeExtensionId);
        return board;
      }
      catch (BoardExistsException ex)
      {
        requestContext.TraceException(240305, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, (Exception) ex);
        throw;
      }
    }

    public virtual BoardSettings GetBoard(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId,
      string backlogLevelId,
      bool bypassCache = false)
    {
      using (requestContext.TraceBlock(290068, 290069, "Agile", TfsTraceLayers.BusinessLogic, "BoardService.GetBoard"))
      {
        BoardSettings board = (BoardSettings) null;
        BoardSettingsDTO dto = (BoardSettingsDTO) null;
        BoardSettingsCacheService service = requestContext.GetService<BoardSettingsCacheService>();
        string settingsCacheKey = this.GetBoardSettingsCacheKey(new BoardInput()
        {
          TeamId = teamId,
          BacklogLevelId = backlogLevelId
        });
        if (bypassCache || !service.TryGetValue(requestContext, settingsCacheKey, out dto))
        {
          using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
          {
            dto = component.GetBoard(projectId, teamId, backlogLevelId);
            if (dto != null)
              service.Set(requestContext, settingsCacheKey, dto);
          }
        }
        if (dto != null)
        {
          board = BoardSettingsDTOConverter.BoardSettingsFromDTO(dto);
          this.UpdateBoardIdMap(requestContext, dto.Id, dto.TeamId, dto.BacklogLevelId);
        }
        return board;
      }
    }

    public virtual BoardSettings RestoreBoard(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId,
      string backlogLevelId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(teamId, nameof (teamId));
      ArgumentUtility.CheckStringForNullOrEmpty(backlogLevelId, nameof (backlogLevelId));
      using (requestContext.TraceBlock(240315, 240316, "Agile", TfsTraceLayers.BusinessLogic, "BoardService.RestoreBoard"))
      {
        BoardSettings boardSettings = (BoardSettings) null;
        BoardSettingsDTO dto = (BoardSettingsDTO) null;
        using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
          dto = component.RestoreBoard(projectId, teamId, backlogLevelId);
        if (dto != null)
        {
          BoardSettingsCacheService service = requestContext.GetService<BoardSettingsCacheService>();
          string settingsCacheKey = this.GetBoardSettingsCacheKey(new BoardInput()
          {
            TeamId = teamId,
            BacklogLevelId = backlogLevelId
          });
          IVssRequestContext requestContext1 = requestContext;
          string key = settingsCacheKey;
          BoardSettingsDTO boardSettingsDto = dto;
          service.Set(requestContext1, key, boardSettingsDto);
          boardSettings = BoardSettingsDTOConverter.BoardSettingsFromDTO(dto);
          this.UpdateBoardIdMap(requestContext, dto.Id, dto.TeamId, dto.BacklogLevelId);
          this.PublishTeamBoardSettingsChangedEvent(requestContext, projectId, dto.Id, KanbanBoardChangeType.Restore);
        }
        return boardSettings;
      }
    }

    public BoardCardSettings GetBoardCardSettings(
      IVssRequestContext requestContext,
      Guid projectId,
      BoardCardSettings.ScopeType boardScope,
      Guid boardScopeId)
    {
      using (requestContext.TraceBlock(290103, 290104, "Agile", TfsTraceLayers.BusinessLogic, "BoardService.GetBoardCardSettings"))
      {
        BoardCardSettings boardCardSettings = (BoardCardSettings) null;
        BoardCardSettingsCacheService service = requestContext.GetService<BoardCardSettingsCacheService>();
        if (service.TryGetValue(requestContext, boardScopeId, out boardCardSettings))
          return boardCardSettings;
        using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
          boardCardSettings = BoardSettingsDTOConverter.BoardCardSettingsFromDTO(boardScope, boardScopeId, component.GetBoardCardSettings(projectId, boardScope, boardScopeId));
        service.Set(requestContext, boardScopeId, boardCardSettings);
        return boardCardSettings;
      }
    }

    public virtual List<CardRule> GetBoardCardRules(
      IVssRequestContext requestContext,
      Guid projectId,
      BoardCardSettings.ScopeType boardScope,
      Guid boardScopeId)
    {
      using (requestContext.TraceBlock(290103, 290104, "Agile", TfsTraceLayers.BusinessLogic, "BoardService.GetBoardCardRules"))
      {
        List<CardRule> boardCardRules = (List<CardRule>) null;
        using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
          boardCardRules = BoardSettingsDTOConverter.BoardCardRulesFromDTO(component.GetBoardCardRules(projectId, boardScope, boardScopeId)).ToList<CardRule>();
        return boardCardRules;
      }
    }

    public void UpdateBoardCardRules(
      IVssRequestContext requestContext,
      Guid projectId,
      BoardCardRules boardCardRules,
      List<string> typesToBeDeleted)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<BoardCardRules>(boardCardRules, nameof (boardCardRules));
      using (requestContext.TraceBlock(290235, 290236, "Agile", TfsTraceLayers.BusinessLogic, "BoardService.UpdateBoardCardRules"))
      {
        using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
          component.UpdateCardRules(projectId, boardCardRules.Scope, boardCardRules.ScopeId, (IEnumerable<BoardCardRuleRow>) boardCardRules.Rules, (IEnumerable<RuleAttributeRow>) boardCardRules.Attributes, typesToBeDeleted);
        if (StringComparer.OrdinalIgnoreCase.Equals(boardCardRules.Scope, "KANBAN"))
        {
          this.PublishTeamBoardSettingsChangedEvent(requestContext, projectId, boardCardRules.ScopeId, KanbanBoardChangeType.UpdateCardRules);
          this.InvalidateBoardCache(requestContext, projectId, new Guid?(boardCardRules.ScopeId));
        }
        if (!StringComparer.OrdinalIgnoreCase.Equals(boardCardRules.Scope, "TASKBOARD"))
          return;
        this.PublishTaskBoardCardSettingsChangedEvent(requestContext, projectId, boardCardRules.ScopeId);
      }
    }

    public virtual IEnumerable<BoardRecord> GetTeamBoards(
      IVssRequestContext requestContext,
      Guid teamId)
    {
      using (requestContext.TraceBlock(290095, 290096, "Agile", TfsTraceLayers.BusinessLogic, "BoardService.GetTeamBoards"))
      {
        IList<BoardRecord> teamBoards;
        using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
          teamBoards = (IList<BoardRecord>) component.GetTeamBoards((IEnumerable<Guid>) new Guid[1]
          {
            teamId
          });
        return (IEnumerable<BoardRecord>) teamBoards;
      }
    }

    public void UpdateBoard(
      IVssRequestContext requestContext,
      Guid projectId,
      BoardSettings boardSettings)
    {
      using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
        component.UpdateBoard(projectId, boardSettings);
    }

    public void UpdateBoardExtension(
      IVssRequestContext requestContext,
      Guid projectId,
      BoardSettings board,
      Guid extensionId)
    {
      board.ExtensionId = new Guid?(extensionId);
      using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
        component.UpdateBoardExtension(projectId, board.Id.Value, extensionId);
      requestContext.GetService<BoardSettingsCacheService>().Remove(requestContext, this.GetBoardSettingsCacheKey(new BoardInput()
      {
        TeamId = board.TeamId,
        BacklogLevelId = board.BacklogLevelId
      }));
    }

    public void UpdateBoardColumns(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid boardId,
      IEnumerable<BoardColumn> columns)
    {
      using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
        component.UpdateBoardColumns(projectId, boardId, columns);
      this.PublishTeamBoardSettingsChangedEvent(requestContext, projectId, boardId, KanbanBoardChangeType.UpdateColumns);
      this.InvalidateBoardCache(requestContext, projectId, new Guid?(boardId));
    }

    public void UpdateBoardRows(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid boardId,
      IEnumerable<BoardRow> rows)
    {
      using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
        component.UpdateBoardRows(projectId, boardId, rows);
      this.PublishTeamBoardSettingsChangedEvent(requestContext, projectId, boardId, KanbanBoardChangeType.UpdateRows);
      this.InvalidateBoardCache(requestContext, projectId, new Guid?(boardId));
    }

    public void UpdateBoardCardSettings(
      IVssRequestContext requestContext,
      Guid projectId,
      BoardCardSettings boardCardSettings)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<BoardCardSettings>(boardCardSettings, nameof (boardCardSettings));
      ArgumentUtility.CheckForEmptyGuid(boardCardSettings.ScopeId, "BoardScopeId");
      using (requestContext.TraceBlock(290105, 290106, "Agile", TfsTraceLayers.BusinessLogic, "BoardService.UpdateBoardCardSettings"))
      {
        BoardCardSettingsCacheService service = requestContext.GetService<BoardCardSettingsCacheService>();
        service.Remove(requestContext, boardCardSettings.ScopeId);
        using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
          component.SetBoardCardSettings(projectId, boardCardSettings);
        if (boardCardSettings.Scope == BoardCardSettings.ScopeType.KANBAN)
        {
          this.PublishTeamBoardSettingsChangedEvent(requestContext, projectId, boardCardSettings.ScopeId, KanbanBoardChangeType.UpdateCardSettings);
          this.InvalidateBoardCache(requestContext, projectId, new Guid?(boardCardSettings.ScopeId));
        }
        if (boardCardSettings.Scope == BoardCardSettings.ScopeType.TASKBOARD)
          this.PublishTaskBoardCardSettingsChangedEvent(requestContext, projectId, boardCardSettings.ScopeId);
        service.Set(requestContext, boardCardSettings.ScopeId, boardCardSettings);
      }
    }

    public virtual IList<BoardColumnRevisionForReporting> GetBoardColumnRevisions(
      IVssRequestContext requestContext,
      int watermark)
    {
      using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
        return component.GetBoardColumnRevisions(watermark);
    }

    public virtual IEnumerable<BoardColumnRevision> GetBoardColumnRevisions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId,
      string backlogLevelId)
    {
      using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
        return component.GetBoardRevisions(projectId, teamId, backlogLevelId);
    }

    public virtual IList<BoardRowRevisionForReporting> GetBoardRowRevisions(
      IVssRequestContext requestContext,
      int watermark)
    {
      using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
        return component.GetBoardRowRevisions(watermark);
    }

    public virtual IEnumerable<BoardRowRevision> GetBoardRowRevisions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId,
      string backlogLevelId)
    {
      using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
        return component.GetBoardRowRevisions(projectId, teamId, backlogLevelId);
    }

    public void DeleteProjectBoards(IVssRequestContext requestContext, string projectUri)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(projectUri, nameof (projectUri));
      try
      {
        requestContext.TraceEnter(240182, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (DeleteProjectBoards));
        Guid[] array = requestContext.GetService<ITeamService>().QueryTeamsInProject(requestContext, ProjectInfo.GetProjectId(projectUri)).Select<WebApiTeam, Guid>((Func<WebApiTeam, Guid>) (team => team.Id)).ToArray<Guid>();
        this.DeleteTeamBoards(requestContext, (IEnumerable<Guid>) array);
      }
      finally
      {
        requestContext.TraceLeave(240183, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (DeleteProjectBoards));
      }
    }

    public void DeleteTeamBoards(IVssRequestContext requestContext, IEnumerable<Guid> teamIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(teamIds, nameof (teamIds));
      try
      {
        requestContext.TraceEnter(240310, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (DeleteTeamBoards));
        if (!teamIds.Any<Guid>())
          return;
        List<BoardRecord> teamBoards;
        using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
          teamBoards = component.GetTeamBoards(teamIds);
        if (!teamBoards.Any<BoardRecord>())
          return;
        this.DeleteBoardsInternal(requestContext, (IEnumerable<BoardRecord>) teamBoards, (IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "teamCount",
            (object) teamIds.Count<Guid>()
          }
        });
      }
      finally
      {
        requestContext.TraceLeave(240311, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (DeleteTeamBoards));
      }
    }

    public void DeleteBoardsByIds(IVssRequestContext requestContext, IEnumerable<Guid> boardIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(boardIds, nameof (boardIds));
      try
      {
        requestContext.TraceEnter(240308, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (DeleteBoardsByIds));
        if (!boardIds.Any<Guid>())
          return;
        using (new PerformanceScenarioHelper(requestContext, nameof (BoardService), nameof (DeleteBoardsByIds)))
        {
          IEnumerable<BoardRecord> boardsByIds;
          using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
            boardsByIds = component.GetBoardsByIds(boardIds);
          if (!boardsByIds.Any<BoardRecord>())
            return;
          this.DeleteBoardsInternal(requestContext, boardsByIds);
        }
      }
      finally
      {
        requestContext.TraceLeave(240309, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (DeleteBoardsByIds));
      }
    }

    public virtual IDictionary<Guid, string> DeleteBoardsByCategoryReferenceNames(
      IVssRequestContext requestContext,
      IReadOnlyCollection<string> categoryReferenceNames)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IReadOnlyCollection<string>>(categoryReferenceNames, nameof (categoryReferenceNames));
      IReadOnlyCollection<Guid> boardIdsToDelete = (IReadOnlyCollection<Guid>) null;
      IDictionary<Guid, string> dictionary = (IDictionary<Guid, string>) null;
      using (requestContext.TraceBlock(240317, 240318, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (DeleteBoardsByCategoryReferenceNames)))
      {
        if (categoryReferenceNames.Any<string>())
        {
          using (new PerformanceScenarioHelper(requestContext, nameof (BoardService), nameof (DeleteBoardsByCategoryReferenceNames)))
          {
            IEnumerable<BoardRecord> categoryReferenceNames1;
            using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
              categoryReferenceNames1 = component.GetBoardsByCategoryReferenceNames((IEnumerable<string>) categoryReferenceNames);
            if (categoryReferenceNames1.Any<BoardRecord>())
            {
              boardIdsToDelete = this.DeleteBoardsInternal(requestContext, categoryReferenceNames1);
              dictionary = (IDictionary<Guid, string>) categoryReferenceNames1.Where<BoardRecord>((Func<BoardRecord, bool>) (boardRecord => boardIdsToDelete.Contains<Guid>(boardRecord.Id))).ToDictionary<BoardRecord, Guid, string>((Func<BoardRecord, Guid>) (k => k.Id), (Func<BoardRecord, string>) (v => v.BacklogLevelId));
            }
          }
        }
      }
      return dictionary ?? (IDictionary<Guid, string>) new Dictionary<Guid, string>();
    }

    public void SoftDeleteBoardsByIds(IVssRequestContext requestContext, IEnumerable<Guid> boardIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(boardIds, nameof (boardIds));
      try
      {
        requestContext.TraceEnter(240313, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (SoftDeleteBoardsByIds));
        if (!boardIds.Any<Guid>())
          return;
        using (PerformanceScenarioHelper performanceScenarioHelper = new PerformanceScenarioHelper(requestContext, nameof (BoardService), nameof (SoftDeleteBoardsByIds)))
        {
          IEnumerable<BoardRecord> boardsByIds;
          using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
            boardsByIds = component.GetBoardsByIds(boardIds);
          if (!boardsByIds.Any<BoardRecord>())
            return;
          List<Guid> list = boardsByIds.Select<BoardRecord, Guid>((Func<BoardRecord, Guid>) (b => b.Id)).ToList<Guid>();
          using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
            component.SoftDeleteBoards((IEnumerable<Guid>) list);
          performanceScenarioHelper.Add("boardCount", (object) list.Count);
          this.PublishTeamBoardSettingsChangedEvent(requestContext, Guid.Empty, boardsByIds, KanbanBoardChangeType.SoftDelete);
        }
      }
      finally
      {
        requestContext.TraceLeave(240314, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (SoftDeleteBoardsByIds));
      }
    }

    public void DeleteBoards(IVssRequestContext requestContext, IEnumerable<BoardSettings> boards)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<BoardSettings>>(boards, nameof (boards));
      try
      {
        requestContext.TraceEnter(240308, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (DeleteBoards));
        if (!boards.Any<BoardSettings>())
          return;
        Guid[] array = boards.Where<BoardSettings>((Func<BoardSettings, bool>) (b => b.ExtensionId.HasValue)).Select<BoardSettings, Guid>((Func<BoardSettings, Guid>) (b => b.ExtensionId.GetValueOrDefault())).ToArray<Guid>();
        requestContext.GetService<IWorkItemTypeExtensionService>().DeleteExtensions(requestContext, (IEnumerable<Guid>) array);
        IEnumerable<Guid> boardIds = boards.Where<BoardSettings>((Func<BoardSettings, bool>) (b => b.Id.HasValue)).Select<BoardSettings, Guid>((Func<BoardSettings, Guid>) (b => b.Id.GetValueOrDefault()));
        using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
          component.DeleteBoards(boardIds);
        this.PublishTeamBoardSettingsChangedEvent(requestContext, Guid.Empty, boards, KanbanBoardChangeType.Delete);
      }
      finally
      {
        requestContext.TraceLeave(240309, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, nameof (DeleteBoards));
      }
    }

    public IEnumerable<string> GetColumnSuggestedValues(
      IVssRequestContext requestContext,
      Guid? projectId = null)
    {
      BoardColumnSuggestedValueService service = requestContext.GetService<BoardColumnSuggestedValueService>();
      SortedSet<string> source = (SortedSet<string>) null;
      if (projectId.HasValue)
      {
        if (!service.TryGetValue(requestContext, projectId.Value, out source))
        {
          using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
          {
            Dictionary<Guid, SortedSet<string>> columnSuggestedValues = component.GetBoardColumnSuggestedValues(projectId);
            if (columnSuggestedValues.Any<KeyValuePair<Guid, SortedSet<string>>>())
            {
              source = columnSuggestedValues[projectId.Value];
              service.Set(requestContext, projectId.Value, source);
            }
            else
              source = new SortedSet<string>();
          }
        }
      }
      else if (!service.TryGetValue(requestContext, this.CollectionKey, out source))
      {
        using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
        {
          Dictionary<Guid, SortedSet<string>> columnSuggestedValues = component.GetBoardColumnSuggestedValues();
          if (columnSuggestedValues.Any<KeyValuePair<Guid, SortedSet<string>>>())
          {
            source = new SortedSet<string>(columnSuggestedValues.SelectMany<KeyValuePair<Guid, SortedSet<string>>, string>((Func<KeyValuePair<Guid, SortedSet<string>>, IEnumerable<string>>) (kvp => (IEnumerable<string>) kvp.Value)));
            service.Set(requestContext, this.CollectionKey, source);
            foreach (Guid key in columnSuggestedValues.Keys)
              service.Set(requestContext, key, columnSuggestedValues[key]);
          }
          else
            source = new SortedSet<string>();
        }
      }
      return (IEnumerable<string>) source.ToList<string>();
    }

    public IEnumerable<string> GetRowSuggestedValues(
      IVssRequestContext requestContext,
      Guid? projectId = null)
    {
      BoardRowSuggestedValueService service = requestContext.GetService<BoardRowSuggestedValueService>();
      SortedSet<string> source = (SortedSet<string>) null;
      if (projectId.HasValue)
      {
        if (!service.TryGetValue(requestContext, projectId.Value, out source))
        {
          using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
          {
            Dictionary<Guid, SortedSet<string>> rowSuggestedValues = component.GetBoardRowSuggestedValues(projectId);
            if (rowSuggestedValues.Any<KeyValuePair<Guid, SortedSet<string>>>())
            {
              source = rowSuggestedValues[projectId.Value];
              service.Set(requestContext, projectId.Value, source);
            }
            else
              source = new SortedSet<string>();
          }
        }
      }
      else if (!service.TryGetValue(requestContext, this.CollectionKey, out source))
      {
        using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
        {
          Dictionary<Guid, SortedSet<string>> rowSuggestedValues = component.GetBoardRowSuggestedValues();
          if (rowSuggestedValues.Any<KeyValuePair<Guid, SortedSet<string>>>())
          {
            source = new SortedSet<string>(rowSuggestedValues.SelectMany<KeyValuePair<Guid, SortedSet<string>>, string>((Func<KeyValuePair<Guid, SortedSet<string>>, IEnumerable<string>>) (kvp => (IEnumerable<string>) kvp.Value)));
            service.Set(requestContext, this.CollectionKey, source);
            foreach (Guid key in rowSuggestedValues.Keys)
              service.Set(requestContext, key, rowSuggestedValues[key]);
          }
          else
            source = new SortedSet<string>();
        }
      }
      return (IEnumerable<string>) source.ToList<string>();
    }

    public Dictionary<string, string> GetBoardOptions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid boardId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(boardId, nameof (boardId));
      using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
        return component.GetBoardOptions(projectId, boardId);
    }

    public BoardOptionRecord GetBoardOptionsRecord(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid boardId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(boardId, nameof (boardId));
      using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
        return component.GetBoardOptionsRecord(projectId, boardId);
    }

    public void UpdateBoardOptions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid boardId,
      int cardReordering,
      bool statusBadgeIsPublic = false,
      bool suppressChangeEvent = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(boardId, nameof (boardId));
      using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
        component.UpdateBoardOptions(projectId, boardId, cardReordering, statusBadgeIsPublic);
      if (!suppressChangeEvent)
        this.PublishTeamBoardSettingsChangedEvent(requestContext, projectId, boardId, KanbanBoardChangeType.UpdateCardReorderingOptions);
      this.InvalidateBoardCache(requestContext, projectId, new Guid?(boardId));
    }

    public virtual IEnumerable<BoardRecord> GetAllBoards(
      IVssRequestContext requestContext,
      Guid? projectId = null,
      Guid? teamId = null)
    {
      using (requestContext.TraceBlock(290540, 290541, "Agile", TfsTraceLayers.BusinessLogic, "BoardService.GetAllBoards"))
      {
        IEnumerable<BoardRecord> allBoards;
        using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
          allBoards = component.GetAllBoards(projectId, teamId);
        return allBoards;
      }
    }

    private void OnBoardCardSettingsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      BoardCardSettingsCacheService service = requestContext.GetService<BoardCardSettingsCacheService>();
      Guid result;
      if (string.IsNullOrEmpty(eventData) || !Guid.TryParse(eventData, out result))
        service.Clear(requestContext);
      else
        service.Remove(requestContext, result);
    }

    private void OnProjectBoardColumnsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      Guid result;
      if (string.IsNullOrEmpty(eventData) || !Guid.TryParse(eventData, out result))
        return;
      BoardColumnSuggestedValueService service = requestContext.GetService<BoardColumnSuggestedValueService>();
      service.Remove(requestContext, result);
      service.Remove(requestContext, this.CollectionKey);
    }

    private void OnProjectBoardRowsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      Guid result;
      if (string.IsNullOrEmpty(eventData) || !Guid.TryParse(eventData, out result))
        return;
      BoardRowSuggestedValueService service = requestContext.GetService<BoardRowSuggestedValueService>();
      service.Remove(requestContext, result);
      service.Remove(requestContext, this.CollectionKey);
    }

    private void OnBoardSettingsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (string.IsNullOrEmpty(eventData))
        return;
      string[] strArray = eventData.Split(',');
      Guid result1;
      Guid result2;
      if (strArray.Length != 2 || !Guid.TryParse(strArray[0], out result1) || !Guid.TryParse(strArray[1], out result2))
        return;
      this.InvalidateBoardCache(requestContext, result1, new Guid?(result2));
    }

    private void OnBoardsDeleted(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (string.IsNullOrEmpty(eventData))
        return;
      string str = eventData;
      char[] chArray = new char[1]{ ',' };
      foreach (string input in str.Split(chArray))
      {
        Guid result;
        if (!string.IsNullOrEmpty(input) && Guid.TryParse(input, out result))
        {
          BoardRowSuggestedValueService service1 = requestContext.GetService<BoardRowSuggestedValueService>();
          service1.Remove(requestContext, result);
          service1.Remove(requestContext, this.CollectionKey);
          BoardColumnSuggestedValueService service2 = requestContext.GetService<BoardColumnSuggestedValueService>();
          service2.Remove(requestContext, result);
          service2.Remove(requestContext, this.CollectionKey);
          this.InvalidateBoardCache(requestContext, result);
        }
      }
    }

    private void OnProjectBoardOptionsChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      if (string.IsNullOrEmpty(eventData))
        return;
      requestContext.GetService<BoardSettingsCacheService>().Remove(requestContext, eventData);
    }

    private IReadOnlyCollection<Guid> DeleteBoardsInternal(
      IVssRequestContext requestContext,
      IEnumerable<BoardRecord> boards,
      IDictionary<string, object> perfHelperProperties = null)
    {
      using (PerformanceScenarioHelper helper = new PerformanceScenarioHelper(requestContext, nameof (BoardService), nameof (DeleteBoardsInternal)))
      {
        Guid[] array = boards.Select<BoardRecord, Guid>((Func<BoardRecord, Guid>) (b => b.WorkItemTypeExtensionId)).ToArray<Guid>();
        requestContext.GetService<IWorkItemTypeExtensionService>().DeleteExtensions(requestContext, (IEnumerable<Guid>) array);
        List<Guid> list = boards.Select<BoardRecord, Guid>((Func<BoardRecord, Guid>) (b => b.Id)).Distinct<Guid>().ToList<Guid>();
        using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
          component.DeleteBoards((IEnumerable<Guid>) list);
        helper.Add("extensionCount", (object) array.Length);
        helper.Add("boardCount", (object) list.Count);
        if (perfHelperProperties != null)
          perfHelperProperties.ToList<KeyValuePair<string, object>>().ForEach((Action<KeyValuePair<string, object>>) (kvp => helper.Add(kvp.Key, kvp.Value)));
        this.PublishTeamBoardSettingsChangedEvent(requestContext, Guid.Empty, boards, KanbanBoardChangeType.Delete);
        return (IReadOnlyCollection<Guid>) list;
      }
    }

    private void PublishTeamBoardSettingsChangedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<BoardRecord> boards,
      KanbanBoardChangeType boardChangeType)
    {
      List<TeamBoardSettingsArtifact> list = boards.Select<BoardRecord, TeamBoardSettingsArtifact>((Func<BoardRecord, TeamBoardSettingsArtifact>) (board => new TeamBoardSettingsArtifact()
      {
        BoardId = board.Id,
        ProjectId = projectId,
        BoardChangeType = boardChangeType,
        WorkItemTypeExtensionId = board.WorkItemTypeExtensionId
      })).ToList<TeamBoardSettingsArtifact>();
      this.PublishEventNotification(requestContext, new TeamBoardSettingsChangedEvent()
      {
        TeamBoardSettingsArtifacts = (IEnumerable<TeamBoardSettingsArtifact>) list
      });
    }

    private void PublishTeamBoardSettingsChangedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<BoardSettings> boardSettings,
      KanbanBoardChangeType boardChangeType)
    {
      List<TeamBoardSettingsArtifact> list = boardSettings.Where<BoardSettings>((Func<BoardSettings, bool>) (boardSetting => boardSetting.Id.HasValue)).Select<BoardSettings, TeamBoardSettingsArtifact>((Func<BoardSettings, TeamBoardSettingsArtifact>) (boardSetting => new TeamBoardSettingsArtifact()
      {
        BoardId = boardSetting.Id.GetValueOrDefault(),
        ProjectId = projectId,
        BoardChangeType = boardChangeType,
        WorkItemTypeExtensionId = boardSetting.ExtensionId.GetValueOrDefault()
      })).ToList<TeamBoardSettingsArtifact>();
      this.PublishEventNotification(requestContext, new TeamBoardSettingsChangedEvent()
      {
        TeamBoardSettingsArtifacts = (IEnumerable<TeamBoardSettingsArtifact>) list
      });
    }

    private void PublishTeamBoardSettingsChangedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid boardId,
      KanbanBoardChangeType boardChangeType,
      Guid? workItemTypeExtensionId = null)
    {
      try
      {
        TeamBoardSettingsArtifact settingsArtifact1 = new TeamBoardSettingsArtifact();
        settingsArtifact1.BoardId = boardId;
        settingsArtifact1.ProjectId = projectId;
        settingsArtifact1.BoardChangeType = boardChangeType;
        Guid? nullable = workItemTypeExtensionId;
        settingsArtifact1.WorkItemTypeExtensionId = nullable ?? Guid.Empty;
        TeamBoardSettingsArtifact settingsArtifact2 = settingsArtifact1;
        if (!workItemTypeExtensionId.HasValue)
        {
          BoardInput boardInput = this.GetBoardInput(requestContext, projectId, boardId);
          if (boardInput != null)
          {
            BoardSettings board = this.GetBoard(requestContext, projectId, boardInput.TeamId, boardInput.BacklogLevelId);
            if (board != null)
            {
              nullable = board.ExtensionId;
              if (nullable.HasValue)
              {
                TeamBoardSettingsArtifact settingsArtifact3 = settingsArtifact2;
                nullable = board.ExtensionId;
                Guid guid = nullable.Value;
                settingsArtifact3.WorkItemTypeExtensionId = guid;
              }
            }
          }
        }
        this.PublishEventNotification(requestContext, new TeamBoardSettingsChangedEvent()
        {
          TeamBoardSettingsArtifacts = (IEnumerable<TeamBoardSettingsArtifact>) new List<TeamBoardSettingsArtifact>()
          {
            settingsArtifact2
          }
        });
      }
      catch (Exception ex)
      {
        requestContext.TraceException(290545, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, ex);
      }
    }

    private void PublishTaskBoardCardSettingsChangedEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid teamId)
    {
      try
      {
        TaskboardCardSettingsChangedEvent notificationEvent = new TaskboardCardSettingsChangedEvent()
        {
          TeamId = teamId,
          ProjectId = projectId
        };
        requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) notificationEvent);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(290547, "WebAccess.Settings", TfsTraceLayers.BusinessLogic, ex);
      }
    }

    private void PublishEventNotification(
      IVssRequestContext requestContext,
      TeamBoardSettingsChangedEvent teamBoardSettingsChangedEvent)
    {
      requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) teamBoardSettingsChangedEvent);
      foreach (TeamBoardSettingsArtifact settingsArtifact in teamBoardSettingsChangedEvent.TeamBoardSettingsArtifacts)
      {
        IVssRequestContext requestContext1 = requestContext;
        string businessLogic = TfsTraceLayers.BusinessLogic;
        // ISSUE: variable of a boxed type
        __Boxed<Guid> boardId = (ValueType) settingsArtifact.BoardId;
        string str1 = settingsArtifact.BoardChangeType.ToString();
        Guid itemTypeExtensionId = settingsArtifact.WorkItemTypeExtensionId;
        string str2;
        if (!itemTypeExtensionId.Equals(Guid.Empty))
        {
          itemTypeExtensionId = settingsArtifact.WorkItemTypeExtensionId;
          str2 = itemTypeExtensionId.ToString();
        }
        else
          str2 = "N/A";
        requestContext1.Trace(290542, TraceLevel.Info, "WebAccess.Settings", businessLogic, "TeamBoardSettingsChangedEvent fired for boardId : {0}, eventType:{1} and extensionId : {2}", (object) boardId, (object) str1, (object) str2);
      }
    }

    private void InvalidateBoardCache(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? boardId = null)
    {
      BoardSettingsCacheService service = requestContext.GetService<BoardSettingsCacheService>();
      if (boardId.HasValue)
      {
        string settingsCacheKey = this.GetBoardSettingsCacheKey(requestContext, projectId, boardId.Value);
        if (settingsCacheKey == null)
          service.Clear(requestContext);
        else
          service.Remove(requestContext, settingsCacheKey);
      }
      else
        service.Clear(requestContext);
    }

    private void UpdateBoardIdMap(
      IVssRequestContext requestContext,
      Guid boardId,
      Guid teamId,
      string backlogLevelId)
    {
      requestContext.GetService<BoardIdToBoardInputMap>().Set(requestContext, boardId, new BoardInput()
      {
        TeamId = teamId,
        BacklogLevelId = backlogLevelId
      });
    }

    private BoardInput GetBoardInput(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid boardId)
    {
      BoardInput boardInput;
      if (!requestContext.GetService<BoardIdToBoardInputMap>().TryGetValue(requestContext, boardId, out boardInput))
      {
        using (BoardSettingsComponent component = BoardSettingsComponent.CreateComponent(requestContext))
          boardInput = component.GetBoardInput(projectId, boardId);
      }
      return boardInput;
    }

    private string GetBoardSettingsCacheKey(BoardInput boardInput) => boardInput.TeamId.ToString() + "_" + boardInput.BacklogLevelId;

    private string GetBoardSettingsCacheKey(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid boardId)
    {
      BoardInput boardInput = this.GetBoardInput(requestContext, projectId, boardId);
      return boardInput != null ? this.GetBoardSettingsCacheKey(boardInput) : (string) null;
    }

    private WebApiTeam GetTeam(IVssRequestContext requestContext, Guid projectId, Guid teamId) => requestContext.GetService<ITeamService>().GetTeamInProject(requestContext, projectId, teamId.ToString());
  }
}
