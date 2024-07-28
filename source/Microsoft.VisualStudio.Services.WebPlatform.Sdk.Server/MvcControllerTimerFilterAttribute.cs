// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.MvcControllerTimerFilterAttribute
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  public class MvcControllerTimerFilterAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      base.OnActionExecuting(filterContext);
      (filterContext.Controller is IPlatformController controller ? controller.TfsRequestContext : (IVssRequestContext) null)?.RequestTimer.SetPreControllerTime();
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      (filterContext.Controller is IPlatformController controller ? controller.TfsRequestContext : (IVssRequestContext) null)?.RequestTimer.SetControllerTime();
      base.OnActionExecuted(filterContext);
    }
  }
}
