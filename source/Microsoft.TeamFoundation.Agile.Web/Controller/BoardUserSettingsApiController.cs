// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.BoardUserSettingsApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Agile.Server.Exceptions;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "boardusersettings")]
  public class BoardUserSettingsApiController : BoardsApiControllerBase
  {
    private static readonly string TraceLayer = nameof (BoardUserSettingsApiController);

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<BoardUserSettingsUpdateFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BoardUserSettingsUpdateFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BoardUserSettingsReadFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BoardUserSettingsReadFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddTranslation(typeof (BoardUserSettingsUpdateFailureException), typeof (BoardUserSettingsUpdateFailureException));
      exceptionMap.AddTranslation(typeof (BoardUserSettingsReadFailureException), typeof (BoardUserSettingsReadFailureException));
    }

    [HttpGet]
    [TraceFilter(290563, 290565)]
    [ClientExample("GET__work_boards_boardId_boardUserSettings.json", "Get board user settings for a boardId", null, null)]
    public Microsoft.TeamFoundation.Work.WebApi.BoardUserSettings GetBoardUserSettings(string board)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(board, nameof (board));
      Guid boardIdFromNameOrId = this.GetBoardIdFromNameOrId(this.TfsRequestContext.GetService<BoardService>(), board);
      try
      {
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardUserSettings boardUserSettings = this.GetBoardSettingsUtils().GetBoardUserSettings(this.TfsRequestContext, this.ProjectId, this.Team, boardIdFromNameOrId);
        return new Microsoft.TeamFoundation.Work.WebApi.BoardUserSettings()
        {
          AutoRefreshState = boardUserSettings.AutoBoardRefreshState
        };
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(290564, TraceLevel.Error, "Agile", BoardUserSettingsApiController.TraceLayer, ex);
        throw new BoardUserSettingsReadFailureException(ex);
      }
    }

    [HttpPatch]
    [TraceFilter(290566, 290568)]
    [ClientExample("PATCH__work_boards_boardId_boardUserSettings.json", "Update a board's user settings", null, null)]
    public Microsoft.TeamFoundation.Work.WebApi.BoardUserSettings UpdateBoardUserSettings(
      string board,
      Dictionary<string, string> boardUserSettings)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(board, nameof (board));
      ArgumentUtility.CheckForNull<Dictionary<string, string>>(boardUserSettings, nameof (boardUserSettings));
      this.CheckBacklogManagementLicense();
      Guid boardIdFromNameOrId = this.GetBoardIdFromNameOrId(this.TfsRequestContext.GetService<BoardService>(), board);
      try
      {
        this.GetBoardSettingsUtils().SetBoardUserSettings(this.TfsRequestContext, this.ProjectId, this.Team, boardIdFromNameOrId, boardUserSettings);
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(290567, TraceLevel.Error, "Agile", BoardUserSettingsApiController.TraceLayer, ex);
        throw new BoardUserSettingsUpdateFailureException(ex);
      }
      return this.GetBoardUserSettings(board);
    }

    [NonAction]
    public virtual IBoardUserSettingsUtils GetBoardSettingsUtils() => BoardUserSettingsUtils.Instance;
  }
}
