// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.BoardsApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Agile.Server.Exceptions;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Location;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "boards")]
  public class BoardsApiController : BoardsApiControllerBase
  {
    private const string c_cardReordering = "cardReordering";
    private const string c_statusBadgeIsPublic = "statusBadgeIsPublic";

    [HttpGet]
    public IEnumerable<BoardReference> GetBoards()
    {
      ArgumentUtility.CheckForNull<ProjectInfo>(this.ProjectInfo, "ProjectInfo");
      ArgumentUtility.CheckForNull<WebApiTeam>(this.Team, "Team");
      ArgumentUtility.CheckForEmptyGuid(this.TeamId, "TeamId");
      return (IEnumerable<BoardReference>) this.GetOrCreateBoards().Select<BoardSettings, BoardReference>((Func<BoardSettings, BoardReference>) (board => new BoardReference()
      {
        Id = board.Id.Value,
        Name = this.BacklogLevelId2BacklogLevelName[board.BacklogLevelId],
        Url = AgileResourceUtils.GetAgileResourceUriString(this.TfsRequestContext, BoardApiConstants.BoardsLocationId, this.ProjectId, this.TeamId, (object) new
        {
          id = board.Id
        })
      })).ToList<BoardReference>();
    }

    [HttpGet]
    public Board GetBoard(string id)
    {
      ArgumentUtility.CheckForNull<ProjectInfo>(this.ProjectInfo, "ProjectInfo");
      ArgumentUtility.CheckForNull<WebApiTeam>(this.Team, "Team");
      string levelIdByNameOrId = this.GetBoardBacklogLevelIdByNameOrId(this.TfsRequestContext.GetService<BoardService>(), id);
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration levelConfiguration = this.AgileSettings.BacklogConfiguration.GetBacklogLevelConfiguration(levelIdByNameOrId);
      string name = levelConfiguration.Name;
      BoardSettings boardSettings = this.GetOrCreateBoardSettings(levelIdByNameOrId);
      if (boardSettings == null)
        throw new BoardDoesNotExistException();
      boardSettings.AllowedMappings = this.GetAllowedStateMappings(this.AgileSettings.BacklogConfiguration, levelConfiguration);
      boardSettings.CanEdit = this.Team.UserIsTeamAdmin(this.TfsRequestContext);
      return this.CreateBoardModel(boardSettings, name);
    }

    [HttpPut]
    public Dictionary<string, string> SetBoardOptions(string id, Dictionary<string, string> options)
    {
      ArgumentUtility.CheckForNull<Dictionary<string, string>>(options, nameof (options));
      this.CheckBacklogManagementLicense();
      BoardService boardService = this.Team.UserIsTeamAdmin(this.TfsRequestContext) ? this.TfsRequestContext.GetService<BoardService>() : throw new NoPermissionUpdateBoardOptions();
      Guid boardIdFromNameOrId = this.GetBoardIdFromNameOrId(boardService, id);
      BoardOptionRecord boardOptionsRecord = boardService.GetBoardOptionsRecord(this.TfsRequestContext, this.ProjectId, boardIdFromNameOrId);
      int cardReordering = boardOptionsRecord != null ? boardOptionsRecord.CardReordering : 0;
      bool statusBadgeIsPublic = boardOptionsRecord != null && boardOptionsRecord.StatusBadgeIsPublic;
      int result1;
      if (options.ContainsKey("cardReordering") && int.TryParse(options["cardReordering"], NumberStyles.Any, (IFormatProvider) CultureInfo.InvariantCulture, out result1))
        cardReordering = result1;
      bool result2;
      if (options.ContainsKey("statusBadgeIsPublic") && bool.TryParse(options["statusBadgeIsPublic"], out result2))
      {
        if (statusBadgeIsPublic != result2)
          TelemetryUtils.PublishBoardBadgeIsPublicStatusChanged(this.TfsRequestContext, result2);
        statusBadgeIsPublic = result2;
      }
      bool suppressChangeEvent = options.Count == 1 && options.ContainsKey("statusBadgeIsPublic");
      boardService.UpdateBoardOptions(this.TfsRequestContext, this.ProjectId, boardIdFromNameOrId, cardReordering, statusBadgeIsPublic, suppressChangeEvent);
      return boardService.GetBoardOptions(this.TfsRequestContext, this.ProjectId, boardIdFromNameOrId);
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<NoPermissionUpdateBoardOptions>(HttpStatusCode.Forbidden);
    }

    private List<BoardSettings> GetOrCreateBoards()
    {
      List<BoardSettings> boardsSettings = new List<BoardSettings>();
      this.ApplicableBacklogLevelIds.ForEach((Action<string>) (board => boardsSettings.Add(this.GetOrCreateBoardSettings(board))));
      return boardsSettings;
    }

    private Board CreateBoardModel(BoardSettings boardSettings, string name)
    {
      string boardId = boardSettings.Id.ToString();
      Board boardModel1 = new Board();
      boardModel1.Id = boardSettings.Id.Value;
      boardModel1.Name = name;
      boardModel1.Url = AgileResourceUtils.GetAgileResourceUriString(this.TfsRequestContext, BoardApiConstants.BoardsLocationId, this.ProjectId, this.TeamId, (object) new
      {
        id = boardSettings.Id
      });
      boardModel1.IsValid = boardSettings.IsValid(this.TfsRequestContext, this.AgileSettings.Process, this.ProjectInfo.Name, this.Team, this.AgileSettings.TeamSettings);
      boardModel1.AllowedMappings = (IDictionary<string, IDictionary<string, string[]>>) boardSettings.AllowedMappings.ToDictionary<KeyValuePair<string, IDictionary<string, string[]>>, string, IDictionary<string, string[]>>((Func<KeyValuePair<string, IDictionary<string, string[]>>, string>) (item => ((Microsoft.TeamFoundation.Work.WebApi.BoardColumnType) int.Parse(item.Key)).ToString()), (Func<KeyValuePair<string, IDictionary<string, string[]>>, IDictionary<string, string[]>>) (item => item.Value));
      boardModel1.CanEdit = boardSettings.CanEdit;
      boardModel1.Revision = 0;
      boardModel1.Columns = (IList<Microsoft.TeamFoundation.Work.WebApi.BoardColumn>) boardSettings.Columns.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, Microsoft.TeamFoundation.Work.WebApi.BoardColumn>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumn, Microsoft.TeamFoundation.Work.WebApi.BoardColumn>) (column =>
      {
        Microsoft.TeamFoundation.Work.WebApi.BoardColumn boardModel2 = new Microsoft.TeamFoundation.Work.WebApi.BoardColumn()
        {
          Id = column.Id,
          Name = column.Name,
          StateMappings = column.StateMappings,
          ItemLimit = new int?(column.ItemLimit),
          ColumnType = (Microsoft.TeamFoundation.Work.WebApi.BoardColumnType) column.ColumnType
        };
        if (column.ColumnType == Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardColumnType.InProgress)
        {
          boardModel2.Description = string.IsNullOrEmpty(column.Description) ? string.Empty : column.Description;
          boardModel2.IsSplit = new bool?(column.IsSplit);
        }
        return boardModel2;
      })).ToList<Microsoft.TeamFoundation.Work.WebApi.BoardColumn>();
      boardModel1.Rows = (IList<Microsoft.TeamFoundation.Work.WebApi.BoardRow>) boardSettings.Rows.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardRow, Microsoft.TeamFoundation.Work.WebApi.BoardRow>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardRow, Microsoft.TeamFoundation.Work.WebApi.BoardRow>) (row => new Microsoft.TeamFoundation.Work.WebApi.BoardRow()
      {
        Id = row.Id,
        Name = row.Name
      })).ToList<Microsoft.TeamFoundation.Work.WebApi.BoardRow>();
      boardModel1.Fields = new BoardFields()
      {
        ColumnField = this.GetFieldReference(KanbanUtils.Instance.GetKanbanColumnFieldReferenceName(this.TfsRequestContext, boardSettings.ExtensionId.Value)),
        DoneField = this.GetFieldReference(KanbanUtils.Instance.GetKanbanDoneFieldReferenceName(this.TfsRequestContext, boardSettings.ExtensionId.Value)),
        RowField = this.GetFieldReference(KanbanUtils.Instance.GetKanbanLaneFieldReferenceName(this.TfsRequestContext, boardSettings.ExtensionId.Value))
      };
      boardModel1.Links = this.GetReferenceLinks(this.TfsRequestContext, this.ProjectId.ToString(), this.TeamId.ToString(), boardId);
      return boardModel1;
    }

    private FieldReference GetFieldReference(string fieldReferenceName) => new FieldReference()
    {
      ReferenceName = fieldReferenceName,
      Url = WitUrlHelper.GetFieldUrl(this.TfsRequestContext, fieldReferenceName)
    };

    private ReferenceLinks GetReferenceLinks(
      IVssRequestContext tfsRequestContext,
      string projectId,
      string teamId,
      string boardId)
    {
      ILocationService service = tfsRequestContext.GetService<ILocationService>();
      ReferenceLinks referenceLinks = new ReferenceLinks();
      referenceLinks.AddLink("self", AgileResourceUtils.GetAgileResourceUriString(tfsRequestContext, BoardApiConstants.BoardsLocationId, this.ProjectId, this.TeamId, (object) new
      {
        id = boardId
      }));
      referenceLinks.AddLink("project", service.GetCoreResourceUriString(tfsRequestContext, CoreConstants.ProjectsLocationId, (object) new
      {
        projectId = projectId
      }));
      referenceLinks.AddLink("team", service.GetCoreResourceUriString(tfsRequestContext, CoreConstants.TeamsLocationId, (object) new
      {
        projectId = projectId,
        teamId = teamId
      }));
      referenceLinks.AddLink("charts", AgileResourceUtils.GetAgileResourceUriString(tfsRequestContext, BoardApiConstants.BoardChartsLocationId, this.ProjectId, this.TeamId, (object) new
      {
        board = boardId
      }));
      referenceLinks.AddLink("columns", AgileResourceUtils.GetAgileResourceUriString(tfsRequestContext, BoardApiConstants.BoardColumnsLocationId, this.ProjectId, this.TeamId, (object) new
      {
        board = boardId
      }));
      referenceLinks.AddLink("rows", AgileResourceUtils.GetAgileResourceUriString(tfsRequestContext, BoardApiConstants.BoardRowsLocationId, this.ProjectId, this.TeamId, (object) new
      {
        board = boardId
      }));
      return referenceLinks;
    }
  }
}
