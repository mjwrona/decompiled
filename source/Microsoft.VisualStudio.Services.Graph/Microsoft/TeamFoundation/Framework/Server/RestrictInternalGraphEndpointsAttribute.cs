// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RestrictInternalGraphEndpointsAttribute
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class RestrictInternalGraphEndpointsAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      if (!(actionContext.ControllerContext.Controller is TfsApiController controller))
        return;
      IVssRequestContext tfsRequestContext = controller.TfsRequestContext;
      if (!ServicePrincipals.IsServicePrincipal(tfsRequestContext, tfsRequestContext.GetAuthenticatedDescriptor()))
        throw new VssResourceNotFoundException("Resource not found");
    }
  }
}
