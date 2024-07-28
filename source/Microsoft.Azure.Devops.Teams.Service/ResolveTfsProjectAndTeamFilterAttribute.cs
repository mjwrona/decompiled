// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Teams.Service.ResolveTfsProjectAndTeamFilterAttribute
// Assembly: Microsoft.Azure.Devops.Teams.Service, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 643BB522-27FA-4CE7-8A1E-31D95F8AC409
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Teams.Service.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Server.Types;
using System.Web.Http.Controllers;

namespace Microsoft.Azure.Devops.Teams.Service
{
  public class ResolveTfsProjectAndTeamFilterAttribute : ResolveTfsProjectFilterAttribute
  {
    public bool RequireExplicitTeam { get; set; }

    public ResolveTfsProjectAndTeamFilterAttribute() => this.RequireExplicitTeam = false;

    public override void OnAuthorization(HttpActionContext actionContext)
    {
      base.OnAuthorization(actionContext);
      if (actionContext == null || actionContext.ControllerContext == null || actionContext.ControllerContext.Controller == null || actionContext.Request == null || !(actionContext.ControllerContext.Controller is ITfsTeamApiController controller))
        return;
      ProjectInfo projectInfo = controller.ProjectInfo;
      if (projectInfo == null)
        return;
      WebApiTeam teamFromRequest = TeamsUtility.GetTeamFromRequest(this.GetRequestContext(actionContext), actionContext.ControllerContext, projectInfo, this.RequireExplicitTeam);
      controller.Team = teamFromRequest;
    }
  }
}
