// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.WebApi.OfferMeterHttpClientBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce.WebApi
{
  [ResourceArea("000080C1-AA68-4FCE-BBC5-C68D94BFF8BE")]
  public abstract class OfferMeterHttpClientBase : VssHttpClientBase
  {
    public OfferMeterHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public OfferMeterHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public OfferMeterHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public OfferMeterHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public OfferMeterHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task CreateOfferMeterDefinitionAsync(
      OfferMeter offerConfig,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      OfferMeterHttpClientBase meterHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("81e37548-a9e0-49f9-8905-650a7260a440");
      HttpContent httpContent = (HttpContent) new ObjectContent<OfferMeter>(offerConfig, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      OfferMeterHttpClientBase meterHttpClientBase2 = meterHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await meterHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<OfferMeter> GetOfferMeterAsync(
      string resourceName,
      string resourceNameResolveMethod,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("81e37548-a9e0-49f9-8905-650a7260a440");
      object routeValues = (object) new
      {
        resourceName = resourceName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (resourceNameResolveMethod), resourceNameResolveMethod);
      return this.SendAsync<OfferMeter>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<OfferMeter>> GetOfferMetersAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<List<OfferMeter>>(new HttpMethod("GET"), new Guid("81e37548-a9e0-49f9-8905-650a7260a440"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<PurchasableOfferMeter> GetPurchasableOfferMeterAsync(
      string resourceName,
      string resourceNameResolveMethod,
      Guid subscriptionId,
      bool includeMeterPricing,
      string offerCode = null,
      Guid? tenantId = null,
      Guid? objectId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("81e37548-a9e0-49f9-8905-650a7260a440");
      object routeValues = (object) new
      {
        resourceName = resourceName
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (resourceNameResolveMethod), resourceNameResolveMethod);
      keyValuePairList.Add(nameof (subscriptionId), subscriptionId.ToString());
      keyValuePairList.Add(nameof (includeMeterPricing), includeMeterPricing.ToString());
      if (offerCode != null)
        keyValuePairList.Add(nameof (offerCode), offerCode);
      Guid guid;
      if (tenantId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        guid = tenantId.Value;
        string str = guid.ToString();
        collection.Add(nameof (tenantId), str);
      }
      if (objectId.HasValue)
      {
        List<KeyValuePair<string, string>> collection = keyValuePairList;
        guid = objectId.Value;
        string str = guid.ToString();
        collection.Add(nameof (objectId), str);
      }
      return this.SendAsync<PurchasableOfferMeter>(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<OfferMeterPrice>> GetOfferMeterPriceAsync(
      string galleryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("d7197e00-dddf-4029-9f9b-21b935a6cf9f");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (galleryId), galleryId);
      return this.SendAsync<List<OfferMeterPrice>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task UpdateOfferMeterPriceAsync(
      IEnumerable<OfferMeterPrice> offerMeterPricing,
      string galleryId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      OfferMeterHttpClientBase meterHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("d7197e00-dddf-4029-9f9b-21b935a6cf9f");
      HttpContent httpContent = (HttpContent) new ObjectContent<IEnumerable<OfferMeterPrice>>(offerMeterPricing, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (galleryId), galleryId);
      OfferMeterHttpClientBase meterHttpClientBase2 = meterHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await meterHttpClientBase2.SendAsync(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
