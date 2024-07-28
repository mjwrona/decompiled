// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataWriteStream
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal sealed class ODataWriteStream : ODataStream
  {
    private Stream stream;

    internal ODataWriteStream(Stream stream, IODataStreamListener listener)
      : base(listener)
    {
      this.stream = stream;
    }

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length
    {
      get
      {
        this.ValidateNotDisposed();
        return this.stream.Length;
      }
    }

    public override long Position
    {
      get
      {
        this.ValidateNotDisposed();
        return this.stream.Position;
      }
      set => throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
      this.ValidateNotDisposed();
      this.stream.SetLength(value);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      this.ValidateNotDisposed();
      this.stream.Write(buffer, offset, count);
    }

    public override Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      this.ValidateNotDisposed();
      return this.stream.WriteAsync(buffer, offset, count, cancellationToken);
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override void Flush()
    {
      this.ValidateNotDisposed();
      this.stream.Flush();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        this.stream = (Stream) null;
      base.Dispose(disposing);
    }
  }
}
