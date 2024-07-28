// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PutBlockWriteOperationsOnGETOnRequestContextAttribute
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  internal class PutBlockWriteOperationsOnGETOnRequestContextAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      if (!(actionContext?.ControllerContext?.Controller is TfsApiController controller))
        return;
      IVssRequestContext tfsRequestContext = controller.TfsRequestContext;
      if (new NonBrowserGetCsrfValidatorBootstrapper(tfsRequestContext, actionContext.Request).Bootstrap().Get())
        return;
      tfsRequestContext.Items["Packaging.BlockWriteOperationOnGetRequest"] = (object) true;
    }
  }
}
