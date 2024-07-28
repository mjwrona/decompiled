// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.SubStream
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Blob
{
  internal sealed class SubStream : Stream
  {
    private Stream wrappedStream;
    private long streamBeginIndex;
    private long substreamLength;
    private long substreamCurrentIndex;
    private Lazy<MemoryStream> readBufferStream;
    private Lazy<byte[]> readBuffer;
    private int readBufferLength;
    private bool shouldSeek;

    public SemaphoreSlim Mutex { get; set; }

    public override long Position
    {
      get => this.substreamCurrentIndex;
      set
      {
        CommonUtility.AssertInBounds<long>(nameof (Position), value, 0L, this.substreamLength);
        if (value >= this.substreamCurrentIndex)
        {
          long offset = value - this.substreamCurrentIndex;
          if (offset <= (long) this.readBufferLength)
          {
            this.readBufferLength -= (int) offset;
            if (this.shouldSeek)
              this.readBufferStream.Value.Seek(offset, SeekOrigin.Current);
          }
          else
          {
            this.readBufferLength = 0;
            this.readBufferStream.Value.Seek(0L, SeekOrigin.End);
          }
        }
        else
        {
          this.readBufferLength = 0;
          this.readBufferStream.Value.Seek(0L, SeekOrigin.End);
        }
        this.substreamCurrentIndex = value;
      }
    }

    public override long Length => this.substreamLength;

    public override bool CanRead => true;

    public override bool CanSeek => true;

    public override bool CanWrite => false;

    private void CheckDisposed()
    {
      if (this.wrappedStream == null)
        throw new ObjectDisposedException("SubStreamWrapper");
    }

    protected override void Dispose(bool disposing)
    {
      this.wrappedStream = (Stream) null;
      this.readBufferStream = (Lazy<MemoryStream>) null;
      this.readBuffer = (Lazy<byte[]>) null;
    }

    public override void Flush() => throw new NotSupportedException();

    public int ReadBufferSize
    {
      get => this.readBuffer.Value.Length;
      set
      {
        this.readBuffer = value >= 131072 ? new Lazy<byte[]>((Func<byte[]>) (() => new byte[value])) : throw new ArgumentOutOfRangeException(string.Format("The argument '{0}' is smaller than minimum of '{1}'", (object) nameof (ReadBufferSize), (object) 131072));
        this.readBufferStream = new Lazy<MemoryStream>((Func<MemoryStream>) (() => new MemoryStream(this.readBuffer.Value, 0, value, true)));
        this.readBufferStream.Value.Seek(0L, SeekOrigin.End);
      }
    }

    public SubStream(
      Stream stream,
      long streamBeginIndex,
      long substreamLength,
      SemaphoreSlim globalSemaphore)
    {
      if (stream == null)
        throw new ArgumentNullException("Stream.");
      if (!stream.CanSeek)
        throw new NotSupportedException("Stream must be seekable.");
      if (globalSemaphore == null)
        throw new ArgumentNullException(nameof (globalSemaphore));
      CommonUtility.AssertInBounds<long>(nameof (streamBeginIndex), streamBeginIndex, 0L, stream.Length);
      this.streamBeginIndex = streamBeginIndex;
      this.wrappedStream = stream;
      this.Mutex = globalSemaphore;
      this.substreamLength = Math.Min(substreamLength, stream.Length - streamBeginIndex);
      this.readBufferLength = 0;
      this.Position = 0L;
      this.ReadBufferSize = 4194304;
    }

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      return this.ReadAsync(buffer, offset, count, CancellationToken.None).AsApm<int>(callback, state);
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
      CommonUtility.AssertNotNull("AsyncResult", (object) asyncResult);
      return CommonUtility.RunWithoutSynchronizationContext<int>((Func<int>) (() => ((Task<int>) asyncResult).Result));
    }

    public override async Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      SubStream subStream = this;
      subStream.CheckDisposed();
      int num1;
      try
      {
        int readCount = subStream.CheckAdjustReadCount(count, offset, buffer.Length);
        ConfiguredTaskAwaitable<int> configuredTaskAwaitable = subStream.readBufferStream.Value.ReadAsync(buffer, offset, Math.Min(readCount, subStream.readBufferLength), cancellationToken).ConfigureAwait(false);
        int bytesRead = await configuredTaskAwaitable;
        int bytesLeft = readCount - bytesRead;
        subStream.shouldSeek = false;
        subStream.Position += (long) bytesRead;
        if (bytesLeft > 0 && subStream.readBufferLength == 0)
        {
          subStream.readBufferStream.Value.Position = 0L;
          configuredTaskAwaitable = subStream.ReadAsyncHelper(subStream.readBuffer.Value, 0, subStream.readBuffer.Value.Length, cancellationToken).ConfigureAwait(false);
          int val1 = await configuredTaskAwaitable;
          subStream.readBufferLength = val1;
          if (val1 > 0)
          {
            bytesLeft = Math.Min(val1, bytesLeft);
            configuredTaskAwaitable = subStream.readBufferStream.Value.ReadAsync(buffer, bytesRead + offset, bytesLeft, cancellationToken).ConfigureAwait(false);
            int num2 = await configuredTaskAwaitable;
            bytesRead += num2;
            subStream.Position += (long) num2;
          }
        }
        num1 = bytesRead;
      }
      finally
      {
        subStream.shouldSeek = true;
      }
      return num1;
    }

    private async Task<int> ReadAsyncHelper(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      SubStream subStream = this;
      await subStream.Mutex.WaitAsync(cancellationToken).ConfigureAwait(false);
      int num = 0;
      try
      {
        subStream.CheckDisposed();
        count = subStream.CheckAdjustReadCount(count, offset, buffer.Length);
        if (subStream.wrappedStream.Position != subStream.streamBeginIndex + subStream.Position)
          subStream.wrappedStream.Seek(subStream.streamBeginIndex + subStream.Position, SeekOrigin.Begin);
        num = await subStream.wrappedStream.ReadAsync(buffer, offset, count, cancellationToken).ConfigureAwait(false);
      }
      finally
      {
        subStream.Mutex.Release();
      }
      return num;
    }

    public override int Read(byte[] buffer, int offset, int count) => CommonUtility.RunWithoutSynchronizationContext<int>((Func<int>) (() => this.ReadAsync(buffer, offset, count).Result));

    public override long Seek(long offset, SeekOrigin origin)
    {
      this.CheckDisposed();
      long num;
      switch (origin)
      {
        case SeekOrigin.Begin:
          num = 0L;
          break;
        case SeekOrigin.Current:
          num = this.Position;
          break;
        case SeekOrigin.End:
          throw new NotSupportedException();
        default:
          throw new ArgumentOutOfRangeException();
      }
      this.Position = num + offset;
      return this.Position;
    }

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    private int CheckAdjustReadCount(int count, int offset, int bufferLength)
    {
      if (offset < 0 || count < 0 || offset + count > bufferLength)
        throw new ArgumentOutOfRangeException();
      long num1 = this.streamBeginIndex + this.Position;
      long num2 = this.streamBeginIndex + this.substreamLength;
      return num1 + (long) count > num2 ? (int) (num2 - num1) : count;
    }
  }
}
