// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.AzureBlobStore
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Blob.Protocol;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal abstract class AzureBlobStore : ITestLogStore
  {
    private ILogStoreConnectionEndpoint _logStoreConnectionEndpoint;
    private ILogStoreContainerAccessPolicy _containerAccessPolicy;
    private ILogStoreRequestOption _logStoreRequestOption;
    private CloudBlobContainer _cloudBlobContainer;
    private string _containerName;

    public AzureBlobStore(
      IVssRequestContext requestContext,
      ILogStoreConnectionEndpoint logStoreConnectionEndpoint,
      string containerName,
      ILogStoreContainerAccessPolicy containerAccessPolicy,
      ILogStoreRequestOption logStoreRequestOption)
    {
      this._logStoreConnectionEndpoint = logStoreConnectionEndpoint;
      this._containerName = LogStoreHelper.SanatizeContainerName(requestContext, containerName);
      this._containerAccessPolicy = containerAccessPolicy;
      this._logStoreRequestOption = logStoreRequestOption;
      this._initializeContainerReference(requestContext);
    }

    public virtual async Task<bool> CreateContainerIfNotExistsAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      try
      {
        int num = await this._cloudBlobContainer.CreateIfNotExistsAsync(this._containerAccessPolicy.GetBlobContainerPublicAccessType(), this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext(), cancellationToken).ConfigureAwait(false) ? 1 : 0;
        return true;
      }
      catch (StorageException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "CreateContainerIfNotExistsAsync - Unable to create the container {0}. Exception Hit: {1}, StatusMessage: {2}", (object) this._containerName, (object) ex.Message, (object) ex.RequestInformation.HttpStatusMessage);
      }
      return false;
    }

    public bool CreateContainerIfNotExists(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      try
      {
        this._cloudBlobContainer.CreateIfNotExists(this._containerAccessPolicy.GetBlobContainerPublicAccessType(), this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext());
        return true;
      }
      catch (StorageException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "CreateContainerIfNotExists - Unable to create the container {0}. Exception Hit: {1}, StatusMessage: {2}", (object) this._containerName, (object) ex.Message, (object) ex.RequestInformation.HttpStatusMessage);
      }
      return false;
    }

    public async Task<bool> CreateSharedAccessPolicyForContainerAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition,
      ILogStoreContainerSASPolicy logStoreContainerSASPolicy,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      ArgumentUtility.CheckForNull<ILogStoreContainerSASPolicy>(logStoreContainerSASPolicy, nameof (logStoreContainerSASPolicy), "LogStorage");
      bool isSuccess = false;
      if (await this.IsContainerExistsAsync(requestContext, logStoreOperationContext, requestContext.CancellationToken).ConfigureAwait(false))
      {
        try
        {
          BlobContainerPermissions permissionsAsync = await this._cloudBlobContainer.GetPermissionsAsync();
          if (!this._isPolicyAlreadyExists(logStoreContainerSASPolicy.GetSharedAccessBlobPolicyName(), permissionsAsync))
          {
            permissionsAsync.SharedAccessPolicies.Add(logStoreContainerSASPolicy.GetSharedAccessBlobPolicyName(), logStoreContainerSASPolicy.GetSharedAccessBlobPolicy());
            await this._cloudBlobContainer.SetPermissionsAsync(permissionsAsync, containerAccessCondition.GetAccessCondition(), this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext(), cancellationToken).ConfigureAwait(false);
          }
          else if (this._tryRenewExpiredPolicy(requestContext, logStoreContainerSASPolicy, permissionsAsync))
            await this._cloudBlobContainer.SetPermissionsAsync(permissionsAsync, containerAccessCondition.GetAccessCondition(), this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext(), cancellationToken).ConfigureAwait(false);
          isSuccess = true;
        }
        catch (StorageException ex)
        {
          requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "CreateSharedAccessPolicyForContainerAsync - Unable to create SAS policy for the container {0}. Exception Hit: {1}, StatusMessage: {2}", (object) this._containerName, (object) ex.Message, (object) ex.RequestInformation.HttpStatusMessage);
          isSuccess = false;
        }
      }
      return isSuccess;
    }

    public async Task<string> GetSharedAccessPolicyForContainerAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition,
      ILogStoreContainerSASPolicy logStoreContainerSASPolicy,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      string empty = string.Empty;
      string sharedAccessSignature;
      try
      {
        if (!await this.CreateSharedAccessPolicyForContainerAsync(requestContext, logStoreOperationContext, containerAccessCondition, logStoreContainerSASPolicy, cancellationToken).ConfigureAwait(false))
        {
          requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "GetSharedAccessPolicyForContainerAsync - {0} is not a valid policy name", (object) logStoreContainerSASPolicy.GetSharedAccessBlobPolicyName());
          return (string) null;
        }
        sharedAccessSignature = this._cloudBlobContainer.GetSharedAccessSignature((SharedAccessBlobPolicy) null, logStoreContainerSASPolicy.GetSharedAccessBlobPolicyName(), new SharedAccessProtocol?(logStoreContainerSASPolicy.GetSASProtocol()), (IPAddressOrRange) null);
      }
      catch (StorageException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "GetSharedAccessPolicyForContainerAsync - Unable to get SAS Uri for the container {0}. Exception Hit: {1}, StatusMessage: {2}", (object) this._containerName, (object) ex.Message, (object) ex.RequestInformation.HttpStatusMessage);
        return string.Empty;
      }
      return this._cloudBlobContainer.Uri?.ToString() + sharedAccessSignature;
    }

    public async Task<string> GetSharedAccessPolicyForBlobAsync(
      IVssRequestContext requestContext,
      string filePath,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition,
      ILogStoreContainerSASPolicy logStoreContainerSASPolicy,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      string empty = string.Empty;
      try
      {
        ConfiguredTaskAwaitable<bool> configuredTaskAwaitable = this.CreateSharedAccessPolicyForContainerAsync(requestContext, logStoreOperationContext, containerAccessCondition, logStoreContainerSASPolicy, cancellationToken).ConfigureAwait(false);
        if (!await configuredTaskAwaitable)
        {
          requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "GetSharedAccessPolicyForBlobAsync - {0} is not a valid policy name", (object) logStoreContainerSASPolicy.GetSharedAccessBlobPolicyName());
          return (string) null;
        }
        CloudBlob blob = this._cloudBlobContainer.GetBlobReference(filePath);
        configuredTaskAwaitable = blob.ExistsAsync().ConfigureAwait(false);
        if (!await configuredTaskAwaitable)
          return (string) null;
        string sharedAccessSignature = blob.GetSharedAccessSignature((SharedAccessBlobPolicy) null, new LogStoreSharedAccessBlobHeaders((string) null, blob.Properties.ContentType).GetSharedAccessBlobHeaders(), logStoreContainerSASPolicy.GetSharedAccessBlobPolicyName(), new SharedAccessProtocol?(logStoreContainerSASPolicy.GetSASProtocol()), (IPAddressOrRange) null);
        return blob.Uri?.ToString() + sharedAccessSignature;
      }
      catch (StorageException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "GetSharedAccessPolicyForBlobAsync - Unable to get SAS Uri for the blob {0} inside container {1}. Exception Hit: {2}, StatusMessage: {3}", (object) filePath, (object) this._containerName, (object) ex.Message, (object) ex.RequestInformation.HttpStatusMessage);
        return string.Empty;
      }
    }

    public bool RevokeSharedAccessPolicyForContainer(
      IVssRequestContext requestContext,
      string policyName,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition)
    {
      ArgumentUtility.CheckForNull<string>(policyName, nameof (policyName), "LogStorage");
      ArgumentUtility.CheckForNull<ILogStoreOperationContext>(logStoreOperationContext, nameof (logStoreOperationContext), "LogStorage");
      ArgumentUtility.CheckForNull<IContainerAccessCondition>(containerAccessCondition, nameof (containerAccessCondition), "LogStorage");
      if (this.IsContainerExistsAsync(requestContext, logStoreOperationContext, requestContext.CancellationToken).Result)
      {
        BlobContainerPermissions result = this.GetBlobContainerPermissions(requestContext).Result;
        if (result != null && this._isPolicyAlreadyExists(policyName, result))
        {
          result.SharedAccessPolicies.Remove(policyName);
          this.SetBlobContainerPermissions(requestContext, result, logStoreOperationContext, containerAccessCondition, requestContext.CancellationToken);
          return true;
        }
      }
      return false;
    }

    public async Task<string> AcquireLeaseForContainerAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition,
      TimeSpan? leaseTime,
      string proposedLeaseId,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      if (!this.IsContainerExistsAsync(requestContext, logStoreOperationContext, requestContext.CancellationToken).Result)
        return (string) null;
      try
      {
        return await this._cloudBlobContainer.AcquireLeaseAsync(leaseTime, proposedLeaseId, containerAccessCondition.GetAccessCondition(), this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext(), cancellationToken).ConfigureAwait(false);
      }
      catch (StorageException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "GetSharedAccessPolicyForContainerAsync - Unable to acquire the lease container {0}. Exception Hit: {1}, StatusMessage: {2}", (object) this._containerName, (object) ex.Message, (object) ex.RequestInformation.HttpStatusMessage);
        return (string) null;
      }
    }

    public async Task<TimeSpan> BreakLeaseForContainerAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition,
      TimeSpan? breakPeriod,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      if (!this.IsContainerExistsAsync(requestContext, logStoreOperationContext, requestContext.CancellationToken).Result)
        return TimeSpan.Zero;
      try
      {
        return await this._cloudBlobContainer.BreakLeaseAsync(breakPeriod, containerAccessCondition.GetAccessCondition(), this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext(), cancellationToken).ConfigureAwait(false);
      }
      catch (StorageException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "BreakLeaseForContainerAsync - Unable to acquire the lease container {0}. Exception Hit: {1}, StatusMessage: {2}", (object) this._containerName, (object) ex.Message, (object) ex.RequestInformation.HttpStatusMessage);
        return TimeSpan.Zero;
      }
    }

    public TimeSpan BreakLeaseForContainer(
      IVssRequestContext requestContext,
      TimeSpan? breakPeriod,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      try
      {
        return this._cloudBlobContainer.BreakLease(breakPeriod, containerAccessCondition.GetAccessCondition(), this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext());
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 404)
          throw new TestObjectNotFoundException("Container not found", ObjectTypes.LogStoreContainer);
        if (ex.RequestInformation.ExtendedErrorInformation.ErrorCode == BlobErrorCodeStrings.LeaseAlreadyBroken || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == BlobErrorCodeStrings.LeaseNotPresentWithLeaseOperation)
          return TimeSpan.Zero;
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "BreakLease - Unable to break the lease on container {0}. Exception Hit: {1}, StatusMessage: {2}", (object) this._containerName, (object) ex.Message, (object) ex.RequestInformation.HttpStatusMessage);
        return TimeSpan.Zero;
      }
    }

    public async Task<bool> IsContainerExistsAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      try
      {
        return await this._cloudBlobContainer.ExistsAsync(this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext(), cancellationToken).ConfigureAwait(false);
      }
      catch (StorageException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "IsContainerExistsAsync - Unable to find the container {0}. Exception Hit: {1}, StatusMessage: {2}", (object) this._containerName, (object) ex.Message, (object) ex.RequestInformation.HttpStatusMessage);
        return false;
      }
    }

    public bool IsContainerExists(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      try
      {
        return this._cloudBlobContainer.Exists(this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext());
      }
      catch (StorageException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "IsContainerExists - Unable to find the container {0}. Exception Hit: {1}, StatusMessage: {2}", (object) this._containerName, (object) ex.Message, (object) ex.RequestInformation.HttpStatusMessage);
        return false;
      }
    }

    public async Task<ILogStoreBlobResultSegment> ListBlobsAsync(
      IVssRequestContext requestContext,
      string filePathPrefix,
      bool useFlatBlobListing,
      ILogStoreBlobListingDetails blobListingDetails,
      int? maxResults,
      ILogStoreBlobContinuationToken blobContinuationToken,
      ILogStoreOperationContext logStoreOperationContext,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      try
      {
        BlobResultSegment blobResultSegment;
        if (await this.IsContainerExistsAsync(requestContext, logStoreOperationContext, cancellationToken).ConfigureAwait(false))
          blobResultSegment = await this._cloudBlobContainer.ListBlobsSegmentedAsync(filePathPrefix, useFlatBlobListing, blobListingDetails.GetBlobListingDetails(), maxResults, blobContinuationToken.GetBlobContinuationToken(), this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext(), cancellationToken).ConfigureAwait(false);
        return (ILogStoreBlobResultSegment) new LogStoreBlobResultSegment(blobResultSegment, requestContext);
      }
      catch (StorageException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "ListBlobs: Unable to find the container {0}. Exception Hit: {1}, StatusMessage: {2}", (object) this._containerName, (object) ex.Message, (object) ex.RequestInformation.HttpStatusMessage);
        return (ILogStoreBlobResultSegment) null;
      }
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
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      try
      {
        int num = this.IsContainerExists(requestContext, logStoreOperationContext) ? 1 : 0;
        BlobResultSegment blobResultSegment = (BlobResultSegment) null;
        if (num != 0)
          blobResultSegment = this._cloudBlobContainer.ListBlobsSegmented(filePathPrefix, useFlatBlobListing, blobListingDetails.GetBlobListingDetails(), maxResults, blobContinuationToken.GetBlobContinuationToken(), this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext());
        return (ILogStoreBlobResultSegment) new LogStoreBlobResultSegment(blobResultSegment, requestContext);
      }
      catch (StorageException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "ListBlobs: Unable to find the container {0}. Exception Hit: {1}, StatusMessage: {2}", (object) this._containerName, (object) ex.Message, (object) ex.RequestInformation.HttpStatusMessage);
        return (ILogStoreBlobResultSegment) null;
      }
    }

    public async Task<ILogStoreBlobResultSegment> ListBlobsInDirectoryAsync(
      IVssRequestContext requestContext,
      string relativeAddress,
      bool useFlatBlobListing,
      ILogStoreBlobListingDetails blobListingDetails,
      int? maxResults,
      ILogStoreBlobContinuationToken blobContinuationToken,
      ILogStoreOperationContext logStoreOperationContext,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      try
      {
        BlobResultSegment blobResultSegment;
        if (await this.IsContainerExistsAsync(requestContext, logStoreOperationContext, cancellationToken).ConfigureAwait(false))
          blobResultSegment = await this._cloudBlobContainer.GetDirectoryReference(relativeAddress).ListBlobsSegmentedAsync(useFlatBlobListing, blobListingDetails.GetBlobListingDetails(), maxResults, blobContinuationToken.GetBlobContinuationToken(), this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext(), cancellationToken).ConfigureAwait(false);
        return (ILogStoreBlobResultSegment) new LogStoreBlobResultSegment(blobResultSegment, requestContext);
      }
      catch (StorageException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "ListBlobsInDirectoryAsync: Unable to find the container {0}. Exception Hit: {1}, StatusMessage: {2}", (object) this._containerName, (object) ex.Message, (object) ex.RequestInformation.HttpStatusMessage);
        return (ILogStoreBlobResultSegment) null;
      }
    }

    public virtual bool DeleteContainer(
      IVssRequestContext requestContext,
      IContainerAccessCondition containerAccessCondition,
      ILogStoreOperationContext logStoreOperationContext)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      try
      {
        this._cloudBlobContainer.Delete(containerAccessCondition.GetAccessCondition(), this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext());
        return true;
      }
      catch (StorageException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "DeleteContainer: Unable to delete the container {0}. Exception Hit: {1}, StatusMessage: {2}", (object) this._containerName, (object) ex.Message, (object) ex.RequestInformation.HttpStatusMessage);
        return false;
      }
    }

    public async Task DownloadToStreamAsync(
      IVssRequestContext requestContext,
      string filePath,
      ILogStoreOperationContext logStoreOperationContext,
      Stream targetStream,
      IContainerAccessCondition containerAccessCondition,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      try
      {
        await this._cloudBlobContainer.GetBlobReference(filePath).DownloadToStreamAsync(targetStream, containerAccessCondition.GetAccessCondition(), this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext(), cancellationToken).ConfigureAwait(false);
      }
      catch (StorageException ex)
      {
        requestContext.Trace(1015682, TraceLevel.Error, "TestManagement", "LogStorage", "DownloadToStreamAsync: Filed to download " + filePath + ". Exception Hit: " + ex.Message + ", StatusMessage: " + ex.RequestInformation.HttpStatusMessage);
      }
    }

    public async Task DownloadRangeToStreamAsync(
      IVssRequestContext requestContext,
      string filePath,
      int offset,
      int length,
      ILogStoreOperationContext logStoreOperationContext,
      Stream targetStream,
      IContainerAccessCondition containerAccessCondition,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      try
      {
        await this._cloudBlobContainer.GetBlobReference(filePath).DownloadRangeToStreamAsync(targetStream, new long?((long) offset), new long?((long) length), containerAccessCondition.GetAccessCondition(), this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext(), cancellationToken).ConfigureAwait(false);
      }
      catch (StorageException ex)
      {
        requestContext.Trace(1015682, TraceLevel.Error, "TestManagement", "LogStorage", "DownloadRangeToStreamAsync: Filed to download " + filePath + ". Exception Hit: " + ex.Message + ", StatusMessage: " + ex.RequestInformation.HttpStatusMessage);
      }
    }

    public void DownloadToStream(
      IVssRequestContext requestContext,
      string filePath,
      ILogStoreOperationContext logStoreOperationContext,
      Stream targetStream,
      IContainerAccessCondition containerAccessCondition)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      ArgumentUtility.CheckStringForNullOrEmpty(filePath, nameof (filePath));
      try
      {
        this._cloudBlobContainer.GetBlobReference(filePath).DownloadToStream(targetStream, containerAccessCondition.GetAccessCondition(), this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext());
      }
      catch (StorageException ex)
      {
        requestContext.Trace(1015682, TraceLevel.Error, "TestManagement", "LogStorage", "DownloadToStreamAsync: Filed to download " + filePath + ". Exception Hit: " + ex.Message + ", StatusMessage: " + ex.RequestInformation.HttpStatusMessage);
      }
    }

    public virtual async Task<bool> CreateBlobAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      string filePath,
      Stream stream,
      CancellationToken cancellationToken,
      IDictionary<string, string> metaData = null,
      bool overwrite = false)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      try
      {
        CloudBlockBlob blob = this._cloudBlobContainer.GetBlockBlobReference(filePath);
        if (await blob.ExistsAsync().ConfigureAwait(false) && !overwrite)
          return false;
        await blob.UploadFromStreamAsync(stream, (AccessCondition) null, this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext(), cancellationToken).ConfigureAwait(false);
        if (metaData != null)
        {
          foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) metaData)
            blob.Metadata[keyValuePair.Key] = keyValuePair.Value;
          await blob.SetMetadataAsync().ConfigureAwait(false);
        }
        return true;
      }
      catch (StorageException ex)
      {
        requestContext.Trace(1015682, TraceLevel.Error, "TestManagement", "LogStorage", "CreateBlobAsync: Failed to create blob " + filePath + ". Exception Hit: " + ex.Message + ", StatusMessage: " + ex.RequestInformation.HttpStatusMessage);
        return false;
      }
      finally
      {
        stream.Dispose();
      }
    }

    public virtual bool CreateBlob(
      IVssRequestContext requestContext,
      string filePath,
      Stream stream,
      IDictionary<string, string> metaData = null,
      bool overwrite = false)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      try
      {
        CloudBlockBlob blockBlobReference = this._cloudBlobContainer.GetBlockBlobReference(filePath);
        if (blockBlobReference.Exists((BlobRequestOptions) null, (OperationContext) null) && !overwrite)
        {
          requestContext.Trace(1015114, TraceLevel.Error, "TestManagement", "LogStorage", "CreateBlob  failed as container already exists for file path: {0}", (object) filePath);
          return false;
        }
        blockBlobReference.UploadFromStream(stream, (AccessCondition) null, this._logStoreRequestOption.GetBlobRequestOptions(), (OperationContext) null);
        if (metaData != null)
        {
          foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) metaData)
            blockBlobReference.Metadata[keyValuePair.Key] = keyValuePair.Value;
          blockBlobReference.SetMetadata((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null);
        }
        return true;
      }
      catch (StorageException ex)
      {
        requestContext.Trace(1015682, TraceLevel.Error, "TestManagement", "LogStorage", "CreateBlob: Failed to create blob " + filePath + ". Exception Hit: " + ex.Message + ", StatusMessage: " + ex.RequestInformation.HttpStatusMessage);
        return false;
      }
      finally
      {
        stream.Dispose();
      }
    }

    public virtual async Task<bool> DeleteBlobAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      string filePath,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      try
      {
        return await this._cloudBlobContainer.GetBlockBlobReference(filePath).DeleteIfExistsAsync(cancellationToken).ConfigureAwait(false);
      }
      catch (StorageException ex)
      {
        requestContext.Trace(1015682, TraceLevel.Error, "TestManagement", "LogStorage", "DeleteBlobAsync: Failed to delete blob " + filePath + ". Exception Hit: " + ex.Message + ", StatusMessage: " + ex.RequestInformation.HttpStatusMessage);
        return false;
      }
    }

    public virtual bool DeleteBlob(IVssRequestContext requestContext, string filePath)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      try
      {
        return this._cloudBlobContainer.GetBlockBlobReference(filePath).DeleteIfExists(DeleteSnapshotsOption.None, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null);
      }
      catch (StorageException ex)
      {
        requestContext.Trace(1015682, TraceLevel.Error, "TestManagement", "LogStorage", "DeleteBlob: Failed to delete blob " + filePath + ". Exception Hit: " + ex.Message + ", StatusMessage: " + ex.RequestInformation.HttpStatusMessage);
        return false;
      }
    }

    public async Task<bool> IsBlobExists(
      IVssRequestContext requestContext,
      string filePath,
      ILogStoreOperationContext logStoreOperationContext,
      CancellationToken cancellationToken)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      try
      {
        return await this._cloudBlobContainer.GetBlockBlobReference(filePath).ExistsAsync(this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext(), cancellationToken).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        requestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "GetContainerMetaDataAsync - Unable to check the blob exists {0} in container {1}. Exception Hit: {2}", (object) this._containerName, (object) filePath, (object) ex.Message);
      }
      return false;
    }

    public bool IsBlobExists(
      IVssRequestContext requestContext,
      string filePath,
      ILogStoreOperationContext logStoreOperationContext)
    {
      ArgumentUtility.CheckForNull<CloudBlobContainer>(this._cloudBlobContainer, "_cloudBlobContainer", "LogStorage");
      try
      {
        return this._cloudBlobContainer.GetBlockBlobReference(filePath).Exists(this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext());
      }
      catch (Exception ex)
      {
        requestContext.Trace(1015971, TraceLevel.Error, "TestManagement", "LogStorage", "IsBlobExists - Unable to check the blob {0} exists in container {1}. Exception Hit: {2}", (object) filePath, (object) this._containerName, (object) ex.Message);
      }
      requestContext.Trace(1015971, TraceLevel.Warning, "TestManagement", "LogStorage", "IsBlobExists - blob {0} does not exist in container {1}.", (object) filePath, (object) this._containerName);
      return false;
    }

    public async Task<IDictionary<string, string>> GetContainerMetaDataAsync(
      IVssRequestContext requestContext,
      ILogStoreOperationContext logStoreOperationContext,
      CancellationToken cancellationToken)
    {
      Dictionary<string, string> metaData = new Dictionary<string, string>();
      try
      {
        if (await this.IsContainerExistsAsync(requestContext, logStoreOperationContext, cancellationToken).ConfigureAwait(false))
        {
          await this._cloudBlobContainer.FetchAttributesAsync();
          return this._cloudBlobContainer.Metadata != null ? this._cloudBlobContainer.Metadata : (IDictionary<string, string>) metaData;
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "GetContainerMetaDataAsync - Unable to fetch the metaData {0}. Exception Hit: {1}", (object) this._containerName, (object) ex.Message);
      }
      return (IDictionary<string, string>) metaData;
    }

    private async Task<BlobContainerPermissions> GetBlobContainerPermissions(
      IVssRequestContext requestContext)
    {
      BlobContainerPermissions permissions = (BlobContainerPermissions) null;
      try
      {
        permissions = await this._cloudBlobContainer.GetPermissionsAsync();
      }
      catch (Exception ex)
      {
        requestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "GetBlobContainerPermissions - Unable to get permission for container {0}. Exception Hit: {1}", (object) this._containerName, (object) ex.Message);
      }
      BlobContainerPermissions containerPermissions = permissions;
      permissions = (BlobContainerPermissions) null;
      return containerPermissions;
    }

    private async void SetBlobContainerPermissions(
      IVssRequestContext requestContext,
      BlobContainerPermissions permissions,
      ILogStoreOperationContext logStoreOperationContext,
      IContainerAccessCondition containerAccessCondition,
      CancellationToken cancellationToken)
    {
      try
      {
        await this._cloudBlobContainer.SetPermissionsAsync(permissions, containerAccessCondition.GetAccessCondition(), this._logStoreRequestOption.GetBlobRequestOptions(), logStoreOperationContext.GetOperationContext(), cancellationToken).ConfigureAwait(false);
      }
      catch (StorageException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "SetPermissionsAsync - Exception Hit: {0}, StatusMessage: {1}", (object) ex.Message, (object) ex.RequestInformation.HttpStatusMessage);
      }
    }

    private void _initializeContainerReference(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<string>(this._containerName, "_containerName", "LogStorage");
      ArgumentUtility.CheckForNull<CloudBlobClient>(this._logStoreConnectionEndpoint.GetCloudBlobClient(), "_logStoreConnectionEndpoint.GetCloudBlobClient()", "LogStorage");
      try
      {
        this._cloudBlobContainer = this._logStoreConnectionEndpoint.GetCloudBlobClient().GetContainerReference(this._containerName);
      }
      catch (StorageException ex)
      {
        requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", "_initializeContainerReference - Unable to create the blob client. Exception Hit: {0}, StatusMessage: {1}", (object) ex.Message, (object) ex.RequestInformation.HttpStatusMessage);
      }
    }

    private bool _isPolicyAlreadyExists(string policyName, BlobContainerPermissions permissions)
    {
      ArgumentUtility.CheckForNull<BlobContainerPermissions>(permissions, nameof (permissions), "LogStorage");
      ArgumentUtility.CheckStringForNullOrEmpty(policyName, nameof (policyName), "LogStorage");
      bool flag = false;
      SharedAccessBlobPolicies sharedAccessPolicies = permissions.SharedAccessPolicies;
      if (sharedAccessPolicies != null && sharedAccessPolicies.Keys != null)
        flag = sharedAccessPolicies.Keys.Contains(policyName);
      return flag;
    }

    private bool _tryRenewExpiredPolicy(
      IVssRequestContext requestContext,
      ILogStoreContainerSASPolicy logStoreContainerSASPolicy,
      BlobContainerPermissions permissions)
    {
      ArgumentUtility.CheckForNull<ILogStoreContainerSASPolicy>(logStoreContainerSASPolicy, nameof (logStoreContainerSASPolicy), "LogStorage");
      bool flag = false;
      SharedAccessBlobPolicies sharedAccessPolicies = permissions.SharedAccessPolicies;
      if (sharedAccessPolicies != null && sharedAccessPolicies.Keys != null && sharedAccessPolicies.Keys.Contains(logStoreContainerSASPolicy.GetSharedAccessBlobPolicyName()))
      {
        SharedAccessBlobPolicy accessBlobPolicy1 = sharedAccessPolicies[logStoreContainerSASPolicy.GetSharedAccessBlobPolicyName()];
        DateTimeOffset? nullable1 = accessBlobPolicy1.SharedAccessExpiryTime;
        if (DateTime.Compare(nullable1.Value.UtcDateTime, DateTime.UtcNow.AddMinutes(10.0)) <= 0)
        {
          flag = true;
          SharedAccessBlobPolicy accessBlobPolicy2 = accessBlobPolicy1;
          nullable1 = logStoreContainerSASPolicy.GetSASExpiryTime();
          DateTimeOffset? nullable2 = new DateTimeOffset?((DateTimeOffset) nullable1.Value.UtcDateTime);
          accessBlobPolicy2.SharedAccessExpiryTime = nullable2;
          requestContext.Trace(1015678, TraceLevel.Verbose, "TestManagement", "LogStorage", "_tryRenewExpiredPolicy - Renewed expired policy {0}", (object) logStoreContainerSASPolicy.GetSharedAccessBlobPolicyName());
        }
      }
      return flag;
    }
  }
}
