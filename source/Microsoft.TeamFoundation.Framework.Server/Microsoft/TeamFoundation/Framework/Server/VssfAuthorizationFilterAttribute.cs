// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssfAuthorizationFilterAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class VssfAuthorizationFilterAttribute : AuthorizationFilterAttribute
  {
    public override void OnAuthorization(HttpActionContext actionContext)
    {
      base.OnAuthorization(actionContext);
      IVssRequestContext tfsRequestContext = actionContext.ControllerContext.Controller is TfsApiController controller ? controller.TfsRequestContext : (IVssRequestContext) null;
      if (tfsRequestContext == null || tfsRequestContext.RequestContextInternal().IdentityValidationStatus.HasFlag((Enum) (IdentityValidationStatus.Validated | IdentityValidationStatus.DelayedIdentityValidation)))
        return;
      actionContext.ActionDescriptor.GetCustomAttributes<RequestRestrictionsAttribute>().ApplyRequestRestrictions(tfsRequestContext, actionContext.ControllerContext?.RouteData?.Values);
      IdentityValidationResult validationResult = tfsRequestContext.IsValidIdentity();
      if (validationResult.IsSuccess)
        return;
      actionContext.Response = actionContext.Request.CreateErrorResponse(validationResult.Exception, actionContext.ControllerContext.Controller);
    }
  }
}
