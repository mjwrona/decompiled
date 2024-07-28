// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Client.OfferMeterPriceHttpClient
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
  [ClientCircuitBreakerSettings(15, 80, MaxConcurrentRequests = 30)]
  [ClientCancellationTimeout(30)]
  public class OfferMeterPriceHttpClient : VssHttpClientBase
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
      }
    };

    public OfferMeterPriceHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public OfferMeterPriceHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public OfferMeterPriceHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public OfferMeterPriceHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public OfferMeterPriceHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task<IList<OfferMeterPrice>> GetOfferMeterPrice(
      string galleryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      OfferMeterPriceHttpClient meterPriceHttpClient1 = this;
      IList<OfferMeterPrice> offerMeterPrice;
      using (new VssHttpClientBase.OperationScope("Commerce", nameof (GetOfferMeterPrice)))
      {
        List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
        collection.Add(nameof (galleryId), galleryId);
        OfferMeterPriceHttpClient meterPriceHttpClient2 = meterPriceHttpClient1;
        HttpMethod get = HttpMethod.Get;
        Guid meterPriceLocationId = CommerceResourceIds.OfferMeterPriceLocationId;
        IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
        object obj = userState;
        ApiResourceVersion version = new ApiResourceVersion(OfferMeterPriceHttpClient.previewApiVersion, 1);
        IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
        object userState1 = obj;
        CancellationToken cancellationToken1 = cancellationToken;
        offerMeterPrice = await meterPriceHttpClient2.SendAsync<IList<OfferMeterPrice>>(get, meterPriceLocationId, (object) null, version, (HttpContent) null, queryParameters, userState1, cancellationToken1).ConfigureAwait(false);
      }
      return offerMeterPrice;
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

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) OfferMeterPriceHttpClient.s_translatedExceptions;
  }
}
