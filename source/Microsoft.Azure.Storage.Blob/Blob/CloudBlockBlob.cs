// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.CloudBlockBlob
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob.Protocol;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Blob
{
  public class CloudBlockBlob : CloudBlob, ICloudBlob, IListBlobItem
  {
    private int streamWriteSizeInBytes = 4194304;
    private bool isStreamWriteSizeModified;

    [DoesServiceRequest]
    public virtual CloudBlobStream OpenWrite(
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, this.BlobType, this.ServiceClient, false);
      if (accessCondition != null)
      {
        if (accessCondition.IsConditional)
        {
          try
          {
            this.FetchAttributes(accessCondition.Clone().RemoveIsIfNotExistsCondition(), options, operationContext);
            if (accessCondition.IsIfNotExists)
              throw CloudBlockBlob.GenerateExceptionForConflictFailure();
          }
          catch (StorageException ex)
          {
            AccessCondition accessCondition1 = accessCondition;
            if (!CloudBlockBlob.ContinueOpenWriteOnFailure(ex, accessCondition1))
              throw;
          }
        }
      }
      options1.AssertPolicyIfRequired();
      if (options1.EncryptionPolicy == null)
        return (CloudBlobStream) new BlobWriteStream(this, accessCondition, options1, operationContext);
      ICryptoTransform encryptionContext = options1.EncryptionPolicy.CreateAndSetEncryptionContext(this.Metadata, false);
      return (CloudBlobStream) new BlobEncryptedWriteStream(this, accessCondition, options1, operationContext, encryptionContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginOpenWrite(AsyncCallback callback, object state) => this.BeginOpenWrite((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginOpenWrite(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<CloudBlobStream>((Func<CancellationToken, Task<CloudBlobStream>>) (token => this.OpenWriteAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual CloudBlobStream EndOpenWrite(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<CloudBlobStream>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<CloudBlobStream> OpenWriteAsync() => this.OpenWriteAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<CloudBlobStream> OpenWriteAsync(CancellationToken cancellationToken) => this.OpenWriteAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<CloudBlobStream> OpenWriteAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.OpenWriteAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual async Task<CloudBlobStream> OpenWriteAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CloudBlockBlob blockBlob = this;
      blockBlob.attributes.AssertNoSnapshot();
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      BlobRequestOptions modifiedOptions = BlobRequestOptions.ApplyDefaults(options, __nonvirtual (blockBlob.BlobType), __nonvirtual (blockBlob.ServiceClient), false);
      if (accessCondition != null && accessCondition.IsConditional)
      {
        try
        {
          await blockBlob.FetchAttributesAsync(accessCondition.Clone().RemoveIsIfNotExistsCondition(), options, operationContext, cancellationToken).ConfigureAwait(false);
          if (accessCondition.IsIfNotExists)
            throw CloudBlockBlob.GenerateExceptionForConflictFailure();
        }
        catch (StorageException ex)
        {
          AccessCondition accessCondition1 = accessCondition;
          if (!CloudBlockBlob.ContinueOpenWriteOnFailure(ex, accessCondition1))
            throw;
        }
      }
      modifiedOptions.AssertPolicyIfRequired();
      if (modifiedOptions.EncryptionPolicy == null)
        return (CloudBlobStream) new BlobWriteStream(blockBlob, accessCondition, modifiedOptions, operationContext);
      // ISSUE: explicit non-virtual call
      ICryptoTransform encryptionContext = modifiedOptions.EncryptionPolicy.CreateAndSetEncryptionContext(__nonvirtual (blockBlob.Metadata), false);
      return (CloudBlobStream) new BlobEncryptedWriteStream(blockBlob, accessCondition, modifiedOptions, operationContext, encryptionContext);
    }

    [DoesServiceRequest]
    public virtual void UploadFromStream(
      Stream source,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.UploadFromStreamHelper(source, new long?(), accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual void UploadFromStream(
      Stream source,
      long length,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.UploadFromStreamHelper(source, new long?(length), accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    internal void UploadFromStreamHelper(
      Stream source,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      CommonUtility.AssertNotNull(nameof (source), (object) source);
      if (length.HasValue)
      {
        CommonUtility.AssertInBounds<long>(nameof (length), length.Value, 1L);
        if (source.CanSeek)
        {
          long? nullable = length;
          long num = source.Length - source.Position;
          if (nullable.GetValueOrDefault() > num & nullable.HasValue)
            throw new ArgumentOutOfRangeException(nameof (length), "The requested number of bytes exceeds the length of the stream remaining from the specified position.");
        }
      }
      long? nullable1 = length;
      this.CheckAdjustBlockSize(nullable1 ?? (source.CanSeek ? new long?(source.Length - source.Position) : length));
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions modifiedOptions = BlobRequestOptions.ApplyDefaults(options, BlobType.BlockBlob, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      bool flag1 = CloudBlockBlob.IsLessThanSingleBlobThreshold(source, length, modifiedOptions, false);
      modifiedOptions.AssertPolicyIfRequired();
      if (modifiedOptions.ParallelOperationThreadCount.Value == 1 & flag1)
      {
        bool flag2 = modifiedOptions.EncryptionPolicy != null;
        Stream stream1 = source;
        using (MemoryStream memoryStream = !flag2 ? (MemoryStream) null : new MemoryStream())
        {
          if (flag2)
          {
            modifiedOptions.AssertPolicyIfRequired();
            if (modifiedOptions.EncryptionPolicy.EncryptionMode != BlobEncryptionMode.FullBlob)
              throw new InvalidOperationException("Invalid BlobEncryptionMode set on the policy. Please set it to FullBlob when the policy is used with UploadFromStream.", (Exception) null);
            ICryptoTransform encryptionContext = modifiedOptions.EncryptionPolicy.CreateAndSetEncryptionContext(this.Metadata, false);
            CryptoStream cryptoStream = new CryptoStream((Stream) memoryStream, encryptionContext, CryptoStreamMode.Write);
            using (ExecutionState<NullType> temporaryExecutionState = BlobCommonUtility.CreateTemporaryExecutionState(options))
            {
              Stream stream2 = source;
              CryptoStream toStream = cryptoStream;
              long? copyLength = length;
              nullable1 = new long?();
              long? maxLength = nullable1;
              ChecksumRequested none = ChecksumRequested.None;
              ExecutionState<NullType> executionState = temporaryExecutionState;
              stream2.WriteToSync<NullType>((Stream) toStream, copyLength, maxLength, none, true, executionState, (StreamDescriptor) null);
              cryptoStream.FlushFinalBlock();
            }
            memoryStream.Seek(0L, SeekOrigin.Begin);
            length = new long?(memoryStream.Length);
            stream1 = (Stream) memoryStream;
          }
          Checksum none1 = Checksum.None;
          bool? nullable2 = modifiedOptions.ChecksumOptions.StoreContentMD5;
          if (nullable2.Value)
          {
            using (ExecutionState<NullType> temporaryExecutionState = BlobCommonUtility.CreateTemporaryExecutionState(options))
            {
              StreamDescriptor streamDescriptor = new StreamDescriptor();
              long position = stream1.Position;
              Stream stream3 = stream1;
              Stream toStream = Stream.Null;
              long? copyLength = length;
              nullable1 = new long?();
              long? maxLength = nullable1;
              ChecksumRequested calculateChecksum = new ChecksumRequested(true, false);
              ExecutionState<NullType> executionState = temporaryExecutionState;
              StreamDescriptor streamCopyState = streamDescriptor;
              stream3.WriteToSync<NullType>(toStream, copyLength, maxLength, calculateChecksum, true, executionState, streamCopyState);
              stream1.Position = position;
              none1.MD5 = streamDescriptor.Md5;
            }
          }
          else
          {
            nullable2 = modifiedOptions.ChecksumOptions.UseTransactionalMD5;
            if (nullable2.Value)
              throw new ArgumentException("When uploading a blob in a single request, StoreBlobContentMD5 must be set to true if UseTransactionalMD5 is true, because the MD5 calculated for the transaction will be stored in the blob.", nameof (options));
          }
          Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.PutBlobImpl(stream1, length, none1, accessCondition, modifiedOptions), modifiedOptions.RetryPolicy, operationContext);
        }
      }
      else if ((modifiedOptions.EncryptionPolicy != null || !source.CanSeek || this.streamWriteSizeInBytes < 4194305 || modifiedOptions.ChecksumOptions.StoreContentMD5.HasValue && modifiedOptions.ChecksumOptions.StoreContentMD5.Value ? 1 : (!modifiedOptions.ChecksumOptions.StoreContentCRC64.HasValue ? 0 : (modifiedOptions.ChecksumOptions.StoreContentCRC64.Value ? 1 : 0))) != 0)
      {
        using (CloudBlobStream cloudBlobStream = this.OpenWrite(accessCondition, modifiedOptions, operationContext))
        {
          using (ExecutionState<NullType> temporaryExecutionState = BlobCommonUtility.CreateTemporaryExecutionState(modifiedOptions))
          {
            Stream stream = source;
            CloudBlobStream toStream = cloudBlobStream;
            long? copyLength = length;
            nullable1 = new long?();
            long? maxLength = nullable1;
            ChecksumRequested none = ChecksumRequested.None;
            ExecutionState<NullType> executionState = temporaryExecutionState;
            stream.WriteToSync<NullType>((Stream) toStream, copyLength, maxLength, none, true, executionState, (StreamDescriptor) null);
            cloudBlobStream.Commit();
          }
        }
      }
      else
      {
        SemaphoreSlim streamReadThrottler = new SemaphoreSlim(1);
        CommonUtility.RunWithoutSynchronizationContext((Action) (() => this.UploadFromMultiStreamAsync(this.OpenMultiSubStream(source, length, streamReadThrottler), accessCondition, modifiedOptions, operationContext, (AggregatingProgressIncrementer) null, CancellationToken.None).GetAwaiter().GetResult()));
      }
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      AsyncCallback callback,
      object state)
    {
      return this.BeginUploadFromStreamHelper(source, new long?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginUploadFromStreamHelper(source, new long?(), accessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return this.BeginUploadFromStreamHelper(source, new long?(), accessCondition, options, operationContext, progressHandler, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      long length,
      AsyncCallback callback,
      object state)
    {
      return this.BeginUploadFromStreamHelper(source, new long?(length), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginUploadFromStreamHelper(source, new long?(length), accessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return this.BeginUploadFromStreamHelper(source, new long?(length), accessCondition, options, operationContext, progressHandler, callback, state);
    }

    internal ICancellableAsyncResult BeginUploadFromStreamHelper(
      Stream source,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginUploadFromStreamHelper(source, length, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, callback, state);
    }

    private ICancellableAsyncResult BeginUploadFromStreamHelper(
      Stream source,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsyncHelper(source, length, accessCondition, options, operationContext, new AggregatingProgressIncrementer(progressHandler), token)), callback, state);
    }

    public virtual void EndUploadFromStream(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(Stream source) => this.UploadFromStreamAsyncHelper(source, new long?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(Stream source, CancellationToken cancellationToken) => this.UploadFromStreamAsyncHelper(source, new long?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(), accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(), accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(), accessCondition, options, operationContext, new AggregatingProgressIncrementer(progressHandler), cancellationToken);
    }

    [DoesServiceRequest]
    internal virtual Task UploadFromStreamAsync(
      Stream source,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AggregatingProgressIncrementer progressIncrementer,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(), accessCondition, options, operationContext, progressIncrementer, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(Stream source, long length) => this.UploadFromStreamAsyncHelper(source, new long?(length), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      long length,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(length), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(length), accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(length), accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(length), accessCondition, options, operationContext, new AggregatingProgressIncrementer(progressHandler), cancellationToken);
    }

    [DoesServiceRequest]
    private async Task UploadFromStreamAsyncHelper(
      Stream source,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AggregatingProgressIncrementer progressIncrementer,
      CancellationToken cancellationToken)
    {
      CloudBlockBlob cloudBlockBlob1 = this;
      CommonUtility.AssertNotNull(nameof (source), (object) source);
      long? nullable1;
      if (length.HasValue)
      {
        CommonUtility.AssertInBounds<long>(nameof (length), length.Value, 1L);
        if (source.CanSeek)
        {
          nullable1 = length;
          long num = source.Length - source.Position;
          if (nullable1.GetValueOrDefault() > num & nullable1.HasValue)
            throw new ArgumentOutOfRangeException(nameof (length), "The requested number of bytes exceeds the length of the stream remaining from the specified position.");
        }
      }
      progressIncrementer = progressIncrementer ?? AggregatingProgressIncrementer.None;
      CloudBlockBlob cloudBlockBlob2 = cloudBlockBlob1;
      nullable1 = length;
      long? streamLength = nullable1 ?? (source.CanSeek ? new long?(source.Length - source.Position) : length);
      cloudBlockBlob2.CheckAdjustBlockSize(streamLength);
      cloudBlockBlob1.attributes.AssertNoSnapshot();
      // ISSUE: explicit non-virtual call
      BlobRequestOptions modifiedOptions = BlobRequestOptions.ApplyDefaults(options, BlobType.BlockBlob, __nonvirtual (cloudBlockBlob1.ServiceClient));
      operationContext = operationContext ?? new OperationContext();
      bool flag1 = CloudBlockBlob.IsLessThanSingleBlobThreshold(source, length, modifiedOptions, false);
      modifiedOptions.AssertPolicyIfRequired();
      ExecutionState<NullType> tempExecutionState;
      if (modifiedOptions.ParallelOperationThreadCount.Value == 1 & flag1)
      {
        bool flag2 = modifiedOptions.EncryptionPolicy != null;
        Stream sourceStream = source;
        using (MemoryStream tempStream = !flag2 ? (MemoryStream) null : new MemoryStream())
        {
          if (flag2)
          {
            modifiedOptions.AssertPolicyIfRequired();
            if (modifiedOptions.EncryptionPolicy.EncryptionMode != BlobEncryptionMode.FullBlob)
              throw new InvalidOperationException("Invalid BlobEncryptionMode set on the policy. Please set it to FullBlob when the policy is used with UploadFromStream.", (Exception) null);
            // ISSUE: explicit non-virtual call
            CryptoStream cryptoStream = new CryptoStream((Stream) tempStream, modifiedOptions.EncryptionPolicy.CreateAndSetEncryptionContext(__nonvirtual (cloudBlockBlob1.Metadata), false), CryptoStreamMode.Write);
            tempExecutionState = BlobCommonUtility.CreateTemporaryExecutionState(options);
            try
            {
              Stream stream = source;
              CryptoStream toStream = cryptoStream;
              // ISSUE: explicit non-virtual call
              IBufferManager bufferManager = __nonvirtual (cloudBlockBlob1.ServiceClient).BufferManager;
              long? copyLength = length;
              nullable1 = new long?();
              long? maxLength = nullable1;
              ChecksumRequested none = ChecksumRequested.None;
              ExecutionState<NullType> executionState = tempExecutionState;
              CancellationToken cancellationToken1 = cancellationToken;
              await stream.WriteToAsync<NullType>((Stream) toStream, bufferManager, copyLength, maxLength, none, executionState, (StreamDescriptor) null, cancellationToken1).ConfigureAwait(false);
              cryptoStream.FlushFinalBlock();
            }
            finally
            {
              tempExecutionState?.Dispose();
            }
            tempExecutionState = (ExecutionState<NullType>) null;
            tempStream.Seek(0L, SeekOrigin.Begin);
            length = new long?(tempStream.Length);
            sourceStream = (Stream) tempStream;
            cryptoStream = (CryptoStream) null;
          }
          Checksum contentChecksum = Checksum.None;
          bool? nullable2 = modifiedOptions.ChecksumOptions.StoreContentMD5;
          if (nullable2.Value)
          {
            tempExecutionState = BlobCommonUtility.CreateTemporaryExecutionState(modifiedOptions);
            try
            {
              StreamDescriptor streamCopyState = new StreamDescriptor();
              long startPosition = sourceStream.Position;
              // ISSUE: explicit non-virtual call
              await sourceStream.WriteToAsync<NullType>(Stream.Null, __nonvirtual (cloudBlockBlob1.ServiceClient).BufferManager, length, new long?(), new ChecksumRequested(true, true), tempExecutionState, streamCopyState, cancellationToken).ConfigureAwait(false);
              sourceStream.Position = startPosition;
              contentChecksum.MD5 = streamCopyState.Md5;
              streamCopyState = (StreamDescriptor) null;
            }
            finally
            {
              tempExecutionState?.Dispose();
            }
            tempExecutionState = (ExecutionState<NullType>) null;
          }
          else
          {
            nullable2 = modifiedOptions.ChecksumOptions.UseTransactionalMD5;
            if (nullable2.Value)
              throw new ArgumentException("When uploading a blob in a single request, StoreBlobContentMD5 must be set to true if UseTransactionalMD5 is true, because the MD5 calculated for the transaction will be stored in the blob.", nameof (options));
          }
          NullType nullType = await Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(cloudBlockBlob1.PutBlobImpl(progressIncrementer.CreateProgressIncrementingStream(sourceStream), length, contentChecksum, accessCondition, modifiedOptions), modifiedOptions.RetryPolicy, operationContext, cancellationToken).ConfigureAwait(false);
          contentChecksum = (Checksum) null;
        }
        sourceStream = (Stream) null;
        modifiedOptions = (BlobRequestOptions) null;
      }
      else
      {
        int num;
        if (modifiedOptions.EncryptionPolicy == null && source.CanSeek && cloudBlockBlob1.streamWriteSizeInBytes >= 4194305)
        {
          bool? nullable3 = modifiedOptions.ChecksumOptions.StoreContentMD5;
          if (nullable3.HasValue)
          {
            nullable3 = modifiedOptions.ChecksumOptions.StoreContentMD5;
            if (nullable3.Value)
              goto label_38;
          }
          nullable3 = modifiedOptions.ChecksumOptions.StoreContentCRC64;
          if (nullable3.HasValue)
          {
            nullable3 = modifiedOptions.ChecksumOptions.StoreContentCRC64;
            num = nullable3.Value ? 1 : 0;
            goto label_39;
          }
          else
          {
            num = 0;
            goto label_39;
          }
        }
label_38:
        num = 1;
label_39:
        if (num != 0)
        {
          using (CloudBlobStream blobStream = cloudBlockBlob1.OpenWrite(accessCondition, modifiedOptions, operationContext))
          {
            tempExecutionState = BlobCommonUtility.CreateTemporaryExecutionState(modifiedOptions);
            try
            {
              Stream stream = source;
              Stream incrementingStream = progressIncrementer.CreateProgressIncrementingStream((Stream) blobStream);
              // ISSUE: explicit non-virtual call
              IBufferManager bufferManager = __nonvirtual (cloudBlockBlob1.ServiceClient).BufferManager;
              long? copyLength = length;
              nullable1 = new long?();
              long? maxLength = nullable1;
              ChecksumRequested none = ChecksumRequested.None;
              ExecutionState<NullType> executionState = tempExecutionState;
              CancellationToken cancellationToken2 = cancellationToken;
              ConfiguredTaskAwaitable configuredTaskAwaitable = stream.WriteToAsync<NullType>(incrementingStream, bufferManager, copyLength, maxLength, none, executionState, (StreamDescriptor) null, cancellationToken2).ConfigureAwait(false);
              await configuredTaskAwaitable;
              configuredTaskAwaitable = blobStream.CommitAsync().ConfigureAwait(false);
              await configuredTaskAwaitable;
            }
            finally
            {
              tempExecutionState?.Dispose();
            }
            tempExecutionState = (ExecutionState<NullType>) null;
          }
          modifiedOptions = (BlobRequestOptions) null;
        }
        else
        {
          SemaphoreSlim mutex = new SemaphoreSlim(1);
          await cloudBlockBlob1.UploadFromMultiStreamAsync(cloudBlockBlob1.OpenMultiSubStream(source, length, mutex), accessCondition, modifiedOptions, operationContext, progressIncrementer, cancellationToken).ConfigureAwait(false);
          modifiedOptions = (BlobRequestOptions) null;
        }
      }
    }

    [DoesServiceRequest]
    public virtual void UploadFromFile(
      string path,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (path), (object) path);
      BlobRequestOptions modifiedOptions = BlobRequestOptions.ApplyDefaults(options, BlobType.BlockBlob, this.ServiceClient);
      if ((modifiedOptions.ChecksumOptions.StoreContentMD5.HasValue && modifiedOptions.ChecksumOptions.StoreContentMD5.Value || modifiedOptions.ChecksumOptions.StoreContentCRC64.HasValue && modifiedOptions.ChecksumOptions.StoreContentCRC64.Value || modifiedOptions.EncryptionPolicy != null ? 1 : (this.streamWriteSizeInBytes < 4194305 ? 1 : 0)) != 0)
      {
        using (FileStream source = new FileStream(path, FileMode.Open, FileAccess.Read))
          this.UploadFromStream((Stream) source, accessCondition, modifiedOptions, operationContext);
      }
      else
      {
        this.CheckAdjustBlockSize(new long?(new FileInfo(path).Length));
        CommonUtility.RunWithoutSynchronizationContext((Action) (() => this.UploadFromMultiStreamAsync(this.OpenMultiFileStream(path), accessCondition, modifiedOptions, operationContext, (AggregatingProgressIncrementer) null, CancellationToken.None).Wait()));
      }
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromFile(
      string path,
      AsyncCallback callback,
      object state)
    {
      return this.BeginUploadFromFile(path, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromFile(
      string path,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginUploadFromFile(path, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginUploadFromFile(
      string path,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromFileAsync(path, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndUploadFromFile(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task UploadFromFileAsync(string path) => this.UploadFromFileAsync(path, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task UploadFromFileAsync(string path, CancellationToken cancellationToken) => this.UploadFromFileAsync(path, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task UploadFromFileAsync(
      string path,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.UploadFromFileAsync(path, accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromFileAsync(
      string path,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadFromFileAsync(path, accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromFileAsync(
      string path,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.UploadFromFileAsync(path, accessCondition, options, operationContext, new AggregatingProgressIncrementer(progressHandler), cancellationToken);
    }

    [DoesServiceRequest]
    internal virtual async Task UploadFromFileAsync(
      string path,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AggregatingProgressIncrementer progressIncrementer,
      CancellationToken cancellationToken)
    {
      CloudBlockBlob cloudBlockBlob = this;
      CommonUtility.AssertNotNull(nameof (path), (object) path);
      // ISSUE: explicit non-virtual call
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.BlockBlob, __nonvirtual (cloudBlockBlob.ServiceClient));
      bool? nullable = options1.ChecksumOptions.StoreContentMD5;
      if (nullable.HasValue)
      {
        nullable = options1.ChecksumOptions.StoreContentMD5;
        if (nullable.Value)
          goto label_6;
      }
      nullable = options1.ChecksumOptions.StoreContentCRC64;
      if (nullable.HasValue)
      {
        nullable = options1.ChecksumOptions.StoreContentCRC64;
        if (nullable.Value)
          goto label_6;
      }
      int num;
      if (options1.EncryptionPolicy == null)
      {
        num = cloudBlockBlob.streamWriteSizeInBytes < 4194305 ? 1 : 0;
        goto label_7;
      }
label_6:
      num = 1;
label_7:
      if (num != 0)
      {
        using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
          await cloudBlockBlob.UploadFromStreamAsync((Stream) fileStream, accessCondition, options1, operationContext, progressIncrementer, cancellationToken).ConfigureAwait(false);
      }
      else
      {
        cloudBlockBlob.CheckAdjustBlockSize(new long?(new FileInfo(path).Length));
        await cloudBlockBlob.UploadFromMultiStreamAsync(cloudBlockBlob.OpenMultiFileStream(path), accessCondition, options1, operationContext, progressIncrementer, cancellationToken).ConfigureAwait(false);
      }
    }

    [DoesServiceRequest]
    public virtual void UploadFromByteArray(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (buffer), (object) buffer);
      using (SyncMemoryStream source = new SyncMemoryStream(buffer, index, count))
        this.UploadFromStream((Stream) source, accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromByteArray(
      byte[] buffer,
      int index,
      int count,
      AsyncCallback callback,
      object state)
    {
      return this.BeginUploadFromByteArray(buffer, index, count, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromByteArray(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginUploadFromByteArray(buffer, index, count, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginUploadFromByteArray(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromByteArrayAsync(buffer, index, count, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual void EndUploadFromByteArray(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task UploadFromByteArrayAsync(byte[] buffer, int index, int count) => this.UploadFromByteArrayAsync(buffer, index, count, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task UploadFromByteArrayAsync(
      byte[] buffer,
      int index,
      int count,
      CancellationToken cancellationToken)
    {
      return this.UploadFromByteArrayAsync(buffer, index, count, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromByteArrayAsync(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.UploadFromByteArrayAsync(buffer, index, count, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromByteArrayAsync(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadFromByteArrayAsync(buffer, index, count, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task UploadFromByteArrayAsync(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (buffer), (object) buffer);
      using (SyncMemoryStream stream = new SyncMemoryStream(buffer, index, count))
        await this.UploadFromStreamAsync((Stream) stream, accessCondition, options, operationContext, progressHandler, cancellationToken).ConfigureAwait(false);
    }

    [DoesServiceRequest]
    public virtual void UploadText(
      string content,
      Encoding encoding = null,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (content), (object) content);
      byte[] bytes = (encoding ?? Encoding.UTF8).GetBytes(content);
      this.UploadFromByteArray(bytes, 0, bytes.Length, accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadText(
      string content,
      AsyncCallback callback,
      object state)
    {
      return this.BeginUploadText(content, (Encoding) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadText(
      string content,
      Encoding encoding,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginUploadText(content, encoding, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginUploadText(
      string content,
      Encoding encoding,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadTextAsync(content, encoding, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual void EndUploadText(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task UploadTextAsync(string content) => this.UploadTextAsync(content, (Encoding) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task UploadTextAsync(string content, CancellationToken cancellationToken) => this.UploadTextAsync(content, (Encoding) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task UploadTextAsync(
      string content,
      Encoding encoding,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.UploadTextAsync(content, encoding, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task UploadTextAsync(
      string content,
      Encoding encoding,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadTextAsync(content, encoding, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task UploadTextAsync(
      string content,
      Encoding encoding,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (content), (object) content);
      byte[] bytes = (encoding ?? Encoding.UTF8).GetBytes(content);
      await this.UploadFromByteArrayAsync(bytes, 0, bytes.Length, accessCondition, options, operationContext, progressHandler, cancellationToken).ConfigureAwait(false);
    }

    [DoesServiceRequest]
    public virtual string DownloadText(
      Encoding encoding = null,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      using (SyncMemoryStream target = new SyncMemoryStream())
      {
        this.DownloadToStream((Stream) target, accessCondition, options, operationContext);
        byte[] buffer = target.GetBuffer();
        return (encoding ?? Encoding.UTF8).GetString(buffer, 0, (int) target.Length);
      }
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadText(AsyncCallback callback, object state) => this.BeginDownloadText((Encoding) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadText(
      Encoding encoding,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginDownloadText(encoding, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginDownloadText(
      Encoding encoding,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<string>((Func<CancellationToken, Task<string>>) (token => this.DownloadTextAsync(encoding, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual string EndDownloadText(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<string>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<string> DownloadTextAsync() => this.DownloadTextAsync((Encoding) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<string> DownloadTextAsync(CancellationToken cancellationToken) => this.DownloadTextAsync((Encoding) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<string> DownloadTextAsync(
      Encoding encoding,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.DownloadTextAsync(encoding, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<string> DownloadTextAsync(
      Encoding encoding,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.DownloadTextAsync(encoding, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task<string> DownloadTextAsync(
      Encoding encoding,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      string str;
      using (SyncMemoryStream stream = new SyncMemoryStream())
      {
        await this.DownloadToStreamAsync((Stream) stream, accessCondition, options, operationContext, progressHandler, cancellationToken).ConfigureAwait(false);
        byte[] buffer = stream.GetBuffer();
        str = (encoding ?? Encoding.UTF8).GetString(buffer, 0, (int) stream.Length);
      }
      return str;
    }

    [DoesServiceRequest]
    public virtual void PutBlock(
      string blockId,
      Stream blockData,
      Checksum contentChecksum,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (blockData), (object) blockData);
      contentChecksum = contentChecksum ?? Checksum.None;
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.BlockBlob, this.ServiceClient);
      ChecksumRequested calculateChecksum = new ChecksumRequested(string.IsNullOrEmpty(contentChecksum.MD5) && options1.ChecksumOptions.UseTransactionalMD5.Value, string.IsNullOrEmpty(contentChecksum.CRC64) && options1.ChecksumOptions.UseTransactionalCRC64.Value);
      operationContext = operationContext ?? new OperationContext();
      Stream source = blockData;
      bool flag = false;
      try
      {
        if (!blockData.CanSeek || calculateChecksum.HasAny)
        {
          ExecutionState<NullType> temporaryExecutionState = BlobCommonUtility.CreateTemporaryExecutionState(options1);
          Stream toStream;
          if (blockData.CanSeek)
          {
            toStream = Stream.Null;
          }
          else
          {
            source = (Stream) new MultiBufferMemoryStream(this.ServiceClient.BufferManager);
            flag = true;
            toStream = source;
          }
          long position = source.Position;
          StreamDescriptor streamCopyState = new StreamDescriptor();
          blockData.WriteToSync<NullType>(toStream, new long?(), new long?(104857600L), calculateChecksum, true, temporaryExecutionState, streamCopyState);
          source.Position = position;
          contentChecksum = new Checksum(calculateChecksum.MD5 ? streamCopyState.Md5 : (string) null, calculateChecksum.CRC64 ? streamCopyState.Crc64 : (string) null);
        }
        Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.PutBlockImpl(source, blockId, contentChecksum, accessCondition, options1), options1.RetryPolicy, operationContext);
      }
      finally
      {
        if (flag)
          source.Dispose();
      }
    }

    [DoesServiceRequest]
    public virtual void PutBlock(
      string blockId,
      Uri sourceUri,
      long? offset,
      long? count,
      string contentMD5,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.PutBlock(blockId, sourceUri, offset, count, new Checksum(contentMD5), accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual void PutBlock(
      string blockId,
      Uri sourceUri,
      long? offset,
      long? count,
      Checksum contentChecksum,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (sourceUri), (object) sourceUri);
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.BlockBlob, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.PutBlockImpl(sourceUri, offset, count, contentChecksum, blockId, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginPutBlock(
      string blockId,
      Stream blockData,
      Checksum contentChecksum,
      AsyncCallback callback,
      object state)
    {
      return this.BeginPutBlock(blockId, blockData, contentChecksum, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginPutBlock(
      string blockId,
      Stream blockData,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginPutBlock(blockId, blockData, contentChecksum, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginPutBlock(
      string blockId,
      Stream blockData,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.PutBlockAsync(blockId, blockData, contentChecksum, accessCondition, options, operationContext, token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginPutBlock(
      string blockId,
      Uri sourceUri,
      long? offset,
      long? count,
      Checksum contentChecksum,
      AsyncCallback callback,
      object state)
    {
      return this.BeginPutBlock(blockId, sourceUri, offset, count, contentChecksum, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginPutBlock(
      string blockId,
      Uri sourceUri,
      long? offset,
      long? count,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      CommonUtility.AssertNotNull(nameof (sourceUri), (object) sourceUri);
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.BlockBlob, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      StorageAsyncResult<NullType> storageAsyncResult = new StorageAsyncResult<NullType>(callback, state);
      ExecutionState<NullType> temporaryExecutionState = BlobCommonUtility.CreateTemporaryExecutionState(options1);
      storageAsyncResult.CancelDelegate = new Action(((CancellableOperationBase) temporaryExecutionState).Cancel);
      try
      {
        this.PutBlockHandler(blockId, sourceUri, offset, count, contentChecksum, accessCondition, options1, operationContext, storageAsyncResult);
      }
      catch (Exception ex)
      {
        storageAsyncResult.OnComplete(ex);
      }
      return (ICancellableAsyncResult) storageAsyncResult;
    }

    private void PutBlockHandler(
      string blockId,
      Uri sourceUri,
      long? offset,
      long? count,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      StorageAsyncResult<NullType> storageAsyncResult)
    {
      lock (storageAsyncResult.CancellationLockerObject)
      {
        ICancellableAsyncResult cancellableAsyncResult = CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.PutBlockAsync(blockId, sourceUri, offset, count, contentChecksum, accessCondition, options, operationContext, token)), (AsyncCallback) (ar =>
        {
          storageAsyncResult.UpdateCompletedSynchronously(ar.CompletedSynchronously);
          try
          {
            ((CancellableAsyncResultTaskWrapper<AccountProperties>) ar).GetAwaiter().GetResult();
            storageAsyncResult.OnComplete();
          }
          catch (Exception ex)
          {
            storageAsyncResult.OnComplete(ex);
          }
        }), (object) null);
        storageAsyncResult.CancelDelegate = new Action(cancellableAsyncResult.Cancel);
        if (!storageAsyncResult.CancelRequested)
          return;
        storageAsyncResult.Cancel();
      }
    }

    public virtual void EndPutBlock(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task PutBlockAsync(string blockId, Stream blockData, string contentMD5) => this.PutBlockAsync(blockId, blockData, new Checksum(contentMD5));

    [DoesServiceRequest]
    public virtual Task PutBlockAsync(string blockId, Stream blockData, Checksum contentChecksum = null) => this.PutBlockAsync(blockId, blockData, contentChecksum, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task PutBlockAsync(
      string blockId,
      Uri sourceUri,
      long? offset,
      long? count,
      string contentMD5)
    {
      return this.PutBlockAsync(blockId, sourceUri, offset, count, new Checksum(contentMD5));
    }

    [DoesServiceRequest]
    public virtual Task PutBlockAsync(
      string blockId,
      Uri sourceUri,
      long? offset,
      long? count,
      Checksum contentChecksum = null)
    {
      return this.PutBlockAsync(blockId, sourceUri, offset, count, contentChecksum, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task PutBlockAsync(
      string blockId,
      Stream blockData,
      string contentMD5,
      CancellationToken cancellationToken)
    {
      return this.PutBlockAsync(blockId, blockData, new Checksum(contentMD5), cancellationToken);
    }

    [DoesServiceRequest]
    internal Task PutBlockAsync(
      string blockId,
      Stream blockData,
      Checksum contentChecksum,
      CancellationToken cancellationToken)
    {
      return this.PutBlockAsync(blockId, blockData, contentChecksum, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task PutBlockAsync(
      string blockId,
      Uri sourceUri,
      long? offset,
      long? count,
      string contentMD5,
      CancellationToken cancellationToken)
    {
      return this.PutBlockAsync(blockId, sourceUri, offset, count, new Checksum(contentMD5), cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task PutBlockAsync(
      string blockId,
      Uri sourceUri,
      long? offset,
      long? count,
      Checksum contentChecksum,
      CancellationToken cancellationToken)
    {
      return this.PutBlockAsync(blockId, sourceUri, offset, count, contentChecksum, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task PutBlockAsync(
      string blockId,
      Stream blockData,
      string contentMD5,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.PutBlockAsync(blockId, blockData, new Checksum(contentMD5), accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual Task PutBlockAsync(
      string blockId,
      Stream blockData,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.PutBlockAsync(blockId, blockData, contentChecksum, accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task PutBlockAsync(
      string blockId,
      Uri sourceUri,
      long? offset,
      long? count,
      string contentMD5,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.PutBlockAsync(blockId, sourceUri, offset, count, new Checksum(contentMD5), accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual Task PutBlockAsync(
      string blockId,
      Uri sourceUri,
      long? offset,
      long? count,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.PutBlockAsync(blockId, sourceUri, offset, count, contentChecksum, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task PutBlockAsync(
      string blockId,
      Stream blockData,
      string contentMD5,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.PutBlockAsync(blockId, blockData, new Checksum(contentMD5), accessCondition, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    internal Task PutBlockAsync(
      string blockId,
      Stream blockData,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.PutBlockAsync(blockId, blockData, contentChecksum, accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task PutBlockAsync(
      string blockId,
      Uri sourceUri,
      long? offset,
      long? count,
      string contentMD5,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.PutBlockAsync(blockId, sourceUri, offset, count, new Checksum(contentMD5), accessCondition, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task PutBlockAsync(
      string blockId,
      Uri sourceUri,
      long? offset,
      long? count,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (sourceUri), (object) sourceUri);
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.BlockBlob, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.PutBlockImpl(sourceUri, offset, count, contentChecksum, blockId, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task PutBlockAsync(
      string blockId,
      Stream blockData,
      string contentMD5,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.PutBlockAsync(blockId, blockData, new Checksum(contentMD5), accessCondition, options, operationContext, progressHandler, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task PutBlockAsync(
      string blockId,
      Stream blockData,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.PutBlockAsync(blockId, blockData, contentChecksum, accessCondition, options, operationContext, new AggregatingProgressIncrementer(progressHandler), cancellationToken);
    }

    [DoesServiceRequest]
    private async Task PutBlockAsync(
      string blockId,
      Stream blockData,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AggregatingProgressIncrementer progressIncrementer,
      CancellationToken cancellationToken)
    {
      CloudBlockBlob cloudBlockBlob = this;
      CommonUtility.AssertNotNull(nameof (blockData), (object) blockData);
      contentChecksum = contentChecksum ?? Checksum.None;
      // ISSUE: explicit non-virtual call
      BlobRequestOptions modifiedOptions = BlobRequestOptions.ApplyDefaults(options, BlobType.BlockBlob, __nonvirtual (cloudBlockBlob.ServiceClient));
      bool? nullable;
      int num1;
      if (string.IsNullOrEmpty(contentChecksum?.MD5))
      {
        nullable = modifiedOptions.ChecksumOptions.UseTransactionalMD5;
        num1 = nullable.Value ? 1 : 0;
      }
      else
        num1 = 0;
      int num2;
      if (string.IsNullOrEmpty(contentChecksum?.CRC64))
      {
        nullable = modifiedOptions.ChecksumOptions.UseTransactionalCRC64;
        num2 = nullable.Value ? 1 : 0;
      }
      else
        num2 = 0;
      ChecksumRequested requiresContentChecksum = new ChecksumRequested(num1 != 0, num2 != 0);
      operationContext = operationContext ?? new OperationContext();
      progressIncrementer = progressIncrementer ?? AggregatingProgressIncrementer.None;
      Stream seekableStream = blockData;
      bool seekableStreamCreated = false;
      try
      {
        if (!blockData.CanSeek || requiresContentChecksum.HasAny)
        {
          ExecutionState<NullType> temporaryExecutionState = BlobCommonUtility.CreateTemporaryExecutionState(modifiedOptions);
          Stream toStream;
          if (blockData.CanSeek)
          {
            toStream = Stream.Null;
          }
          else
          {
            // ISSUE: explicit non-virtual call
            seekableStream = (Stream) new MultiBufferMemoryStream(__nonvirtual (cloudBlockBlob.ServiceClient).BufferManager);
            seekableStreamCreated = true;
            toStream = seekableStream;
          }
          long startPosition = seekableStream.Position;
          StreamDescriptor streamCopyState = new StreamDescriptor();
          // ISSUE: explicit non-virtual call
          await blockData.WriteToAsync<NullType>(toStream, __nonvirtual (cloudBlockBlob.ServiceClient).BufferManager, new long?(), new long?(104857600L), requiresContentChecksum, temporaryExecutionState, streamCopyState, cancellationToken).ConfigureAwait(false);
          seekableStream.Position = startPosition;
          contentChecksum = new Checksum(requiresContentChecksum.MD5 ? streamCopyState.Md5 : (string) null, requiresContentChecksum.CRC64 ? streamCopyState.Crc64 : (string) null);
          streamCopyState = (StreamDescriptor) null;
        }
        NullType nullType = await Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(cloudBlockBlob.PutBlockImpl(progressIncrementer.CreateProgressIncrementingStream(seekableStream), blockId, contentChecksum, accessCondition, modifiedOptions), modifiedOptions.RetryPolicy, operationContext, cancellationToken).ConfigureAwait(false);
      }
      finally
      {
        if (seekableStreamCreated)
          seekableStream.Dispose();
      }
      modifiedOptions = (BlobRequestOptions) null;
      seekableStream = (Stream) null;
    }

    [DoesServiceRequest]
    public virtual void SetStandardBlobTier(
      StandardBlobTier standardBlobTier,
      RehydratePriority? rehydratePriority = null,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.BlockBlob, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.SetStandardBlobTierImpl(standardBlobTier, rehydratePriority, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetStandardBlobTier(
      StandardBlobTier standardBlobTier,
      AsyncCallback callback,
      object state)
    {
      return this.BeginSetStandardBlobTier(standardBlobTier, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetStandardBlobTier(
      StandardBlobTier standardBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.SetStandardBlobTierAsync(standardBlobTier, new RehydratePriority?(), accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndSetStandardBlobTier(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task SetStandardBlobTierAsync(StandardBlobTier standardBlobTier) => this.SetStandardBlobTierAsync(standardBlobTier, new RehydratePriority?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task SetStandardBlobTierAsync(
      StandardBlobTier standardBlobTier,
      CancellationToken cancellationToken)
    {
      return this.SetStandardBlobTierAsync(standardBlobTier, new RehydratePriority?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task SetStandardBlobTierAsync(
      StandardBlobTier standardBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.SetStandardBlobTierAsync(standardBlobTier, new RehydratePriority?(), accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task SetStandardBlobTierAsync(
      StandardBlobTier standardBlobTier,
      RehydratePriority? rehydratePriority,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.BlockBlob, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.SetStandardBlobTierImpl(standardBlobTier, rehydratePriority, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual string StartCopy(
      CloudBlockBlob source,
      StandardBlobTier? standardBlockBlobTier = null,
      RehydratePriority? rehydratePriority = null,
      AccessCondition sourceAccessCondition = null,
      AccessCondition destAccessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return this.StartCopy(source, Checksum.None, false, standardBlockBlobTier, rehydratePriority, sourceAccessCondition, destAccessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public string StartCopy(
      CloudBlockBlob source,
      Checksum contentChecksum,
      bool syncCopy,
      StandardBlobTier? standardBlockBlobTier,
      RehydratePriority? rehydratePriority,
      AccessCondition sourceAccessCondition = null,
      AccessCondition destAccessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return this.StartCopy(CloudBlob.SourceBlobToUri((CloudBlob) source), contentChecksum, syncCopy, new PremiumPageBlobTier?(), standardBlockBlobTier, rehydratePriority, sourceAccessCondition, destAccessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginStartCopy(
      CloudBlockBlob source,
      AsyncCallback callback,
      object state)
    {
      return this.BeginStartCopy(CloudBlob.SourceBlobToUri((CloudBlob) source), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginStartCopy(
      CloudBlockBlob source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginStartCopy(source, new StandardBlobTier?(), sourceAccessCondition, destAccessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginStartCopy(
      CloudBlockBlob source,
      StandardBlobTier? standardBlockBlobTier,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginStartCopy(source, (string) null, false, false, standardBlockBlobTier, sourceAccessCondition, destAccessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginStartCopy(
      CloudBlockBlob source,
      string contentMD5,
      bool incrementalCopy,
      bool syncCopy,
      StandardBlobTier? standardBlockBlobTier,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginStartCopy(CloudBlob.SourceBlobToUri((CloudBlob) source), contentMD5, incrementalCopy, syncCopy, new PremiumPageBlobTier?(), standardBlockBlobTier, sourceAccessCondition, destAccessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(CloudBlockBlob source) => this.StartCopyAsync(source, new StandardBlobTier?(), new RehydratePriority?(), (AccessCondition) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      CloudBlockBlob source,
      CancellationToken cancellationToken)
    {
      return this.StartCopyAsync(source, new StandardBlobTier?(), new RehydratePriority?(), (AccessCondition) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      CloudBlockBlob source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.StartCopyAsync(source, new StandardBlobTier?(), new RehydratePriority?(), sourceAccessCondition, destAccessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      CloudBlockBlob source,
      StandardBlobTier? standardBlockBlobTier,
      RehydratePriority? rehydratePriority,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.StartCopyAsync(source, Checksum.None, false, false, standardBlockBlobTier, rehydratePriority, sourceAccessCondition, destAccessCondition, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public Task<string> StartCopyAsync(
      CloudBlockBlob source,
      Checksum contentChecksum,
      bool incrementalCopy,
      bool syncCopy,
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
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<string>(this.StartCopyImpl(this.attributes, CloudBlob.SourceBlobToUri((CloudBlob) source), contentChecksum, incrementalCopy, syncCopy, new PremiumPageBlobTier?(), standardBlockBlobTier, rehydratePriority, sourceAccessCondition, destAccessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual IEnumerable<ListBlockItem> DownloadBlockList(
      BlockListingFilter blockListingFilter = BlockListingFilter.Committed,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.BlockBlob, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<IEnumerable<ListBlockItem>>(this.GetBlockListImpl(blockListingFilter, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadBlockList(
      AsyncCallback callback,
      object state)
    {
      return this.BeginDownloadBlockList(BlockListingFilter.Committed, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadBlockList(
      BlockListingFilter blockListingFilter,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<IEnumerable<ListBlockItem>>((Func<CancellationToken, Task<IEnumerable<ListBlockItem>>>) (token => this.DownloadBlockListAsync(blockListingFilter, accessCondition, options, operationContext)), callback, state);
    }

    public virtual IEnumerable<ListBlockItem> EndDownloadBlockList(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<IEnumerable<ListBlockItem>>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<IEnumerable<ListBlockItem>> DownloadBlockListAsync() => this.DownloadBlockListAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<IEnumerable<ListBlockItem>> DownloadBlockListAsync(
      CancellationToken cancellationToken)
    {
      return this.DownloadBlockListAsync(BlockListingFilter.Committed, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<IEnumerable<ListBlockItem>> DownloadBlockListAsync(
      BlockListingFilter blockListingFilter,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.DownloadBlockListAsync(blockListingFilter, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<IEnumerable<ListBlockItem>> DownloadBlockListAsync(
      BlockListingFilter blockListingFilter,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.BlockBlob, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<IEnumerable<ListBlockItem>>(this.GetBlockListImpl(blockListingFilter, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual CloudBlockBlob CreateSnapshot(
      IDictionary<string, string> metadata = null,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.BlockBlob, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<CloudBlockBlob>(this.CreateSnapshotImpl(metadata, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreateSnapshot(AsyncCallback callback, object state) => this.BeginCreateSnapshot((IDictionary<string, string>) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreateSnapshot(
      IDictionary<string, string> metadata,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<CloudBlockBlob>((Func<CancellationToken, Task<CloudBlockBlob>>) (token => this.CreateSnapshotAsync(metadata, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual CloudBlockBlob EndCreateSnapshot(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<CloudBlockBlob>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<CloudBlockBlob> CreateSnapshotAsync() => this.CreateSnapshotAsync((IDictionary<string, string>) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<CloudBlockBlob> CreateSnapshotAsync(CancellationToken cancellationToken) => this.CreateSnapshotAsync((IDictionary<string, string>) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<CloudBlockBlob> CreateSnapshotAsync(
      IDictionary<string, string> metadata,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.CreateSnapshotAsync(metadata, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<CloudBlockBlob> CreateSnapshotAsync(
      IDictionary<string, string> metadata,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.BlockBlob, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<CloudBlockBlob>(this.CreateSnapshotImpl(metadata, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void PutBlockList(
      IEnumerable<string> blockList,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.BlockBlob, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.PutBlockListImpl(blockList.Select<string, PutBlockListItem>((Func<string, PutBlockListItem>) (i => new PutBlockListItem(i, BlockSearchMode.Latest))), accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginPutBlockList(
      IEnumerable<string> blockList,
      AsyncCallback callback,
      object state)
    {
      return this.BeginPutBlockList(blockList, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginPutBlockList(
      IEnumerable<string> blockList,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.PutBlockListAsync(blockList, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndPutBlockList(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task PutBlockListAsync(IEnumerable<string> blockList) => this.PutBlockListAsync(blockList, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task PutBlockListAsync(
      IEnumerable<string> blockList,
      CancellationToken cancellationToken)
    {
      return this.PutBlockListAsync(blockList, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task PutBlockListAsync(
      IEnumerable<string> blockList,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.PutBlockListAsync(blockList, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task PutBlockListAsync(
      IEnumerable<string> blockList,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.BlockBlob, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.PutBlockListImpl(blockList.Select<string, PutBlockListItem>((Func<string, PutBlockListItem>) (i => new PutBlockListItem(i, BlockSearchMode.Latest))), accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    private RESTCommand<NullType> PutBlobImpl(
      Stream stream,
      long? length,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      long offset = stream.Position;
      length = new long?(length ?? stream.Length - offset);
      this.Properties.ContentChecksum = contentChecksum;
      CappedLengthReadOnlyStream cappedStream = new CappedLengthReadOnlyStream(stream, length.Value + offset);
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildContent = (Func<RESTCommand<NullType>, OperationContext, HttpContent>) ((cmd, ctx) => HttpContentFactory.BuildContentFromStream<NullType>((Stream) cappedStream, offset, length, Checksum.None, cmd, ctx));
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        BlobRequest.VerifyHttpsCustomerProvidedKey(uri, options);
        StorageRequestMessage request = BlobHttpRequestMessageFactory.Put(uri, serverTimeout, this.Properties, BlobType.BlockBlob, 0L, new PremiumPageBlobTier?(), accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options);
        BlobHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Created, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(this.attributes, resp, false);
        cmd.CurrentResult.IsRequestServerEncrypted = HttpResponseParsers.ParseServerRequestEncrypted(resp);
        this.Properties.Length = length.Value;
        BlobResponse.ValidateCPKHeaders(resp, options, true);
        return NullType.Value;
      });
      return cmd1;
    }

    internal RESTCommand<NullType> PutBlockImpl(
      Stream source,
      string blockId,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      options.AssertNoEncryptionPolicyOrStrictMode();
      long offset = source.Position;
      long length = source.Length - offset;
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildContent = (Func<RESTCommand<NullType>, OperationContext, HttpContent>) ((cmd, ctx) => HttpContentFactory.BuildContentFromStream<NullType>(source, offset, new long?(length), contentChecksum, cmd, ctx));
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        BlobRequest.VerifyHttpsCustomerProvidedKey(uri, options);
        return BlobHttpRequestMessageFactory.PutBlock(uri, serverTimeout, blockId, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options);
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Created, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        cmd.CurrentResult.IsRequestServerEncrypted = HttpResponseParsers.ParseServerRequestEncrypted(resp);
        cmd.CurrentResult.EncryptionKeySHA256 = HttpResponseParsers.ParseEncryptionKeySHA256(resp);
        BlobResponse.ValidateCPKHeaders(resp, options, true);
        cmd.CurrentResult.EncryptionScope = HttpResponseParsers.ParseEncryptionScope(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    internal RESTCommand<NullType> PutBlockImpl(
      Uri sourceUri,
      long? offset,
      long? count,
      Checksum contentChecksum,
      string blockId,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      options.AssertNoEncryptionPolicyOrStrictMode();
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        BlobRequest.VerifyHttpsCustomerProvidedKey(uri, options);
        return BlobHttpRequestMessageFactory.PutBlock(uri, sourceUri, offset, count, contentChecksum, serverTimeout, blockId, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options);
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Created, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        cmd.CurrentResult.IsRequestServerEncrypted = HttpResponseParsers.ParseServerRequestEncrypted(resp);
        cmd.CurrentResult.EncryptionKeySHA256 = HttpResponseParsers.ParseEncryptionKeySHA256(resp);
        cmd.CurrentResult.EncryptionScope = HttpResponseParsers.ParseEncryptionScope(resp);
        BlobResponse.ValidateCPKHeaders(resp, options, true);
        return NullType.Value;
      });
      return cmd1;
    }

    internal RESTCommand<NullType> PutBlockListImpl(
      IEnumerable<PutBlockListItem> blocks,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      MultiBufferMemoryStream memoryStream = new MultiBufferMemoryStream(this.ServiceClient.BufferManager);
      BlobRequest.WriteBlockListBody(blocks, (Stream) memoryStream);
      memoryStream.Seek(0L, SeekOrigin.Begin);
      Checksum contentChecksum = new Checksum(!options.ChecksumOptions.UseTransactionalMD5.HasValue || !options.ChecksumOptions.UseTransactionalMD5.Value ? (string) null : memoryStream.ComputeMD5Hash(), !options.ChecksumOptions.UseTransactionalCRC64.HasValue || !options.ChecksumOptions.UseTransactionalCRC64.Value ? (string) null : memoryStream.ComputeCRC64Hash());
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildContent = (Func<RESTCommand<NullType>, OperationContext, HttpContent>) ((cmd, ctx) => HttpContentFactory.BuildContentFromStream<NullType>((Stream) memoryStream, 0L, new long?(memoryStream.Length), contentChecksum, cmd, ctx));
      cmd1.StreamToDispose = (Stream) memoryStream;
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        BlobRequest.VerifyHttpsCustomerProvidedKey(uri, options);
        StorageRequestMessage request = BlobHttpRequestMessageFactory.PutBlockList(uri, serverTimeout, this.Properties, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options);
        BlobHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Created, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(this.attributes, resp, false);
        cmd.CurrentResult.IsRequestServerEncrypted = HttpResponseParsers.ParseServerRequestEncrypted(resp);
        cmd.CurrentResult.EncryptionKeySHA256 = HttpResponseParsers.ParseEncryptionKeySHA256(resp);
        cmd.CurrentResult.EncryptionScope = HttpResponseParsers.ParseEncryptionScope(resp);
        this.Properties.Length = -1L;
        BlobResponse.ValidateCPKHeaders(resp, options, true);
        return NullType.Value;
      });
      return cmd1;
    }

    internal RESTCommand<IEnumerable<ListBlockItem>> GetBlockListImpl(
      BlockListingFilter typesOfBlocks,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<IEnumerable<ListBlockItem>> cmd1 = new RESTCommand<IEnumerable<ListBlockItem>>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<IEnumerable<ListBlockItem>>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<IEnumerable<ListBlockItem>>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.GetBlockList(uri, serverTimeout, this.SnapshotTime, typesOfBlocks, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<IEnumerable<ListBlockItem>>, HttpResponseMessage, Exception, OperationContext, IEnumerable<ListBlockItem>>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<IEnumerable<ListBlockItem>>(HttpStatusCode.OK, resp, (IEnumerable<ListBlockItem>) null, (StorageCommandBase<IEnumerable<ListBlockItem>>) cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<IEnumerable<ListBlockItem>>, HttpResponseMessage, OperationContext, CancellationToken, Task<IEnumerable<ListBlockItem>>>) ((cmd, resp, ctx, ct) =>
      {
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(this.attributes, resp, true);
        return GetBlockListResponse.ParseAsync(cmd.ResponseStream, ct);
      });
      return cmd1;
    }

    internal RESTCommand<CloudBlockBlob> CreateSnapshotImpl(
      IDictionary<string, string> metadata,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<CloudBlockBlob> cmd1 = new RESTCommand<CloudBlockBlob>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<CloudBlockBlob>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<CloudBlockBlob>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = BlobHttpRequestMessageFactory.Snapshot(uri, serverTimeout, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options);
        if (metadata != null)
          BlobHttpRequestMessageFactory.AddMetadata(request, metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<CloudBlockBlob>, HttpResponseMessage, Exception, OperationContext, CloudBlockBlob>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<CloudBlockBlob>(HttpStatusCode.Created, resp, (CloudBlockBlob) null, (StorageCommandBase<CloudBlockBlob>) cmd, ex);
        CloudBlockBlob snapshotImpl = new CloudBlockBlob(this.Name, new DateTimeOffset?(NavigationHelper.ParseSnapshotTime(BlobHttpResponseParsers.GetSnapshotTime(resp))), this.Container);
        snapshotImpl.attributes.Metadata = (IDictionary<string, string>) new Dictionary<string, string>(metadata ?? this.Metadata, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        snapshotImpl.attributes.Properties = new BlobProperties(this.Properties);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(snapshotImpl.attributes, resp, false);
        return snapshotImpl;
      });
      return cmd1;
    }

    internal RESTCommand<NullType> SetStandardBlobTierImpl(
      StandardBlobTier standardBlobTier,
      RehydratePriority? rehydratePriority,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.SetBlobTier(uri, serverTimeout, standardBlobTier.ToString(), rehydratePriority, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(new HttpStatusCode[2]
        {
          HttpStatusCode.OK,
          HttpStatusCode.Accepted
        }, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(this.attributes, resp, false);
        this.attributes.Properties.RehydrationStatus = new RehydrationStatus?();
        this.attributes.Properties.BlobTierInferred = new bool?(false);
        if (resp.StatusCode.Equals((object) HttpStatusCode.OK))
          this.attributes.Properties.StandardBlobTier = new StandardBlobTier?(standardBlobTier);
        else
          this.attributes.Properties.StandardBlobTier = new StandardBlobTier?(StandardBlobTier.Archive);
        return NullType.Value;
      });
      return cmd1;
    }

    private static bool IsLessThanSingleBlobThreshold(
      Stream source,
      long? length,
      BlobRequestOptions modifiedOptions,
      bool noPadding)
    {
      if (!source.CanSeek)
        return false;
      ref long? local = ref length;
      long? nullable = length;
      long num1 = nullable ?? source.Length - source.Position;
      local = new long?(num1);
      if (modifiedOptions.EncryptionPolicy != null)
        length = new long?(modifiedOptions.EncryptionPolicy.GetEncryptedLength(length.Value, noPadding));
      nullable = length;
      long num2 = modifiedOptions.SingleBlobUploadThresholdInBytes.Value;
      return nullable.GetValueOrDefault() <= num2 & nullable.HasValue;
    }

    private static void ContinueAsyncOperation(
      StorageAsyncResult<NullType> storageAsyncResult,
      IAsyncResult result,
      Action actionToTakeInTheLock)
    {
      storageAsyncResult.UpdateCompletedSynchronously(result.CompletedSynchronously);
      try
      {
        lock (storageAsyncResult.CancellationLockerObject)
        {
          storageAsyncResult.CancelDelegate = (Action) null;
          actionToTakeInTheLock();
        }
      }
      catch (Exception ex)
      {
        storageAsyncResult.OnComplete(ex);
      }
    }

    private static bool ContinueOpenWriteOnFailure(
      StorageException exception,
      AccessCondition accessCondition)
    {
      return exception.RequestInformation != null && (exception.RequestInformation.HttpStatusCode == 404 && string.IsNullOrEmpty(accessCondition.IfMatchETag) || exception.RequestInformation.HttpStatusCode == 403);
    }

    public CloudBlockBlob(Uri blobAbsoluteUri)
      : this(blobAbsoluteUri, (StorageCredentials) null)
    {
    }

    public CloudBlockBlob(Uri blobAbsoluteUri, StorageCredentials credentials)
      : this(blobAbsoluteUri, new DateTimeOffset?(), credentials)
    {
    }

    public CloudBlockBlob(Uri blobAbsoluteUri, CloudBlobClient client)
      : this(blobAbsoluteUri, new DateTimeOffset?(), client)
    {
    }

    public CloudBlockBlob(
      Uri blobAbsoluteUri,
      DateTimeOffset? snapshotTime,
      StorageCredentials credentials)
      : this(new StorageUri(blobAbsoluteUri), snapshotTime, credentials)
    {
    }

    public CloudBlockBlob(
      Uri blobAbsoluteUri,
      DateTimeOffset? snapshotTime,
      CloudBlobClient client)
      : this(new StorageUri(blobAbsoluteUri), snapshotTime, client)
    {
    }

    public CloudBlockBlob(
      StorageUri blobAbsoluteUri,
      DateTimeOffset? snapshotTime,
      StorageCredentials credentials)
      : base(blobAbsoluteUri, snapshotTime, credentials)
    {
      this.Properties.BlobType = BlobType.BlockBlob;
    }

    public CloudBlockBlob(
      StorageUri blobAbsoluteUri,
      DateTimeOffset? snapshotTime,
      CloudBlobClient client)
      : base(blobAbsoluteUri, snapshotTime, client)
    {
      this.Properties.BlobType = BlobType.BlockBlob;
    }

    internal CloudBlockBlob(
      string blobName,
      DateTimeOffset? snapshotTime,
      CloudBlobContainer container)
      : base(blobName, snapshotTime, container)
    {
      this.Properties.BlobType = BlobType.BlockBlob;
    }

    internal CloudBlockBlob(BlobAttributes attributes, CloudBlobClient serviceClient)
      : base(attributes, serviceClient)
    {
      this.Properties.BlobType = BlobType.BlockBlob;
    }

    public int StreamWriteSizeInBytes
    {
      get => this.streamWriteSizeInBytes;
      set
      {
        CommonUtility.AssertInBounds<long>(nameof (StreamWriteSizeInBytes), (long) value, 16384L, 104857600L);
        this.isStreamWriteSizeModified = true;
        this.streamWriteSizeInBytes = value;
      }
    }

    internal bool IsStreamWriteSizeModified => this.isStreamWriteSizeModified;

    [DoesServiceRequest]
    internal Task UploadFromMultiStreamAsync(
      IEnumerable<Stream> streamList,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.UploadFromMultiStreamAsync(streamList, accessCondition, options, operationContext, new AggregatingProgressIncrementer(progressHandler), cancellationToken);
    }

    [DoesServiceRequest]
    private async Task UploadFromMultiStreamAsync(
      IEnumerable<Stream> streamList,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AggregatingProgressIncrementer progressIncrementer,
      CancellationToken cancellationToken)
    {
      CloudBlockBlob cloudBlockBlob = this;
      CommonUtility.AssertNotNull(nameof (streamList), (object) streamList);
      // ISSUE: explicit non-virtual call
      BlobRequestOptions modifiedOptions = BlobRequestOptions.ApplyDefaults(options, BlobType.BlockBlob, __nonvirtual (cloudBlockBlob.ServiceClient));
      operationContext = operationContext ?? new OperationContext();
      int parallelOperations = modifiedOptions.ParallelOperationThreadCount.Value;
      List<string> blockList = new List<string>();
      List<Task> uploadTaskList = new List<Task>();
      int blockNum = 0;
      progressIncrementer = progressIncrementer ?? AggregatingProgressIncrementer.None;
      foreach (Stream block in streamList)
      {
        if (uploadTaskList.Count == parallelOperations)
        {
          Task task = await Task.WhenAny(uploadTaskList.ToArray()).ConfigureAwait(false);
          uploadTaskList.RemoveAll((Predicate<Task>) (putBlockUpload =>
          {
            if (!putBlockUpload.IsCompleted)
              return false;
            if (putBlockUpload.Exception != null)
              throw putBlockUpload.Exception;
            return true;
          }));
        }
        string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("Block_{0}", (object) (++blockNum).ToString("00000"))));
        blockList.Add(base64String);
        Stream localBlock = block;
        try
        {
          Task task = cloudBlockBlob.PutBlockAsync(base64String, block, Checksum.None, accessCondition, modifiedOptions, operationContext, progressIncrementer, cancellationToken);
          task.ContinueWith((Action<Task>) (finishedUpload => localBlock.Dispose()));
          uploadTaskList.Add(task);
        }
        catch (Exception ex)
        {
          localBlock.Dispose();
          throw;
        }
      }
      await Task.WhenAll((IEnumerable<Task>) uploadTaskList).ConfigureAwait(false);
      await cloudBlockBlob.PutBlockListAsync((IEnumerable<string>) blockList, accessCondition, modifiedOptions, operationContext, cancellationToken).ConfigureAwait(false);
      modifiedOptions = (BlobRequestOptions) null;
      blockList = (List<string>) null;
      uploadTaskList = (List<Task>) null;
    }

    private IEnumerable<Stream> OpenMultiFileStream(string path)
    {
      int totalBlocks = (int) Math.Ceiling((double) new FileInfo(path).Length / (double) this.streamWriteSizeInBytes);
      for (long i = 0; i < (long) totalBlocks; ++i)
      {
        FileStream wrappedStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        wrappedStream.Seek(i * (long) this.streamWriteSizeInBytes, SeekOrigin.Begin);
        yield return (Stream) new ReadLengthLimitingStream((Stream) wrappedStream, (long) this.streamWriteSizeInBytes);
      }
    }

    private IEnumerable<Stream> OpenMultiSubStream(
      Stream wrappedStream,
      long? length,
      SemaphoreSlim mutex)
    {
      if (!wrappedStream.CanSeek)
        throw new ArgumentException();
      int totalBlocks = (int) Math.Ceiling((double) (length ?? wrappedStream.Length - wrappedStream.Position) / (double) this.streamWriteSizeInBytes);
      long offset = wrappedStream.Position;
      SemaphoreSlim streamReadThrottler = new SemaphoreSlim(1);
      for (long i = 0; i < (long) totalBlocks; ++i)
        yield return (Stream) new SubStream(wrappedStream, offset + i * (long) this.streamWriteSizeInBytes, (long) this.streamWriteSizeInBytes, streamReadThrottler);
    }

    internal void CheckAdjustBlockSize(long? streamLength)
    {
      if (streamLength.HasValue && (int) Math.Ceiling((double) streamLength.Value / (double) this.streamWriteSizeInBytes) > 50000)
      {
        if (!this.IsStreamWriteSizeModified)
        {
          long? nullable = streamLength;
          long num = 5242880000000;
          if (!(nullable.GetValueOrDefault() > num & nullable.HasValue))
          {
            this.streamWriteSizeInBytes = (int) Math.Ceiling((double) streamLength.Value / 50000.0);
            return;
          }
        }
        throw new StorageException("The total blocks required for this upload exceeds the maximum block limit. Please increase the block size if applicable and ensure the Blob size is not greater than the maximum Blob size limit.");
      }
    }

    private static StorageException GenerateExceptionForConflictFailure() => new StorageException(new RequestResult()
    {
      HttpStatusMessage = "The specified blob already exists.",
      HttpStatusCode = 409,
      ExtendedErrorInformation = (StorageExtendedErrorInformation) null
    }, "The specified blob already exists.", (Exception) null);
  }
}
