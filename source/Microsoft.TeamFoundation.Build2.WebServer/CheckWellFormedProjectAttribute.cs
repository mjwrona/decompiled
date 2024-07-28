// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.CheckWellFormedProjectAttribute
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  public class CheckWellFormedProjectAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      TfsProjectApiController controller = actionContext.ControllerContext?.Controller as TfsProjectApiController;
      if (this.Required && controller?.ProjectInfo == null)
        throw new ProjectDoesNotExistException(Resources.ProjectRequired()).Expected("Build2");
      if (controller?.ProjectInfo != null && controller.ProjectInfo.State != ProjectState.WellFormed)
        throw new InvalidProjectException(Resources.ProjectIsNotWellFormed()).Expected("Build2");
      base.OnActionExecuting(actionContext);
    }

    public bool Required { get; set; }
  }
}
