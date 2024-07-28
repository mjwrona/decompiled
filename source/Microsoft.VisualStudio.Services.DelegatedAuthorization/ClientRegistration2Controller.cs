// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.ClientRegistration2Controller
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "DelegatedAuth", ResourceName = "Registration", ResourceVersion = 2)]
  public class ClientRegistration2Controller : ClientRegistrationController
  {
    private const string Area = "DelegatedAuthorization";
    private const string Layer = "ClientRegistration2Controller";

    [HttpPut]
    [ClientResponseType(typeof (Registration), null, null)]
    public HttpResponseMessage CreateRegistration(Registration registration, bool includeSecret)
    {
      try
      {
        return this.Request.CreateResponse<Registration>(HttpStatusCode.OK, !this.TfsRequestContext.IsFeatureEnabled("VisualStudio.DelegatedAuthorizationService.ClearRegistrationExcessFields") ? this.DelegatedAuthorizationService.Value.Create(this.TfsRequestContext, registration, includeSecret) : this.DelegatedAuthorizationServiceAdapter.Create(this.TfsRequestContext, registration, includeSecret));
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpGet]
    [ClientResponseType(typeof (Registration), null, null)]
    public HttpResponseMessage GetRegistration(Guid registrationId, bool includeSecret)
    {
      ArgumentUtility.CheckForEmptyGuid(registrationId, nameof (registrationId));
      try
      {
        return this.Request.CreateResponse<Registration>(HttpStatusCode.OK, this.DelegatedAuthorizationService.Value.Get(this.TfsRequestContext, registrationId, includeSecret));
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpPost]
    [ClientResponseType(typeof (Registration), null, null)]
    public HttpResponseMessage UpdateRegistration(Registration registration, bool includeSecret)
    {
      Guid? registrationId = registration?.RegistrationId;
      Guid empty = Guid.Empty;
      if ((registrationId.HasValue ? (registrationId.HasValue ? (registrationId.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0)
        throw new ArgumentException("Registration details is required.");
      try
      {
        return this.Request.CreateResponse<Registration>(HttpStatusCode.OK, !this.TfsRequestContext.IsFeatureEnabled("VisualStudio.DelegatedAuthorizationService.ClearRegistrationExcessFields") ? this.DelegatedAuthorizationService.Value.Update(this.TfsRequestContext, registration, includeSecret) : this.DelegatedAuthorizationServiceAdapter.Update(this.TfsRequestContext, registration, includeSecret));
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }
  }
}
