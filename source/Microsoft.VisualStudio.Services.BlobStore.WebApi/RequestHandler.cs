// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.RequestHandler
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public class RequestHandler
  {
    public const string KeepUntilHeader = "X-MS-KeepUntils";
    public const string KeepUntilSignature = "X-MS-Signature";
    public long calls;
    public long throttledCalls;
    public long xCacheHits;
    public long xCacheMisses;
    private IAppTraceSource tracer;
    private readonly HttpClient basicClient;
    private const int DefaultRedirectTimeoutInSeconds = 60;
    private int redirectTimeoutInSeconds;

    public RequestHandler(IAppTraceSource traceSource, HttpClient client)
    {
      this.tracer = traceSource;
      this.basicClient = client;
      this.SetRedirectTimeout(new int?());
    }

    public RequestHandler(
      IAppTraceSource traceSource,
      HttpClient client,
      int defaultRedirectTimeoutInSeconds)
      : this(traceSource, client)
    {
      this.SetRedirectTimeout(new int?(defaultRedirectTimeoutInSeconds));
    }

    public void SetRedirectTimeout(int? redirectTimeOutSeconds)
    {
      string environmentVariable = Environment.GetEnvironmentVariable("VSO_DEDUP_REDIRECT_TIMEOUT_IN_SEC");
      if (!string.IsNullOrWhiteSpace(environmentVariable))
      {
        if (int.TryParse(environmentVariable, out this.redirectTimeoutInSeconds))
          this.tracer.Info("Dedup redirect timeout is set to " + environmentVariable + " from environment variable.");
        else
          this.tracer.Warn(string.Format("Dedup redirect timeout {0} should be an integer. Timeout value not changed: {1}", (object) environmentVariable, (object) this.redirectTimeoutInSeconds));
      }
      else if (redirectTimeOutSeconds.HasValue)
      {
        this.redirectTimeoutInSeconds = redirectTimeOutSeconds.Value;
        this.tracer.Info(string.Format("Dedup redirect timeout is set to {0}.", (object) this.redirectTimeoutInSeconds));
      }
      else
        this.redirectTimeoutInSeconds = 60;
    }

    public void SetTracer(IAppTraceSource tracer) => this.tracer = tracer;

    public Dictionary<string, string> CreateHeadersFromReceipts(SummaryKeepUntilReceipt receipt)
    {
      Dictionary<string, string> headersFromReceipts = (Dictionary<string, string>) null;
      if (receipt != null)
      {
        headersFromReceipts = new Dictionary<string, string>();
        string str = string.Join(",", ((IEnumerable<KeepUntilBlobReference?>) receipt.KeepUntils).Select<KeepUntilBlobReference?, string>((Func<KeepUntilBlobReference?, string>) (k => k?.KeepUntilString ?? string.Empty)));
        headersFromReceipts.Add("X-MS-KeepUntils", str);
        headersFromReceipts.Add("X-MS-Signature", Convert.ToBase64String(receipt.Signature));
      }
      return headersFromReceipts;
    }

    private static bool IsAzureTableUri(Uri uri) => uri.Host.EndsWith(".table.core.windows.net", StringComparison.OrdinalIgnoreCase);

    public async Task<DedupCompressedBuffer> GetDedupAsync(
      HttpResponseMessage responseMessage,
      DedupIdentifier dedupId,
      CancellationToken cancellationToken)
    {
      bool isCompressed = responseMessage.Content.Headers.ContentEncoding.Contains("xpress");
      Uri location = responseMessage.Headers.Location;
      if (location != (Uri) null)
        return await this.HandleRedirectAsync(isCompressed, location, (HttpClient) null, cancellationToken).ConfigureAwait(false);
      int rawLength = (int) responseMessage.Content.Headers.ContentLength.Value;
      IPoolHandle<byte[]> chunkBuffer = ChunkerHelper.BorrowChunkBuffer(rawLength);
      using (Stream rawStream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
      {
        using (Stream cancellableStream = rawStream.WrapWithCancellationEnforcement(dedupId.ValueString))
          await cancellableStream.ReadToEntireBufferAsync(new ArraySegment<byte>(chunkBuffer.Value, 0, rawLength), cancellationToken).ConfigureAwait(false);
      }
      return !isCompressed ? DedupCompressedBuffer.FromUncompressed(chunkBuffer, 0, rawLength) : DedupCompressedBuffer.FromCompressed(chunkBuffer, 0, rawLength);
    }

    public Task<DedupCompressedBuffer> HandleRedirectAsync(
      bool knownToBeCompressed,
      Uri storageUri,
      HttpClient httpClient,
      CancellationToken cancellationToken)
    {
      int attempts = 0;
      HttpClient client = httpClient ?? this.basicClient;
      Uri primaryUri = storageUri;
      return AsyncHttpRetryHelper.InvokeAsync<DedupCompressedBuffer>((Func<Task<DedupCompressedBuffer>>) (async () =>
      {
        attempts++;
        TimeSpan timeout = TimeSpan.FromSeconds((double) this.redirectTimeoutInSeconds);
        using (CancellationTokenSource timeoutSource = new CancellationTokenSource(timeout))
        {
          Exception ex;
          int num;
          try
          {
            using (CancellationTokenSource combinedSource = CancellationTokenSource.CreateLinkedTokenSource(timeoutSource.Token, cancellationToken))
            {
              using (HttpResponseMessage responseMessage = await this.GetRedirectResponseAsync(storageUri, httpClient, combinedSource.Token).ConfigureAwait(false))
                return responseMessage.StatusCode == HttpStatusCode.NotFound ? (DedupCompressedBuffer) null : await RequestHandler.ReadResponseAsync(knownToBeCompressed, storageUri, responseMessage, combinedSource.Token).ConfigureAwait(false);
            }
          }
          catch (Exception ex1) when (
          {
            // ISSUE: unable to correctly present filter
            ex = ex1;
            if (AsyncHttpRetryHelper.IsTransientException(ex, cancellationToken) || ex is OperationCanceledException && timeoutSource.Token.IsCancellationRequested)
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
            if (primaryUri != storageUri)
            {
              storageUri = primaryUri;
              this.tracer.Info("Retrying with readable primary: " + storageUri.GetLeftPart(UriPartial.Path));
            }
            else if (attempts == 3)
            {
              storageUri = await StorageReadAccessSecondaryRegionHelper.ProcessRetries(storageUri, client, cancellationToken);
              this.tracer.Info("Retrying with fall-back storage, fallbackUri: " + storageUri.GetLeftPart(UriPartial.Path));
            }
            throw new AsyncHttpRetryHelper.RetryableException(string.Format("Attempt timed out after {0}.", (object) timeout), ex);
          }
          ex = (Exception) null;
        }
        timeout = new TimeSpan();
        DedupCompressedBuffer compressedBuffer;
        return compressedBuffer;
      }), 5, this.tracer, (Func<Exception, bool>) null, cancellationToken, false, storageUri.RemoveQueryParameter("sig").AbsoluteUri);
    }

    private Task<HttpResponseMessage> GetRedirectResponseAsync(
      Uri redirect,
      HttpClient httpClient,
      CancellationToken cancellationToken)
    {
      ServicePointManager.FindServicePoint(redirect).UpdateServicePointSettings(ServicePointConstants.MaxConnectionsPerProc64);
      Func<string> func;
      return AsyncHttpRetryHelper.InvokeAsync<HttpResponseMessage>((Func<Task<HttpResponseMessage>>) (async () =>
      {
        Interlocked.Increment(ref this.calls);
        HttpRequestMessage request = RequestHandler.CreateRequest(redirect);
        HttpClient httpClient1 = httpClient ?? this.basicClient;
        IDisposable messageToDispose = (IDisposable) null;
        try
        {
          HttpResponseMessage redirectResponseAsync = await httpClient1.SendAsync(request, cancellationToken).EnforceCancellation<HttpResponseMessage>(cancellationToken, func ?? (func = (Func<string>) (() => "Timed out waiting for response for " + redirect.RemoveQueryParameter("sig").AbsoluteUri + ".")), "D:\\a\\_work\\1\\s\\BlobStore\\Client\\WebApi\\DedupStoreHttpClient.cs", nameof (GetRedirectResponseAsync), 923).ConfigureAwait(false);
          messageToDispose = (IDisposable) redirectResponseAsync;
          if (redirectResponseAsync.StatusCode == HttpStatusCode.NotFound)
            return redirectResponseAsync;
          if (redirectResponseAsync.StatusCode == HttpStatusCode.ServiceUnavailable)
          {
            Interlocked.Increment(ref this.throttledCalls);
            throw new AsyncHttpRetryHelper.RetryableException("HTTP 503 throttling.");
          }
          redirectResponseAsync.EnsureSuccessStatusCode();
          IEnumerable<string> values;
          if (redirectResponseAsync.Headers.TryGetValues("X-Cache", out values))
          {
            Interlocked.Add(ref this.xCacheHits, (long) values.Count<string>((Func<string, bool>) (h => h.StartsWith("HIT"))));
            Interlocked.Add(ref this.xCacheMisses, (long) values.Count<string>((Func<string, bool>) (h => h.StartsWith("MISS"))));
          }
          messageToDispose = (IDisposable) null;
          return redirectResponseAsync;
        }
        finally
        {
          messageToDispose?.Dispose();
        }
      }), 5, this.tracer, (Func<Exception, bool>) (e => e is HttpRequestException), cancellationToken, false, redirect.RemoveQueryParameter("sig").AbsoluteUri);
    }

    internal static HttpRequestMessage CreateRequest(Uri redirect)
    {
      HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, redirect);
      if (RequestHandler.IsAzureTableUri(redirect))
        request.Headers.Add("Accept", "application/json;odata=minimalmetadata");
      return request;
    }

    internal static async Task<DedupCompressedBuffer> ReadResponseAsync(
      bool knownToBeCompressed,
      Uri redirect,
      HttpResponseMessage responseMessage,
      CancellationToken cancellationToken)
    {
      int size = (int) responseMessage.Content.Headers.ContentLength.Value;
      IPoolHandle<byte[]> wireBuffer = ChunkerHelper.BorrowChunkBuffer(size);
      int maxChunkSize = size <= ChunkerHelper.GetMaxNodeContentSize() ? ChunkerHelper.GetMaxNodeContentSize() : ChunkerHelper.GetMaxChunkContentSize();
      bool isCompressed;
      int chunkSize;
      if (RequestHandler.IsAzureTableUri(redirect))
      {
        RequestHandler.ContentTableRow contentTableRow = JsonSerializer.Deserialize<RequestHandler.ContentTableRow>(await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false));
        byte[] src1 = Convert.FromBase64String(contentTableRow.Content00);
        chunkSize = src1.Length;
        System.Buffer.BlockCopy((Array) src1, 0, (Array) wireBuffer.Value, 0, src1.Length);
        if (contentTableRow.Content01.Length > 0)
        {
          byte[] src2 = Convert.FromBase64String(contentTableRow.Content01);
          System.Buffer.BlockCopy((Array) src2, 0, (Array) wireBuffer.Value, chunkSize, src2.Length);
          chunkSize += src2.Length;
        }
        isCompressed = contentTableRow.IsCompressed;
      }
      else
      {
        isCompressed = knownToBeCompressed | responseMessage.Content.Headers.ContentEncoding.Contains("xpress");
        using (Stream stream = (await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false)).WrapWithCancellationEnforcement(redirect.AbsoluteUri))
        {
          chunkSize = 0;
          while (true)
          {
            int num;
            if ((num = await stream.ReadAsync(wireBuffer.Value, chunkSize, maxChunkSize - chunkSize).ConfigureAwait(false)) != 0)
              chunkSize += num;
            else
              break;
          }
        }
      }
      DedupCompressedBuffer compressedBuffer = !isCompressed ? DedupCompressedBuffer.FromUncompressed(wireBuffer, 0, chunkSize) : DedupCompressedBuffer.FromCompressed(wireBuffer, 0, chunkSize);
      wireBuffer = (IPoolHandle<byte[]>) null;
      return compressedBuffer;
    }

    private class ContentTableRow
    {
      public string Content00 { get; set; }

      public string Content01 { get; set; }

      public bool IsCompressed { get; set; }
    }
  }
}
