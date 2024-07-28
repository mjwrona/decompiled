// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AccountResourceControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Commerce.Migration.Utilities;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public abstract class AccountResourceControllerBase : CsmControllerBase
  {
    public AccountResourceControllerBase()
    {
    }

    internal AccountResourceControllerBase(HttpControllerContext controllerContext) => this.Initialize(controllerContext);

    internal override string Layer => nameof (AccountResourceControllerBase);

    protected internal override bool ExemptFromGlobalExceptionFormatting => true;

    [HttpPut]
    [TraceDetailsFilter(5106081, 5106087)]
    [CsmControllerExceptionHandler(5106085)]
    [ClientResponseType(typeof (AccountResource), null, null)]
    [ClientResponseCode(HttpStatusCode.OK, "The operation succeeded. The Azure DevOps Services account resource was created or updated.", false)]
    [ClientResponseCode(HttpStatusCode.NotFound, "The Azure DevOps Services account does not exist.", true)]
    public virtual HttpResponseMessage Accounts_CreateOrUpdate(
      Guid subscriptionId,
      string resourceGroupName,
      string resourceName,
      [FromBody] AccountResourceRequest requestData)
    {
      this.CheckParameters(subscriptionId, resourceGroupName, resourceName);
      ArgumentUtility.CheckForNull<AccountResourceRequest>(requestData, nameof (requestData));
      this.TfsRequestContext.Trace(5106086, TraceLevel.Verbose, this.Area, this.Layer, "Putting Resource " + resourceName + " for " + this.TfsRequestContext.AuthenticatedUserName);
      AccountResourceRequestInternal request = new AccountResourceRequestInternal(requestData, subscriptionId, resourceGroupName, resourceName, "Microsoft.VisualStudio");
      request.PopulatePropertyFields(this.TfsRequestContext);
      request.SetIdentityInfo(this.Request.Headers);
      request.Validate(this.TfsRequestContext);
      AzureResourceAccount azureResourceAccount = AccountResourceControllerBase.CreateAzureResourceAccount(request);
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      using (IVssRequestContext newTfsRequestContext = vssRequestContext.GetService<ITeamFoundationHostManagementService>().BeginUserRequest(vssRequestContext, vssRequestContext.ServiceHost.InstanceId, this.TfsRequestContext.UserContext))
      {
        newTfsRequestContext.Items["Commerce.RequestSource"] = (object) "Ibiza";
        Microsoft.VisualStudio.Services.Identity.Identity bindPendingIdentity = IdentityHelper.GetOrCreateBindPendingIdentity(newTfsRequestContext.Elevate(), request.TenantId, CommerceIdentityHelper.GetEmailFromUpn(request.Upn), callerName: nameof (Accounts_CreateOrUpdate));
        AzureResourceAccount subscriptionLinkedAccount = this.GetExistingSubscriptionLinkedAccount(newTfsRequestContext, request, azureResourceAccount, this.Area, this.Layer);
        if ((subscriptionLinkedAccount == null ? 0 : (subscriptionLinkedAccount.AccountId != Guid.Empty ? 1 : 0)) != 0)
        {
          if (request.OperationType != AccountResourceRequestOperationType.Link)
            this.CheckCollectionAndOwnership(vssRequestContext, subscriptionLinkedAccount.AccountId, bindPendingIdentity);
          if (request.OperationType != AccountResourceRequestOperationType.Update)
            return this.CreateHttpResponse((object) this.CreateGetResponseBody(subscriptionLinkedAccount, "account"));
        }
        try
        {
          switch (request.OperationType)
          {
            case AccountResourceRequestOperationType.Create:
              azureResourceAccount = this.CreateAccount(newTfsRequestContext, request, azureResourceAccount, bindPendingIdentity, this.Area, this.Layer);
              break;
            case AccountResourceRequestOperationType.Update:
              azureResourceAccount = this.UpdateAccount(newTfsRequestContext, request, this.Area, this.Layer);
              break;
            case AccountResourceRequestOperationType.Link:
              azureResourceAccount = this.LinkAccount(newTfsRequestContext, request, azureResourceAccount, bindPendingIdentity, this.Area, this.Layer);
              break;
          }
        }
        catch (AccountNotFoundException ex)
        {
          return this.Request.CreateResponse(HttpStatusCode.NotFound);
        }
        this.SaveAccountTags(newTfsRequestContext, request, azureResourceAccount);
      }
      AccountResource getResponseBody = this.CreateGetResponseBody(azureResourceAccount, "account");
      this.LogResponse(getResponseBody);
      return this.CreateHttpResponse((object) getResponseBody);
    }

    [HttpPatch]
    [TraceDetailsFilter(5109206, 5109209)]
    [CsmControllerExceptionHandler(5109207)]
    [ClientResponseType(typeof (AccountResource), null, null)]
    [ClientResponseCode(HttpStatusCode.OK, "The operation succeeded. The Azure DevOps Services account resource was updated.", false)]
    [ClientResponseCode(HttpStatusCode.NotFound, "The Azure DevOps Services account does not exist.", true)]
    public virtual HttpResponseMessage Accounts_Update(
      Guid subscriptionId,
      string resourceGroupName,
      string resourceName,
      [FromBody] AccountTagRequest requestData)
    {
      this.CheckParameters(subscriptionId, resourceGroupName, resourceName);
      ArgumentUtility.CheckForNull<AccountTagRequest>(requestData, nameof (requestData));
      this.TfsRequestContext.Trace(5106088, TraceLevel.Verbose, this.Area, this.Layer, "Patching Resource " + resourceName + " for " + this.TfsRequestContext.AuthenticatedUserName);
      AzureResourceAccount accountWithFallback = this.GetAzureResourceAccountWithFallback(this.TfsRequestContext, subscriptionId, AccountProviderNamespace.VisualStudioOnline, resourceName, true);
      if (accountWithFallback == null || resourceGroupName != accountWithFallback.AzureCloudServiceName)
      {
        this.TfsRequestContext.Trace(5106089, TraceLevel.Error, this.Area, this.Layer, "Incoming provision request not found for request content.");
        throw new HttpResponseException(HttpStatusCode.NotFound);
      }
      AccountResourceRequestInternal request = new AccountResourceRequestInternal();
      request.Tags = requestData.Tags;
      this.SaveAccountTags(this.TfsRequestContext, request, accountWithFallback);
      AccountResource getResponseBody = this.CreateGetResponseBody(accountWithFallback, "account");
      this.LogResponse(getResponseBody);
      return this.CreateHttpResponse((object) getResponseBody);
    }

    private void LogResponse(AccountResource response)
    {
      try
      {
        JsonSerializerSettings settings = new JsonSerializerSettings()
        {
          NullValueHandling = NullValueHandling.Include
        };
        this.TfsRequestContext.Trace(5109191, TraceLevel.Info, this.Area, this.Layer, "Response data:" + Environment.NewLine + JsonConvert.SerializeObject((object) response, settings));
      }
      catch
      {
      }
    }

    [HttpDelete]
    [TraceDetailsFilter(5106101, 5106106)]
    [CsmControllerExceptionHandler(5106105)]
    [ClientResponseType(typeof (void), null, null)]
    [ClientResponseCode(HttpStatusCode.OK, "The operation succeeded. The Azure DevOps Services account resource was deleted.", false)]
    public virtual HttpResponseMessage Accounts_Delete(
      Guid subscriptionId,
      string resourceGroupName,
      string resourceName)
    {
      this.CheckParameters(subscriptionId, resourceGroupName, resourceName);
      this.TfsRequestContext.Trace(5106102, TraceLevel.Verbose, this.Area, this.Layer, "Incoming DeleteResource request for " + resourceName + " resource of type account " + string.Format("from subscription {0}, resource group {1}", (object) subscriptionId, (object) resourceGroupName));
      PlatformSubscriptionService service = this.TfsRequestContext.GetService<PlatformSubscriptionService>();
      AzureResourceAccount azureResourceAccount = service.GetAzureResourceAccount(this.TfsRequestContext, subscriptionId, AccountProviderNamespace.VisualStudioOnline, resourceName);
      if (azureResourceAccount == null || azureResourceAccount.AccountId.Equals(Guid.Empty) || !azureResourceAccount.AzureCloudServiceName.Equals(resourceGroupName, StringComparison.OrdinalIgnoreCase) || !azureResourceAccount.AzureResourceName.Equals(resourceName, StringComparison.OrdinalIgnoreCase))
        throw new AzureResourceAccountMissingException(resourceName);
      service.DeleteAzureResourceAccount(this.TfsRequestContext, azureResourceAccount, CustomerIntelligenceEntryPoint.Ibiza);
      this.TfsRequestContext.Trace(5103505, TraceLevel.Info, this.Area, this.Layer, string.Format("Called subscription service to delete subscription relationship for account {0} and subscription {1}", (object) azureResourceAccount.AccountId, (object) azureResourceAccount.AzureSubscriptionId));
      return new HttpResponseMessage(HttpStatusCode.OK);
    }

    [HttpGet]
    [TraceDetailsFilter(5106107, 5106112)]
    [CsmControllerExceptionHandler(5106111)]
    [ClientResponseType(typeof (AccountResource), null, null)]
    [ClientResponseCode(HttpStatusCode.OK, "The operation succeeded. The response contains the details of the Azure DevOps Services account resource.", false)]
    [ClientResponseCode(HttpStatusCode.NotFound, "The Azure DevOps Services account does not exist.", true)]
    public virtual HttpResponseMessage Accounts_Get(
      Guid subscriptionId,
      string resourceGroupName,
      string resourceName)
    {
      this.TfsRequestContext.Trace(5106108, TraceLevel.Verbose, this.Area, this.Layer, string.Format("Get Resource {0} for CUID {1}", (object) resourceName, (object) this.TfsRequestContext.GetUserCuid()));
      AzureResourceAccount accountWithFallback = this.GetAzureResourceAccountWithFallback(this.TfsRequestContext, subscriptionId, AccountProviderNamespace.VisualStudioOnline, resourceName);
      if (!this.ValidateAzureResourceAccount(accountWithFallback, resourceGroupName, subscriptionId))
        return new HttpResponseMessage(HttpStatusCode.NotFound);
      AccountResource getResponseBody = this.CreateGetResponseBody(accountWithFallback, "account");
      this.LogResponse(getResponseBody);
      return this.CreateHttpResponse((object) getResponseBody);
    }

    private bool ValidateAzureResourceAccount(
      AzureResourceAccount azureResourceAccount,
      string resourceGroupName,
      Guid subscriptionId)
    {
      Guid? accountId = azureResourceAccount?.AccountId;
      Guid empty = Guid.Empty;
      return (accountId.HasValue ? (accountId.HasValue ? (accountId.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0 && string.Equals(azureResourceAccount?.AzureCloudServiceName, resourceGroupName, StringComparison.OrdinalIgnoreCase) && azureResourceAccount != null && azureResourceAccount.AzureSubscriptionId == subscriptionId;
    }

    internal void SaveAccountTags(
      IVssRequestContext newTfsRequestContext,
      AccountResourceRequestInternal request,
      AzureResourceAccount azureResourceAccount)
    {
      if (request.Tags == null)
        return;
      CollectionHelper.WithCollectionContext(newTfsRequestContext, azureResourceAccount.AccountId, (Action<IVssRequestContext>) (collectionContext =>
      {
        CommercePropertyStore commercePropertyStore = new CommercePropertyStore();
        if (commercePropertyStore.HasPropertyKind(collectionContext, CommerceConstants.AccountTagPropertyKind))
          commercePropertyStore.DeletePropertyKind(collectionContext, CommerceConstants.AccountTagPropertyKind);
        if (!request.Tags.Any<KeyValuePair<string, string>>())
          return;
        commercePropertyStore.CreatePropertyKind(collectionContext, CommerceConstants.AccountTagPropertyKind, string.Format("AccountTags.{0}", (object) collectionContext.ServiceHost.InstanceId));
        commercePropertyStore.UpdateProperties(collectionContext, CommerceConstants.AccountTagPropertyKind, new PropertiesCollection((IDictionary<string, object>) request.Tags.ToDictionary<KeyValuePair<string, string>, string, object>((System.Func<KeyValuePair<string, string>, string>) (pair => pair.Key), (System.Func<KeyValuePair<string, string>, object>) (pair => (object) pair.Value))));
        MigrationUtilities.DualWriteAccountTags(newTfsRequestContext, azureResourceAccount.CollectionId, request.Tags);
      }), method: nameof (SaveAccountTags));
    }

    protected virtual AccountResource CreateGetResponseBody(
      AzureResourceAccount azureResourceAccount,
      string resourceType)
    {
      return CsmUtilities.CreateResourceGetResponse(this.TfsRequestContext, azureResourceAccount, "Microsoft.VisualStudio", resourceType);
    }

    internal virtual AzureResourceAccount GetAzureResourceAccountWithFallback(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespace,
      string resourceName,
      bool bypassCache = false)
    {
      PlatformSubscriptionService service = requestContext.GetService<PlatformSubscriptionService>();
      AzureResourceAccount accountWithFallback = service.GetAzureResourceAccount(requestContext, subscriptionId, providerNamespace, resourceName, bypassCache);
      Guid collectionId;
      if (accountWithFallback == null && !resourceName.StartsWith("AZR-SUBSCRIPTION-", StringComparison.OrdinalIgnoreCase) && HostNameResolver.TryGetCollectionServiceHostId(requestContext, resourceName, out collectionId))
        accountWithFallback = service.GetAzureResourceAccountByCollectionId(requestContext, collectionId, bypassCache);
      return accountWithFallback;
    }

    internal void CheckParameters(
      Guid subscriptionId,
      string resourceGroupName,
      string resourceName)
    {
      ArgumentUtility.CheckForEmptyGuid(subscriptionId, nameof (subscriptionId));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceGroupName, nameof (resourceGroupName));
      ArgumentUtility.CheckStringForNullOrEmpty(resourceName, nameof (resourceName));
    }

    internal AzureResourceAccount CreateAccount(
      IVssRequestContext newTfsRequestContext,
      AccountResourceRequestInternal request,
      AzureResourceAccount azureResourceAccount,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string areaName,
      string layerName)
    {
      PlatformSubscriptionService service = newTfsRequestContext.GetService<PlatformSubscriptionService>();
      newTfsRequestContext.Trace(5106088, TraceLevel.Verbose, areaName, layerName, "Entering create mode, calling create and link account");
      IVssRequestContext requestContext = newTfsRequestContext;
      AzureResourceAccount azureResourceAccount1 = azureResourceAccount;
      string resourceName = request.ResourceName;
      Microsoft.VisualStudio.Services.Identity.Identity ownerIdentity = identity;
      int requestSource = (int) request.GetRequestSource();
      return service.CreateAndLinkAccount(requestContext, azureResourceAccount1, resourceName, ownerIdentity, (CustomerIntelligenceEntryPoint) requestSource);
    }

    internal AzureResourceAccount LinkAccount(
      IVssRequestContext newTfsRequestContext,
      AccountResourceRequestInternal request,
      AzureResourceAccount azureResourceAccount,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string areaName,
      string layerName)
    {
      PlatformSubscriptionService service = newTfsRequestContext.GetService<PlatformSubscriptionService>();
      newTfsRequestContext.Trace(5106088, TraceLevel.Verbose, areaName, layerName, "Entering link mode, calling link account by name");
      IVssRequestContext requestContext = newTfsRequestContext;
      AzureResourceAccount azureResourceAccount1 = azureResourceAccount;
      string accountName = request.AccountName;
      Microsoft.VisualStudio.Services.Identity.Identity requestorIdentity = identity;
      return service.LinkCollectionByName(requestContext, azureResourceAccount1, accountName, requestorIdentity);
    }

    internal AzureResourceAccount UpdateAccount(
      IVssRequestContext newTfsRequestContext,
      AccountResourceRequestInternal request,
      string areaName,
      string layerName)
    {
      PlatformSubscriptionService service = newTfsRequestContext.GetService<PlatformSubscriptionService>();
      newTfsRequestContext.Trace(5106088, TraceLevel.Verbose, areaName, layerName, "Entering update mode, calling update azure resource account associated resources");
      AzureResourceAccount accountWithFallback = this.GetAzureResourceAccountWithFallback(newTfsRequestContext, request.SubscriptionId, AccountProviderNamespace.VisualStudioOnline, request.ResourceName, true);
      if (accountWithFallback == null || request.ResourceGroupName != accountWithFallback.AzureCloudServiceName)
      {
        newTfsRequestContext.Trace(5106089, TraceLevel.Error, areaName, layerName, "Incoming provision request not found for request content.");
        throw new HttpResponseException(HttpStatusCode.NotFound);
      }
      IVssRequestContext requestContext = newTfsRequestContext;
      AzureResourceAccount resourceAccount = accountWithFallback;
      service.UpdateAzureResourceAccount(requestContext, resourceAccount);
      return accountWithFallback;
    }

    internal static AzureResourceAccount CreateAzureResourceAccount(
      AccountResourceRequestInternal request)
    {
      return new AzureResourceAccount()
      {
        AzureCloudServiceName = request.ResourceGroupName,
        AzureResourceName = request.ResourceName,
        AzureSubscriptionId = request.SubscriptionId,
        AzureGeoRegion = request.Location,
        ProviderNamespaceId = AccountProviderNamespace.VisualStudioOnline,
        ETag = string.Empty
      };
    }

    private AzureResourceAccount GetExistingSubscriptionLinkedAccount(
      IVssRequestContext newTfsRequestContext,
      AccountResourceRequestInternal request,
      AzureResourceAccount azureResourceAccount,
      string areaName,
      string layerName)
    {
      AzureResourceAccount accountWithFallback1 = this.GetAzureResourceAccountWithFallback(newTfsRequestContext, request.SubscriptionId, AccountProviderNamespace.VisualStudioOnline, request.ResourceName, true);
      if (accountWithFallback1 != null)
        return accountWithFallback1;
      PlatformSubscriptionService service = newTfsRequestContext.GetService<PlatformSubscriptionService>();
      try
      {
        service.InitializeInProgressOperation(newTfsRequestContext, azureResourceAccount);
      }
      catch (ConstraintException ex)
      {
        newTfsRequestContext.Trace(5106085, TraceLevel.Verbose, areaName, layerName, string.Format("Initializing new AzureResourceAccount failed with ConstraintException as it already exists in database. SubscriptionId: {0}, CouldServiceName: {1}, AzureResourceName: {2} ", (object) azureResourceAccount.AzureSubscriptionId, (object) azureResourceAccount.AzureCloudServiceName, (object) azureResourceAccount.AzureResourceName));
        AzureResourceAccount accountWithFallback2 = this.GetAzureResourceAccountWithFallback(newTfsRequestContext, request.SubscriptionId, AccountProviderNamespace.VisualStudioOnline, request.ResourceName, true);
        if (accountWithFallback2 != null)
          return accountWithFallback2;
      }
      return (AzureResourceAccount) null;
    }

    private void CheckCollectionAndOwnership(
      IVssRequestContext requestContext,
      Guid collectionId,
      Microsoft.VisualStudio.Services.Identity.Identity requestorIdentity)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.BypassCollectionOwnershipCheck"))
        return;
      PlatformSubscriptionService platformSubscriptionService = requestContext.GetService<PlatformSubscriptionService>();
      CollectionHelper.WithCollectionContext(requestContext, collectionId, (Action<IVssRequestContext, Microsoft.VisualStudio.Services.Identity.Identity>) ((collectionContext, identity) => platformSubscriptionService.CheckCollectionAndOwnership(collectionContext, identity)), false, requestorIdentity, nameof (CheckCollectionAndOwnership));
    }
  }
}
