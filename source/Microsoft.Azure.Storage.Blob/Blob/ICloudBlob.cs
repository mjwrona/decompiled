// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.ICloudBlob
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Blob
{
  public interface ICloudBlob : IListBlobItem
  {
    Stream OpenRead(
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginOpenRead(AsyncCallback callback, object state);

    ICancellableAsyncResult BeginOpenRead(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    Stream EndOpenRead(IAsyncResult asyncResult);

    Task<Stream> OpenReadAsync();

    Task<Stream> OpenReadAsync(CancellationToken cancellationToken);

    Task<Stream> OpenReadAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task<Stream> OpenReadAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    void UploadFromStream(
      Stream source,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    void UploadFromStream(
      Stream source,
      long length,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      AsyncCallback callback,
      object state);

    ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      long length,
      AsyncCallback callback,
      object state);

    ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    void EndUploadFromStream(IAsyncResult asyncResult);

    Task UploadFromStreamAsync(Stream source);

    Task UploadFromStreamAsync(Stream source, CancellationToken cancellationToken);

    Task UploadFromStreamAsync(
      Stream source,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task UploadFromStreamAsync(
      Stream source,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    Task UploadFromStreamAsync(Stream source, long length);

    Task UploadFromStreamAsync(Stream source, long length, CancellationToken cancellationToken);

    Task UploadFromStreamAsync(
      Stream source,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task UploadFromStreamAsync(
      Stream source,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    void UploadFromFile(
      string path,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginUploadFromFile(string path, AsyncCallback callback, object state);

    ICancellableAsyncResult BeginUploadFromFile(
      string path,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    void EndUploadFromFile(IAsyncResult asyncResult);

    Task UploadFromFileAsync(string path);

    Task UploadFromFileAsync(string path, CancellationToken cancellationToken);

    Task UploadFromFileAsync(
      string path,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task UploadFromFileAsync(
      string path,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    void UploadFromByteArray(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginUploadFromByteArray(
      byte[] buffer,
      int index,
      int count,
      AsyncCallback callback,
      object state);

    ICancellableAsyncResult BeginUploadFromByteArray(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    void EndUploadFromByteArray(IAsyncResult asyncResult);

    Task UploadFromByteArrayAsync(byte[] buffer, int index, int count);

    Task UploadFromByteArrayAsync(
      byte[] buffer,
      int index,
      int count,
      CancellationToken cancellationToken);

    Task UploadFromByteArrayAsync(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task UploadFromByteArrayAsync(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    void DownloadToStream(
      Stream target,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginDownloadToStream(
      Stream target,
      AsyncCallback callback,
      object state);

    ICancellableAsyncResult BeginDownloadToStream(
      Stream target,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    void EndDownloadToStream(IAsyncResult asyncResult);

    Task DownloadToStreamAsync(Stream target);

    Task DownloadToStreamAsync(Stream target, CancellationToken cancellationToken);

    Task DownloadToStreamAsync(
      Stream target,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task DownloadToStreamAsync(
      Stream target,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    void DownloadToFile(
      string path,
      FileMode mode,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginDownloadToFile(
      string path,
      FileMode mode,
      AsyncCallback callback,
      object state);

    ICancellableAsyncResult BeginDownloadToFile(
      string path,
      FileMode mode,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    void EndDownloadToFile(IAsyncResult asyncResult);

    Task DownloadToFileAsync(string path, FileMode mode);

    Task DownloadToFileAsync(string path, FileMode mode, CancellationToken cancellationToken);

    Task DownloadToFileAsync(
      string path,
      FileMode mode,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task DownloadToFileAsync(
      string path,
      FileMode mode,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    int DownloadToByteArray(
      byte[] target,
      int index,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginDownloadToByteArray(
      byte[] target,
      int index,
      AsyncCallback callback,
      object state);

    ICancellableAsyncResult BeginDownloadToByteArray(
      byte[] target,
      int index,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    int EndDownloadToByteArray(IAsyncResult asyncResult);

    Task<int> DownloadToByteArrayAsync(byte[] target, int index);

    Task<int> DownloadToByteArrayAsync(
      byte[] target,
      int index,
      CancellationToken cancellationToken);

    Task<int> DownloadToByteArrayAsync(
      byte[] target,
      int index,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task<int> DownloadToByteArrayAsync(
      byte[] target,
      int index,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    void DownloadRangeToStream(
      Stream target,
      long? offset,
      long? length,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginDownloadRangeToStream(
      Stream target,
      long? offset,
      long? length,
      AsyncCallback callback,
      object state);

    ICancellableAsyncResult BeginDownloadRangeToStream(
      Stream target,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    void EndDownloadRangeToStream(IAsyncResult asyncResult);

    Task DownloadRangeToStreamAsync(Stream target, long? offset, long? length);

    Task DownloadRangeToStreamAsync(
      Stream target,
      long? offset,
      long? length,
      CancellationToken cancellationToken);

    Task DownloadRangeToStreamAsync(
      Stream target,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task DownloadRangeToStreamAsync(
      Stream target,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    int DownloadRangeToByteArray(
      byte[] target,
      int index,
      long? blobOffset,
      long? length,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginDownloadRangeToByteArray(
      byte[] target,
      int index,
      long? blobOffset,
      long? length,
      AsyncCallback callback,
      object state);

    ICancellableAsyncResult BeginDownloadRangeToByteArray(
      byte[] target,
      int index,
      long? blobOffset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    int EndDownloadRangeToByteArray(IAsyncResult asyncResult);

    Task<int> DownloadRangeToByteArrayAsync(
      byte[] target,
      int index,
      long? blobOffset,
      long? length);

    Task<int> DownloadRangeToByteArrayAsync(
      byte[] target,
      int index,
      long? blobOffset,
      long? length,
      CancellationToken cancellationToken);

    Task<int> DownloadRangeToByteArrayAsync(
      byte[] target,
      int index,
      long? blobOffset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task<int> DownloadRangeToByteArrayAsync(
      byte[] target,
      int index,
      long? blobOffset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    bool Exists(BlobRequestOptions options = null, OperationContext operationContext = null);

    ICancellableAsyncResult BeginExists(AsyncCallback callback, object state);

    ICancellableAsyncResult BeginExists(
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    bool EndExists(IAsyncResult asyncResult);

    Task<bool> ExistsAsync();

    Task<bool> ExistsAsync(CancellationToken cancellationToken);

    Task<bool> ExistsAsync(BlobRequestOptions options, OperationContext operationContext);

    Task<bool> ExistsAsync(
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    void FetchAttributes(
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginFetchAttributes(AsyncCallback callback, object state);

    ICancellableAsyncResult BeginFetchAttributes(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    void EndFetchAttributes(IAsyncResult asyncResult);

    Task FetchAttributesAsync();

    Task FetchAttributesAsync(CancellationToken cancellationToken);

    Task FetchAttributesAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task FetchAttributesAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    void SetMetadata(
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginSetMetadata(AsyncCallback callback, object state);

    ICancellableAsyncResult BeginSetMetadata(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    void EndSetMetadata(IAsyncResult asyncResult);

    Task SetMetadataAsync();

    Task SetMetadataAsync(CancellationToken cancellationToken);

    Task SetMetadataAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task SetMetadataAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    void SetProperties(
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginSetProperties(AsyncCallback callback, object state);

    ICancellableAsyncResult BeginSetProperties(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    void EndSetProperties(IAsyncResult asyncResult);

    Task SetPropertiesAsync();

    Task SetPropertiesAsync(CancellationToken cancellationToken);

    Task SetPropertiesAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task SetPropertiesAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    void Delete(
      DeleteSnapshotsOption deleteSnapshotsOption = DeleteSnapshotsOption.None,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginDelete(AsyncCallback callback, object state);

    ICancellableAsyncResult BeginDelete(
      DeleteSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    void EndDelete(IAsyncResult asyncResult);

    Task DeleteAsync();

    Task DeleteAsync(CancellationToken cancellationToken);

    Task DeleteAsync(
      DeleteSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task DeleteAsync(
      DeleteSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    bool DeleteIfExists(
      DeleteSnapshotsOption deleteSnapshotsOption = DeleteSnapshotsOption.None,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginDeleteIfExists(AsyncCallback callback, object state);

    ICancellableAsyncResult BeginDeleteIfExists(
      DeleteSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    bool EndDeleteIfExists(IAsyncResult asyncResult);

    Task<bool> DeleteIfExistsAsync();

    Task<bool> DeleteIfExistsAsync(CancellationToken cancellationToken);

    Task<bool> DeleteIfExistsAsync(
      DeleteSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task<bool> DeleteIfExistsAsync(
      DeleteSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    string AcquireLease(
      TimeSpan? leaseTime,
      string proposedLeaseId,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginAcquireLease(
      TimeSpan? leaseTime,
      string proposedLeaseId,
      AsyncCallback callback,
      object state);

    ICancellableAsyncResult BeginAcquireLease(
      TimeSpan? leaseTime,
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    string EndAcquireLease(IAsyncResult asyncResult);

    Task<string> AcquireLeaseAsync(TimeSpan? leaseTime, string proposedLeaseId = null);

    Task<string> AcquireLeaseAsync(
      TimeSpan? leaseTime,
      string proposedLeaseId,
      CancellationToken cancellationToken);

    Task<string> AcquireLeaseAsync(
      TimeSpan? leaseTime,
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task<string> AcquireLeaseAsync(
      TimeSpan? leaseTime,
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    void RenewLease(
      AccessCondition accessCondition,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginRenewLease(
      AccessCondition accessCondition,
      AsyncCallback callback,
      object state);

    ICancellableAsyncResult BeginRenewLease(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    void EndRenewLease(IAsyncResult asyncResult);

    Task RenewLeaseAsync(AccessCondition accessCondition);

    Task RenewLeaseAsync(AccessCondition accessCondition, CancellationToken cancellationToken);

    Task RenewLeaseAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task RenewLeaseAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    string ChangeLease(
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginChangeLease(
      string proposedLeaseId,
      AccessCondition accessCondition,
      AsyncCallback callback,
      object state);

    ICancellableAsyncResult BeginChangeLease(
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    string EndChangeLease(IAsyncResult asyncResult);

    Task<string> ChangeLeaseAsync(string proposedLeaseId, AccessCondition accessCondition);

    Task<string> ChangeLeaseAsync(
      string proposedLeaseId,
      AccessCondition accessCondition,
      CancellationToken cancellationToken);

    Task<string> ChangeLeaseAsync(
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task<string> ChangeLeaseAsync(
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    void ReleaseLease(
      AccessCondition accessCondition,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginReleaseLease(
      AccessCondition accessCondition,
      AsyncCallback callback,
      object state);

    ICancellableAsyncResult BeginReleaseLease(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    void EndReleaseLease(IAsyncResult asyncResult);

    Task ReleaseLeaseAsync(AccessCondition accessCondition);

    Task ReleaseLeaseAsync(AccessCondition accessCondition, CancellationToken cancellationToken);

    Task ReleaseLeaseAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task ReleaseLeaseAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    TimeSpan BreakLease(
      TimeSpan? breakPeriod = null,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginBreakLease(
      TimeSpan? breakPeriod,
      AsyncCallback callback,
      object state);

    ICancellableAsyncResult BeginBreakLease(
      TimeSpan? breakPeriod,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    TimeSpan EndBreakLease(IAsyncResult asyncResult);

    Task<TimeSpan> BreakLeaseAsync(TimeSpan? breakPeriod);

    Task<TimeSpan> BreakLeaseAsync(TimeSpan? breakPeriod, CancellationToken cancellationToken);

    Task<TimeSpan> BreakLeaseAsync(
      TimeSpan? breakPeriod,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task<TimeSpan> BreakLeaseAsync(
      TimeSpan? breakPeriod,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    void AbortCopy(
      string copyId,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null);

    ICancellableAsyncResult BeginAbortCopy(string copyId, AsyncCallback callback, object state);

    ICancellableAsyncResult BeginAbortCopy(
      string copyId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    void EndAbortCopy(IAsyncResult asyncResult);

    Task AbortCopyAsync(string copyId);

    Task AbortCopyAsync(string copyId, CancellationToken cancellationToken);

    Task AbortCopyAsync(
      string copyId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext);

    Task AbortCopyAsync(
      string copyId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    [DoesServiceRequest]
    ICancellableAsyncResult BeginGetAccountProperties(AsyncCallback callback, object state);

    [DoesServiceRequest]
    ICancellableAsyncResult BeginGetAccountProperties(
      BlobRequestOptions requestOptions,
      OperationContext operationContext,
      AsyncCallback callback,
      object state);

    AccountProperties EndGetAccountProperties(IAsyncResult asyncResult);

    [DoesServiceRequest]
    Task<AccountProperties> GetAccountPropertiesAsync();

    [DoesServiceRequest]
    Task<AccountProperties> GetAccountPropertiesAsync(CancellationToken cancellationToken);

    [DoesServiceRequest]
    Task<AccountProperties> GetAccountPropertiesAsync(
      BlobRequestOptions requestOptions,
      OperationContext operationContext);

    [DoesServiceRequest]
    Task<AccountProperties> GetAccountPropertiesAsync(
      BlobRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken);

    [DoesServiceRequest]
    AccountProperties GetAccountProperties(
      BlobRequestOptions requestOptions = null,
      OperationContext operationContext = null);

    string Name { get; }

    CloudBlobClient ServiceClient { get; }

    int StreamWriteSizeInBytes { get; set; }

    int StreamMinimumReadSizeInBytes { get; set; }

    BlobProperties Properties { get; }

    IDictionary<string, string> Metadata { get; }

    DateTimeOffset? SnapshotTime { get; }

    bool IsSnapshot { get; }

    Uri SnapshotQualifiedUri { get; }

    StorageUri SnapshotQualifiedStorageUri { get; }

    CopyState CopyState { get; }

    BlobType BlobType { get; }

    string GetSharedAccessSignature(SharedAccessBlobPolicy policy);

    string GetSharedAccessSignature(SharedAccessBlobPolicy policy, string groupPolicyIdentifier);

    string GetSharedAccessSignature(SharedAccessBlobPolicy policy, SharedAccessBlobHeaders headers);

    string GetSharedAccessSignature(
      SharedAccessBlobPolicy policy,
      SharedAccessBlobHeaders headers,
      string groupPolicyIdentifier);

    string GetSharedAccessSignature(
      SharedAccessBlobPolicy policy,
      SharedAccessBlobHeaders headers,
      string groupPolicyIdentifier,
      SharedAccessProtocol? protocols,
      IPAddressOrRange ipAddressOrRange);
  }
}
