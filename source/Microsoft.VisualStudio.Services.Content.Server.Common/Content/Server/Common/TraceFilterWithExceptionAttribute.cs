// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.TraceFilterWithExceptionAttribute
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public class TraceFilterWithExceptionAttribute : TraceFilterAttribute
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
