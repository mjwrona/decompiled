// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataReadStream
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;

namespace Microsoft.OData
{
  internal abstract class ODataReadStream : ODataStream
  {
    protected ODataBatchReaderStream batchReaderStream;

    private ODataReadStream(ODataBatchReaderStream batchReaderStream, IODataStreamListener listener)
      : base(listener)
    {
      this.batchReaderStream = batchReaderStream;
    }

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
      get => throw new NotSupportedException();
      set => throw new NotSupportedException();
    }

    public override void Flush() => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    internal static ODataReadStream Create(
      ODataBatchReaderStream batchReaderStream,
      IODataStreamListener listener,
      int length)
    {
      return (ODataReadStream) new ODataReadStream.ODataBatchOperationReadStreamWithLength(batchReaderStream, listener, length);
    }

    internal static ODataReadStream Create(
      ODataBatchReaderStream batchReaderStream,
      IODataStreamListener listener)
    {
      return (ODataReadStream) new ODataReadStream.ODataBatchOperationReadStreamWithDelimiter(batchReaderStream, listener);
    }

    private sealed class ODataBatchOperationReadStreamWithLength : ODataReadStream
    {
      private int length;

      internal ODataBatchOperationReadStreamWithLength(
        ODataBatchReaderStream batchReaderStream,
        IODataStreamListener listener,
        int length)
        : base(batchReaderStream, listener)
      {
        ExceptionUtils.CheckIntegerNotNegative(length, nameof (length));
        this.length = length;
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        ExceptionUtils.CheckArgumentNotNull<byte[]>(buffer, nameof (buffer));
        ExceptionUtils.CheckIntegerNotNegative(offset, nameof (offset));
        ExceptionUtils.CheckIntegerNotNegative(count, nameof (count));
        this.ValidateNotDisposed();
        if (this.length == 0)
          return 0;
        int num = this.batchReaderStream.ReadWithLength(buffer, offset, Math.Min(count, this.length));
        this.length -= num;
        return num;
      }
    }

    private sealed class ODataBatchOperationReadStreamWithDelimiter : ODataReadStream
    {
      private bool exhausted;

      internal ODataBatchOperationReadStreamWithDelimiter(
        ODataBatchReaderStream batchReaderStream,
        IODataStreamListener listener)
        : base(batchReaderStream, listener)
      {
      }

      public override int Read(byte[] buffer, int offset, int count)
      {
        ExceptionUtils.CheckArgumentNotNull<byte[]>(buffer, nameof (buffer));
        ExceptionUtils.CheckIntegerNotNegative(offset, nameof (offset));
        ExceptionUtils.CheckIntegerNotNegative(count, nameof (count));
        this.ValidateNotDisposed();
        if (this.exhausted)
          return 0;
        int num = this.batchReaderStream.ReadWithDelimiter(buffer, offset, count);
        if (num < count)
          this.exhausted = true;
        return num;
      }
    }
  }
}
