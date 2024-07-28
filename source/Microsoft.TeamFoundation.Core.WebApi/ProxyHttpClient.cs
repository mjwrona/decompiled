// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.ProxyHttpClient
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [ResourceArea("79134C72-4A58-4B42-976C-04E7115F32BF")]
  public class ProxyHttpClient : VssHttpClientBase
  {
    public ProxyHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ProxyHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ProxyHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ProxyHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ProxyHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<Proxy> CreateOrUpdateProxyAsync(
      Proxy proxy,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("ec1f4311-f2b4-4c15-b2b8-8990b80d2908");
      HttpContent httpContent = (HttpContent) new ObjectContent<Proxy>(proxy, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 2);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<Proxy>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public async Task DeleteProxyAsync(
      string proxyUrl,
      string site = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ProxyHttpClient proxyHttpClient = this;
      HttpMethod method = new HttpMethod("DELETE");
      Guid locationId = new Guid("ec1f4311-f2b4-4c15-b2b8-8990b80d2908");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (proxyUrl), proxyUrl);
      if (site != null)
        keyValuePairList.Add(nameof (site), site);
      using (await proxyHttpClient.SendAsync(method, locationId, version: new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<List<Proxy>> GetProxiesAsync(
      string proxyUrl = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("ec1f4311-f2b4-4c15-b2b8-8990b80d2908");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (proxyUrl != null)
        keyValuePairList.Add(nameof (proxyUrl), proxyUrl);
      return this.SendAsync<List<Proxy>>(method, locationId, version: new ApiResourceVersion(7.2, 2), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }
  }
}
