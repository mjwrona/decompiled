// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ServerSideDedup.ServerSideDedupAttribute
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.ServerSideDedup
{
  public class ServerSideDedupAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      if (!this.IsUserAllowed(actionContext.Request.GetIVssRequestContext()))
        throw new UnauthorizedBlobstoreRequestException(Microsoft.VisualStudio.Services.BlobStore.Server.Resources.DirectAccessForbidden());
      base.OnActionExecuting(actionContext);
    }

    public override Task OnActionExecutingAsync(
      HttpActionContext actionContext,
      CancellationToken cancellationToken)
    {
      return this.IsUserAllowed(actionContext.Request.GetIVssRequestContext()) ? base.OnActionExecutingAsync(actionContext, cancellationToken) : throw new UnauthorizedBlobstoreRequestException(Microsoft.VisualStudio.Services.BlobStore.Server.Resources.DirectAccessForbidden());
    }

    public bool IsUserAllowed(IVssRequestContext requestContext) => ServicePrincipals.IsServicePrincipal(requestContext, requestContext.UserContext) || requestContext.IsFeatureEnabled("Blobstore.Features.ServerDedupAllowDirectCalls");
  }
}
