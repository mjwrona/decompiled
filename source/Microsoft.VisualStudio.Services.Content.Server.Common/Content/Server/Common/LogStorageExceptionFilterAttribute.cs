// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.LogStorageExceptionFilterAttribute
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  internal class LogStorageExceptionFilterAttribute : ExceptionFilterAttribute
  {
    public override Task OnExceptionAsync(
      HttpActionExecutedContext actionExecutedContext,
      CancellationToken cancellationToken)
    {
      if (actionExecutedContext.Exception == null || actionExecutedContext.ActionContext.ControllerContext.Controller == null)
        return base.OnExceptionAsync(actionExecutedContext, cancellationToken);
      Exception finalException = actionExecutedContext.Exception;
      HttpResponseMessage errorResponse;
      if (actionExecutedContext.Exception is IExceptionToStatusCodeMapper exception)
      {
        errorResponse = actionExecutedContext.Request.CreateErrorResponse(exception.MapToStatusCode(), finalException, actionExecutedContext.ActionContext.ControllerContext.Controller);
        finalException = exception.WrapInException();
      }
      else
        errorResponse = actionExecutedContext.Request.CreateErrorResponse(actionExecutedContext.Exception, actionExecutedContext.ActionContext.ControllerContext.Controller, cancellationToken, out finalException);
      actionExecutedContext.Exception = finalException;
      actionExecutedContext.Response = errorResponse;
      return Task.CompletedTask;
    }
  }
}
