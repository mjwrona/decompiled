// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.ParallelDownloadToFile
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Blob
{
  internal sealed class ParallelDownloadToFile
  {
    private CloudBlob Blob;
    private readonly long Offset;
    private long? Length;
    private readonly string FilePath;
    private CancellationToken cancellationToken;
    private BlobRequestOptions blobRequestOptions;
    private OperationContext operationContext;
    private AccessCondition accessCondition;

    public Task Task { get; private set; }

    private int MaxIdleTimeInMs { get; set; }

    private ParallelDownloadToFile(
      CloudBlob blob,
      string filePath,
      long offset,
      long? length,
      int maxIdleTimeInMs,
      CancellationToken cancellationToken)
    {
      this.FilePath = filePath;
      this.Blob = blob;
      this.Offset = offset;
      this.Length = length;
      this.MaxIdleTimeInMs = maxIdleTimeInMs;
      this.cancellationToken = cancellationToken;
    }

    public static ParallelDownloadToFile Start(
      CloudBlob blob,
      string filePath,
      FileMode fileMode,
      int parallelIOCount,
      long? rangeSizeInBytes,
      long offset,
      long? length,
      int maxIdleTimeInMs,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      ParallelDownloadToFile parallelDownloadToFile = new ParallelDownloadToFile(blob, filePath, offset, length, maxIdleTimeInMs, cancellationToken)
      {
        operationContext = operationContext,
        blobRequestOptions = options,
        accessCondition = accessCondition
      };
      parallelDownloadToFile.Task = parallelDownloadToFile.StartAsync(fileMode, parallelIOCount, rangeSizeInBytes);
      return parallelDownloadToFile;
    }

    private async Task StartAsync(FileMode fileMode, int parallelIOCount, long? rangeSizeInBytes)
    {
      CommonUtility.AssertInBounds<int>(nameof (parallelIOCount), parallelIOCount, 1);
      BlobRequestOptions blobRequestOptions1 = this.blobRequestOptions;
      bool? nullable;
      int num1;
      if (blobRequestOptions1 == null)
      {
        num1 = 0;
      }
      else
      {
        nullable = (bool?) blobRequestOptions1.ChecksumOptions?.UseTransactionalMD5;
        bool flag = true;
        num1 = nullable.GetValueOrDefault() == flag & nullable.HasValue ? 1 : 0;
      }
      bool useTransactionalMD5 = num1 != 0;
      BlobRequestOptions blobRequestOptions2 = this.blobRequestOptions;
      int num2;
      if (blobRequestOptions2 == null)
      {
        num2 = 0;
      }
      else
      {
        nullable = (bool?) blobRequestOptions2.ChecksumOptions?.UseTransactionalCRC64;
        bool flag = true;
        num2 = nullable.GetValueOrDefault() == flag & nullable.HasValue ? 1 : 0;
      }
      bool useTransactionalCRC64 = num2 != 0;
      rangeSizeInBytes = new long?(this.ValidateOrGetRangeSize(useTransactionalMD5, useTransactionalCRC64, rangeSizeInBytes));
      ConfiguredTaskAwaitable configuredTaskAwaitable = this.Blob.FetchAttributesAsync(this.accessCondition, this.blobRequestOptions, this.operationContext).ConfigureAwait(false);
      await configuredTaskAwaitable;
      if (this.accessCondition == null)
        this.accessCondition = new AccessCondition();
      this.accessCondition.IfMatchETag = this.Blob.Properties.ETag;
      long val2 = this.Blob.Properties.Length - this.Offset;
      this.Length = this.Length.HasValue ? new long?(Math.Min(this.Length.Value, val2)) : new long?(val2);
      List<Task> downloadTaskList;
      Dictionary<Task, MemoryMappedViewStream> taskToStream;
      if (this.Offset == 0L && this.Length.Value == 0L)
      {
        File.Create(this.FilePath).Close();
        downloadTaskList = (List<Task>) null;
        taskToStream = (Dictionary<Task, MemoryMappedViewStream>) null;
      }
      else
      {
        CommonUtility.AssertInBounds<long>("length", this.Length.Value, 1L, long.MaxValue);
        int totalIOReadCalls = (int) Math.Ceiling((double) this.Length.Value / (double) rangeSizeInBytes.Value);
        downloadTaskList = new List<Task>();
        taskToStream = new Dictionary<Task, MemoryMappedViewStream>();
        using (MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(this.FilePath, fileMode, (string) null, this.Length.Value))
        {
          int num3;
          int num4 = num3 - 1;
          try
          {
            Task downloadRangeTask;
            for (int i = 0; i < totalIOReadCalls; ++i)
            {
              if (downloadTaskList.Count >= parallelIOCount)
              {
                downloadRangeTask = await Task.WhenAny((IEnumerable<Task>) downloadTaskList).ConfigureAwait(false);
                configuredTaskAwaitable = downloadRangeTask.ConfigureAwait(false);
                await configuredTaskAwaitable;
                taskToStream[downloadRangeTask].Dispose();
                taskToStream.Remove(downloadRangeTask);
                downloadTaskList.Remove(downloadRangeTask);
                downloadRangeTask = (Task) null;
              }
              long offset = (long) i * rangeSizeInBytes.Value;
              long num5 = rangeSizeInBytes.Value;
              if (i == totalIOReadCalls - 1)
                num5 = this.Length.Value - (long) i * num5;
              MemoryMappedViewStream viewStream = mmf.CreateViewStream(offset, num5);
              Task streamWrapperAsync = this.DownloadToStreamWrapperAsync(viewStream, this.Offset + offset, num5, this.accessCondition, this.blobRequestOptions, this.operationContext, this.cancellationToken);
              taskToStream.Add(streamWrapperAsync, viewStream);
              downloadTaskList.Add(streamWrapperAsync);
            }
            while (downloadTaskList.Count > 0)
            {
              downloadRangeTask = await Task.WhenAny((IEnumerable<Task>) downloadTaskList).ConfigureAwait(false);
              configuredTaskAwaitable = downloadRangeTask.ConfigureAwait(false);
              await configuredTaskAwaitable;
              taskToStream[downloadRangeTask].Dispose();
              taskToStream.Remove(downloadRangeTask);
              downloadTaskList.Remove(downloadRangeTask);
              downloadRangeTask = (Task) null;
            }
          }
          finally
          {
            foreach (KeyValuePair<Task, MemoryMappedViewStream> keyValuePair in taskToStream)
              keyValuePair.Value.Dispose();
          }
        }
        downloadTaskList = (List<Task>) null;
        taskToStream = (Dictionary<Task, MemoryMappedViewStream>) null;
      }
    }

    private async Task DownloadToStreamWrapperAsync(
      MemoryMappedViewStream viewStream,
      long blobOffset,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      long startingOffset = blobOffset;
      long startingLength = length;
      ParallelDownloadStream largeDownloadStream = (ParallelDownloadStream) null;
      try
      {
label_2:
        try
        {
          largeDownloadStream = new ParallelDownloadStream(viewStream, this.MaxIdleTimeInMs);
          using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(largeDownloadStream.HangingCancellationToken, cancellationToken))
            await this.Blob.DownloadRangeToStreamAsync((Stream) largeDownloadStream, new long?(blobOffset), new long?(length), this.accessCondition, this.blobRequestOptions, this.operationContext, cts.Token).ConfigureAwait(false);
          largeDownloadStream = (ParallelDownloadStream) null;
        }
        catch (OperationCanceledException ex)
        {
          if (!cancellationToken.IsCancellationRequested)
          {
            blobOffset = startingOffset + largeDownloadStream.Position;
            length = startingLength - largeDownloadStream.Position;
            if (length == 0L)
              largeDownloadStream = (ParallelDownloadStream) null;
            else
              goto label_2;
          }
          else
            throw;
        }
      }
      finally
      {
        largeDownloadStream?.Close();
      }
    }

    private long ValidateOrGetRangeSize(
      bool useTransactionalMD5,
      bool useTransactionalCRC64,
      long? rangeSizeInBytes)
    {
      if (rangeSizeInBytes.HasValue)
      {
        CommonUtility.AssertInBounds<long>(nameof (rangeSizeInBytes), rangeSizeInBytes.Value, 4194304L);
        if (useTransactionalMD5 && rangeSizeInBytes.Value != 4194304L)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The argument '{0}' is invalid when using MD5. When MD5 is enabled the range size must be '{1}' MB.", (object) rangeSizeInBytes, (object) 4194304));
        if (useTransactionalCRC64 && rangeSizeInBytes.Value != 4194304L)
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The argument '{0}' is invalid when using CRC64. When CRC64 is enabled the range size must be '{1}' MB.", (object) rangeSizeInBytes, (object) 4194304));
        long? nullable1 = rangeSizeInBytes;
        long? nullable2 = nullable1.HasValue ? new long?(nullable1.GetValueOrDefault() % 4096L) : new long?();
        long num = 0;
        if (!(nullable2.GetValueOrDefault() == num & nullable2.HasValue))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The range size of '{0}' is invalid. Please use a size that is greater than or equal to '{1}' MB and is a multiple of 4 KB.", (object) rangeSizeInBytes, (object) 4194304));
      }
      else
        rangeSizeInBytes = !useTransactionalMD5 ? (!useTransactionalCRC64 ? new long?(16777216L) : new long?(4194304L)) : new long?(4194304L);
      return rangeSizeInBytes.Value;
    }
  }
}
