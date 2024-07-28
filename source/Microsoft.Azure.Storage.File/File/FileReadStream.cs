// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.FileReadStream
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.File
{
  internal sealed class FileReadStream : FileReadStreamBase
  {
    internal FileReadStream(
      CloudFile file,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
      : base(file, accessCondition, options, operationContext)
    {
    }

    internal FileReadStream(FileReadStream otherStream)
      : this(otherStream.file, otherStream.accessCondition, otherStream.options, otherStream.operationContext)
    {
    }

    public string ContentType => this.fileProperties.ContentType;

    public override int Read(byte[] buffer, int offset, int count) => this.ReadAsync(buffer, offset, count, CancellationToken.None).GetAwaiter().GetResult();

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
      FileReadStream fileReadStream = this;
      int num;
      try
      {
        fileReadStream.internalBuffer.SetLength(0L);
        await fileReadStream.file.DownloadRangeToStreamAsync((Stream) fileReadStream.internalBuffer, new long?(fileReadStream.currentOffset), new long?((long) fileReadStream.GetReadSize()), (AccessCondition) null, fileReadStream.options, fileReadStream.operationContext, cancellationToken).ConfigureAwait(false);
        if (!fileReadStream.file.Properties.ETag.Equals(fileReadStream.accessCondition.IfMatchETag, StringComparison.Ordinal))
          throw new StorageException(new RequestResult()
          {
            HttpStatusMessage = (string) null,
            HttpStatusCode = 412,
            ExtendedErrorInformation = (StorageExtendedErrorInformation) null
          }, "The condition specified using HTTP conditional header(s) is not met.", (Exception) null);
        fileReadStream.internalBuffer.Seek(0L, SeekOrigin.Begin);
        num = fileReadStream.ConsumeBuffer(buffer, offset, count);
      }
      catch (Exception ex)
      {
        fileReadStream.lastException = ex;
        throw;
      }
      return num;
    }
  }
}
