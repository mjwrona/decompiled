// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.WebPerformanceTimerAttribute
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  public class WebPerformanceTimerAttribute : ActionFilterAttribute
  {
    private const string c_resultExecutedItemsKey = "WebPerfTimings.ResultExecuted";
    private const string c_actionExecutedItemsKey = "WebPerfTimings.ActionExecuted";
    private const string c_actionGroupName = "Controller.Action";
    private const string c_resultGroupName = "Controller.Result";

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      PerformanceTimer? nullable = new PerformanceTimer?(WebPerformanceTimer.StartMeasure(filterContext.RequestContext, "Controller.Action"));
      filterContext.HttpContext.Items[(object) "WebPerfTimings.ActionExecuted"] = (object) nullable;
      base.OnActionExecuting(filterContext);
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      base.OnActionExecuted(filterContext);
      if (!filterContext.HttpContext.Items.Contains((object) "WebPerfTimings.ActionExecuted"))
        return;
      PerformanceTimer? nullable = new PerformanceTimer?((PerformanceTimer) filterContext.HttpContext.Items[(object) "WebPerfTimings.ActionExecuted"]);
      if (!nullable.HasValue)
        return;
      nullable.Value.End();
    }

    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
      PerformanceTimer? nullable = new PerformanceTimer?(WebPerformanceTimer.StartMeasure(filterContext.RequestContext, "Controller.Result"));
      filterContext.HttpContext.Items[(object) "WebPerfTimings.ResultExecuted"] = (object) nullable;
      base.OnResultExecuting(filterContext);
    }

    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {
      base.OnResultExecuted(filterContext);
      if (!filterContext.HttpContext.Items.Contains((object) "WebPerfTimings.ResultExecuted"))
        return;
      PerformanceTimer? nullable = new PerformanceTimer?((PerformanceTimer) filterContext.HttpContext.Items[(object) "WebPerfTimings.ResultExecuted"]);
      if (!nullable.HasValue)
        return;
      nullable.Value.End();
    }

    public static void EndResultExecutedTimer(RequestContext requestContext)
    {
      object obj = requestContext.HttpContext.Items[(object) "WebPerfTimings.ResultExecuted"];
      if (obj == null)
        return;
      ((PerformanceTimer) obj).End();
    }
  }
}
