// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.AzureStorage.TracingCloudBlobContainer
// Assembly: Microsoft.VisualStudio.Services.BlobStore.AzureStorage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8BF1D977-E244-4825-BEA6-8BA4E1DDDB84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.AzureStorage.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.AzureStorage
{
  internal class TracingCloudBlobContainer : ICloudBlobContainer
  {
    private readonly ICloudBlobContainer container;
    private readonly ContainerStorageAccessStatistics accessStatistics;
    private readonly bool isBlobOperationTimeoutEnabled;

    public TracingCloudBlobContainer(
      ICloudBlobContainer container,
      ContainerStorageAccessStatistics accessStatistics,
      bool isBlobOperationTimeoutEnabled)
    {
      this.container = container;
      this.accessStatistics = accessStatistics ?? new ContainerStorageAccessStatistics();
      this.isBlobOperationTimeoutEnabled = isBlobOperationTimeoutEnabled;
    }

    public bool RequiresVssRequestContext => this.container.RequiresVssRequestContext;

    public string Name => this.container.Name;

    public string AccountName => this.container.AccountName;

    public StorageUri StorageUri => this.container.StorageUri;

    public Task<bool> CreateIfNotExistsAsync(
      VssRequestPump.Processor processor,
      BlobContainerPublicAccessType accessType = BlobContainerPublicAccessType.Off,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.accessStatistics.Increment(ContainerStorageAccessStatistics.Methods.CreateIfNotExistsAsync);
      return this.container.CreateIfNotExistsAsync(processor, accessType, options, operationContext);
    }

    public Task<bool> DeleteIfExistsAsync(
      VssRequestPump.Processor processor,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.accessStatistics.Increment(ContainerStorageAccessStatistics.Methods.DeleteIfExistsAsync);
      return this.container.DeleteIfExistsAsync(processor, accessCondition, options, operationContext);
    }

    public ICloudBlockBlob GetBlockBlobReference(string blobName)
    {
      this.accessStatistics.Increment(ContainerStorageAccessStatistics.Methods.GetBlockBlobReference);
      ICloudBlockBlob blockBlobReference = this.container.GetBlockBlobReference(blobName);
      BlobStorageAccessStatistics blobStatistics = this.accessStatistics.BlobStatistics;
      BlobRequestOptions defaultOptions;
      if (!this.isBlobOperationTimeoutEnabled)
      {
        defaultOptions = (BlobRequestOptions) null;
      }
      else
      {
        defaultOptions = new BlobRequestOptions();
        defaultOptions.MaximumExecutionTime = new TimeSpan?(TimeSpan.FromMinutes(5.0));
      }
      return (ICloudBlockBlob) new TracingCloudBlockBlob(blockBlobReference, blobStatistics, defaultOptions);
    }

    public Task<BlobContainerPermissions> GetPermissionsAsync(
      VssRequestPump.Processor processor,
      OperationContext operationContext)
    {
      this.accessStatistics.Increment(ContainerStorageAccessStatistics.Methods.GetPermissionsAsync);
      return this.container.GetPermissionsAsync(processor, operationContext);
    }

    public string GetSharedAccessSignature(
      VssRequestPump.Processor processor,
      SharedAccessBlobPolicy policy,
      string policyName)
    {
      this.accessStatistics.Increment(ContainerStorageAccessStatistics.Methods.GetSharedAccessSignature);
      return this.container.GetSharedAccessSignature(processor, policy, policyName);
    }

    public Task<IBlobResultSegment> ListBlobsSegmentedAsync(
      VssRequestPump.Processor processor,
      string prefix,
      bool useFlatListing,
      BlobListingDetails blobListingDetails,
      int? maxResults,
      BlobContinuationToken currentToken,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      this.accessStatistics.Increment(ContainerStorageAccessStatistics.Methods.ListBlobsSegmentedAsync);
      return this.container.ListBlobsSegmentedAsync(processor, prefix, useFlatListing, blobListingDetails, maxResults, currentToken, options, operationContext);
    }

    public Task SetPermissionsAsync(
      VssRequestPump.Processor processor,
      BlobContainerPermissions permissions,
      OperationContext operationContext)
    {
      this.accessStatistics.Increment(ContainerStorageAccessStatistics.Methods.SetPermissionsAsync);
      return this.container.SetPermissionsAsync(processor, permissions, operationContext);
    }
  }
}
