// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.StreamExtensions
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public static class StreamExtensions
  {
    public static async Task ReadToEntireBufferAsync(
      this Stream content,
      ArraySegment<byte> buffer,
      CancellationToken cancellationToken)
    {
      if (await content.ReadToBufferAsync(buffer, cancellationToken).ConfigureAwait(false) != buffer.Count)
        throw new EndOfStreamException();
    }

    public static async Task<int> ReadToBufferAsync(
      this Stream content,
      ArraySegment<byte> buffer,
      CancellationToken cancellationToken)
    {
      int bytesReadTotal = 0;
      int bufferOffset = buffer.Offset;
      int num;
      for (int bytesToRead = buffer.Count; bytesToRead > 0; bytesToRead -= num)
      {
        num = await content.ReadAsync(buffer.Array, bufferOffset, bytesToRead, cancellationToken).ConfigureAwait(false);
        if (num != 0)
        {
          bytesReadTotal += num;
          bufferOffset += num;
        }
        else
          break;
      }
      return bytesReadTotal;
    }

    public static Stream WrapWithCancellationEnforcement(this Stream content, string name) => (Stream) new StreamExtensions.EnforcedCancellationStream(content, name);

    private class EnforcedCancellationStream : Stream
    {
      private readonly Stream baseStream;
      private readonly string name;

      public EnforcedCancellationStream(Stream baseStream, string name)
      {
        this.baseStream = baseStream;
        this.name = name;
      }

      public override bool CanRead => this.baseStream.CanRead;

      public override bool CanSeek => this.baseStream.CanSeek;

      public override bool CanWrite => this.baseStream.CanWrite;

      public override long Length => this.baseStream.Length;

      public override long Position
      {
        get => this.baseStream.Position;
        set => this.baseStream.Position = value;
      }

      public override void Flush() => this.baseStream.Flush();

      public override int Read(byte[] buffer, int offset, int count) => this.baseStream.Read(buffer, offset, count);

      public override long Seek(long offset, SeekOrigin origin) => this.baseStream.Seek(offset, origin);

      public override void SetLength(long value) => this.baseStream.SetLength(value);

      public override void Write(byte[] buffer, int offset, int count) => this.baseStream.Write(buffer, offset, count);

      public override Task<int> ReadAsync(
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken)
      {
        return this.baseStream.ReadAsync(buffer, offset, count, cancellationToken).EnforceCancellation<int>(cancellationToken, (Func<string>) (() => "Timed out waiting for ReadAsync from '" + this.name + "'."), "D:\\a\\_work\\1\\s\\ArtifactServices\\Shared\\BlobStore.Common\\StreamExtensions.cs", nameof (ReadAsync), 104);
      }

      public override Task WriteAsync(
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken)
      {
        return this.baseStream.WriteAsync(buffer, offset, count, cancellationToken).EnforceCancellation(cancellationToken, (Func<string>) (() => "Timed out waiting for WriteAsync from '" + this.name + "'."), "D:\\a\\_work\\1\\s\\ArtifactServices\\Shared\\BlobStore.Common\\StreamExtensions.cs", nameof (WriteAsync), 110);
      }

      public override Task CopyToAsync(
        Stream destination,
        int bufferSize,
        CancellationToken cancellationToken)
      {
        return this.baseStream.CopyToAsync(destination, bufferSize, cancellationToken).EnforceCancellation(cancellationToken, (Func<string>) (() => "Timed out waiting for CopyToAsync from '" + this.name + "'."), "D:\\a\\_work\\1\\s\\ArtifactServices\\Shared\\BlobStore.Common\\StreamExtensions.cs", nameof (CopyToAsync), 116);
      }

      public override Task FlushAsync(CancellationToken cancellationToken) => this.baseStream.FlushAsync(cancellationToken).EnforceCancellation(cancellationToken, (Func<string>) (() => "Timed out waiting for FlushAsync from '" + this.name + "'."), "D:\\a\\_work\\1\\s\\ArtifactServices\\Shared\\BlobStore.Common\\StreamExtensions.cs", nameof (FlushAsync), 122);

      protected override void Dispose(bool disposing)
      {
        if (disposing)
          this.baseStream.Dispose();
        base.Dispose(disposing);
      }
    }
  }
}
