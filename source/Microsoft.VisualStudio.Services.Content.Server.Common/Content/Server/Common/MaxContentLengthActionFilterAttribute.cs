// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.MaxContentLengthActionFilterAttribute
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class MaxContentLengthActionFilterAttribute : ActionFilterAttribute
  {
    private readonly string registryRootPath;
    private readonly long defaultMaxRequestContentLength;

    public MaxContentLengthActionFilterAttribute(
      string maxRequestContentLengthRegistryPath,
      long defaultMaxRequestContentLength)
    {
      this.registryRootPath = maxRequestContentLengthRegistryPath;
      this.defaultMaxRequestContentLength = defaultMaxRequestContentLength;
    }

    public override void OnActionExecuting(HttpActionContext actionContext) => this.AssertMaxRequestContentLength(actionContext);

    public override Task OnActionExecutingAsync(
      HttpActionContext actionContext,
      CancellationToken cancellationToken)
    {
      this.AssertMaxRequestContentLength(actionContext);
      return base.OnActionExecutingAsync(actionContext, cancellationToken);
    }

    private void AssertMaxRequestContentLength(HttpActionContext actionContext)
    {
      TfsApiController controller = (TfsApiController) actionContext.ControllerContext.Controller;
      long? contentLength = actionContext.Request.Content.Headers.ContentLength;
      if (!contentLength.HasValue)
        return;
      string maxRequestContentLengthRegistryPath = this.registryRootPath + "/MaxRequestContentLength";
      ContentLengthThrottleHelper.AssertMaxRequestContentLength(controller.TfsRequestContext, maxRequestContentLengthRegistryPath, contentLength.Value, true, this.defaultMaxRequestContentLength);
    }
  }
}
