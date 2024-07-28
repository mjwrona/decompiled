// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ArmAdapterService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class ArmAdapterService : AzureClientBase, IArmAdapterService, IVssFrameworkService
  {
    private static readonly RegistryQuery baseRegistryQuery = (RegistryQuery) "/Service/Commerce/*";
    private static readonly ActionPerformanceTracer performanceTracer = new ActionPerformanceTracer("Commerce", nameof (ArmAdapterService));
    internal ServiceSettings serviceSettings;
    private const string Area = "Commerce";
    private const string Layer = "ArmAdapterService";
    private const string TargetServiceName = "ARM";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), false, in ArmAdapterService.baseRegistryQuery);
      Interlocked.CompareExchange<ServiceSettings>(ref this.serviceSettings, new ServiceSettings(requestContext, (IArmAdapterServiceConfigurationHelper) new ArmAdapterServiceConfigurationHelper()), (ServiceSettings) null);
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));
    }

    public IEnumerable<AzureSubscriptionInfo> GetSubscriptionsForUser(
      IVssRequestContext requestContext,
      Guid? tenantId = null,
      AzureErrorBehavior errorBehavior = AzureErrorBehavior.ReturnDefault)
    {
      requestContext.TraceEnter(5106711, "Commerce", nameof (ArmAdapterService), nameof (GetSubscriptionsForUser));
      tenantId = this.GetTenantIdFromRequestContext(requestContext, 5106711, tenantId);
      try
      {
        Uri serviceUri = this.BuildUriForGetSubscriptions(requestContext);
        List<AzureSubscriptionInfo> subscriptionsForUser = new List<AzureSubscriptionInfo>();
        while (serviceUri != (Uri) null)
        {
          AzureSubscriptionResponseWrapper<ArmAzureSubscriptionInfo> armResponseObject = this.GetArmResponseObject<AzureSubscriptionResponseWrapper<ArmAzureSubscriptionInfo>>(requestContext, serviceUri, HttpMethod.Get, tenantId, errorBehavior: errorBehavior);
          serviceUri = (Uri) null;
          if (armResponseObject != null && armResponseObject.Value != null)
          {
            subscriptionsForUser.AddRange(armResponseObject.Value.Select<ArmAzureSubscriptionInfo, AzureSubscriptionInfo>((Func<ArmAzureSubscriptionInfo, AzureSubscriptionInfo>) (x => x.ToAzureSubscriptionInfo())));
            if (!string.IsNullOrEmpty(armResponseObject.NextLink))
              serviceUri = new Uri(armResponseObject.NextLink);
          }
        }
        return (IEnumerable<AzureSubscriptionInfo>) subscriptionsForUser;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106713, "Commerce", nameof (ArmAdapterService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106712, "Commerce", nameof (ArmAdapterService), nameof (GetSubscriptionsForUser));
      }
    }

    public AzureSubscriptionInfo GetSubscriptionForUser(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AzureErrorBehavior errorBehavior = AzureErrorBehavior.ReturnDefault,
      Guid? tenantId = null)
    {
      requestContext.TraceEnter(5106715, "Commerce", nameof (ArmAdapterService), nameof (GetSubscriptionForUser));
      tenantId = this.GetTenantIdFromRequestContext(requestContext, 5106715, tenantId);
      try
      {
        Uri subscriptions = this.BuildUriForGetSubscriptions(requestContext, subscriptionId);
        IVssRequestContext requestContext1 = requestContext;
        Uri serviceUri = subscriptions;
        HttpMethod get = HttpMethod.Get;
        AzureErrorBehavior azureErrorBehavior = errorBehavior;
        Guid? tenantId1 = tenantId;
        int errorBehavior1 = (int) azureErrorBehavior;
        ArmAzureSubscriptionInfo armResponseObject = this.GetArmResponseObject<ArmAzureSubscriptionInfo>(requestContext1, serviceUri, get, tenantId1, errorBehavior: (AzureErrorBehavior) errorBehavior1);
        requestContext.Trace(CommerceTracePoints.GetSubscriptionForUser, TraceLevel.Info, "Commerce", nameof (ArmAdapterService), string.Format("Arm Response Object - Id: {0}, SubscriptionId {1}, ", (object) armResponseObject?.Id, (object) armResponseObject?.SubscriptionId) + "DisplayName " + armResponseObject?.DisplayName + ", State " + armResponseObject?.State + ", LocationPlacementId " + armResponseObject?.SubscriptionPolicies?.LocationPlacementId + ", QuotaId " + armResponseObject?.SubscriptionPolicies?.QuotaId + ", SpendingLimit " + armResponseObject?.SubscriptionPolicies?.SpendingLimit);
        return armResponseObject != null ? armResponseObject.ToAzureSubscriptionInfo() : (AzureSubscriptionInfo) null;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106717, "Commerce", nameof (ArmAdapterService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106716, "Commerce", nameof (ArmAdapterService), nameof (GetSubscriptionForUser));
      }
    }

    public UsageAggregatesResponse GetBillingUsage(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      DateTime startTime,
      DateTime endTime,
      Guid? tenantId = null)
    {
      requestContext.TraceEnter(5106720, "Commerce", nameof (ArmAdapterService), nameof (GetBillingUsage));
      try
      {
        Uri usage = this.BuildUriForGetUsage(requestContext, subscriptionId, startTime, endTime);
        requestContext.Trace(5106723, TraceLevel.Info, "Commerce", nameof (ArmAdapterService), "URI used for contacting Azure billing usage API: " + usage?.AbsoluteUri);
        return this.GetArmResponseObject<UsageAggregatesResponse>(requestContext, usage, HttpMethod.Get, tenantId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106722, "Commerce", nameof (ArmAdapterService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106721, "Commerce", nameof (ArmAdapterService), nameof (GetBillingUsage));
      }
    }

    public AzureRateCard GetMeterPricing(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string locale,
      string currencyCode,
      string azureOfferCode,
      string regionInfo,
      Guid? tenantId = null)
    {
      requestContext.TraceEnter(5106701, "Commerce", nameof (ArmAdapterService), nameof (GetMeterPricing));
      ArgumentUtility.CheckStringForNullOrEmpty(locale, nameof (locale));
      ArgumentUtility.CheckStringForNullOrEmpty(currencyCode, nameof (currencyCode));
      ArgumentUtility.CheckStringForNullOrEmpty(azureOfferCode, nameof (azureOfferCode));
      ArgumentUtility.CheckStringForNullOrEmpty(regionInfo, nameof (regionInfo));
      try
      {
        Uri serviceUri = this.BuildRatecardApiUri(requestContext, subscriptionId, locale, currencyCode, azureOfferCode, regionInfo);
        return this.GetArmResponseObject<AzureRateCard>(requestContext, serviceUri, HttpMethod.Get, tenantId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106703, "Commerce", nameof (ArmAdapterService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106702, "Commerce", nameof (ArmAdapterService), nameof (GetMeterPricing));
      }
    }

    public AzureAuthorizationResponseWrapper GetAzureSubscriptionAuthorization(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      Guid? tenantId = null)
    {
      requestContext.TraceEnter(5106731, "Commerce", nameof (ArmAdapterService), nameof (GetAzureSubscriptionAuthorization));
      tenantId = this.GetTenantIdFromRequestContext(requestContext, 5106731, tenantId);
      try
      {
        Uri serviceUri = new Uri(this.serviceSettings.ArmBaseUri, string.Format("subscriptions/{0}/providers/microsoft.Authorization/classicAdministrators?api-version={1}", (object) subscriptionId, (object) this.serviceSettings.ClassicAdministratorsVersion(requestContext)));
        return this.GetArmResponseObject<AzureAuthorizationResponseWrapper>(requestContext, serviceUri, HttpMethod.Get, tenantId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106733, "Commerce", nameof (ArmAdapterService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106732, "Commerce", nameof (ArmAdapterService), nameof (GetAzureSubscriptionAuthorization));
      }
    }

    public AzureRoleDefinitionResponseWrapper GetSubscriptionRoleDefinitions(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string filter = "",
      Guid? tenantId = null)
    {
      requestContext.TraceEnter(5106735, "Commerce", nameof (ArmAdapterService), nameof (GetSubscriptionRoleDefinitions));
      tenantId = this.GetTenantIdFromRequestContext(requestContext, 5106735, tenantId);
      try
      {
        IDictionary<string, string> queryParameters = (IDictionary<string, string>) new Dictionary<string, string>()
        {
          {
            "$filter",
            filter
          }
        };
        Uri serviceUri = this.PrepareUri(new Uri(this.serviceSettings.ArmBaseUri.ToString()), this.serviceSettings.RoleDefinitionsVersion(requestContext), queryParameters, string.Format("/subscriptions/{0}/providers/microsoft.Authorization/roleDefinitions", (object) subscriptionId));
        return this.GetArmResponseObject<AzureRoleDefinitionResponseWrapper>(requestContext, serviceUri, HttpMethod.Get, tenantId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106737, "Commerce", nameof (ArmAdapterService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106736, "Commerce", nameof (ArmAdapterService), nameof (GetSubscriptionRoleDefinitions));
      }
    }

    public AzureRoleAssignmentResponseWrapper GetSubscriptionRoleAssignments(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string filter = "",
      Guid? tenantId = null)
    {
      requestContext.TraceEnter(5106738, "Commerce", nameof (ArmAdapterService), nameof (GetSubscriptionRoleAssignments));
      tenantId = this.GetTenantIdFromRequestContext(requestContext, 5106738, tenantId);
      try
      {
        IDictionary<string, string> queryParameters = (IDictionary<string, string>) new Dictionary<string, string>()
        {
          {
            "$filter",
            filter
          }
        };
        Uri serviceUri = this.PrepareUri(new Uri(this.serviceSettings.ArmBaseUri.ToString()), this.serviceSettings.RoleAssignmentsVersion(requestContext), queryParameters, string.Format("/subscriptions/{0}/providers/microsoft.Authorization/roleAssignments", (object) subscriptionId));
        return this.GetArmResponseObject<AzureRoleAssignmentResponseWrapper>(requestContext, serviceUri, HttpMethod.Get, tenantId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106740, "Commerce", nameof (ArmAdapterService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106739, "Commerce", nameof (ArmAdapterService), nameof (GetSubscriptionRoleAssignments));
      }
    }

    public void RegisterSubscriptionAgainstResourceProvider(
      IVssRequestContext requestContext,
      Guid subscriptionId)
    {
      requestContext.TraceEnter(5106815, "Commerce", nameof (ArmAdapterService), nameof (RegisterSubscriptionAgainstResourceProvider));
      ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
      try
      {
        Uri serviceUri = new Uri(this.serviceSettings.ArmBaseUri, string.Format("subscriptions/{0}/providers/Microsoft.VisualStudio/register?api-version={1}", (object) subscriptionId, (object) this.serviceSettings.RegisterApiVersion(requestContext)));
        AzureSubscriptionRegistrationStatus armResponseObject = this.GetArmResponseObject<AzureSubscriptionRegistrationStatus>(requestContext, serviceUri, HttpMethod.Post);
        requestContext.Trace(5106818, TraceLevel.Info, "Commerce", nameof (ArmAdapterService), string.Format("Subscription Registration status for {0}: {1}", (object) subscriptionId, (object) armResponseObject?.RegistrationState));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106816, "Commerce", nameof (ArmAdapterService), ex);
      }
      finally
      {
        requestContext.TraceLeave(5106817, "Commerce", nameof (ArmAdapterService), nameof (RegisterSubscriptionAgainstResourceProvider));
      }
    }

    public ResourceResponse DeleteResource(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName,
      string accountName,
      string extensionName,
      Guid? tenantId = null)
    {
      Uri extensionResourceUrl = this.GetExtensionResourceUrl(requestContext, subscriptionId, resourceGroupName, accountName, extensionName);
      return this.GetArmResponseObject<ResourceResponse>(requestContext, extensionResourceUrl, HttpMethod.Delete, tenantId);
    }

    public ResourceResponse GetResource(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName,
      string accountName,
      string extensionName,
      Guid? tenantId = null)
    {
      Uri extensionResourceUrl = this.GetExtensionResourceUrl(requestContext, subscriptionId, resourceGroupName, accountName, extensionName);
      return this.GetArmResponseObject<ResourceResponse>(requestContext, extensionResourceUrl, HttpMethod.Get, tenantId);
    }

    public ResourceResponse PatchResource(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName,
      string accountName,
      string extensionName,
      ResourceRequest request,
      AzureErrorBehavior errorBehavior = AzureErrorBehavior.ReturnDefault,
      Guid? tenantId = null)
    {
      Uri extensionResourceUrl = this.GetExtensionResourceUrl(requestContext, subscriptionId, resourceGroupName, accountName, extensionName);
      return this.PutArmRequestObject<ResourceResponse>(requestContext, extensionResourceUrl, new HttpMethod("PATCH"), (object) request, errorBehavior: errorBehavior, tenantId: tenantId);
    }

    public ResourceResponse PutResource(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName,
      string accountName,
      string extensionName,
      ResourceRequest request,
      AzureErrorBehavior errorBehavior = AzureErrorBehavior.ReturnDefault,
      Guid? tenantId = null)
    {
      Uri extensionResourceUrl = this.GetExtensionResourceUrl(requestContext, subscriptionId, resourceGroupName, accountName, extensionName);
      return this.PutArmRequestObject<ResourceResponse>(requestContext, extensionResourceUrl, HttpMethod.Put, (object) request, errorBehavior: errorBehavior, tenantId: tenantId);
    }

    public ResourceResponse PutResource(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName,
      string accountName,
      ResourceRequest request,
      AzureErrorBehavior errorBehavior = AzureErrorBehavior.ReturnDefault,
      Guid? tenantId = null)
    {
      Uri accountResourceUrl = this.GetAccountResourceUrl(requestContext, subscriptionId, resourceGroupName, accountName);
      return this.PutArmRequestObject<ResourceResponse>(requestContext, accountResourceUrl, HttpMethod.Put, (object) request, errorBehavior: errorBehavior, tenantId: tenantId);
    }

    public ResourceResponse GetAccountResource(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName,
      string accountName,
      Guid? tenantId = null)
    {
      Uri accountResourceUrl = this.GetAccountResourceUrl(requestContext, subscriptionId, resourceGroupName, accountName);
      return this.GetArmResponseObject<ResourceResponse>(requestContext, accountResourceUrl, HttpMethod.Get, tenantId, (IList<HttpStatusCode>) new HttpStatusCode[1]
      {
        HttpStatusCode.NotFound
      });
    }

    public ResourceGroupResponse CreateResourceGroup(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName,
      string location,
      Guid? tenantId = null)
    {
      Uri resourceGroupUrl = this.GetResourceGroupUrl(requestContext, subscriptionId, resourceGroupName);
      ResourceGroupRequest content = new ResourceGroupRequest()
      {
        location = location
      };
      return this.PutArmRequestObject<ResourceGroupResponse>(requestContext, resourceGroupUrl, HttpMethod.Put, (object) content, tenantId: tenantId);
    }

    public ResourceGroupResponse GetResourceGroup(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName,
      Guid? tenantId = null)
    {
      Uri resourceGroupUrl = this.GetResourceGroupUrl(requestContext, subscriptionId, resourceGroupName);
      return this.GetArmResponseObject<ResourceGroupResponse>(requestContext, resourceGroupUrl, HttpMethod.Get, tenantId);
    }

    private Uri GetResourceGroupUrl(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName)
    {
      Uri armBaseUri = this.serviceSettings.ArmBaseUri;
      string str = this.serviceSettings.ArmResourceGroupApiVersion(requestContext);
      string relativeUri = string.Format("subscriptions/{0}/resourcegroups/{1}?api-version={2}", (object) subscriptionId, (object) resourceGroupName, (object) str);
      return new Uri(armBaseUri, relativeUri);
    }

    private Uri GetExtensionResourceUrl(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName,
      string accountName,
      string extensionName)
    {
      Uri armBaseUri = this.serviceSettings.ArmBaseUri;
      string str = this.serviceSettings.ArmResourceAccountExtensionsApiVersion(requestContext);
      string relativeUri = string.Format("subscriptions/{0}/resourcegroups/{1}/providers/microsoft.visualstudio/account/{2}/extension/{3}?api-version={4}", (object) subscriptionId, (object) resourceGroupName, (object) accountName, (object) extensionName, (object) str);
      return new Uri(armBaseUri, relativeUri);
    }

    private Uri GetAccountResourceUrl(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string resourceGroupName,
      string accountName)
    {
      Uri armBaseUri = this.serviceSettings.ArmBaseUri;
      string str = this.serviceSettings.ArmResourceAccountApiVersion(requestContext);
      string relativeUri = string.Format("subscriptions/{0}/resourcegroups/{1}/providers/microsoft.visualstudio/account/{2}?api-version={3}", (object) subscriptionId, (object) resourceGroupName, (object) accountName, (object) str);
      return new Uri(armBaseUri, relativeUri);
    }

    public virtual AgreementResponse GetAgreement(
      IVssRequestContext requestContext,
      string subscriptionId,
      string publisherId,
      string offerId,
      string planId)
    {
      requestContext.TraceEnter(5106791, "Commerce", nameof (ArmAdapterService), nameof (GetAgreement));
      try
      {
        return this.AgreementAction(requestContext, subscriptionId, publisherId, offerId, planId, (string) null, HttpMethod.Get);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106793, "Commerce", nameof (ArmAdapterService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106792, "Commerce", nameof (ArmAdapterService), nameof (GetAgreement));
      }
    }

    private AgreementResponse AgreementAction(
      IVssRequestContext requestContext,
      string subscriptionId,
      string publisherId,
      string offerId,
      string planId,
      string action,
      HttpMethod method,
      Guid? tenantId = null)
    {
      string urlPath = this.ConstructAgreementUrlPath(subscriptionId, publisherId, offerId, planId, action);
      Uri serviceUri = this.PrepareUri(this.serviceSettings.ArmBaseUri, this.serviceSettings.ArmAgreementApiVersion(requestContext), urlPath: urlPath);
      return this.GetArmResponseObject<AgreementResponse>(requestContext, serviceUri, method, tenantId);
    }

    public virtual AgreementResponse SignAgreement(
      IVssRequestContext requestContext,
      string subscriptionId,
      string publisherId,
      string offerId,
      string planId,
      Guid? tenantId = null)
    {
      requestContext.TraceEnter(5106821, "Commerce", nameof (ArmAdapterService), nameof (SignAgreement));
      try
      {
        return this.AgreementAction(requestContext, subscriptionId, publisherId, offerId, planId, "sign", HttpMethod.Post, tenantId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106823, "Commerce", nameof (ArmAdapterService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106822, "Commerce", nameof (ArmAdapterService), nameof (SignAgreement));
      }
    }

    public virtual AgreementResponse CancelAgreement(
      IVssRequestContext requestContext,
      string subscriptionId,
      string publisherId,
      string offerId,
      string planId)
    {
      requestContext.TraceEnter(5106811, "Commerce", nameof (ArmAdapterService), nameof (CancelAgreement));
      try
      {
        return this.AgreementAction(requestContext, subscriptionId, publisherId, offerId, planId, "cancel", HttpMethod.Post);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106813, "Commerce", nameof (ArmAdapterService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5106812, "Commerce", nameof (ArmAdapterService), nameof (CancelAgreement));
      }
    }

    private string ConstructAgreementUrlPath(
      string subscriptionId,
      string publisherId,
      string offerId,
      string planId,
      string action = null)
    {
      return action == null ? "/subscriptions/" + subscriptionId + "/providers/Microsoft.MarketplaceOrdering/agreements/" + publisherId + "/offers/" + offerId + "/plans/" + planId : "/subscriptions/" + subscriptionId + "/providers/Microsoft.MarketplaceOrdering/agreements/" + publisherId + "/offers/" + offerId + "/plans/" + planId + "/" + action;
    }

    private Uri BuildUriForGetUsage(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      DateTime startTime,
      DateTime endTime)
    {
      DateTime dateTime1 = startTime.Date;
      dateTime1 = dateTime1.AddHours((double) startTime.Hour);
      dateTime1 = dateTime1.ToUniversalTime();
      string str1 = dateTime1.ToString("O");
      DateTime dateTime2 = endTime.Date;
      dateTime2 = dateTime2.AddHours((double) (endTime.Hour + 1));
      dateTime2 = dateTime2.ToUniversalTime();
      string str2 = dateTime2.ToString("O");
      IDictionary<string, string> queryParameters = (IDictionary<string, string>) new Dictionary<string, string>()
      {
        {
          "reportedStartTime",
          str1
        },
        {
          "reportedEndTime",
          str2
        },
        {
          "aggregationGranularity",
          "hourly"
        }
      };
      string urlPath = string.Format("/subscriptions/{0}/providers/Microsoft.Commerce/UsageAggregates", (object) subscriptionId);
      return this.PrepareUri(this.serviceSettings.RateCardBaseUri, this.serviceSettings.RateCardApiVersion(requestContext), queryParameters, urlPath);
    }

    private Uri BuildUriForGetSubscriptions(IVssRequestContext requestContext, Guid subscriptionId = default (Guid))
    {
      string str = this.serviceSettings.ArmApiVersion(requestContext);
      return subscriptionId == Guid.Empty ? new Uri(this.serviceSettings.ArmBaseUri, "subscriptions?api-version=" + str) : new Uri(this.serviceSettings.ArmBaseUri, "subscriptions/" + subscriptionId.ToString() + "?api-version=" + str);
    }

    private Uri BuildRatecardApiUri(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string locale,
      string currency,
      string azureOfferCode,
      string regionInfo)
    {
      object[] objArray = new object[5]
      {
        (object) ("api-version=" + this.serviceSettings.RateCardApiVersion(requestContext)),
        (object) ("OfferDurableId eq '" + azureOfferCode + "'"),
        (object) ("Currency eq '" + currency + "'"),
        (object) ("Locale eq '" + locale + "'"),
        (object) ("RegionInfo eq '" + regionInfo + "'")
      };
      return new Uri(new Uri(this.serviceSettings.RateCardBaseUri, string.Format("subscriptions/{0}/providers/Microsoft.Commerce/RateCard", (object) subscriptionId)), string.Format("?{0}&$filter= {1} and {2} and {3} and {4}", objArray));
    }

    private TOutput PutArmRequestObject<TOutput>(
      IVssRequestContext requestContext,
      Uri serviceUri,
      HttpMethod httpMethod,
      object content,
      IList<HttpStatusCode> whitelistedStatusCodes = null,
      AzureErrorBehavior errorBehavior = AzureErrorBehavior.ReturnDefault,
      Guid? tenantId = null)
    {
      int armRequestTimeOut = this.serviceSettings.ArmRequestTimeOut;
      ICommerceHttpHelper service = requestContext.GetService<ICommerceHttpHelper>();
      JwtSecurityToken securityToken = this.GetSecurityToken(requestContext, tenantId);
      IVssRequestContext requestContext1 = requestContext;
      JwtSecurityToken jwtToken = securityToken;
      int requestTimeOut = armRequestTimeOut;
      HttpClient withJwtTokenAuth = service.GetHttpClientWithJwtTokenAuth(requestContext1, "ARM", jwtToken, requestTimeOut);
      if (withJwtTokenAuth == null)
        return default (TOutput);
      HttpResponseMessage httpResponseMessage;
      using (ArmAdapterService.performanceTracer.Trace(requestContext, 5106728, "GetHttpResponseMessage"))
        httpResponseMessage = this.GetHttpResponseMessage(requestContext, withJwtTokenAuth, serviceUri, httpMethod, armRequestTimeOut * 80 / 100, content, whitelistedStatusCodes);
      if (httpResponseMessage?.Content != null && (httpResponseMessage.IsSuccessStatusCode || errorBehavior == AzureErrorBehavior.SameAsSuccess))
        return this.ParseArmResponse<TOutput>(requestContext, httpResponseMessage.Content);
      if (errorBehavior == AzureErrorBehavior.Throw)
        this.ThrowForNonSuccessfulResponse(requestContext, httpResponseMessage);
      return default (TOutput);
    }

    private TOutput GetArmResponseObject<TOutput>(
      IVssRequestContext requestContext,
      Uri serviceUri,
      HttpMethod httpMethod,
      Guid? tenantId = null,
      IList<HttpStatusCode> whitelistedStatusCodes = null,
      AzureErrorBehavior errorBehavior = AzureErrorBehavior.ReturnDefault,
      Action<HttpResponseMessage> failureAction = null)
    {
      int armRequestTimeOut = this.serviceSettings.ArmRequestTimeOut;
      ICommerceHttpHelper service = requestContext.GetService<ICommerceHttpHelper>();
      requestContext.TraceProperties<IdentityDescriptor>(5106724, "Commerce", nameof (ArmAdapterService), requestContext.UserContext, (string) null);
      JwtSecurityToken jwtSecurityToken = AzureAccessTokenProvider.UseProdAzureResourcesOnDevFabric(requestContext) ? AzureAccessTokenProvider.GetSecurityToken((string) null) : this.GetSecurityToken(requestContext, tenantId);
      IVssRequestContext requestContext1 = requestContext;
      JwtSecurityToken jwtToken = jwtSecurityToken;
      int requestTimeOut = armRequestTimeOut;
      HttpClient withJwtTokenAuth = service.GetHttpClientWithJwtTokenAuth(requestContext1, "ARM", jwtToken, requestTimeOut);
      if (withJwtTokenAuth == null)
        return default (TOutput);
      HttpResponseMessage httpResponseMessage;
      using (ArmAdapterService.performanceTracer.Trace(requestContext, 5106727, "GetHttpResponseMessage"))
        httpResponseMessage = this.GetHttpResponseMessage(requestContext, withJwtTokenAuth, serviceUri, httpMethod, armRequestTimeOut * 80 / 100, whitelistedStatusCodes: whitelistedStatusCodes);
      if (!httpResponseMessage.IsSuccessStatusCode)
      {
        switch (errorBehavior)
        {
          case AzureErrorBehavior.Throw:
            this.ThrowForNonSuccessfulResponse(requestContext, httpResponseMessage);
            break;
          case AzureErrorBehavior.SameAsSuccess:
            goto label_9;
        }
        return default (TOutput);
      }
label_9:
      return httpResponseMessage.Content != null ? this.ParseArmResponse<TOutput>(requestContext, httpResponseMessage.Content) : default (TOutput);
    }

    private void ThrowForNonSuccessfulResponse(
      IVssRequestContext requestContext,
      HttpResponseMessage responseMessage)
    {
      if (responseMessage == null)
        throw new AzureResponseException("AzureResourceManager failed to respond", (string) null);
      if (responseMessage.StatusCode == HttpStatusCode.Forbidden)
        throw new AzureUnauthorizedAccessException(responseMessage.RequestMessage?.RequestUri);
      requestContext.TraceConditionally(5108837, TraceLevel.Verbose, "Commerce", nameof (ArmAdapterService), (Func<string>) (() => string.Join(";\n", responseMessage.Headers.ToList<KeyValuePair<string, IEnumerable<string>>>().Select<KeyValuePair<string, IEnumerable<string>>, string>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (kv => kv.Key + ":" + string.Join(",", kv.Value))))));
      IEnumerable<string> values;
      responseMessage.Headers.TryGetValues("x-ms-correlation-request-id", out values);
      string correlationId = values != null ? values.FirstOrDefault<string>() : (string) null;
      throw new AzureResponseException(responseMessage.StatusCode, responseMessage.ReasonPhrase, correlationId);
    }

    private TOutput ParseArmResponse<TOutput>(
      IVssRequestContext requestContext,
      HttpContent httpContent)
    {
      try
      {
        return httpContent.ReadAsAsync<TOutput>((IEnumerable<MediaTypeFormatter>) new MediaTypeFormatter[1]
        {
          (MediaTypeFormatter) CommerceHttpHelper.JsonMediaTypeFormatter
        }).Result;
      }
      catch (UnsupportedMediaTypeException ex)
      {
        requestContext.TraceException(5106719, "Commerce", nameof (ArmAdapterService), (Exception) ex);
        throw new AzureResponseException(HostingResources.UnknownArmResponseError(), (string) null);
      }
    }

    private JwtSecurityToken GetSecurityToken(IVssRequestContext requestContext, Guid? tenantId = null)
    {
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IAadTokenService service = vssRequestContext.GetService<IAadTokenService>();
        if (!tenantId.HasValue)
          tenantId = new Guid?(AadIdentityHelper.GetIdentityTenantId(requestContext.UserContext));
        requestContext.Trace(5106718, TraceLevel.Info, "Commerce", nameof (ArmAdapterService), string.Format("Security token is requested for tenantId: {0}", (object) tenantId));
        if (requestContext.IsUserContext)
          return service.AcquireToken(vssRequestContext, service.DefaultResource, tenantId.ToString(), requestContext.UserContext);
        Guid? nullable = tenantId;
        Guid empty = Guid.Empty;
        return (nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) != 0 ? (JwtSecurityToken) null : service.AcquireAppToken(requestContext, service.DefaultResource, tenantId.ToString());
      }
      catch (AadCredentialsNotFoundException ex)
      {
        requestContext.TraceException(5109101, "Commerce", nameof (ArmAdapterService), (Exception) ex);
        return (JwtSecurityToken) null;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5106714, "Commerce", nameof (ArmAdapterService), ex);
        return (JwtSecurityToken) null;
      }
    }

    private HttpResponseMessage GetHttpResponseMessage(
      IVssRequestContext requestContext,
      HttpClient httpClient,
      Uri serviceUri,
      HttpMethod httpMethod,
      int slowRequestThresholdMilliseconds,
      object content = null,
      IList<HttpStatusCode> whitelistedStatusCodes = null,
      Action<HttpResponseMessage> failureAction = null)
    {
      try
      {
        requestContext.TraceEnter(5106725, "Commerce", nameof (ArmAdapterService), new object[4]
        {
          (object) serviceUri,
          (object) httpMethod,
          (object) slowRequestThresholdMilliseconds,
          (object) whitelistedStatusCodes
        }, nameof (GetHttpResponseMessage));
        ICommerceHttpHelper service = requestContext.GetService<ICommerceHttpHelper>();
        this.SetServicePointOptions(requestContext, serviceUri);
        IVssRequestContext requestContext1 = requestContext;
        HttpClient httpClient1 = httpClient;
        Uri serviceUri1 = serviceUri;
        HttpMethod httpMethod1 = httpMethod;
        object content1 = content;
        int slowRequestThresholdMilliseconds1 = slowRequestThresholdMilliseconds;
        IList<HttpStatusCode> whitelistedStatusCodes1 = whitelistedStatusCodes;
        Action<HttpResponseMessage> failureAction1 = failureAction;
        return service.GetHttpResponseMessage(requestContext1, httpClient1, serviceUri1, httpMethod1, content1, slowRequestThresholdMilliseconds1, whitelistedStatusCodes1, failureAction1);
      }
      finally
      {
        requestContext.TraceLeave(5106726, "Commerce", nameof (ArmAdapterService), nameof (GetHttpResponseMessage));
      }
    }

    private void SetServicePointOptions(IVssRequestContext requestContext, Uri requestUri)
    {
      if (requestUri == (Uri) null || !requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableArmServicePointOptions"))
        return;
      ServicePoint servicePoint = ServicePointManager.FindServicePoint(new Uri(requestUri.GetLeftPart(UriPartial.Authority)));
      servicePoint.ConnectionLimit = 4 * Environment.ProcessorCount;
      servicePoint.Expect100Continue = false;
      servicePoint.UseNagleAlgorithm = false;
      servicePoint.SetTcpKeepAlive(true, (int) TimeSpan.FromMinutes(5.0).TotalMilliseconds, (int) TimeSpan.FromSeconds(5.0).TotalMilliseconds);
    }

    internal void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write<ServiceSettings>(ref this.serviceSettings, new ServiceSettings(requestContext, (IArmAdapterServiceConfigurationHelper) new ArmAdapterServiceConfigurationHelper()));
    }

    internal Guid? GetTenantIdFromRequestContext(
      IVssRequestContext requestContext,
      int tracePoint,
      Guid? tenantId = null)
    {
      Guid guid;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableUseSubscriptionTenantFromRequestContext") && requestContext.Items.TryGetValue<Guid>("subscriptionTenantId", out guid))
      {
        requestContext.TraceAlways(tracePoint, TraceLevel.Info, "Commerce", nameof (ArmAdapterService), "");
        if (guid != Guid.Empty && (!tenantId.HasValue || tenantId.Value == Guid.Empty))
        {
          requestContext.TraceAlways(tracePoint, TraceLevel.Info, "Commerce", nameof (ArmAdapterService), string.Format("Found tenantId in Request context {0} and updated TenantId ", (object) guid));
          tenantId = new Guid?(guid);
        }
      }
      return tenantId;
    }

    public AzureRoleAssignmentResponseWrapper GetSubscriptionRoleAssignmentsMSA(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string filter = "")
    {
      Guid? fromRequestContext = this.GetTenantIdFromRequestContext(requestContext, 5106738, new Guid?(Guid.Empty));
      try
      {
        Guid? nullable;
        if (fromRequestContext.HasValue)
        {
          nullable = fromRequestContext;
          Guid empty = Guid.Empty;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
          {
            JwtSecurityToken securityToken = this.GetSecurityToken(requestContext, fromRequestContext);
            requestContext.TraceAlways(5106738, TraceLevel.Info, "Commerce", nameof (ArmAdapterService), string.Format(" OId value {0} from Jwtsecuritytoken for tenantID {1} ", (object) securityToken.Claims.First<Claim>((Func<Claim, bool>) (claim => claim.Type == "oid")).Value, (object) fromRequestContext));
            Guid result;
            AzureRoleAssignmentResponseWrapper subscriptionRoleAssignments;
            if (!Guid.TryParse(securityToken.Claims.First<Claim>((Func<Claim, bool>) (claim => claim.Type == "oid")).Value, out result))
            {
              IVssRequestContext requestContext1 = requestContext;
              Guid subscriptionId1 = subscriptionId;
              string filter1 = filter;
              nullable = new Guid?();
              Guid? tenantId = nullable;
              subscriptionRoleAssignments = this.GetSubscriptionRoleAssignments(requestContext1, subscriptionId1, filter1, tenantId);
            }
            else
            {
              IVssRequestContext requestContext2 = requestContext;
              Guid subscriptionId2 = subscriptionId;
              string filter2 = string.Format("assignedTo('{0}')", (object) result);
              nullable = new Guid?();
              Guid? tenantId = nullable;
              subscriptionRoleAssignments = this.GetSubscriptionRoleAssignments(requestContext2, subscriptionId2, filter2, tenantId);
            }
            return subscriptionRoleAssignments;
          }
        }
        IVssRequestContext requestContext3 = requestContext;
        Guid subscriptionId3 = subscriptionId;
        string filter3 = filter;
        nullable = new Guid?();
        Guid? tenantId1 = nullable;
        return this.GetSubscriptionRoleAssignments(requestContext3, subscriptionId3, filter3, tenantId1);
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(5106740, TraceLevel.Error, "Commerce", nameof (ArmAdapterService), " Exception happened in GetSubscriptionRoleAssignmentsMSA " + ex.Message);
        return this.GetSubscriptionRoleAssignments(requestContext, subscriptionId, filter, new Guid?());
      }
    }
  }
}
