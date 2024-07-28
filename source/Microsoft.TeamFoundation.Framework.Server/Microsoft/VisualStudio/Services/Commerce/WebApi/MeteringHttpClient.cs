// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.WebApi.MeteringHttpClient
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce.WebApi
{
  [ResourceArea("4C19F9C8-67BD-4C18-800B-55DC62C3017F")]
  [ClientCircuitBreakerSettings(30, 80, MaxConcurrentRequests = 40)]
  [ClientCancellationTimeout(60)]
  public class MeteringHttpClient : VssHttpClientBase
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
      using (new VssHttpClientBase.OperationScope("Meters", nameof (GetResourceStatus)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (nextBillingPeriod), nextBillingPeriod.ToString());
        MeteringHttpClient meteringHttpClient2 = meteringHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid meterLocationid = CommerceServiceResourceIds.MeterLocationid;
        var routeValues = new
        {
          resourceName = resourceName.ToString()
        };
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
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
      using (new VssHttpClientBase.OperationScope("Meters", nameof (GetResourceStatus)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (nextBillingPeriod), nextBillingPeriod.ToString());
        MeteringHttpClient meteringHttpClient2 = meteringHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid meterLocationid = CommerceServiceResourceIds.MeterLocationid;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
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
      using (new VssHttpClientBase.OperationScope("Meters", "UpdateMeter"))
      {
        SubscriptionResource subscriptionResource = (SubscriptionResource) await meteringHttpClient1.GetResourceStatus(new ResourceName?(name), true, cancellationToken: cancellationToken).ConfigureAwait(false);
        subscriptionResource.IncludedQuantity = includedQuantity;
        subscriptionResource.MaximumQuantity = maximumQuantity;
        MeteringHttpClient meteringHttpClient2 = meteringHttpClient1;
        HttpMethod method = new HttpMethod("PATCH");
        Guid meterLocationid = CommerceServiceResourceIds.MeterLocationid;
        var routeValues = new
        {
          resourceName = name.ToString()
        };
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
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
      using (new VssHttpClientBase.OperationScope("Meters", "UpdateMeter"))
      {
        SubscriptionResource subscriptionResource = (SubscriptionResource) await meteringHttpClient1.GetResourceStatus(new ResourceName?(name), true, cancellationToken: cancellationToken).ConfigureAwait(false);
        subscriptionResource.IsPaidBillingEnabled = paidBillingStatus;
        MeteringHttpClient meteringHttpClient2 = meteringHttpClient1;
        HttpMethod method = new HttpMethod("PATCH");
        Guid meterLocationid = CommerceServiceResourceIds.MeterLocationid;
        var routeValues = new
        {
          resourceName = name.ToString()
        };
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        HttpRequestMessage message = await meteringHttpClient2.CreateRequestMessageAsync(method, meterLocationid, (object) routeValues, version, (HttpContent) null, (List<KeyValuePair<string, string>>) null, userState1, cancellationToken1).ConfigureAwait(false);
        message.Content = (HttpContent) new ObjectContent(subscriptionResource.GetType(), (object) subscriptionResource, meteringHttpClient1.Formatter);
        HttpResponseMessage httpResponseMessage = await meteringHttpClient1.SendAsync(message, userState, cancellationToken).ConfigureAwait(false);
        subscriptionResource = (SubscriptionResource) null;
      }
    }

    public virtual async Task<HttpResponseMessage> ReportUsage(
      Guid eventUserId,
      ResourceName resourceName,
      int quantity,
      string eventId,
      DateTime billingEventDateTime,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MeteringHttpClient meteringHttpClient1 = this;
      HttpResponseMessage httpResponseMessage;
      using (new VssHttpClientBase.OperationScope("UsageEvents", nameof (ReportUsage)))
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
        Guid usageEventLocationid = CommerceServiceResourceIds.UsageEventLocationid;
        var routeValues = new
        {
          resourceName = resourceName.ToString()
        };
        ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
        CancellationToken cancellationToken1 = cancellationToken;
        object userState1 = userState;
        CancellationToken cancellationToken2 = cancellationToken1;
        HttpRequestMessage message = await meteringHttpClient2.CreateRequestMessageAsync(post, usageEventLocationid, (object) routeValues, version, (HttpContent) null, (List<KeyValuePair<string, string>>) null, userState1, cancellationToken2).ConfigureAwait(false);
        message.Content = (HttpContent) new ObjectContent(requestContent.GetType(), (object) requestContent, meteringHttpClient1.Formatter);
        httpResponseMessage = await meteringHttpClient1.SendAsync(message, userState, cancellationToken).ConfigureAwait(false);
      }
      return httpResponseMessage;
    }

    public virtual async Task<IEnumerable<IUsageEventAggregate>> GetUsage(
      DateTime startTime,
      DateTime endTime,
      TimeSpan timeSpan,
      ResourceName resource,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MeteringHttpClient meteringHttpClient1 = this;
      IEnumerable<IUsageEventAggregate> usage;
      using (new VssHttpClientBase.OperationScope("UsageEvents", nameof (GetUsage)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (startTime), startTime.ToUniversalTime().ToString("u"));
        collection.Add(nameof (endTime), endTime.ToUniversalTime().ToString("u"));
        collection.Add(nameof (timeSpan), timeSpan.ToString("c"));
        MeteringHttpClient meteringHttpClient2 = meteringHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid usageEventLocationid = CommerceServiceResourceIds.UsageEventLocationid;
        var routeValues = new
        {
          resourceName = resource.ToString()
        };
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        usage = (IEnumerable<IUsageEventAggregate>) await meteringHttpClient2.SendAsync<IEnumerable<UsageEventAggregate>>(get, usageEventLocationid, (object) routeValues, version, (HttpContent) null, queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
      }
      return usage;
    }

    public virtual async Task UpdateMeter(
      SubscriptionResource meter,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MeteringHttpClient meteringHttpClient1 = this;
      using (new VssHttpClientBase.OperationScope("Meters", nameof (UpdateMeter)))
      {
        MeteringHttpClient meteringHttpClient2 = meteringHttpClient1;
        HttpMethod method = new HttpMethod("PATCH");
        Guid meterLocationid = CommerceServiceResourceIds.MeterLocationid;
        var routeValues = new
        {
          resourceName = meter.Name.ToString()
        };
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion("5.0-preview.1");
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        HttpRequestMessage message = meteringHttpClient2.CreateRequestMessageAsync(method, meterLocationid, (object) routeValues, version, (HttpContent) null, (List<KeyValuePair<string, string>>) null, userState1, cancellationToken1).SyncResult<HttpRequestMessage>();
        message.Content = (HttpContent) new ObjectContent(meter.GetType(), (object) meter, meteringHttpClient1.Formatter);
        HttpResponseMessage httpResponseMessage = await meteringHttpClient1.SendAsync(message, userState, cancellationToken).ConfigureAwait(false);
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
    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) MeteringHttpClient.s_translatedExceptions;
  }
}
