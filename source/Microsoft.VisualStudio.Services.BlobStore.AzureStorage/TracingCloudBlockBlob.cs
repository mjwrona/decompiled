// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.TracingCloudBlockBlob
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  internal class TracingCloudBlockBlob : ICloudBlockBlob, IListBlobItem
  {
    private readonly ICloudBlockBlob blob;
    private readonly BlobStorageAccessStatistics accessStatistics;
    private readonly BlobRequestOptions defaultOptions;

    public TracingCloudBlockBlob(
      ICloudBlockBlob blob,
      BlobStorageAccessStatistics accessStatistics,
      BlobRequestOptions defaultOptions)
    {
      this.blob = blob;
      this.accessStatistics = accessStatistics ?? new BlobStorageAccessStatistics();
      this.defaultOptions = defaultOptions;
    }

    public BlobProperties Properties => this.blob.Properties;

    public string Name => this.blob.Name;

    public IDictionary<string, string> Metadata => this.blob.Metadata;

    public Uri Uri => this.blob.Uri;

    public StorageUri StorageUri => this.blob.StorageUri;

    public CloudBlobDirectory Parent => this.blob.Parent;

    public CloudBlobContainer Container => this.blob.Container;

    public Task<bool> DeleteIfExistsAsync(
      VssRequestPump.Processor processor,
      DeleteSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      this.accessStatistics.Increment(BlobStorageAccessStatistics.Methods.DeleteIfExistsAsync);
      return this.blob.DeleteIfExistsAsync(processor, deleteSnapshotsOption, accessCondition, options ?? this.defaultOptions, operationContext);
    }

    public Task<IEnumerable<ListBlockItem>> DownloadBlockListAsync(
      VssRequestPump.Processor processor,
      BlockListingFilter blockListingFilter,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      this.accessStatistics.Increment(BlobStorageAccessStatistics.Methods.DownloadBlockListAsync);
      return this.blob.DownloadBlockListAsync(processor, blockListingFilter, accessCondition, options ?? this.defaultOptions, operationContext);
    }

    public Task DownloadToStreamNeedsRetryAsync(
      VssRequestPump.Processor processor,
      Stream target,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.accessStatistics.Increment(BlobStorageAccessStatistics.Methods.DownloadToStreamNeedsRetryAsync);
      return this.blob.DownloadToStreamNeedsRetryAsync(processor, target, accessCondition, options ?? this.defaultOptions, operationContext);
    }

    public Task<bool> ExistsAsync(
      VssRequestPump.Processor processor,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.accessStatistics.Increment(BlobStorageAccessStatistics.Methods.ExistsAsync);
      return this.blob.ExistsAsync(processor, options ?? this.defaultOptions, operationContext);
    }

    public Task FetchAttributesAsync(
      VssRequestPump.Processor processor,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.accessStatistics.Increment(BlobStorageAccessStatistics.Methods.FetchAttributesAsync);
      return this.blob.FetchAttributesAsync(processor, accessCondition, options ?? this.defaultOptions, operationContext);
    }

    public ICloudBlobContainer GetContainer() => this.blob.GetContainer();

    public string GetSharedAccessSignature(
      SharedAccessBlobPolicy policy,
      SharedAccessBlobHeaders headers,
      string policyName)
    {
      this.accessStatistics.Increment(BlobStorageAccessStatistics.Methods.GetSharedAccessSignature);
      return this.blob.GetSharedAccessSignature(policy, headers, policyName);
    }

    public Task<Stream> OpenReadNeedsRetryAsync(
      VssRequestPump.Processor processor,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.accessStatistics.Increment(BlobStorageAccessStatistics.Methods.OpenReadNeedsRetryAsync);
      return this.blob.OpenReadNeedsRetryAsync(processor, accessCondition, options ?? this.defaultOptions, operationContext);
    }

    public Task PutBlockByteArrayAsync(
      VssRequestPump.Processor processor,
      string blockId,
      ArraySegment<byte> blockData,
      OperationContext operationContext = null)
    {
      this.accessStatistics.Increment(BlobStorageAccessStatistics.Methods.PutBlockAsync);
      return this.blob.PutBlockByteArrayAsync(processor, blockId, blockData, operationContext);
    }

    public Task PutBlockListAsync(
      VssRequestPump.Processor processor,
      IEnumerable<string> blockList,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      this.accessStatistics.Increment(BlobStorageAccessStatistics.Methods.PutBlockListAsync);
      return this.blob.PutBlockListAsync(processor, blockList, accessCondition, options ?? this.defaultOptions, operationContext);
    }

    public Task PutBlockStreamAsync(
      VssRequestPump.Processor processor,
      string blockId,
      Stream blockData,
      OperationContext operationContext = null)
    {
      this.accessStatistics.Increment(BlobStorageAccessStatistics.Methods.PutBlockAsync);
      return this.blob.PutBlockStreamAsync(processor, blockId, blockData, operationContext);
    }

    public Task SetMetadataAsync(
      VssRequestPump.Processor processor,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.accessStatistics.Increment(BlobStorageAccessStatistics.Methods.SetMetadataAsync);
      return this.blob.SetMetadataAsync(processor, accessCondition, options ?? this.defaultOptions, operationContext);
    }

    public Task<bool> UndeleteAsync(
      VssRequestPump.Processor processor,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      this.accessStatistics.Increment(BlobStorageAccessStatistics.Methods.UndeleteAsync);
      return this.blob.UndeleteAsync(processor, accessCondition, options ?? this.defaultOptions, operationContext);
    }

    public Task UploadFromByteArrayAsync(
      VssRequestPump.Processor processor,
      ArraySegment<byte> buffer,
      bool useHttpClient,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.accessStatistics.Increment(BlobStorageAccessStatistics.Methods.UploadFromByteArrayAsync);
      return this.blob.UploadFromByteArrayAsync(processor, buffer, useHttpClient, accessCondition, options ?? this.defaultOptions, operationContext);
    }
  }
}
