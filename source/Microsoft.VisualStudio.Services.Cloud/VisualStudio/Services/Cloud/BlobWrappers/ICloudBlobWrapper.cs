// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BlobWrappers.ICloudBlobWrapper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.BlobWrappers
{
  public interface ICloudBlobWrapper : ICloudBlobReadOnlyInfo
  {
    void Delete(
      DeleteSnapshotsOption deleteSnapshotsOption = DeleteSnapshotsOption.None,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    int DownloadRangeToByteArray(
      byte[] target,
      int index,
      long? blobOffset,
      long? length,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    Task<int> DownloadRangeToByteArrayAsync(
      byte[] target,
      int index,
      long? blobOffset,
      long? length,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    void PutBlock(
      string blockId,
      Stream blockData,
      string contentMD5,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    Task PutBlockAsync(
      string blockId,
      Stream blockData,
      string contentMD5,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    void PutBlockList(
      IEnumerable<string> blockList,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    Task PutBlockListAsync(
      IEnumerable<string> blockList,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    void WritePages(
      Stream pageData,
      long startOffset,
      string contentMD5 = null,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    void UploadFromByteArray(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    Task UploadFromByteArrayAsync(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    void SetMetadata(
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    string StartCopyFromBlob(
      Uri source,
      AccessCondition sourceAccessCondition = null,
      AccessCondition destAccessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    void FetchAttributes();

    Task FetchAttributesAsync();

    void CopyProperties(ICloudBlobWrapper other);
  }
}
