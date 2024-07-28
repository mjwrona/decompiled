// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LogExceptionFilterAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class LogExceptionFilterAttribute : ExceptionFilterAttribute
  {
    public override Task OnExceptionAsync(
      HttpActionExecutedContext actionExecutedContext,
      CancellationToken cancellationToken)
    {
      if (actionExecutedContext.ActionContext.ControllerContext.Controller is TfsApiController controller && controller.ExemptFromGlobalExceptionFormatting)
        return Task.CompletedTask;
      if (actionExecutedContext.Exception == null || actionExecutedContext.ActionContext.ControllerContext.Controller == null)
        return base.OnExceptionAsync(actionExecutedContext, cancellationToken);
      this.SetErrorResponseFromException(actionExecutedContext, cancellationToken);
      return Task.CompletedTask;
    }

    protected void SetErrorResponseFromException(
      HttpActionExecutedContext actionExecutedContext,
      CancellationToken cancellationToken)
    {
      Exception finalException;
      HttpResponseMessage errorResponse = actionExecutedContext.Request.CreateErrorResponse(actionExecutedContext.Exception, actionExecutedContext.ActionContext.ControllerContext.Controller, cancellationToken, out finalException);
      actionExecutedContext.Exception = finalException;
      actionExecutedContext.Response = errorResponse;
    }
  }
}
