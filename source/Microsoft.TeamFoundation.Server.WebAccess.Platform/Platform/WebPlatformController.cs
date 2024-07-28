// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Platform.WebPlatformController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Platform
{
  [VssfMvcAuthorizationFilter]
  [TfsActivityLogFilter]
  [TfsAntiForgeryValidation]
  [CdnFallback]
  [SecurityHeaders(false, true, true)]
  [MvcControllerTimerFilter]
  [WebPerformanceTimer(Order = 100)]
  public abstract class WebPlatformController : Controller, IPlatformController
  {
    public IVssRequestContext TfsRequestContext { get; private set; }

    public virtual string TraceArea => "WebPlatform";

    protected override void Initialize(RequestContext requestContext)
    {
      base.Initialize(requestContext);
      this.TfsRequestContext = requestContext.HttpContext.Items[(object) "IVssRequestContext"] as IVssRequestContext;
      if (this.TfsRequestContext == null)
        return;
      this.TfsRequestContext.Items["webRequestContext"] = (object) requestContext;
    }

    public void DoInitialize(RequestContext requestContext) => this.Initialize(requestContext);

    public virtual string GetActivityLogCommandPrefix() => (string) null;

    public void EnterMethod(MethodInformation methodInformation)
    {
      if (string.IsNullOrEmpty(this.TfsRequestContext.ServiceName))
        this.TfsRequestContext.ServiceName = "Web Platform";
      this.TfsRequestContext.EnterMethod(methodInformation);
    }

    protected override void OnActionExecuting(ActionExecutingContext ctx)
    {
      base.OnActionExecuting(ctx);
      using (WebPerformanceTimer.StartMeasure(ctx.RequestContext, "WebPlatformController.SetPreferredCulture"))
        TeamFoundationApplicationCore.SetPreferredCulture(this.HttpContext, this.TfsRequestContext);
    }

    protected override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      base.OnActionExecuted(filterContext);
      if (this.TfsRequestContext == null)
        return;
      WebContext webContext = WebContextFactory.GetWebContext(this.TfsRequestContext, false);
      if (webContext == null)
        return;
      webContext.NavigationContext.CommandName = this.TfsRequestContext.Method?.Name;
    }
  }
}
