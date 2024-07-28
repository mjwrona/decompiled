// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.BoardCardSettingsApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "cardsettings")]
  public class BoardCardSettingsApiController : BoardsApiControllerBase
  {
    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Agile.Server.Exceptions.NoPermissionUpdateCardSettingsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Agile.Server.Exceptions.CardSettingsValidationFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.TeamFoundation.Agile.Server.Exceptions.CardSettingsUpdateFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.Azure.Boards.Agile.Common.Exceptions.NoPermissionUpdateCardSettingsException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.Azure.Boards.Agile.Common.Exceptions.CardSettingsValidationFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<Microsoft.Azure.Boards.Agile.Common.Exceptions.CardSettingsUpdateFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddTranslation(typeof (Microsoft.TeamFoundation.Agile.Server.Exceptions.NoPermissionUpdateCardSettingsException), typeof (Microsoft.Azure.Boards.Agile.Common.Exceptions.NoPermissionUpdateCardSettingsException));
      exceptionMap.AddTranslation(typeof (Microsoft.TeamFoundation.Agile.Server.Exceptions.CardSettingsValidationFailureException), typeof (Microsoft.Azure.Boards.Agile.Common.Exceptions.CardSettingsValidationFailureException));
      exceptionMap.AddTranslation(typeof (Microsoft.TeamFoundation.Agile.Server.Exceptions.CardSettingsUpdateFailureException), typeof (Microsoft.Azure.Boards.Agile.Common.Exceptions.CardSettingsUpdateFailureException));
    }

    protected Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings ConvertToLegacyBoardCardSettings(
      Microsoft.TeamFoundation.Work.WebApi.BoardCardSettings boardCardSettings,
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings.ScopeType scopeType,
      Guid scopeId)
    {
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings boardCardSettings1 = new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings();
      boardCardSettings1.Scope = scopeType;
      boardCardSettings1.ScopeId = scopeId;
      foreach (KeyValuePair<string, List<Microsoft.TeamFoundation.Work.WebApi.FieldSetting>> card in boardCardSettings.Cards)
      {
        List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting> setting = new List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting>();
        ArgumentUtility.CheckForNull<List<Microsoft.TeamFoundation.Work.WebApi.FieldSetting>>(card.Value, "boardCardSettings.cards." + card.Key + ".fieldSettings");
        foreach (Microsoft.TeamFoundation.Work.WebApi.FieldSetting fieldSetting1 in card.Value)
        {
          Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting fieldSetting2 = new Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting();
          foreach (KeyValuePair<string, string> keyValuePair in (Dictionary<string, string>) fieldSetting1)
            fieldSetting2[keyValuePair.Key] = keyValuePair.Value;
          setting.Add(fieldSetting2);
        }
        boardCardSettings1.setCard(card.Key, setting);
      }
      return boardCardSettings1;
    }

    private Microsoft.TeamFoundation.Work.WebApi.BoardCardSettings convertFromLegacyBoardCardSettings(
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings boardCardSettings)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings>(boardCardSettings, nameof (boardCardSettings));
      Microsoft.TeamFoundation.Work.WebApi.BoardCardSettings boardCardSettings1 = new Microsoft.TeamFoundation.Work.WebApi.BoardCardSettings();
      boardCardSettings1.Cards = new Dictionary<string, List<Microsoft.TeamFoundation.Work.WebApi.FieldSetting>>();
      foreach (KeyValuePair<string, List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting>> card in boardCardSettings.Cards)
      {
        List<Microsoft.TeamFoundation.Work.WebApi.FieldSetting> fieldSettingList = new List<Microsoft.TeamFoundation.Work.WebApi.FieldSetting>();
        foreach (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.FieldSetting fieldSetting1 in card.Value)
        {
          Microsoft.TeamFoundation.Work.WebApi.FieldSetting fieldSetting2 = new Microsoft.TeamFoundation.Work.WebApi.FieldSetting();
          foreach (KeyValuePair<string, string> keyValuePair in (Dictionary<string, string>) fieldSetting1)
          {
            if (!keyValuePair.Value.IsNullOrEmpty<char>())
              fieldSetting2[keyValuePair.Key] = keyValuePair.Value;
          }
          fieldSettingList.Add(fieldSetting2);
        }
        boardCardSettings1.Cards[card.Key] = fieldSettingList;
      }
      return boardCardSettings1;
    }

    [HttpGet]
    [ClientLocationId("07C3B467-BC60-4F05-8E34-599CE288FAFC")]
    public Microsoft.TeamFoundation.Work.WebApi.BoardCardSettings GetBoardCardSettings(string board)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(board, nameof (board));
      BoardService service = this.TfsRequestContext.GetService<BoardService>();
      Guid boardIdFromNameOrId = this.GetBoardIdFromNameOrId(service, board);
      return this.convertFromLegacyBoardCardSettings(this.GetBoardCardSettings(this.GetBoardBacklogLevelIdByNameOrId(service, boardIdFromNameOrId.ToString()), boardIdFromNameOrId));
    }

    [HttpPut]
    [ClientLocationId("07C3B467-BC60-4F05-8E34-599CE288FAFC")]
    public Microsoft.TeamFoundation.Work.WebApi.BoardCardSettings UpdateBoardCardSettings(
      Microsoft.TeamFoundation.Work.WebApi.BoardCardSettings boardCardSettingsToSave,
      string board)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Work.WebApi.BoardCardSettings>(boardCardSettingsToSave, "boardCardSettings");
      ArgumentUtility.CheckForNull<Dictionary<string, List<Microsoft.TeamFoundation.Work.WebApi.FieldSetting>>>(boardCardSettingsToSave.Cards, "boardCardSettings.Cards");
      ArgumentUtility.CheckStringForNullOrEmpty(board, nameof (board));
      this.CheckBacklogManagementLicense();
      this.CheckAdminPermission();
      BoardService service = this.TfsRequestContext.GetService<BoardService>();
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings boardCardSettings = this.ConvertToLegacyBoardCardSettings(boardCardSettingsToSave, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings.ScopeType.KANBAN, this.GetBoardIdFromNameOrId(service, board));
      Guid boardIdFromNameOrId = this.GetBoardIdFromNameOrId(service, board);
      string levelIdByNameOrId = this.GetBoardBacklogLevelIdByNameOrId(service, boardIdFromNameOrId.ToString());
      return this.convertFromLegacyBoardCardSettings(this.UpdateBoardCardSettings(service, boardCardSettings, levelIdByNameOrId, boardIdFromNameOrId));
    }
  }
}
