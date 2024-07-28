// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Client.SubscriptionHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce.Client
{
  [ResourceArea("365D9DCD-4492-4AE3-B5BA-AD0FF4AB74B3")]
  [ClientCircuitBreakerSettings(100, 80, MaxConcurrentRequests = 40)]
  public class SubscriptionHttpClient : VssHttpClientBase
  {
    internal static readonly Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>()
    {
      {
        "InvalidResourceException",
        typeof (InvalidResourceException)
      },
      {
        "CommerceSecurityException",
        typeof (CommerceSecurityException)
      },
      {
        "AccountNotFoundException",
        typeof (AccountNotFoundException)
      }
    };

    public SubscriptionHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public SubscriptionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public SubscriptionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public SubscriptionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public SubscriptionHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task<IEnumerable<IAzureSubscription>> GetAzureSubscriptions(
      List<Guid> subscriptionIds,
      AccountProviderNamespace providerNamespaceId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      List<Guid> source = subscriptionIds;
      if ((source != null ? (!source.Any<Guid>() ? 1 : 0) : 0) != 0)
        return (IEnumerable<IAzureSubscription>) new List<IAzureSubscription>();
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (GetAzureSubscriptions)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
        List<KeyValuePair<string, string>> queryParameters = collection;
        foreach (Guid subscriptionId in subscriptionIds)
          queryParameters.Add(new KeyValuePair<string, string>("ids", subscriptionId.ToString()));
        return (IEnumerable<IAzureSubscription>) await this.SendAsync<IEnumerable<AzureSubscription>>(HttpMethod.Get, CommerceResourceIds.SubscriptionLocationId, (object) null, (ApiResourceVersion) null, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) queryParameters, (object) null, cancellationToken).ConfigureAwait(false);
      }
    }

    public virtual async Task<IEnumerable<ISubscriptionAccount>> GetAccounts(
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IEnumerable<ISubscriptionAccount> accounts;
      using (new VssHttpClientBase.OperationScope("Commerce", "GetAccountsBySubscription"))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
        accounts = (IEnumerable<ISubscriptionAccount>) await this.SendAsync<IEnumerable<SubscriptionAccount>>(HttpMethod.Get, CommerceResourceIds.SubscriptionLocationId, (object) new
        {
          subscriptionId = subscriptionId
        }, (ApiResourceVersion) null, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, (object) null, cancellationToken).ConfigureAwait(false);
      }
      return accounts;
    }

    public virtual async Task<IEnumerable<ISubscriptionAccount>> GetAccounts(
      AccountProviderNamespace providerNamespaceId,
      Guid memberId,
      bool queryOnlyOwnerAccounts,
      bool inlcudeDisabledAccounts = false,
      bool includeMSAAccounts = false,
      IEnumerable<Guid> serviceOwners = null,
      string galleryId = null,
      bool addUnlinkedSubscription = false,
      bool queryAccountsByUpn = false,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IEnumerable<ISubscriptionAccount> accounts;
      using (new VssHttpClientBase.OperationScope("Commerce", "GetAccountsOwnedByIdentity"))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
        keyValuePairList.Add(nameof (memberId), memberId.ToString());
        keyValuePairList.Add(nameof (queryOnlyOwnerAccounts), queryOnlyOwnerAccounts.ToString());
        keyValuePairList.Add(nameof (inlcudeDisabledAccounts), inlcudeDisabledAccounts.ToString());
        keyValuePairList.Add(nameof (includeMSAAccounts), includeMSAAccounts.ToString());
        keyValuePairList.Add(nameof (queryAccountsByUpn), queryAccountsByUpn.ToString());
        if (!string.IsNullOrEmpty(galleryId))
        {
          keyValuePairList.Add(nameof (galleryId), galleryId);
          keyValuePairList.Add(nameof (addUnlinkedSubscription), addUnlinkedSubscription.ToString());
        }
        if (serviceOwners != null)
        {
          foreach (Guid serviceOwner in serviceOwners)
            keyValuePairList.Add(new KeyValuePair<string, string>(nameof (serviceOwners), serviceOwner.ToString()));
        }
        accounts = (IEnumerable<ISubscriptionAccount>) await this.SendAsync<IEnumerable<SubscriptionAccount>>(HttpMethod.Get, CommerceResourceIds.SubscriptionLocationId, (object) null, (ApiResourceVersion) null, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, (object) null, cancellationToken).ConfigureAwait(false);
      }
      return accounts;
    }

    public virtual Task CreateSubscription(
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      int anniversaryDay,
      SubscriptionStatus status,
      SubscriptionSource source,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      throw new NotImplementedException();
    }

    public virtual async Task LinkAccount(
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      Guid ownerId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SubscriptionHttpClient subscriptionHttpClient = this;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (LinkAccount)))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
        keyValuePairList.Add(nameof (accountId), accountId.ToString());
        keyValuePairList.Add(nameof (ownerId), ownerId.ToString());
        keyValuePairList.Add("hydrate", false.ToString());
        HttpResponseMessage httpResponseMessage = await subscriptionHttpClient.SendAsync(HttpMethod.Put, CommerceResourceIds.SubscriptionLocationId, (object) new
        {
          subscriptionId = subscriptionId
        }, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
    }

    public virtual async Task LinkAccount(
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      Guid ownerId,
      bool hydrate,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SubscriptionHttpClient subscriptionHttpClient = this;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (LinkAccount)))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
        keyValuePairList.Add(nameof (accountId), accountId.ToString());
        keyValuePairList.Add(nameof (ownerId), ownerId.ToString());
        keyValuePairList.Add(nameof (hydrate), hydrate.ToString());
        HttpResponseMessage httpResponseMessage = await subscriptionHttpClient.SendAsync(HttpMethod.Put, CommerceResourceIds.SubscriptionLocationId, (object) new
        {
          subscriptionId = subscriptionId
        }, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
    }

    public virtual async Task UnlinkAccount(
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      Guid ownerId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SubscriptionHttpClient subscriptionHttpClient = this;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (UnlinkAccount)))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
        keyValuePairList.Add(nameof (accountId), accountId.ToString());
        keyValuePairList.Add(nameof (ownerId), ownerId.ToString());
        keyValuePairList.Add("hydrate", false.ToString());
        HttpResponseMessage httpResponseMessage = await subscriptionHttpClient.SendAsync(HttpMethod.Delete, CommerceResourceIds.SubscriptionLocationId, (object) new
        {
          subscriptionId = subscriptionId
        }, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
    }

    public virtual async Task UnlinkAccount(
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      Guid ownerId,
      bool hydrate,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SubscriptionHttpClient subscriptionHttpClient = this;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (UnlinkAccount)))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
        keyValuePairList.Add(nameof (accountId), accountId.ToString());
        keyValuePairList.Add(nameof (ownerId), ownerId.ToString());
        keyValuePairList.Add(nameof (hydrate), hydrate.ToString());
        HttpResponseMessage httpResponseMessage = await subscriptionHttpClient.SendAsync(HttpMethod.Delete, CommerceResourceIds.SubscriptionLocationId, (object) new
        {
          subscriptionId = subscriptionId
        }, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
    }

    public virtual async Task<ISubscriptionAccount> GetSubscriptionAccount(
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ISubscriptionAccount subscriptionAccount;
      using (new VssHttpClientBase.OperationScope("Commerce", "GetAccountSubscriptionId"))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
        keyValuePairList.Add(nameof (accountId), accountId.ToString());
        subscriptionAccount = (ISubscriptionAccount) await this.SendAsync<SubscriptionAccount>(HttpMethod.Get, CommerceResourceIds.SubscriptionLocationId, (object) null, (ApiResourceVersion) null, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, (object) null, cancellationToken).ConfigureAwait(false);
      }
      return subscriptionAccount;
    }

    public virtual async Task<ISubscriptionAccount> GetAzureSubscriptionForPurchase(
      Guid subscriptionId,
      string galleryItemId,
      Guid accountId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SubscriptionHttpClient subscriptionHttpClient1 = this;
      ISubscriptionAccount subscriptionForPurchase;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (GetAzureSubscriptionForPurchase)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add<Guid>("subscriptionid", subscriptionId);
        collection.Add(nameof (galleryItemId), galleryItemId);
        collection.Add<Guid>(nameof (accountId), accountId);
        SubscriptionHttpClient subscriptionHttpClient2 = subscriptionHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid subscriptionLocationId = CommerceResourceIds.SubscriptionLocationId;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        var routeValues = new
        {
          subscriptionId = subscriptionId,
          galleryItemId = galleryItemId,
          accountId = accountId
        };
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        CancellationToken cancellationToken1 = cancellationToken;
        subscriptionForPurchase = (ISubscriptionAccount) await subscriptionHttpClient2.SendAsync<SubscriptionAccount>(get, subscriptionLocationId, (object) routeValues, (ApiResourceVersion) null, (HttpContent) null, queryParameters, (object) null, cancellationToken1).ConfigureAwait(false);
      }
      return subscriptionForPurchase;
    }

    public virtual async Task<IEnumerable<ISubscriptionAccount>> GetAzureSubscriptionForUser(
      Guid? subscriptionId = null,
      bool queryAcrossTenants = false,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SubscriptionHttpClient subscriptionHttpClient1 = this;
      IEnumerable<ISubscriptionAccount> subscriptionForUser;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (GetAzureSubscriptionForUser)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add<Guid?>(nameof (subscriptionId), subscriptionId);
        collection.Add(nameof (queryAcrossTenants), queryAcrossTenants.ToString());
        SubscriptionHttpClient subscriptionHttpClient2 = subscriptionHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid subscriptionLocationId = CommerceResourceIds.SubscriptionLocationId;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        var routeValues = new
        {
          subscriptionId = subscriptionId
        };
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        CancellationToken cancellationToken1 = cancellationToken;
        subscriptionForUser = (IEnumerable<ISubscriptionAccount>) await subscriptionHttpClient2.SendAsync<IEnumerable<SubscriptionAccount>>(get, subscriptionLocationId, (object) routeValues, (ApiResourceVersion) null, (HttpContent) null, queryParameters, (object) null, cancellationToken1).ConfigureAwait(false);
      }
      return subscriptionForUser;
    }

    public virtual async Task ChangeSubscriptionAccount(
      Guid newSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      bool hydrate,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      SubscriptionHttpClient subscriptionHttpClient1 = this;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (ChangeSubscriptionAccount)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (providerNamespaceId), providerNamespaceId.ToString());
        collection.Add(nameof (accountId), accountId.ToString());
        collection.Add(nameof (hydrate), hydrate.ToString());
        SubscriptionHttpClient subscriptionHttpClient2 = subscriptionHttpClient1;
        HttpMethod method = new HttpMethod("PATCH");
        Guid subscriptionLocationId = CommerceResourceIds.SubscriptionLocationId;
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        var routeValues = new
        {
          subscriptionId = newSubscriptionId
        };
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        CancellationToken cancellationToken1 = cancellationToken;
        HttpRequestMessage message = subscriptionHttpClient2.CreateRequestMessageAsync(method, subscriptionLocationId, (object) routeValues, (ApiResourceVersion) null, (HttpContent) null, queryParameters, (object) null, cancellationToken1).SyncResult<HttpRequestMessage>();
        message.Content = (HttpContent) new ObjectContent(newSubscriptionId.GetType(), (object) newSubscriptionId, subscriptionHttpClient1.Formatter);
        HttpResponseMessage httpResponseMessage = await subscriptionHttpClient1.SendAsync(message, cancellationToken: cancellationToken).ConfigureAwait(false);
      }
    }

    [ExcludeFromCodeCoverage]
    public new virtual async Task<T> SendAsync<T>(
      HttpMethod method,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await base.SendAsync<T>(method, locationId, routeValues, version, content, queryParameters, userState, cancellationToken).ConfigureAwait(false);
    }

    internal virtual async Task<HttpRequestMessage> CreateRequestMessageAsync(
      HttpMethod method,
      Guid locationId,
      object routeValues = null,
      ApiResourceVersion version = null,
      HttpContent content = null,
      List<KeyValuePair<string, string>> queryParameters = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await this.CreateRequestMessageAsync(method, locationId, routeValues, version, content, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState, cancellationToken).ConfigureAwait(false);
    }

    [ExcludeFromCodeCoverage]
    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) SubscriptionHttpClient.s_translatedExceptions;
  }
}
