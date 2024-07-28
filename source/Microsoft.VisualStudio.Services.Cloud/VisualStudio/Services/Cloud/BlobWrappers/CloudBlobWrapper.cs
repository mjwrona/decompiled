// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.BlobWrappers.CloudBlobWrapper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.BlobWrappers
{
  public class CloudBlobWrapper : ICloudBlobWrapper, ICloudBlobReadOnlyInfo
  {
    internal ICloudBlob m_blob;
    public static readonly string SourceLastModifiedKey = "SourceLastModified";
    public static readonly string TargetOrphanedDate = nameof (TargetOrphanedDate);

    public CloudBlobWrapper(ICloudBlob blob) => this.m_blob = blob;

    public IDictionary<string, string> Metadata => this.m_blob.Metadata;

    public BlobType BlobType => this.m_blob.BlobType;

    public bool IsServerEncrypted => this.m_blob.Properties.IsServerEncrypted;

    public Uri Uri => this.m_blob.Uri;

    public string Name => this.m_blob.Name;

    public string ContainerName => this.m_blob.Container.Name;

    public string ETag => this.m_blob.Properties.ETag;

    public string ContentMD5 => this.m_blob.Properties.ContentMD5;

    public CopyState CopyState => this.m_blob.CopyState;

    public void Delete(
      DeleteSnapshotsOption deleteSnapshotsOption = DeleteSnapshotsOption.None,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.m_blob.Delete(deleteSnapshotsOption, accessCondition, options, operationContext);
    }

    public int DownloadRangeToByteArray(
      byte[] target,
      int index,
      long? blobOffset,
      long? length,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return this.m_blob.DownloadRangeToByteArray(target, index, blobOffset, length, accessCondition, options, operationContext);
    }

    public async Task<int> DownloadRangeToByteArrayAsync(
      byte[] target,
      int index,
      long? blobOffset,
      long? length,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return await this.m_blob.DownloadRangeToByteArrayAsync(target, index, blobOffset, length, accessCondition, options, operationContext);
    }

    public void PutBlock(
      string blockId,
      Stream blockData,
      string contentMD5,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      (this.m_blob as CloudBlockBlob).PutBlock(blockId, blockData, (Checksum) contentMD5, accessCondition, options, operationContext);
    }

    public async Task PutBlockAsync(
      string blockId,
      Stream blockData,
      string contentMD5,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      await (this.m_blob as CloudBlockBlob).PutBlockAsync(blockId, blockData, contentMD5, accessCondition, options, operationContext);
    }

    public void PutBlockList(
      IEnumerable<string> blockList,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      (this.m_blob as CloudBlockBlob).PutBlockList(blockList, accessCondition, options, operationContext);
    }

    public async Task PutBlockListAsync(
      IEnumerable<string> blockList,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      await (this.m_blob as CloudBlockBlob).PutBlockListAsync(blockList, accessCondition, options, operationContext);
    }

    public void WritePages(
      Stream pageData,
      long startOffset,
      string contentMD5 = null,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      (this.m_blob as CloudPageBlob).WritePages(pageData, startOffset, (Checksum) contentMD5, accessCondition, options, operationContext);
    }

    public long GetLength() => this.m_blob.Properties.Length;

    public DateTimeOffset? GetLastModified() => this.m_blob.Properties.LastModified;

    public void UploadFromByteArray(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.m_blob.UploadFromByteArray(buffer, index, count, accessCondition, options, operationContext);
    }

    public async Task UploadFromByteArrayAsync(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      await this.m_blob.UploadFromByteArrayAsync(buffer, index, count, accessCondition, options, operationContext);
    }

    public void CopyProperties(ICloudBlobWrapper other)
    {
      ICloudBlob blob = (other as CloudBlobWrapper).m_blob;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) blob.Metadata)
      {
        if (!string.Equals(keyValuePair.Key, CloudBlobWrapper.SourceLastModifiedKey) && !string.Equals(keyValuePair.Key, CloudBlobWrapper.TargetOrphanedDate))
          this.m_blob.Metadata[keyValuePair.Key] = keyValuePair.Value;
      }
      if (!this.m_blob.Metadata.ContainsKey(CloudBlobWrapper.SourceLastModifiedKey))
        this.m_blob.Metadata[CloudBlobWrapper.SourceLastModifiedKey] = blob.Properties.LastModified.ToString();
      BlobProperties properties = blob.Properties;
      this.m_blob.Properties.CacheControl = properties.CacheControl;
      this.m_blob.Properties.ContentDisposition = properties.ContentDisposition;
      this.m_blob.Properties.ContentEncoding = properties.ContentEncoding;
      this.m_blob.Properties.ContentLanguage = properties.ContentLanguage;
      this.m_blob.Properties.ContentMD5 = properties.ContentMD5;
      this.m_blob.Properties.ContentType = properties.ContentType;
    }

    public void SetMetadata(
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.m_blob.SetMetadata(accessCondition, options, operationContext);
    }

    public string StartCopyFromBlob(
      Uri source,
      AccessCondition sourceAccessCondition = null,
      AccessCondition destAccessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return ((CloudBlob) this.m_blob).StartCopy(source, sourceAccessCondition, destAccessCondition, options, operationContext);
    }

    public void FetchAttributes() => this.m_blob.FetchAttributes();

    public async Task FetchAttributesAsync() => await this.m_blob.FetchAttributesAsync();
  }
}
