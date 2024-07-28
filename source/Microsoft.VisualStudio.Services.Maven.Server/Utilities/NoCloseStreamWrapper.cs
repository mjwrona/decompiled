// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Utilities.NoCloseStreamWrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using System;
using System.IO;
using System.Runtime.Remoting;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Utilities
{
  public class NoCloseStreamWrapper : Stream
  {
    private readonly Stream innerStream;

    public NoCloseStreamWrapper(Stream innerStream) => this.innerStream = innerStream;

    public override void Close()
    {
    }

    protected override void Dispose(bool disposing)
    {
    }

    public override string ToString() => this.innerStream.ToString();

    public override bool Equals(object obj) => this.innerStream.Equals(obj);

    public override int GetHashCode() => this.innerStream.GetHashCode();

    public override object InitializeLifetimeService() => this.innerStream.InitializeLifetimeService();

    public override ObjRef CreateObjRef(Type requestedType) => this.innerStream.CreateObjRef(requestedType);

    public override Task CopyToAsync(
      Stream destination,
      int bufferSize,
      CancellationToken cancellationToken)
    {
      return this.innerStream.CopyToAsync(destination, bufferSize, cancellationToken);
    }

    public override void Flush() => this.innerStream.Flush();

    public override Task FlushAsync(CancellationToken cancellationToken) => this.innerStream.FlushAsync(cancellationToken);

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      return this.innerStream.BeginRead(buffer, offset, count, callback, state);
    }

    public override int EndRead(IAsyncResult asyncResult) => this.innerStream.EndRead(asyncResult);

    public override Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      return this.innerStream.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      return this.innerStream.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void EndWrite(IAsyncResult asyncResult) => this.innerStream.EndWrite(asyncResult);

    public override Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      return this.innerStream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    public override long Seek(long offset, SeekOrigin origin) => this.innerStream.Seek(offset, origin);

    public override void SetLength(long value) => this.innerStream.SetLength(value);

    public override int Read(byte[] buffer, int offset, int count) => this.innerStream.Read(buffer, offset, count);

    public override int ReadByte() => this.innerStream.ReadByte();

    public override void Write(byte[] buffer, int offset, int count) => this.innerStream.Write(buffer, offset, count);

    public override void WriteByte(byte value) => this.innerStream.WriteByte(value);

    public override bool CanRead => this.innerStream.CanRead;

    public override bool CanSeek => this.innerStream.CanSeek;

    public override bool CanTimeout => this.innerStream.CanTimeout;

    public override bool CanWrite => this.innerStream.CanWrite;

    public override long Length => this.innerStream.Length;

    public override long Position
    {
      get => this.innerStream.Position;
      set => this.innerStream.Position = value;
    }

    public override int ReadTimeout
    {
      get => this.innerStream.ReadTimeout;
      set => this.innerStream.ReadTimeout = value;
    }

    public override int WriteTimeout
    {
      get => this.innerStream.WriteTimeout;
      set => this.innerStream.WriteTimeout = value;
    }
  }
}
