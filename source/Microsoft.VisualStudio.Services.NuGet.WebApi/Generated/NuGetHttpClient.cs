// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.Generated.NuGetHttpClient
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi.Generated
{
  [ResourceArea("{B3BE7473-68EA-4A81-BFC7-9530BAAA19AD}")]
  public class NuGetHttpClient : NuGetHttpClientBase
  {
    public NuGetHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public NuGetHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public NuGetHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public NuGetHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public NuGetHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public override async Task<Stream> DownloadPackageAsync(
      string feedId,
      string packageName,
      string packageVersion,
      string sourceProtocolVersion = null,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NuGetHttpClient nuGetHttpClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("6ea81b8c-7386-490b-a71f-6cf23c80b388");
      object routeValues = (object) new
      {
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(sourceProtocolVersion))
        keyValuePairList.Add(nameof (sourceProtocolVersion), sourceProtocolVersion);
      HttpResponseMessage httpResponseMessage = await nuGetHttpClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion("3.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken);
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task SetPackageVersionListedAsync(
      PackageVersionDetails packageVersionDetails,
      string feedId,
      string packageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      await this.UpdatePackageVersionAsync(packageVersionDetails, feedId, packageName, packageVersion, userState, cancellationToken);
    }
  }
}
