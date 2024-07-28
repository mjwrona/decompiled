// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.CloudBlob
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.KeyVault.Core;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob.Protocol;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Auth;
using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Blob
{
  public class CloudBlob : IListBlobItem
  {
    private int streamMinimumReadSizeInBytes = 4194304;
    private CloudBlobContainer container;
    private CloudBlobDirectory parent;
    internal readonly BlobAttributes attributes;

    [DoesServiceRequest]
    public virtual Stream OpenRead(
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.FetchAttributes(accessCondition, options, operationContext);
      return (Stream) new BlobReadStream(this, AccessCondition.CloneConditionWithETag(accessCondition, this.Properties.ETag), BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient, false), operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginOpenRead(AsyncCallback callback, object state) => this.BeginOpenRead((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginOpenRead(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<Stream>((Func<CancellationToken, Task<Stream>>) (token => this.OpenReadAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual Stream EndOpenRead(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<Stream>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<Stream> OpenReadAsync() => this.OpenReadAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<Stream> OpenReadAsync(CancellationToken cancellationToken) => this.OpenReadAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<Stream> OpenReadAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.OpenReadAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual async Task<Stream> OpenReadAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CloudBlob blob = this;
      await blob.FetchAttributesAsync(accessCondition, options, operationContext, cancellationToken).ConfigureAwait(false);
      // ISSUE: explicit non-virtual call
      AccessCondition accessCondition1 = AccessCondition.CloneConditionWithETag(accessCondition, __nonvirtual (blob.Properties).ETag);
      // ISSUE: explicit non-virtual call
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, __nonvirtual (blob.ServiceClient), false);
      return (Stream) new BlobReadStream(blob, accessCondition1, options1, operationContext);
    }

    [DoesServiceRequest]
    public virtual void DownloadToStream(
      Stream target,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.DownloadRangeToStream(target, new long?(), new long?(), accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadToStream(
      Stream target,
      AsyncCallback callback,
      object state)
    {
      return this.BeginDownloadToStream(target, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadToStream(
      Stream target,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginDownloadRangeToStream(target, new long?(), new long?(), accessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    internal ICancellableAsyncResult BeginDownloadToStream(
      Stream target,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.DownloadToStreamAsync(target, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndDownloadToStream(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task DownloadToStreamAsync(Stream target) => this.DownloadRangeToStreamAsync(target, new long?(), new long?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task DownloadToStreamAsync(Stream target, CancellationToken cancellationToken) => this.DownloadRangeToStreamAsync(target, new long?(), new long?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task DownloadToStreamAsync(
      Stream target,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.DownloadRangeToStreamAsync(target, new long?(), new long?(), accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task DownloadToStreamAsync(
      Stream target,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.DownloadRangeToStreamAsync(target, new long?(), new long?(), accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task DownloadToStreamAsync(
      Stream target,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.DownloadRangeToStreamAsync(target, new long?(), new long?(), accessCondition, options, operationContext, progressHandler, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void RotateEncryptionKey(
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions modifiedOptions = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      CloudBlob.WrappedKeyData wrappedKeyData = CommonUtility.RunWithoutSynchronizationContext<CloudBlob.WrappedKeyData>((Func<CloudBlob.WrappedKeyData>) (() => this.RotateEncryptionHelper(accessCondition, modifiedOptions, CancellationToken.None).GetAwaiter().GetResult()));
      BlobEncryptionData encryptionData = wrappedKeyData.encryptionData;
      encryptionData.WrappedContentKey = new WrappedKey(modifiedOptions.EncryptionPolicy.Key.Kid, wrappedKeyData.encryptedKey, wrappedKeyData.algorithm);
      this.Metadata["encryptiondata"] = JsonConvert.SerializeObject((object) encryptionData, Formatting.None);
      if (accessCondition == null)
        accessCondition = new AccessCondition();
      accessCondition.IfMatchETag = this.Properties.ETag;
      try
      {
        this.SetMetadata(accessCondition, modifiedOptions, operationContext);
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 412)
          throw new StorageException(ex.RequestInformation, "Precondition failed.  If this is due to an incorrect ETag value, call FetchAttributes on the local blob object and retry rotating the encryption key.", (Exception) ex);
        throw;
      }
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginRotateEncryptionKey(
      AsyncCallback callback,
      object state)
    {
      return this.BeginRotateEncryptionKey((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginRotateEncryptionKey(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.RotateEncryptionKeyAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndRotateEncryptionKey(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task RotateEncryptionKeyAsync() => this.RotateEncryptionKeyAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task RotateEncryptionKeyAsync(CancellationToken cancellationToken) => this.RotateEncryptionKeyAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task RotateEncryptionKeyAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.RotateEncryptionKeyAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual async Task RotateEncryptionKeyAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions modifiedOptions = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      CloudBlob.WrappedKeyData wrappedKeyData = await this.RotateEncryptionHelper(accessCondition, modifiedOptions, cancellationToken).ConfigureAwait(false);
      BlobEncryptionData encryptionData = wrappedKeyData.encryptionData;
      encryptionData.WrappedContentKey = new WrappedKey(modifiedOptions.EncryptionPolicy.Key.Kid, wrappedKeyData.encryptedKey, wrappedKeyData.algorithm);
      this.Metadata["encryptiondata"] = JsonConvert.SerializeObject((object) encryptionData, Formatting.None);
      if (accessCondition == null)
        accessCondition = new AccessCondition();
      accessCondition.IfMatchETag = this.Properties.ETag;
      try
      {
        await this.SetMetadataAsync(accessCondition, modifiedOptions, operationContext, cancellationToken).ConfigureAwait(false);
        modifiedOptions = (BlobRequestOptions) null;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 412)
          throw new StorageException(ex.RequestInformation, "Precondition failed.  If this is due to an incorrect ETag value, call FetchAttributes on the local blob object and retry rotating the encryption key.", (Exception) ex);
        modifiedOptions = (BlobRequestOptions) null;
      }
    }

    [DoesServiceRequest]
    public virtual void DownloadToFile(
      string path,
      FileMode mode,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (path), (object) path);
      FileStream target = new FileStream(path, mode, FileAccess.Write, FileShare.None);
      try
      {
        using (target)
          this.DownloadToStream((Stream) target, accessCondition, options, operationContext);
      }
      catch (Exception ex1)
      {
        if (mode == FileMode.Create || mode == FileMode.CreateNew)
        {
          try
          {
            System.IO.File.Delete(path);
          }
          catch (Exception ex2)
          {
          }
        }
        throw;
      }
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadToFile(
      string path,
      FileMode mode,
      AsyncCallback callback,
      object state)
    {
      return this.BeginDownloadToFile(path, mode, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadToFile(
      string path,
      FileMode mode,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginDownloadToFile(path, mode, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginDownloadToFile(
      string path,
      FileMode mode,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.DownloadToFileAsync(path, mode, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndDownloadToFile(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task DownloadToFileAsync(string path, FileMode mode) => this.DownloadToFileAsync(path, mode, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task DownloadToFileAsync(
      string path,
      FileMode mode,
      CancellationToken cancellationToken)
    {
      return this.DownloadToFileAsync(path, mode, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task DownloadToFileAsync(
      string path,
      FileMode mode,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.DownloadToFileAsync(path, mode, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task DownloadToFileAsync(
      string path,
      FileMode mode,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.DownloadToFileAsync(path, mode, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task DownloadToFileAsync(
      string path,
      FileMode mode,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (path), (object) path);
      FileStream target = new FileStream(path, mode, FileAccess.Write, FileShare.None);
      try
      {
        using (target)
          await this.DownloadToStreamAsync((Stream) target, accessCondition, options, operationContext, progressHandler, cancellationToken).ConfigureAwait(false);
      }
      catch (Exception ex1)
      {
        if (mode == FileMode.Create || mode == FileMode.CreateNew)
        {
          try
          {
            System.IO.File.Delete(path);
          }
          catch (Exception ex2)
          {
          }
        }
        throw;
      }
    }

    [DoesServiceRequest]
    public virtual Task DownloadToFileParallelAsync(
      string path,
      FileMode mode,
      int parallelIOCount,
      long? rangeSizeInBytes)
    {
      return this.DownloadToFileParallelAsync(path, mode, parallelIOCount, rangeSizeInBytes, 0L, new long?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task DownloadToFileParallelAsync(
      string path,
      FileMode mode,
      int parallelIOCount,
      long? rangeSizeInBytes,
      CancellationToken cancellationToken)
    {
      return this.DownloadToFileParallelAsync(path, mode, parallelIOCount, rangeSizeInBytes, 0L, new long?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task DownloadToFileParallelAsync(
      string path,
      FileMode mode,
      int parallelIOCount,
      long? rangeSizeInBytes,
      long offset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return ParallelDownloadToFile.Start(this, path, mode, parallelIOCount, rangeSizeInBytes, offset, length, 120000, accessCondition, options, operationContext, cancellationToken).Task;
    }

    [DoesServiceRequest]
    public virtual int DownloadToByteArray(
      byte[] target,
      int index,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return this.DownloadRangeToByteArray(target, index, new long?(), new long?(), accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadToByteArray(
      byte[] target,
      int index,
      AsyncCallback callback,
      object state)
    {
      return this.BeginDownloadToByteArray(target, index, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadToByteArray(
      byte[] target,
      int index,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginDownloadRangeToByteArray(target, index, new long?(), new long?(), accessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginDownloadToByteArray(
      byte[] target,
      int index,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<int>((Func<CancellationToken, Task<int>>) (token => this.DownloadToByteArrayAsync(target, index, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual int EndDownloadToByteArray(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<int>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<int> DownloadToByteArrayAsync(byte[] target, int index) => this.DownloadRangeToByteArrayAsync(target, index, new long?(), new long?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<int> DownloadToByteArrayAsync(
      byte[] target,
      int index,
      CancellationToken cancellationToken)
    {
      return this.DownloadRangeToByteArrayAsync(target, index, new long?(), new long?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<int> DownloadToByteArrayAsync(
      byte[] target,
      int index,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.DownloadRangeToByteArrayAsync(target, index, new long?(), new long?(), accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<int> DownloadToByteArrayAsync(
      byte[] target,
      int index,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.DownloadRangeToByteArrayAsync(target, index, new long?(), new long?(), accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<int> DownloadToByteArrayAsync(
      byte[] target,
      int index,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.DownloadRangeToByteArrayAsync(target, index, new long?(), new long?(), accessCondition, options, operationContext, progressHandler, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void DownloadRangeToStream(
      Stream target,
      long? offset,
      long? length,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (target), (object) target);
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.GetBlobImpl(this.attributes, target, offset, length, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadRangeToStream(
      Stream target,
      long? offset,
      long? length,
      AsyncCallback callback,
      object state)
    {
      return this.BeginDownloadRangeToStream(target, offset, length, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadRangeToStream(
      Stream target,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginDownloadRangeToStream(target, offset, length, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginDownloadRangeToStream(
      Stream target,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.DownloadRangeToStreamAsync(target, offset, length, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual void EndDownloadRangeToStream(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task DownloadRangeToStreamAsync(Stream target, long? offset, long? length) => this.DownloadRangeToStreamAsync(target, offset, length, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task DownloadRangeToStreamAsync(
      Stream target,
      long? offset,
      long? length,
      CancellationToken cancellationToken)
    {
      return this.DownloadRangeToStreamAsync(target, offset, length, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task DownloadRangeToStreamAsync(
      Stream target,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.DownloadRangeToStreamAsync(target, offset, length, accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task DownloadRangeToStreamAsync(
      Stream target,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.DownloadRangeToStreamAsync(target, offset, length, accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task DownloadRangeToStreamAsync(
      Stream target,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.DownloadRangeToStreamAsync(target, offset, length, accessCondition, options, operationContext, new AggregatingProgressIncrementer(progressHandler), cancellationToken);
    }

    [DoesServiceRequest]
    private Task DownloadRangeToStreamAsync(
      Stream target,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AggregatingProgressIncrementer progressIncrementer,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (target), (object) target);
      progressIncrementer = progressIncrementer ?? AggregatingProgressIncrementer.None;
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.GetBlobImpl(this.attributes, progressIncrementer.CreateProgressIncrementingStream(target), offset, length, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual int DownloadRangeToByteArray(
      byte[] target,
      int index,
      long? blobOffset,
      long? length,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      using (SyncMemoryStream target1 = new SyncMemoryStream(target, index))
      {
        this.DownloadRangeToStream((Stream) target1, blobOffset, length, accessCondition, options, operationContext);
        return (int) target1.Position;
      }
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadRangeToByteArray(
      byte[] target,
      int index,
      long? blobOffset,
      long? length,
      AsyncCallback callback,
      object state)
    {
      return this.BeginDownloadRangeToByteArray(target, index, blobOffset, length, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadRangeToByteArray(
      byte[] target,
      int index,
      long? blobOffset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginDownloadRangeToByteArray(target, index, blobOffset, length, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginDownloadRangeToByteArray(
      byte[] target,
      int index,
      long? blobOffset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<int>((Func<CancellationToken, Task<int>>) (token => this.DownloadRangeToByteArrayAsync(target, index, blobOffset, length, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual int EndDownloadRangeToByteArray(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<int>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<int> DownloadRangeToByteArrayAsync(
      byte[] target,
      int index,
      long? blobOffset,
      long? length)
    {
      return this.DownloadRangeToByteArrayAsync(target, index, blobOffset, length, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<int> DownloadRangeToByteArrayAsync(
      byte[] target,
      int index,
      long? blobOffset,
      long? length,
      CancellationToken cancellationToken)
    {
      return this.DownloadRangeToByteArrayAsync(target, index, blobOffset, length, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<int> DownloadRangeToByteArrayAsync(
      byte[] target,
      int index,
      long? blobOffset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.DownloadRangeToByteArrayAsync(target, index, blobOffset, length, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<int> DownloadRangeToByteArrayAsync(
      byte[] target,
      int index,
      long? blobOffset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.DownloadRangeToByteArrayAsync(target, index, blobOffset, length, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task<int> DownloadRangeToByteArrayAsync(
      byte[] target,
      int index,
      long? blobOffset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      int position;
      using (SyncMemoryStream stream = new SyncMemoryStream(target, index))
      {
        await this.DownloadRangeToStreamAsync((Stream) stream, blobOffset, length, accessCondition, options, operationContext, progressHandler, cancellationToken).ConfigureAwait(false);
        position = (int) stream.Position;
      }
      return position;
    }

    [DoesServiceRequest]
    public virtual bool Exists(BlobRequestOptions options = null, OperationContext operationContext = null) => this.Exists(false, options, operationContext);

    private bool Exists(
      bool primaryOnly,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<bool>(this.ExistsImpl(this.attributes, options1, primaryOnly), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginExists(AsyncCallback callback, object state) => this.BeginExists((BlobRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginExists(
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginExists(false, options, operationContext, callback, state);
    }

    private ICancellableAsyncResult BeginExists(
      bool primaryOnly,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<bool>((Func<CancellationToken, Task<bool>>) (token => this.ExistsAsync(primaryOnly, options, operationContext, token)), callback, state);
    }

    public virtual bool EndExists(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<bool>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync() => this.ExistsAsync(false, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync(CancellationToken cancellationToken) => this.ExistsAsync(false, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync(
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.ExistsAsync(false, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync(
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.ExistsAsync(false, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync(
      bool primaryOnly,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<bool>(this.ExistsImpl(this.attributes, options1, primaryOnly), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void FetchAttributes(
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.FetchAttributesImpl(this.attributes, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginFetchAttributes(
      AsyncCallback callback,
      object state)
    {
      return this.BeginFetchAttributes((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginFetchAttributes(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.FetchAttributesAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndFetchAttributes(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync() => this.FetchAttributesAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync(CancellationToken cancellationToken) => this.FetchAttributesAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.FetchAttributesAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.FetchAttributesImpl(this.attributes, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void SetMetadata(
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.SetMetadataImpl(this.attributes, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetMetadata(AsyncCallback callback, object state) => this.BeginSetMetadata((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetMetadata(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.SetMetadataAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndSetMetadata(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync() => this.SetMetadataAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync(CancellationToken cancellationToken) => this.SetMetadataAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.SetMetadataAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.SetMetadataImpl(this.attributes, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void SetProperties(
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.SetPropertiesImpl(this.attributes, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetProperties(AsyncCallback callback, object state) => this.BeginSetProperties((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetProperties(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.SetPropertiesAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndSetProperties(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task SetPropertiesAsync() => this.SetPropertiesAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task SetPropertiesAsync(CancellationToken cancellationToken) => this.SetPropertiesAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task SetPropertiesAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.SetPropertiesAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task SetPropertiesAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.SetPropertiesImpl(this.attributes, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void Delete(
      DeleteSnapshotsOption deleteSnapshotsOption = DeleteSnapshotsOption.None,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.DeleteBlobImpl(this.attributes, deleteSnapshotsOption, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDelete(AsyncCallback callback, object state) => this.BeginDelete(DeleteSnapshotsOption.None, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDelete(
      DeleteSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.DeleteAsync(deleteSnapshotsOption, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndDelete(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task DeleteAsync() => this.DeleteAsync(DeleteSnapshotsOption.None, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task DeleteAsync(CancellationToken cancellationToken) => this.DeleteAsync(DeleteSnapshotsOption.None, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task DeleteAsync(
      DeleteSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.DeleteAsync(deleteSnapshotsOption, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task DeleteAsync(
      DeleteSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.DeleteBlobImpl(this.attributes, deleteSnapshotsOption, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual bool DeleteIfExists(
      DeleteSnapshotsOption deleteSnapshotsOption = DeleteSnapshotsOption.None,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      try
      {
        this.Delete(deleteSnapshotsOption, accessCondition, options1, operationContext);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 404)
        {
          if (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == BlobErrorCodeStrings.BlobNotFound || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == BlobErrorCodeStrings.ContainerNotFound)
            return false;
          throw;
        }
        else
          throw;
      }
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDeleteIfExists(AsyncCallback callback, object state) => this.BeginDeleteIfExists(DeleteSnapshotsOption.None, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDeleteIfExists(
      DeleteSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<bool>((Func<CancellationToken, Task<bool>>) (token => this.DeleteIfExistsAsync(deleteSnapshotsOption, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual bool EndDeleteIfExists(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<bool>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<bool> DeleteIfExistsAsync() => this.DeleteIfExistsAsync(DeleteSnapshotsOption.None, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<bool> DeleteIfExistsAsync(CancellationToken cancellationToken) => this.DeleteIfExistsAsync(DeleteSnapshotsOption.None, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<bool> DeleteIfExistsAsync(
      DeleteSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.DeleteIfExistsAsync(deleteSnapshotsOption, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual async Task<bool> DeleteIfExistsAsync(
      DeleteSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      try
      {
        await this.DeleteAsync(deleteSnapshotsOption, accessCondition, options1, operationContext, cancellationToken).ConfigureAwait(false);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 404)
        {
          if (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == BlobErrorCodeStrings.BlobNotFound || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == BlobErrorCodeStrings.ContainerNotFound)
            return false;
          throw;
        }
        else
          throw;
      }
    }

    [DoesServiceRequest]
    public virtual void Undelete(
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.UndeleteBlobImpl(this.attributes, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUndelete(AsyncCallback callback, object state) => this.BeginUndelete((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUndelete(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UndeleteAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndUndelete(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task UndeleteAsync() => this.UndeleteAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task UndeleteAsync(CancellationToken cancellationToken) => this.UndeleteAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task UndeleteAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.UndeleteAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task UndeleteAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.UndeleteBlobImpl(this.attributes, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual string AcquireLease(
      TimeSpan? leaseTime,
      string proposedLeaseId,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<string>(this.AcquireLeaseImpl(this.attributes, leaseTime, proposedLeaseId, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAcquireLease(
      TimeSpan? leaseTime,
      string proposedLeaseId,
      AsyncCallback callback,
      object state)
    {
      return this.BeginAcquireLease(leaseTime, proposedLeaseId, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAcquireLease(
      TimeSpan? leaseTime,
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<string>((Func<CancellationToken, Task<string>>) (token => this.AcquireLeaseAsync(leaseTime, proposedLeaseId, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual string EndAcquireLease(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<string>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<string> AcquireLeaseAsync(TimeSpan? leaseTime, string proposedLeaseId = null) => this.AcquireLeaseAsync(leaseTime, proposedLeaseId, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<string> AcquireLeaseAsync(
      TimeSpan? leaseTime,
      string proposedLeaseId,
      CancellationToken cancellationToken)
    {
      return this.AcquireLeaseAsync(leaseTime, proposedLeaseId, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<string> AcquireLeaseAsync(
      TimeSpan? leaseTime,
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.AcquireLeaseAsync(leaseTime, proposedLeaseId, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<string> AcquireLeaseAsync(
      TimeSpan? leaseTime,
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<string>(this.AcquireLeaseImpl(this.attributes, leaseTime, proposedLeaseId, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void RenewLease(
      AccessCondition accessCondition,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.RenewLeaseImpl(this.attributes, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginRenewLease(
      AccessCondition accessCondition,
      AsyncCallback callback,
      object state)
    {
      return this.BeginRenewLease(accessCondition, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginRenewLease(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.RenewLeaseAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndRenewLease(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task RenewLeaseAsync(AccessCondition accessCondition) => this.RenewLeaseAsync(accessCondition, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task RenewLeaseAsync(
      AccessCondition accessCondition,
      CancellationToken cancellationToken)
    {
      return this.RenewLeaseAsync(accessCondition, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task RenewLeaseAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.RenewLeaseAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task RenewLeaseAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.RenewLeaseImpl(this.attributes, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual string ChangeLease(
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<string>(this.ChangeLeaseImpl(this.attributes, proposedLeaseId, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginChangeLease(
      string proposedLeaseId,
      AccessCondition accessCondition,
      AsyncCallback callback,
      object state)
    {
      return this.BeginChangeLease(proposedLeaseId, accessCondition, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginChangeLease(
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<string>((Func<CancellationToken, Task<string>>) (token => this.ChangeLeaseAsync(proposedLeaseId, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual string EndChangeLease(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<string>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<string> ChangeLeaseAsync(
      string proposedLeaseId,
      AccessCondition accessCondition)
    {
      return this.ChangeLeaseAsync(proposedLeaseId, accessCondition, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<string> ChangeLeaseAsync(
      string proposedLeaseId,
      AccessCondition accessCondition,
      CancellationToken cancellationToken)
    {
      return this.ChangeLeaseAsync(proposedLeaseId, accessCondition, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<string> ChangeLeaseAsync(
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.ChangeLeaseAsync(proposedLeaseId, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<string> ChangeLeaseAsync(
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<string>(this.ChangeLeaseImpl(this.attributes, proposedLeaseId, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void ReleaseLease(
      AccessCondition accessCondition,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.ReleaseLeaseImpl(this.attributes, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginReleaseLease(
      AccessCondition accessCondition,
      AsyncCallback callback,
      object state)
    {
      return this.BeginReleaseLease(accessCondition, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginReleaseLease(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.ReleaseLeaseAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndReleaseLease(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task ReleaseLeaseAsync(AccessCondition accessCondition) => this.ReleaseLeaseAsync(accessCondition, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task ReleaseLeaseAsync(
      AccessCondition accessCondition,
      CancellationToken cancellationToken)
    {
      return this.ReleaseLeaseAsync(accessCondition, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task ReleaseLeaseAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.ReleaseLeaseAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task ReleaseLeaseAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.ReleaseLeaseImpl(this.attributes, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual TimeSpan BreakLease(
      TimeSpan? breakPeriod = null,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<TimeSpan>(this.BreakLeaseImpl(this.attributes, breakPeriod, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginBreakLease(
      TimeSpan? breakPeriod,
      AsyncCallback callback,
      object state)
    {
      return this.BeginBreakLease(breakPeriod, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginBreakLease(
      TimeSpan? breakPeriod,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<TimeSpan>((Func<CancellationToken, Task<TimeSpan>>) (token => this.BreakLeaseAsync(breakPeriod, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual TimeSpan EndBreakLease(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<TimeSpan>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<TimeSpan> BreakLeaseAsync(TimeSpan? breakPeriod) => this.BreakLeaseAsync(breakPeriod, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<TimeSpan> BreakLeaseAsync(
      TimeSpan? breakPeriod,
      CancellationToken cancellationToken)
    {
      return this.BreakLeaseAsync(breakPeriod, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<TimeSpan> BreakLeaseAsync(
      TimeSpan? breakPeriod,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.BreakLeaseAsync(breakPeriod, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<TimeSpan> BreakLeaseAsync(
      TimeSpan? breakPeriod,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<TimeSpan>(this.BreakLeaseImpl(this.attributes, breakPeriod, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual string StartCopy(
      Uri source,
      AccessCondition sourceAccessCondition = null,
      AccessCondition destAccessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return this.StartCopy(source, new PremiumPageBlobTier?(), new StandardBlobTier?(), new RehydratePriority?(), sourceAccessCondition, destAccessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    internal virtual string StartCopy(
      Uri source,
      PremiumPageBlobTier? premiumPageBlobTier,
      StandardBlobTier? standardBlockBlobTier,
      RehydratePriority? rehydratePriority,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.StartCopy(source, Checksum.None, false, premiumPageBlobTier, standardBlockBlobTier, rehydratePriority, sourceAccessCondition, destAccessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    internal virtual string StartCopy(
      Uri source,
      Checksum contentChecksum,
      bool syncCopy,
      PremiumPageBlobTier? premiumPageBlobTier,
      StandardBlobTier? standardBlockBlobTier,
      RehydratePriority? rehydratePriority,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      CommonUtility.AssertNotNull(nameof (source), (object) source);
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<string>(this.StartCopyImpl(this.attributes, source, contentChecksum, false, syncCopy, premiumPageBlobTier, standardBlockBlobTier, rehydratePriority, sourceAccessCondition, destAccessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginStartCopy(
      Uri source,
      AsyncCallback callback,
      object state)
    {
      return this.BeginStartCopy(source, new PremiumPageBlobTier?(), new StandardBlobTier?(), (AccessCondition) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginStartCopy(
      Uri source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginStartCopy(source, new PremiumPageBlobTier?(), new StandardBlobTier?(), sourceAccessCondition, destAccessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    internal virtual ICancellableAsyncResult BeginStartCopy(
      Uri source,
      PremiumPageBlobTier? premiumPageBlobTier,
      StandardBlobTier? standardBlockBlobTier,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginStartCopy(source, (string) null, false, false, premiumPageBlobTier, standardBlockBlobTier, sourceAccessCondition, destAccessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    internal virtual ICancellableAsyncResult BeginStartCopy(
      Uri source,
      string contentMD5,
      bool incrementalCopy,
      bool syncCopy,
      PremiumPageBlobTier? premiumPageBlobTier,
      StandardBlobTier? standardBlockBlobTier,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<string>((Func<CancellationToken, Task<string>>) (token => this.StartCopyAsync(source, new Checksum(contentMD5), incrementalCopy, syncCopy, premiumPageBlobTier, standardBlockBlobTier, new RehydratePriority?(), sourceAccessCondition, destAccessCondition, options, operationContext, token)), callback, state);
    }

    public virtual string EndStartCopy(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<string>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(Uri source) => this.StartCopyAsync(source, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(Uri source, CancellationToken cancellationToken) => this.StartCopyAsync(source, (AccessCondition) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      Uri source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.StartCopyAsync(source, sourceAccessCondition, destAccessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      Uri source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.StartCopyAsync(source, Checksum.None, false, false, new PremiumPageBlobTier?(), new StandardBlobTier?(), new RehydratePriority?(), sourceAccessCondition, destAccessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      Uri source,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.StartCopyAsync(source, Checksum.None, false, false, premiumPageBlobTier, new StandardBlobTier?(), new RehydratePriority?(), sourceAccessCondition, destAccessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      Uri source,
      StandardBlobTier? standardBlockBlobTier,
      RehydratePriority? rehydratePriority,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.StartCopyAsync(source, Checksum.None, false, false, new PremiumPageBlobTier?(), standardBlockBlobTier, rehydratePriority, sourceAccessCondition, destAccessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    private Task<string> StartCopyAsync(
      Uri source,
      Checksum contentChecksum,
      bool incrementalCopy,
      bool syncCopy,
      PremiumPageBlobTier? premiumPageBlobTier,
      StandardBlobTier? standardBlockBlobTier,
      RehydratePriority? rehydratePriority,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (source), (object) source);
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<string>(this.StartCopyImpl(this.attributes, source, contentChecksum, false, syncCopy, premiumPageBlobTier, standardBlockBlobTier, rehydratePriority, sourceAccessCondition, destAccessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void AbortCopy(
      string copyId,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.AbortCopyImpl(this.attributes, copyId, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAbortCopy(
      string copyId,
      AsyncCallback callback,
      object state)
    {
      return this.BeginAbortCopy(copyId, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAbortCopy(
      string copyId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.AbortCopyAsync(copyId, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndAbortCopy(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task AbortCopyAsync(string copyId) => this.AbortCopyAsync(copyId, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task AbortCopyAsync(string copyId, CancellationToken cancellationToken) => this.AbortCopyAsync(copyId, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task AbortCopyAsync(
      string copyId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.AbortCopyAsync(copyId, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task AbortCopyAsync(
      string copyId,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.AbortCopyImpl(this.attributes, copyId, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual CloudBlob Snapshot(
      IDictionary<string, string> metadata = null,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<CloudBlob>(this.SnapshotImpl(metadata, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSnapshot(AsyncCallback callback, object state) => this.BeginSnapshot((IDictionary<string, string>) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSnapshot(
      IDictionary<string, string> metadata,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<CloudBlob>((Func<CancellationToken, Task<CloudBlob>>) (token => this.SnapshotAsync(metadata, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual CloudBlob EndSnapshot(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<CloudBlob>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<CloudBlob> SnapshotAsync() => this.SnapshotAsync((IDictionary<string, string>) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<CloudBlob> SnapshotAsync(CancellationToken cancellationToken) => this.SnapshotAsync((IDictionary<string, string>) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<CloudBlob> SnapshotAsync(
      IDictionary<string, string> metadata,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.SnapshotAsync(metadata, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<CloudBlob> SnapshotAsync(
      IDictionary<string, string> metadata,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<CloudBlob>(this.SnapshotImpl(metadata, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    private RESTCommand<NullType> GetBlobImpl(
      BlobAttributes blobAttributes,
      Stream destStream,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      string lockedETag = (string) null;
      AccessCondition lockedAccessCondition = (AccessCondition) null;
      bool isRangeGet = offset.HasValue;
      int discardFirst = 0;
      long? endOffset = new long?();
      bool bufferIV = false;
      long? userSpecifiedLength = length;
      options.AssertPolicyIfRequired();
      if (isRangeGet && options.EncryptionPolicy != null)
      {
        long? nullable;
        if (length.HasValue)
        {
          endOffset = new long?(offset.Value + length.Value - 1L);
          if ((endOffset.Value + 1L) % 16L != 0L)
          {
            nullable = endOffset;
            long num = (long) (int) (16L - (endOffset.Value + 1L) % 16L);
            endOffset = nullable.HasValue ? new long?(nullable.GetValueOrDefault() + num) : new long?();
          }
        }
        discardFirst = (int) (offset.Value % 16L);
        nullable = offset;
        long num1 = (long) discardFirst;
        offset = nullable.HasValue ? new long?(nullable.GetValueOrDefault() - num1) : new long?();
        nullable = offset;
        long num2 = 15;
        if (nullable.GetValueOrDefault() > num2 & nullable.HasValue)
        {
          nullable = offset;
          long num3 = 16;
          offset = nullable.HasValue ? new long?(nullable.GetValueOrDefault() - num3) : new long?();
          bufferIV = true;
        }
        if (endOffset.HasValue)
          length = new long?(endOffset.Value - offset.Value + 1L);
      }
      bool arePropertiesPopulated = false;
      bool decryptStreamCreated = false;
      ICryptoTransform transform = (ICryptoTransform) null;
      string storedMD5 = (string) null;
      string storedCRC64 = (string) null;
      long startingOffset = offset.HasValue ? offset.Value : 0L;
      long? startingLength = length;
      long? validateLength = new long?();
      RESTCommand<NullType> getCmd = new RESTCommand<NullType>(this.ServiceClient.Credentials, blobAttributes.StorageUri, this.ServiceClient.HttpClient);
      int num4 = options.ChecksumOptions.UseTransactionalMD5.Value ? 1 : 0;
      bool? nullable1 = options.ChecksumOptions.UseTransactionalCRC64;
      int num5 = nullable1.Value ? 1 : 0;
      ChecksumRequested checksumRequested = new ChecksumRequested(num4 != 0, num5 != 0);
      options.ApplyToStorageCommand<NullType>(getCmd);
      getCmd.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      getCmd.RetrieveResponseStream = true;
      getCmd.DestinationStream = destStream;
      RESTCommand<NullType> restCommand = getCmd;
      nullable1 = options.ChecksumOptions.DisableContentMD5Validation;
      int num6 = !nullable1.Value ? 1 : 0;
      nullable1 = options.ChecksumOptions.DisableContentCRC64Validation;
      int num7 = !nullable1.Value ? 1 : 0;
      ChecksumRequested checksumRequested1 = new ChecksumRequested(num6 != 0, num7 != 0);
      restCommand.ChecksumRequestedForResponseStream = checksumRequested1;
      getCmd.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        BlobRequest.VerifyHttpsCustomerProvidedKey(uri, options);
        return BlobHttpRequestMessageFactory.Get(uri, serverTimeout, blobAttributes.SnapshotTime, offset, length, checksumRequested, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options);
      });
      getCmd.RecoveryAction = (Action<StorageCommandBase<NullType>, Exception, OperationContext>) ((cmd, ex, ctx) =>
      {
        if (lockedAccessCondition == null && !string.IsNullOrEmpty(lockedETag))
        {
          lockedAccessCondition = AccessCondition.GenerateIfMatchCondition(lockedETag);
          if (accessCondition != null)
            lockedAccessCondition.LeaseId = accessCondition.LeaseId;
        }
        if (cmd.StreamCopyState != null)
        {
          offset = new long?(startingOffset + cmd.StreamCopyState.Length);
          if (startingLength.HasValue)
            length = new long?(startingLength.Value - cmd.StreamCopyState.Length);
        }
        ChecksumRequested checksumRequestedAndNotPopulated = new ChecksumRequested(options.ChecksumOptions.UseTransactionalMD5.Value && !arePropertiesPopulated, options.ChecksumOptions.UseTransactionalCRC64.Value && !arePropertiesPopulated);
        getCmd.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((command, uri, builder, cnt, serverTimeout, context) => BlobHttpRequestMessageFactory.Get(uri, serverTimeout, blobAttributes.SnapshotTime, offset, length, checksumRequestedAndNotPopulated, lockedAccessCondition ?? accessCondition, cnt, context, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options));
      });
      getCmd.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(offset.HasValue ? HttpStatusCode.PartialContent : HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        if (!arePropertiesPopulated)
        {
          CloudBlob.UpdateAfterFetchAttributes(blobAttributes, resp);
          storedMD5 = HttpResponseParsers.GetContentMD5(resp);
          storedCRC64 = HttpResponseParsers.GetContentCRC64(resp);
          if (options.EncryptionPolicy != null)
          {
            cmd.DestinationStream = BlobEncryptionPolicy.WrapUserStreamWithDecryptStream(this, cmd.DestinationStream, options, blobAttributes, isRangeGet, out transform, endOffset, userSpecifiedLength, discardFirst, bufferIV);
            decryptStreamCreated = true;
          }
          bool? nullable2 = options.ChecksumOptions.DisableContentMD5Validation;
          if (!nullable2.Value)
          {
            nullable2 = options.ChecksumOptions.UseTransactionalMD5;
            if (nullable2.Value && string.IsNullOrEmpty(storedMD5))
              throw new StorageException(cmd.CurrentResult, "MD5 does not exist. If you do not want to force validation, please disable UseTransactionalMD5.", (Exception) null)
              {
                IsRetryable = false
              };
          }
          nullable2 = options.ChecksumOptions.DisableContentCRC64Validation;
          if (!nullable2.Value)
          {
            nullable2 = options.ChecksumOptions.UseTransactionalCRC64;
            if (nullable2.Value && string.IsNullOrEmpty(storedCRC64))
              throw new StorageException(cmd.CurrentResult, "CRC64 does not exist. If you do not want to force validation, please disable UseTransactionalCRC64.", (Exception) null)
              {
                IsRetryable = false
              };
          }
          getCmd.CommandLocationMode = cmd.CurrentResult.TargetLocation == StorageLocation.Primary ? CommandLocationMode.PrimaryOnly : CommandLocationMode.SecondaryOnly;
          lockedETag = blobAttributes.Properties.ETag;
          validateLength = resp.Content.Headers.ContentLength;
          arePropertiesPopulated = true;
        }
        BlobResponse.ValidateCPKHeaders(resp, options, false);
        cmd.CurrentResult.IsServiceEncrypted = HttpResponseParsers.ParseServiceEncrypted(resp);
        cmd.CurrentResult.EncryptionKeySHA256 = HttpResponseParsers.ParseEncryptionKeySHA256(resp);
        cmd.CurrentResult.EncryptionScope = HttpResponseParsers.ParseEncryptionScope(resp);
        return NullType.Value;
      });
      getCmd.PostProcessResponseAsync = (Func<RESTCommand<NullType>, HttpResponseMessage, OperationContext, CancellationToken, Task<NullType>>) ((cmd, resp, ctx, ct) =>
      {
        BlobResponse.ValidateCPKHeaders(resp, options, false);
        cmd.CurrentResult.IsServiceEncrypted = HttpResponseParsers.ParseServiceEncrypted(resp);
        cmd.CurrentResult.EncryptionKeySHA256 = HttpResponseParsers.ParseEncryptionKeySHA256(resp);
        HttpResponseParsers.ValidateResponseStreamChecksumAndLength<NullType>(validateLength, storedMD5, storedCRC64, (StorageCommandBase<NullType>) cmd);
        cmd.CurrentResult.EncryptionScope = HttpResponseParsers.ParseEncryptionScope(resp);
        return NullType.ValueTask;
      });
      getCmd.DisposeAction = (Action<RESTCommand<NullType>>) (cmd =>
      {
        if (!decryptStreamCreated)
          return;
        try
        {
          if (cmd.DestinationStream != null)
            cmd.DestinationStream.Close();
          transform?.Dispose();
        }
        catch (Exception ex)
        {
          throw new StorageException(cmd.CurrentResult, "Cryptographic error occurred. Please check the inner exception for more details.", ex)
          {
            IsRetryable = false
          };
        }
      });
      return getCmd;
    }

    private RESTCommand<NullType> FetchAttributesImpl(
      BlobAttributes blobAttributes,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, blobAttributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        BlobRequest.VerifyHttpsCustomerProvidedKey(uri, options);
        return BlobHttpRequestMessageFactory.GetProperties(uri, serverTimeout, blobAttributes.SnapshotTime, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options);
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        CloudBlob.UpdateAfterFetchAttributes(blobAttributes, resp);
        cmd.CurrentResult.IsServiceEncrypted = HttpResponseParsers.ParseServiceEncrypted(resp);
        cmd.CurrentResult.EncryptionKeySHA256 = HttpResponseParsers.ParseEncryptionKeySHA256(resp);
        cmd.CurrentResult.EncryptionScope = HttpResponseParsers.ParseEncryptionScope(resp);
        BlobResponse.ValidateCPKHeaders(resp, options, false);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<bool> ExistsImpl(
      BlobAttributes blobAttributes,
      BlobRequestOptions options,
      bool primaryOnly)
    {
      RESTCommand<bool> cmd1 = new RESTCommand<bool>(this.ServiceClient.Credentials, blobAttributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<bool>(cmd1);
      cmd1.CommandLocationMode = primaryOnly ? CommandLocationMode.PrimaryOnly : CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<bool>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.GetProperties(uri, serverTimeout, blobAttributes.SnapshotTime, (AccessCondition) null, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options));
      cmd1.PreProcessResponse = (Func<RESTCommand<bool>, HttpResponseMessage, Exception, OperationContext, bool>) ((cmd, resp, ex, ctx) =>
      {
        if (resp.StatusCode == HttpStatusCode.NotFound)
          return false;
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<bool>(HttpStatusCode.OK, resp, true, (StorageCommandBase<bool>) cmd, ex);
        CloudBlob.UpdateAfterFetchAttributes(blobAttributes, resp);
        return true;
      });
      return cmd1;
    }

    private RESTCommand<NullType> SetMetadataImpl(
      BlobAttributes blobAttributes,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, blobAttributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        BlobRequest.VerifyHttpsCustomerProvidedKey(uri, options);
        StorageRequestMessage request = BlobHttpRequestMessageFactory.SetMetadata(uri, serverTimeout, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options);
        BlobHttpRequestMessageFactory.AddMetadata(request, blobAttributes.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(blobAttributes, resp, false);
        cmd.CurrentResult.IsRequestServerEncrypted = HttpResponseParsers.ParseServerRequestEncrypted(resp);
        cmd.CurrentResult.EncryptionKeySHA256 = HttpResponseParsers.ParseEncryptionKeySHA256(resp);
        cmd.CurrentResult.EncryptionScope = HttpResponseParsers.ParseEncryptionScope(resp);
        BlobResponse.ValidateCPKHeaders(resp, options, true);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<NullType> SetPropertiesImpl(
      BlobAttributes blobAttributes,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, blobAttributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = BlobHttpRequestMessageFactory.SetProperties(uri, serverTimeout, blobAttributes.Properties, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        BlobHttpRequestMessageFactory.AddMetadata(request, blobAttributes.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(blobAttributes, resp, false);
        return NullType.Value;
      });
      return cmd1;
    }

    internal RESTCommand<NullType> DeleteBlobImpl(
      BlobAttributes attributes,
      DeleteSnapshotsOption deleteSnapshotsOption,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.Delete(uri, serverTimeout, attributes.SnapshotTime, deleteSnapshotsOption, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Accepted, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex));
      return cmd1;
    }

    private RESTCommand<string> AcquireLeaseImpl(
      BlobAttributes attributes,
      TimeSpan? leaseTime,
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      int leaseDuration = -1;
      if (leaseTime.HasValue)
      {
        CommonUtility.AssertInBounds<TimeSpan>(nameof (leaseTime), leaseTime.Value, TimeSpan.FromSeconds(15.0), TimeSpan.FromSeconds(60.0));
        leaseDuration = (int) leaseTime.Value.TotalSeconds;
      }
      RESTCommand<string> cmd1 = new RESTCommand<string>(this.ServiceClient.Credentials, attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<string>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<string>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.Lease(uri, serverTimeout, LeaseAction.Acquire, proposedLeaseId, new int?(leaseDuration), new int?(), accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<string>, HttpResponseMessage, Exception, OperationContext, string>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<string>(HttpStatusCode.Created, resp, (string) null, (StorageCommandBase<string>) cmd, ex);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(attributes, resp, false);
        return BlobHttpResponseParsers.GetLeaseId(resp);
      });
      return cmd1;
    }

    private RESTCommand<NullType> RenewLeaseImpl(
      BlobAttributes attributes,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      CommonUtility.AssertNotNull(nameof (accessCondition), (object) accessCondition);
      if (accessCondition.LeaseId == null)
        throw new ArgumentException("A lease ID must be specified when renewing a lease.", nameof (accessCondition));
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.Lease(uri, serverTimeout, LeaseAction.Renew, (string) null, new int?(), new int?(), accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(attributes, resp, false);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<string> ChangeLeaseImpl(
      BlobAttributes attributes,
      string proposedLeaseId,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      CommonUtility.AssertNotNull(nameof (accessCondition), (object) accessCondition);
      CommonUtility.AssertNotNull(nameof (proposedLeaseId), (object) proposedLeaseId);
      if (accessCondition.LeaseId == null)
        throw new ArgumentException("A lease ID must be specified when changing a lease.", nameof (accessCondition));
      RESTCommand<string> cmd1 = new RESTCommand<string>(this.ServiceClient.Credentials, attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<string>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<string>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.Lease(uri, serverTimeout, LeaseAction.Change, proposedLeaseId, new int?(), new int?(), accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<string>, HttpResponseMessage, Exception, OperationContext, string>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<string>(HttpStatusCode.OK, resp, (string) null, (StorageCommandBase<string>) cmd, ex);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(attributes, resp, false);
        return BlobHttpResponseParsers.GetLeaseId(resp);
      });
      return cmd1;
    }

    private RESTCommand<NullType> ReleaseLeaseImpl(
      BlobAttributes attributes,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      CommonUtility.AssertNotNull(nameof (accessCondition), (object) accessCondition);
      if (accessCondition.LeaseId == null)
        throw new ArgumentException("A lease ID must be specified when releasing a lease.", nameof (accessCondition));
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.Lease(uri, serverTimeout, LeaseAction.Release, (string) null, new int?(), new int?(), accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(attributes, resp, false);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<TimeSpan> BreakLeaseImpl(
      BlobAttributes attributes,
      TimeSpan? breakPeriod,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      int? breakSeconds = new int?();
      if (breakPeriod.HasValue)
      {
        CommonUtility.AssertInBounds<TimeSpan>(nameof (breakPeriod), breakPeriod.Value, TimeSpan.FromSeconds(0.0), TimeSpan.FromSeconds(60.0));
        breakSeconds = new int?((int) breakPeriod.Value.TotalSeconds);
      }
      RESTCommand<TimeSpan> cmd1 = new RESTCommand<TimeSpan>(this.ServiceClient.Credentials, attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<TimeSpan>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<TimeSpan>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.Lease(uri, serverTimeout, LeaseAction.Break, (string) null, new int?(), breakSeconds, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<TimeSpan>, HttpResponseMessage, Exception, OperationContext, TimeSpan>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<TimeSpan>(HttpStatusCode.Accepted, resp, TimeSpan.Zero, (StorageCommandBase<TimeSpan>) cmd, ex);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(attributes, resp, false);
        return TimeSpan.FromSeconds((double) (BlobHttpResponseParsers.GetRemainingLeaseTime(resp) ?? throw new StorageException(cmd.CurrentResult, "Valid lease time expected but not received from the service.", (Exception) null)).Value);
      });
      return cmd1;
    }

    internal RESTCommand<string> StartCopyImpl(
      BlobAttributes attributes,
      Uri source,
      Checksum sourceContentChecksum,
      bool incrementalCopy,
      bool syncCopy,
      PremiumPageBlobTier? premiumPageBlobTier,
      StandardBlobTier? standardBlockBlobTier,
      RehydratePriority? rehydratePriority,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options)
    {
      if (sourceAccessCondition != null && !string.IsNullOrEmpty(sourceAccessCondition.LeaseId))
        throw new ArgumentException("A lease condition cannot be specified on the source of a copy.", nameof (sourceAccessCondition));
      RESTCommand<string> cmd1 = new RESTCommand<string>(this.ServiceClient.Credentials, attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<string>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<string>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = BlobHttpRequestMessageFactory.CopyFrom(uri, serverTimeout, source, sourceContentChecksum, incrementalCopy, syncCopy, premiumPageBlobTier, standardBlockBlobTier, rehydratePriority, sourceAccessCondition, destAccessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        BlobHttpRequestMessageFactory.AddMetadata(request, attributes.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<string>, HttpResponseMessage, Exception, OperationContext, string>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<string>(HttpStatusCode.Accepted, resp, (string) null, (StorageCommandBase<string>) cmd, ex);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(attributes, resp, false);
        CopyState copyAttributes = BlobHttpResponseParsers.GetCopyAttributes(resp);
        attributes.CopyState = copyAttributes;
        this.attributes.Properties.PremiumPageBlobTier = premiumPageBlobTier;
        if (premiumPageBlobTier.HasValue)
          this.attributes.Properties.BlobTierInferred = new bool?(false);
        return copyAttributes.CopyId;
      });
      return cmd1;
    }

    private RESTCommand<NullType> AbortCopyImpl(
      BlobAttributes attributes,
      string copyId,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      CommonUtility.AssertNotNull(nameof (copyId), (object) copyId);
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.AbortCopy(uri, serverTimeout, copyId, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.NoContent, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex));
      return cmd1;
    }

    private RESTCommand<CloudBlob> SnapshotImpl(
      IDictionary<string, string> metadata,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<CloudBlob> cmd1 = new RESTCommand<CloudBlob>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<CloudBlob>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<CloudBlob>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        BlobRequest.VerifyHttpsCustomerProvidedKey(uri, options);
        StorageRequestMessage request = BlobHttpRequestMessageFactory.Snapshot(uri, serverTimeout, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options);
        BlobHttpRequestMessageFactory.AddMetadata(request, metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<CloudBlob>, HttpResponseMessage, Exception, OperationContext, CloudBlob>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<CloudBlob>(HttpStatusCode.Created, resp, (CloudBlob) null, (StorageCommandBase<CloudBlob>) cmd, ex);
        CloudBlob cloudBlob = new CloudBlob(this.Name, new DateTimeOffset?(NavigationHelper.ParseSnapshotTime(BlobHttpResponseParsers.GetSnapshotTime(resp))), this.Container);
        cloudBlob.attributes.Metadata = (IDictionary<string, string>) new Dictionary<string, string>(metadata ?? this.Metadata, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        cloudBlob.attributes.Properties = new BlobProperties(this.Properties);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(cloudBlob.attributes, resp, false);
        if (metadata == null)
          return cloudBlob;
        cmd.CurrentResult.IsRequestServerEncrypted = HttpResponseParsers.ParseServerRequestEncrypted(resp);
        cmd.CurrentResult.EncryptionKeySHA256 = HttpResponseParsers.ParseEncryptionKeySHA256(resp);
        BlobResponse.ValidateCPKHeaders(resp, options, true);
        cmd.CurrentResult.EncryptionScope = HttpResponseParsers.ParseEncryptionScope(resp);
        return cloudBlob;
      });
      return cmd1;
    }

    private RESTCommand<NullType> UndeleteBlobImpl(
      BlobAttributes blobAttributes,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, blobAttributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.Undelete(uri, serverTimeout, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex));
      return cmd1;
    }

    private void ValidateKeyRotationArguments(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      bool encryptionMetadataAvailable)
    {
      if (accessCondition != null && accessCondition.IsConditional)
        throw new ArgumentException("Cannot supply an AccessCondition with an ETag or Modified-Since condition for key rotation.  An If-Match condition will be automatically applied.", nameof (accessCondition));
      if (options.EncryptionPolicy == null)
        throw new ArgumentException("Encryption Policy on the Request Options cannot be null for an encryption key rotation call.", "options.EncryptionPolicy");
      if (options.EncryptionPolicy.Key == null)
        throw new ArgumentException("Must supply a new encryption key as the Encryption Policy's \"Key\" parameter when rotating encryption keys.", "options.EncryptionPolicy.Key");
      if (options.EncryptionPolicy.KeyResolver == null)
        throw new ArgumentException("Must supply a key resolver on the encryption policy when rotating encryption keys.", "options.EncryptionPolicy.KeyResolver");
      if (this.Properties.ETag == null)
        throw new InvalidOperationException("Cannot rotate the encryption key when the ETag is not available on the blob.  You need to call FetchAttributes before calling this method.");
      if (!encryptionMetadataAvailable)
        throw new InvalidOperationException("Cannot rotate the encryption key when encryption metadata is not available on the blob.  Either the blob is not encrypted, or you need to call FetchAttributes before calling this method.");
    }

    private async Task<CloudBlob.WrappedKeyData> RotateEncryptionHelper(
      AccessCondition accessCondition,
      BlobRequestOptions modifiedOptions,
      CancellationToken cancellationToken)
    {
      string str;
      bool encryptionMetadataAvailable = this.Metadata.TryGetValue("encryptiondata", out str);
      this.ValidateKeyRotationArguments(accessCondition, modifiedOptions, encryptionMetadataAvailable);
      BlobEncryptionData encryptionData = JsonConvert.DeserializeObject<BlobEncryptionData>(str);
      if (encryptionData.WrappedContentKey.EncryptedKey == null)
        throw new InvalidOperationException("Cannot rotate encryption key when the encryption metadata does not contain a KeyID.");
      IKey ikey = await modifiedOptions.EncryptionPolicy.KeyResolver.ResolveKeyAsync(encryptionData.WrappedContentKey.KeyId, cancellationToken).ConfigureAwait(false);
      if (ikey == null)
        throw new ArgumentException("KeyResolver is not able to resolve the existing encryption key used to encrypt this blob.");
      Tuple<byte[], string> tuple = await modifiedOptions.EncryptionPolicy.Key.WrapKeyAsync(await ikey.UnwrapKeyAsync(encryptionData.WrappedContentKey.EncryptedKey, encryptionData.WrappedContentKey.Algorithm, cancellationToken).ConfigureAwait(false), (string) null, cancellationToken).ConfigureAwait(false);
      CloudBlob.WrappedKeyData wrappedKeyData = new CloudBlob.WrappedKeyData()
      {
        encryptedKey = tuple.Item1,
        algorithm = tuple.Item2,
        encryptionData = encryptionData
      };
      encryptionData = (BlobEncryptionData) null;
      return wrappedKeyData;
    }

    internal static void BlobOutputStreamCommitCallback(IAsyncResult result)
    {
      StorageAsyncResult<NullType> asyncState = (StorageAsyncResult<NullType>) result.AsyncState;
      CloudBlobStream operationState = (CloudBlobStream) asyncState.OperationState;
      asyncState.UpdateCompletedSynchronously(result.CompletedSynchronously);
      try
      {
        operationState.EndCommit(result);
        operationState.Dispose();
        asyncState.OnComplete();
      }
      catch (Exception ex)
      {
        asyncState.OnComplete(ex);
      }
    }

    internal static void UpdateAfterFetchAttributes(
      BlobAttributes blobAttributes,
      HttpResponseMessage response)
    {
      BlobProperties properties = BlobHttpResponseParsers.GetProperties(response);
      if (blobAttributes.Properties.BlobType != BlobType.Unspecified && blobAttributes.Properties.BlobType != properties.BlobType)
        throw new InvalidOperationException("Blob type of the blob reference doesn't match blob type of the blob.");
      blobAttributes.Properties = properties;
      blobAttributes.Metadata = BlobHttpResponseParsers.GetMetadata(response);
      blobAttributes.CopyState = BlobHttpResponseParsers.GetCopyAttributes(response);
    }

    internal static void UpdateETagLMTLengthAndSequenceNumber(
      BlobAttributes blobAttributes,
      HttpResponseMessage response,
      bool updateLength)
    {
      BlobProperties properties1 = BlobHttpResponseParsers.GetProperties(response);
      blobAttributes.Properties.ETag = properties1.ETag ?? blobAttributes.Properties.ETag;
      BlobProperties properties2 = blobAttributes.Properties;
      DateTimeOffset? nullable1 = properties1.Created;
      DateTimeOffset? nullable2 = nullable1 ?? blobAttributes.Properties.Created;
      properties2.Created = nullable2;
      BlobProperties properties3 = blobAttributes.Properties;
      nullable1 = properties1.LastModified;
      DateTimeOffset? nullable3 = nullable1 ?? blobAttributes.Properties.LastModified;
      properties3.LastModified = nullable3;
      blobAttributes.Properties.PageBlobSequenceNumber = properties1.PageBlobSequenceNumber ?? blobAttributes.Properties.PageBlobSequenceNumber;
      BlobProperties properties4 = blobAttributes.Properties;
      int? nullable4 = properties1.AppendBlobCommittedBlockCount;
      int? nullable5 = nullable4 ?? blobAttributes.Properties.AppendBlobCommittedBlockCount;
      properties4.AppendBlobCommittedBlockCount = nullable5;
      blobAttributes.Properties.IsServerEncrypted = properties1.IsServerEncrypted;
      blobAttributes.Properties.IsIncrementalCopy = properties1.IsIncrementalCopy;
      BlobProperties properties5 = blobAttributes.Properties;
      nullable1 = properties1.DeletedTime;
      DateTimeOffset? nullable6 = nullable1 ?? blobAttributes.Properties.DeletedTime;
      properties5.DeletedTime = nullable6;
      BlobProperties properties6 = blobAttributes.Properties;
      nullable4 = properties1.RemainingDaysBeforePermanentDelete;
      int? nullable7 = nullable4 ?? blobAttributes.Properties.RemainingDaysBeforePermanentDelete;
      properties6.RemainingDaysBeforePermanentDelete = nullable7;
      if (!updateLength)
        return;
      blobAttributes.Properties.Length = properties1.Length;
    }

    internal static Uri SourceBlobToUri(CloudBlob source)
    {
      CommonUtility.AssertNotNull(nameof (source), (object) source);
      return source.ServiceClient.Credentials.TransformUri(source.SnapshotQualifiedUri);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetAccountProperties(
      AsyncCallback callback,
      object state)
    {
      return this.BeginGetAccountProperties((BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetAccountProperties(
      BlobRequestOptions requestOptions,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      requestOptions = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return CancellableAsyncResultTaskWrapper.Create<AccountProperties>((Func<CancellationToken, Task<AccountProperties>>) (token => this.GetAccountPropertiesAsync(requestOptions, operationContext)), callback, state);
    }

    public virtual AccountProperties EndGetAccountProperties(IAsyncResult asyncResult)
    {
      CommonUtility.AssertNotNull(nameof (asyncResult), (object) asyncResult);
      return ((CancellableAsyncResultTaskWrapper<AccountProperties>) asyncResult).GetAwaiter().GetResult();
    }

    [DoesServiceRequest]
    public virtual Task<AccountProperties> GetAccountPropertiesAsync() => this.GetAccountPropertiesAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<AccountProperties> GetAccountPropertiesAsync(
      CancellationToken cancellationToken)
    {
      return this.GetAccountPropertiesAsync((BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<AccountProperties> GetAccountPropertiesAsync(
      BlobRequestOptions requestOptions,
      OperationContext operationContext)
    {
      return this.GetAccountPropertiesAsync(requestOptions, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<AccountProperties> GetAccountPropertiesAsync(
      BlobRequestOptions requestOptions,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      requestOptions = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<AccountProperties>(this.GetAccountPropertiesImpl(requestOptions), requestOptions.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual AccountProperties GetAccountProperties(
      BlobRequestOptions requestOptions = null,
      OperationContext operationContext = null)
    {
      requestOptions = BlobRequestOptions.ApplyDefaults(requestOptions, BlobType.Unspecified, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<AccountProperties>(this.GetAccountPropertiesImpl(requestOptions), requestOptions.RetryPolicy, operationContext);
    }

    private RESTCommand<AccountProperties> GetAccountPropertiesImpl(
      BlobRequestOptions requestOptions)
    {
      RESTCommand<AccountProperties> cmd1 = new RESTCommand<AccountProperties>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<AccountProperties>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.GetAccountProperties(uri, builder, serverTimeout, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.RetrieveResponseStream = true;
      cmd1.PreProcessResponse = (Func<RESTCommand<AccountProperties>, HttpResponseMessage, Exception, OperationContext, AccountProperties>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<AccountProperties>(HttpStatusCode.OK, resp, (AccountProperties) null, (StorageCommandBase<AccountProperties>) cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<AccountProperties>, HttpResponseMessage, OperationContext, CancellationToken, Task<AccountProperties>>) ((cmd, resp, ctx, ct) => Task.FromResult<AccountProperties>(BlobHttpResponseParsers.ReadAccountProperties(resp)));
      requestOptions.ApplyToStorageCommand<AccountProperties>(cmd1);
      return cmd1;
    }

    public CloudBlob(Uri blobAbsoluteUri)
      : this(blobAbsoluteUri, (StorageCredentials) null)
    {
    }

    public CloudBlob(Uri blobAbsoluteUri, StorageCredentials credentials)
      : this(blobAbsoluteUri, new DateTimeOffset?(), credentials)
    {
    }

    public CloudBlob(Uri blobAbsoluteUri, CloudBlobClient client)
      : this(blobAbsoluteUri, new DateTimeOffset?(), client)
    {
    }

    public CloudBlob(
      Uri blobAbsoluteUri,
      DateTimeOffset? snapshotTime,
      StorageCredentials credentials)
      : this(new StorageUri(blobAbsoluteUri), snapshotTime, credentials)
    {
    }

    public CloudBlob(Uri blobAbsoluteUri, DateTimeOffset? snapshotTime, CloudBlobClient client)
      : this(new StorageUri(blobAbsoluteUri), snapshotTime, client)
    {
    }

    public CloudBlob(
      StorageUri blobAbsoluteUri,
      DateTimeOffset? snapshotTime,
      StorageCredentials credentials)
    {
      CommonUtility.AssertNotNull(nameof (blobAbsoluteUri), (object) blobAbsoluteUri);
      CommonUtility.AssertNotNull(nameof (blobAbsoluteUri), (object) blobAbsoluteUri.PrimaryUri);
      this.attributes = new BlobAttributes();
      this.SnapshotTime = snapshotTime;
      this.ParseQueryAndVerify(blobAbsoluteUri, credentials);
      this.Properties.BlobType = BlobType.Unspecified;
    }

    public CloudBlob(
      StorageUri blobAbsoluteUri,
      DateTimeOffset? snapshotTime,
      CloudBlobClient client)
    {
      CommonUtility.AssertNotNull(nameof (blobAbsoluteUri), (object) blobAbsoluteUri);
      CommonUtility.AssertNotNull(nameof (blobAbsoluteUri), (object) blobAbsoluteUri.PrimaryUri);
      this.attributes = new BlobAttributes();
      this.SnapshotTime = snapshotTime;
      this.ServiceClient = client;
      this.ParseQueryAndVerify(blobAbsoluteUri, client.Credentials);
      this.Properties.BlobType = BlobType.Unspecified;
    }

    internal CloudBlob(string blobName, DateTimeOffset? snapshotTime, CloudBlobContainer container)
    {
      CommonUtility.AssertNotNullOrEmpty(nameof (blobName), blobName);
      CommonUtility.AssertNotNull(nameof (container), (object) container);
      this.attributes = new BlobAttributes();
      this.attributes.StorageUri = NavigationHelper.AppendPathToUri(container.StorageUri, blobName);
      this.Name = blobName;
      this.ServiceClient = container.ServiceClient;
      this.container = container;
      this.SnapshotTime = snapshotTime;
      this.Properties.BlobType = BlobType.Unspecified;
    }

    internal CloudBlob(BlobAttributes attributes, CloudBlobClient serviceClient)
    {
      this.attributes = attributes;
      this.ServiceClient = serviceClient;
      this.ParseQueryAndVerify(this.StorageUri, this.ServiceClient.Credentials);
      this.Properties.BlobType = BlobType.Unspecified;
    }

    public CloudBlobClient ServiceClient { get; private set; }

    public int StreamMinimumReadSizeInBytes
    {
      get => this.streamMinimumReadSizeInBytes;
      set
      {
        CommonUtility.AssertInBounds<long>(nameof (StreamMinimumReadSizeInBytes), (long) value, 16384L);
        this.streamMinimumReadSizeInBytes = value;
      }
    }

    public BlobProperties Properties => this.attributes.Properties;

    public IDictionary<string, string> Metadata => this.attributes.Metadata;

    public Uri Uri => this.attributes.Uri;

    public StorageUri StorageUri => this.attributes.StorageUri;

    public DateTimeOffset? SnapshotTime
    {
      get => this.attributes.SnapshotTime;
      private set => this.attributes.SnapshotTime = value;
    }

    public bool IsSnapshot => this.SnapshotTime.HasValue;

    public bool IsDeleted
    {
      get => this.attributes.IsDeleted;
      private set => this.attributes.IsDeleted = value;
    }

    public Uri SnapshotQualifiedUri
    {
      get
      {
        if (!this.SnapshotTime.HasValue)
          return this.Uri;
        UriQueryBuilder uriQueryBuilder = new UriQueryBuilder();
        uriQueryBuilder.Add("snapshot", Request.ConvertDateTimeToSnapshotString(this.SnapshotTime.Value));
        return uriQueryBuilder.AddToUri(this.Uri);
      }
    }

    public StorageUri SnapshotQualifiedStorageUri
    {
      get
      {
        if (!this.SnapshotTime.HasValue)
          return this.StorageUri;
        UriQueryBuilder uriQueryBuilder = new UriQueryBuilder();
        uriQueryBuilder.Add("snapshot", Request.ConvertDateTimeToSnapshotString(this.SnapshotTime.Value));
        return uriQueryBuilder.AddToUri(this.StorageUri);
      }
    }

    public virtual CopyState CopyState => this.attributes.CopyState;

    public virtual string Name { get; private set; }

    public CloudBlobContainer Container
    {
      get
      {
        if (this.container == null)
          this.container = this.ServiceClient.GetContainerReference(NavigationHelper.GetContainerName(this.Uri, new bool?(this.ServiceClient.UsePathStyleUris)));
        return this.container;
      }
    }

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

    public BlobType BlobType
    {
      get => this.Properties.BlobType;
      internal set => this.Properties.BlobType = value;
    }

    public string GetSharedAccessSignature(SharedAccessBlobPolicy policy) => this.GetSharedAccessSignature(policy, (SharedAccessBlobHeaders) null, (string) null);

    public string GetSharedAccessSignature(
      SharedAccessBlobPolicy policy,
      string groupPolicyIdentifier)
    {
      return this.GetSharedAccessSignature(policy, (SharedAccessBlobHeaders) null, groupPolicyIdentifier);
    }

    public string GetSharedAccessSignature(
      SharedAccessBlobPolicy policy,
      SharedAccessBlobHeaders headers)
    {
      return this.GetSharedAccessSignature(policy, headers, (string) null);
    }

    public string GetSharedAccessSignature(
      SharedAccessBlobPolicy policy,
      SharedAccessBlobHeaders headers,
      string groupPolicyIdentifier)
    {
      return this.GetSharedAccessSignature(policy, headers, groupPolicyIdentifier, new SharedAccessProtocol?(), (IPAddressOrRange) null);
    }

    public string GetSharedAccessSignature(
      SharedAccessBlobPolicy policy,
      SharedAccessBlobHeaders headers,
      string groupPolicyIdentifier,
      SharedAccessProtocol? protocols,
      IPAddressOrRange ipAddressOrRange)
    {
      if (!this.ServiceClient.Credentials.IsSharedKey)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Cannot create Shared Access Signature unless Account Key credentials are used."));
      string str = this.IsSnapshot ? "bs" : "b";
      string canonicalName = this.GetCanonicalName(true);
      StorageAccountKey key = this.ServiceClient.Credentials.Key;
      string hash = BlobSharedAccessSignatureHelper.GetHash(policy, headers, groupPolicyIdentifier, canonicalName, "2019-07-07", protocols, ipAddressOrRange, key.KeyValue, str, this.SnapshotTime);
      return BlobSharedAccessSignatureHelper.GetSignature(policy, headers, groupPolicyIdentifier, str, hash, key.KeyName, "2019-07-07", protocols, ipAddressOrRange).ToString();
    }

    public string GetUserDelegationSharedAccessSignature(
      UserDelegationKey delegationKey,
      SharedAccessBlobPolicy policy,
      SharedAccessBlobHeaders headers = null,
      SharedAccessProtocol? protocols = null,
      IPAddressOrRange ipAddressOrRange = null)
    {
      string canonicalName = this.GetCanonicalName(true);
      string str = this.IsSnapshot ? "bs" : "b";
      string hash = BlobSharedAccessSignatureHelper.GetHash(policy, headers, canonicalName, "2019-07-07", str, this.SnapshotTime, protocols, ipAddressOrRange, delegationKey);
      return BlobSharedAccessSignatureHelper.GetSignature(policy, headers, (string) null, str, hash, (string) null, "2019-07-07", protocols, ipAddressOrRange, delegationKey).ToString();
    }

    private string GetCanonicalName(bool ignoreSnapshotTime)
    {
      string canonicalName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/{1}/{2}/{3}", (object) "blob", (object) (this.ServiceClient.Credentials.AccountName ?? NavigationHelper.GetAccountNameFromUri(this.ServiceClient.BaseUri, new bool?())), (object) this.Container.Name, (object) this.Name.Replace('\\', '/'));
      if (!ignoreSnapshotTime)
      {
        DateTimeOffset? snapshotTime = this.SnapshotTime;
        if (snapshotTime.HasValue)
        {
          string str = canonicalName;
          snapshotTime = this.SnapshotTime;
          string snapshotString = Request.ConvertDateTimeToSnapshotString(snapshotTime.Value);
          canonicalName = str + "?snapshot=" + snapshotString;
        }
      }
      return canonicalName;
    }

    private void ParseQueryAndVerify(StorageUri address, StorageCredentials credentials)
    {
      StorageCredentials parsedCredentials;
      DateTimeOffset? parsedSnapshot;
      this.attributes.StorageUri = NavigationHelper.ParseBlobQueryAndVerify(address, out parsedCredentials, out parsedSnapshot);
      if (parsedCredentials != null && credentials != null && !credentials.Equals(new StorageCredentials()))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot provide credentials as part of the address and as constructor parameter. Either pass in the address or use a different constructor."));
      if (parsedSnapshot.HasValue && this.SnapshotTime.HasValue && !parsedSnapshot.Value.Equals(this.SnapshotTime.Value))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Multiple different snapshot times provided as part of query '{0}' and as constructor parameter '{1}'.", (object) parsedSnapshot, (object) this.SnapshotTime));
      if (parsedSnapshot.HasValue)
        this.SnapshotTime = parsedSnapshot;
      if (this.ServiceClient == null)
        this.ServiceClient = new CloudBlobClient(NavigationHelper.GetServiceClientBaseAddress(this.StorageUri, new bool?()), credentials ?? parsedCredentials);
      this.Name = NavigationHelper.GetBlobName(this.Uri, new bool?(this.ServiceClient.UsePathStyleUris));
    }

    private struct WrappedKeyData
    {
      public byte[] encryptedKey;
      public string algorithm;
      public BlobEncryptionData encryptionData;
    }
  }
}
