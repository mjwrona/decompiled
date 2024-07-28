// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.RequireTeamAttribute
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class RequireTeamAttribute : RequireProjectAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      using (WebPerformanceTimer.StartMeasure(filterContext.RequestContext, "Controller.Attributes.RequireTeam"))
      {
        base.OnActionExecuting(filterContext);
        if (filterContext.Controller is TfsController controller && controller.TfsRequestContext.GetWebTeamContext().Team == null)
          throw new TeamFoundationServiceException("Team is required");
      }
    }
  }
}
