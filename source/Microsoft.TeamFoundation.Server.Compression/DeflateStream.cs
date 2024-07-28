// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Compression.DeflateStream
// Assembly: Microsoft.TeamFoundation.Server.Compression, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E666AAE4-36CD-4581-80AF-1B631308AB46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.Compression.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Server.Compression
{
  public class DeflateStream : Stream
  {
    internal const int DefaultBufferSize = 4096;
    private Stream _stream;
    private CompressionMode _mode;
    private bool _leaveOpen;
    private Inflater _inflater;
    private Deflater _deflater;
    private byte[] _buffer;
    private ByteArray _byteArray;
    private int _asyncOperations;
    private bool _wroteBytes;
    private readonly bool _writeHeadersForZeroByteContent;

    public DeflateStream(Stream stream, CompressionMode mode)
      : this(stream, mode, false)
    {
    }

    public DeflateStream(Stream stream, CompressionMode mode, bool leaveOpen)
      : this(stream, mode, leaveOpen, -15)
    {
    }

    public DeflateStream(Stream stream, CompressionLevel compressionLevel)
      : this(stream, compressionLevel, false)
    {
    }

    public DeflateStream(Stream stream, CompressionLevel compressionLevel, bool leaveOpen)
      : this(stream, compressionLevel, leaveOpen, -15)
    {
    }

    internal DeflateStream(
      Stream stream,
      CompressionMode mode,
      bool leaveOpen,
      int windowBits,
      bool writeHeadersForZeroByteContent = false)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      this._writeHeadersForZeroByteContent = writeHeadersForZeroByteContent;
      if (mode != CompressionMode.Decompress)
      {
        if (mode != CompressionMode.Compress)
          throw new ArgumentException(SR.ArgumentOutOfRange_Enum, nameof (mode));
        this.InitializeDeflater(stream, leaveOpen, windowBits, CompressionLevel.Optimal);
      }
      else
        this.InitializeInflater(stream, leaveOpen, windowBits);
    }

    internal DeflateStream(
      Stream stream,
      CompressionLevel compressionLevel,
      bool leaveOpen,
      int windowBits,
      bool writeHeadersForZeroByteContent = false)
    {
      if (stream == null)
        throw new ArgumentNullException(nameof (stream));
      this._writeHeadersForZeroByteContent = writeHeadersForZeroByteContent;
      this.InitializeDeflater(stream, leaveOpen, windowBits, compressionLevel);
    }

    internal void InitializeInflater(Stream stream, bool leaveOpen, int windowBits)
    {
      if (!stream.CanRead)
        throw new ArgumentException(SR.NotSupported_UnreadableStream, nameof (stream));
      this._inflater = new Inflater(windowBits);
      this._stream = stream;
      this._mode = CompressionMode.Decompress;
      this._leaveOpen = leaveOpen;
      this._byteArray = new ByteArray(4096);
      this._buffer = this._byteArray.Bytes;
    }

    internal void InitializeDeflater(
      Stream stream,
      bool leaveOpen,
      int windowBits,
      CompressionLevel compressionLevel)
    {
      if (!stream.CanWrite)
        throw new ArgumentException(SR.NotSupported_UnwritableStream, nameof (stream));
      this._deflater = new Deflater(compressionLevel, windowBits);
      this._stream = stream;
      this._mode = CompressionMode.Compress;
      this._leaveOpen = leaveOpen;
      this._byteArray = new ByteArray(4096);
      this._buffer = this._byteArray.Bytes;
    }

    public Stream BaseStream => this._stream;

    public int AvailableInputBytes => this._inflater.AvailableInput;

    public override bool CanRead => this._stream != null && this._mode == CompressionMode.Decompress && this._stream.CanRead;

    public override bool CanWrite => this._stream != null && this._mode == CompressionMode.Compress && this._stream.CanWrite;

    public override bool CanSeek => false;

    public override long Length => throw new NotSupportedException(SR.NotSupported);

    public override long Position
    {
      get => throw new NotSupportedException(SR.NotSupported);
      set => throw new NotSupportedException(SR.NotSupported);
    }

    public override void Flush()
    {
      this.EnsureNotDisposed();
      if (this._mode != CompressionMode.Compress)
        return;
      this.FlushBuffers();
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
      if (this._asyncOperations != 0)
        throw new InvalidOperationException(SR.InvalidBeginCall);
      this.EnsureNotDisposed();
      if (cancellationToken.IsCancellationRequested)
        return (Task) this.CreateCanceledTask(cancellationToken);
      return this._mode == CompressionMode.Compress && this._wroteBytes ? this.FlushAsyncCore(cancellationToken) : this.CreateCompletedTask();
    }

    private async Task FlushAsyncCore(CancellationToken cancellationToken)
    {
      Interlocked.Increment(ref this._asyncOperations);
      try
      {
        await this.WriteDeflaterOutputAsync(cancellationToken).ConfigureAwait(false);
        bool flushSuccessful;
        do
        {
          int bytesRead;
          flushSuccessful = this._deflater.Flush(this._buffer, out bytesRead);
          if (flushSuccessful)
            await this._stream.WriteAsync(this._buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
        }
        while (flushSuccessful);
      }
      finally
      {
        Interlocked.Decrement(ref this._asyncOperations);
      }
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException(SR.NotSupported);

    public override void SetLength(long value) => throw new NotSupportedException(SR.NotSupported);

    public override int ReadByte()
    {
      this.EnsureDecompressionMode();
      this.EnsureNotDisposed();
      byte b;
      return !this._inflater.Inflate(out b) ? base.ReadByte() : (int) b;
    }

    public override int Read(byte[] array, int offset, int count)
    {
      this.EnsureDecompressionMode();
      this.ValidateParameters(array, offset, count);
      this.EnsureNotDisposed();
      int offset1 = offset;
      int length = count;
      while (true)
      {
        int num = this._inflater.Inflate(array, offset1, length);
        offset1 += num;
        length -= num;
        if (length != 0 && !this._inflater.Finished())
        {
          int count1 = this._stream.Read(this._buffer, 0, this._buffer.Length);
          if (count1 > 0)
          {
            if (count1 <= this._buffer.Length)
              this._inflater.SetInput(this._buffer, 0, count1);
            else
              break;
          }
          else
            goto label_6;
        }
        else
          goto label_6;
      }
      throw new InvalidDataException(SR.GenericInvalidData);
label_6:
      return count - length;
    }

    private void ValidateParameters(byte[] array, int offset, int count)
    {
      if (array == null)
        throw new ArgumentNullException(nameof (array));
      if (offset < 0)
        throw new ArgumentOutOfRangeException(nameof (offset));
      if (count < 0)
        throw new ArgumentOutOfRangeException(nameof (count));
      if (array.Length - offset < count)
        throw new ArgumentException(SR.InvalidArgumentOffsetCount);
    }

    private void EnsureNotDisposed()
    {
      if (this._stream != null)
        return;
      DeflateStream.ThrowStreamClosedException();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowStreamClosedException() => throw new ObjectDisposedException((string) null, SR.ObjectDisposed_StreamClosed);

    private void EnsureDecompressionMode()
    {
      if (this._mode == CompressionMode.Decompress)
        return;
      DeflateStream.ThrowCannotReadFromDeflateStreamException();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowCannotReadFromDeflateStreamException() => throw new InvalidOperationException(SR.CannotReadFromDeflateStream);

    private void EnsureCompressionMode()
    {
      if (this._mode == CompressionMode.Compress)
        return;
      DeflateStream.ThrowCannotWriteToDeflateStreamException();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowCannotWriteToDeflateStreamException() => throw new InvalidOperationException(SR.CannotWriteToDeflateStream);

    public override Task<int> ReadAsync(
      byte[] array,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      this.EnsureDecompressionMode();
      if (this._asyncOperations != 0)
        throw new InvalidOperationException(SR.InvalidBeginCall);
      this.ValidateParameters(array, offset, count);
      this.EnsureNotDisposed();
      if (cancellationToken.IsCancellationRequested)
        return this.CreateCanceledTask(cancellationToken);
      Interlocked.Increment(ref this._asyncOperations);
      Task<int> readTask = (Task<int>) null;
      try
      {
        int result = this._inflater.Inflate(array, offset, count);
        if (result != 0)
          return Task.FromResult<int>(result);
        if (this._inflater.Finished())
          return Task.FromResult<int>(0);
        readTask = this._stream.ReadAsync(this._buffer, 0, this._buffer.Length, cancellationToken);
        if (readTask == null)
          throw new InvalidOperationException(SR.NotSupported_UnreadableStream);
        return this.ReadAsyncCore(readTask, array, offset, count, cancellationToken);
      }
      finally
      {
        if (readTask == null)
          Interlocked.Decrement(ref this._asyncOperations);
      }
    }

    private async Task<int> ReadAsyncCore(
      Task<int> readTask,
      byte[] array,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      try
      {
        int num;
        do
        {
          int count1 = await readTask.ConfigureAwait(false);
          this.EnsureNotDisposed();
          if (count1 <= 0)
            return 0;
          if (count1 > this._buffer.Length)
            throw new InvalidDataException(SR.GenericInvalidData);
          cancellationToken.ThrowIfCancellationRequested();
          this._inflater.SetInput(this._buffer, 0, count1);
          num = this._inflater.Inflate(array, offset, count);
          if (num == 0 && !this._inflater.Finished())
            readTask = this._stream.ReadAsync(this._buffer, 0, this._buffer.Length, cancellationToken);
          else
            goto label_10;
        }
        while (readTask != null);
        throw new InvalidOperationException(SR.NotSupported_UnreadableStream);
label_10:
        return num;
      }
      finally
      {
        Interlocked.Decrement(ref this._asyncOperations);
      }
    }

    public override void Write(byte[] array, int offset, int count)
    {
      this.EnsureCompressionMode();
      this.ValidateParameters(array, offset, count);
      this.EnsureNotDisposed();
      this.WriteDeflaterOutput();
      this._deflater.SetInput(array, offset, count);
      this.WriteDeflaterOutput();
      this._wroteBytes = true;
    }

    private void WriteDeflaterOutput()
    {
      while (!this._deflater.NeedsInput())
      {
        int deflateOutput = this._deflater.GetDeflateOutput(this._buffer);
        if (deflateOutput > 0)
          this._stream.Write(this._buffer, 0, deflateOutput);
      }
    }

    private void FlushBuffers()
    {
      if (!this._wroteBytes)
        return;
      this.WriteDeflaterOutput();
      bool flag;
      do
      {
        int bytesRead;
        flag = this._deflater.Flush(this._buffer, out bytesRead);
        if (flag)
          this._stream.Write(this._buffer, 0, bytesRead);
      }
      while (flag);
    }

    private void PurgeBuffers(bool disposing)
    {
      if (!disposing || this._stream == null || this._mode != CompressionMode.Compress)
        return;
      if (this._wroteBytes || this._writeHeadersForZeroByteContent)
      {
        this.WriteDeflaterOutput();
        bool flag;
        do
        {
          int bytesRead;
          flag = this._deflater.Finish(this._buffer, out bytesRead);
          if (bytesRead > 0)
            this._stream.Write(this._buffer, 0, bytesRead);
        }
        while (!flag);
      }
      else
      {
        do
          ;
        while (!this._deflater.Finish(this._buffer, out int _));
      }
    }

    protected override void Dispose(bool disposing)
    {
      try
      {
        this.PurgeBuffers(disposing);
      }
      finally
      {
        try
        {
          if (disposing)
          {
            if (!this._leaveOpen)
            {
              if (this._stream != null)
                this._stream.Dispose();
            }
          }
        }
        finally
        {
          this._stream = (Stream) null;
          try
          {
            if (this._deflater != null)
              this._deflater.Dispose();
            if (this._inflater != null)
              this._inflater.Dispose();
            if (this._byteArray != null)
              this._byteArray.Dispose();
          }
          finally
          {
            this._deflater = (Deflater) null;
            this._inflater = (Inflater) null;
            this._byteArray = (ByteArray) null;
            base.Dispose(disposing);
          }
        }
      }
    }

    public override Task WriteAsync(
      byte[] array,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      this.EnsureCompressionMode();
      if (this._asyncOperations != 0)
        throw new InvalidOperationException(SR.InvalidBeginCall);
      this.ValidateParameters(array, offset, count);
      this.EnsureNotDisposed();
      return cancellationToken.IsCancellationRequested ? (Task) this.CreateCanceledTask(cancellationToken) : this.WriteAsyncCore(array, offset, count, cancellationToken);
    }

    private async Task WriteAsyncCore(
      byte[] array,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      Interlocked.Increment(ref this._asyncOperations);
      try
      {
        await this.WriteDeflaterOutputAsync(cancellationToken).ConfigureAwait(false);
        this._deflater.SetInput(array, offset, count);
        await this.WriteDeflaterOutputAsync(cancellationToken).ConfigureAwait(false);
        this._wroteBytes = true;
      }
      finally
      {
        Interlocked.Decrement(ref this._asyncOperations);
      }
    }

    private async Task WriteDeflaterOutputAsync(CancellationToken cancellationToken)
    {
      while (!this._deflater.NeedsInput())
      {
        int deflateOutput = this._deflater.GetDeflateOutput(this._buffer);
        if (deflateOutput > 0)
          await this._stream.WriteAsync(this._buffer, 0, deflateOutput, cancellationToken).ConfigureAwait(false);
      }
    }

    private Task<int> CreateCanceledTask(CancellationToken cancellationToken) => new Task<int>((Func<int>) (() => 0), cancellationToken, TaskCreationOptions.None);

    private Task CreateCompletedTask()
    {
      Task<int> completedTask = new Task<int>((Func<int>) (() => 0));
      completedTask.RunSynchronously();
      return (Task) completedTask;
    }
  }
}
