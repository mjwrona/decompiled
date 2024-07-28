// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.Legacy.LegacyTaskAgentPackageHttpClient
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi.Legacy
{
  public class LegacyTaskAgentPackageHttpClient : TaskAgentHttpClientBase
  {
    public LegacyTaskAgentPackageHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public LegacyTaskAgentPackageHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public LegacyTaskAgentPackageHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public LegacyTaskAgentPackageHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public LegacyTaskAgentPackageHttpClient(
      Uri baseUrl,
      HttpMessageHandler pipeline,
      bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<TaskPackageMetadata> GetPackageAsync(
      string packageType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync<TaskPackageMetadata>(new HttpMethod("GET"), new Guid("8ffcd551-079c-493a-9c02-54346299d144"), (object) new
      {
        packageType = packageType
      }, new ApiResourceVersion("1.0"), userState: userState, cancellationToken: cancellationToken);
    }

    public async Task<Stream> GetPackageZipAsync(
      string packageType,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      LegacyTaskAgentPackageHttpClient packageHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("8ffcd551-079c-493a-9c02-54346299d144");
      object routeValues = (object) new
      {
        packageType = packageType
      };
      HttpRequestMessage message = await packageHttpClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("1.0"), cancellationToken: cancellationToken, mediaType: "application/zip").ConfigureAwait(false);
      HttpResponseMessage httpResponseMessage = await packageHttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }
  }
}
