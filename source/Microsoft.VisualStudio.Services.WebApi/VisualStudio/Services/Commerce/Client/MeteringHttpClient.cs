// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Client.MeteringHttpClient
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
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce.Client
{
  [ResourceArea("365D9DCD-4492-4AE3-B5BA-AD0FF4AB74B3")]
  [ClientCircuitBreakerSettings(30, 80, MaxConcurrentRequests = 40)]
  [ClientCancellationTimeout(60)]
  public class MeteringHttpClient : VssHttpClientBase
  {
    protected static readonly Version previewApiVersion = new Version(2, 0);
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
      },
      {
        "AccountQuantityException",
        typeof (AccountQuantityException)
      }
    };

    public MeteringHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public MeteringHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public MeteringHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public MeteringHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public MeteringHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task<ISubscriptionResource> GetResourceStatus(
      ResourceName? resourceName,
      bool nextBillingPeriod,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MeteringHttpClient meteringHttpClient1 = this;
      ISubscriptionResource resourceStatus;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (GetResourceStatus)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (nextBillingPeriod), nextBillingPeriod.ToString());
        MeteringHttpClient meteringHttpClient2 = meteringHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid meterLocationid = CommerceResourceIds.MeterLocationid;
        var routeValues = new
        {
          resourceName = resourceName.ToString()
        };
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion(MeteringHttpClient.previewApiVersion, 2);
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        resourceStatus = (ISubscriptionResource) await meteringHttpClient2.SendAsync<SubscriptionResource>(get, meterLocationid, (object) routeValues, version, (HttpContent) null, queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
      }
      return resourceStatus;
    }

    public virtual async Task<IEnumerable<ISubscriptionResource>> GetResourceStatus(
      bool nextBillingPeriod,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MeteringHttpClient meteringHttpClient1 = this;
      IEnumerable<ISubscriptionResource> resourceStatus;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (GetResourceStatus)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (nextBillingPeriod), nextBillingPeriod.ToString());
        MeteringHttpClient meteringHttpClient2 = meteringHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid meterLocationid = CommerceResourceIds.MeterLocationid;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion(MeteringHttpClient.previewApiVersion, 2);
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        resourceStatus = (IEnumerable<ISubscriptionResource>) await meteringHttpClient2.SendAsync<IEnumerable<SubscriptionResource>>(get, meterLocationid, (object) null, version, (HttpContent) null, queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
      }
      return resourceStatus;
    }

    public virtual async Task SetAccountQuantity(
      ResourceName name,
      int includedQuantity,
      int maximumQuantity,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MeteringHttpClient meteringHttpClient1 = this;
      using (new VssHttpClientBase.OperationScope("Commerce", "UpdateMeter"))
      {
        SubscriptionResource subscriptionResource = (SubscriptionResource) await meteringHttpClient1.GetResourceStatus(new ResourceName?(name), true, cancellationToken: cancellationToken).ConfigureAwait(false);
        subscriptionResource.IncludedQuantity = includedQuantity;
        subscriptionResource.MaximumQuantity = maximumQuantity;
        MeteringHttpClient meteringHttpClient2 = meteringHttpClient1;
        HttpMethod method = new HttpMethod("PATCH");
        Guid meterLocationid = CommerceResourceIds.MeterLocationid;
        var routeValues = new
        {
          resourceName = name.ToString()
        };
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion(MeteringHttpClient.previewApiVersion, 2);
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        HttpRequestMessage message = meteringHttpClient2.CreateRequestMessageAsync(method, meterLocationid, (object) routeValues, version, (HttpContent) null, (List<KeyValuePair<string, string>>) null, userState1, cancellationToken1).SyncResult<HttpRequestMessage>();
        message.Content = (HttpContent) new ObjectContent(subscriptionResource.GetType(), (object) subscriptionResource, meteringHttpClient1.Formatter);
        HttpResponseMessage httpResponseMessage = await meteringHttpClient1.SendAsync(message, userState, cancellationToken).ConfigureAwait(false);
      }
    }

    public virtual async Task TogglePaidBilling(
      ResourceName name,
      bool paidBillingStatus,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MeteringHttpClient meteringHttpClient1 = this;
      using (new VssHttpClientBase.OperationScope("Commerce", "UpdateMeter"))
      {
        SubscriptionResource subscriptionResource = (SubscriptionResource) await meteringHttpClient1.GetResourceStatus(new ResourceName?(name), true, cancellationToken: cancellationToken).ConfigureAwait(false);
        subscriptionResource.IsPaidBillingEnabled = paidBillingStatus;
        MeteringHttpClient meteringHttpClient2 = meteringHttpClient1;
        HttpMethod method = new HttpMethod("PATCH");
        Guid meterLocationid = CommerceResourceIds.MeterLocationid;
        var routeValues = new
        {
          resourceName = name.ToString()
        };
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion(MeteringHttpClient.previewApiVersion, 2);
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        HttpRequestMessage message = await meteringHttpClient2.CreateRequestMessageAsync(method, meterLocationid, (object) routeValues, version, (HttpContent) null, (List<KeyValuePair<string, string>>) null, userState1, cancellationToken1).ConfigureAwait(false);
        message.Content = (HttpContent) new ObjectContent(subscriptionResource.GetType(), (object) subscriptionResource, meteringHttpClient1.Formatter);
        HttpResponseMessage httpResponseMessage = await meteringHttpClient1.SendAsync(message, userState, cancellationToken).ConfigureAwait(false);
        subscriptionResource = (SubscriptionResource) null;
      }
    }

    public virtual async Task ReportUsage(
      Guid eventUserId,
      ResourceName resourceName,
      int quantity,
      string eventId,
      DateTime billingEventDateTime,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MeteringHttpClient meteringHttpClient1 = this;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (ReportUsage)))
      {
        var requestContent = new
        {
          BillableDate = billingEventDateTime,
          EventId = eventId,
          AssociatedUser = eventUserId,
          Quantity = quantity,
          ResouceName = resourceName
        };
        MeteringHttpClient meteringHttpClient2 = meteringHttpClient1;
        HttpMethod post = HttpMethod.Post;
        Guid usageEventLocationid = CommerceResourceIds.UsageEventLocationid;
        var routeValues = new
        {
          resourceName = resourceName.ToString()
        };
        CancellationToken cancellationToken1 = cancellationToken;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        HttpRequestMessage message = await meteringHttpClient2.CreateRequestMessageAsync(post, usageEventLocationid, (object) routeValues, (ApiResourceVersion) null, (HttpContent) null, (List<KeyValuePair<string, string>>) null, userState1, cancellationToken2).ConfigureAwait(false);
        message.Content = (HttpContent) new ObjectContent(requestContent.GetType(), (object) requestContent, meteringHttpClient1.Formatter);
        HttpResponseMessage httpResponseMessage = await meteringHttpClient1.SendAsync(message, userState, cancellationToken).ConfigureAwait(false);
        requestContent = null;
      }
    }

    public virtual async Task<IEnumerable<IUsageEventAggregate>> GetUsage(
      DateTime startTime,
      DateTime endTime,
      TimeSpan timeSpan,
      ResourceName resource,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IEnumerable<IUsageEventAggregate> usage;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (GetUsage)))
      {
        List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
        keyValuePairList.Add(nameof (startTime), startTime.ToString());
        keyValuePairList.Add(nameof (endTime), endTime.ToString());
        keyValuePairList.Add(nameof (timeSpan), timeSpan.ToString());
        usage = (IEnumerable<IUsageEventAggregate>) await this.SendAsync<IEnumerable<UsageEventAggregate>>(HttpMethod.Get, CommerceResourceIds.UsageEventLocationid, (object) new
        {
          resourceName = resource.ToString()
        }, (ApiResourceVersion) null, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState, cancellationToken).ConfigureAwait(false);
      }
      return usage;
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
    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) MeteringHttpClient.s_translatedExceptions;
  }
}
