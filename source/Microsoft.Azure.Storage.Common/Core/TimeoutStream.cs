// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.TimeoutStream
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Core
{
  internal class TimeoutStream : Stream
  {
    private readonly Stream wrappedStream;
    private TimeSpan readTimeout;
    private TimeSpan writeTimeout;
    private CancellationTokenSource cancellationTokenSource;

    public TimeoutStream(Stream wrappedStream, TimeSpan timeout)
      : this(wrappedStream, timeout, timeout)
    {
    }

    public TimeoutStream(Stream wrappedStream, TimeSpan readTimeout, TimeSpan writeTimeout)
    {
      CommonUtility.AssertNotNull("WrappedStream", (object) wrappedStream);
      CommonUtility.AssertNotNull(nameof (ReadTimeout), (object) readTimeout);
      CommonUtility.AssertNotNull(nameof (WriteTimeout), (object) writeTimeout);
      this.wrappedStream = wrappedStream;
      this.readTimeout = readTimeout;
      this.writeTimeout = writeTimeout;
      this.UpdateReadTimeout();
      this.UpdateWriteTimeout();
      this.InitializeTokenSource();
    }

    public override long Position
    {
      get => this.wrappedStream.Position;
      set => this.wrappedStream.Position = value;
    }

    public override long Length => this.wrappedStream.Length;

    public override bool CanWrite => this.wrappedStream.CanWrite;

    public override bool CanTimeout => this.wrappedStream.CanTimeout;

    public override bool CanSeek => this.wrappedStream.CanSeek;

    public override bool CanRead => this.wrappedStream.CanRead;

    public override int ReadTimeout
    {
      get => (int) this.readTimeout.TotalMilliseconds;
      set
      {
        this.readTimeout = TimeSpan.FromMilliseconds((double) value);
        this.UpdateReadTimeout();
      }
    }

    public override int WriteTimeout
    {
      get => (int) this.writeTimeout.TotalMilliseconds;
      set
      {
        this.writeTimeout = TimeSpan.FromMilliseconds((double) value);
        this.UpdateWriteTimeout();
      }
    }

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      return this.wrappedStream.BeginRead(buffer, offset, count, callback, state);
    }

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      return this.wrappedStream.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void Close() => this.wrappedStream.Close();

    public override Task CopyToAsync(
      Stream destination,
      int bufferSize,
      CancellationToken cancellationToken)
    {
      return this.wrappedStream.CopyToAsync(destination, bufferSize, cancellationToken);
    }

    public override int EndRead(IAsyncResult asyncResult) => this.wrappedStream.EndRead(asyncResult);

    public override void EndWrite(IAsyncResult asyncResult) => this.wrappedStream.EndWrite(asyncResult);

    public override void Flush() => this.wrappedStream.Flush();

    public override async Task FlushAsync(CancellationToken cancellationToken)
    {
      bool dispose;
      CancellationTokenSource source = this.StartTimeout(cancellationToken, out dispose);
      try
      {
        await this.wrappedStream.FlushAsync(source.Token).ConfigureAwait(false);
      }
      catch (ObjectDisposedException ex)
      {
        source.Token.ThrowIfCancellationRequested();
        throw;
      }
      finally
      {
        this.StopTimeout(source, dispose);
      }
      source = (CancellationTokenSource) null;
    }

    public override int Read(byte[] buffer, int offset, int count) => this.wrappedStream.Read(buffer, offset, count);

    public override async Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      bool dispose;
      CancellationTokenSource source = this.StartTimeout(cancellationToken, out dispose);
      int num;
      try
      {
        num = await this.wrappedStream.ReadAsync(buffer, offset, count, source.Token).ConfigureAwait(false);
      }
      catch (ObjectDisposedException ex)
      {
        source.Token.ThrowIfCancellationRequested();
        throw;
      }
      finally
      {
        this.StopTimeout(source, dispose);
      }
      source = (CancellationTokenSource) null;
      return num;
    }

    public override int ReadByte() => this.wrappedStream.ReadByte();

    public override long Seek(long offset, SeekOrigin origin) => this.wrappedStream.Seek(offset, origin);

    public override void SetLength(long value) => this.wrappedStream.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count) => this.wrappedStream.Write(buffer, offset, count);

    public override async Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      bool dispose;
      CancellationTokenSource source = this.StartTimeout(cancellationToken, out dispose);
      try
      {
        await this.wrappedStream.WriteAsync(buffer, offset, count, source.Token).ConfigureAwait(false);
      }
      catch (ObjectDisposedException ex)
      {
        source.Token.ThrowIfCancellationRequested();
        throw;
      }
      finally
      {
        this.StopTimeout(source, dispose);
      }
      source = (CancellationTokenSource) null;
    }

    public override void WriteByte(byte value) => this.wrappedStream.WriteByte(value);

    private void InitializeTokenSource()
    {
      this.cancellationTokenSource = new CancellationTokenSource();
      this.cancellationTokenSource.Token.Register((Action) (() => this.DisposeStream()));
    }

    private void DisposeStream() => this.wrappedStream.Dispose();

    private CancellationTokenSource StartTimeout(
      CancellationToken additionalToken,
      out bool dispose)
    {
      if (this.cancellationTokenSource.IsCancellationRequested)
        this.InitializeTokenSource();
      CancellationTokenSource cancellationTokenSource;
      if (additionalToken.CanBeCanceled)
      {
        cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(additionalToken, this.cancellationTokenSource.Token);
        dispose = true;
      }
      else
      {
        cancellationTokenSource = this.cancellationTokenSource;
        dispose = false;
      }
      this.cancellationTokenSource.CancelAfter(this.readTimeout);
      return cancellationTokenSource;
    }

    private void StopTimeout(CancellationTokenSource source, bool dispose)
    {
      this.cancellationTokenSource.CancelAfter(Timeout.InfiniteTimeSpan);
      if (!dispose)
        return;
      source.Dispose();
    }

    private void UpdateReadTimeout()
    {
      if (!this.wrappedStream.CanTimeout)
        return;
      if (!this.wrappedStream.CanRead)
        return;
      try
      {
        this.wrappedStream.ReadTimeout = (int) this.readTimeout.TotalMilliseconds;
      }
      catch
      {
      }
    }

    private void UpdateWriteTimeout()
    {
      if (!this.wrappedStream.CanTimeout)
        return;
      if (!this.wrappedStream.CanWrite)
        return;
      try
      {
        this.wrappedStream.WriteTimeout = (int) this.writeTimeout.TotalMilliseconds;
      }
      catch
      {
      }
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (!disposing)
        return;
      this.cancellationTokenSource.Dispose();
      this.wrappedStream.Dispose();
    }
  }
}
