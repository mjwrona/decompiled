// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.AppTokenPairsController
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "Token", ResourceName = "AppTokenPairs")]
  public class AppTokenPairsController : DelegatedAuthorizationControllerBase
  {
    private const string Area = "DelegatedAuthorization";
    private const string Layer = "AppTokenPairsController";

    [HttpPost]
    [ClientResponseType(typeof (AccessTokenResult), null, null)]
    public HttpResponseMessage ExchangeAppToken(AppTokenSecretPair appInfo)
    {
      ArgumentUtility.CheckForNull<AppTokenSecretPair>(appInfo, "app");
      ArgumentUtility.CheckStringForNullOrEmpty(appInfo.AppToken, "appToken");
      ArgumentUtility.CheckStringForNullOrEmpty(appInfo.ClientSecret, "clientSecret");
      JsonWebToken jwtToken1 = IdentityHelper.GetJwtToken(this.TfsRequestContext, appInfo.AppToken);
      if (jwtToken1 == null)
      {
        this.TfsRequestContext.Trace(1050026, TraceLevel.Error, "DelegatedAuthorization", nameof (AppTokenPairsController), "AppToken is not in correct format.");
        throw new ArgumentException();
      }
      JsonWebToken jwtToken2 = IdentityHelper.GetJwtToken(this.TfsRequestContext, appInfo.ClientSecret);
      if (jwtToken2 == null)
      {
        this.TfsRequestContext.Trace(1050028, TraceLevel.Error, "DelegatedAuthorization", nameof (AppTokenPairsController), "clientSecret is not in correct format.");
        throw new ArgumentException();
      }
      try
      {
        IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
        AccessTokenResult accessTokenResult = vssRequestContext.GetService<IDelegatedAuthorizationService>().ExchangeAppToken(vssRequestContext, jwtToken1, jwtToken2);
        if (accessTokenResult.HasError)
        {
          this.TfsRequestContext.Trace(1050029, TraceLevel.Error, "DelegatedAuthorization", nameof (AppTokenPairsController), string.Format("{0} - error creating ExchangeAppToken.", (object) accessTokenResult.AccessTokenError));
          throw new ExchangeAppTokenCreateException(accessTokenResult.AccessTokenError.ToString());
        }
        return this.Request.CreateResponse<AccessTokenResult>(HttpStatusCode.OK, accessTokenResult);
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }
  }
}
