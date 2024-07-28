// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.DefaultDownloader
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class DefaultDownloader : IDownloader
  {
    private static readonly HttpClient basicHttpClient = new HttpClient();
    protected int DefaultRetryCount = 3;
    private readonly IAppTraceSource tracer;
    private readonly Guid correlationId;
    protected readonly ParallelHttpDownload.DownloadConfiguration configuration;
    private readonly HttpClient httpClient;
    internal Func<HttpClient, HttpRequestMessage, HttpCompletionOption, CancellationToken, Task<HttpResponseMessage>> SendAsyncOverride;
    internal Func<int, TimeSpan, TimeSpan, TimeSpan, TimeSpan> GetExponentialBackoffOverride;
    private const string ClientRequestIdHeader = "x-ms-client-request-id";
    private const string StorageVersionHeader = "x-ms-version";
    private const string TargetStorageVersion = "2017-04-17";

    private DefaultDownloader(IAppTraceSource tracer, Guid correlationId)
    {
      ArgumentUtility.CheckForNull<IAppTraceSource>(tracer, nameof (tracer));
      this.tracer = tracer;
      ArgumentUtility.CheckForEmptyGuid(correlationId, nameof (correlationId));
      this.correlationId = correlationId;
      if (this.httpClient != null)
        return;
      this.httpClient = DefaultDownloader.basicHttpClient;
    }

    public DefaultDownloader(
      ParallelHttpDownload.DownloadConfiguration configuration,
      IAppTraceSource tracer,
      Guid correlationId,
      HttpClient httpClient = null)
      : this(tracer, correlationId)
    {
      ArgumentUtility.CheckForNull<ParallelHttpDownload.DownloadConfiguration>(configuration, nameof (configuration));
      this.configuration = configuration;
      this.httpClient = httpClient ?? DefaultDownloader.basicHttpClient;
    }

    public async Task<DownloadResult> DownloadAsync(
      string path,
      string uri,
      long? knownSize,
      CancellationToken cancellationToken)
    {
      long bytesDownloaded = 0;
      Uri storageAccountUri;
      if (!Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out storageAccountUri))
        return new DownloadResult(HttpStatusCode.BadRequest, 0, bytesDownloaded);
      Directory.CreateDirectory(Path.GetDirectoryName(path));
      using (StreamWithRange httpStream = await this.DownloadBlobWithRetriesOnReadAccessSecondary(storageAccountUri, knownSize, cancellationToken))
      {
        string logPrefix = "ParallelHttpDownload.Download";
        return new DownloadResult(HttpStatusCode.OK, 0, await ParallelHttpDownload.Download(this.configuration, storageAccountUri, httpStream, (ParallelHttpDownload.StreamSegmentFactory) ((start, length, token) => this.GetBlobAsync(storageAccountUri, start, new long?(length), new long?(length), new long?(httpStream.Range.WholeLength), new TimeSpan?(this.configuration.ReadBufferTimeout), token)), path, FileMode.Create, (ParallelHttpDownload.LogSegmentStart) ((destinationPath, offset, endOffset) => this.tracer.Verbose(string.Format("{0} {1} [{2}, {3}) start.", (object) logPrefix, (object) destinationPath, (object) offset, (object) endOffset))), (ParallelHttpDownload.LogSegmentStop) ((destinationPath, offset, endOffset) => this.tracer.Verbose(string.Format("{0} {1} [{2}, {3}) end.", (object) logPrefix, (object) destinationPath, (object) offset, (object) endOffset))), (ParallelHttpDownload.LogSegmentFailed) ((destinationPath, offset, endOffset, message) => this.tracer.Info(string.Format("{0}  {1} [{2}, {3}) failed. (message: {4})", (object) logPrefix, (object) destinationPath, (object) offset, (object) endOffset, (object) message))), cancellationToken).ConfigureAwait(false));
      }
    }

    private Task<StreamWithRange> DownloadBlobWithRetriesOnReadAccessSecondary(
      Uri downloadUri,
      long? knownSize,
      CancellationToken cancellationToken)
    {
      Decimal halfwayAttempt = Math.Ceiling((Decimal) this.DefaultRetryCount / 2M);
      int attempts = 0;
      Uri primaryUri = downloadUri;
      return AsyncHttpRetryHelper.InvokeAsync<StreamWithRange>((Func<Task<StreamWithRange>>) (async () =>
      {
        attempts++;
        Exception ex;
        int num;
        try
        {
          return await this.GetBlobAsync(downloadUri, 0L, new long?(this.configuration.SegmentSizeInBytes), knownSize.HasValue ? new long?(Math.Min(knownSize.Value, this.configuration.SegmentSizeInBytes)) : new long?(), knownSize, new TimeSpan?(this.configuration.ReadBufferTimeout), cancellationToken);
        }
        catch (Exception ex1) when (
        {
          // ISSUE: unable to correctly present filter
          ex = ex1;
          if (AsyncHttpRetryHelper.IsTransientException(ex, cancellationToken) && !cancellationToken.IsCancellationRequested)
          {
            SuccessfulFiltering;
          }
          else
            throw;
        }
        )
        {
          num = 1;
        }
        if (num == 1)
        {
          if (primaryUri != downloadUri)
          {
            downloadUri = primaryUri;
            this.tracer.Info("Retrying with readable primary: " + downloadUri.GetLeftPart(UriPartial.Path));
          }
          else if ((Decimal) attempts == halfwayAttempt)
          {
            downloadUri = await StorageReadAccessSecondaryRegionHelper.ProcessRetries(downloadUri, this.httpClient, cancellationToken);
            this.tracer.Info("Retrying with readable secondary, fallbackUri: " + string.Format("{0} with attempts {1}.", (object) downloadUri.GetLeftPart(UriPartial.Path), (object) attempts));
          }
          throw new AsyncHttpRetryHelper.RetryableException("Retrying with primary " + string.Format("{0}, attempts after {1}.", (object) downloadUri.GetLeftPart(UriPartial.Path), (object) attempts), ex);
        }
        ex = (Exception) null;
        StreamWithRange streamWithRange;
        return streamWithRange;
      }), this.DefaultRetryCount, this.tracer, cancellationToken, false, downloadUri.RemoveQueryParameter("sig").AbsoluteUri);
    }

    public Task<StreamWithRange> GetBlobAsync(
      Uri downloadUri,
      long? knownLength,
      TimeSpan? timeout,
      CancellationToken cancellationToken)
    {
      return this.GetBlobAsync(downloadUri, 0L, new long?(), knownLength, knownLength, timeout, cancellationToken);
    }

    public async Task<StreamWithRange> GetBlobAsync(
      Uri downloadUri,
      long firstBytePosition,
      long? maxLengthToRequest,
      long? expectedRangeLength,
      long? knownWholeFileLength,
      TimeSpan? timeout,
      CancellationToken cancellationToken)
    {
      long? nullable1 = knownWholeFileLength;
      long num = 0;
      StreamWithRange blobAsync;
      if (nullable1.GetValueOrDefault() == num & nullable1.HasValue)
      {
        MemoryStream memoryStream = new MemoryStream(new byte[0], false);
        blobAsync = new StreamWithRange((Stream) memoryStream, new StreamRange((Stream) memoryStream));
      }
      else if (downloadUri.IsFile)
      {
        FileStream fileStream = System.IO.File.OpenRead(downloadUri.LocalPath);
        fileStream.Position = firstBytePosition;
        blobAsync = new StreamWithRange((Stream) fileStream, new StreamRange((Stream) fileStream));
      }
      else if (!this.GetStreamViaAzureSdk(downloadUri))
      {
        ServicePointManager.FindServicePoint(downloadUri).UpdateServicePointSettings(ServicePointConstants.MaxConnectionsPerProc8);
        long? maxBytePositionInclusive = !maxLengthToRequest.HasValue ? new long?() : new long?(firstBytePosition + maxLengthToRequest.Value - 1L);
        Func<Task<StreamWithRange>> taskGenerator = (Func<Task<StreamWithRange>>) (() => this.GetHttpRangeAsync(downloadUri, firstBytePosition, maxBytePositionInclusive, cancellationToken));
        int segmentDownloadRetries = this.configuration.MaxSegmentDownloadRetries;
        IAppTraceSource tracer = this.tracer;
        Func<int, TimeSpan, TimeSpan, TimeSpan, TimeSpan> exponentialBackoffOverride = this.GetExponentialBackoffOverride;
        string leftPart = downloadUri.GetLeftPart(UriPartial.Path);
        TimeSpan? minBackoff = new TimeSpan?();
        TimeSpan? maxBackoff = new TimeSpan?();
        TimeSpan? deltaBackoff = new TimeSpan?();
        Func<int, TimeSpan, TimeSpan, TimeSpan, TimeSpan> getExponentialBackoff = exponentialBackoffOverride;
        blobAsync = await new AsyncHttpRetryHelper<StreamWithRange>(taskGenerator, segmentDownloadRetries, tracer, false, leftPart, minBackoff: minBackoff, maxBackoff: maxBackoff, deltaBackoff: deltaBackoff, getExponentialBackoff: getExponentialBackoff).InvokeAsync(cancellationToken).ConfigureAwait(false);
      }
      else
      {
        int? nullable2 = new int?();
        if (maxLengthToRequest.HasValue)
          nullable2 = new int?((int) maxLengthToRequest.Value);
        Stream stream = await this.GetStreamThroughAzureBlobs(downloadUri, new int?(nullable2.Value), timeout, cancellationToken).ConfigureAwait(false);
        stream.Position = firstBytePosition;
        blobAsync = new StreamWithRange(stream, new StreamRange(stream));
      }
      if (expectedRangeLength.HasValue && blobAsync.Range.Length != blobAsync.Range.WholeLength && expectedRangeLength.Value != blobAsync.Range.Length)
        throw new EndOfStreamException(string.Format("Expected stream of length {0}, but encountered stream of length {1}.", (object) expectedRangeLength.Value, (object) blobAsync.Range.Length));
      if (knownWholeFileLength.HasValue && knownWholeFileLength.Value != blobAsync.Range.WholeLength)
        throw new EndOfStreamException(string.Format("Expected a blob of length {0} bytes, but encountered a blob of length {1}.", (object) knownWholeFileLength.Value, (object) blobAsync.Range.WholeLength));
      return blobAsync;
    }

    private async Task<StreamWithRange> GetHttpRangeAsync(
      Uri downloadUri,
      long firstBytePosition,
      long? lastBytePositionInclusive,
      CancellationToken cancellationToken)
    {
      Func<HttpClient, HttpRequestMessage, HttpCompletionOption, CancellationToken, Task<HttpResponseMessage>> sendAsync = this.SendAsyncOverride ?? (Func<HttpClient, HttpRequestMessage, HttpCompletionOption, CancellationToken, Task<HttpResponseMessage>>) ((httpClient, req, completion, cancellation) => httpClient.SendAsync(req, completion, cancellation));
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, downloadUri);
      request.Headers.Add("x-ms-client-request-id", this.correlationId.ToString());
      request.Headers.Add("x-ms-version", "2017-04-17");
      request.Headers.Range = new RangeHeaderValue(new long?(firstBytePosition), lastBytePositionInclusive);
      HttpResponseMessage response = (HttpResponseMessage) null;
      bool requestSucceeded = false;
      try
      {
        response = await sendAsync(this.httpClient, request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        if (response.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
        {
          if ((await sendAsync(this.httpClient, new HttpRequestMessage(HttpMethod.Head, downloadUri), HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false)).Content.Headers.ContentLength.Value == 0L)
          {
            MemoryStream memoryStream = new MemoryStream(Array.Empty<byte>());
            return new StreamWithRange((Stream) memoryStream, new StreamRange((Stream) memoryStream));
          }
        }
        this.EnsureSuccessStatusCodeWithoutDisposingContent(response);
        requestSucceeded = true;
      }
      catch (HttpRequestException ex) when (
      {
        // ISSUE: unable to correctly present filter
        HttpRequestMessage request1 = request;
        HttpResponseMessage response1 = response;
        if (ex.SetHttpMessagesForTracing(request1, response1))
        {
          SuccessfulFiltering;
        }
        else
          throw;
      }
      )
      {
        throw new InvalidOperationException("The exception filter should not have entered this catch block (8D2D95DB-6A9E-4BDF-B3F0-C319F336322E)");
      }
      finally
      {
        int num;
        if (num < 0 && response != null && !requestSucceeded)
          response.Dispose();
      }
      long responseLength = response.Content.Headers.ContentLength.Value;
      if (response.StatusCode == HttpStatusCode.OK)
        return new StreamWithRange((await response.Content.ReadAsStreamAsync().EnforceCancellation<Stream>(cancellationToken, file: "D:\\a\\_work\\1\\s\\ArtifactServices\\Shared\\BlobStore.Common\\DefaultDownloader.cs", member: nameof (GetHttpRangeAsync), line: 346).ConfigureAwait(false)).WrapWithCancellationEnforcement(downloadUri.AbsoluteUri), StreamRange.FullRange(responseLength));
      if (response.Content.Headers.ContentRange == null)
      {
        ContentRangeHeaderNotFoundException exception = new ContentRangeHeaderNotFoundException("Expected Content-Range header was not found in the response.");
        exception.SetHttpMessagesForTracing(request, response);
        throw exception;
      }
      StreamRange responseRange = new StreamRange(response.Content.Headers.ContentRange);
      if (responseLength == 0L && responseRange.Length != 0L)
        throw new AsyncHttpRetryHelper.RetryableException("Content-Length is zero, but Range is greater than zero.");
      return new StreamWithRange((await response.Content.ReadAsStreamAsync().EnforceCancellation<Stream>(cancellationToken, file: "D:\\a\\_work\\1\\s\\ArtifactServices\\Shared\\BlobStore.Common\\DefaultDownloader.cs", member: nameof (GetHttpRangeAsync), line: 368).ConfigureAwait(false)).WrapWithCancellationEnforcement(downloadUri.AbsoluteUri), responseRange);
    }

    protected virtual bool GetStreamViaAzureSdk(Uri downloadUri) => false;

    protected virtual Task<Stream> GetStreamThroughAzureBlobs(
      Uri azureUri,
      int? overrideStreamMinimumReadSizeInBytes,
      TimeSpan? requestTimeout,
      CancellationToken cancellationToken)
    {
      throw new NotSupportedException();
    }

    private void EnsureSuccessStatusCodeWithoutDisposingContent(HttpResponseMessage response)
    {
      if (!response.IsSuccessStatusCode)
        throw new HttpRequestException(SafeStringFormat.FormatSafe("{0} {1} {2}", (object) (int) response.StatusCode, (object) response.StatusCode.ToString(), (object) response.ReasonPhrase));
    }
  }
}
