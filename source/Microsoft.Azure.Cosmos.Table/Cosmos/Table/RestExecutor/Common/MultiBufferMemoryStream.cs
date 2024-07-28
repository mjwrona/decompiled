// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.Common.MultiBufferMemoryStream
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.Common
{
  internal class MultiBufferMemoryStream : Stream
  {
    private const int DefaultSmallBufferSize = 65536;
    private readonly int bufferSize;
    private List<byte[]> bufferBlocks;
    private long length;
    private long capacity;
    private long position;
    private volatile bool disposed;

    internal MultiBufferMemoryStream(int bufferSize = 65536)
    {
      this.bufferBlocks = new List<byte[]>();
      this.bufferSize = bufferSize;
      if (bufferSize <= 0)
        throw new ArgumentOutOfRangeException(nameof (bufferSize), "Buffer size must be a positive, non-zero value");
    }

    public override bool CanRead => !this.disposed;

    public override bool CanSeek => !this.disposed;

    public override bool CanWrite => !this.disposed;

    public override long Length => this.length;

    public override long Position
    {
      get => this.position;
      set => this.Seek(value, SeekOrigin.Begin);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      CommonUtility.AssertNotNull(nameof (buffer), (object) buffer);
      CommonUtility.AssertInBounds<int>(nameof (offset), offset, 0, buffer.Length);
      CommonUtility.AssertInBounds<int>(nameof (count), count, 0, buffer.Length - offset);
      return this.ReadInternal(buffer, offset, count);
    }

    public override Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      return Task.FromResult<int>(this.Read(buffer, offset, count));
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      long val;
      switch (origin)
      {
        case SeekOrigin.Begin:
          val = offset;
          break;
        case SeekOrigin.Current:
          val = this.position + offset;
          break;
        case SeekOrigin.End:
          val = this.Length + offset;
          break;
        default:
          CommonUtility.ArgumentOutOfRange(nameof (origin), (object) origin);
          throw new ArgumentOutOfRangeException(nameof (origin));
      }
      CommonUtility.AssertInBounds<long>(nameof (offset), val, 0L, this.Length);
      this.position = val;
      return this.position;
    }

    public override void SetLength(long value)
    {
      this.Reserve(value);
      this.length = value;
      this.position = Math.Min(this.position, this.length);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      CommonUtility.AssertNotNull(nameof (buffer), (object) buffer);
      CommonUtility.AssertInBounds<int>(nameof (offset), offset, 0, buffer.Length);
      CommonUtility.AssertInBounds<int>(nameof (count), count, 0, buffer.Length - offset);
      if (this.position + (long) count > this.capacity)
        this.Reserve(this.position + (long) count);
      this.WriteInternal(buffer, offset, count);
      this.length = Math.Max(this.length, this.position);
    }

    public override Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      this.Write(buffer, offset, count);
      return (Task) Task.FromResult<bool>(true);
    }

    public override void Flush()
    {
    }

    public async Task FastCopyToAsync(
      Stream destination,
      DateTime? expiryTime,
      CancellationToken token)
    {
      MultiBufferMemoryStream bufferMemoryStream = this;
      CommonUtility.AssertNotNull(nameof (destination), (object) destination);
      long leftToRead = bufferMemoryStream.Length - bufferMemoryStream.Position;
      try
      {
        while (leftToRead != 0L)
        {
          if (expiryTime.HasValue && DateTime.UtcNow.CompareTo(expiryTime.Value) > 0)
            throw new TimeoutException();
          ArraySegment<byte> currentBlock = bufferMemoryStream.GetCurrentBlock();
          int blockReadLength = (int) Math.Min(leftToRead, (long) currentBlock.Count);
          await destination.WriteAsync(currentBlock.Array, currentBlock.Offset, blockReadLength, token).ConfigureAwait(false);
          bufferMemoryStream.AdvancePosition(ref leftToRead, blockReadLength);
        }
      }
      catch (Exception ex)
      {
        if (expiryTime.HasValue && DateTime.UtcNow.CompareTo(expiryTime.Value) > 0)
          throw new TimeoutException();
        throw;
      }
    }

    public void FastCopyTo(Stream destination, DateTime? expiryTime)
    {
      CommonUtility.AssertNotNull(nameof (destination), (object) destination);
      long leftToProcess = this.Length - this.Position;
      try
      {
        while (leftToProcess != 0L)
        {
          if (expiryTime.HasValue && DateTime.UtcNow.CompareTo(expiryTime.Value) > 0)
            throw new TimeoutException();
          ArraySegment<byte> currentBlock = this.GetCurrentBlock();
          int num = (int) Math.Min(leftToProcess, (long) currentBlock.Count);
          destination.Write(currentBlock.Array, currentBlock.Offset, num);
          this.AdvancePosition(ref leftToProcess, num);
        }
      }
      catch (Exception ex)
      {
        if (expiryTime.HasValue && DateTime.UtcNow.CompareTo(expiryTime.Value) > 0)
          throw new TimeoutException();
        throw;
      }
    }

    private void Reserve(long requiredSize)
    {
      if (requiredSize < 0L)
        throw new ArgumentOutOfRangeException(nameof (requiredSize), "The size must be positive");
      while (requiredSize > this.capacity)
        this.AddBlock();
    }

    private void AddBlock()
    {
      byte[] numArray = new byte[this.bufferSize];
      if (numArray.Length != this.bufferSize)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The IBufferManager provided an incorrect length buffer to the stream, Expected {0}, received {1}. Buffer length should equal the value returned by IBufferManager.GetDefaultBufferSize().", (object) this.bufferSize, (object) numArray.Length));
      this.bufferBlocks.Add(numArray);
      this.capacity += (long) this.bufferSize;
    }

    private int ReadInternal(byte[] buffer, int offset, int count)
    {
      int num1 = (int) Math.Min(this.Length - this.Position, (long) count);
      int leftToProcess = num1;
      while (leftToProcess != 0)
      {
        ArraySegment<byte> currentBlock = this.GetCurrentBlock();
        int num2 = Math.Min(leftToProcess, currentBlock.Count);
        Buffer.BlockCopy((Array) currentBlock.Array, currentBlock.Offset, (Array) buffer, offset, num2);
        this.AdvancePosition(ref offset, ref leftToProcess, num2);
      }
      return num1;
    }

    private void WriteInternal(byte[] buffer, int offset, int count)
    {
      while (count != 0)
      {
        ArraySegment<byte> currentBlock = this.GetCurrentBlock();
        int num = Math.Min(count, currentBlock.Count);
        Buffer.BlockCopy((Array) buffer, offset, (Array) currentBlock.Array, currentBlock.Offset, num);
        this.AdvancePosition(ref offset, ref count, num);
      }
    }

    private void AdvancePosition(ref int offset, ref int leftToProcess, int amountProcessed)
    {
      this.position += (long) amountProcessed;
      offset += amountProcessed;
      leftToProcess -= amountProcessed;
    }

    private void AdvancePosition(ref long leftToProcess, int amountProcessed)
    {
      this.position += (long) amountProcessed;
      leftToProcess -= (long) amountProcessed;
    }

    private ArraySegment<byte> GetCurrentBlock()
    {
      int index = (int) (this.position / (long) this.bufferSize);
      int offset = (int) (this.position % (long) this.bufferSize);
      byte[] bufferBlock = this.bufferBlocks[index];
      return new ArraySegment<byte>(bufferBlock, offset, bufferBlock.Length - offset);
    }

    protected override void Dispose(bool disposing)
    {
      if (!this.disposed)
      {
        this.disposed = true;
        if (disposing)
          this.bufferBlocks.Clear();
      }
      base.Dispose(disposing);
    }
  }
}
