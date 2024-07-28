// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.ResolveTfsProjectFilterAttribute
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Server.Types
{
  public class ResolveTfsProjectFilterAttribute : AuthorizationFilterAttribute
  {
    public override void OnAuthorization(HttpActionContext actionContext)
    {
      if (actionContext.Request == null || actionContext.ControllerContext?.RouteData?.Values == null)
        return;
      IVssRequestContext requestContext = this.GetRequestContext(actionContext);
      if (requestContext.RequestRestrictions().RequiredAuthentication == RequiredAuthentication.Anonymous)
      {
        HttpActionDescriptor actionDescriptor1 = actionContext.ActionDescriptor;
        if ((actionDescriptor1 != null ? (actionDescriptor1.GetCustomAttributes<AllowAnonymousProjectLevelRequestsAttribute>().Any<AllowAnonymousProjectLevelRequestsAttribute>() ? 1 : 0) : 0) == 0)
        {
          HttpActionDescriptor actionDescriptor2 = actionContext.ActionDescriptor;
          int num;
          if (actionDescriptor2 == null)
          {
            num = 0;
          }
          else
          {
            HttpControllerDescriptor controllerDescriptor = actionDescriptor2.ControllerDescriptor;
            bool? nullable = controllerDescriptor != null ? new bool?(controllerDescriptor.GetCustomAttributes<AllowAnonymousProjectLevelRequestsAttribute>(true).Any<AllowAnonymousProjectLevelRequestsAttribute>()) : new bool?();
            bool flag = true;
            num = nullable.GetValueOrDefault() == flag & nullable.HasValue ? 1 : 0;
          }
          if (num == 0)
            goto label_8;
        }
        requestContext = requestContext.Elevate();
      }
label_8:
      ProjectInfo projectFromRoute = ProjectUtility.GetProjectFromRoute(requestContext, actionContext.ControllerContext.RouteData.Values);
      if (projectFromRoute == null || !(actionContext.ControllerContext.Controller is ITfsProjectApiController controller))
        return;
      controller.ProjectInfo = projectFromRoute;
    }

    protected IVssRequestContext GetRequestContext(HttpActionContext actionContext) => (IVssRequestContext) ((HttpContextBase) actionContext.Request.Properties[TfsApiPropertyKeys.HttpContext]).Items[(object) HttpContextConstants.IVssRequestContext];
  }
}
