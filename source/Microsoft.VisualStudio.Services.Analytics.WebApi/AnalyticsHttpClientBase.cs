// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.AnalyticsHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi
{
  [ResourceArea("{F47C4501-5E41-4A7C-B17B-19B7CEF00B91}")]
  [ClientCancellationTimeout(60)]
  [ClientCircuitBreakerSettings(10, 80)]
  public abstract class AnalyticsHttpClientBase : VssHttpClientBase
  {
    public AnalyticsHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public AnalyticsHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public AnalyticsHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public AnalyticsHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public AnalyticsHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public virtual async Task InvalidateShardAsync(
      StageShardInvalid shard,
      string table,
      int providerShard,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AnalyticsHttpClientBase analyticsHttpClientBase1 = this;
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("328a8d58-1727-4715-9a3d-e236feebd247");
      object obj1 = (object) new
      {
        table = table,
        providerShard = providerShard
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<StageShardInvalid>(shard, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      AnalyticsHttpClientBase analyticsHttpClientBase2 = analyticsHttpClientBase1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await analyticsHttpClientBase2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public virtual Task<int> GetSpaceRequirementsAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<int>(new HttpMethod("GET"), new Guid("aeefb135-59c0-4a10-a477-e1981d657175"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<StageProviderShardInfo> CreateShardAsync(
      string table,
      int providerShard,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<StageProviderShardInfo>(new HttpMethod("PUT"), new Guid("9bd3f7d0-e20d-4e7b-95ba-854704939f9e"), (object) new
      {
        table = table,
        providerShard = providerShard
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task DeleteShardAsync(
      string table,
      int providerShard,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("9bd3f7d0-e20d-4e7b-95ba-854704939f9e"), (object) new
      {
        table = table,
        providerShard = providerShard
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public virtual Task<StageProviderShardInfo> GetShardAsync(
      string table,
      int providerShard,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<StageProviderShardInfo>(new HttpMethod("GET"), new Guid("9bd3f7d0-e20d-4e7b-95ba-854704939f9e"), (object) new
      {
        table = table,
        providerShard = providerShard
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<StageTableInfo> GetTableAsync(
      string table,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<StageTableInfo>(new HttpMethod("GET"), new Guid("9bd3f7d0-e20d-4e7b-95ba-854704939f9e"), (object) new
      {
        table = table
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual Task<IngestResult> StageRecordsAsync(
      Stream uploadStream,
      string table,
      int providerShard,
      int stream,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod httpMethod = new HttpMethod("POST");
      Guid guid = new Guid("9bd3f7d0-e20d-4e7b-95ba-854704939f9e");
      object obj1 = (object) new
      {
        table = table,
        providerShard = providerShard,
        stream = stream
      };
      HttpContent httpContent = (HttpContent) new StreamContent(uploadStream);
      httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      return this.SendAsync<IngestResult>(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2);
    }

    public virtual Task<AnalyticsStateDetails> GetStateAsync(
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<AnalyticsStateDetails>(new HttpMethod("GET"), new Guid("0b79c382-d776-40b9-87b4-407fb8f7df24"), version: new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public virtual async Task UpdateStateAsync(
      AnalyticsState state,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("PATCH"), new Guid("0b79c382-d776-40b9-87b4-407fb8f7df24"), (object) new
      {
        state = state
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }
  }
}
