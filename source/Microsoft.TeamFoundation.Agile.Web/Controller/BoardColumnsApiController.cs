// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.BoardColumnsApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Boards.Agile.Common.Exceptions;
using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Core.WebApi;
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
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "columns")]
  public class BoardColumnsApiController : BoardsApiControllerBase
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<DeletedBoardColumnIsNotEmptyException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BoardValidationFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BoardUpdateFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<NoPermissionUpdateBoardColumnsException>(HttpStatusCode.BadRequest);
    }

    [HttpPut]
    [ClientExample("PUT__work_boards__boardName__columns.json", "Update columns on a board", null, null)]
    public IList<Microsoft.TeamFoundation.Work.WebApi.BoardColumn> UpdateBoardColumns(
      IList<Microsoft.TeamFoundation.Work.WebApi.BoardColumn> boardColumns,
      string board)
    {
      ArgumentUtility.CheckForNull<IList<Microsoft.TeamFoundation.Work.WebApi.BoardColumn>>(boardColumns, nameof (boardColumns));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) boardColumns, nameof (boardColumns));
      ArgumentUtility.CheckStringForNullOrEmpty(board, nameof (board));
      this.CheckBacklogManagementLicense();
      BoardService boardService = this.Team.UserIsTeamAdmin(this.TfsRequestContext) ? this.TfsRequestContext.GetService<BoardService>() : throw new NoPermissionUpdateBoardColumnsException(Microsoft.TeamFoundation.Agile.Server.AgileResources.UpdateBoardColumns_UserIsNotTeamAdmin);
      string levelIdByNameOrId = this.GetBoardBacklogLevelIdByNameOrId(boardService, board);
      BoardSettings boardSettings = this.GetOrCreateBoardSettings(levelIdByNameOrId);
      IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn> columns = boardSettings.Columns;
      IEnumerable<Guid?> deletedColumnIds = boardSettings.Columns.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, Guid?>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, Guid?>) (c => c.Id)).ToList<Guid?>().Except<Guid?>(boardColumns.Select<Microsoft.TeamFoundation.Work.WebApi.BoardColumn, Guid?>((Func<Microsoft.TeamFoundation.Work.WebApi.BoardColumn, Guid?>) (c => c.Id)));
      IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn> deletedColumns = boardSettings.Columns.Where<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, bool>) (c => deletedColumnIds.Contains<Guid?>(c.Id)));
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration levelConfiguration = this.AgileSettings.BacklogConfiguration.GetBacklogLevelConfiguration(boardSettings.BacklogLevelId);
      this.UpdateBoardSettingColumnsFromModelColumns(boardSettings, (IEnumerable<Microsoft.TeamFoundation.Work.WebApi.BoardColumn>) boardColumns);
      BoardColumnsValidator columnsValidator = boardSettings.GetBoardColumnsValidator(this.TfsRequestContext, this.AgileSettings.Process, this.AgileSettings.BacklogConfiguration, this.ProjectInfo.Name, this.AgileSettings.TeamSettings, this.GetOrCreateBoardSettings(levelIdByNameOrId));
      if (!columnsValidator.Validate(false))
        throw new BoardValidationFailureException(columnsValidator.GetUserFriendlyErrorMessage());
      this.ValidateDeletedColumns(boardSettings, deletedColumns, levelConfiguration);
      this.TfsRequestContext.Trace(290097, TraceLevel.Verbose, "AgileService", "AgileService", "Begin update Kanban WIT Extension and board.\r\n Team Id: {0}, board: {1}.", (object) this.TeamId, (object) board);
      try
      {
        this.UpdateWITExtensionAndBoard(boardSettings, boardService, deletedColumns, levelConfiguration);
      }
      catch (Exception ex)
      {
        throw new BoardUpdateFailureException(ex);
      }
      this.TfsRequestContext.Trace(290098, TraceLevel.Verbose, "AgileService", "AgileService", "End update Kanban WIT Extension and board.\r\n Team Id: {0}, board: {1}.", (object) this.TeamId, (object) board);
      TelemetryUtils.PublishUpdateColumnsData(this.TfsRequestContext, columns, boardSettings.Columns);
      return this.CreateBoardColumnsModel(this.GetOrCreateBoardSettings(levelIdByNameOrId).Columns);
    }

    [NonAction]
    public virtual void UpdateWITExtensionAndBoard(
      BoardSettings boardSettings,
      BoardService boardService,
      IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn> deletedColumns,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration backlogLevel)
    {
      KanbanUtils.Instance.UpdateExtension(this.TfsRequestContext, CommonStructureProjectInfo.ConvertProjectInfo(this.ProjectInfo), this.Team, backlogLevel, boardSettings, true, true, 60000, this.AgileSettings.TeamSettings, deletedColumns);
      if (this.TfsRequestContext.GetService<TeamFoundationResourceManagementService>().GetServiceVersion(this.TfsRequestContext, "BoardSettings", "Default").Version > 6)
        boardService.UpdateBoardColumns(this.TfsRequestContext, this.ProjectInfo.Id, boardSettings.Id.Value, boardSettings.Columns);
      else
        boardService.UpdateBoard(this.TfsRequestContext, this.ProjectInfo.Id, boardSettings);
    }

    [NonAction]
    public virtual void ValidateDeletedColumns(
      BoardSettings boardSettings,
      IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn> deletedColumns,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration backlogLevel)
    {
      KanbanHelper.ValidateDeletedKanbanColumns(this.TfsRequestContext, this.AgileSettings, new BacklogContext(new Microsoft.TeamFoundation.Agile.Server.Team()
      {
        Id = this.TeamId,
        Name = this.Team.Name
      }, backlogLevel), boardSettings.ExtensionId.Value, deletedColumns);
    }

    [HttpGet]
    [ClientExample("GET__work_boards__boardId__columns.json", "Get board columns by boardId", null, null)]
    [ClientExample("GET__work_boards__boardName__columns.json", "Get board columns by boardName", null, null)]
    public IList<Microsoft.TeamFoundation.Work.WebApi.BoardColumn> GetBoardColumns(string board)
    {
      ArgumentUtility.CheckForNull<string>(board, nameof (board));
      return this.CreateBoardColumnsModel(this.GetOrCreateBoardSettings(this.GetBoardBacklogLevelIdByNameOrId(this.TfsRequestContext.GetService<BoardService>(), board)).Columns);
    }

    private IList<Microsoft.TeamFoundation.Work.WebApi.BoardColumn> CreateBoardColumnsModel(
      IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn> columns)
    {
      return (IList<Microsoft.TeamFoundation.Work.WebApi.BoardColumn>) columns.OrderBy<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, int>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, int>) (c => c.Order)).Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, Microsoft.TeamFoundation.Work.WebApi.BoardColumn>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, Microsoft.TeamFoundation.Work.WebApi.BoardColumn>) (column =>
      {
        Microsoft.TeamFoundation.Work.WebApi.BoardColumn boardColumnsModel = new Microsoft.TeamFoundation.Work.WebApi.BoardColumn()
        {
          Id = column.Id,
          Name = column.Name,
          StateMappings = column.StateMappings,
          ItemLimit = new int?(column.ItemLimit),
          ColumnType = (Microsoft.TeamFoundation.Work.WebApi.BoardColumnType) column.ColumnType
        };
        if (column.ColumnType == Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumnType.InProgress)
        {
          boardColumnsModel.Description = string.IsNullOrEmpty(column.Description) ? string.Empty : column.Description;
          boardColumnsModel.IsSplit = new bool?(column.IsSplit);
        }
        return boardColumnsModel;
      })).ToList<Microsoft.TeamFoundation.Work.WebApi.BoardColumn>();
    }

    private void UpdateBoardSettingColumnsFromModelColumns(
      BoardSettings boardSettings,
      IEnumerable<Microsoft.TeamFoundation.Work.WebApi.BoardColumn> boardColumns)
    {
      List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn> boardColumnList = new List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn>();
      int num1 = boardColumns.Count<Microsoft.TeamFoundation.Work.WebApi.BoardColumn>();
      int num2 = 0;
      this.NormalizeColumn(boardColumns, boardSettings);
      foreach (Microsoft.TeamFoundation.Work.WebApi.BoardColumn boardColumn1 in boardColumns)
      {
        Microsoft.TeamFoundation.Work.WebApi.BoardColumn modelColumn = boardColumn1;
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn boardColumn2 = boardSettings.Columns.FirstOrDefault<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, bool>) (c =>
        {
          Guid? id1 = c.Id;
          Guid? id2 = modelColumn.Id;
          if (id1.HasValue != id2.HasValue)
            return false;
          return !id1.HasValue || id1.GetValueOrDefault() == id2.GetValueOrDefault();
        }));
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn boardColumn3 = new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn();
        boardColumn3.Description = modelColumn.Description == null ? (boardColumn2 == null ? string.Empty : boardColumn2.Description) : modelColumn.Description;
        boardColumn3.Name = modelColumn.Name;
        boardColumn3.Id = modelColumn.Id;
        bool? isSplit = modelColumn.IsSplit;
        int num3;
        if (isSplit.HasValue)
        {
          isSplit = modelColumn.IsSplit;
          num3 = isSplit.Value ? 1 : 0;
        }
        else
          num3 = boardColumn2 == null ? 0 : (boardColumn2.IsSplit ? 1 : 0);
        boardColumn3.IsSplit = num3 != 0;
        int? itemLimit1 = modelColumn.ItemLimit;
        int itemLimit2;
        if (itemLimit1.HasValue)
        {
          itemLimit1 = modelColumn.ItemLimit;
          itemLimit2 = itemLimit1.Value;
        }
        else
          itemLimit2 = boardColumn2 == null ? 0 : boardColumn2.ItemLimit;
        boardColumn3.ItemLimit = itemLimit2;
        boardColumn3.StateMappings = modelColumn.StateMappings;
        boardColumn3.Order = num2++;
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn boardColumn4 = boardColumn3;
        boardColumn4.ColumnType = boardColumn4.Order != 0 ? (boardColumn4.Order != num1 - 1 ? Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumnType.InProgress : Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumnType.Outgoing) : Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumnType.Incoming;
        boardColumnList.Add(boardColumn4);
      }
      boardSettings.Columns = (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn>) boardColumnList;
    }

    [NonAction]
    public virtual void NormalizeColumn(
      IEnumerable<Microsoft.TeamFoundation.Work.WebApi.BoardColumn> boardColumns,
      BoardSettings boardSettings)
    {
      IDictionary<string, IDictionary<string, string[]>> allowedStateMappings = KanbanUtils.Instance.GetColumnTypeAllowedStateMappings(this.TfsRequestContext, this.ProjectInfo.Name, this.AgileSettings.BacklogConfiguration, this.AgileSettings.BacklogConfiguration.GetBacklogLevelConfiguration(boardSettings.BacklogLevelId), this.AgileSettings.TeamSettings.BugsBehavior == Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BugsBehavior.AsRequirements);
      foreach (Microsoft.TeamFoundation.Work.WebApi.BoardColumn boardColumn in boardColumns)
      {
        if (boardColumn.Name != null)
          boardColumn.Name = boardColumn.Name.Trim();
        if (boardColumn.StateMappings != null)
        {
          bool flag = true;
          IDictionary<string, string[]> dictionary1;
          if (allowedStateMappings.TryGetValue(boardColumn.ColumnType.ToString(), out dictionary1))
          {
            Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> stateMapping in boardColumn.StateMappings)
            {
              string workItemType = stateMapping.Key;
              string workItemState = stateMapping.Value;
              if (workItemType == null || workItemState == null)
              {
                flag = false;
                break;
              }
              string key = dictionary1.Keys.Where<string>((Func<string, bool>) (k => TFStringComparer.WorkItemTypeName.Equals(workItemType, k))).FirstOrDefault<string>();
              if (key != null)
              {
                string str = ((IEnumerable<string>) dictionary1[key]).Where<string>((Func<string, bool>) (s => TFStringComparer.WorkItemStateName.Equals(workItemState, s))).FirstOrDefault<string>();
                if (str != null)
                {
                  dictionary2.Add(key, str);
                }
                else
                {
                  flag = false;
                  break;
                }
              }
              else
              {
                flag = false;
                break;
              }
            }
            if (flag)
              boardColumn.StateMappings = dictionary2;
          }
        }
      }
    }
  }
}
