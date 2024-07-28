// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.BoardsApiControllerBase
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Agile.Server.Services;
using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  public abstract class BoardsApiControllerBase : TfsTeamApiController
  {
    private IBoardsRestApiHelper m_boardsApiHelper;

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<ArgumentException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentOutOfRangeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<ArgumentNullException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<KeyNotFoundException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<SqlException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<SqlTypeException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Agile.Server.Exceptions.CardRulesValidationFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.Azure.Boards.Agile.Common.Exceptions.CardRulesValidationFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<UnauthorizedAccessException>(HttpStatusCode.Forbidden);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Agile.Server.Exceptions.BoardDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<Microsoft.Azure.Boards.Agile.Common.Exceptions.BoardDoesNotExistException>(HttpStatusCode.NotFound);
      exceptionMap.AddTranslation(typeof (Microsoft.TeamFoundation.Agile.Server.Exceptions.BoardDoesNotExistException), typeof (Microsoft.Azure.Boards.Agile.Common.Exceptions.BoardDoesNotExistException));
      exceptionMap.AddTranslation(typeof (Microsoft.TeamFoundation.Agile.Server.Exceptions.CardRulesValidationFailureException), typeof (Microsoft.Azure.Boards.Agile.Common.Exceptions.CardRulesValidationFailureException));
    }

    internal virtual IBoardsRestApiHelper BoardsApiHelper
    {
      get
      {
        if (this.m_boardsApiHelper == null)
        {
          ArgumentUtility.CheckForNull<WebApiTeam>(this.Team, "TfsTeamApiController.Team");
          this.m_boardsApiHelper = (IBoardsRestApiHelper) new BoardsRestApiHelper(this.TfsRequestContext, this.ProjectInfo, this.Team);
        }
        return this.m_boardsApiHelper;
      }
    }

    public virtual IAgileSettings AgileSettings => this.BoardsApiHelper.AgileSettings;

    protected Dictionary<string, string> LowerBacklogLevelName2BacklogLevelId => this.BoardsApiHelper.LowerBacklogLevelName2BacklogLevelId;

    protected Dictionary<string, string> BacklogLevelId2BacklogLevelName => this.BoardsApiHelper.BacklogLevelId2BacklogLevelName;

    protected List<string> ApplicableBacklogLevelIds => this.BoardsApiHelper.ApplicableBacklogLevelIds;

    protected CommonStructureProjectInfo CssProjectInfo => this.BoardsApiHelper.CssProjectInfo;

    [NonAction]
    public virtual Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings GetBoardCardSettings(
      string boardCategoryReferenceName,
      Guid boardId)
    {
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration levelConfiguration = this.AgileSettings.BacklogConfiguration.GetBacklogLevelConfiguration(boardCategoryReferenceName);
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings boardCardSettings = KanbanUtils.Instance.GetOrCreateBoardCardSettings(this.TfsRequestContext, this.CssProjectInfo, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings.ScopeType.KANBAN, boardId);
      if (boardCardSettings == null)
        this.TfsRequestContext.Trace(1530014, TraceLevel.Verbose, "AgileService", "AgileService", "Failed to get board card settings for the Board category: {0}, BoardId: {1}, and TeamId: {2}", (object) boardCategoryReferenceName, (object) boardId, (object) this.TeamId);
      CardSettingsUtils.Instance.ValidateAndReconcileSettingsForGET(this.TfsRequestContext, levelConfiguration, this.AgileSettings, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings.ScopeType.KANBAN, boardId, boardCardSettings);
      return boardCardSettings;
    }

    [NonAction]
    public virtual void ValidateBoardCardRules(
      Dictionary<string, BoardCardRules> boardCardRuleTypeMapping,
      Guid boardId,
      IDictionary<Guid, string> tagNamesReplacedbyId)
    {
      foreach (string key in boardCardRuleTypeMapping.Keys)
      {
        BoardCardRules boardCardRules = boardCardRuleTypeMapping[key];
        boardCardRules.ScopeId = boardId;
        boardCardRules.Scope = "KANBAN";
        string message = this.CardRulesValidation(boardCardRules, key, tagNamesReplacedbyId);
        if (!string.IsNullOrEmpty(message))
        {
          this.TfsRequestContext.Trace(1530017, TraceLevel.Error, "Agile", "Controller", "Invalid board card rules data received for board {0} for team {1}.", (object) boardCardRules.ScopeId, (object) this.Team.Name);
          throw new Microsoft.TeamFoundation.Agile.Server.Exceptions.CardRulesValidationFailureException(message);
        }
      }
    }

    [NonAction]
    public virtual void UpdateBoardCardRules(
      BoardService boardService,
      BoardCardRules boardCardRules,
      List<string> typesToBeDeleted)
    {
      try
      {
        this.UpdateBoardCardRulesThroughBoardService(boardService, this.ProjectInfo.Id, boardCardRules, typesToBeDeleted);
      }
      catch (Exception ex)
      {
        throw new Microsoft.TeamFoundation.Agile.Server.Exceptions.CardRulesUpdateFailureException(ex);
      }
    }

    [NonAction]
    public virtual void UpdateBoardCardRulesThroughBoardService(
      BoardService boardService,
      Guid projectId,
      BoardCardRules boardCardRules,
      List<string> typesToBeDeleted)
    {
      boardService.UpdateBoardCardRules(this.TfsRequestContext, projectId, boardCardRules, typesToBeDeleted);
    }

    [NonAction]
    public virtual Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings UpdateBoardCardSettings(
      BoardService boardService,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings boardCardSettings,
      string boardCategoryReferenceName,
      Guid boardId)
    {
      string message = this.CardSettingsUtilsValidateAndReconcile(boardCardSettings);
      if (!string.IsNullOrEmpty(message))
      {
        this.TfsRequestContext.Trace(1530014, TraceLevel.Error, "Agile", "Controller", "Invalid board card settings data received for board {0} for team {1}.", (object) boardCardSettings.ScopeId, (object) this.Team.Name);
        throw new Microsoft.TeamFoundation.Agile.Server.Exceptions.CardSettingsValidationFailureException(message);
      }
      this.UpdateBoardCardSettingsThroughBoardService(boardService, this.ProjectInfo.Id, boardCardSettings);
      return this.GetBoardCardSettings(boardCategoryReferenceName, boardId);
    }

    [NonAction]
    public virtual void UpdateTaskboardCardSettings(
      BoardService boardService,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings boardCardSettings)
    {
      string message = this.CardSettingsUtilsValidateAndReconcile(boardCardSettings);
      if (!string.IsNullOrEmpty(message))
      {
        this.TfsRequestContext.Trace(290955, TraceLevel.Error, "Agile", "Controller", "Invalid board card settings data received for taskboard for team {0}. Error message: {1}", (object) boardCardSettings.ScopeId, (object) message);
        throw new Microsoft.TeamFoundation.Agile.Server.Exceptions.CardSettingsValidationFailureException(message);
      }
      this.TfsRequestContext.Trace(290100, TraceLevel.Verbose, "Agile", "Controller", "Updating card settings for taskboard for teamId {0}", (object) boardCardSettings.ScopeId);
      this.UpdateBoardCardSettingsThroughBoardService(boardService, this.ProjectInfo.Id, boardCardSettings);
    }

    [NonAction]
    public virtual string CardSettingsUtilsValidateAndReconcile(Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings boardCardSettings) => CardSettingsUtils.Instance.ValidateAndReconcileBoardCardSettingsForSET(this.TfsRequestContext, (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration) null, this.AgileSettings, boardCardSettings, this.TeamId);

    [NonAction]
    public virtual string CardRulesValidation(
      BoardCardRules boardCardRules,
      string type,
      IDictionary<Guid, string> tagNamesReplacedbyId)
    {
      return CardSettingsUtils.Instance.ValidateCardRules(this.TfsRequestContext, boardCardRules, type, tagNamesReplacedbyId);
    }

    [NonAction]
    public virtual void UpdateBoardCardSettingsThroughBoardService(
      BoardService boardService,
      Guid projectId,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings boardCardSettings)
    {
      boardService.UpdateBoardCardSettings(this.TfsRequestContext, projectId, boardCardSettings);
    }

    [NonAction]
    public virtual BoardSettings GetOrCreateBoardSettings(string backlogLevelId) => this.BoardsApiHelper.GetOrCreateBoardSettings(backlogLevelId);

    [NonAction]
    public virtual IDictionary<string, IDictionary<string, string[]>> GetAllowedStateMappings(
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration backlogLevel)
    {
      return KanbanUtils.Instance.GetColumnTypeAllowedStateMappings(this.TfsRequestContext, this.AgileSettings.ProjectName, backlogConfiguration, backlogLevel, this.AgileSettings.TeamSettings.BugsBehavior == Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BugsBehavior.AsRequirements);
    }

    protected string GetBoardBacklogLevelIdByNameOrId(BoardService boardService, string board) => this.BoardsApiHelper.GetBoardBacklogLevelIdByNameOrId(boardService, board);

    protected Guid GetBoardIdFromNameOrId(BoardService boardService, string board) => this.BoardsApiHelper.GetBoardIdFromNameOrId(boardService, board);

    protected Dictionary<string, BoardCardRules> ConvertCardRulesToRowView(
      BoardCardRuleSettings boardCardRuleSettings)
    {
      return this.BoardsApiHelper.ConvertCardRulesToRowView(boardCardRuleSettings);
    }

    protected void CheckBacklogManagementLicense()
    {
      if (this.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      this.TfsRequestContext.GetService<IAgileProjectPermissionService>().GetAdvanceBacklogManagementPermission(this.TfsRequestContext, this.ProjectId, true);
    }

    protected void CheckAdminPermission()
    {
      if (!this.Team.UserIsTeamAdmin(this.TfsRequestContext))
        throw new Microsoft.TeamFoundation.Agile.Server.Exceptions.NoPermissionUpdateCardRulesException();
    }
  }
}
