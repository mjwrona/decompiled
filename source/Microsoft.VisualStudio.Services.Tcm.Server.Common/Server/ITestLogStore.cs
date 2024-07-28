// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITestLogStore
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
  [CLSCompliant(false)]
  public interface ITestLogStore
  {
    Task<bool> CreateContainerIfNotExistsAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      CancellationToken cancellationToken);

    bool CreateContainerIfNotExists(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext);

    Task<bool> IsContainerExistsAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      CancellationToken cancellationToken);

    Task<bool> CreateSharedAccessPolicyForContainerAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition,
      ILogStoreContainerSASPolicy logStoreContainerSASPolicy,
      CancellationToken cancellationToken);

    Task<string> GetSharedAccessPolicyForContainerAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition,
      ILogStoreContainerSASPolicy logStoreContainerSASPolicy,
      CancellationToken cancellationToken);

    Task<string> GetSharedAccessPolicyForBlobAsync(
      IVssRequestContext requestContext,
      string filePath,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition,
      ILogStoreContainerSASPolicy logStoreContainerSASPolicy,
      CancellationToken cancellationToken);

    Task<string> AcquireLeaseForContainerAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition,
      TimeSpan? leaseTime,
      string proposedLeaseId,
      CancellationToken cancellationToken);

    Task<TimeSpan> BreakLeaseForContainerAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition,
      TimeSpan? breakPeriod,
      CancellationToken cancellationToken);

    TimeSpan BreakLeaseForContainer(
      IVssRequestContext requestContext,
      TimeSpan? breakPeriod,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition);

    bool RevokeSharedAccessPolicyForContainer(
      IVssRequestContext requestContext,
      string policyName,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition);

    Task<ILogStoreBlobResultSegment> ListBlobsAsync(
      IVssRequestContext requestContext,
      string filePathPrefix,
      bool useFlatBlobListing,
      ILogStoreBlobListingDetails blobListingDetails,
      int? maxResults,
      ILogStoreBlobContinuationToken blobContinuationToken,
      ILogStoreOperationContext logStoreOperationContext,
      CancellationToken cancellationToken);

    ILogStoreBlobResultSegment ListBlobs(
      IVssRequestContext requestContext,
      string filePathPrefix,
      bool useFlatBlobListing,
      ILogStoreBlobListingDetails blobListingDetails,
      int? maxResults,
      ILogStoreBlobContinuationToken blobContinuationToken,
      ILogStoreOperationContext logStoreOperationContext);

    bool CreateBlob(
      IVssRequestContext requestContext,
      string filePath,
      Stream stream,
      IDictionary<string, string> metaData,
      bool overwrite = false);

    Task<ILogStoreBlobResultSegment> ListBlobsInDirectoryAsync(
      IVssRequestContext requestContext,
      string relativeAddress,
      bool useFlatBlobListing,
      ILogStoreBlobListingDetails blobListingDetails,
      int? maxResults,
      ILogStoreBlobContinuationToken blobContinuationToken,
      ILogStoreOperationContext logStoreOperationContext,
      CancellationToken cancellationToken);

    bool DeleteContainer(
      IVssRequestContext requestContext,
      IContainerAccessCondition containerAccessCondition,
      ILogStoreOperationContext logStoreOperationContext);

    Task DownloadToStreamAsync(
      IVssRequestContext requestContext,
      string filePath,
      ILogStoreOperationContext logStoreOperationContext,
      Stream targetStream,
      IContainerAccessCondition containerAccessCondition,
      CancellationToken cancellationToken);

    void DownloadToStream(
      IVssRequestContext requestContext,
      string filePath,
      ILogStoreOperationContext logStoreOperationContext,
      Stream targetStream,
      IContainerAccessCondition containerAccessCondition);

    Task<bool> CreateBlobAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      string filePath,
      Stream stream,
      CancellationToken cancellationToken,
      IDictionary<string, string> metaData,
      bool overwrite = false);

    Task<bool> DeleteBlobAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      string filePath,
      CancellationToken cancellationToken);

    bool DeleteBlob(IVssRequestContext requestContext, string filePath);

    Task<IDictionary<string, string>> GetContainerMetaDataAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      CancellationToken cancellationToken);

    Task<bool> IsBlobExists(
      IVssRequestContext requestContext,
      string filePath,
      ILogStoreOperationContext logStoreOperationContext,
      CancellationToken cancellationToken);

    bool IsBlobExists(
      IVssRequestContext requestContext,
      string filePath,
      ILogStoreOperationContext logStoreOperationContext);

    Task DownloadRangeToStreamAsync(
      IVssRequestContext requestContext,
      string filePath,
      int offset,
      int length,
      ILogStoreOperationContext logStoreOperationContext,
      Stream targetStream,
      IContainerAccessCondition containerAccessCondition,
      CancellationToken cancellationToken);
  }
}
