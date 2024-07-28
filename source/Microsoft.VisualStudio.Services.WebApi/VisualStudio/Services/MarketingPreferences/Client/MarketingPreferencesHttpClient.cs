// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MarketingPreferences.Client.MarketingPreferencesHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.MarketingPreferences.Client
{
  [ResourceArea("F4AA2205-FF00-4EEE-8216-C7A73CEE155C")]
  [ClientCircuitBreakerSettings(15, 50, MaxConcurrentRequests = 40)]
  public class MarketingPreferencesHttpClient : VssHttpClientBase
  {
    public MarketingPreferencesHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public MarketingPreferencesHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public MarketingPreferencesHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public MarketingPreferencesHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public MarketingPreferencesHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<bool> GetContactWithOffersAsync(
      string descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<bool>(new HttpMethod("GET"), new Guid("6e529270-1f14-4e92-a11d-b496bbba4ed7"), (object) new
      {
        descriptor = descriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task SetContactWithOffersAsync(
      string descriptor,
      bool value,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MarketingPreferencesHttpClient preferencesHttpClient = this;
      HttpMethod method = new HttpMethod("PUT");
      Guid locationId = new Guid("6e529270-1f14-4e92-a11d-b496bbba4ed7");
      object routeValues = (object) new
      {
        descriptor = descriptor
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (value), value.ToString());
      using (await preferencesHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual Task<Microsoft.VisualStudio.Services.MarketingPreferences.MarketingPreferences> GetMarketingPreferencesAsync(
      string descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Microsoft.VisualStudio.Services.MarketingPreferences.MarketingPreferences>(new HttpMethod("GET"), new Guid("0e2ebf6e-1b6c-423d-b207-06b1afdfe332"), (object) new
      {
        descriptor = descriptor
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual async Task SetMarketingPreferencesAsync(
      Microsoft.VisualStudio.Services.MarketingPreferences.MarketingPreferences preferences,
      string descriptor,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      MarketingPreferencesHttpClient preferencesHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("0e2ebf6e-1b6c-423d-b207-06b1afdfe332");
      object obj1 = (object) new{ descriptor = descriptor };
      HttpContent httpContent = (HttpContent) new ObjectContent<Microsoft.VisualStudio.Services.MarketingPreferences.MarketingPreferences>(preferences, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      MarketingPreferencesHttpClient preferencesHttpClient2 = preferencesHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await preferencesHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
