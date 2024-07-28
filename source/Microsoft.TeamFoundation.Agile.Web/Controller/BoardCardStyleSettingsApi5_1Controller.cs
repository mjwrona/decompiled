// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.BoardCardStyleSettingsApi5_1Controller
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Agile.Server.Exceptions;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DataAccess;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "cardrulesettings", ResourceVersion = 2)]
  [ControllerApiVersion(5.1)]
  public class BoardCardStyleSettingsApi5_1Controller : BoardCardStyleSettingsApiController
  {
    [HttpPatch]
    [ClientLocationId("3f84a8d1-1aab-423e-a94b-6dcbdcca511f")]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage UpdateTaskboardCardRuleSettings(
      BoardCardRuleSettings boardCardRuleSettings)
    {
      ArgumentUtility.CheckForNull<BoardCardRuleSettings>(boardCardRuleSettings, nameof (boardCardRuleSettings));
      ArgumentUtility.CheckForNull<Dictionary<string, List<Microsoft.TeamFoundation.Work.WebApi.Rule>>>(boardCardRuleSettings.rules, "rules");
      this.CheckAdminPermission();
      this.CheckBacklogManagementLicense();
      Dictionary<string, BoardCardRules> rowView = this.ConvertCardRulesToRowView(boardCardRuleSettings);
      BoardCardRules boardCardRules = new BoardCardRules();
      List<string> typesToBeDeleted = new List<string>();
      Guid teamId = this.TeamId;
      foreach (string key in rowView.Keys)
      {
        rowView[key].ScopeId = teamId;
        rowView[key].Scope = "TASKBOARD";
        string message = this.CardRulesValidation(rowView[key], key, (IDictionary<Guid, string>) null);
        if (!string.IsNullOrEmpty(message))
        {
          this.TfsRequestContext.Trace(290250, TraceLevel.Error, "Agile", "Controller", "Invalid board card rules data received for taskboard for team {0}. Error message: {1}", (object) teamId.ToString(), (object) message);
          throw new CardRulesValidationFailureException(message);
        }
        boardCardRules.Attributes = boardCardRules.Attributes.Concat<RuleAttributeRow>((IEnumerable<RuleAttributeRow>) rowView[key].Attributes).ToList<RuleAttributeRow>();
        boardCardRules.Rules = boardCardRules.Rules.Concat<BoardCardRuleRow>((IEnumerable<BoardCardRuleRow>) rowView[key].Rules).ToList<BoardCardRuleRow>();
        typesToBeDeleted.Add(key);
      }
      BoardService service = this.TfsRequestContext.GetService<BoardService>();
      boardCardRules.ScopeId = teamId;
      boardCardRules.Scope = "TASKBOARD";
      this.UpdateBoardCardRules(service, boardCardRules, typesToBeDeleted);
      this.TfsRequestContext.Trace(290251, TraceLevel.Verbose, "Agile", "Controller", "Completed updating card rules for taskboard teamId {0}.", (object) teamId);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }
  }
}
