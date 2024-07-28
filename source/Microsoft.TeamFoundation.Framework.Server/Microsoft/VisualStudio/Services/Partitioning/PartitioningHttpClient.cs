// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Partitioning.PartitioningHttpClient
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

namespace Microsoft.VisualStudio.Services.Partitioning
{
  [ResourceArea("{0129E64E-3F98-43F8-9073-212C19D832CB}")]
  [ClientCancellationTimeout(30)]
  [ClientCircuitBreakerSettings(15, 50)]
  public class PartitioningHttpClient : VssHttpClientBase
  {
    public PartitioningHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public PartitioningHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public PartitioningHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public PartitioningHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public PartitioningHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public async Task CreatePartitionContainerAsync(
      PartitionContainer container,
      Guid containerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PartitioningHttpClient partitioningHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("55fdd96f-cbfe-461a-b0ac-890454ff434a");
      object obj1 = (object) new
      {
        containerId = containerId
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<PartitionContainer>(container, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PartitioningHttpClient partitioningHttpClient2 = partitioningHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await partitioningHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task DeletePartitionContainerAsync(
      Guid containerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("55fdd96f-cbfe-461a-b0ac-890454ff434a"), (object) new
      {
        containerId = containerId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<PartitionContainer> GetPartitionContainerAsync(
      Guid containerId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<PartitionContainer>(new HttpMethod("GET"), new Guid("55fdd96f-cbfe-461a-b0ac-890454ff434a"), (object) new
      {
        containerId = containerId
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<PartitionContainer>> QueryPartitionContainersAsync(
      Guid? containerType = null,
      IEnumerable<string> requiredTags = null,
      bool? isAcquirable = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("55fdd96f-cbfe-461a-b0ac-890454ff434a");
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (containerType.HasValue)
        keyValuePairList.Add(nameof (containerType), containerType.Value.ToString());
      if (requiredTags != null && requiredTags.Any<string>())
        keyValuePairList.Add(nameof (requiredTags), string.Join(",", requiredTags));
      if (isAcquirable.HasValue)
        keyValuePairList.Add(nameof (isAcquirable), isAcquirable.Value.ToString());
      return this.SendAsync<List<PartitionContainer>>(method, locationId, version: new ApiResourceVersion(7.2, 1), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task CreatePartitionAsync(
      Partition partition,
      string partitionKey,
      Guid containerType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PartitioningHttpClient partitioningHttpClient1 = this;
      HttpMethod httpMethod = new HttpMethod("PUT");
      Guid guid = new Guid("4ece3a4b-1d02-4313-8843-dd7b02c8f639");
      object obj1 = (object) new
      {
        partitionKey = partitionKey,
        containerType = containerType
      };
      HttpContent httpContent = (HttpContent) new ObjectContent<Partition>(partition, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true));
      PartitioningHttpClient partitioningHttpClient2 = partitioningHttpClient1;
      HttpMethod method = httpMethod;
      Guid locationId = guid;
      object routeValues = obj1;
      ApiResourceVersion version = new ApiResourceVersion(7.2, 1);
      object obj2 = userState;
      CancellationToken cancellationToken1 = cancellationToken;
      HttpContent content = httpContent;
      object userState1 = obj2;
      CancellationToken cancellationToken2 = cancellationToken1;
      using (await partitioningHttpClient2.SendAsync(method, locationId, routeValues, version, content, userState: userState1, cancellationToken: cancellationToken2).ConfigureAwait(false))
        ;
    }

    public async Task DeletePartitionAsync(
      string partitionKey,
      Guid containerType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      using (await this.SendAsync(new HttpMethod("DELETE"), new Guid("4ece3a4b-1d02-4313-8843-dd7b02c8f639"), (object) new
      {
        partitionKey = partitionKey,
        containerType = containerType
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false))
        ;
    }

    public Task<Partition> QueryPartitionAsync(
      string partitionKey,
      Guid containerType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<Partition>(new HttpMethod("GET"), new Guid("4ece3a4b-1d02-4313-8843-dd7b02c8f639"), (object) new
      {
        partitionKey = partitionKey,
        containerType = containerType
      }, new ApiResourceVersion(7.2, 1), userState: userState, cancellationToken: cancellationToken);
    }
  }
}
