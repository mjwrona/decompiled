// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.InMemoryLogStore
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class InMemoryLogStore : ITestLogStore
  {
    public bool DeleteBlob(IVssRequestContext requestContext, string filePath) => throw new NotImplementedException();

    public Task<string> AcquireLeaseForContainerAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition,
      TimeSpan? leaseTime,
      string proposedLeaseId,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<TimeSpan> BreakLeaseForContainerAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition,
      TimeSpan? breakPeriod,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<bool> CreateContainerIfNotExistsAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public bool CreateContainerIfNotExists(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext)
    {
      throw new NotImplementedException();
    }

    public Task<bool> CreateSharedAccessPolicyForContainerAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition,
      ILogStoreContainerSASPolicy logStoreContainerSASPolicy,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<string> GetSharedAccessPolicyForContainerAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition,
      ILogStoreContainerSASPolicy logStoreContainerSASPolicy,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<string> GetSharedAccessPolicyForBlobAsync(
      IVssRequestContext requestContext,
      string filePath,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition,
      ILogStoreContainerSASPolicy logStoreContainerSASPolicy,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<bool> IsContainerExistsAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public bool RevokeSharedAccessPolicyForContainer(
      IVssRequestContext requestContext,
      string policyName,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition)
    {
      throw new NotImplementedException();
    }

    public Task<ILogStoreBlobResultSegment> ListBlobsAsync(
      IVssRequestContext requestContext,
      string filePathPrefix,
      bool useFlatBlobListing,
      ILogStoreBlobListingDetails blobListingDetails,
      int? maxResults,
      ILogStoreBlobContinuationToken blobContinuationToken,
      ILogStoreOperationContext logStoreOperationContext,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public TimeSpan BreakLeaseForContainer(
      IVssRequestContext requestContext,
      TimeSpan? breakPeriod,
      ILogStoreOperationContext logStoreOperationContext = null,
      IContainerAccessCondition containerAccessCondition = null)
    {
      throw new NotImplementedException();
    }

    public bool DeleteContainer(
      IVssRequestContext requestContext,
      IContainerAccessCondition containerAccessCondition,
      ILogStoreOperationContext logStoreOperationContext)
    {
      throw new NotImplementedException();
    }

    public Task DownloadToStreamAsync(
      IVssRequestContext requestContext,
      string filePath,
      ILogStoreOperationContext logStoreOperationContext,
      Stream targetStream,
      IContainerAccessCondition containerAccessCondition,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<bool> CreateBlobAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      string filePath,
      Stream stream,
      CancellationToken cancellationToken,
      IDictionary<string, string> metaData,
      bool overwrite = false)
    {
      throw new NotImplementedException();
    }

    public Task<IDictionary<string, string>> GetContainerMetaDataAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<bool> IsBlobExists(
      IVssRequestContext requestContext,
      string filePath,
      ILogStoreOperationContext logStoreOperationContext,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public bool IsBlobExists(
      IVssRequestContext requestContext,
      string filePath,
      ILogStoreOperationContext logStoreOperationContext)
    {
      throw new NotImplementedException();
    }

    public Task<ILogStoreBlobResultSegment> ListBlobsInDirectoryAsync(
      IVssRequestContext requestContext,
      string relativeAddress,
      bool useFlatBlobListing,
      ILogStoreBlobListingDetails blobListingDetails,
      int? maxResults,
      ILogStoreBlobContinuationToken blobContinuationToken,
      ILogStoreOperationContext logStoreOperationContext,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<bool> DeleteBlobAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      string filePath,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public void DownloadToStream(
      IVssRequestContext requestContext,
      string filePath,
      ILogStoreOperationContext logStoreOperationContext,
      Stream targetStream,
      IContainerAccessCondition containerAccessCondition)
    {
      throw new NotImplementedException();
    }

    public ILogStoreBlobResultSegment ListBlobs(
      IVssRequestContext requestContext,
      string filePathPrefix,
      bool useFlatBlobListing,
      ILogStoreBlobListingDetails blobListingDetails,
      int? maxResults,
      ILogStoreBlobContinuationToken blobContinuationToken,
      ILogStoreOperationContext logStoreOperationContext)
    {
      throw new NotImplementedException();
    }

    public bool CreateBlob(
      IVssRequestContext requestContext,
      string filePath,
      Stream stream,
      IDictionary<string, string> metaData,
      bool overwrite = false)
    {
      throw new NotImplementedException();
    }

    public Task DownloadRangeToStreamAsync(
      IVssRequestContext requestContext,
      string filePath,
      int offset,
      int length,
      ILogStoreOperationContext logStoreOperationContext,
      Stream targetStream,
      IContainerAccessCondition containerAccessCondition,
      CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
  }
}
