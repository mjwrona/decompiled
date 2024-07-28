// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.TeamAutomationRulesSettingsController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.Work.WebApi.Contracts.AutomationRules;
using Microsoft.TeamFoundation.WorkItemTracking.Server.AutomationRules;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "automationrules", ResourceVersion = 2)]
  [ControllerApiVersion(7.1)]
  [ClientInternalUseOnly(true)]
  public class TeamAutomationRulesSettingsController : TfsTeamApiController
  {
    [HttpPatch]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage UpdateAutomationRule(
      [FromBody] TeamAutomationRulesSettingsRequestModel ruleRequestModel)
    {
      BacklogLevelConfiguration backlogLevel;
      new AgileSettings(this.TfsRequestContext, CommonStructureProjectInfo.ConvertProjectInfo(this.ProjectInfo), this.Team).BacklogConfiguration.TryGetBacklogByName(ruleRequestModel.BacklogLevelName, out backlogLevel);
      if (backlogLevel == null)
        throw new BacklogDoesNotExistException(ruleRequestModel.BacklogLevelName);
      this.TfsRequestContext.GetService<IAutomaitonRulesService>().UpdateWorkItemAutomationRule(this.TfsRequestContext, this.Team, this.ProjectInfo.Id, new TeamAutomationRulesSettings(this.TeamId, backlogLevel.Id, ruleRequestModel.RulesStates));
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }
  }
}
