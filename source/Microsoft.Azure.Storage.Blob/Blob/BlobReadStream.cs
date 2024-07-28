// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.BlobReadStream
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Blob
{
  internal sealed class BlobReadStream : BlobReadStreamBase
  {
    private volatile bool readPending;

    internal BlobReadStream(
      CloudBlob blob,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
      : base(blob, accessCondition, options, operationContext)
    {
    }

    public override int Read(byte[] buffer, int offset, int count) => this.ReadAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      return (IAsyncResult) CancellableAsyncResultTaskWrapper.Create<int>((Func<CancellationToken, Task<int>>) (token => this.ReadAsync(buffer, offset, count, token)), callback, state);
    }

    public override int EndRead(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<int>) asyncResult).GetAwaiter().GetResult();

    public override Task<int> ReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (buffer), (object) buffer);
      CommonUtility.AssertInBounds<int>(nameof (offset), offset, 0, buffer.Length);
      CommonUtility.AssertInBounds<int>(nameof (count), count, 0, buffer.Length - offset);
      if (this.lastException != null)
        throw this.lastException;
      if (this.currentOffset == this.Length || count == 0)
        return Task.FromResult<int>(0);
      int result = this.ConsumeBuffer(buffer, offset, count);
      return result > 0 ? Task.FromResult<int>(result) : this.DispatchReadAsync(buffer, offset, count, cancellationToken);
    }

    private async Task<int> DispatchReadAsync(
      byte[] buffer,
      int offset,
      int count,
      CancellationToken cancellationToken)
    {
      BlobReadStream blobReadStream = this;
      int num;
      try
      {
        blobReadStream.internalBuffer.SetLength(0L);
        await blobReadStream.blob.DownloadRangeToStreamAsync((Stream) blobReadStream.internalBuffer, new long?(blobReadStream.currentOffset), new long?((long) blobReadStream.GetReadSize()), blobReadStream.accessCondition, blobReadStream.options, blobReadStream.operationContext, cancellationToken).ConfigureAwait(false);
        blobReadStream.internalBuffer.Seek(0L, SeekOrigin.Begin);
        num = blobReadStream.ConsumeBuffer(buffer, offset, count);
      }
      catch (Exception ex)
      {
        blobReadStream.lastException = ex;
        throw;
      }
      return num;
    }
  }
}
