// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.ByteCountingStream
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
  internal class ByteCountingStream : Stream
  {
    private readonly Stream wrappedStream;
    private readonly RequestResult requestObject;
    private readonly bool reverseCapture;

    public ByteCountingStream(
      Stream wrappedStream,
      RequestResult requestObject,
      bool reverseCapture = false)
    {
      CommonUtility.AssertNotNull("WrappedStream", (object) wrappedStream);
      CommonUtility.AssertNotNull("RequestObject", (object) requestObject);
      this.wrappedStream = wrappedStream;
      this.requestObject = requestObject;
      this.reverseCapture = reverseCapture;
    }

    public override bool CanRead => this.wrappedStream.CanRead;

    public override bool CanSeek => this.wrappedStream.CanSeek;

    public override bool CanTimeout => this.wrappedStream.CanTimeout;

    public override bool CanWrite => this.wrappedStream.CanWrite;

    public override long Length => this.wrappedStream.Length;

    public override long Position
    {
      get => this.wrappedStream.Position;
      set => this.wrappedStream.Position = value;
    }

    public override int ReadTimeout
    {
      get => this.wrappedStream.ReadTimeout;
      set => this.wrappedStream.ReadTimeout = value;
    }

    public override int WriteTimeout
    {
      get => this.wrappedStream.WriteTimeout;
      set => this.wrappedStream.WriteTimeout = value;
    }

    public override void Flush() => this.wrappedStream.Flush();

    public override void SetLength(long value) => this.wrappedStream.SetLength(value);

    public override long Seek(long offset, SeekOrigin origin) => this.wrappedStream.Seek(offset, origin);

    public override int Read(byte[] buffer, int offset, int count)
    {
      int count1 = this.wrappedStream.Read(buffer, offset, count);
      this.CaptureRead(count1);
      return count1;
    }

    public override async Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      int count1 = await this.wrappedStream.ReadAsync(buffer, offset, count, cancellationToken);
      this.CaptureRead(count1);
      return count1;
    }

    public override int ReadByte()
    {
      int num = this.wrappedStream.ReadByte();
      if (num == -1)
        return num;
      this.CaptureRead(1);
      return num;
    }

    public override void Close() => this.wrappedStream.Close();

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      return this.wrappedStream.BeginRead(buffer, offset, count, callback, state);
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
      int count = this.wrappedStream.EndRead(asyncResult);
      this.CaptureRead(count);
      return count;
    }

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      IAsyncResult asyncResult = this.wrappedStream.BeginWrite(buffer, offset, count, callback, state);
      this.CaptureWrite(count);
      return asyncResult;
    }

    public override void EndWrite(IAsyncResult asyncResult) => this.wrappedStream.EndWrite(asyncResult);

    public override void Write(byte[] buffer, int offset, int count)
    {
      this.wrappedStream.Write(buffer, offset, count);
      this.CaptureWrite(count);
    }

    public override async Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      await this.wrappedStream.WriteAsync(buffer, offset, count, cancellationToken);
      this.CaptureWrite(count);
    }

    public override void WriteByte(byte value)
    {
      this.wrappedStream.WriteByte(value);
      this.CaptureWrite(1);
    }

    private void CaptureWrite(int count)
    {
      if (this.reverseCapture)
        this.requestObject.IngressBytes += (long) count;
      else
        this.requestObject.EgressBytes += (long) count;
    }

    private void CaptureRead(int count)
    {
      if (this.reverseCapture)
        this.requestObject.EgressBytes += (long) count;
      else
        this.requestObject.IngressBytes += (long) count;
    }

    protected override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
      if (!disposing)
        return;
      this.wrappedStream.Dispose();
    }
  }
}
