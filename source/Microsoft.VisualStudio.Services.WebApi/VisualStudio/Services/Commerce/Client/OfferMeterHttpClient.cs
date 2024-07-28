// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Client.OfferMeterHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

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
  [ClientCircuitBreakerSettings(30, 80, MaxConcurrentRequests = 40)]
  [ClientCancellationTimeout(60)]
  public class OfferMeterHttpClient : VssHttpClientBase
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
      }
    };

    public OfferMeterHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public OfferMeterHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public OfferMeterHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public OfferMeterHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public OfferMeterHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task<IOfferMeter> GetMeterFromGalleryId(
      string resourceName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      OfferMeterHttpClient offerMeterHttpClient1 = this;
      IOfferMeter meterFromGalleryId;
      using (new VssHttpClientBase.OperationScope("Commerce", "GetOfferMeter"))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add("resourceNameResolveMethod", "GalleryId");
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        OfferMeterHttpClient offerMeterHttpClient2 = offerMeterHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid offerMeterLocationId = CommerceResourceIds.OfferMeterLocationId;
        var routeValues = new{ resourceName = resourceName };
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion(OfferMeterHttpClient.previewApiVersion, 1);
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        meterFromGalleryId = (IOfferMeter) await offerMeterHttpClient2.SendAsync<OfferMeter>(get, offerMeterLocationId, (object) routeValues, version, (HttpContent) null, queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
      }
      return meterFromGalleryId;
    }

    public virtual async Task<IOfferMeter> GetMeterFromMeterName(
      string resourceName,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      OfferMeterHttpClient offerMeterHttpClient1 = this;
      IOfferMeter meterFromMeterName;
      using (new VssHttpClientBase.OperationScope("Commerce", "GetOfferMeter"))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add("resourceNameResolveMethod", "MeterName");
        OfferMeterHttpClient offerMeterHttpClient2 = offerMeterHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid offerMeterLocationId = CommerceResourceIds.OfferMeterLocationId;
        var routeValues = new{ resourceName = resourceName };
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion(OfferMeterHttpClient.previewApiVersion, 1);
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        meterFromMeterName = (IOfferMeter) await offerMeterHttpClient2.SendAsync<OfferMeter>(get, offerMeterLocationId, (object) routeValues, version, (HttpContent) null, queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
      }
      return meterFromMeterName;
    }

    public virtual async Task<List<OfferMeter>> GetMeters(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      OfferMeterHttpClient offerMeterHttpClient1 = this;
      List<OfferMeter> meters;
      using (new VssHttpClientBase.OperationScope("Commerce", "GetOfferMeters"))
      {
        OfferMeterHttpClient offerMeterHttpClient2 = offerMeterHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid offerMeterLocationId = CommerceResourceIds.OfferMeterLocationId;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion(OfferMeterHttpClient.previewApiVersion, 1);
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        meters = await offerMeterHttpClient2.SendAsync<List<OfferMeter>>(get, offerMeterLocationId, (object) null, version, (HttpContent) null, (IEnumerable<KeyValuePair<string, string>>) null, userState1, cancellationToken1).ConfigureAwait(false);
      }
      return meters;
    }

    public virtual async Task CreateOfferMeterDefinition(
      IOfferMeter meterConfig,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      OfferMeterHttpClient offerMeterHttpClient1 = this;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (CreateOfferMeterDefinition)))
      {
        HttpContent httpContent = (HttpContent) new ObjectContent<IOfferMeter>(meterConfig, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
        OfferMeterHttpClient offerMeterHttpClient2 = offerMeterHttpClient1;
        HttpMethod post = HttpMethod.Post;
        ApiResourceVersion apiResourceVersion = new ApiResourceVersion(OfferMeterHttpClient.previewApiVersion, 1);
        Guid offerMeterLocationId = CommerceResourceIds.OfferMeterLocationId;
        ApiResourceVersion version = apiResourceVersion;
        object obj = userState;
        HttpContent content = httpContent;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        HttpRequestMessage message = await offerMeterHttpClient2.CreateRequestMessageAsync(post, offerMeterLocationId, (object) null, version, content, (List<KeyValuePair<string, string>>) null, userState1, cancellationToken1).ConfigureAwait(false);
        HttpResponseMessage httpResponseMessage = await offerMeterHttpClient1.SendAsync(message, userState, cancellationToken).ConfigureAwait(false);
      }
    }

    public virtual async Task<PurchasableOfferMeter> GetPurchasableOfferMeter(
      string resourceName,
      string resourceNameResolveMethod,
      Guid? subscriptionId,
      bool includeMeterPricing,
      string offerCode = null,
      Guid? tenantId = null,
      Guid? objectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      OfferMeterHttpClient offerMeterHttpClient1 = this;
      PurchasableOfferMeter purchasableOfferMeter;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (GetPurchasableOfferMeter)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (resourceName), resourceName);
        collection.Add(nameof (resourceNameResolveMethod), resourceNameResolveMethod);
        collection.Add<Guid?>(nameof (subscriptionId), subscriptionId);
        collection.Add<bool>(nameof (includeMeterPricing), includeMeterPricing);
        collection.Add(nameof (offerCode), offerCode);
        collection.Add<Guid?>(nameof (tenantId), tenantId);
        collection.Add<Guid?>(nameof (objectId), objectId);
        List<KeyValuePair<string, string>> keyValuePairList = collection;
        OfferMeterHttpClient offerMeterHttpClient2 = offerMeterHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid offerMeterLocationId = CommerceResourceIds.OfferMeterLocationId;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) keyValuePairList;
        var routeValues = new
        {
          resourceName = resourceName,
          resourceNameResolveMethod = resourceNameResolveMethod,
          subscriptionId = subscriptionId,
          includeMeterPricing = includeMeterPricing,
          offerCode = offerCode,
          tenantId = tenantId,
          objectId = objectId
        };
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        CancellationToken cancellationToken1 = cancellationToken;
        purchasableOfferMeter = await offerMeterHttpClient2.SendAsync<PurchasableOfferMeter>(get, offerMeterLocationId, (object) routeValues, (ApiResourceVersion) null, (HttpContent) null, queryParameters, (object) null, cancellationToken1).ConfigureAwait(false);
      }
      return purchasableOfferMeter;
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
    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) OfferMeterHttpClient.s_translatedExceptions;
  }
}
