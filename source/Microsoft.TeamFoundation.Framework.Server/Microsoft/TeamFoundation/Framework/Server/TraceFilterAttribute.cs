// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TraceFilterAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TraceFilterAttribute : ActionFilterAttribute
  {
    public TraceFilterAttribute()
    {
    }

    public TraceFilterAttribute(int tracePointEnter, int tracePointLeave)
    {
      this.TracePointEnter = tracePointEnter;
      this.TracePointLeave = tracePointLeave;
    }

    public int TracePointEnter { get; private set; }

    public int TracePointLeave { get; private set; }

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      base.OnActionExecuting(actionContext);
      if (!(actionContext.ControllerContext.Controller is TfsApiController controller))
        return;
      controller.TfsRequestContext.TraceEnter(this.TracePointEnter, controller.TraceArea, actionContext.ActionDescriptor.ControllerDescriptor.ControllerName, actionContext.ActionDescriptor.ActionName);
    }

    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    {
      base.OnActionExecuted(actionExecutedContext);
      if (!(actionExecutedContext.ActionContext.ControllerContext.Controller is TfsApiController controller))
        return;
      controller.TfsRequestContext.TraceLeave(this.TracePointLeave, controller.TraceArea, actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName, actionExecutedContext.ActionContext.ActionDescriptor.ActionName);
    }
  }
}
