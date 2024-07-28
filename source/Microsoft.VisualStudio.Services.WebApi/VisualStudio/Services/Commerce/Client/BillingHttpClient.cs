// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Client.BillingHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce.Client
{
  [ResourceArea("365D9DCD-4492-4AE3-B5BA-AD0FF4AB74B3")]
  [ClientCircuitBreakerSettings(30, 80, MaxConcurrentRequests = 55)]
  [ClientCancellationTimeout(60)]
  public class BillingHttpClient : VssHttpClientBase
  {
    protected static readonly Version previewApiVersion = new Version(2, 0);
    protected static readonly Version apiVersion40 = new Version(4, 0);
    internal static readonly Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>()
    {
      {
        "InvalidResourceException",
        typeof (InvalidResourceException)
      },
      {
        "InvalidOperationException",
        typeof (InvalidOperationException)
      },
      {
        "CommerceSecurityException",
        typeof (CommerceSecurityException)
      },
      {
        "AccountNotFoundException",
        typeof (AccountNotFoundException)
      },
      {
        "AccountQuantityException",
        typeof (AccountQuantityException)
      },
      {
        "UserIsNotSubscriptionAdminException",
        typeof (UserIsNotSubscriptionAdminException)
      },
      {
        "UserIsNotAccountOwnerException",
        typeof (UserIsNotAccountOwnerException)
      },
      {
        "UnsupportedSubscriptionTypeException",
        typeof (UnsupportedSubscriptionTypeException)
      }
    };

    public BillingHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public BillingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public BillingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public BillingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public BillingHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task<IOfferSubscription> GetResourceUsage(
      string offerMeterName,
      ResourceRenewalGroup renewalGroup,
      bool nextBillingPeriod,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BillingHttpClient billingHttpClient1 = this;
      IOfferSubscription resourceUsage;
      using (new VssHttpClientBase.OperationScope("Commerce", "GetOfferSubscriptionForRenewalGroup"))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add<ResourceRenewalGroup>(nameof (renewalGroup), renewalGroup);
        collection.Add(nameof (nextBillingPeriod), nextBillingPeriod.ToString());
        collection.Add("galleryId", offerMeterName);
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        BillingHttpClient billingHttpClient2 = billingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid subscriptionResourceId = CommerceResourceIds.OfferSubscriptionResourceId;
        var routeValues = new
        {
          resourceName = offerMeterName
        };
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion(BillingHttpClient.previewApiVersion, 1);
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        resourceUsage = (IOfferSubscription) await billingHttpClient2.SendAsync<OfferSubscription>(get, subscriptionResourceId, (object) routeValues, version, (HttpContent) null, queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
      }
      return resourceUsage;
    }

    public virtual async Task<IOfferSubscription> GetResourceUsage(
      string offerMeterName,
      bool nextBillingPeriod,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BillingHttpClient billingHttpClient1 = this;
      IOfferSubscription resourceUsage;
      using (new VssHttpClientBase.OperationScope("Commerce", "GetOfferSubscription"))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (nextBillingPeriod), nextBillingPeriod.ToString());
        collection.Add("galleryId", offerMeterName);
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        BillingHttpClient billingHttpClient2 = billingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid subscriptionResourceId = CommerceResourceIds.OfferSubscriptionResourceId;
        var routeValues = new
        {
          resourceName = offerMeterName
        };
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion(BillingHttpClient.previewApiVersion, 1);
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        resourceUsage = (IOfferSubscription) await billingHttpClient2.SendAsync<OfferSubscription>(get, subscriptionResourceId, (object) routeValues, version, (HttpContent) null, queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
      }
      return resourceUsage;
    }

    public virtual async Task<IEnumerable<IOfferSubscription>> GetResourceUsage(
      bool nextBillingPeriod,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BillingHttpClient billingHttpClient1 = this;
      IEnumerable<IOfferSubscription> resourceUsage;
      using (new VssHttpClientBase.OperationScope("Commerce", "GetOfferSubscriptions"))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (nextBillingPeriod), nextBillingPeriod.ToString());
        BillingHttpClient billingHttpClient2 = billingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid subscriptionResourceId = CommerceResourceIds.OfferSubscriptionResourceId;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion(BillingHttpClient.previewApiVersion, 1);
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        resourceUsage = (IEnumerable<IOfferSubscription>) await billingHttpClient2.SendAsync<IEnumerable<OfferSubscription>>(get, subscriptionResourceId, (object) null, version, (HttpContent) null, queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
      }
      return resourceUsage;
    }

    public virtual async Task<IEnumerable<IOfferSubscription>> GetAllOfferSubscriptionsForUser(
      bool validateAzuresubscription = false,
      bool nextBillingPeriod = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BillingHttpClient billingHttpClient1 = this;
      IEnumerable<IOfferSubscription> subscriptionsForUser;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (GetAllOfferSubscriptionsForUser)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (validateAzuresubscription), validateAzuresubscription.ToString());
        collection.Add(nameof (nextBillingPeriod), nextBillingPeriod.ToString());
        BillingHttpClient billingHttpClient2 = billingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid subscriptionResourceId = CommerceResourceIds.OfferSubscriptionResourceId;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion(BillingHttpClient.previewApiVersion, 1);
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        subscriptionsForUser = (IEnumerable<IOfferSubscription>) await billingHttpClient2.SendAsync<IEnumerable<OfferSubscription>>(get, subscriptionResourceId, (object) null, version, (HttpContent) null, queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
      }
      return subscriptionsForUser;
    }

    public virtual async Task<IEnumerable<IOfferSubscription>> GetOfferSubscriptionsForGalleryItem(
      Guid azureSubscriptionId,
      string galleryItemId,
      bool nextBillingPeriod = false,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BillingHttpClient billingHttpClient1 = this;
      IEnumerable<IOfferSubscription> subscriptionsForGalleryItem;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (GetOfferSubscriptionsForGalleryItem)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (galleryItemId), galleryItemId.ToString());
        collection.Add(nameof (azureSubscriptionId), azureSubscriptionId.ToString());
        collection.Add(nameof (nextBillingPeriod), nextBillingPeriod.ToString());
        BillingHttpClient billingHttpClient2 = billingHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid subscriptionResourceId = CommerceResourceIds.OfferSubscriptionResourceId;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion(BillingHttpClient.previewApiVersion, 1);
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        subscriptionsForGalleryItem = (IEnumerable<IOfferSubscription>) await billingHttpClient2.SendAsync<IEnumerable<OfferSubscription>>(get, subscriptionResourceId, (object) null, version, (HttpContent) null, queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
      }
      return subscriptionsForGalleryItem;
    }

    public virtual async Task SetAccountQuantity(
      string offerMeterName,
      ResourceRenewalGroup renewalGroup,
      int? includedQuantity,
      int? maximumQuantity,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BillingHttpClient billingHttpClient1 = this;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (SetAccountQuantity)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (offerMeterName), offerMeterName);
        collection.Add<ResourceRenewalGroup>("meterRenewalGroup", renewalGroup);
        collection.Add<int?>("newIncludedQuantity", includedQuantity);
        collection.Add<int?>("newMaximumQuantity", maximumQuantity);
        List<KeyValuePair<string, string>> keyValuePairList1 = collection;
        BillingHttpClient billingHttpClient2 = billingHttpClient1;
        HttpMethod method = new HttpMethod("PATCH");
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(BillingHttpClient.previewApiVersion, 1);
        Guid subscriptionResourceId = CommerceResourceIds.OfferSubscriptionResourceId;
        List<KeyValuePair<string, string>> keyValuePairList2 = keyValuePairList1;
        var routeValues = new
        {
          offerMeterName = offerMeterName
        };
        ApiResourceVersion version = apiResourceVersion;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList2;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        HttpRequestMessage message = billingHttpClient2.CreateRequestMessageAsync(method, subscriptionResourceId, (object) routeValues, version, (HttpContent) null, queryParameters, userState1, cancellationToken1).SyncResult<HttpRequestMessage>();
        HttpResponseMessage httpResponseMessage = await billingHttpClient1.SendAsync(message, userState, cancellationToken).ConfigureAwait(false);
      }
    }

    public virtual async Task SetAccountQuantity(
      string offerMeterName,
      int includedQuantity,
      int maximumQuantity,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BillingHttpClient billingHttpClient1 = this;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (SetAccountQuantity)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (offerMeterName), offerMeterName);
        collection.Add<int>("newIncludedQuantity", includedQuantity);
        collection.Add<int>("newMaximumQuantity", maximumQuantity);
        List<KeyValuePair<string, string>> keyValuePairList1 = collection;
        BillingHttpClient billingHttpClient2 = billingHttpClient1;
        HttpMethod method = new HttpMethod("PATCH");
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(BillingHttpClient.previewApiVersion, 1);
        Guid subscriptionResourceId = CommerceResourceIds.OfferSubscriptionResourceId;
        List<KeyValuePair<string, string>> keyValuePairList2 = keyValuePairList1;
        var routeValues = new
        {
          resourceName = offerMeterName
        };
        ApiResourceVersion version = apiResourceVersion;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList2;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        HttpRequestMessage message = billingHttpClient2.CreateRequestMessageAsync(method, subscriptionResourceId, (object) routeValues, version, (HttpContent) null, queryParameters, userState1, cancellationToken1).SyncResult<HttpRequestMessage>();
        message.Content = (HttpContent) null;
        HttpResponseMessage httpResponseMessage = await billingHttpClient1.SendAsync(message, userState, cancellationToken).ConfigureAwait(false);
      }
    }

    public virtual async Task TogglePaidBilling(
      string offerMeterName,
      bool paidBillingStatus,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BillingHttpClient billingHttpClient1 = this;
      using (new VssHttpClientBase.OperationScope("Commerce", "UpdateOfferSubscription"))
      {
        List<KeyValuePair<string, string>> queryParameters = new List<KeyValuePair<string, string>>();
        queryParameters.Add(nameof (offerMeterName), offerMeterName);
        OfferSubscription subscriptionResource = (OfferSubscription) await billingHttpClient1.GetResourceUsage(offerMeterName, true, cancellationToken: cancellationToken).ConfigureAwait(false);
        subscriptionResource.IsPaidBillingEnabled = paidBillingStatus;
        BillingHttpClient billingHttpClient2 = billingHttpClient1;
        HttpMethod method = new HttpMethod("PATCH");
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(BillingHttpClient.previewApiVersion, 1);
        Guid subscriptionResourceId = CommerceResourceIds.OfferSubscriptionResourceId;
        List<KeyValuePair<string, string>> keyValuePairList = queryParameters;
        var routeValues = new
        {
          resourceName = offerMeterName
        };
        ApiResourceVersion version = apiResourceVersion;
        List<KeyValuePair<string, string>> queryParameters1 = keyValuePairList;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        HttpRequestMessage message = await billingHttpClient2.CreateRequestMessageAsync(method, subscriptionResourceId, (object) routeValues, version, (HttpContent) null, queryParameters1, userState1, cancellationToken1).ConfigureAwait(false);
        message.Content = (HttpContent) new ObjectContent(subscriptionResource.GetType(), (object) subscriptionResource, billingHttpClient1.Formatter);
        HttpResponseMessage httpResponseMessage = await billingHttpClient1.SendAsync(message, userState, cancellationToken).ConfigureAwait(false);
        queryParameters = (List<KeyValuePair<string, string>>) null;
        subscriptionResource = (OfferSubscription) null;
      }
    }

    public virtual async Task CreateOfferSubscription(
      string offerMeterName,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      Guid? billingTarget = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BillingHttpClient billingHttpClient1 = this;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (CreateOfferSubscription)))
      {
        OfferSubscription requestContent = new OfferSubscription()
        {
          OfferMeter = new OfferMeter()
          {
            GalleryId = offerMeterName
          },
          AzureSubscriptionId = azureSubscriptionId,
          RenewalGroup = renewalGroup,
          CommittedQuantity = quantity
        };
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add<Guid?>(nameof (billingTarget), billingTarget);
        BillingHttpClient billingHttpClient2 = billingHttpClient1;
        HttpMethod post = HttpMethod.Post;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(BillingHttpClient.previewApiVersion, 1);
        Guid subscriptionResourceId = CommerceResourceIds.OfferSubscriptionResourceId;
        var routeValues = new
        {
          resourceName = offerMeterName
        };
        ApiResourceVersion version = apiResourceVersion;
        List<KeyValuePair<string, string>> queryParameters = collection;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        HttpRequestMessage message = await billingHttpClient2.CreateRequestMessageAsync(post, subscriptionResourceId, (object) routeValues, version, (HttpContent) null, queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
        message.Content = (HttpContent) new ObjectContent(requestContent.GetType(), (object) requestContent, billingHttpClient1.Formatter);
        HttpResponseMessage httpResponseMessage = await billingHttpClient1.SendAsync(message, userState, cancellationToken).ConfigureAwait(false);
        requestContent = (OfferSubscription) null;
      }
    }

    public virtual Task CancelOfferSubscription(
      string offerMeterName,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      string cancelReason,
      Guid? billingTarget = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.CancelOfferSubscription(offerMeterName, azureSubscriptionId, renewalGroup, cancelReason, billingTarget, false, (object) null, cancellationToken);
    }

    public virtual async Task CancelOfferSubscription(
      string offerMeterName,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      string cancelReason,
      Guid? billingTarget,
      bool immediate,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BillingHttpClient billingHttpClient1 = this;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (CancelOfferSubscription)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (cancelReason), cancelReason);
        if (billingTarget.HasValue)
          collection.Add<Guid?>(nameof (billingTarget), billingTarget);
        collection.Add<bool>(nameof (immediate), immediate);
        OfferSubscription offerSubscription = new OfferSubscription()
        {
          OfferMeter = new OfferMeter()
          {
            GalleryId = offerMeterName
          },
          AzureSubscriptionId = azureSubscriptionId,
          RenewalGroup = renewalGroup
        };
        BillingHttpClient billingHttpClient2 = billingHttpClient1;
        HttpMethod method = new HttpMethod("PATCH");
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(BillingHttpClient.previewApiVersion, 1);
        Guid subscriptionResourceId = CommerceResourceIds.OfferSubscriptionResourceId;
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        var routeValues = new
        {
          resourceName = offerMeterName
        };
        ApiResourceVersion version = apiResourceVersion;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        HttpRequestMessage message = await billingHttpClient2.CreateRequestMessageAsync(method, subscriptionResourceId, (object) routeValues, version, (HttpContent) null, queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
        message.Content = (HttpContent) new ObjectContent(offerSubscription.GetType(), (object) offerSubscription, billingHttpClient1.Formatter);
        HttpResponseMessage httpResponseMessage = await billingHttpClient1.SendAsync(message, userState, cancellationToken).ConfigureAwait(false);
        offerSubscription = (OfferSubscription) null;
      }
    }

    public virtual async Task EnableTrialOrPreviewOfferSubscription(
      string offerMeterName,
      ResourceRenewalGroup renewalGroup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BillingHttpClient billingHttpClient1 = this;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (EnableTrialOrPreviewOfferSubscription)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (offerMeterName), offerMeterName);
        collection.Add<ResourceRenewalGroup>(nameof (renewalGroup), renewalGroup);
        BillingHttpClient billingHttpClient2 = billingHttpClient1;
        HttpMethod post = HttpMethod.Post;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(BillingHttpClient.previewApiVersion, 1);
        Guid subscriptionResourceId = CommerceResourceIds.OfferSubscriptionResourceId;
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        var routeValues = new
        {
          resourceName = offerMeterName
        };
        ApiResourceVersion version = apiResourceVersion;
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        HttpRequestMessage message = await billingHttpClient2.CreateRequestMessageAsync(post, subscriptionResourceId, (object) routeValues, version, (HttpContent) null, queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
        HttpResponseMessage httpResponseMessage = await billingHttpClient1.SendAsync(message, userState, cancellationToken).ConfigureAwait(false);
      }
    }

    public virtual async Task EnableTrialOfferSubscriptionExtension(
      string offerMeterName,
      ResourceRenewalGroup renewalGroup,
      DateTime endDate,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BillingHttpClient billingHttpClient1 = this;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (EnableTrialOfferSubscriptionExtension)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (offerMeterName), offerMeterName);
        collection.Add<ResourceRenewalGroup>(nameof (renewalGroup), renewalGroup);
        collection.Add<DateTime>(nameof (endDate), endDate);
        BillingHttpClient billingHttpClient2 = billingHttpClient1;
        HttpMethod method = new HttpMethod("PATCH");
        Guid subscriptionResourceId = CommerceResourceIds.OfferSubscriptionResourceId;
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        var routeValues = new
        {
          resourceName = offerMeterName
        };
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        HttpRequestMessage message = await billingHttpClient2.CreateRequestMessageAsync(method, subscriptionResourceId, (object) routeValues, (ApiResourceVersion) null, (HttpContent) null, queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
        HttpResponseMessage httpResponseMessage = await billingHttpClient1.SendAsync(message, userState, cancellationToken).ConfigureAwait(false);
      }
    }

    public virtual async Task DecreaseResourceQuantity(
      string offerMeterName,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      bool shouldBeImmediate,
      Guid? azureSubscriptionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BillingHttpClient billingHttpClient1 = this;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (DecreaseResourceQuantity)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (offerMeterName), offerMeterName);
        collection.Add<ResourceRenewalGroup>(nameof (renewalGroup), renewalGroup);
        collection.Add<int>(nameof (quantity), quantity);
        collection.Add<bool>(nameof (shouldBeImmediate), shouldBeImmediate);
        collection.Add<Guid?>(nameof (azureSubscriptionId), azureSubscriptionId);
        BillingHttpClient billingHttpClient2 = billingHttpClient1;
        HttpMethod method = new HttpMethod("PATCH");
        Guid subscriptionResourceId = CommerceResourceIds.OfferSubscriptionResourceId;
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        var routeValues = new
        {
          resourceName = offerMeterName
        };
        List<KeyValuePair<string, string>> queryParameters = keyValuePairList;
        object userState1 = userState;
        CancellationToken cancellationToken1 = cancellationToken;
        HttpRequestMessage message = await billingHttpClient2.CreateRequestMessageAsync(method, subscriptionResourceId, (object) routeValues, (ApiResourceVersion) null, (HttpContent) null, queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
        HttpResponseMessage httpResponseMessage = await billingHttpClient1.SendAsync(message, userState, cancellationToken).ConfigureAwait(false);
      }
    }

    public virtual async Task CreatePurchaseRequest(
      PurchaseRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BillingHttpClient billingHttpClient1 = this;
      HttpContent httpContent = (HttpContent) new ObjectContent<PurchaseRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      BillingHttpClient billingHttpClient2 = billingHttpClient1;
      HttpMethod put = HttpMethod.Put;
      Guid requestLocationId = CommerceResourceIds.PurchaseRequestLocationId;
      ApiResourceVersion version = new ApiResourceVersion(BillingHttpClient.apiVersion40, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      HttpResponseMessage httpResponseMessage = await billingHttpClient2.SendAsync(put, requestLocationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
    }

    public virtual async Task UpdatePurchaseRequest(
      PurchaseRequest request,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      BillingHttpClient billingHttpClient1 = this;
      HttpContent httpContent = (HttpContent) new ObjectContent<PurchaseRequest>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      BillingHttpClient billingHttpClient2 = billingHttpClient1;
      HttpMethod method = new HttpMethod("PATCH");
      Guid requestLocationId = CommerceResourceIds.PurchaseRequestLocationId;
      ApiResourceVersion version = new ApiResourceVersion(BillingHttpClient.apiVersion40, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      HttpResponseMessage httpResponseMessage = await billingHttpClient2.SendAsync(method, requestLocationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false);
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
    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) BillingHttpClient.s_translatedExceptions;
  }
}
