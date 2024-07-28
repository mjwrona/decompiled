// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.HttpClientExtensions
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class HttpClientExtensions
  {
    public static Task<HttpResponseMessage> PatchAsync(
      this HttpClient client,
      string uri,
      HttpContent content)
    {
      return client.PatchAsync(uri, content, new CancellationToken(false));
    }

    public static Task<HttpResponseMessage> PatchAsync(
      this HttpClient client,
      string uri,
      HttpContent content,
      CancellationToken cancellationToken)
    {
      HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("PATCH"), uri)
      {
        Content = content
      };
      return client.SendAsync(request, cancellationToken);
    }

    public static Task<HttpResponseMessage> DeleteAsync(
      this HttpClient client,
      string uri,
      HttpContent content,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("DELETE"), uri)
      {
        Content = content
      };
      return client.SendAsync(request, cancellationToken);
    }

    public static async Task<HttpResponseMessage> DownloadFileFromTfsAsync(
      this HttpClient client,
      Uri requestUri,
      Stream stream,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckForNull<Stream>(stream, nameof (stream));
      ArgumentUtility.CheckForNull<Uri>(requestUri, nameof (requestUri));
      HttpResponseMessage response = await client.GetAsync(requestUri.ToString(), cancellationToken).ConfigureAwait(false);
      if (response.IsSuccessStatusCode && response.StatusCode != HttpStatusCode.NoContent)
      {
        bool decompress;
        if (VssStringComparer.ContentType.Equals(response.Content.Headers.ContentType.MediaType, "application/octet-stream") || VssStringComparer.ContentType.Equals(response.Content.Headers.ContentType.MediaType, "application/zip") || VssStringComparer.ContentType.Equals(response.Content.Headers.ContentType.MediaType, "application/xml") || VssStringComparer.ContentType.Equals(response.Content.Headers.ContentType.MediaType, "text/plain"))
        {
          decompress = false;
        }
        else
        {
          if (!VssStringComparer.ContentType.Equals(response.Content.Headers.ContentType.MediaType, "application/gzip"))
            throw new Exception(WebApiResources.UnsupportedContentType((object) response.Content.Headers.ContentType.MediaType));
          decompress = true;
        }
        using (HttpClientExtensions.DownloadStream downloadStream = new HttpClientExtensions.DownloadStream(stream, decompress, response.Content.Headers.ContentMD5))
        {
          await response.Content.CopyToAsync((Stream) downloadStream).ConfigureAwait(false);
          downloadStream.ValidateHash();
        }
      }
      HttpResponseMessage httpResponseMessage = response;
      response = (HttpResponseMessage) null;
      return httpResponseMessage;
    }

    private class DownloadStream : Stream
    {
      private readonly bool m_decompress;
      private MD5 m_hashProvider;
      private readonly byte[] m_expectedHashValue;
      private readonly Stream m_stream;

      public DownloadStream(Stream stream, bool decompress, byte[] hashValue)
      {
        this.m_stream = stream;
        this.m_decompress = decompress;
        this.m_expectedHashValue = hashValue;
        if (hashValue == null || hashValue.Length != 16)
          return;
        this.m_expectedHashValue = hashValue;
        this.m_hashProvider = MD5Utility.TryCreateMD5Provider();
      }

      public override bool CanRead => this.m_stream.CanRead;

      public override bool CanSeek => this.m_stream.CanSeek;

      public override bool CanWrite => this.m_stream.CanWrite;

      public override long Length => this.m_stream.Length;

      public override long Position
      {
        get => this.m_stream.Position;
        set => this.m_stream.Position = value;
      }

      protected override void Dispose(bool disposing)
      {
        if (this.m_hashProvider != null)
        {
          this.m_hashProvider.Dispose();
          this.m_hashProvider = (MD5) null;
        }
        base.Dispose(disposing);
      }

      public override void Flush() => this.m_stream.Flush();

      public override int Read(byte[] buffer, int offset, int count) => this.m_stream.Read(buffer, offset, count);

      public override long Seek(long offset, SeekOrigin origin) => this.m_stream.Seek(offset, origin);

      public override void SetLength(long value) => this.m_stream.SetLength(value);

      public override void Write(byte[] buffer, int offset, int count)
      {
        byte[] outputBuffer;
        int outputOffset;
        int outputCount;
        this.Transform(buffer, offset, count, out outputBuffer, out outputOffset, out outputCount);
        this.m_stream.Write(outputBuffer, outputOffset, outputCount);
      }

      public override IAsyncResult BeginWrite(
        byte[] buffer,
        int offset,
        int count,
        AsyncCallback callback,
        object state)
      {
        byte[] outputBuffer;
        int outputOffset;
        int outputCount;
        this.Transform(buffer, offset, count, out outputBuffer, out outputOffset, out outputCount);
        return this.m_stream.BeginWrite(outputBuffer, outputOffset, outputCount, callback, state);
      }

      public override void EndWrite(IAsyncResult asyncResult) => this.m_stream.EndWrite(asyncResult);

      public override Task WriteAsync(
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken)
      {
        byte[] outputBuffer;
        int outputOffset;
        int outputCount;
        this.Transform(buffer, offset, count, out outputBuffer, out outputOffset, out outputCount);
        return this.m_stream.WriteAsync(outputBuffer, outputOffset, outputCount, cancellationToken);
      }

      public override void WriteByte(byte value) => this.Write(new byte[1]
      {
        value
      }, 0, 1);

      public void ValidateHash()
      {
        if (this.m_hashProvider == null)
          return;
        this.m_hashProvider.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
        if (!ArrayUtility.Equals(this.m_hashProvider.Hash, this.m_expectedHashValue))
          throw new Exception(WebApiResources.DownloadCorrupted());
      }

      private void Transform(
        byte[] buffer,
        int offset,
        int count,
        out byte[] outputBuffer,
        out int outputOffset,
        out int outputCount)
      {
        if (this.m_decompress)
        {
          using (MemoryStream memoryStream1 = new MemoryStream(buffer, offset, count))
          {
            using (GZipStream gzipStream = new GZipStream((Stream) memoryStream1, CompressionMode.Decompress))
            {
              int count1 = 4096;
              byte[] buffer1 = new byte[count1];
              using (MemoryStream memoryStream2 = new MemoryStream())
              {
                int count2;
                do
                {
                  count2 = gzipStream.Read(buffer1, 0, count1);
                  if (count2 > 0)
                    memoryStream2.Write(buffer1, 0, count2);
                }
                while (count2 > 0);
                outputBuffer = memoryStream2.ToArray();
                outputOffset = 0;
                outputCount = outputBuffer.Length;
              }
            }
          }
        }
        else
        {
          outputBuffer = buffer;
          outputOffset = offset;
          outputCount = count;
        }
        if (this.m_hashProvider == null || outputCount <= 0)
          return;
        this.m_hashProvider.TransformBlock(outputBuffer, outputOffset, outputCount, (byte[]) null, 0);
      }
    }
  }
}
