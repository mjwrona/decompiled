// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.CloudBlobDirectory
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Blob
{
  public class CloudBlobDirectory : IListBlobItem
  {
    private CloudBlobDirectory parent;

    [DoesServiceRequest]
    public virtual IEnumerable<IListBlobItem> ListBlobs(
      bool useFlatBlobListing = false,
      BlobListingDetails blobListingDetails = BlobListingDetails.None,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return this.Container.ListBlobs(this.Prefix, useFlatBlobListing, blobListingDetails, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual BlobResultSegment ListBlobsSegmented(BlobContinuationToken currentToken) => this.ListBlobsSegmented(false, BlobListingDetails.None, new int?(), currentToken, (BlobRequestOptions) null, (OperationContext) null);

    [DoesServiceRequest]
    public virtual BlobResultSegment ListBlobsSegmented(
      bool useFlatBlobListing,
      BlobListingDetails blobListingDetails,
      int? maxResults,
      BlobContinuationToken currentToken,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.Container.ListBlobsSegmented(this.Prefix, useFlatBlobListing, blobListingDetails, maxResults, currentToken, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginListBlobsSegmented(
      BlobContinuationToken currentToken,
      AsyncCallback callback,
      object state)
    {
      return this.BeginListBlobsSegmented(false, BlobListingDetails.None, new int?(), currentToken, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginListBlobsSegmented(
      bool useFlatBlobListing,
      BlobListingDetails blobListingDetails,
      int? maxResults,
      BlobContinuationToken currentToken,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<BlobResultSegment>((Func<CancellationToken, Task<BlobResultSegment>>) (token => this.ListBlobsSegmentedAsync(useFlatBlobListing, blobListingDetails, maxResults, currentToken, options, operationContext, token)), callback, state);
    }

    public virtual BlobResultSegment EndListBlobsSegmented(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<BlobResultSegment>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<BlobResultSegment> ListBlobsSegmentedAsync(
      BlobContinuationToken currentToken)
    {
      return this.Container.ListBlobsSegmentedAsync(this.Prefix, false, BlobListingDetails.None, new int?(), currentToken, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<BlobResultSegment> ListBlobsSegmentedAsync(
      BlobContinuationToken currentToken,
      CancellationToken cancellationToken)
    {
      return this.Container.ListBlobsSegmentedAsync(this.Prefix, false, BlobListingDetails.None, new int?(), currentToken, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<BlobResultSegment> ListBlobsSegmentedAsync(
      bool useFlatBlobListing,
      BlobListingDetails blobListingDetails,
      int? maxResults,
      BlobContinuationToken currentToken,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.ListBlobsSegmentedAsync(useFlatBlobListing, blobListingDetails, maxResults, currentToken, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<BlobResultSegment> ListBlobsSegmentedAsync(
      bool useFlatBlobListing,
      BlobListingDetails blobListingDetails,
      int? maxResults,
      BlobContinuationToken currentToken,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.Container.ListBlobsSegmentedAsync(this.Prefix, useFlatBlobListing, blobListingDetails, maxResults, currentToken, options, operationContext, cancellationToken);
    }

    public CloudBlobDirectory()
    {
    }

    internal CloudBlobDirectory(StorageUri uri, string prefix, CloudBlobContainer container)
    {
      CommonUtility.AssertNotNull(nameof (uri), (object) uri);
      CommonUtility.AssertNotNull(nameof (prefix), (object) prefix);
      CommonUtility.AssertNotNull(nameof (container), (object) container);
      this.ServiceClient = container.ServiceClient;
      this.Container = container;
      this.Prefix = prefix;
      this.StorageUri = uri;
    }

    public CloudBlobClient ServiceClient { get; private set; }

    public Uri Uri => this.StorageUri.PrimaryUri;

    public StorageUri StorageUri { get; private set; }

    public CloudBlobContainer Container { get; private set; }

    public CloudBlobDirectory Parent
    {
      get
      {
        string parentName;
        StorageUri parentAddress;
        if (this.parent == null && NavigationHelper.GetBlobParentNameAndAddress(this.StorageUri, this.ServiceClient.DefaultDelimiter, new bool?(this.ServiceClient.UsePathStyleUris), out parentName, out parentAddress))
          this.parent = new CloudBlobDirectory(parentAddress, parentName, this.Container);
        return this.parent;
      }
    }

    public string Prefix { get; private set; }

    public virtual CloudPageBlob GetPageBlobReference(string blobName) => this.GetPageBlobReference(blobName, new DateTimeOffset?());

    public virtual CloudPageBlob GetPageBlobReference(string blobName, DateTimeOffset? snapshotTime)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (blobName), blobName);
      return new CloudPageBlob(NavigationHelper.AppendPathToUri(this.StorageUri, blobName, this.ServiceClient.DefaultDelimiter), snapshotTime, this.ServiceClient);
    }

    public virtual CloudBlockBlob GetBlockBlobReference(string blobName) => this.GetBlockBlobReference(blobName, new DateTimeOffset?());

    public virtual CloudBlockBlob GetBlockBlobReference(
      string blobName,
      DateTimeOffset? snapshotTime)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (blobName), blobName);
      return new CloudBlockBlob(NavigationHelper.AppendPathToUri(this.StorageUri, blobName, this.ServiceClient.DefaultDelimiter), snapshotTime, this.ServiceClient);
    }

    public virtual CloudAppendBlob GetAppendBlobReference(string blobName) => this.GetAppendBlobReference(blobName, new DateTimeOffset?());

    public virtual CloudAppendBlob GetAppendBlobReference(
      string blobName,
      DateTimeOffset? snapshotTime)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (blobName), blobName);
      return new CloudAppendBlob(NavigationHelper.AppendPathToUri(this.StorageUri, blobName, this.ServiceClient.DefaultDelimiter), snapshotTime, this.ServiceClient);
    }

    public virtual CloudBlob GetBlobReference(string blobName) => this.GetBlobReference(blobName, new DateTimeOffset?());

    public virtual CloudBlob GetBlobReference(string blobName, DateTimeOffset? snapshotTime)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (blobName), blobName);
      return new CloudBlob(NavigationHelper.AppendPathToUri(this.StorageUri, blobName, this.ServiceClient.DefaultDelimiter), snapshotTime, this.ServiceClient);
    }

    public virtual CloudBlobDirectory GetDirectoryReference(string itemName)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (itemName), itemName);
      if (!itemName.EndsWith(this.ServiceClient.DefaultDelimiter, StringComparison.Ordinal))
        itemName += this.ServiceClient.DefaultDelimiter;
      return new CloudBlobDirectory(NavigationHelper.AppendPathToUri(this.StorageUri, itemName, this.ServiceClient.DefaultDelimiter), this.Prefix + itemName, this.Container);
    }
  }
}
