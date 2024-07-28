// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ContributedPageController
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System.Web.Mvc;
using System.Web.UI;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  [OutputCache(Duration = 0, VaryByParam = "None", Location = OutputCacheLocation.Any, NoStore = true)]
  public class ContributedPageController : WebSdkMvcControllerBase
  {
    public override string TraceArea => "WebApi";

    [PublicPageRequestRestrictions]
    public ActionResult Execute() => (ActionResult) new ContributedPageActionResult(this.TfsRequestContext);

    protected override void OnActionExecuting(ActionExecutingContext ctx)
    {
      base.OnActionExecuting(ctx);
      using (PerformanceTimer.StartMeasure(this.TfsRequestContext, "PreRequestHandlers"))
      {
        foreach (ContributedRequestHandler<IPreExecuteContributedRequestHandler> requestHandler in this.TfsRequestContext.GetService<IContributionRoutingService>().GetRequestHandlers<IPreExecuteContributedRequestHandler>(this.TfsRequestContext))
        {
          using (PerformanceTimer.StartMeasure(this.TfsRequestContext, "PreRequestHandler", requestHandler.Id))
            requestHandler.Handler.OnPreExecute(this.TfsRequestContext, ctx, requestHandler.Properties);
          if (ctx.Result != null)
            break;
        }
      }
    }

    protected override void OnResultExecuted(ResultExecutedContext filterContext)
    {
      base.OnResultExecuted(filterContext);
      using (PerformanceTimer.StartMeasure(this.TfsRequestContext, "PostRequestHandlers"))
      {
        foreach (ContributedRequestHandler<IPostExecuteContributedRequestHandler> requestHandler in this.TfsRequestContext.GetService<IContributionRoutingService>().GetRequestHandlers<IPostExecuteContributedRequestHandler>(this.TfsRequestContext))
        {
          using (PerformanceTimer.StartMeasure(this.TfsRequestContext, "PostRequestHandler", requestHandler.Id))
            requestHandler.Handler.OnPostExecute(this.TfsRequestContext, filterContext, requestHandler.Properties);
        }
      }
      PerformanceTimer.SendCustomerIntelligenceData(this.TfsRequestContext);
    }
  }
}
