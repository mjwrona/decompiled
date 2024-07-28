// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ValidateRequestUrlSignatureFilterAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ValidateRequestUrlSignatureFilterAttribute : AuthorizationFilterAttribute
  {
    public override void OnAuthorization(HttpActionContext actionContext)
    {
      ArgumentUtility.CheckForNull<HttpActionContext>(actionContext, nameof (actionContext));
      ArgumentUtility.CheckForNull<HttpRequestMessage>(actionContext.Request, "Request");
      IVssWebRequestContext webRequestContext = this.GetWebRequestContext(actionContext);
      UrlValidationResult validationResult = webRequestContext.GetService<IUrlSigningService>().Validate((IVssRequestContext) webRequestContext, webRequestContext.RequestUri);
      if (validationResult.IsValid)
        webRequestContext.Items[RequestContextItemsKeys.RequestUriSignatureValidated] = (object) true;
      else
        actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, validationResult.Error);
    }

    protected IVssWebRequestContext GetWebRequestContext(HttpActionContext actionContext) => (IVssRequestContext) ((HttpContextBase) actionContext.Request.Properties[TfsApiPropertyKeys.HttpContext]).Items[(object) HttpContextConstants.IVssRequestContext] as IVssWebRequestContext;
  }
}
