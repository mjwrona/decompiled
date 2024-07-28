// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.RestrictDeploymentIdentitiesAccessAttribute
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class RestrictDeploymentIdentitiesAccessAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      base.OnActionExecuting(actionContext);
      if (!(actionContext.ControllerContext.Controller is TfsApiController controller))
        throw new InvalidOperationException("The RestrictDeploymentIdentitiesAccess attribute is only valid on subclasses of TfsApiController.");
      IVssRequestContext tfsRequestContext = controller.TfsRequestContext;
      if (!tfsRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return;
      IdentitiesControllerBase.DeploymentAccessChecker.CheckDeploymentAccess(tfsRequestContext);
    }
  }
}
