// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ResourceTypesController
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.WindowsAzure.CloudServiceManagement.ResourceProviderCommunication;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [XmlFormatter]
  public class ResourceTypesController : CommerceControllerBase
  {
    public const string OperationTypeQueryParameter = "op";
    public const string CheckNameAvailabilityOperationType = "checknameavailability";

    public ResourceTypesController()
    {
    }

    internal override string Layer => nameof (ResourceTypesController);

    internal ResourceTypesController(HttpControllerContext controllerContext) => this.Initialize(controllerContext);

    [HttpGet]
    [CaptureRdfeOperationId]
    [TraceFilter(5103601, 5103650)]
    [TraceExceptions(5103649)]
    public HttpResponseMessage PerformResourceTypeOperations(
      string subscriptionId,
      string cloudServiceName,
      string resourceType)
    {
      Guid result;
      if (!Guid.TryParse(subscriptionId, out result))
        throw new ArgumentException("subscriptionId with value \"" + subscriptionId + "\" does not contain valid Guid");
      ArgumentUtility.CheckStringForNullOrEmpty(cloudServiceName, nameof (cloudServiceName));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceType, nameof (resourceType));
      ArgumentUtility.CheckForEmptyGuid(result, nameof (subscriptionId));
      NameValueCollection queryString = HttpUtility.ParseQueryString(this.Request.RequestUri.Query);
      string str1 = queryString.Get("op");
      string str2 = !string.IsNullOrEmpty(str1) ? str1.ToLowerInvariant() : throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
      {
        ReasonPhrase = HostingResources.QueryParameterMissing((object) "op")
      });
      if (str2 == "checknameavailability")
      {
        string resourceName = queryString.Get("resourceName");
        return this.Request.CreateResponse<ResourceNameAvailabilityResponse>(HttpStatusCode.OK, this.CheckNameAvailability(resourceType, resourceName));
      }
      throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
      {
        ReasonPhrase = HostingResources.InvalidOperationType((object) str2)
      });
    }

    public ResourceNameAvailabilityResponse CheckNameAvailability(
      string resourceType,
      string resourceName)
    {
      if (this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new InvalidOperationException();
      this.TfsRequestContext.GetService<IPermissionCheckerService>().CheckPermission(this.TfsRequestContext, 1, CommerceSecurity.CommerceSecurityNamespaceId);
      resourceType = resourceType.ToLower();
      if (resourceType == "account")
      {
        resourceName = !string.IsNullOrEmpty(resourceName) ? HttpUtility.UrlDecode(resourceName) : throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
          ReasonPhrase = HostingResources.QueryParameterMissing((object) nameof (resourceName))
        });
        bool flag = false;
        try
        {
          string invalidReason;
          if (!this.TfsRequestContext.GetExtension<ICommerceAccountHandler>().CheckAccountNameAvailability(this.TfsRequestContext, resourceName, out invalidReason))
            throw new AccountPropertyException(invalidReason);
          flag = true;
        }
        catch
        {
        }
        return new ResourceNameAvailabilityResponse()
        {
          IsAvailable = flag
        };
      }
      throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
      {
        ReasonPhrase = HostingResources.InvalidResourceType((object) resourceType)
      });
    }

    public static class CheckNameAvailabilityParameters
    {
      public const string ResourceNameParameter = "resourceName";
    }
  }
}
