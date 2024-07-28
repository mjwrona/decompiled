// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.WebApi.SubscriptionHttpClientBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce.WebApi
{
  [ResourceArea("AC02550F-721A-4913-8EA5-CADAE535B03F")]
  public abstract class SubscriptionHttpClientBase : VssHttpClientBase
  {
    public SubscriptionHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public SubscriptionHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public SubscriptionHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public SubscriptionHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public SubscriptionHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual Task<SubscriptionAccount> GetAccountDetailsAsync(
      Guid accountId,
      IEnumerable<Guid> serviceOwners,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0288f4e6-21d3-4529-ac5f-1719f99a4396");
      object routeValues = (object) new
      {
        accountId = accountId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddIEnumerableAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (serviceOwners), (object) serviceOwners);
      return this.SendAsync<SubscriptionAccount>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<SubscriptionAccount>> GetAccountsByIdentityForOfferIdV2Async(
      AccountProviderNamespace providerNamespaceId,
      Guid memberId,
      bool queryOnlyOwnerAccounts,
      bool includeDisabledAccounts,
      bool includeMSAAccounts,
      IEnumerable<Guid> serviceOwners,
      string galleryId,
      bool? addUnlinkedSubscription = null,
      bool? queryAccountsByUpn = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0288f4e6-21d3-4529-ac5f-1719f99a4396");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
      keyValuePairList.Add(nameof (memberId), memberId.ToString());
      keyValuePairList.Add(nameof (queryOnlyOwnerAccounts), queryOnlyOwnerAccounts.ToString());
      keyValuePairList.Add(nameof (includeDisabledAccounts), includeDisabledAccounts.ToString());
      keyValuePairList.Add(nameof (includeMSAAccounts), includeMSAAccounts.ToString());
      this.AddIEnumerableAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (serviceOwners), (object) serviceOwners);
      keyValuePairList.Add(nameof (galleryId), galleryId);
      bool flag;
      if (addUnlinkedSubscription.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = addUnlinkedSubscription.Value;
        string str = flag.ToString();
        collection.Add(nameof (addUnlinkedSubscription), str);
      }
      if (queryAccountsByUpn.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = queryAccountsByUpn.Value;
        string str = flag.ToString();
        collection.Add(nameof (queryAccountsByUpn), str);
      }
      return this.SendAsync<List<SubscriptionAccount>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<SubscriptionAccount>> GetAccountsByIdentityV2Async(
      AccountProviderNamespace providerNamespaceId,
      Guid memberId,
      bool queryOnlyOwnerAccounts,
      bool includeDisabledAccounts,
      bool includeMSAAccounts,
      IEnumerable<Guid> serviceOwners,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0288f4e6-21d3-4529-ac5f-1719f99a4396");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
      keyValuePairList.Add(nameof (memberId), memberId.ToString());
      keyValuePairList.Add(nameof (queryOnlyOwnerAccounts), queryOnlyOwnerAccounts.ToString());
      keyValuePairList.Add(nameof (includeDisabledAccounts), includeDisabledAccounts.ToString());
      keyValuePairList.Add(nameof (includeMSAAccounts), includeMSAAccounts.ToString());
      this.AddIEnumerableAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (serviceOwners), (object) serviceOwners);
      return this.SendAsync<List<SubscriptionAccount>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<Guid> GetOrganizationTenantIdAsync(
      string organizationId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("0288f4e6-21d3-4529-ac5f-1719f99a4396");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (organizationId), organizationId);
      return this.SendAsync<Guid>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task ChangeSubscriptionAccountAsync(
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      bool? hydrate = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SubscriptionHttpClientBase subscriptionHttpClientBase = this;
      HttpMethod method = new HttpMethod("PATCH");
      Guid locationId = new Guid("94de86a2-03e3-42db-a2e8-1a82bf13a262");
      object routeValues = (object) new
      {
        subscriptionId = subscriptionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
      keyValuePairList.Add(nameof (accountId), accountId.ToString());
      if (hydrate.HasValue)
        keyValuePairList.Add(nameof (hydrate), hydrate.Value.ToString());
      using (await subscriptionHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task ChangeSubscriptionAccountWithTenantAsync(
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      Guid subscriptionTenantId,
      bool? hydrate = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SubscriptionHttpClientBase subscriptionHttpClientBase = this;
      HttpMethod method = new HttpMethod("PATCH");
      Guid locationId = new Guid("94de86a2-03e3-42db-a2e8-1a82bf13a262");
      object routeValues = (object) new
      {
        subscriptionId = subscriptionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
      keyValuePairList.Add(nameof (accountId), accountId.ToString());
      keyValuePairList.Add(nameof (subscriptionTenantId), subscriptionTenantId.ToString());
      if (hydrate.HasValue)
        keyValuePairList.Add(nameof (hydrate), hydrate.Value.ToString());
      using (await subscriptionHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<SubscriptionAccount>> GetAccountsAsync(
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("94de86a2-03e3-42db-a2e8-1a82bf13a262");
      object routeValues = (object) new
      {
        subscriptionId = subscriptionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
      return this.SendAsync<List<SubscriptionAccount>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<SubscriptionAccount>> GetAccountsByIdentityAsync(
      AccountProviderNamespace providerNamespaceId,
      Guid memberId,
      bool queryOnlyOwnerAccounts,
      bool inlcudeDisabledAccounts,
      bool includeMSAAccounts,
      IEnumerable<Guid> serviceOwners,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("94de86a2-03e3-42db-a2e8-1a82bf13a262");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
      keyValuePairList.Add(nameof (memberId), memberId.ToString());
      keyValuePairList.Add(nameof (queryOnlyOwnerAccounts), queryOnlyOwnerAccounts.ToString());
      keyValuePairList.Add(nameof (inlcudeDisabledAccounts), inlcudeDisabledAccounts.ToString());
      keyValuePairList.Add(nameof (includeMSAAccounts), includeMSAAccounts.ToString());
      this.AddIEnumerableAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (serviceOwners), (object) serviceOwners);
      return this.SendAsync<List<SubscriptionAccount>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<SubscriptionAccount>> GetAccountsByIdentityForOfferIdAsync(
      AccountProviderNamespace providerNamespaceId,
      Guid memberId,
      bool queryOnlyOwnerAccounts,
      bool inlcudeDisabledAccounts,
      bool includeMSAAccounts,
      IEnumerable<Guid> serviceOwners,
      string galleryId,
      bool? addUnlinkedSubscription = null,
      bool? queryAccountsByUpn = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("94de86a2-03e3-42db-a2e8-1a82bf13a262");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
      keyValuePairList.Add(nameof (memberId), memberId.ToString());
      keyValuePairList.Add(nameof (queryOnlyOwnerAccounts), queryOnlyOwnerAccounts.ToString());
      keyValuePairList.Add(nameof (inlcudeDisabledAccounts), inlcudeDisabledAccounts.ToString());
      keyValuePairList.Add(nameof (includeMSAAccounts), includeMSAAccounts.ToString());
      this.AddIEnumerableAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (serviceOwners), (object) serviceOwners);
      keyValuePairList.Add(nameof (galleryId), galleryId);
      bool flag;
      if (addUnlinkedSubscription.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = addUnlinkedSubscription.Value;
        string str = flag.ToString();
        collection.Add(nameof (addUnlinkedSubscription), str);
      }
      if (queryAccountsByUpn.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        flag = queryAccountsByUpn.Value;
        string str = flag.ToString();
        collection.Add(nameof (queryAccountsByUpn), str);
      }
      return this.SendAsync<List<SubscriptionAccount>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<SubscriptionAccount> GetAzureSubscriptionForPurchaseAsync(
      Guid subscriptionId,
      string galleryItemId,
      Guid? accountId = null,
      Guid? subscriptionTenantId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("94de86a2-03e3-42db-a2e8-1a82bf13a262");
      object routeValues = (object) new
      {
        subscriptionId = subscriptionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (galleryItemId), galleryItemId);
      Guid guid;
      if (accountId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        guid = accountId.Value;
        string str = guid.ToString();
        collection.Add(nameof (accountId), str);
      }
      if (subscriptionTenantId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        guid = subscriptionTenantId.Value;
        string str = guid.ToString();
        collection.Add(nameof (subscriptionTenantId), str);
      }
      return this.SendAsync<SubscriptionAccount>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<SubscriptionAccount>> GetAzureSubscriptionForUserAsync(
      Guid? subscriptionId = null,
      bool? queryAcrossTenants = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("94de86a2-03e3-42db-a2e8-1a82bf13a262");
      object routeValues = (object) new
      {
        subscriptionId = subscriptionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (queryAcrossTenants.HasValue)
        keyValuePairList.Add(nameof (queryAcrossTenants), queryAcrossTenants.Value.ToString());
      return this.SendAsync<List<SubscriptionAccount>>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<AzureSubscription>> GetAzureSubscriptionsAsync(
      IEnumerable<Guid> ids,
      AccountProviderNamespace providerNamespaceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("94de86a2-03e3-42db-a2e8-1a82bf13a262");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      this.AddIEnumerableAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (ids), (object) ids);
      keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
      return this.SendAsync<List<AzureSubscription>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<SubscriptionAccount> GetSubscriptionAccountAsync(
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("94de86a2-03e3-42db-a2e8-1a82bf13a262");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
      keyValuePairList.Add(nameof (accountId), accountId.ToString());
      return this.SendAsync<SubscriptionAccount>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<SubscriptionAccount> GetSubscriptionAccountByNameAsync(
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      string accountName,
      IEnumerable<Guid> serviceOwners,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("94de86a2-03e3-42db-a2e8-1a82bf13a262");
      object routeValues = (object) new
      {
        subscriptionId = subscriptionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
      keyValuePairList.Add(nameof (accountName), accountName);
      this.AddIEnumerableAsQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (serviceOwners), (object) serviceOwners);
      return this.SendAsync<SubscriptionAccount>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<bool> IsAssignmentBillingEnabledAsync(
      Guid accountId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("94de86a2-03e3-42db-a2e8-1a82bf13a262");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (accountId), accountId.ToString());
      return this.SendAsync<bool>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<bool> IsPortalStaticPageEnabledAsync(
      Guid directoryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("94de86a2-03e3-42db-a2e8-1a82bf13a262");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (directoryId), directoryId.ToString());
      return this.SendAsync<bool>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<bool> IsProjectCollectionAdminAsync(
      Guid memberId,
      Guid collectionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("94de86a2-03e3-42db-a2e8-1a82bf13a262");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (memberId), memberId.ToString());
      keyValuePairList.Add(nameof (collectionId), collectionId.ToString());
      return this.SendAsync<bool>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task LinkAccountAsync(
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      Guid ownerId,
      bool? hydrate = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SubscriptionHttpClientBase subscriptionHttpClientBase = this;
      HttpMethod method = new HttpMethod("PUT");
      Guid locationId = new Guid("94de86a2-03e3-42db-a2e8-1a82bf13a262");
      object routeValues = (object) new
      {
        subscriptionId = subscriptionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
      keyValuePairList.Add(nameof (accountId), accountId.ToString());
      keyValuePairList.Add(nameof (ownerId), ownerId.ToString());
      if (hydrate.HasValue)
        keyValuePairList.Add(nameof (hydrate), hydrate.Value.ToString());
      using (await subscriptionHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task LinkAccountWithTenantAsync(
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      Guid ownerId,
      Guid subscriptionTenantId,
      bool? hydrate = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SubscriptionHttpClientBase subscriptionHttpClientBase = this;
      HttpMethod method = new HttpMethod("PUT");
      Guid locationId = new Guid("94de86a2-03e3-42db-a2e8-1a82bf13a262");
      object routeValues = (object) new
      {
        subscriptionId = subscriptionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
      keyValuePairList.Add(nameof (accountId), accountId.ToString());
      keyValuePairList.Add(nameof (ownerId), ownerId.ToString());
      keyValuePairList.Add(nameof (subscriptionTenantId), subscriptionTenantId.ToString());
      if (hydrate.HasValue)
        keyValuePairList.Add(nameof (hydrate), hydrate.Value.ToString());
      using (await subscriptionHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task UnlinkAccountAsync(
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      Guid ownerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SubscriptionHttpClientBase subscriptionHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("94de86a2-03e3-42db-a2e8-1a82bf13a262");
      object routeValues = (object) new
      {
        subscriptionId = subscriptionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
      keyValuePairList.Add(nameof (accountId), accountId.ToString());
      keyValuePairList.Add(nameof (ownerId), ownerId.ToString());
      using (await subscriptionHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task UnlinkAccountWithTenantAsync(
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      Guid ownerId,
      Guid subscriptionTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SubscriptionHttpClientBase subscriptionHttpClientBase = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("94de86a2-03e3-42db-a2e8-1a82bf13a262");
      object routeValues = (object) new
      {
        subscriptionId = subscriptionId
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
      keyValuePairList.Add(nameof (accountId), accountId.ToString());
      keyValuePairList.Add(nameof (ownerId), ownerId.ToString());
      keyValuePairList.Add(nameof (subscriptionTenantId), subscriptionTenantId.ToString());
      using (await subscriptionHttpClientBase.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }
  }
}
