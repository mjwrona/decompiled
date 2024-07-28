// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.MultiBufferMemoryStream
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core
{
  public class MultiBufferMemoryStream : Stream
  {
    private const int DefaultSmallBufferSize = 65536;
    private readonly int bufferSize;
    private List<byte[]> bufferBlocks;
    private long length;
    private long capacity;
    private long position;
    private IBufferManager bufferManager;
    private volatile bool disposed;

    public MultiBufferMemoryStream(IBufferManager bufferManager, int bufferSize = 65536)
    {
      this.bufferBlocks = new List<byte[]>();
      this.bufferManager = bufferManager;
      this.bufferSize = this.bufferManager == null ? bufferSize : this.bufferManager.GetDefaultBufferSize();
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

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      CommonUtility.AssertNotNull(nameof (buffer), (object) buffer);
      CommonUtility.AssertInBounds<int>(nameof (offset), offset, 0, buffer.Length);
      CommonUtility.AssertInBounds<int>(nameof (count), count, 0, buffer.Length - offset);
      StorageAsyncResult<int> storageAsyncResult = new StorageAsyncResult<int>(callback, state);
      try
      {
        storageAsyncResult.Result = this.Read(buffer, offset, count);
        storageAsyncResult.OnComplete();
      }
      catch (Exception ex)
      {
        storageAsyncResult.OnComplete(ex);
      }
      return (IAsyncResult) storageAsyncResult;
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
      StorageAsyncResult<int> storageAsyncResult = (StorageAsyncResult<int>) asyncResult;
      storageAsyncResult.End();
      return storageAsyncResult.Result;
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

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      CommonUtility.AssertNotNull(nameof (buffer), (object) buffer);
      CommonUtility.AssertInBounds<int>(nameof (offset), offset, 0, buffer.Length);
      CommonUtility.AssertInBounds<int>(nameof (count), count, 0, buffer.Length - offset);
      StorageAsyncResult<NullType> storageAsyncResult = new StorageAsyncResult<NullType>(callback, state);
      try
      {
        this.Write(buffer, offset, count);
        storageAsyncResult.OnComplete();
      }
      catch (Exception ex)
      {
        storageAsyncResult.OnComplete(ex);
      }
      return (IAsyncResult) storageAsyncResult;
    }

    public override void EndWrite(IAsyncResult asyncResult) => ((StorageCommandAsyncResult) asyncResult).End();

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
          if (expiryTime.HasValue && DateTime.Now.CompareTo(expiryTime.Value) > 0)
            throw new TimeoutException();
          ArraySegment<byte> currentBlock = bufferMemoryStream.GetCurrentBlock();
          int blockReadLength = (int) Math.Min(leftToRead, (long) currentBlock.Count);
          await destination.WriteAsync(currentBlock.Array, currentBlock.Offset, blockReadLength, token).ConfigureAwait(false);
          bufferMemoryStream.AdvancePosition(ref leftToRead, blockReadLength);
        }
      }
      catch (Exception ex)
      {
        if (expiryTime.HasValue && DateTime.Now.CompareTo(expiryTime.Value) > 0)
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
          if (expiryTime.HasValue && DateTime.Now.CompareTo(expiryTime.Value) > 0)
            throw new TimeoutException();
          ArraySegment<byte> currentBlock = this.GetCurrentBlock();
          int num = (int) Math.Min(leftToProcess, (long) currentBlock.Count);
          destination.Write(currentBlock.Array, currentBlock.Offset, num);
          this.AdvancePosition(ref leftToProcess, num);
        }
      }
      catch (Exception ex)
      {
        if (expiryTime.HasValue && DateTime.Now.CompareTo(expiryTime.Value) > 0)
          throw new TimeoutException();
        throw;
      }
    }

    public IAsyncResult BeginFastCopyTo(
      Stream destination,
      DateTime? expiryTime,
      AsyncCallback callback,
      object state)
    {
      CommonUtility.AssertNotNull(nameof (destination), (object) destination);
      StorageAsyncResult<NullType> result = new StorageAsyncResult<NullType>(callback, state);
      result.OperationState = (object) new MultiBufferMemoryStream.CopyState()
      {
        Destination = destination,
        ExpiryTime = expiryTime
      };
      this.FastCopyToInternal(result);
      return (IAsyncResult) result;
    }

    private void FastCopyToInternal(StorageAsyncResult<NullType> result)
    {
      MultiBufferMemoryStream.CopyState operationState = (MultiBufferMemoryStream.CopyState) result.OperationState;
      long leftToProcess = this.Length - this.Position;
      try
      {
        while (leftToProcess != 0L)
        {
          DateTime? expiryTime = operationState.ExpiryTime;
          if (expiryTime.HasValue)
          {
            DateTime now = DateTime.Now;
            ref DateTime local = ref now;
            expiryTime = operationState.ExpiryTime;
            DateTime dateTime = expiryTime.Value;
            if (local.CompareTo(dateTime) > 0)
              throw new TimeoutException();
          }
          ArraySegment<byte> currentBlock = this.GetCurrentBlock();
          int num = (int) Math.Min(leftToProcess, (long) currentBlock.Count);
          this.AdvancePosition(ref leftToProcess, num);
          IAsyncResult asyncResult = operationState.Destination.BeginWrite(currentBlock.Array, currentBlock.Offset, num, new AsyncCallback(this.FastCopyToCallback), (object) result);
          if (!asyncResult.CompletedSynchronously)
            return;
          operationState.Destination.EndWrite(asyncResult);
        }
        result.OnComplete();
      }
      catch (Exception ex)
      {
        DateTime? expiryTime = operationState.ExpiryTime;
        if (expiryTime.HasValue)
        {
          DateTime now = DateTime.Now;
          ref DateTime local = ref now;
          expiryTime = operationState.ExpiryTime;
          DateTime dateTime = expiryTime.Value;
          if (local.CompareTo(dateTime) > 0)
          {
            result.OnComplete((Exception) new TimeoutException());
            return;
          }
        }
        result.OnComplete(ex);
      }
    }

    private void FastCopyToCallback(IAsyncResult asyncResult)
    {
      if (asyncResult.CompletedSynchronously)
        return;
      StorageAsyncResult<NullType> asyncState = (StorageAsyncResult<NullType>) asyncResult.AsyncState;
      asyncState.UpdateCompletedSynchronously(asyncResult.CompletedSynchronously);
      MultiBufferMemoryStream.CopyState operationState = (MultiBufferMemoryStream.CopyState) asyncState.OperationState;
      try
      {
        operationState.Destination.EndWrite(asyncResult);
        this.FastCopyToInternal(asyncState);
      }
      catch (Exception ex)
      {
        DateTime? expiryTime = operationState.ExpiryTime;
        if (expiryTime.HasValue)
        {
          DateTime now = DateTime.Now;
          ref DateTime local = ref now;
          expiryTime = operationState.ExpiryTime;
          DateTime dateTime = expiryTime.Value;
          if (local.CompareTo(dateTime) > 0)
          {
            asyncState.OnComplete((Exception) new TimeoutException());
            return;
          }
        }
        asyncState.OnComplete(ex);
      }
    }

    public void EndFastCopyTo(IAsyncResult asyncResult) => ((StorageCommandAsyncResult) asyncResult).End();

    public string ComputeMD5Hash()
    {
      using (MD5Wrapper md5Wrapper = new MD5Wrapper())
      {
        long leftToProcess = this.Length - this.Position;
        while (leftToProcess != 0L)
        {
          ArraySegment<byte> currentBlock = this.GetCurrentBlock();
          int num = (int) Math.Min(leftToProcess, (long) currentBlock.Count);
          md5Wrapper.UpdateHash(currentBlock.Array, currentBlock.Offset, num);
          this.AdvancePosition(ref leftToProcess, num);
        }
        return md5Wrapper.ComputeHash();
      }
    }

    public string ComputeCRC64Hash()
    {
      using (Crc64Wrapper crc64Wrapper = new Crc64Wrapper())
      {
        long leftToProcess = this.Length - this.Position;
        while (leftToProcess != 0L)
        {
          ArraySegment<byte> currentBlock = this.GetCurrentBlock();
          int num = (int) Math.Min(leftToProcess, (long) currentBlock.Count);
          crc64Wrapper.UpdateHash(currentBlock.Array, currentBlock.Offset, num);
          this.AdvancePosition(ref leftToProcess, num);
        }
        return crc64Wrapper.ComputeHash();
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
      byte[] numArray = this.bufferManager == null ? new byte[this.bufferSize] : this.bufferManager.TakeBuffer(this.bufferSize);
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
        {
          if (this.bufferManager != null)
          {
            foreach (byte[] bufferBlock in this.bufferBlocks)
              this.bufferManager.ReturnBuffer(bufferBlock);
          }
          this.bufferBlocks.Clear();
        }
      }
      base.Dispose(disposing);
    }

    private class CopyState
    {
      public Stream Destination { get; set; }

      public DateTime? ExpiryTime { get; set; }
    }
  }
}
