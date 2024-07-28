// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsTraceFilterAttribute
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class TfsTraceFilterAttribute : ActionFilterAttribute
  {
    public TfsTraceFilterAttribute()
    {
    }

    public TfsTraceFilterAttribute(int tracePointEnter, int tracePointLeave)
    {
      this.TracePointEnter = tracePointEnter;
      this.TracePointLeave = tracePointLeave;
    }

    public int TracePointEnter { get; set; }

    public int TracePointLeave { get; set; }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      if (filterContext.Controller is IPlatformController controller)
        controller.TfsRequestContext.TraceEnter(this.TracePointEnter, controller.TraceArea, filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName);
      base.OnActionExecuting(filterContext);
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      base.OnActionExecuted(filterContext);
      if (!(filterContext.Controller is IPlatformController controller))
        return;
      controller.TfsRequestContext.TraceLeave(this.TracePointLeave, controller.TraceArea, filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, filterContext.ActionDescriptor.ActionName);
    }

    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
      if (filterContext.Controller is IPlatformController controller)
        controller.TfsRequestContext.TraceEnter(this.TracePointEnter, controller.TraceArea, TfsTraceLayers.Framework, "ResultExecution");
      base.OnResultExecuting(filterContext);
    }

    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {
      base.OnResultExecuted(filterContext);
      if (!(filterContext.Controller is IPlatformController controller))
        return;
      controller.TfsRequestContext.TraceLeave(this.TracePointEnter, controller.TraceArea, TfsTraceLayers.Framework, "ResultExecution");
    }
  }
}
