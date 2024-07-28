// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.CloudBlobContainerWrapper
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class CloudBlobContainerWrapper : ICloudBlobContainer
  {
    private static readonly StorageUri DevStorageUri = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudBlobClient().StorageUri;
    internal readonly CloudBlobContainer container;
    public readonly bool SupportsHttps;

    public CloudBlobContainerWrapper(CloudBlobContainer container, bool enableTracing)
    {
      this.SupportsHttps = container.ServiceClient.StorageUri != CloudBlobContainerWrapper.DevStorageUri;
      this.container = container;
      this.RequiresVssRequestContext = enableTracing;
    }

    public bool RequiresVssRequestContext { get; private set; }

    public string Name => this.container.Name;

    public string AccountName
    {
      get
      {
        CloudBlobContainer container = this.container;
        return container == null ? (string) null : container.GetAccountName();
      }
    }

    public StorageUri StorageUri => this.container.StorageUri;

    public async Task<IBlobResultSegment> ListBlobsSegmentedAsync(
      VssRequestPump.Processor processor,
      string prefix,
      [Optional] bool useFlatListing,
      [Optional] BlobListingDetails blobListingDetails,
      [Optional] int? maxResults,
      [Optional] BlobContinuationToken currentToken,
      [Optional] BlobRequestOptions options,
      [Optional] OperationContext operationContext)
    {
      CloudBlobContainerWrapper container = this;
      IBlobResultSegment blobResultSegment1;
      using (new ActivityLogPerfTimer(processor, "BlobStorage"))
      {
        operationContext = processor.CreateStorageContext(operationContext);
        Microsoft.Azure.Storage.Blob.BlobResultSegment blobResultSegment2 = await AsyncHttpRetryHelper.InvokeAsync<Microsoft.Azure.Storage.Blob.BlobResultSegment>((Func<Task<Microsoft.Azure.Storage.Blob.BlobResultSegment>>) (() => this.container.ListBlobsSegmentedAsync(prefix, useFlatListing, blobListingDetails, maxResults, currentToken, options, operationContext, processor.CancellationToken)), 5, (IAppTraceSource) NoopAppTraceSource.Instance, (Func<Exception, bool>) (e => e is TimeoutException || e.InnerException is TimeoutException), processor.CancellationToken, false, nameof (ListBlobsSegmentedAsync));
        List<CloudBlockBlobWrapper> wrappedBlobs = blobResultSegment2.Results is ICollection<IListBlobItem> results ? new List<CloudBlockBlobWrapper>(results.Count) : new List<CloudBlockBlobWrapper>();
        foreach (IListBlobItem result in blobResultSegment2.Results)
        {
          if (!(result is CloudBlockBlob blockBlob))
            throw new NotImplementedException();
          wrappedBlobs.Add(new CloudBlockBlobWrapper(container, blockBlob));
        }
        blobResultSegment1 = (IBlobResultSegment) new BlobResultSegmentWrapper(blobResultSegment2.ContinuationToken, (IReadOnlyList<IListBlobItem>) wrappedBlobs);
      }
      return blobResultSegment1;
    }

    public ICloudBlockBlob GetBlockBlobReference(string blobName) => (ICloudBlockBlob) new CloudBlockBlobWrapper(this, this.container.GetBlockBlobReference(blobName));

    public async Task<bool> CreateIfNotExistsAsync(
      VssRequestPump.Processor processor,
      BlobContainerPublicAccessType accessType,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      using (new ActivityLogPerfTimer(processor, "BlobStorage"))
      {
        operationContext = processor.CreateStorageContext(operationContext);
        try
        {
          return await this.container.CreateIfNotExistsAsync(accessType, options, operationContext, processor.CancellationToken).ConfigureAwait(false);
        }
        catch (StorageException ex)
        {
          if (ex.RequestInformation.HttpStatusCode == 409)
            return false;
          throw new ExpandedStorageException(ex, this.AccountName);
        }
      }
    }

    public Task<bool> DeleteIfExistsAsync(
      VssRequestPump.Processor processor,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      using (new ActivityLogPerfTimer(processor, "BlobStorage"))
      {
        operationContext = processor.CreateStorageContext(operationContext);
        return this.container.DeleteIfExistsAsync(accessCondition, options, operationContext, processor.CancellationToken);
      }
    }

    public string GetSharedAccessSignature(
      VssRequestPump.Processor processor,
      SharedAccessBlobPolicy policy,
      string policyName)
    {
      using (new ActivityLogPerfTimer(processor, "BlobStorage"))
        return this.container.GetSharedAccessSignature(policy, policyName, new SharedAccessProtocol?(this.SupportsHttps ? SharedAccessProtocol.HttpsOnly : SharedAccessProtocol.HttpsOrHttp), (IPAddressOrRange) null);
    }

    public async Task<BlobContainerPermissions> GetPermissionsAsync(
      VssRequestPump.Processor processor,
      [Optional] OperationContext operationContext)
    {
      BlobContainerPermissions permissionsAsync;
      using (new ActivityLogPerfTimer(processor, "BlobStorage"))
      {
        operationContext = processor.CreateStorageContext(operationContext);
        permissionsAsync = await this.container.GetPermissionsAsync((AccessCondition) null, (BlobRequestOptions) null, operationContext).ConfigureAwait(false);
      }
      return permissionsAsync;
    }

    public async Task SetPermissionsAsync(
      VssRequestPump.Processor processor,
      BlobContainerPermissions permissions,
      [Optional] OperationContext operationContext)
    {
      using (new ActivityLogPerfTimer(processor, "BlobStorage"))
      {
        operationContext = processor.CreateStorageContext(operationContext);
        await this.container.SetPermissionsAsync(permissions, (AccessCondition) null, (BlobRequestOptions) null, operationContext).ConfigureAwait(false);
      }
    }
  }
}
