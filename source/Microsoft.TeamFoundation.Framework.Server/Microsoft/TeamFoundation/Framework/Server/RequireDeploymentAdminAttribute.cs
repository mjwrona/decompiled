// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RequireDeploymentAdminAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Diagnostics;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class RequireDeploymentAdminAttribute : ActionFilterAttribute
  {
    private static readonly string s_area = "WebApiFilterAttribute";
    private static readonly string s_layer = nameof (RequireDeploymentAdminAttribute);

    public override void OnActionExecuting(HttpActionContext filterContext)
    {
      base.OnActionExecuting(filterContext);
      if (filterContext.ControllerContext.Controller is TfsApiController controller)
      {
        if (!this.IsDeploymentAdmin(controller.TfsRequestContext))
          throw new InvalidOperationException("User does not have permission to execute this operation.");
      }
      else
      {
        TeamFoundationTracingService.TraceRaw(10012050, TraceLevel.Error, RequireDeploymentAdminAttribute.s_area, RequireDeploymentAdminAttribute.s_layer, "No TfsApiController associated with filter context. Defaulting to invalid operation response.");
        throw new InvalidOperationException("Invalid operation. No TfsApiController associated with filter context.");
      }
    }

    protected bool IsDeploymentAdmin(IVssRequestContext TfsRequestContext)
    {
      IVssRequestContext vssRequestContext = TfsRequestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 2, false);
    }
  }
}
