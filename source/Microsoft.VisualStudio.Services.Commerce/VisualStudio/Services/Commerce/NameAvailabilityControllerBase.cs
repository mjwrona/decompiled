// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.NameAvailabilityControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [CaptureExternalOperationIdHeader("x-ms-correlation-request-id")]
  [SetCsmV2ResponseHeaders]
  public abstract class NameAvailabilityControllerBase : CsmControllerBase
  {
    internal override string Layer => nameof (NameAvailabilityControllerBase);

    public NameAvailabilityControllerBase()
    {
    }

    internal NameAvailabilityControllerBase(HttpControllerContext controllerContext) => this.Initialize(controllerContext);

    [HttpPost]
    [TraceDetailsFilter(5109021, 5109022)]
    [CsmControllerExceptionHandler(5109023)]
    [ClientResponseCode(HttpStatusCode.OK, "The operation succeeded. The response contains the result of the name availability check.", false)]
    public virtual CheckNameAvailabilityResult Accounts_CheckNameAvailability(
      Guid subscriptionId,
      [FromBody] CheckNameAvailabilityParameter request)
    {
      ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
      ArgumentUtility.CheckForNull<CheckNameAvailabilityParameter>(request, nameof (request));
      this.TfsRequestContext.GetService<IPermissionCheckerService>().CheckPermission(this.TfsRequestContext, 1, CommerceSecurity.CommerceSecurityNamespaceId);
      string lower = request.ResourceType.ToLower();
      string resourceName = request.ResourceName;
      if (lower == "account")
      {
        string str = !string.IsNullOrEmpty(resourceName) ? HttpUtility.UrlDecode(resourceName) : throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
          ReasonPhrase = HostingResources.QueryParameterMissing((object) "resourceName")
        });
        CheckNameAvailabilityResult availabilityResult = new CheckNameAvailabilityResult();
        try
        {
          string invalidReason;
          if (!this.TfsRequestContext.GetExtension<ICommerceAccountHandler>().CheckAccountNameAvailability(this.TfsRequestContext, str, out invalidReason))
          {
            availabilityResult.Message = invalidReason;
          }
          else
          {
            AccountNameValidator.ValidateAccountName(str);
            availabilityResult.NameAvailable = true;
          }
        }
        catch (AccountPropertyException ex)
        {
          availabilityResult.Message = ex.Message;
        }
        return availabilityResult;
      }
      throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
      {
        ReasonPhrase = HostingResources.InvalidResourceType((object) lower)
      });
    }
  }
}
