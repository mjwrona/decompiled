// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.SignedInTokenController
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "SignedInToken")]
  [ClientInclude(~RestClientLanguages.Swagger2)]
  public class SignedInTokenController : TfsApiController
  {
    private const string s_area = "SignedIn";
    private const string s_layer = "SignedInTokenController";

    [HttpGet]
    [ClientResponseType(typeof (AccessTokenResult), null, null)]
    public HttpResponseMessage GetSignedInToken()
    {
      this.TfsRequestContext.TraceEnter(10002000, "SignedIn", nameof (SignedInTokenController), nameof (GetSignedInToken));
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = this.TfsRequestContext.GetUserIdentity();
        if (!IdentityHelper.IsUserIdentity(this.TfsRequestContext, (IReadOnlyVssIdentity) userIdentity))
        {
          this.TfsRequestContext.Trace(10002015, TraceLevel.Error, "SignedIn", nameof (SignedInTokenController), string.Format("Request to generate OAuth signedin token for service identity {0}", (object) userIdentity.DisplayName));
          return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, (Exception) new IllegalIdentityException(userIdentity.DisplayName));
        }
        IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
        IDelegatedAuthorizationService service = vssRequestContext.GetService<IDelegatedAuthorizationService>();
        JsonWebToken tokenForCurrentUser = service.GenerateImplicitGrantTokenForCurrentUser(vssRequestContext, ResponseType.IdToken);
        return this.Request.CreateResponse<AccessTokenResult>(HttpStatusCode.OK, service.Exchange(vssRequestContext, GrantType.Implicit, tokenForCurrentUser, (JsonWebToken) null));
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(10002009, "SignedIn", nameof (SignedInTokenController), nameof (GetSignedInToken));
      }
    }

    public override string ActivityLogArea => "Identities";
  }
}
