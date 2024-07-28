// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.RegistrationSecretController
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "DelegatedAuth", ResourceName = "RegistrationSecret")]
  public class RegistrationSecretController : DelegatedAuthorizationControllerBase
  {
    private const string Area = "DelegatedAuthorization";
    private const string Layer = "RegistrationSecretController";

    [HttpGet]
    [ClientResponseType(typeof (JsonWebToken), null, null)]
    public HttpResponseMessage GetSecret(Guid registrationId)
    {
      try
      {
        return this.Request.CreateResponse<JsonWebToken>(HttpStatusCode.OK, this.TfsRequestContext.GetService<PlatformDelegatedAuthorizationRegistrationService>().GetSecret(this.TfsRequestContext, registrationId));
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpPut]
    [ClientResponseType(typeof (Registration), null, null)]
    public HttpResponseMessage RotateSecret(Guid registrationId) => this.Request.CreateResponse<Registration>(HttpStatusCode.OK, this.TfsRequestContext.GetService<PlatformDelegatedAuthorizationRegistrationService>().RotateSecret(this.TfsRequestContext, registrationId));
  }
}
