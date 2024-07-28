// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Http.TraceFilterWithExceptionAttribute
// Assembly: Microsoft.VisualStudio.Services.Licensing.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F3070F25-7414-49A0-9C00-005379F04A49
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Licensing.Http
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public sealed class TraceFilterWithExceptionAttribute : TraceFilterAttribute
  {
    public TraceFilterWithExceptionAttribute(
      int tracePointEnter,
      int tracePointLeave,
      int tracePointException)
      : base(tracePointEnter, tracePointLeave)
    {
      this.TracePointException = tracePointException;
    }

    public int TracePointException { get; private set; }

    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    {
      Exception exception = actionExecutedContext.Exception;
      if (exception != null)
      {
        HttpActionContext actionContext = actionExecutedContext.ActionContext;
        if (actionContext.ControllerContext.Controller is TfsApiController controller && !controller.BaseHttpExceptions.ContainsKey(exception.GetType()))
        {
          controller.TfsRequestContext.TraceException(this.TracePointException, controller.TraceArea, actionContext.ActionDescriptor.ControllerDescriptor.ControllerName, exception);
          TeamFoundationEventLog.Default.LogException(controller.TfsRequestContext, exception.Message, exception);
        }
      }
      base.OnActionExecuted(actionExecutedContext);
    }
  }
}
