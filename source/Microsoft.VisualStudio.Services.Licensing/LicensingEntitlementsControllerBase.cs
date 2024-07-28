// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingEntitlementsControllerBase
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Licensing.Http;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public abstract class LicensingEntitlementsControllerBase : LicensingApiController
  {
    protected readonly ServiceFactory<ILicensingEntitlementService> m_accountEntitlementServiceFactory;
    protected static readonly Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>()
    {
      {
        typeof (LicenseNotAvailableException),
        HttpStatusCode.Forbidden
      },
      {
        typeof (InvalidLicensingOperation),
        HttpStatusCode.BadRequest
      },
      {
        typeof (InvalidQueryStringException),
        HttpStatusCode.BadRequest
      }
    };

    public LicensingEntitlementsControllerBase()
      : this((ServiceFactory<ILicensingEntitlementService>) (x => x.GetService<ILicensingEntitlementService>()))
    {
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) LicensingEntitlementsControllerBase.s_httpExceptions;

    [HttpGet]
    [ActionName("AllUsers")]
    [TraceFilterWithException(1039400, 1039409, 1039408)]
    [ClientLocationId("EA37BE6F-8CD7-48DD-983D-2B72D6E3DA0F")]
    public IEnumerable<AccountEntitlement> GetAccountEntitlements() => this.GetAccountEntitlementsInternal();

    [HttpGet]
    [ActionName("AllUsers")]
    [TraceFilterWithException(1039470, 1039478, 1039479)]
    [ClientLocationId("EA37BE6F-8CD7-48DD-983D-2B72D6E3DA0F")]
    public IEnumerable<AccountEntitlement> GetAccountEntitlements(int top, int skip = 0) => this.GetAccountEntitlementsInternal(new int?(top), skip);

    private IEnumerable<AccountEntitlement> GetAccountEntitlementsInternal(int? top = null, int skip = 0)
    {
      ILicensingEntitlementService entitlementService = this.m_accountEntitlementServiceFactory(this.TfsRequestContext);
      return (IEnumerable<AccountEntitlement>) (top.HasValue ? (IEnumerable<AccountEntitlement>) entitlementService.GetAccountEntitlements(this.TfsRequestContext, top.Value, skip) : (IEnumerable<AccountEntitlement>) entitlementService.GetAccountEntitlements(this.TfsRequestContext)).WithIdentityRefs(this.TfsRequestContext);
    }

    [HttpGet]
    [ActionName("CurrentUser")]
    [TraceFilterWithException(1039450, 1039459, 1039458)]
    [ClientLocationId("C01E9FD5-0D8C-4D5E-9A68-734BD8DA6A38")]
    [ClientResponseType(typeof (AccountEntitlement), null, null)]
    public HttpResponseMessage GetAccountEntitlement()
    {
      AccountEntitlement accountEntitlement = this.m_accountEntitlementServiceFactory(this.TfsRequestContext).GetAccountEntitlement(this.TfsRequestContext, this.TfsRequestContext.GetUserId()).WithIdentityRef(this.TfsRequestContext);
      return !(accountEntitlement != (AccountEntitlement) null) ? this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "No valid licenses were found for user") : this.Request.CreateResponse<AccountEntitlement>(HttpStatusCode.OK, accountEntitlement);
    }

    [HttpGet]
    [ActionName("user")]
    [TraceFilterWithException(1039430, 1039439, 1039438)]
    [ClientLocationId("6490E566-B299-49A7-A4E4-28749752581F")]
    [ClientResponseType(typeof (AccountEntitlement), null, null)]
    public HttpResponseMessage GetAccountEntitlementForUser(
      Guid userId,
      bool determineRights = true,
      bool createIfNotExists = true)
    {
      ValidationHelper.ValidateCollectionLevelStorageKey(this.TfsRequestContext, userId, determineRights);
      AccountEntitlement accountEntitlement = this.m_accountEntitlementServiceFactory(this.TfsRequestContext).GetAccountEntitlement(this.TfsRequestContext, userId, determineRights: determineRights, createIfNotExists: createIfNotExists).WithIdentityRef(this.TfsRequestContext);
      return !(accountEntitlement != (AccountEntitlement) null) ? this.Request.CreateErrorResponse(HttpStatusCode.NotFound, "No valid licenses were found for user") : this.Request.CreateResponse<AccountEntitlement>(HttpStatusCode.OK, accountEntitlement);
    }

    [HttpPut]
    [ActionName("user")]
    [TraceFilterWithException(1039420, 1039429, 1039428)]
    [ClientLocationId("6490E566-B299-49A7-A4E4-28749752581F")]
    [ClientResponseType(typeof (AccountEntitlement), null, null)]
    public HttpResponseMessage AssignAccountEntitlementForUser(
      Guid userId,
      [FromBody] AccountEntitlementUpdateModel body,
      LicensingOrigin origin = LicensingOrigin.None)
    {
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      ArgumentUtility.CheckForNull<AccountEntitlementUpdateModel>(body, nameof (body));
      ValidationHelper.ValidateCollectionLevelStorageKey(this.TfsRequestContext, userId, true);
      AccountEntitlement accountEntitlement = this.m_accountEntitlementServiceFactory(this.TfsRequestContext).AssignAccountEntitlement(this.TfsRequestContext, userId, body.License).WithIdentityRef(this.TfsRequestContext);
      if (body.License.Source == LicensingSource.Auto)
      {
        HttpResponseMessage httpResponseMessage = accountEntitlement != (AccountEntitlement) null ? this.Request.CreateResponse<AccountEntitlement>(HttpStatusCode.Created, accountEntitlement) : this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "User could not be assigned an available license");
        httpResponseMessage.Headers.Location = new Uri(this.Request.RequestUri, "..");
        return httpResponseMessage;
      }
      return !(accountEntitlement != (AccountEntitlement) null) ? this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Failed to assign supplied license to user") : this.Request.CreateResponse<AccountEntitlement>(HttpStatusCode.OK, accountEntitlement);
    }

    [HttpPost]
    [ActionName("assignAvailable")]
    [TraceFilterWithException(1039410, 1039419, 1039418)]
    [ClientLocationId("C01E9FD5-0D8C-4D5E-9A68-734BD8DA6A38")]
    [ClientResponseType(typeof (AccountEntitlement), null, null)]
    public HttpResponseMessage AssignAvailableAccountEntitlement(
      Guid userId,
      LicensingOrigin origin = LicensingOrigin.None)
    {
      ValidationHelper.ValidateCollectionLevelStorageKey(this.TfsRequestContext, userId, true);
      AccountEntitlement accountEntitlement = this.m_accountEntitlementServiceFactory(this.TfsRequestContext).AssignAvailableAccountEntitlement(this.TfsRequestContext, userId, origin).WithIdentityRef(this.TfsRequestContext);
      HttpResponseMessage httpResponseMessage = accountEntitlement != (AccountEntitlement) null ? this.Request.CreateResponse<AccountEntitlement>(HttpStatusCode.Created, accountEntitlement) : this.Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "User could not be assigned an available license");
      httpResponseMessage.Headers.Location = new Uri(this.Request.RequestUri, "..");
      return httpResponseMessage;
    }

    [HttpDelete]
    [ActionName("user")]
    [TraceFilterWithException(1039460, 1039469, 1039468)]
    [ClientLocationId("6490E566-B299-49A7-A4E4-28749752581F")]
    public void DeleteUserEntitlements(Guid userId)
    {
      ValidationHelper.ValidateCollectionLevelStorageKey(this.TfsRequestContext, userId);
      ArgumentUtility.CheckForEmptyGuid(userId, nameof (userId));
      this.m_accountEntitlementServiceFactory(this.TfsRequestContext).DeleteAccountEntitlement(this.TfsRequestContext, userId);
    }

    internal LicensingEntitlementsControllerBase(
      ServiceFactory<ILicensingEntitlementService> accountEntitlementServiceFactory)
    {
      this.m_accountEntitlementServiceFactory = accountEntitlementServiceFactory;
    }
  }
}
