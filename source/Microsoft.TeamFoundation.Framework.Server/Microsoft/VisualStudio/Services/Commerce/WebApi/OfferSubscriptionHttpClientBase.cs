// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.WebApi.OfferSubscriptionHttpClientBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Commerce.WebApi
{
  [ResourceArea("5D4A2F52-5A08-41FB-8CCA-768ADD070E18")]
  public abstract class OfferSubscriptionHttpClientBase : VssHttpClientBase
  {
    public OfferSubscriptionHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public OfferSubscriptionHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public OfferSubscriptionHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public OfferSubscriptionHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public OfferSubscriptionHttpClientBase(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task CancelOfferSubscriptionAsync(
      OfferSubscription offerSubscription,
      string cancelReason,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      OfferSubscriptionHttpClientBase subscriptionHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("7c13d166-01c5-4ccd-8a75-e5ad6ab3b0a6");
      HttpContent httpContent = (HttpContent) new ObjectContent<OfferSubscription>(offerSubscription, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (cancelReason), cancelReason);
      OfferSubscriptionHttpClientBase subscriptionHttpClientBase2 = subscriptionHttpClientBase1;
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
      using (await subscriptionHttpClientBase2.SendAsync(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task CancelOfferSubscriptionAsync(
      OfferSubscription offerSubscription,
      string cancelReason,
      Guid billingTarget,
      bool? immediate = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      OfferSubscriptionHttpClientBase subscriptionHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("7c13d166-01c5-4ccd-8a75-e5ad6ab3b0a6");
      HttpContent httpContent = (HttpContent) new ObjectContent<OfferSubscription>(offerSubscription, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      collection.Add(nameof (cancelReason), cancelReason);
      collection.Add(nameof (billingTarget), billingTarget.ToString());
      if (immediate.HasValue)
        collection.Add(nameof (immediate), immediate.Value.ToString());
      OfferSubscriptionHttpClientBase subscriptionHttpClientBase2 = subscriptionHttpClientBase1;
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
      using (await subscriptionHttpClientBase2.SendAsync(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task CreateOfferSubscriptionAsync(
      OfferSubscription offerSubscription,
      string offerCode = null,
      Guid? tenantId = null,
      Guid? objectId = null,
      Guid? billingTarget = null,
      bool? skipSubscriptionValidation = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      OfferSubscriptionHttpClientBase subscriptionHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("7c13d166-01c5-4ccd-8a75-e5ad6ab3b0a6");
      HttpContent httpContent = (HttpContent) new ObjectContent<OfferSubscription>(offerSubscription, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (offerCode != null)
        collection.Add(nameof (offerCode), offerCode);
      if (tenantId.HasValue)
        collection.Add(nameof (tenantId), tenantId.Value.ToString());
      if (objectId.HasValue)
        collection.Add(nameof (objectId), objectId.Value.ToString());
      if (billingTarget.HasValue)
        collection.Add(nameof (billingTarget), billingTarget.Value.ToString());
      if (skipSubscriptionValidation.HasValue)
        collection.Add(nameof (skipSubscriptionValidation), skipSubscriptionValidation.Value.ToString());
      OfferSubscriptionHttpClientBase subscriptionHttpClientBase2 = subscriptionHttpClientBase1;
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
      using (await subscriptionHttpClientBase2.SendAsync(method, locationId, version: version, content: content, queryParameters: queryParameters, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual async Task DecreaseResourceQuantityAsync(
      string offerMeterName,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      bool shouldBeImmediate,
      Guid azureSubscriptionId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      OfferSubscriptionHttpClientBase subscriptionHttpClientBase = this;
      HttpMethod method = new HttpMethod("PATCH");
      Guid locationId = new Guid("7c13d166-01c5-4ccd-8a75-e5ad6ab3b0a6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (offerMeterName), offerMeterName);
      keyValuePairList.Add(nameof (renewalGroup), renewalGroup.ToString());
      keyValuePairList.Add(nameof (quantity), quantity.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (shouldBeImmediate), shouldBeImmediate.ToString());
      keyValuePairList.Add(nameof (azureSubscriptionId), azureSubscriptionId.ToString());
      using (await subscriptionHttpClientBase.SendAsync(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task EnableTrialOfferSubscriptionExtensionAsync(
      string offerMeterName,
      ResourceRenewalGroup renewalGroup,
      DateTime endDate,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      OfferSubscriptionHttpClientBase subscriptionHttpClientBase = this;
      HttpMethod method = new HttpMethod("PATCH");
      Guid locationId = new Guid("7c13d166-01c5-4ccd-8a75-e5ad6ab3b0a6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (offerMeterName), offerMeterName);
      keyValuePairList.Add(nameof (renewalGroup), renewalGroup.ToString());
      subscriptionHttpClientBase.AddDateTimeToQueryParams((IList<KeyValuePair<string, string>>) keyValuePairList, nameof (endDate), endDate);
      using (await subscriptionHttpClientBase.SendAsync(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task EnableTrialOrPreviewOfferSubscriptionAsync(
      string offerMeterName,
      ResourceRenewalGroup renewalGroup,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      OfferSubscriptionHttpClientBase subscriptionHttpClientBase = this;
      HttpMethod method = new HttpMethod("POST");
      Guid locationId = new Guid("7c13d166-01c5-4ccd-8a75-e5ad6ab3b0a6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (offerMeterName), offerMeterName);
      keyValuePairList.Add(nameof (renewalGroup), renewalGroup.ToString());
      using (await subscriptionHttpClientBase.SendAsync(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<List<OfferSubscription>> GetAllOfferSubscriptionsForUserAsync(
      bool validateAzuresubscription,
      bool nextBillingPeriod,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7c13d166-01c5-4ccd-8a75-e5ad6ab3b0a6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (validateAzuresubscription), validateAzuresubscription.ToString());
      keyValuePairList.Add(nameof (nextBillingPeriod), nextBillingPeriod.ToString());
      return this.SendAsync<List<OfferSubscription>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<OfferSubscription> GetOfferSubscriptionAsync(
      string galleryId,
      bool? nextBillingPeriod = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7c13d166-01c5-4ccd-8a75-e5ad6ab3b0a6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (galleryId), galleryId);
      if (nextBillingPeriod.HasValue)
        keyValuePairList.Add(nameof (nextBillingPeriod), nextBillingPeriod.Value.ToString());
      return this.SendAsync<OfferSubscription>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<OfferSubscription> GetOfferSubscriptionForRenewalGroupAsync(
      string galleryId,
      ResourceRenewalGroup renewalGroup,
      bool? nextBillingPeriod = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7c13d166-01c5-4ccd-8a75-e5ad6ab3b0a6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (galleryId), galleryId);
      keyValuePairList.Add(nameof (renewalGroup), renewalGroup.ToString());
      if (nextBillingPeriod.HasValue)
        keyValuePairList.Add(nameof (nextBillingPeriod), nextBillingPeriod.Value.ToString());
      return this.SendAsync<OfferSubscription>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<OfferSubscription>> GetOfferSubscriptionsAsync(
      bool? nextBillingPeriod = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7c13d166-01c5-4ccd-8a75-e5ad6ab3b0a6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (nextBillingPeriod.HasValue)
        keyValuePairList.Add(nameof (nextBillingPeriod), nextBillingPeriod.Value.ToString());
      return this.SendAsync<List<OfferSubscription>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<List<OfferSubscription>> GetOfferSubscriptionsForGalleryItemAsync(
      string galleryItemId,
      Guid azureSubscriptionId,
      bool? nextBillingPeriod = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("7c13d166-01c5-4ccd-8a75-e5ad6ab3b0a6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (galleryItemId), galleryItemId);
      keyValuePairList.Add(nameof (azureSubscriptionId), azureSubscriptionId.ToString());
      if (nextBillingPeriod.HasValue)
        keyValuePairList.Add(nameof (nextBillingPeriod), nextBillingPeriod.Value.ToString());
      return this.SendAsync<List<OfferSubscription>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task SetAccountQuantityAsync(
      string offerMeterName,
      ResourceRenewalGroup meterRenewalGroup,
      int newIncludedQuantity,
      int newMaximumQuantity,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      OfferSubscriptionHttpClientBase subscriptionHttpClientBase = this;
      HttpMethod method = new HttpMethod("PATCH");
      Guid locationId = new Guid("7c13d166-01c5-4ccd-8a75-e5ad6ab3b0a6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (offerMeterName), offerMeterName);
      keyValuePairList.Add(nameof (meterRenewalGroup), meterRenewalGroup.ToString());
      keyValuePairList.Add(nameof (newIncludedQuantity), newIncludedQuantity.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      keyValuePairList.Add(nameof (newMaximumQuantity), newMaximumQuantity.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      using (await subscriptionHttpClientBase.SendAsync(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual async Task UpdateOfferSubscriptionAsync(
      OfferSubscription offerSubscription,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      OfferSubscriptionHttpClientBase subscriptionHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("PATCH");
      Guid guid = new Guid("7c13d166-01c5-4ccd-8a75-e5ad6ab3b0a6");
      HttpContent httpContent = (HttpContent) new ObjectContent<OfferSubscription>(offerSubscription, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      OfferSubscriptionHttpClientBase subscriptionHttpClientBase2 = subscriptionHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await subscriptionHttpClientBase2.SendAsync(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
