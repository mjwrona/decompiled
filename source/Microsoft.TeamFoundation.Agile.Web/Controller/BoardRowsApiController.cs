// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.BoardRowsApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Boards.Agile.Common.Exceptions;
using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "rows")]
  public class BoardRowsApiController : BoardsApiControllerBase
  {
    private const string WebAccessAgileKanbanBoardReduceRace = "WebAccess.Agile.KanbanBoard.ReduceRace";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<NoPermissionUpdateBoardRowsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BoardUpdateFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BoardValidationFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<DeletedBoardRowIsNotEmptyException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BoardValidatorInvalidCharException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BoardValidatorRowNameLengthInvalidException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BoardValidatorDuplicateRowNameException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BoardValidatorRowCountInvalidException>(HttpStatusCode.BadRequest);
    }

    [HttpPut]
    [ClientExample("PUT__work_boards__boardName__rows.json", "Update rows on a board", null, null)]
    public IList<Microsoft.TeamFoundation.Work.WebApi.BoardRow> UpdateBoardRows(
      IList<Microsoft.TeamFoundation.Work.WebApi.BoardRow> boardRows,
      string board)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) boardRows, nameof (boardRows));
      ArgumentUtility.CheckStringForNullOrEmpty(board, nameof (board));
      this.CheckBacklogManagementLicense();
      BoardService boardService = this.Team.UserIsTeamAdmin(this.TfsRequestContext) ? this.TfsRequestContext.GetService<BoardService>() : throw new NoPermissionUpdateBoardRowsException(Microsoft.TeamFoundation.Agile.Server.AgileResources.UpdateBoardRows_UserIsNotTeamAdmin);
      string levelIdByNameOrId = this.GetBoardBacklogLevelIdByNameOrId(boardService, board);
      BoardSettings boardSettings = this.GetOrCreateBoardSettings(levelIdByNameOrId);
      IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardRow> rows = boardSettings.Rows;
      boardSettings.Rows = boardRows.Select<Microsoft.TeamFoundation.Work.WebApi.BoardRow, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardRow>((Func<Microsoft.TeamFoundation.Work.WebApi.BoardRow, int, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardRow>) ((r, index) => new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardRow()
      {
        Id = r.Id,
        Name = r.Name != null ? r.Name.Trim() : (string) null,
        Order = index,
        Color = r.Color
      }));
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfiguration backlogConfiguration = this.TfsRequestContext.GetService<IBacklogConfigurationService>().GetProjectBacklogConfiguration(this.TfsRequestContext, this.ProjectInfo.Id);
      BoardRowsValidator boardRowsValidator = boardSettings.GetBoardRowsValidator(this.TfsRequestContext, backlogConfiguration);
      if (!boardRowsValidator.Validate(false))
        throw new BoardValidationFailureException(boardRowsValidator.GetUserFriendlyErrorMessage());
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration levelConfiguration = this.AgileSettings.BacklogConfiguration.GetBacklogLevelConfiguration(boardSettings.BacklogLevelId);
      List<Guid?> rowIdsToDelete = rows.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardRow, Guid?>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardRow, Guid?>) (r => r.Id)).ToList<Guid?>().Except<Guid?>(boardRows.Select<Microsoft.TeamFoundation.Work.WebApi.BoardRow, Guid?>((Func<Microsoft.TeamFoundation.Work.WebApi.BoardRow, Guid?>) (r => r.Id))).ToList<Guid?>();
      if (!rowIdsToDelete.IsNullOrEmpty<Guid?>())
      {
        IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardRow> rowsToDelete = rows.Where<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardRow>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardRow, bool>) (r => rowIdsToDelete.Contains(r.Id)));
        this.ValidateRowsToDelete(boardSettings, rowsToDelete, levelConfiguration);
      }
      this.TfsRequestContext.Trace(290200, TraceLevel.Verbose, "AgileService", "AgileService", "Begin update Kanban WIT Extension and board.\r\n Team Id: {0}, board: {1}.", (object) this.TeamId, (object) board);
      try
      {
        this.UpdateWITExtensionAndBoard(boardSettings, boardService, levelConfiguration);
      }
      catch (Exception ex)
      {
        throw new BoardUpdateFailureException(ex);
      }
      this.TfsRequestContext.Trace(290201, TraceLevel.Verbose, "AgileService", "AgileService", "End update Kanban WIT Extension and board.\r\n Team Id: {0}, board: {1}.", (object) this.TeamId, (object) board);
      return this.CreateBoardRowsModel(this.GetOrCreateBoardSettings(levelIdByNameOrId).Rows);
    }

    [NonAction]
    public virtual void UpdateWITExtensionAndBoard(
      BoardSettings boardSettings,
      BoardService boardService,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration backlogLevel)
    {
      if (this.TfsRequestContext.IsFeatureEnabled("WebAccess.Agile.KanbanBoard.ReduceRace"))
      {
        CommonStructureProjectInfo project = CommonStructureProjectInfo.ConvertProjectInfo(this.ProjectInfo);
        BoardSettings boardSettings1 = KanbanUtils.Instance.GetBoardSettings(this.TfsRequestContext, this.Team, backlogLevel, project, true);
        boardService.UpdateBoardRows(this.TfsRequestContext, this.ProjectInfo.Id, boardSettings.Id.Value, boardSettings.Rows);
        KanbanUtils.Instance.UpdateExtension(this.TfsRequestContext, project, this.Team, backlogLevel, boardSettings, true, true, 60000, this.AgileSettings.TeamSettings, oldBoardSettings: boardSettings1);
      }
      else
      {
        KanbanUtils.Instance.UpdateExtension(this.TfsRequestContext, CommonStructureProjectInfo.ConvertProjectInfo(this.ProjectInfo), this.Team, backlogLevel, boardSettings, true, true, 60000, this.AgileSettings.TeamSettings);
        boardService.UpdateBoardRows(this.TfsRequestContext, this.ProjectInfo.Id, boardSettings.Id.Value, boardSettings.Rows);
      }
    }

    [HttpGet]
    [ClientExample("GET__work_boards__boardId__rows.json", "Get a kanban board's rows by boardId", null, null)]
    [ClientExample("GET__work_boards__boardName__rows.json", "Get a kanban board's rows by boardName", null, null)]
    public IList<Microsoft.TeamFoundation.Work.WebApi.BoardRow> GetBoardRows(string board)
    {
      ArgumentUtility.CheckForNull<string>(board, nameof (board));
      return this.CreateBoardRowsModel(this.GetOrCreateBoardSettings(this.GetBoardBacklogLevelIdByNameOrId(this.TfsRequestContext.GetService<BoardService>(), board)).Rows);
    }

    [NonAction]
    public virtual void ValidateRowsToDelete(
      BoardSettings boardSettings,
      IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardRow> rowsToDelete,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration backlogLevel)
    {
      KanbanHelper.ValidateDeletedKanbanRows(this.TfsRequestContext, this.AgileSettings, new BacklogContext(new Microsoft.TeamFoundation.Agile.Server.Team()
      {
        Id = this.TeamId,
        Name = this.Team.Name
      }, this.AgileSettings.BacklogConfiguration.GetBacklogByName(backlogLevel.Name)), boardSettings, rowsToDelete);
    }

    private IList<Microsoft.TeamFoundation.Work.WebApi.BoardRow> CreateBoardRowsModel(
      IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardRow> rows)
    {
      return (IList<Microsoft.TeamFoundation.Work.WebApi.BoardRow>) rows.OrderBy<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardRow, int>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardRow, int>) (r => r.Order)).Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardRow, Microsoft.TeamFoundation.Work.WebApi.BoardRow>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardRow, Microsoft.TeamFoundation.Work.WebApi.BoardRow>) (r => new Microsoft.TeamFoundation.Work.WebApi.BoardRow()
      {
        Id = r.Id,
        Name = r.Name,
        Color = r.Color
      })).ToList<Microsoft.TeamFoundation.Work.WebApi.BoardRow>();
    }
  }
}
