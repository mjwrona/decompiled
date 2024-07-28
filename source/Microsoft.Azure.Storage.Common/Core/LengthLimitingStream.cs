// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.LengthLimitingStream
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.IO;

namespace Microsoft.Azure.Storage.Core
{
  internal class LengthLimitingStream : Stream
  {
    private readonly Stream wrappedStream;
    private long startOffset;
    private long? endOffset;
    private long position;
    private long? length;

    public LengthLimitingStream(Stream wrappedStream, long start, long? length = null)
    {
      this.wrappedStream = wrappedStream;
      this.startOffset = start;
      this.length = length;
      if (!length.HasValue)
        return;
      long startOffset = this.startOffset;
      long? nullable1 = this.length;
      long num = 1;
      long? nullable2 = nullable1.HasValue ? new long?(nullable1.GetValueOrDefault() - num) : new long?();
      long? nullable3;
      if (!nullable2.HasValue)
      {
        nullable1 = new long?();
        nullable3 = nullable1;
      }
      else
        nullable3 = new long?(startOffset + nullable2.GetValueOrDefault());
      this.endOffset = nullable3;
    }

    public override bool CanRead => this.wrappedStream.CanRead;

    public override bool CanSeek => this.wrappedStream.CanSeek;

    public override bool CanWrite => this.wrappedStream.CanWrite;

    public override long Length => !this.length.HasValue ? this.wrappedStream.Length : this.length.Value;

    public override long Position
    {
      get => this.position;
      set => this.Seek(value, SeekOrigin.Begin);
    }

    public override void Flush() => this.wrappedStream.Flush();

    public override void SetLength(long value)
    {
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

    public override int Read(byte[] buffer, int offset, int count) => this.wrappedStream.Read(buffer, offset, count);

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      return this.wrappedStream.BeginRead(buffer, offset, count, callback, state);
    }

    public override int EndRead(IAsyncResult asyncResult) => this.wrappedStream.EndRead(asyncResult);

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (this.position < this.startOffset)
      {
        int num = (int) Math.Min(this.startOffset - this.position, (long) count);
        offset += num;
        count -= num;
        this.position += (long) num;
      }
      if (this.endOffset.HasValue)
        count = (int) Math.Min(this.endOffset.Value + 1L - this.position, (long) count);
      if (count <= 0)
        return;
      this.wrappedStream.Write(buffer, offset, count);
      this.position += (long) count;
    }

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      if (this.position < this.startOffset)
      {
        int num = (int) Math.Min(this.startOffset - this.position, (long) count);
        offset += num;
        count -= num;
        this.position += (long) num;
      }
      if (this.endOffset.HasValue)
        count = (int) Math.Min(this.endOffset.Value + 1L - this.position, (long) count);
      StorageAsyncResult<NullType> state1 = new StorageAsyncResult<NullType>(callback, state);
      if (count <= 0)
      {
        state1.OnComplete();
      }
      else
      {
        state1.OperationState = (object) count;
        this.wrappedStream.BeginWrite(buffer, offset, count, new AsyncCallback(this.WriteStreamCallback), (object) state1);
      }
      return (IAsyncResult) state1;
    }

    private void WriteStreamCallback(IAsyncResult ar)
    {
      StorageAsyncResult<NullType> asyncState = (StorageAsyncResult<NullType>) ar.AsyncState;
      asyncState.UpdateCompletedSynchronously(ar.CompletedSynchronously);
      Exception exception = (Exception) null;
      try
      {
        this.wrappedStream.EndWrite(ar);
        this.position += (long) (int) asyncState.OperationState;
      }
      catch (Exception ex)
      {
        exception = ex;
      }
      asyncState.OnComplete(exception);
    }

    public override void EndWrite(IAsyncResult asyncResult) => ((StorageCommandAsyncResult) asyncResult).End();

    protected override void Dispose(bool disposing)
    {
    }
  }
}
