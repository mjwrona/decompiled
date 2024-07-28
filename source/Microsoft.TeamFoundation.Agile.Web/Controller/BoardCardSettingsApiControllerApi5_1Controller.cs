// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.BoardCardSettingsApiControllerApi5_1Controller
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "cardsettings", ResourceVersion = 2)]
  [ControllerApiVersion(5.1)]
  public class BoardCardSettingsApiControllerApi5_1Controller : BoardCardSettingsApiController
  {
    [HttpPut]
    [ClientLocationId("0d63745f-31f3-4cf3-9056-2a064e567637")]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage UpdateTaskboardCardSettings(Microsoft.TeamFoundation.Work.WebApi.BoardCardSettings boardCardSettingsToSave)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Work.WebApi.BoardCardSettings>(boardCardSettingsToSave, nameof (boardCardSettingsToSave));
      ArgumentUtility.CheckForNull<Dictionary<string, List<Microsoft.TeamFoundation.Work.WebApi.FieldSetting>>>(boardCardSettingsToSave.Cards, "Cards");
      this.CheckAdminPermission();
      this.CheckBacklogManagementLicense();
      Guid teamId = this.TeamId;
      BoardService service = this.TfsRequestContext.GetService<BoardService>();
      this.TfsRequestContext.Trace(290954, TraceLevel.Verbose, "Agile", "Controller", "Updating card settings for taskboard for team {0} with settings : {1}", (object) teamId, (object) boardCardSettingsToSave.ToString());
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings boardCardSettings = this.ConvertToLegacyBoardCardSettings(boardCardSettingsToSave, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettings.ScopeType.TASKBOARD, teamId);
      this.UpdateTaskboardCardSettings(service, boardCardSettings);
      this.TfsRequestContext.Trace(290101, TraceLevel.Verbose, "Agile", "Controller", "Completed updating card settings for taskboard for teamId {0}.", (object) teamId);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }
  }
}
