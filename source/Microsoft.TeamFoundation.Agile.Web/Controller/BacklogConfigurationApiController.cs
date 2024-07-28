// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.BacklogConfigurationApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "backlogconfiguration")]
  [ResolveTfsProjectAndTeamFilter(RequireExplicitTeam = true)]
  public class BacklogConfigurationApiController : TfsTeamApiController
  {
    [HttpGet]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.Work.WebApi.BacklogConfiguration), null, null)]
    [ClientExample("GET__work_backlogconfiguration.json", "Get backlog configuration for a team", null, null)]
    public HttpResponseMessage GetBacklogConfigurations()
    {
      IBacklogConfigurationService service = this.TfsRequestContext.GetService<IBacklogConfigurationService>();
      return this.Request.CreateResponse<Microsoft.TeamFoundation.Work.WebApi.BacklogConfiguration>(HttpStatusCode.OK, (this.Team != null ? new AgileSettings(this.TfsRequestContext, CommonStructureProjectInfo.ConvertProjectInfo(this.ProjectInfo), this.Team).BacklogConfiguration : service.GetProjectBacklogConfiguration(this.TfsRequestContext, this.ProjectId)).Convert(this.TfsRequestContext, this.ProjectId, this.TeamId));
    }
  }
}
