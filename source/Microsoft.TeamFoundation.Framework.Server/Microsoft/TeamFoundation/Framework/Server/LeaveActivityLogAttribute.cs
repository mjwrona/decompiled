// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LeaveActivityLogAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class LeaveActivityLogAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    {
      base.OnActionExecuted(actionExecutedContext);
      if (!(actionExecutedContext.ActionContext.ControllerContext.Controller is TfsApiController controller))
        return;
      try
      {
        if (controller.TfsRequestContext.Status == null && actionExecutedContext.Exception != null)
          controller.TfsRequestContext.Status = actionExecutedContext.Exception;
        ApiTelemetryService service = controller.TfsRequestContext.To(TeamFoundationHostType.Deployment).GetService<ApiTelemetryService>();
        CustomerIntelligenceData ciData;
        if (!service.ShouldLog(controller.TfsRequestContext, actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerType) || !actionExecutedContext.ActionContext.RequestContext.RouteData.Values.TryGetValue<CustomerIntelligenceData>("apiCIData", out ciData) || actionExecutedContext.Response == null)
          return;
        service.AddResponseCode(controller.TfsRequestContext, ciData, (int) actionExecutedContext.Response.StatusCode);
        service.PublishTelemetry(controller.TfsRequestContext, ciData);
      }
      finally
      {
        controller.TfsRequestContext.LeaveMethod();
      }
    }
  }
}
