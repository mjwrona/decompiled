// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Rntbd.RntbdStreamReader
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;
using System.Buffers;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Rntbd
{
  internal sealed class RntbdStreamReader : IDisposable
  {
    private const int BufferSize = 16384;
    private readonly Stream stream;
    private byte[] buffer;
    private int offset;
    private int length;

    public RntbdStreamReader(Stream stream)
    {
      this.stream = stream;
      this.buffer = ArrayPool<byte>.Shared.Rent(16384);
      this.offset = 0;
      this.length = 0;
    }

    internal int AvailableByteCount => this.length;

    public void Dispose()
    {
      byte[] buffer = this.buffer;
      this.buffer = (byte[]) null;
      ArrayPool<byte>.Shared.Return(buffer);
    }

    public ValueTask<int> ReadAsync(byte[] payload, int offset, int count)
    {
      if (payload.Length < offset + count)
        throw new ArgumentException(nameof (payload));
      return this.length > 0 ? new ValueTask<int>(this.CopyFromAvailableBytes(payload, offset, count)) : this.PopulateBytesAndReadAsync(payload, offset, count);
    }

    public ValueTask<int> ReadAsync(MemoryStream payload, int count) => this.length > 0 ? new ValueTask<int>(this.CopyFromAvailableBytes(payload, count)) : this.PopulateBytesAndReadAsync(payload, count);

    private async ValueTask<int> PopulateBytesAndReadAsync(byte[] payload, int offset, int count)
    {
      if (count >= this.buffer.Length)
        return await this.stream.ReadAsync(payload, offset, count);
      this.offset = 0;
      this.length = await this.stream.ReadAsync(this.buffer, 0, this.buffer.Length);
      return this.length != 0 ? this.CopyFromAvailableBytes(payload, offset, count) : this.length;
    }

    private async ValueTask<int> PopulateBytesAndReadAsync(MemoryStream payload, int count)
    {
      this.offset = 0;
      this.length = await this.stream.ReadAsync(this.buffer, 0, this.buffer.Length);
      return this.length != 0 ? this.CopyFromAvailableBytes(payload, count) : this.length;
    }

    private int CopyFromAvailableBytes(byte[] payload, int offset, int count)
    {
      try
      {
        if (count >= this.length)
        {
          Array.Copy((Array) this.buffer, this.offset, (Array) payload, offset, this.length);
          int length = this.length;
          this.length = 0;
          this.offset = 0;
          return length;
        }
        Array.Copy((Array) this.buffer, this.offset, (Array) payload, offset, count);
        this.length -= count;
        this.offset += count;
        return count;
      }
      catch (Exception ex)
      {
        throw new IOException("Error copying buffered bytes", ex);
      }
    }

    private int CopyFromAvailableBytes(MemoryStream payload, int count)
    {
      try
      {
        if (count >= this.length)
        {
          int length = this.length;
          payload.Write(this.buffer, this.offset, this.length);
          this.length = 0;
          this.offset = 0;
          return length;
        }
        payload.Write(this.buffer, this.offset, count);
        this.length -= count;
        this.offset += count;
        return count;
      }
      catch (Exception ex)
      {
        throw new IOException("Error copying buffered bytes", ex);
      }
    }
  }
}
