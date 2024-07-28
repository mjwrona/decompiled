// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphCompatHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Graph.Client
{
  [ResourceArea("BB1E7EC9-E901-4B68-999A-DE7012B920F8")]
  [ClientCircuitBreakerSettings(20, 80, MaxConcurrentRequests = 55)]
  public class GraphCompatHttpClientBase : VssHttpClientBase
  {
    public GraphCompatHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public GraphCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public GraphCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public GraphCompatHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public GraphCompatHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task<PagedGraphUsers> ListUsersAsync(
      IEnumerable<string> subjectTypes = null,
      string continuationToken = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GraphCompatHttpClientBase compatHttpClientBase = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("005e26ec-6b77-4e4f-a986-b3827bf241f5");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (subjectTypes != null && subjectTypes.Any<string>())
        keyValuePairList.Add(nameof (subjectTypes), string.Join(",", subjectTypes));
      if (continuationToken != null)
        keyValuePairList.Add(nameof (continuationToken), continuationToken);
      PagedGraphUsers pagedGraphUsers1;
      using (HttpRequestMessage requestMessage = await compatHttpClientBase.CreateRequestMessageAsync(method, locationId, version: new ApiResourceVersion("5.2-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken).ConfigureAwait(false))
      {
        PagedGraphUsers returnObject = new PagedGraphUsers();
        using (HttpResponseMessage response = await compatHttpClientBase.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false))
        {
          response.EnsureSuccessStatusCode();
          returnObject.ContinuationToken = compatHttpClientBase.GetHeaderValue(response, "X-MS-ContinuationToken");
          PagedGraphUsers pagedGraphUsers = returnObject;
          pagedGraphUsers.GraphUsers = (IEnumerable<GraphUser>) await compatHttpClientBase.ReadContentAsAsync<List<GraphUser>>(response, cancellationToken).ConfigureAwait(false);
          pagedGraphUsers = (PagedGraphUsers) null;
        }
        pagedGraphUsers1 = returnObject;
      }
      return pagedGraphUsers1;
    }
  }
}
