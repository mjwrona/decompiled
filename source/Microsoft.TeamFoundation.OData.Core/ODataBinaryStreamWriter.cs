// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataBinaryStreamWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Buffers;
using Microsoft.OData.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  internal sealed class ODataBinaryStreamWriter : Stream
  {
    private readonly TextWriter Writer;
    private byte[] trailingBytes = new byte[0];
    private char[] streamingBuffer;
    private ICharArrayPool bufferPool;
    private static byte[] emptyByteArray = new byte[0];

    public ODataBinaryStreamWriter(TextWriter writer) => this.Writer = writer;

    public ODataBinaryStreamWriter(
      TextWriter writer,
      ref char[] streamingBuffer,
      ICharArrayPool bufferPool)
    {
      this.Writer = writer;
      this.streamingBuffer = streamingBuffer;
      this.bufferPool = bufferPool;
    }

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => throw new NotImplementedException();

    public override long Position
    {
      get => throw new NotImplementedException();
      set => throw new NotImplementedException();
    }

    public override void Write(byte[] bytes, int offset, int count)
    {
      byte[] first = ODataBinaryStreamWriter.emptyByteArray;
      int length = this.trailingBytes.Length;
      int count1 = length > 0 ? 3 - length : 0;
      if (count + length < 3)
      {
        this.trailingBytes = ((IEnumerable<byte>) this.trailingBytes).Concat<byte>(((IEnumerable<byte>) bytes).Skip<byte>(offset).Take<byte>(count)).ToArray<byte>();
      }
      else
      {
        if (length > 0)
          first = ((IEnumerable<byte>) this.trailingBytes).Concat<byte>(((IEnumerable<byte>) bytes).Skip<byte>(offset).Take<byte>(count1)).ToArray<byte>();
        int count2 = (count - count1) % 3;
        this.trailingBytes = ((IEnumerable<byte>) bytes).Skip<byte>(offset + count - count2).Take<byte>(count2).ToArray<byte>();
        JsonValueUtils.WriteBinaryString(this.Writer, ((IEnumerable<byte>) first).Concat<byte>(((IEnumerable<byte>) bytes).Skip<byte>(offset + count1).Take<byte>(count - count1 - count2)).ToArray<byte>(), ref this.streamingBuffer, this.bufferPool);
      }
    }

    public override Task WriteAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      return TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.Write(buffer, offset, count)));
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotImplementedException();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

    public override void SetLength(long value) => throw new NotImplementedException();

    public override void Flush() => this.Writer.Flush();

    protected override void Dispose(bool disposing)
    {
      if (disposing && this.trailingBytes != null && this.trailingBytes.Length != 0)
      {
        this.Writer.Write(Convert.ToBase64String(this.trailingBytes, 0, this.trailingBytes.Length));
        this.trailingBytes = (byte[]) null;
      }
      this.Writer.Flush();
      base.Dispose(disposing);
    }
  }
}
