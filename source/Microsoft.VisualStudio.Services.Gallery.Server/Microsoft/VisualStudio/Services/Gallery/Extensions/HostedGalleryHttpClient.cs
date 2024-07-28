// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Extensions.HostedGalleryHttpClient
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Extensions
{
  internal class HostedGalleryHttpClient : VssHttpClientBase, IHostedGalleryHttpClient
  {
    private Uri m_baseUrl;

    public HostedGalleryHttpClient(Uri baseUrl)
      : base(baseUrl, new VssCredentials())
    {
      this.m_baseUrl = baseUrl;
    }

    public async Task<Stream> GetAsset(
      string assetUrl,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HostedGalleryHttpClient galleryHttpClient = this;
      HttpMethod httpMethod = new HttpMethod("GET");
      HttpRequestMessage assetRequestMessage1 = galleryHttpClient.CreateAssetRequestMessage(httpMethod, assetUrl, "application/octet-stream");
      HttpResponseMessage httpResponseMessage = await galleryHttpClient.SendAsync(assetRequestMessage1, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      if (httpResponseMessage.StatusCode == HttpStatusCode.Found && httpResponseMessage.Headers.Location != (Uri) null)
      {
        HttpRequestMessage assetRequestMessage2 = galleryHttpClient.CreateAssetRequestMessage(httpMethod, httpResponseMessage.Headers.Location.ToString(), "application/octet-stream");
        httpResponseMessage = await galleryHttpClient.SendAsync(assetRequestMessage2, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
      }
      httpResponseMessage.EnsureSuccessStatusCode();
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    public async Task<ExtensionQueryResult> GetQueryResult(
      string queryURL,
      ExtensionQuery query,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HostedGalleryHttpClient galleryHttpClient = this;
      HttpMethod method = new HttpMethod("POST");
      MediaTypeWithQualityHeaderValue mediaType = new MediaTypeWithQualityHeaderValue("application/json");
      mediaType.Parameters.Add(new NameValueHeaderValue("api-version", "3.0-preview.1"));
      HttpContent httpContent = (HttpContent) new ObjectContent<ExtensionQuery>(query, (MediaTypeFormatter) new VssJsonMediaTypeFormatter(true), (MediaTypeHeaderValue) mediaType);
      // ISSUE: explicit non-virtual call
      string requestUri = __nonvirtual (galleryHttpClient.BaseAddress)?.ToString() + queryURL;
      return await galleryHttpClient.SendAsync<ExtensionQueryResult>(new HttpRequestMessage(method, requestUri)
      {
        Content = httpContent,
        Headers = {
          Accept = {
            mediaType
          }
        }
      }, cancellationToken: cancellationToken);
    }

    protected HttpRequestMessage CreateAssetRequestMessage(
      HttpMethod method,
      string assetUrl,
      string mediaType)
    {
      MediaTypeWithQualityHeaderValue qualityHeaderValue = new MediaTypeWithQualityHeaderValue(mediaType);
      return new HttpRequestMessage(method, assetUrl)
      {
        Headers = {
          Accept = {
            qualityHeaderValue
          }
        }
      };
    }

    protected override bool ShouldThrowError(HttpResponseMessage response) => response.StatusCode != HttpStatusCode.Found && !response.IsSuccessStatusCode;
  }
}
