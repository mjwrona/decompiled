// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.ClientRegistrationController
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  [ControllerApiVersion(1.0)]
  [VersionedApiControllerCustomName(Area = "DelegatedAuth", ResourceName = "Registration")]
  public class ClientRegistrationController : DelegatedAuthorizationControllerBase
  {
    public const string removeRegistrationUnusedFieldsFF = "VisualStudio.DelegatedAuthorizationService.ClearRegistrationExcessFields";
    public readonly IDelegatedAuthorizationRegistrationServiceAdapter DelegatedAuthorizationServiceAdapter;
    public readonly Lazy<IDelegatedAuthorizationRegistrationService> DelegatedAuthorizationService;
    private const string Area = "DelegatedAuthorization";
    private const string Layer = "ClientRegistrationController";

    public ClientRegistrationController()
    {
      this.DelegatedAuthorizationService = new Lazy<IDelegatedAuthorizationRegistrationService>((Func<IDelegatedAuthorizationRegistrationService>) (() => this.TfsRequestContext.GetService<IDelegatedAuthorizationRegistrationService>()));
      this.DelegatedAuthorizationServiceAdapter = (IDelegatedAuthorizationRegistrationServiceAdapter) new DelegatedAuthorizationRegistrationServiceAdapter(this.DelegatedAuthorizationService);
    }

    [HttpPut]
    [ClientResponseType(typeof (Registration), null, null)]
    public HttpResponseMessage CreateRegistration(Registration registration)
    {
      try
      {
        return this.Request.CreateResponse<Registration>(HttpStatusCode.OK, !this.TfsRequestContext.IsFeatureEnabled("VisualStudio.DelegatedAuthorizationService.ClearRegistrationExcessFields") ? this.DelegatedAuthorizationService.Value.Create(this.TfsRequestContext, registration) : this.DelegatedAuthorizationServiceAdapter.Create(this.TfsRequestContext, registration));
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteRegistration(Guid registrationId)
    {
      try
      {
        this.DelegatedAuthorizationService.Value.Delete(this.TfsRequestContext, registrationId);
        return this.Request.CreateResponse(HttpStatusCode.OK);
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpPost]
    [ClientResponseType(typeof (Registration), null, null)]
    public HttpResponseMessage UpdateRegistration(Registration registration)
    {
      Guid? registrationId = registration?.RegistrationId;
      Guid empty = Guid.Empty;
      if ((registrationId.HasValue ? (registrationId.HasValue ? (registrationId.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0)
        throw new ArgumentException("Registration details is required.");
      try
      {
        return this.Request.CreateResponse<Registration>(HttpStatusCode.OK, !this.TfsRequestContext.IsFeatureEnabled("VisualStudio.DelegatedAuthorizationService.ClearRegistrationExcessFields") ? this.DelegatedAuthorizationService.Value.Update(this.TfsRequestContext, registration) : this.DelegatedAuthorizationServiceAdapter.Update(this.TfsRequestContext, registration));
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpGet]
    [ClientResponseType(typeof (Registration), null, null)]
    public HttpResponseMessage GetRegistration(Guid registrationId)
    {
      ArgumentUtility.CheckForEmptyGuid(registrationId, nameof (registrationId));
      try
      {
        return this.Request.CreateResponse<Registration>(HttpStatusCode.OK, this.DelegatedAuthorizationService.Value.Get(this.TfsRequestContext, registrationId));
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }

    [HttpGet]
    [ClientResponseType(typeof (List<Registration>), null, null)]
    public HttpResponseMessage GetRegistrations()
    {
      try
      {
        return this.Request.CreateResponse<IList<Registration>>(HttpStatusCode.OK, this.DelegatedAuthorizationService.Value.List(this.TfsRequestContext));
      }
      catch (DelegatedAuthorizationControllerBase.InternalServerErrorException ex)
      {
        return this.Request.CreateResponse<string>(HttpStatusCode.InternalServerError, ex.Message);
      }
    }
  }
}
