// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.ItemHttpClientBase
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public abstract class ItemHttpClientBase : ArtifactHttpClient
  {
    private static readonly ApiResourceVersion DefaultVersion = new ApiResourceVersion(1.0, 1);
    private const string ItemHttpClientEnvironmentVariablePrefix = "VSO_AS_HTTP_";
    private static readonly ParallelHttpDownload.DownloadConfiguration ParallelSegmentDownloadConfig = ParallelHttpDownload.DownloadConfiguration.ReadFromEnvironment();

    public ItemHttpClientBase(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public ItemHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public ItemHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public ItemHttpClientBase(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public ItemHttpClientBase(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<HttpResponseMessage> AssociateAsync(
      AssociationsItem item,
      Guid resourceId,
      object routeValues = null,
      ApiResourceVersion version = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.PatchAsync((StoredItem) item, resourceId, routeValues, version, queryParameters, cancellationToken);
    }

    protected async Task<HttpResponseMessage> GetAllowRedirectAsync(
      Guid resourceId,
      object routeValues = null,
      ApiResourceVersion version = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ItemHttpClientBase itemHttpClientBase = this;
      HttpRequestMessage message = await itemHttpClientBase.CreateRequestMessageAsync(HttpMethod.Get, resourceId, routeValues, version, queryParameters: queryParameters, cancellationToken: cancellationToken).ConfigureAwait(false);
      message.Properties.Add("ExpectedStatus", (object) new HttpStatusCode[1]
      {
        HttpStatusCode.SeeOther
      });
      HttpResponseMessage originalResponse = await itemHttpClientBase.SendAsync(message, completionOption, (object) null, cancellationToken);
      return await itemHttpClientBase.HandleRedirectAsync(originalResponse, cancellationToken).ConfigureAwait(false);
    }

    protected async Task<T> GetAllowRedirectAsync<T>(
      Guid resourceId,
      object routeValues = null,
      ApiResourceVersion version = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : Item
    {
      return await (await this.HandleRedirectAsync(await this.GetAllowRedirectAsync(resourceId, routeValues, version, queryParameters, completionOption, cancellationToken).ConfigureAwait(false), cancellationToken).ConfigureAwait(false)).Content.ReadItemAsync<T>().ConfigureAwait(false);
    }

    protected Task<HttpResponseMessage> DeleteAsync(
      Item item,
      Guid resourceId,
      object routeValues = null,
      ApiResourceVersion version = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync(HttpMethod.Delete, item, resourceId, routeValues, version, queryParameters, cancellationToken);
    }

    protected Task<HttpResponseMessage> PatchAsync(
      StoredItem item,
      Guid resourceId,
      object routeValues = null,
      ApiResourceVersion version = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync(new HttpMethod("PATCH"), (Item) item, resourceId, routeValues, version, queryParameters, cancellationToken);
    }

    protected async Task<T> PostAsync<T>(
      HttpContent content,
      Guid resourceId,
      object routeValues = null,
      ApiResourceVersion version = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : Item
    {
      return await (await this.SendAsync(HttpMethod.Post, content, resourceId, routeValues, version, queryParameters, completionOption, cancellationToken).ConfigureAwait(false)).Content.ReadItemAsync<T>().ConfigureAwait(false);
    }

    protected Task<HttpResponseMessage> PostAsync(
      Item item,
      Guid resourceId,
      object routeValues = null,
      ApiResourceVersion version = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync(HttpMethod.Post, item, resourceId, routeValues, version, queryParameters, cancellationToken);
    }

    protected Task<HttpResponseMessage> PutAsync(
      Item item,
      Guid resourceId,
      object routeValues = null,
      ApiResourceVersion version = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.SendAsync(HttpMethod.Put, item, resourceId, routeValues, version, queryParameters, cancellationToken);
    }

    protected Task<HttpResponseMessage> SendAsync(
      HttpMethod method,
      Item item,
      Guid resourceId,
      object routeValues = null,
      ApiResourceVersion version = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      StringContent content = item == null ? (StringContent) null : new StringContent(item.ToJson().ToString(), StrictEncodingWithoutBOM.UTF8, "application/json");
      return this.SendAsync(method, resourceId, routeValues, version, (HttpContent) content, queryParameters, (object) version, cancellationToken);
    }

    protected Task<HttpResponseMessage> SendAsync(
      HttpMethod method,
      HttpContent content,
      Guid resourceId,
      object routeValues = null,
      ApiResourceVersion version = null,
      IEnumerable<KeyValuePair<string, string>> queryParameters = null,
      HttpCompletionOption completionOption = HttpCompletionOption.ResponseContentRead,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      version = version ?? ItemHttpClientBase.DefaultVersion;
      return this.SendAsync(method, resourceId, completionOption, routeValues, version, content, queryParameters, cancellationToken: cancellationToken);
    }

    protected virtual IDownloader GetDownloader(
      ParallelHttpDownload.DownloadConfiguration configuration,
      IAppTraceSource tracer,
      Guid correlationId,
      HttpClient httpClient = null)
    {
      return (IDownloader) new DefaultDownloader(configuration, tracer, correlationId, httpClient);
    }

    private async Task<HttpResponseMessage> HandleRedirectAsync(
      HttpResponseMessage originalResponse,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ItemHttpClientBase itemHttpClientBase = this;
      if (!ItemHttpClientBase.IsRedirect(originalResponse.StatusCode))
        return originalResponse;
      originalResponse.Headers.Location.Scheme.ToUpperInvariant();
      IAppTraceSource tracer = itemHttpClientBase.tracer == null ? (IAppTraceSource) NoopAppTraceSource.Instance : itemHttpClientBase.tracer;
      IDownloader downloader = itemHttpClientBase.GetDownloader(ItemHttpClientBase.ParallelSegmentDownloadConfig, tracer, VssClientHttpRequestSettings.Default.SessionId);
      using (TempFile tempFile = new TempFile((IFileSystem) FileSystem.Instance, Path.GetTempPath()))
      {
        System.IO.File.Create(tempFile.Path).Dispose();
        DownloadResult downloadResult = await downloader.DownloadAsync(tempFile.Path, originalResponse.Headers.Location.AbsoluteUri, new long?(), cancellationToken).ConfigureAwait(false);
        byte[] content = System.IO.File.Exists(tempFile.Path) ? System.IO.File.ReadAllBytes(tempFile.Path) : throw new VssServiceResponseException(HttpStatusCode.NotFound, "File not exists", (Exception) null);
        if ((long) content.Length != downloadResult.BytesDownloaded)
          throw new EndOfStreamException(string.Format("The download is not complete, FileSize: {0} bytes, Downloaded: {1} bytes", (object) content.Length, (object) downloadResult.BytesDownloaded));
        return new HttpResponseMessage(downloadResult.HttpStatusCode)
        {
          Content = (HttpContent) new ByteArrayContent(content)
        };
      }
    }

    private static bool IsRedirect(HttpStatusCode statusCode)
    {
      switch (statusCode)
      {
        case HttpStatusCode.Found:
        case HttpStatusCode.SeeOther:
          return true;
        default:
          return false;
      }
    }
  }
}
