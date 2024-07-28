// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentitySwapController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "Swap")]
  [ClientInternalUseOnly(true)]
  public class IdentitySwapController : IdentitiesControllerBase
  {
    [HttpPost]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage SwapIdentity(SwapIdentityInfo info)
    {
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      IVssSecurityNamespace securityNamespace = vssRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId);
      EvaluationPrincipal evaluationPrincipal = new EvaluationPrincipal(this.TfsRequestContext.GetAuthenticatedDescriptor());
      if (!securityNamespace.QueryableSecurityNamespaceInternal().PrincipalHasPermission(vssRequestContext, evaluationPrincipal, FrameworkSecurity.FrameworkNamespaceToken, 4, securityNamespace.NamespaceExtension.AlwaysAllowAdministrators))
        securityNamespace.ThrowAccessDeniedException(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 4, evaluationPrincipal);
      this.TfsRequestContext.GetService<ISwapIdentityService>().SwapIdentity(this.TfsRequestContext, info.Id1, info.Id2);
      return new HttpResponseMessage()
      {
        StatusCode = HttpStatusCode.OK,
        RequestMessage = this.Request
      };
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) IdentitiesController.s_httpIdentityServiceExceptions;

    public override string TraceArea => "IdentityService";
  }
}
