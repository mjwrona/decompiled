// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.FileReadStream_old
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.IO;

namespace Microsoft.Azure.Storage.File
{
  internal sealed class FileReadStream_old : FileReadStreamBase
  {
    private volatile bool readPending;

    internal FileReadStream_old(
      CloudFile file,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
      : base(file, accessCondition, options, operationContext)
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      CommonUtility.AssertNotNull(nameof (buffer), (object) buffer);
      CommonUtility.AssertInBounds<int>(nameof (offset), offset, 0, buffer.Length);
      CommonUtility.AssertInBounds<int>(nameof (count), count, 0, buffer.Length - offset);
      if (this.lastException != null)
        throw this.lastException;
      if (this.currentOffset == this.Length || count == 0)
        return 0;
      int num = this.ConsumeBuffer(buffer, offset, count);
      return num > 0 ? num : this.DispatchReadSync(buffer, offset, count);
    }

    public override IAsyncResult BeginRead(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      CommonUtility.AssertNotNull(nameof (buffer), (object) buffer);
      CommonUtility.AssertInBounds<int>(nameof (offset), offset, 0, buffer.Length);
      CommonUtility.AssertInBounds<int>(nameof (count), count, 0, buffer.Length - offset);
      if (this.readPending)
        throw new InvalidOperationException("File stream has a pending read operation. Please call EndRead first.");
      try
      {
        this.readPending = true;
        StorageAsyncResult<int> storageAsyncResult = new StorageAsyncResult<int>(callback, state);
        if (this.lastException != null)
        {
          storageAsyncResult.OnComplete(this.lastException);
          return (IAsyncResult) storageAsyncResult;
        }
        if (this.currentOffset == this.Length || count == 0)
        {
          storageAsyncResult.Result = 0;
          storageAsyncResult.OnComplete();
          return (IAsyncResult) storageAsyncResult;
        }
        int num = this.ConsumeBuffer(buffer, offset, count);
        if (num > 0)
        {
          storageAsyncResult.Result = num;
          storageAsyncResult.OnComplete();
          return (IAsyncResult) storageAsyncResult;
        }
        this.DispatchReadAsync(storageAsyncResult, buffer, offset, count);
        return (IAsyncResult) storageAsyncResult;
      }
      catch (Exception ex)
      {
        this.readPending = false;
        throw;
      }
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
      StorageAsyncResult<int> storageAsyncResult = (StorageAsyncResult<int>) asyncResult;
      this.readPending = false;
      storageAsyncResult.End();
      return storageAsyncResult.Result;
    }

    private void DispatchReadAsync(
      StorageAsyncResult<int> storageAsyncResult,
      byte[] buffer,
      int offset,
      int count)
    {
      storageAsyncResult.OperationState = (object) new ArraySegment<byte>(buffer, offset, count);
      try
      {
        this.internalBuffer.SetLength(0L);
        this.file.BeginDownloadRangeToStream((Stream) this.internalBuffer, new long?(this.currentOffset), new long?((long) this.GetReadSize()), (AccessCondition) null, this.options, this.operationContext, new AsyncCallback(this.DownloadRangeToStreamCallback), (object) storageAsyncResult);
      }
      catch (Exception ex)
      {
        this.lastException = ex;
        throw;
      }
    }

    private void DownloadRangeToStreamCallback(IAsyncResult ar)
    {
      StorageAsyncResult<int> asyncState = (StorageAsyncResult<int>) ar.AsyncState;
      asyncState.UpdateCompletedSynchronously(ar.CompletedSynchronously);
      try
      {
        this.file.EndDownloadRangeToStream(ar);
        if (!this.file.Properties.ETag.Equals(this.accessCondition.IfMatchETag, StringComparison.Ordinal))
          throw new StorageException(new RequestResult()
          {
            HttpStatusMessage = (string) null,
            HttpStatusCode = 412,
            ExtendedErrorInformation = (StorageExtendedErrorInformation) null
          }, "The condition specified using HTTP conditional header(s) is not met.", (Exception) null);
        ArraySegment<byte> operationState = (ArraySegment<byte>) asyncState.OperationState;
        this.internalBuffer.Seek(0L, SeekOrigin.Begin);
        asyncState.Result = this.ConsumeBuffer(operationState.Array, operationState.Offset, operationState.Count);
      }
      catch (Exception ex)
      {
        this.lastException = ex;
      }
      asyncState.OnComplete(this.lastException);
    }

    private int DispatchReadSync(byte[] buffer, int offset, int count)
    {
      try
      {
        this.internalBuffer.SetLength(0L);
        this.file.DownloadRangeToStream((Stream) this.internalBuffer, new long?(this.currentOffset), new long?((long) this.GetReadSize()), options: this.options, operationContext: this.operationContext);
        if (!this.file.Properties.ETag.Equals(this.accessCondition.IfMatchETag, StringComparison.Ordinal))
          throw new StorageException(new RequestResult()
          {
            HttpStatusMessage = (string) null,
            HttpStatusCode = 412,
            ExtendedErrorInformation = (StorageExtendedErrorInformation) null
          }, "The condition specified using HTTP conditional header(s) is not met.", (Exception) null);
        this.internalBuffer.Seek(0L, SeekOrigin.Begin);
        return this.ConsumeBuffer(buffer, offset, count);
      }
      catch (Exception ex)
      {
        this.lastException = ex;
        throw;
      }
    }
  }
}
