// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Client.Internal.NpmApiClient
// Assembly: Microsoft.VisualStudio.Services.Npm.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC112C7A-461D-4C09-ACCC-062015D895F6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Client.Internal.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Npm.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Client.Internal
{
  public class NpmApiClient : InternalNpmHttpClient
  {
    public NpmApiClient(Uri baseUrl, VssCredentials credentials, VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public NpmApiClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public NpmApiClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public NpmApiClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public NpmApiClient(Uri protocolEndpoint, VssCredentials creds)
      : base(protocolEndpoint, creds)
    {
    }

    public virtual async Task ForcePackageUpstreamRefreshAsync(
      string feedId,
      string packageScope,
      string unscopedPackageName,
      string packageVersion,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmApiClient npmApiClient = this;
      HttpMethod method = new HttpMethod("HEAD");
      int num = !string.IsNullOrEmpty(packageScope) ? 1 : 0;
      Guid locationId = num != 0 ? new Guid("09a4eafd-123a-495c-979c-0eda7bdb9a14") : new Guid("75caa482-cb1e-47cd-9f2c-c048a4b7a43e");
      object routeValues = num == 0 ? (object) new
      {
        feedId = feedId,
        packageName = unscopedPackageName,
        packageVersion = packageVersion
      } : (object) new
      {
        feedId = feedId,
        packageScope = packageScope,
        unscopedPackageName = unscopedPackageName,
        packageVersion = packageVersion
      };
      (await npmApiClient.SendAsync(method, locationId, routeValues, new ApiResourceVersion("5.1-preview.1"), userState: userState, cancellationToken: cancellationToken)).EnsureSuccessStatusCode();
    }

    public override async Task<Stream> GetContentUnscopedPackageInternalAsync(
      string project,
      string feedId,
      string packageName,
      string packageVersion,
      Guid aadTenantId,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      NpmApiClient npmApiClient = this;
      HttpMethod method = new HttpMethod("GET");
      Guid locationId = new Guid("a498b0e6-cd4d-483e-b0c4-c8b8e98d9309");
      object routeValues = (object) new
      {
        project = project,
        feedId = feedId,
        packageName = packageName,
        packageVersion = packageVersion
      };
      List<KeyValuePair<string, string>> keyValuePairList = new List<KeyValuePair<string, string>>();
      keyValuePairList.Add(nameof (aadTenantId), aadTenantId.ToString());
      HttpResponseMessage httpResponseMessage;
      using (HttpRequestMessage requestMessage = await npmApiClient.CreateRequestMessageAsync(method, locationId, routeValues, new ApiResourceVersion("6.0-preview.1"), queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, cancellationToken: cancellationToken, mediaType: "application/octet-stream").ConfigureAwait(false))
        httpResponseMessage = await npmApiClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      if (NpmApiClient.IsRedirect(httpResponseMessage.StatusCode) && httpResponseMessage.Headers.Location != (Uri) null)
        return await new HttpClient().GetStreamAsync(httpResponseMessage.Headers.Location.ToString());
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    protected override Task HandleResponseAsync(
      HttpResponseMessage response,
      CancellationToken cancellationToken)
    {
      return response.StatusCode != HttpStatusCode.SeeOther && !NpmApiClient.IsRedirect(response.StatusCode) ? base.HandleResponseAsync(response, cancellationToken) : (Task) Task.FromResult<bool>(true);
    }

    private static bool IsRedirect(HttpStatusCode statusCode) => statusCode == HttpStatusCode.MovedPermanently || statusCode == HttpStatusCode.Found || statusCode == HttpStatusCode.SeeOther;
  }
}
