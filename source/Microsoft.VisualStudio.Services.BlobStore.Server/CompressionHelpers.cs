// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.CompressionHelpers
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public static class CompressionHelpers
  {
    public static async Task<DedupCompressedBuffer> GetPossiblyCompressedBufferAsync(
      HttpContent content)
    {
      DedupCompressedBuffer compressedBufferAsync;
      using (MemoryStream contentStream = new MemoryStream())
      {
        await content.CopyToAsync((Stream) contentStream).ConfigureAwait(true);
        byte[] array = contentStream.ToArray();
        compressedBufferAsync = !content.Headers.ContentEncoding.Contains("xpress") ? DedupCompressedBuffer.FromUncompressed(array) : DedupCompressedBuffer.FromCompressed(array);
      }
      return compressedBufferAsync;
    }

    public static ByteArrayContent CreatePossiblyCompressedResponseContent(
      HttpRequestMessage request,
      DedupCompressedBuffer buffer)
    {
      bool isCompressed;
      ArraySegment<byte> buffer1;
      buffer.GetBytes(out isCompressed, out buffer1);
      ByteArrayContent compressedResponseContent;
      if (request.Headers.AcceptEncoding.Contains(DedupStoreHttpClient.XpressCompressionHeader) & isCompressed)
      {
        compressedResponseContent = new ByteArrayContent(buffer1.CreateCopy<byte>());
        compressedResponseContent.Headers.ContentEncoding.Add("xpress");
      }
      else
        compressedResponseContent = new ByteArrayContent(buffer.Uncompressed.CreateCopy<byte>());
      compressedResponseContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      return compressedResponseContent;
    }
  }
}
