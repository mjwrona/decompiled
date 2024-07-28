// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NameResolution.NameResolutionHttpClient
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NameResolution
{
  [ResourceArea("{81AEC033-EAE2-42B8-82F6-90B93A662EF5}")]
  [ClientCancellationTimeout(30)]
  [ClientCircuitBreakerSettings(20, 50)]
  public class NameResolutionHttpClient : VssHttpClientBase
  {
    public NameResolutionHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public NameResolutionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public NameResolutionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public NameResolutionHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public NameResolutionHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task DeleteNameResolutionEntryAsync(
      string @namespace,
      string name,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("cae3d437-cd60-485a-b8b0-ce6acf234e44"), (object) new
      {
        @namespace = @namespace,
        name = name
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<NameResolutionEntry> GetNameResolutionEntryAsync(
      string @namespace,
      string name,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<NameResolutionEntry>(new HttpMethod("GET"), new Guid("cae3d437-cd60-485a-b8b0-ce6acf234e44"), (object) new
      {
        @namespace = @namespace,
        name = name
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<NameResolutionEntry>> QueryNameResolutionEntriesAsync(
      IEnumerable<string> namespaces = null,
      string name = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("cae3d437-cd60-485a-b8b0-ce6acf234e44");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (namespaces != null && namespaces.Any<string>())
        keyValuePairList.Add(nameof (namespaces), string.Join(",", namespaces));
      if (name != null)
        keyValuePairList.Add(nameof (name), name);
      return this.SendAsync<List<NameResolutionEntry>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<NameResolutionEntry>> QueryNameResolutionEntriesForValueAsync(
      Guid value,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("cae3d437-cd60-485a-b8b0-ce6acf234e44");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (value), value.ToString());
      return this.SendAsync<List<NameResolutionEntry>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task SetNameResolutionEntryAsync(
      NameResolutionEntry entry,
      string @namespace,
      string name,
      bool? overwriteExisting = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NameResolutionHttpClient resolutionHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("cae3d437-cd60-485a-b8b0-ce6acf234e44");
      object obj1 = (object) new
      {
        @namespace = @namespace,
        name = name
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<NameResolutionEntry>(entry, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (overwriteExisting.HasValue)
        collection.Add(nameof (overwriteExisting), overwriteExisting.Value.ToString());
      NameResolutionHttpClient resolutionHttpClient2 = resolutionHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      IEnumerable<KeyValuePair<string, string>> keyValuePairs = (IEnumerable<KeyValuePair<string, string>>) collection;
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      IEnumerable<KeyValuePair<string, string>> queryParameters = keyValuePairs;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await resolutionHttpClient2.SendAsync(method, locationId, routeValues, version, content, queryParameters, userState1, cancellationToken2).ConfigureAwait(false))
        ;
    }
  }
}
