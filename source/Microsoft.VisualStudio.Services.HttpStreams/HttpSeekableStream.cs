// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HttpStreams.HttpSeekableStream
// Assembly: Microsoft.VisualStudio.Services.HttpStreams, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08EEF7AF-2ADD-4A01-B7DB-5972BBFA47F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.HttpStreams.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Microsoft.VisualStudio.Services.HttpStreams
{
  public class HttpSeekableStream : Stream
  {
    public const int DefaultBufferSize = 81920;
    private const int MaximumBufferSize = 2147483647;
    private readonly Uri uri;
    private HttpClient httpClient;
    private EntityTagHeaderValue etag;
    private long? length;
    private long currentPosition;
    private HttpBuffer streamBuffer;
    private HttpBuffer backBuffer;

    public HttpSeekableStream(
      Uri uri,
      int bufferSize = 81920,
      HttpMessageHandler messageHandler = null,
      bool disposeHandler = true)
    {
      this.uri = uri;
      this.BufferSize = bufferSize;
      this.httpClient = new HttpClient(messageHandler ?? (HttpMessageHandler) new HttpClientHandler(), messageHandler == null || disposeHandler);
    }

    public int BufferSize { get; set; }

    public override long Position
    {
      get => this.currentPosition;
      set => this.Seek(value, SeekOrigin.Begin);
    }

    public long TotalRetrievedBytes { get; private set; }

    public int TotalRequests { get; private set; }

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => false;

    public override long Length
    {
      get
      {
        this.EnsureHaveLength();
        return this.length.Value;
      }
    }

    public override int ReadTimeout
    {
      get => (int) this.httpClient.Timeout.TotalMilliseconds;
      set => this.httpClient.Timeout = TimeSpan.FromMilliseconds((double) value);
    }

    private bool AtEndOfStream => this.Position == this.Length;

    public override long Seek(long offset, SeekOrigin origin)
    {
      this.EnsureNotDisposed();
      long num;
      switch (origin)
      {
        case SeekOrigin.Begin:
          num = offset;
          break;
        case SeekOrigin.Current:
          num = checked (this.currentPosition + offset);
          break;
        case SeekOrigin.End:
          num = this.SeekFromEnd(offset);
          break;
        default:
          throw new ArgumentException(Resources.Error_UnknownSeekOrigin((object) origin), nameof (origin));
      }
      this.currentPosition = num >= 0L ? num : throw new IOException(Resources.Error_SeekPastBeginning());
      return num;
    }

    public override void SetLength(long value) => throw HttpSeekableStream.WritesNotSupported();

    public override async Task CopyToAsync(
      Stream destination,
      int bufferSize,
      CancellationToken cancellationToken)
    {
      HttpSeekableStream httpSeekableStream = this;
      byte[] buffer = new byte[bufferSize];
      while (true)
      {
        cancellationToken.ThrowIfCancellationRequested();
        int count = await httpSeekableStream.ReadAsync(buffer, 0, bufferSize, cancellationToken).ConfigureAwait(false);
        if (count != 0)
          await destination.WriteAsync(buffer, 0, count, cancellationToken).ConfigureAwait(false);
        else
          break;
      }
      buffer = (byte[]) null;
    }

    public override void Flush()
    {
    }

    public override Task FlushAsync(CancellationToken cancellationToken) => (Task) Task.FromResult<int>(0);

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      throw new NotImplementedException();
    }

    public override int EndRead(IAsyncResult asyncResult) => throw new NotImplementedException();

    public override int Read(byte[] buffer, int offset, int count) => AsyncPump.Run<int>((Func<Task<int>>) (async () => await this.ReadAsync(buffer, offset, count)));

    public override async Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      HttpSeekableStream httpSeekableStream = this;
      ArgumentUtility.CheckForNull<byte[]>(buffer, nameof (buffer));
      ArgumentUtility.CheckForOutOfRange(offset, nameof (offset), 0);
      ArgumentUtility.CheckForOutOfRange(count, nameof (count), 0);
      httpSeekableStream.EnsureNotDisposed();
      httpSeekableStream.EnsurePositionIsWithinStream();
      if (count == 0 || httpSeekableStream.AtEndOfStream)
        return 0;
      if (!httpSeekableStream.IsRangeInBuffer(httpSeekableStream.Position, count))
        await httpSeekableStream.LoadIntoStreamBuffer(httpSeekableStream.Position, count, cancellationToken).ConfigureAwait(false);
      return httpSeekableStream.ReadFromStreamBuffer(buffer, offset, count);
    }

    public override int ReadByte()
    {
      this.EnsureNotDisposed();
      this.EnsurePositionIsWithinStream();
      if (this.AtEndOfStream)
        return -1;
      if (!this.IsRangeInBuffer(this.Position, 1))
        AsyncPump.Run((Func<Task>) (async () =>
        {
          HttpSeekableStream httpSeekableStream = this;
          await httpSeekableStream.LoadIntoStreamBuffer(httpSeekableStream.Position, 1, CancellationToken.None);
        }));
      return this.ReadByteFromStreamBuffer();
    }

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      throw HttpSeekableStream.WritesNotSupported();
    }

    public override void EndWrite(IAsyncResult asyncResult) => throw HttpSeekableStream.WritesNotSupported();

    public override Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      throw HttpSeekableStream.WritesNotSupported();
    }

    public override void Write(byte[] buffer, int offset, int count) => throw HttpSeekableStream.WritesNotSupported();

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (!disposing)
        return;
      Interlocked.Exchange<HttpClient>(ref this.httpClient, (HttpClient) null)?.Dispose();
    }

    private static NotSupportedException WritesNotSupported() => new NotSupportedException(Resources.Error_WritesNotSupported());

    private long SeekFromEnd(long offset)
    {
      if (!this.length.HasValue && offset < 0L && -offset < (long) this.BufferSize)
        AsyncPump.Run((Func<Task>) (async () => await this.LoadIntoStreamBuffer(offset, this.BufferSize, CancellationToken.None)));
      return checked (this.Length + offset);
    }

    private bool IsRangeInBuffer(long position, int count) => this.streamBuffer != null && this.streamBuffer.IsRangeInBuffer(position, count);

    private void EnsurePositionIsWithinStream()
    {
      if (this.Position > this.Length)
        throw new IOException(Resources.Error_ReadPastEnd((object) this.Position, (object) this.Length));
    }

    private async Task LoadIntoStreamBuffer(
      long position,
      int count,
      CancellationToken cancellationToken)
    {
      HttpSeekableStream httpSeekableStream = this;
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, httpSeekableStream.uri))
      {
        if (position < 0L)
          position = httpSeekableStream.Length + position;
        long num1 = Math.Min((long) Math.Max(count, httpSeekableStream.BufferSize), httpSeekableStream.Length);
        long num2 = Math.Min(position, httpSeekableStream.Length - num1);
        long num3 = num2 + num1 - 1L;
        request.Headers.Range = new RangeHeaderValue(new long?(num2), new long?(num3));
        if (httpSeekableStream.etag != null)
          request.Headers.IfMatch.Add(httpSeekableStream.etag);
        using (HttpResponseMessage responseMessage = await httpSeekableStream.SendHttpRequestAsync(request, cancellationToken).ConfigureAwait(false))
        {
          httpSeekableStream.SeekableStreamEnsureSuccessStatusCode(responseMessage);
          if (responseMessage.StatusCode != HttpStatusCode.PartialContent)
            throw new UnexpectedServerResponseException(Resources.Error_NotRangeResponse());
          ContentRangeHeaderValue contentRange = responseMessage.Content.Headers.ContentRange;
          if (contentRange == null)
            throw new UnexpectedServerResponseException(Resources.Error_NoContentRangeHeader());
          if (!contentRange.HasLength)
            throw new UnexpectedServerResponseException(Resources.Error_NoContentRangeLength());
          if (!contentRange.HasRange)
            throw new UnexpectedServerResponseException(Resources.Error_NoContentRangeRange());
          if (!httpSeekableStream.length.HasValue)
            httpSeekableStream.length = new long?(contentRange.Length.Value);
          if (httpSeekableStream.etag == null)
            httpSeekableStream.etag = responseMessage.Headers.ETag;
          long expectedRetrievedLength = checked (contentRange.To.Value - contentRange.From.Value + 1L);
          if (expectedRetrievedLength > (long) int.MaxValue)
            throw new UnexpectedServerResponseException(Resources.Error_ReturnedRangeTooLarge());
          long newBufferStart = contentRange.From.Value;
          if (httpSeekableStream.backBuffer == null || expectedRetrievedLength > (long) httpSeekableStream.backBuffer.Length)
            httpSeekableStream.backBuffer = new HttpBuffer(Math.Max((int) expectedRetrievedLength, httpSeekableStream.BufferSize));
          using (Stream responseStream = await responseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false))
          {
            await httpSeekableStream.backBuffer.ReplaceContentFromStreamAsync(responseStream, newBufferStart, (int) expectedRetrievedLength, cancellationToken).ConfigureAwait(false);
            httpSeekableStream.TotalRetrievedBytes += (long) httpSeekableStream.backBuffer.Length;
          }
          HttpBuffer streamBuffer = httpSeekableStream.streamBuffer;
          httpSeekableStream.streamBuffer = httpSeekableStream.backBuffer;
          httpSeekableStream.backBuffer = streamBuffer;
          if (httpSeekableStream.backBuffer != null)
          {
            if (httpSeekableStream.backBuffer.BufferSize > httpSeekableStream.BufferSize)
              httpSeekableStream.backBuffer = (HttpBuffer) null;
          }
        }
      }
    }

    private async Task<HttpResponseMessage> SendHttpRequestAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ++this.TotalRequests;
      string message1 = string.Format("HttpSeekableStream Request #{0}: {1} {2}", (object) this.TotalRequests, (object) request.Method, (object) request.RequestUri);
      if (request.Headers.Range != null)
        message1 = message1 + " " + request.Headers.Range.ToString();
      Trace.WriteLine(message1);
      HttpResponseMessage httpResponseMessage = await this.httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
      string message2 = string.Format("HttpSeekableStream Response #{0}: {1}", (object) this.TotalRequests, (object) httpResponseMessage.StatusCode);
      if (httpResponseMessage.Content != null)
      {
        if (httpResponseMessage.Content.Headers.ContentRange != null)
          message2 = message2 + ", Range: " + httpResponseMessage.Content.Headers.ContentRange?.ToString();
        long? contentLength = httpResponseMessage.Content.Headers.ContentLength;
        if (contentLength.HasValue)
        {
          string str1 = message2;
          contentLength = httpResponseMessage.Content.Headers.ContentLength;
          string str2 = contentLength.ToString();
          message2 = str1 + ", Length: " + str2;
        }
      }
      Trace.WriteLine(message2);
      return httpResponseMessage;
    }

    private int ReadFromStreamBuffer(byte[] buffer, int offset, int count)
    {
      if (!this.streamBuffer.IsPositionInBuffer(this.Position))
        throw new UnexpectedPositionNotInBufferException(this.Position, this.streamBuffer);
      long sourceIndex = this.Position - this.streamBuffer.StartPosition;
      long val2 = (long) this.streamBuffer.Length - sourceIndex;
      int length = (int) Math.Min((long) count, val2);
      Array.Copy((Array) this.streamBuffer.Bytes, sourceIndex, (Array) buffer, (long) offset, (long) length);
      this.currentPosition += (long) length;
      return length;
    }

    private int ReadByteFromStreamBuffer()
    {
      if (!this.streamBuffer.IsPositionInBuffer(this.Position))
        throw new UnexpectedPositionNotInBufferException(this.Position, this.streamBuffer);
      int num = (int) this.streamBuffer.Bytes[this.Position - this.streamBuffer.StartPosition];
      ++this.currentPosition;
      return num;
    }

    private void EnsureHaveLength()
    {
      if (this.length.HasValue)
        return;
      using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, this.uri))
      {
        using (HttpResponseMessage message = TaskExtensions.SyncResult(this.SendHttpRequestAsync(request)))
        {
          this.SeekableStreamEnsureSuccessStatusCode(message);
          this.etag = message.Headers.ETag;
          this.length = message.Content.Headers.ContentLength;
        }
      }
      if (!this.length.HasValue)
        throw new UnexpectedServerResponseException(Resources.Error_CantDetermineLength());
    }

    private void EnsureNotDisposed()
    {
      if (this.httpClient == null)
        throw new ObjectDisposedException(nameof (HttpSeekableStream));
    }

    private void SeekableStreamEnsureSuccessStatusCode(HttpResponseMessage message)
    {
      if (!message.IsSuccessStatusCode)
      {
        string message1 = string.Format("Cannot read data from host {0}. Status code: {1} ({2}). ", (object) this.uri.Host, (object) (int) message.StatusCode, (object) message.ReasonPhrase);
        string query = message.RequestMessage.RequestUri.Query;
        if (!string.IsNullOrWhiteSpace(query))
        {
          string str1 = HttpUtility.ParseQueryString(query).Get("st") ?? "";
          string str2 = HttpUtility.ParseQueryString(query).Get("se") ?? "";
          message1 = message1 + "SignedStart: " + str1 + " SignedExpiry: " + str2;
        }
        throw new HttpSeekableStreamRequestException(message1, message.StatusCode);
      }
    }
  }
}
