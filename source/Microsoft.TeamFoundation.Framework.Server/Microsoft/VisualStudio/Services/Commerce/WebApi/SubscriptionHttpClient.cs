// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.WebApi.SubscriptionHttpClient
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
  [ClientCircuitBreakerSettings(30, 80, MaxConcurrentRequests = 40)]
  [ClientCancellationTimeout(60)]
  public class SubscriptionHttpClient : SubscriptionHttpClientBase
  {
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

    public SubscriptionHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
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

    public override Task<List<SubscriptionAccount>> GetAccountsByIdentityForOfferIdAsync(
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
      if (serviceOwners != null)
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
      return this.SendAsync<List<SubscriptionAccount>>(method, locationId, version: new ApiResourceVersion("5.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
