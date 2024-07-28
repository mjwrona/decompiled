// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.FileContainer.Client.FileContainerHttpClient
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.FileContainer.Client
{
  public class FileContainerHttpClient : VssHttpClientBase
  {
    private const int c_defaultChunkSize = 8388608;
    private const int c_defaultChunkRetryTimes = 3;
    private const int c_maxChunkSize = 25165824;
    private const int c_ContentChunkMultiple = 2097152;
    private static Dictionary<string, Type> s_translatedExceptions = new Dictionary<string, Type>();
    private static readonly ApiResourceVersion s_currentApiVersion;

    public event EventHandler<ReportTraceEventArgs> UploadFileReportTrace;

    public event EventHandler<ReportProgressEventArgs> UploadFileReportProgress;

    static FileContainerHttpClient()
    {
      FileContainerHttpClient.s_translatedExceptions.Add("ArtifactUriNotSupportedException", typeof (ArtifactUriNotSupportedException));
      FileContainerHttpClient.s_translatedExceptions.Add("ContainerAlreadyExistsException", typeof (ContainerAlreadyExistsException));
      FileContainerHttpClient.s_translatedExceptions.Add("ContainerItemCopyDuplicateTargetsException", typeof (ContainerItemCopyDuplicateTargetsException));
      FileContainerHttpClient.s_translatedExceptions.Add("ContainerItemCopySourcePendingUploadException", typeof (ContainerItemCopySourcePendingUploadException));
      FileContainerHttpClient.s_translatedExceptions.Add("ContainerItemCopyTargetChildOfSourceException", typeof (ContainerItemCopyTargetChildOfSourceException));
      FileContainerHttpClient.s_translatedExceptions.Add("ContainerItemExistsException", typeof (ContainerItemExistsException));
      FileContainerHttpClient.s_translatedExceptions.Add("ContainerItemNotFoundException", typeof (ContainerItemNotFoundException));
      FileContainerHttpClient.s_translatedExceptions.Add("ContainerNoContentException", typeof (ContainerNoContentException));
      FileContainerHttpClient.s_translatedExceptions.Add("ContainerNotFoundException", typeof (ContainerNotFoundException));
      FileContainerHttpClient.s_translatedExceptions.Add("ContainerUnexpectedContentTypeException", typeof (ContainerUnexpectedContentTypeException));
      FileContainerHttpClient.s_translatedExceptions.Add("ContainerWriteAccessDeniedException", typeof (ContainerWriteAccessDeniedException));
      FileContainerHttpClient.s_translatedExceptions.Add("PendingUploadNotFoundException", typeof (PendingUploadNotFoundException));
      FileContainerHttpClient.s_currentApiVersion = new ApiResourceVersion(1.0, 4);
    }

    public FileContainerHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public FileContainerHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public FileContainerHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public FileContainerHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public FileContainerHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    public Task<List<Microsoft.VisualStudio.Services.FileContainer.FileContainer>> QueryContainersAsync(
      List<Uri> artifactUris,
      Guid scopeIdentifier,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      List<KeyValuePair<string, string>> queryParameters = this.AppendContainerQueryString(artifactUris, scopeIdentifier);
      return this.SendAsync<List<Microsoft.VisualStudio.Services.FileContainer.FileContainer>>(HttpMethod.Get, FileContainerResourceIds.FileContainer, version: FileContainerHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public Task<List<FileContainerItem>> QueryContainerItemsAsync(
      long containerId,
      Guid scopeIdentifier,
      string itemPath = null,
      object userState = null,
      bool includeDownloadTickets = false,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.QueryContainerItemsAsync(containerId, scopeIdentifier, false, itemPath, userState, includeDownloadTickets, cancellationToken);
    }

    public Task<List<FileContainerItem>> QueryContainerItemsAsync(
      long containerId,
      Guid scopeIdentifier,
      bool isShallow,
      string itemPath = null,
      object userState = null,
      bool includeDownloadTickets = false,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.QueryContainerItemsAsync(containerId, scopeIdentifier, isShallow, false, itemPath, userState, includeDownloadTickets, cancellationToken);
    }

    public Task<List<FileContainerItem>> QueryContainerItemsAsync(
      long containerId,
      Guid scopeIdentifier,
      bool isShallow,
      bool includeBlobMetadata,
      string itemPath = null,
      object userState = null,
      bool includeDownloadTickets = false,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      if (containerId < 1L)
        throw new ArgumentException(WebApiResources.ContainerIdMustBeGreaterThanZero(), nameof (containerId));
      List<KeyValuePair<string, string>> queryParameters = this.AppendItemQueryString(itemPath, scopeIdentifier, includeDownloadTickets, isShallow, includeBlobMetadata);
      return this.SendAsync<List<FileContainerItem>>(HttpMethod.Get, FileContainerResourceIds.FileContainer, (object) new
      {
        containerId = containerId
      }, FileContainerHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken);
    }

    public async Task<HttpResponseMessage> CreateItemForArtifactUpload(
      long containerId,
      string itemPath,
      Guid scopeIdentifier,
      string artifactHash,
      long fileLength,
      CancellationToken cancellationToken = default (CancellationToken),
      object userState = null)
    {
      FileContainerHttpClient containerHttpClient = this;
      List<KeyValuePair<string, string>> keyValuePairList = containerHttpClient.AppendItemQueryString(itemPath, scopeIdentifier);
      keyValuePairList.Add(nameof (artifactHash), artifactHash);
      keyValuePairList.Add<long>(nameof (fileLength), fileLength);
      HttpRequestMessage message = await containerHttpClient.CreateRequestMessageAsync(HttpMethod.Put, FileContainerResourceIds.FileContainer, (object) new
      {
        containerId = containerId
      }, FileContainerHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) keyValuePairList, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      return await containerHttpClient.SendAsync(message, userState, cancellationToken).ConfigureAwait(false);
    }

    public async Task<HttpResponseMessage> UploadFileAsync(
      long containerId,
      string itemPath,
      Stream fileStream,
      Guid scopeIdentifier,
      CancellationToken cancellationToken = default (CancellationToken),
      int chunkSize = 8388608,
      bool uploadFirstChunk = false,
      object userState = null,
      bool compressStream = true)
    {
      FileContainerHttpClient containerHttpClient = this;
      if (containerId < 1L)
        throw new ArgumentException(WebApiResources.ContainerIdMustBeGreaterThanZero(), nameof (containerId));
      ArgumentUtility.CheckForNull<Stream>(fileStream, nameof (fileStream));
      if (fileStream.Length == 0L)
      {
        List<KeyValuePair<string, string>> queryParameters = containerHttpClient.AppendItemQueryString(itemPath, scopeIdentifier);
        HttpRequestMessage message = await containerHttpClient.CreateRequestMessageAsync(HttpMethod.Put, FileContainerResourceIds.FileContainer, (object) new
        {
          containerId = containerId
        }, FileContainerHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
        return await containerHttpClient.SendAsync(message, userState, cancellationToken).ConfigureAwait(false);
      }
      ApiResourceVersion gzipSupportedVersion = new ApiResourceVersion(new Version(1, 0), 2);
      ApiResourceVersion apiResourceVersion = await containerHttpClient.NegotiateRequestVersionAsync(FileContainerResourceIds.FileContainer, FileContainerHttpClient.s_currentApiVersion, userState, cancellationToken).ConfigureAwait(false);
      if (compressStream && (apiResourceVersion.ApiVersion < gzipSupportedVersion.ApiVersion || apiResourceVersion.ApiVersion == gzipSupportedVersion.ApiVersion && apiResourceVersion.ResourceVersion < gzipSupportedVersion.ResourceVersion))
        compressStream = false;
      Stream streamToUpload = fileStream;
      bool gzipped = false;
      long filelength = fileStream.Length;
      try
      {
        if (compressStream)
        {
          streamToUpload = filelength <= (long) ushort.MaxValue ? (Stream) new MemoryStream((int) filelength + 8) : (Stream) System.IO.File.Create(Path.GetTempFileName(), 32768, FileOptions.Asynchronous | FileOptions.DeleteOnClose);
          using (GZipStream zippedStream = new GZipStream(streamToUpload, CompressionMode.Compress, true))
            await fileStream.CopyToAsync((Stream) zippedStream).ConfigureAwait(false);
          if (streamToUpload.Length >= filelength)
          {
            streamToUpload.Dispose();
            streamToUpload = fileStream;
          }
          else
            gzipped = true;
          streamToUpload.Seek(0L, SeekOrigin.Begin);
        }
        return await containerHttpClient.UploadFileAsync(containerId, itemPath, streamToUpload, (byte[]) null, filelength, gzipped, scopeIdentifier, cancellationToken, chunkSize, uploadFirstChunk: uploadFirstChunk, userState: userState);
      }
      finally
      {
        int num;
        if (num < 0 && gzipped && streamToUpload != null)
          streamToUpload.Dispose();
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task<HttpResponseMessage> UploadFileAsync(
      long containerId,
      string itemPath,
      Stream fileStream,
      byte[] contentId,
      long fileLength,
      bool isGzipped,
      Guid scopeIdentifier,
      CancellationToken cancellationToken = default (CancellationToken),
      int chunkSize = 8388608,
      int chunkRetryTimes = 3,
      bool uploadFirstChunk = false,
      object userState = null)
    {
      FileContainerHttpClient containerHttpClient = this;
      if (containerId < 1L)
        throw new ArgumentException(WebApiResources.ContainerIdMustBeGreaterThanZero(), nameof (containerId));
      if (chunkSize > 25165824)
        chunkSize = 25165824;
      if (contentId != null && chunkSize % 2097152 != 0)
        throw new ArgumentException(FileContainerResources.ChunksizeWrongWithContentId((object) 2097152), nameof (chunkSize));
      ArgumentUtility.CheckForNull<Stream>(fileStream, nameof (fileStream));
      ApiResourceVersion gzipSupportedVersion = new ApiResourceVersion(new Version(1, 0), 2);
      ApiResourceVersion apiResourceVersion = await containerHttpClient.NegotiateRequestVersionAsync(FileContainerResourceIds.FileContainer, FileContainerHttpClient.s_currentApiVersion, userState, cancellationToken).ConfigureAwait(false);
      if (isGzipped && (apiResourceVersion.ApiVersion < gzipSupportedVersion.ApiVersion || apiResourceVersion.ApiVersion == gzipSupportedVersion.ApiVersion && apiResourceVersion.ResourceVersion < gzipSupportedVersion.ResourceVersion))
        throw new ArgumentException(FileContainerResources.GzipNotSupportedOnServer(), nameof (isGzipped));
      if (isGzipped && fileStream.Length >= fileLength)
        throw new ArgumentException(FileContainerResources.BadCompression(), nameof (fileLength));
      HttpRequestMessage requestMessage = (HttpRequestMessage) null;
      List<KeyValuePair<string, string>> query = containerHttpClient.AppendItemQueryString(itemPath, scopeIdentifier);
      if (fileStream.Length == 0L)
      {
        containerHttpClient.FileUploadTrace(itemPath, "Upload zero byte file '" + itemPath + "'.");
        requestMessage = await containerHttpClient.CreateRequestMessageAsync(HttpMethod.Put, FileContainerResourceIds.FileContainer, (object) new
        {
          containerId = containerId
        }, FileContainerHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) query, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
        return await containerHttpClient.SendAsync(requestMessage, userState, cancellationToken).ConfigureAwait(false);
      }
      bool multiChunk = false;
      int totalChunks = 1;
      if (fileStream.Length > (long) chunkSize)
      {
        totalChunks = (int) Math.Ceiling((double) fileStream.Length / (double) chunkSize);
        containerHttpClient.FileUploadTrace(itemPath, string.Format("Begin chunking upload file '{0}', chunk size '{1} Bytes', total chunks '{2}'.", (object) itemPath, (object) chunkSize, (object) totalChunks));
        multiChunk = true;
      }
      else
      {
        containerHttpClient.FileUploadTrace(itemPath, "File '" + itemPath + "' will be uploaded in one chunk.");
        chunkSize = (int) fileStream.Length;
      }
      StreamParser streamParser = new StreamParser(fileStream, chunkSize);
      SubStream currentStream = streamParser.GetNextStream();
      HttpResponseMessage response = (HttpResponseMessage) null;
      byte[] dataToSend = new byte[chunkSize];
      int currentChunk = 0;
      Stopwatch uploadTimer = new Stopwatch();
      while (currentStream.Length > 0L && !cancellationToken.IsCancellationRequested)
      {
        ++currentChunk;
        for (int attempt = 1; attempt <= chunkRetryTimes && !cancellationToken.IsCancellationRequested; ++attempt)
        {
          if (attempt > 1)
          {
            TimeSpan randomBackoff = BackoffTimerHelper.GetRandomBackoff(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(10.0));
            containerHttpClient.FileUploadTrace(itemPath, string.Format("Backoff {0} seconds before attempt '{1}' chunk '{2}' of file '{3}'.", (object) randomBackoff.TotalSeconds, (object) attempt, (object) currentChunk, (object) itemPath));
            await Task.Delay(randomBackoff, cancellationToken).ConfigureAwait(false);
            currentStream.Seek(0L, SeekOrigin.Begin);
          }
          containerHttpClient.FileUploadTrace(itemPath, string.Format("Attempt '{0}' for uploading chunk '{1}' of file '{2}'.", (object) attempt, (object) currentChunk, (object) itemPath));
          int bytesToCopy = (int) currentStream.Length;
          using (MemoryStream ms = new MemoryStream(dataToSend))
            await currentStream.CopyToAsync((Stream) ms, bytesToCopy, cancellationToken).ConfigureAwait(false);
          HttpContent byteArrayContent = (HttpContent) new ByteArrayContent(dataToSend, 0, bytesToCopy);
          byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
          byteArrayContent.Headers.ContentLength = new long?(currentStream.Length);
          byteArrayContent.Headers.ContentRange = new ContentRangeHeaderValue(currentStream.StartingPostionOnOuterStream, currentStream.EndingPostionOnOuterStream, streamParser.Length);
          containerHttpClient.FileUploadTrace(itemPath, string.Format("Generate new HttpRequest for uploading file '{0}', chunk '{1}' of '{2}'.", (object) itemPath, (object) currentChunk, (object) totalChunks));
          try
          {
            if (requestMessage != null)
            {
              requestMessage.Dispose();
              requestMessage = (HttpRequestMessage) null;
            }
            requestMessage = await containerHttpClient.CreateRequestMessageAsync(HttpMethod.Put, FileContainerResourceIds.FileContainer, (object) new
            {
              containerId = containerId
            }, FileContainerHttpClient.s_currentApiVersion, byteArrayContent, (IEnumerable<KeyValuePair<string, string>>) query, userState, cancellationToken).ConfigureAwait(false);
          }
          catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
          {
            throw;
          }
          catch (Exception ex) when (attempt < chunkRetryTimes)
          {
            containerHttpClient.FileUploadTrace(itemPath, string.Format("Chunk '{0}' attempt '{1}' of file '{2}' fail to create HttpRequest. Error: {3}.", (object) currentChunk, (object) attempt, (object) itemPath, (object) ex.ToString()));
            continue;
          }
          if (isGzipped)
          {
            byteArrayContent.Headers.ContentEncoding.Add("gzip");
            byteArrayContent.Headers.Add("x-tfs-filelength", fileLength.ToString((IFormatProvider) CultureInfo.InvariantCulture));
          }
          if (contentId != null)
            byteArrayContent.Headers.Add("x-vso-contentId", Convert.ToBase64String(contentId));
          containerHttpClient.FileUploadTrace(itemPath, string.Format("Start uploading file '{0}' to server, chunk '{1}'.", (object) itemPath, (object) currentChunk));
          uploadTimer.Restart();
          try
          {
            if (response != null)
            {
              response.Dispose();
              response = (HttpResponseMessage) null;
            }
            response = await containerHttpClient.SendAsync(requestMessage, userState, cancellationToken).ConfigureAwait(false);
          }
          catch (OperationCanceledException ex) when (cancellationToken.IsCancellationRequested)
          {
            throw;
          }
          catch (Exception ex) when (attempt < chunkRetryTimes)
          {
            containerHttpClient.FileUploadTrace(itemPath, string.Format("Chunk '{0}' attempt '{1}' of file '{2}' fail to send request to server. Error: {3}.", (object) currentChunk, (object) attempt, (object) itemPath, (object) ex.ToString()));
            continue;
          }
          uploadTimer.Stop();
          containerHttpClient.FileUploadTrace(itemPath, string.Format("Finished upload chunk '{0}' of file '{1}', elapsed {2} (ms), response code '{3}'.", (object) currentChunk, (object) itemPath, (object) uploadTimer.ElapsedMilliseconds, (object) response.StatusCode));
          if (multiChunk)
            containerHttpClient.FileUploadProgress(itemPath, currentChunk, (int) Math.Ceiling((double) fileStream.Length / (double) chunkSize));
          if (!response.IsSuccessStatusCode)
            containerHttpClient.FileUploadTrace(itemPath, string.Format("Chunk '{0}' attempt '{1}' of file '{2}' received non-success status code {3} for sending request.", (object) currentChunk, (object) attempt, (object) itemPath, (object) response.StatusCode));
          else
            break;
        }
        if (response.IsSuccessStatusCode)
        {
          if (contentId != null && response.StatusCode == HttpStatusCode.Created)
          {
            containerHttpClient.FileUploadTrace(itemPath, "Stop chunking upload the rest of the file '" + itemPath + "', since server already has all the content.");
            break;
          }
          currentStream = streamParser.GetNextStream();
          if (uploadFirstChunk)
            break;
        }
        else
          break;
      }
      cancellationToken.ThrowIfCancellationRequested();
      return response;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task<List<FileContainerItem>> CreateItemsAsync(
      long containerId,
      List<FileContainerItem> items,
      Guid scopeIdentifier,
      CancellationToken cancellationToken = default (CancellationToken),
      object userState = null)
    {
      FileContainerHttpClient containerHttpClient = this;
      List<FileContainerItem> updatedItems = items.Select<FileContainerItem, FileContainerItem>((Func<FileContainerItem, FileContainerItem>) (x =>
      {
        x.ContainerId = containerId;
        x.Status = ContainerItemStatus.PendingUpload;
        return x;
      })).ToList<FileContainerItem>();
      try
      {
        return await containerHttpClient.PostAsync<List<FileContainerItem>, List<FileContainerItem>>(updatedItems, FileContainerResourceIds.FileContainer, (object) new
        {
          containerId = containerId,
          scopeIdentifier = scopeIdentifier
        }, FileContainerHttpClient.s_currentApiVersion, userState: userState, cancellationToken: cancellationToken);
      }
      catch (Exception ex)
      {
        return updatedItems;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<HttpResponseMessage> UploadFileToTfsAsync(
      long containerId,
      string itemPath,
      Stream fileStream,
      Guid scopeIdentifier,
      CancellationToken cancellationToken,
      int chunkSize = 8388608,
      bool uploadFirstChunk = false,
      object userState = null)
    {
      return this.UploadFileAsync(containerId, itemPath, fileStream, scopeIdentifier, cancellationToken, chunkSize, uploadFirstChunk, userState);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public Task<HttpResponseMessage> DownloadFileFromTfsAsync(Uri requestUri, Stream stream) => this.Client.DownloadFileFromTfsAsync(requestUri, stream);

    public Task<Stream> DownloadFileAsync(
      long containerId,
      string itemPath,
      CancellationToken cancellationToken,
      Guid scopeIdentifier,
      object userState = null)
    {
      return this.DownloadAsync(containerId, itemPath, "application/octet-stream", cancellationToken, scopeIdentifier, userState);
    }

    public Task<Stream> DownloadItemAsZipAsync(
      long containerId,
      string itemPath,
      CancellationToken cancellationToken,
      Guid scopeIdentifier,
      object userState = null)
    {
      return this.DownloadAsync(containerId, itemPath, "application/zip", cancellationToken, scopeIdentifier, userState);
    }

    public Task DeleteContainerItem(
      long containerId,
      string itemPath,
      Guid scopeIdentifier,
      CancellationToken cancellationToken = default (CancellationToken),
      object userState = null)
    {
      if (containerId < 1L)
        throw new ArgumentException(WebApiResources.ContainerIdMustBeGreaterThanZero(), nameof (containerId));
      List<KeyValuePair<string, string>> queryParameters = this.AppendItemQueryString(itemPath, scopeIdentifier);
      return (Task) this.DeleteAsync(FileContainerResourceIds.FileContainer, (object) new
      {
        containerId = containerId
      }, FileContainerHttpClient.s_currentApiVersion, (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState, cancellationToken);
    }

    private async Task<HttpResponseMessage> ContainerGetRequestAsync(
      long containerId,
      string itemPath,
      string contentType,
      CancellationToken cancellationToken,
      Guid scopeIdentifier,
      object userState = null)
    {
      FileContainerHttpClient containerHttpClient = this;
      if (containerId < 1L)
        throw new ArgumentException(WebApiResources.ContainerIdMustBeGreaterThanZero(), nameof (containerId));
      List<KeyValuePair<string, string>> queryParameters = containerHttpClient.AppendItemQueryString(itemPath, scopeIdentifier);
      HttpRequestMessage message = await containerHttpClient.CreateRequestMessageAsync(HttpMethod.Get, FileContainerResourceIds.FileContainer, (object) new
      {
        containerId = containerId
      }, FileContainerHttpClient.s_currentApiVersion, queryParameters: (IEnumerable<KeyValuePair<string, string>>) queryParameters, userState: userState, cancellationToken: cancellationToken).ConfigureAwait(false);
      if (!string.IsNullOrEmpty(contentType))
      {
        message.Headers.Accept.Clear();
        MediaTypeWithQualityHeaderValue qualityHeaderValue = new MediaTypeWithQualityHeaderValue(contentType);
        qualityHeaderValue.Parameters.Add(new NameValueHeaderValue("api-version", "1.0"));
        qualityHeaderValue.Parameters.Add(new NameValueHeaderValue("res-version", "1"));
        message.Headers.Accept.Add(qualityHeaderValue);
      }
      return await containerHttpClient.SendAsync(message, HttpCompletionOption.ResponseHeadersRead, userState, cancellationToken).ConfigureAwait(false);
    }

    private List<KeyValuePair<string, string>> AppendContainerQueryString(
      List<Uri> artifactUris,
      Guid scopeIdentifier)
    {
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (artifactUris != null && artifactUris.Count > 0)
      {
        string str = string.Join(",", artifactUris.Select<Uri, string>((Func<Uri, string>) (x => x.AbsoluteUri)));
        collection.Add(nameof (artifactUris), str);
      }
      collection.Add("scope", scopeIdentifier.ToString());
      return collection;
    }

    private List<KeyValuePair<string, string>> AppendItemQueryString(
      string itemPath,
      Guid scopeIdentifier,
      bool includeDownloadTickets = false,
      bool isShallow = false,
      bool includeBlobMetadata = false,
      bool isMetadata = false)
    {
      List<KeyValuePair<string, string>> collection = new List<KeyValuePair<string, string>>();
      if (!string.IsNullOrEmpty(itemPath))
      {
        itemPath = FileContainerItem.EnsurePathFormat(itemPath);
        collection.Add(nameof (itemPath), itemPath);
      }
      if (includeDownloadTickets)
        collection.Add(nameof (includeDownloadTickets), "true");
      if (isShallow)
        collection.Add(nameof (isShallow), "true");
      if (includeBlobMetadata)
        collection.Add(nameof (includeBlobMetadata), "true");
      if (isMetadata)
        collection.Add("metadata", "true");
      collection.Add("scope", scopeIdentifier.ToString());
      return collection;
    }

    private async Task<Stream> DownloadAsync(
      long containerId,
      string itemPath,
      string contentType,
      CancellationToken cancellationToken,
      Guid scopeIdentifier,
      object userState = null)
    {
      HttpResponseMessage httpResponseMessage = await this.ContainerGetRequestAsync(containerId, itemPath, contentType, cancellationToken, scopeIdentifier, userState).ConfigureAwait(false);
      httpResponseMessage.EnsureSuccessStatusCode();
      if (httpResponseMessage.StatusCode == HttpStatusCode.NoContent)
        throw new ContainerNoContentException();
      if (!VssStringComparer.ContentType.Equals(httpResponseMessage.Content.Headers.ContentType.MediaType, contentType))
        throw new ContainerUnexpectedContentTypeException(contentType, httpResponseMessage.Content.Headers.ContentType.MediaType);
      return httpResponseMessage.Content.Headers.ContentEncoding.Contains<string>("gzip", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? (Stream) new GZipStream(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false), CompressionMode.Decompress) : await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false);
    }

    private void FileUploadTrace(string file, string message)
    {
      if (this.UploadFileReportTrace == null)
        return;
      this.UploadFileReportTrace((object) this, new ReportTraceEventArgs(file, message));
    }

    private void FileUploadProgress(string file, int currentChunk, int totalChunks)
    {
      if (this.UploadFileReportProgress == null)
        return;
      this.UploadFileReportProgress((object) this, new ReportProgressEventArgs(file, currentChunk, totalChunks));
    }

    protected override IDictionary<string, Type> TranslatedExceptions => (IDictionary<string, Type>) FileContainerHttpClient.s_translatedExceptions;
  }
}
