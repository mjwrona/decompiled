// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tracing.WebApi.TracingHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Tracing.WebApi
{
  public class TracingHttpClient : VssHttpClientBase
  {
    public TracingHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public TracingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public TracingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public TracingHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public TracingHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<TraceFilter> AddTraceFilterAsync(
      TraceFilter traceFilter,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("a317c8da-01a3-4d01-8b11-0d2115810ad6");
      HttpContent httpContent = (HttpContent) new ObjectContent<TraceFilter>(traceFilter, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      ApiResourceVersion version = new ApiResourceVersion(6.0, 3);
      object obj = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<TraceFilter>(method, locationId, version: version, content: content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public Task<TraceFilter> GetTraceFilterAsync(
      Guid traceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TraceFilter>(new HttpMethod("GET"), new Guid("a317c8da-01a3-4d01-8b11-0d2115810ad6"), (object) new
      {
        traceId = traceId
      }, new ApiResourceVersion(6.0, 3), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<TraceFilter>> GetTraceFiltersAsync(
      Guid? ownerId = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a317c8da-01a3-4d01-8b11-0d2115810ad6");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (ownerId.HasValue)
        keyValuePairList.Add(nameof (ownerId), ownerId.Value.ToString());
      return this.SendAsync<List<TraceFilter>>(method, locationId, version: new ApiResourceVersion(6.0, 3), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task RemoveTraceFilterAsync(
      Guid traceId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("a317c8da-01a3-4d01-8b11-0d2115810ad6"), (object) new
      {
        traceId = traceId
      }, new ApiResourceVersion(6.0, 3), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }
  }
}
