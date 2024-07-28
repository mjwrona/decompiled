// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.WebSdkMvcControllerBase
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  [VssfMvcAuthorizationFilter]
  [TfsActivityLogFilter]
  [SecurityHeaders(true, true, true)]
  [CdnFallback]
  [MvcControllerTimerFilter]
  [WebPerformanceTimer(Order = 100)]
  public abstract class WebSdkMvcControllerBase : Controller, IPlatformController
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
      using (WebPerformanceTimer.StartMeasure(ctx.RequestContext, "WebSdkMvcControllerBase.SetPreferredCulture"))
        TeamFoundationApplicationCore.SetPreferredCulture(this.HttpContext, this.TfsRequestContext);
    }
  }
}
