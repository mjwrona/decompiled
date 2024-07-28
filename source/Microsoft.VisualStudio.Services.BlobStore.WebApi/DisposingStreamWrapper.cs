// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.DisposingStreamWrapper
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public class DisposingStreamWrapper : Stream
  {
    private readonly Stream streamImplementation;
    private IReadOnlyList<IDisposable> disposables;

    public DisposingStreamWrapper(Stream streamImplementation, params IDisposable[] disposables)
      : this(streamImplementation, (IEnumerable<IDisposable>) disposables)
    {
    }

    public DisposingStreamWrapper(Stream streamImplementation, IEnumerable<IDisposable> disposables)
    {
      this.streamImplementation = streamImplementation;
      this.disposables = (IReadOnlyList<IDisposable>) disposables.Prepend<IDisposable>((IDisposable) streamImplementation).ToList<IDisposable>();
    }

    private void DisposeAllDisposables()
    {
      IReadOnlyList<IDisposable> disposableList = Interlocked.Exchange<IReadOnlyList<IDisposable>>(ref this.disposables, (IReadOnlyList<IDisposable>) null);
      if (disposableList == null)
        return;
      foreach (IDisposable disposable in (IEnumerable<IDisposable>) disposableList)
        disposable.Dispose();
    }

    public override Task CopyToAsync(
      Stream destination,
      int bufferSize,
      CancellationToken cancellationToken)
    {
      return this.streamImplementation.CopyToAsync(destination, bufferSize, cancellationToken);
    }

    public override void Close() => this.DisposeAllDisposables();

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.DisposeAllDisposables();
    }

    public override void Flush() => this.streamImplementation.Flush();

    public override Task FlushAsync(CancellationToken cancellationToken) => this.streamImplementation.FlushAsync(cancellationToken);

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      return this.streamImplementation.BeginRead(buffer, offset, count, callback, state);
    }

    public override int EndRead(IAsyncResult asyncResult) => this.streamImplementation.EndRead(asyncResult);

    public override Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      return this.streamImplementation.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      return this.streamImplementation.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void EndWrite(IAsyncResult asyncResult) => this.streamImplementation.EndWrite(asyncResult);

    public override Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      return this.streamImplementation.WriteAsync(buffer, offset, count, cancellationToken);
    }

    public override long Seek(long offset, SeekOrigin origin) => this.streamImplementation.Seek(offset, origin);

    public override void SetLength(long value) => this.streamImplementation.SetLength(value);

    public override int Read(byte[] buffer, int offset, int count) => this.streamImplementation.Read(buffer, offset, count);

    public override int ReadByte() => this.streamImplementation.ReadByte();

    public override void Write(byte[] buffer, int offset, int count) => this.streamImplementation.Write(buffer, offset, count);

    public override void WriteByte(byte value) => this.streamImplementation.WriteByte(value);

    public override bool CanRead => this.streamImplementation.CanRead;

    public override bool CanSeek => this.streamImplementation.CanSeek;

    public override bool CanTimeout => this.streamImplementation.CanTimeout;

    public override bool CanWrite => this.streamImplementation.CanWrite;

    public override long Length => this.streamImplementation.Length;

    public override long Position
    {
      get => this.streamImplementation.Position;
      set => this.streamImplementation.Position = value;
    }

    public override int ReadTimeout
    {
      get => this.streamImplementation.ReadTimeout;
      set => this.streamImplementation.ReadTimeout = value;
    }

    public override int WriteTimeout
    {
      get => this.streamImplementation.WriteTimeout;
      set => this.streamImplementation.WriteTimeout = value;
    }
  }
}
