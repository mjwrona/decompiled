// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.SetXFrameOptionsAttribute
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using Microsoft.TeamFoundation.Server.WebAccess.Platform;
using Microsoft.VisualStudio.Services.Identity;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public class SetXFrameOptionsAttribute : ActionFilterAttribute
  {
    private const string c_frameOptionsDefault = "SAMEORIGIN";
    private const string c_frameOptionsHeader = "X-FRAME-OPTIONS";

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      base.OnActionExecuting(filterContext);
      if (!(filterContext.Controller is WebPlatformAreaController controller) || !controller.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      bool flag = controller.TfsRequestContext.UserContext != (IdentityDescriptor) null;
      if (!(!filterContext.HttpContext.Response.HeadersWritten & flag))
        return;
      filterContext.HttpContext.Response.Headers.Set("X-FRAME-OPTIONS", "SAMEORIGIN");
    }
  }
}
