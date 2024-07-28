// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.CloudAppendBlob
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
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Blob
{
  public class CloudAppendBlob : CloudBlob, ICloudBlob, IListBlobItem
  {
    private int streamWriteSizeInBytes = 4194304;

    [DoesServiceRequest]
    public virtual CloudBlobStream OpenWrite(
      bool createNew,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, this.BlobType, this.ServiceClient, false);
      ICryptoTransform transform = (ICryptoTransform) null;
      if (createNew)
      {
        if (options != null && options.EncryptionPolicy != null)
          transform = options.EncryptionPolicy.CreateAndSetEncryptionContext(this.Metadata, false);
        this.CreateOrReplace(accessCondition, options, operationContext);
      }
      else
      {
        bool? nullable = options1.ChecksumOptions.StoreContentMD5;
        if (nullable.Value)
          throw new ArgumentException("MD5 cannot be calculated for an existing blob because it would require reading the existing data. Please disable StoreBlobContentMD5.");
        nullable = options1.ChecksumOptions.StoreContentCRC64;
        if (nullable.Value)
          throw new ArgumentException("CRC64 cannot be calculated for an existing blob because it would require reading the existing data. Please disable StoreBlobContentCRC64.");
        if (options1.EncryptionPolicy != null)
          throw new ArgumentException("Encryption is not supported for a blob that already exists. Please do not specify an encryption policy.");
        this.FetchAttributes(accessCondition, options, operationContext);
      }
      if (accessCondition != null)
        accessCondition = new AccessCondition()
        {
          LeaseId = accessCondition.LeaseId,
          IfAppendPositionEqual = accessCondition.IfAppendPositionEqual,
          IfMaxSizeLessThanOrEqual = accessCondition.IfMaxSizeLessThanOrEqual
        };
      return options1.EncryptionPolicy != null ? (CloudBlobStream) new BlobEncryptedWriteStream(this, accessCondition, options1, operationContext, transform) : (CloudBlobStream) new BlobWriteStream(this, accessCondition, options1, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginOpenWrite(
      bool createNew,
      AsyncCallback callback,
      object state)
    {
      return this.BeginOpenWrite(createNew, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginOpenWrite(
      bool createNew,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<CloudBlobStream>((Func<CancellationToken, Task<CloudBlobStream>>) (token => this.OpenWriteAsync(createNew, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual CloudBlobStream EndOpenWrite(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<CloudBlobStream>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<CloudBlobStream> OpenWriteAsync(bool createNew) => this.OpenWriteAsync(createNew, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<CloudBlobStream> OpenWriteAsync(
      bool createNew,
      CancellationToken cancellationToken)
    {
      return this.OpenWriteAsync(createNew, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<CloudBlobStream> OpenWriteAsync(
      bool createNew,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.OpenWriteAsync(createNew, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual async Task<CloudBlobStream> OpenWriteAsync(
      bool createNew,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CloudAppendBlob appendBlob = this;
      appendBlob.attributes.AssertNoSnapshot();
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      BlobRequestOptions modifiedOptions = BlobRequestOptions.ApplyDefaults(options, __nonvirtual (appendBlob.BlobType), __nonvirtual (appendBlob.ServiceClient), false);
      ICryptoTransform transform = (ICryptoTransform) null;
      if (createNew)
      {
        if (options != null && options.EncryptionPolicy != null)
        {
          // ISSUE: explicit non-virtual call
          transform = options.EncryptionPolicy.CreateAndSetEncryptionContext(__nonvirtual (appendBlob.Metadata), false);
        }
        await appendBlob.CreateOrReplaceAsync(accessCondition, options, operationContext, cancellationToken).ConfigureAwait(false);
      }
      else
      {
        bool? nullable = modifiedOptions.ChecksumOptions.StoreContentMD5;
        if (nullable.Value)
          throw new ArgumentException("MD5 cannot be calculated for an existing blob because it would require reading the existing data. Please disable StoreBlobContentMD5.");
        nullable = modifiedOptions.ChecksumOptions.StoreContentCRC64;
        if (nullable.Value)
          throw new ArgumentException("CRC64 cannot be calculated for an existing blob because it would require reading the existing data. Please disable StoreBlobContentCRC64.");
        if (modifiedOptions.EncryptionPolicy != null)
          throw new ArgumentException("Encryption is not supported for a blob that already exists. Please do not specify an encryption policy.");
        await appendBlob.FetchAttributesAsync(accessCondition, options, operationContext, cancellationToken).ConfigureAwait(false);
      }
      if (accessCondition != null)
        accessCondition = new AccessCondition()
        {
          LeaseId = accessCondition.LeaseId,
          IfAppendPositionEqual = accessCondition.IfAppendPositionEqual,
          IfMaxSizeLessThanOrEqual = accessCondition.IfMaxSizeLessThanOrEqual
        };
      CloudBlobStream cloudBlobStream = modifiedOptions.EncryptionPolicy == null ? (CloudBlobStream) new BlobWriteStream(appendBlob, accessCondition, modifiedOptions, operationContext) : (CloudBlobStream) new BlobEncryptedWriteStream(appendBlob, accessCondition, modifiedOptions, operationContext, transform);
      modifiedOptions = (BlobRequestOptions) null;
      transform = (ICryptoTransform) null;
      return cloudBlobStream;
    }

    [DoesServiceRequest]
    public virtual void UploadFromStream(
      Stream source,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.UploadFromStreamHelper(source, new long?(), true, accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual void UploadFromStream(
      Stream source,
      long length,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.UploadFromStreamHelper(source, new long?(length), true, accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual void AppendFromStream(
      Stream source,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.UploadFromStreamHelper(source, new long?(), false, accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual void AppendFromStream(
      Stream source,
      long length,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.UploadFromStreamHelper(source, new long?(length), false, accessCondition, options, operationContext);
    }

    internal void UploadFromStreamHelper(
      Stream source,
      long? length,
      bool createNew,
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
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.AppendBlob, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      using (CloudBlobStream toStream = this.OpenWrite(createNew, accessCondition, options1, operationContext))
      {
        using (ExecutionState<NullType> temporaryExecutionState = BlobCommonUtility.CreateTemporaryExecutionState(options1))
        {
          source.WriteToSync<NullType>((Stream) toStream, length, new long?(), ChecksumRequested.None, true, temporaryExecutionState, (StreamDescriptor) null);
          toStream.Commit();
        }
      }
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsyncHelper(source, new long?(), true, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, token)), callback, state);
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
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsyncHelper(source, new long?(), true, accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, token)), callback, state);
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
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsyncHelper(source, new long?(), true, accessCondition, options, operationContext, new AggregatingProgressIncrementer(progressHandler), token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      long length,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsyncHelper(source, new long?(length), true, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, token)), callback, state);
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
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsyncHelper(source, new long?(length), true, accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, token)), callback, state);
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
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsyncHelper(source, new long?(length), true, accessCondition, options, operationContext, new AggregatingProgressIncrementer(progressHandler), token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAppendFromStream(
      Stream source,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsyncHelper(source, new long?(), false, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAppendFromStream(
      Stream source,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsyncHelper(source, new long?(), false, accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, token)), callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginAppendFromStream(
      Stream source,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsyncHelper(source, new long?(), false, accessCondition, options, operationContext, new AggregatingProgressIncrementer(progressHandler), token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAppendFromStream(
      Stream source,
      long length,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsyncHelper(source, new long?(length), false, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAppendFromStream(
      Stream source,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsyncHelper(source, new long?(length), false, accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, token)), callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginAppendFromStream(
      Stream source,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsyncHelper(source, new long?(length), false, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual void EndUploadFromStream(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    public virtual void EndAppendFromStream(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public Task UploadFromStreamAsyncHelper(
      Stream source,
      long? length,
      bool createNew,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, length, createNew, accessCondition, options, operationContext, new AggregatingProgressIncrementer(progressHandler), cancellationToken);
    }

    [DoesServiceRequest]
    private async Task UploadFromStreamAsyncHelper(
      Stream source,
      long? length,
      bool createNew,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AggregatingProgressIncrementer progressIncrementer,
      CancellationToken cancellationToken)
    {
      CloudAppendBlob cloudAppendBlob = this;
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
      progressIncrementer = progressIncrementer ?? AggregatingProgressIncrementer.None;
      cloudAppendBlob.attributes.AssertNoSnapshot();
      // ISSUE: explicit non-virtual call
      BlobRequestOptions modifiedOptions = BlobRequestOptions.ApplyDefaults(options, BlobType.AppendBlob, __nonvirtual (cloudAppendBlob.ServiceClient));
      operationContext = operationContext ?? new OperationContext();
      using (CloudBlobStream blobStream = await cloudAppendBlob.OpenWriteAsync(createNew, accessCondition, modifiedOptions, operationContext, cancellationToken).ConfigureAwait(false))
      {
        using (ExecutionState<NullType> tempExecutionState = BlobCommonUtility.CreateTemporaryExecutionState(modifiedOptions))
        {
          // ISSUE: explicit non-virtual call
          await source.WriteToAsync<NullType>(progressIncrementer.CreateProgressIncrementingStream((Stream) blobStream), __nonvirtual (cloudAppendBlob.ServiceClient).BufferManager, length, new long?(), ChecksumRequested.None, tempExecutionState, (StreamDescriptor) null, cancellationToken).ConfigureAwait(false);
          await blobStream.CommitAsync().ConfigureAwait(false);
        }
      }
      modifiedOptions = (BlobRequestOptions) null;
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(Stream source) => this.UploadFromStreamAsyncHelper(source, new long?(), true, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(Stream source, CancellationToken cancellationToken) => this.UploadFromStreamAsyncHelper(source, new long?(), true, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(), true, accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(), true, accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, cancellationToken);
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
      return this.UploadFromStreamAsyncHelper(source, new long?(), true, accessCondition, options, operationContext, new AggregatingProgressIncrementer(progressHandler), cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(Stream source, long length) => this.UploadFromStreamAsyncHelper(source, new long?(length), true, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      long length,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(length), true, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(length), true, accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, CancellationToken.None);
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
      return this.UploadFromStreamAsyncHelper(source, new long?(length), true, accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, cancellationToken);
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
      return this.UploadFromStreamAsyncHelper(source, new long?(length), true, accessCondition, options, operationContext, new AggregatingProgressIncrementer(progressHandler), cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task AppendFromStreamAsync(Stream source) => this.UploadFromStreamAsyncHelper(source, new long?(), false, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task AppendFromStreamAsync(Stream source, CancellationToken cancellationToken) => this.UploadFromStreamAsyncHelper(source, new long?(), false, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task AppendFromStreamAsync(
      Stream source,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(), false, accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task AppendFromStreamAsync(
      Stream source,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(), false, accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task AppendFromStreamAsync(
      Stream source,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(), false, accessCondition, options, operationContext, new AggregatingProgressIncrementer(progressHandler), cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task AppendFromStreamAsync(Stream source, long length) => this.UploadFromStreamAsyncHelper(source, new long?(length), false, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task AppendFromStreamAsync(
      Stream source,
      long length,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(length), false, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (AggregatingProgressIncrementer) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task AppendFromStreamAsync(
      Stream source,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(length), false, accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task AppendFromStreamAsync(
      Stream source,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(length), false, accessCondition, options, operationContext, (AggregatingProgressIncrementer) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task AppendFromStreamAsync(
      Stream source,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(length), false, accessCondition, options, operationContext, new AggregatingProgressIncrementer(progressHandler), cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void UploadFromFile(
      string path,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (path), (object) path);
      using (FileStream source = new FileStream(path, FileMode.Open, FileAccess.Read))
        this.UploadFromStream((Stream) source, accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual void AppendFromFile(
      string path,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (path), (object) path);
      using (FileStream source = new FileStream(path, FileMode.Open, FileAccess.Read))
        this.AppendFromStream((Stream) source, accessCondition, options, operationContext);
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
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromFileAsync(path, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAppendFromFile(
      string path,
      AsyncCallback callback,
      object state)
    {
      return this.BeginAppendFromFile(path, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAppendFromFile(
      string path,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginAppendFromFile(path, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, callback, state);
    }

    [DoesServiceRequest]
    public ICancellableAsyncResult BeginAppendFromFile(
      string path,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.AppendFromFileAsync(path, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual void EndUploadFromFile(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    public virtual void EndAppendFromFile(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task UploadFromFileAsync(string path) => this.UploadFromFileAsync(path, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task UploadFromFileAsync(string path, CancellationToken cancellationToken) => this.UploadFromFileAsync(path, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task UploadFromFileAsync(
      string path,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.UploadFromFileAsync(path, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromFileAsync(
      string path,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadFromFileAsync(path, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task UploadFromFileAsync(
      string path,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (path), (object) path);
      using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
        await this.UploadFromStreamAsync((Stream) fileStream, accessCondition, options, operationContext, progressHandler, cancellationToken).ConfigureAwait(false);
    }

    [DoesServiceRequest]
    public virtual Task AppendFromFileAsync(string path) => this.AppendFromFileAsync(path, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task AppendFromFileAsync(string path, CancellationToken cancellationToken) => this.AppendFromFileAsync(path, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task AppendFromFileAsync(
      string path,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.AppendFromFileAsync(path, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task AppendFromFileAsync(
      string path,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.AppendFromFileAsync(path, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task AppendFromFileAsync(
      string path,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (path), (object) path);
      using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
        await this.AppendFromStreamAsync((Stream) fileStream, accessCondition, options, operationContext, progressHandler, cancellationToken).ConfigureAwait(false);
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
    public virtual void AppendFromByteArray(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (buffer), (object) buffer);
      using (SyncMemoryStream source = new SyncMemoryStream(buffer, index, count))
        this.AppendFromStream((Stream) source, accessCondition, options, operationContext);
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

    public virtual void EndUploadFromByteArray(IAsyncResult asyncResult)
    {
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAppendFromByteArray(
      byte[] buffer,
      int index,
      int count,
      AsyncCallback callback,
      object state)
    {
      return this.BeginAppendFromByteArray(buffer, index, count, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAppendFromByteArray(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginAppendFromByteArray(buffer, index, count, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginAppendFromByteArray(
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
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.AppendFromByteArrayAsync(buffer, index, count, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual void EndAppendFromByteArray(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

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
    public virtual Task AppendFromByteArrayAsync(byte[] buffer, int index, int count) => this.AppendFromByteArrayAsync(buffer, index, count, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task AppendFromByteArrayAsync(
      byte[] buffer,
      int index,
      int count,
      CancellationToken cancellationToken)
    {
      return this.AppendFromByteArrayAsync(buffer, index, count, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task AppendFromByteArrayAsync(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.AppendFromByteArrayAsync(buffer, index, count, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task AppendFromByteArrayAsync(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.AppendFromByteArrayAsync(buffer, index, count, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task AppendFromByteArrayAsync(
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
        await this.AppendFromStreamAsync((Stream) stream, accessCondition, options, operationContext, progressHandler, cancellationToken).ConfigureAwait(false);
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
    public virtual void AppendText(
      string content,
      Encoding encoding = null,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (content), (object) content);
      byte[] bytes = (encoding ?? Encoding.UTF8).GetBytes(content);
      this.AppendFromByteArray(bytes, 0, bytes.Length, accessCondition, options, operationContext);
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
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadTextAsync(content, encoding, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndUploadText(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAppendText(
      string content,
      AsyncCallback callback,
      object state)
    {
      return this.BeginAppendText(content, (Encoding) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAppendText(
      string content,
      Encoding encoding,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginAppendText(content, encoding, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginAppendText(
      string content,
      Encoding encoding,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.AppendTextAsync(content, encoding, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndAppendText(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

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
    public virtual Task AppendTextAsync(string content) => this.AppendTextAsync(content, (Encoding) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task AppendTextAsync(string content, CancellationToken cancellationToken) => this.AppendTextAsync(content, (Encoding) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task AppendTextAsync(
      string content,
      Encoding encoding,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.AppendTextAsync(content, encoding, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task AppendTextAsync(
      string content,
      Encoding encoding,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.AppendTextAsync(content, encoding, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task AppendTextAsync(
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
      await this.AppendFromByteArrayAsync(bytes, 0, bytes.Length, accessCondition, options, operationContext, progressHandler, cancellationToken).ConfigureAwait(false);
    }

    [DoesServiceRequest]
    public virtual void CreateOrReplace(
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.AppendBlob, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.CreateImpl(accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreateOrReplace(
      AsyncCallback callback,
      object state)
    {
      return this.BeginCreateOrReplace((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreateOrReplace(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.CreateOrReplaceAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndCreateOrReplace(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task CreateOrReplaceAsync() => this.CreateOrReplaceAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task CreateOrReplaceAsync(CancellationToken cancellationToken) => this.CreateOrReplaceAsync((AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task CreateOrReplaceAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.CreateOrReplaceAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task CreateOrReplaceAsync(
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.AppendBlob, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.CreateImpl(accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual long AppendBlock(
      Stream blockData,
      Checksum contentChecksum = null,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (blockData), (object) blockData);
      contentChecksum = contentChecksum ?? Checksum.None;
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.AppendBlob, this.ServiceClient);
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
          blockData.WriteToSync<NullType>(toStream, new long?(), new long?(4194304L), calculateChecksum, true, temporaryExecutionState, streamCopyState);
          source.Position = position;
          contentChecksum = new Checksum(calculateChecksum.MD5 ? streamCopyState.Md5 : (string) null, calculateChecksum.CRC64 ? streamCopyState.Crc64 : (string) null);
        }
        return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<long>(this.AppendBlockImpl(source, contentChecksum, accessCondition, options1), options1.RetryPolicy, operationContext);
      }
      finally
      {
        if (flag)
          source.Dispose();
      }
    }

    [DoesServiceRequest]
    public virtual long AppendBlock(
      Uri sourceUri,
      long offset,
      long count,
      Checksum sourceContentChecksum,
      AccessCondition sourceAccessCondition = null,
      AccessCondition destAccessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (sourceUri), (object) sourceUri);
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.AppendBlob, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<long>(this.AppendBlockImpl(sourceUri, offset, count, sourceContentChecksum, sourceAccessCondition, destAccessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAppendBlock(
      Stream blockData,
      AsyncCallback callback,
      object state)
    {
      return this.BeginAppendBlock(blockData, Checksum.None, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAppendBlock(
      Stream blockData,
      Checksum contentChecksum,
      AsyncCallback callback,
      object state)
    {
      return this.BeginAppendBlock(blockData, contentChecksum, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAppendBlock(
      Stream blockData,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginAppendBlock(blockData, contentChecksum, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginAppendBlock(
      Stream blockData,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<long>((Func<CancellationToken, Task<long>>) (token => this.AppendBlockAsync(blockData, contentChecksum, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    [DoesServiceRequest]
    public ICancellableAsyncResult BeginAppendBlock(
      Uri sourceUri,
      long offset,
      long count,
      Checksum sourceContentChecksum,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<long>((Func<CancellationToken, Task<long>>) (token => this.AppendBlockAsync(sourceUri, offset, count, sourceContentChecksum, sourceAccessCondition, destAccessCondition, options, operationContext, token)), callback, state);
    }

    public virtual long EndAppendBlock(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<long>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<long> AppendBlockAsync(Stream blockData, string contentMD5 = null) => this.AppendBlockAsync(blockData, contentMD5, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<long> AppendBlockAsync(
      Stream blockData,
      string contentMD5,
      CancellationToken cancellationToken)
    {
      return this.AppendBlockAsync(blockData, contentMD5, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<long> AppendBlockAsync(
      Stream blockData,
      string contentMD5,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.AppendBlockAsync(blockData, contentMD5, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<long> AppendBlockAsync(
      Stream blockData,
      string contentMD5,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.AppendBlockAsync(blockData, contentMD5, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<long> AppendBlockAsync(
      Stream blockData,
      string contentMD5,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.AppendBlockAsync(blockData, new Checksum(contentMD5), accessCondition, options, operationContext, progressHandler, cancellationToken);
    }

    [DoesServiceRequest]
    internal async Task<long> AppendBlockAsync(
      Stream blockData,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      CloudAppendBlob cloudAppendBlob = this;
      CommonUtility.AssertNotNull(nameof (blockData), (object) blockData);
      contentChecksum = contentChecksum ?? Checksum.None;
      // ISSUE: explicit non-virtual call
      BlobRequestOptions modifiedOptions = BlobRequestOptions.ApplyDefaults(options, BlobType.AppendBlob, __nonvirtual (cloudAppendBlob.ServiceClient));
      bool? nullable;
      int num1;
      if (string.IsNullOrEmpty(contentChecksum.MD5))
      {
        nullable = modifiedOptions.ChecksumOptions.UseTransactionalMD5;
        num1 = nullable.Value ? 1 : 0;
      }
      else
        num1 = 0;
      int num2;
      if (string.IsNullOrEmpty(contentChecksum.CRC64))
      {
        nullable = modifiedOptions.ChecksumOptions.UseTransactionalCRC64;
        num2 = nullable.Value ? 1 : 0;
      }
      else
        num2 = 0;
      ChecksumRequested requiresContentChecksum = new ChecksumRequested(num1 != 0, num2 != 0);
      operationContext = operationContext ?? new OperationContext();
      Stream seekableStream = blockData;
      bool seekableStreamCreated = false;
      long num3;
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
            seekableStream = (Stream) new MultiBufferMemoryStream(__nonvirtual (cloudAppendBlob.ServiceClient).BufferManager);
            seekableStreamCreated = true;
            toStream = seekableStream;
          }
          long startPosition = seekableStream.Position;
          StreamDescriptor streamCopyState = new StreamDescriptor();
          // ISSUE: explicit non-virtual call
          await blockData.WriteToAsync<NullType>(toStream, __nonvirtual (cloudAppendBlob.ServiceClient).BufferManager, new long?(), new long?(4194304L), requiresContentChecksum, temporaryExecutionState, streamCopyState, cancellationToken).ConfigureAwait(false);
          seekableStream.Position = startPosition;
          contentChecksum = new Checksum(requiresContentChecksum.MD5 ? streamCopyState.Md5 : (string) null, requiresContentChecksum.CRC64 ? streamCopyState.Crc64 : (string) null);
          streamCopyState = (StreamDescriptor) null;
        }
        num3 = await Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<long>(cloudAppendBlob.AppendBlockImpl(new AggregatingProgressIncrementer(progressHandler).CreateProgressIncrementingStream(seekableStream), contentChecksum, accessCondition, modifiedOptions), modifiedOptions.RetryPolicy, operationContext, cancellationToken).ConfigureAwait(false);
      }
      finally
      {
        if (seekableStreamCreated)
          seekableStream.Dispose();
      }
      modifiedOptions = (BlobRequestOptions) null;
      seekableStream = (Stream) null;
      return num3;
    }

    [DoesServiceRequest]
    public virtual Task<long> AppendBlockAsync(
      Uri sourceUri,
      long offset,
      long count,
      string sourceContentMd5,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.AppendBlockAsync(sourceUri, offset, count, new Checksum(sourceContentMd5), sourceAccessCondition, destAccessCondition, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    private Task<long> AppendBlockAsync(
      Uri sourceUri,
      long offset,
      long count,
      Checksum sourceContentChecksum,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (sourceUri), (object) sourceUri);
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<long>(this.AppendBlockImpl(sourceUri, offset, count, sourceContentChecksum, sourceAccessCondition, destAccessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
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
    public virtual string StartCopy(
      CloudAppendBlob source,
      AccessCondition sourceAccessCondition = null,
      AccessCondition destAccessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return this.StartCopy(CloudBlob.SourceBlobToUri((CloudBlob) source), sourceAccessCondition, destAccessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginStartCopy(
      CloudAppendBlob source,
      AsyncCallback callback,
      object state)
    {
      return this.BeginStartCopy(CloudBlob.SourceBlobToUri((CloudBlob) source), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginStartCopy(
      CloudAppendBlob source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginStartCopy(CloudBlob.SourceBlobToUri((CloudBlob) source), sourceAccessCondition, destAccessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(CloudAppendBlob source) => this.StartCopyAsync(source, (AccessCondition) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      CloudAppendBlob source,
      CancellationToken cancellationToken)
    {
      return this.StartCopyAsync(source, (AccessCondition) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      CloudAppendBlob source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.StartCopyAsync(source, sourceAccessCondition, destAccessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      CloudAppendBlob source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.StartCopyAsync(CloudBlob.SourceBlobToUri((CloudBlob) source), sourceAccessCondition, destAccessCondition, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual CloudAppendBlob CreateSnapshot(
      IDictionary<string, string> metadata = null,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.AppendBlob, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<CloudAppendBlob>(this.CreateSnapshotImpl(metadata, accessCondition, options1), options1.RetryPolicy, operationContext);
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
      return CancellableAsyncResultTaskWrapper.Create<CloudAppendBlob>((Func<CancellationToken, Task<CloudAppendBlob>>) (token => this.CreateSnapshotAsync(metadata, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual CloudAppendBlob EndCreateSnapshot(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<CloudAppendBlob>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<CloudAppendBlob> CreateSnapshotAsync() => this.CreateSnapshotAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<CloudAppendBlob> CreateSnapshotAsync(CancellationToken cancellationToken) => this.CreateSnapshotAsync((IDictionary<string, string>) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<CloudAppendBlob> CreateSnapshotAsync(
      IDictionary<string, string> metadata,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.CreateSnapshotAsync(metadata, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<CloudAppendBlob> CreateSnapshotAsync(
      IDictionary<string, string> metadata,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.AppendBlob, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<CloudAppendBlob>(this.CreateSnapshotImpl(metadata, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    private RESTCommand<NullType> CreateImpl(
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        BlobRequest.VerifyHttpsCustomerProvidedKey(uri, options);
        StorageRequestMessage request = BlobHttpRequestMessageFactory.Put(uri, serverTimeout, this.Properties, BlobType.AppendBlob, 0L, new PremiumPageBlobTier?(), accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options);
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
        this.Properties.Length = 0L;
        BlobResponse.ValidateCPKHeaders(resp, options, true);
        return NullType.Value;
      });
      return cmd1;
    }

    internal RESTCommand<long> AppendBlockImpl(
      Stream source,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      options.AssertNoEncryptionPolicyOrStrictMode();
      long offset = source.Position;
      long length = source.Length - offset;
      RESTCommand<long> cmd1 = new RESTCommand<long>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<long>(cmd1);
      cmd1.BuildContent = (Func<RESTCommand<long>, OperationContext, HttpContent>) ((cmd, ctx) => HttpContentFactory.BuildContentFromStream<long>(source, offset, new long?(length), contentChecksum, cmd, ctx));
      cmd1.BuildRequest = (Func<RESTCommand<long>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        BlobRequest.VerifyHttpsCustomerProvidedKey(uri, options);
        return BlobHttpRequestMessageFactory.AppendBlock(uri, serverTimeout, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options);
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<long>, HttpResponseMessage, Exception, OperationContext, long>) ((cmd, resp, ex, ctx) =>
      {
        long retVal = -1;
        if (resp.Headers.Contains("x-ms-blob-append-offset"))
          retVal = long.Parse(resp.Headers.GetHeaderSingleValueOrDefault("x-ms-blob-append-offset"), (IFormatProvider) CultureInfo.InvariantCulture);
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<long>(HttpStatusCode.Created, resp, retVal, (StorageCommandBase<long>) cmd, ex);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(this.attributes, resp, false);
        cmd.CurrentResult.IsRequestServerEncrypted = HttpResponseParsers.ParseServerRequestEncrypted(resp);
        cmd.CurrentResult.EncryptionKeySHA256 = HttpResponseParsers.ParseEncryptionKeySHA256(resp);
        BlobResponse.ValidateCPKHeaders(resp, options, true);
        cmd.CurrentResult.EncryptionScope = HttpResponseParsers.ParseEncryptionScope(resp);
        return retVal;
      });
      return cmd1;
    }

    internal RESTCommand<long> AppendBlockImpl(
      Uri sourceUri,
      long offset,
      long count,
      Checksum sourceContentChecksum,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<long> cmd1 = new RESTCommand<long>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<long>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<long>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        BlobRequest.VerifyHttpsCustomerProvidedKey(uri, options);
        return BlobHttpRequestMessageFactory.AppendBlock(uri, sourceUri, new long?(offset), new long?(count), sourceContentChecksum, serverTimeout, sourceAccessCondition, destAccessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options);
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<long>, HttpResponseMessage, Exception, OperationContext, long>) ((cmd, resp, ex, ctx) =>
      {
        long retVal = -1;
        if (resp.Headers.Contains("x-ms-blob-append-offset"))
          retVal = long.Parse(resp.Headers.GetHeaderSingleValueOrDefault("x-ms-blob-append-offset"));
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<long>(HttpStatusCode.Created, resp, retVal, (StorageCommandBase<long>) cmd, ex);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(this.attributes, resp, false);
        cmd.CurrentResult.IsRequestServerEncrypted = HttpResponseParsers.ParseServerRequestEncrypted(resp);
        cmd.CurrentResult.EncryptionKeySHA256 = HttpResponseParsers.ParseEncryptionKeySHA256(resp);
        BlobResponse.ValidateCPKHeaders(resp, options, true);
        cmd.CurrentResult.EncryptionScope = HttpResponseParsers.ParseEncryptionScope(resp);
        return retVal;
      });
      return cmd1;
    }

    internal RESTCommand<CloudAppendBlob> CreateSnapshotImpl(
      IDictionary<string, string> metadata,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<CloudAppendBlob> cmd1 = new RESTCommand<CloudAppendBlob>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<CloudAppendBlob>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<CloudAppendBlob>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = BlobHttpRequestMessageFactory.Snapshot(uri, serverTimeout, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options);
        if (metadata != null)
          BlobHttpRequestMessageFactory.AddMetadata(request, metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<CloudAppendBlob>, HttpResponseMessage, Exception, OperationContext, CloudAppendBlob>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<CloudAppendBlob>(HttpStatusCode.Created, resp, (CloudAppendBlob) null, (StorageCommandBase<CloudAppendBlob>) cmd, ex);
        CloudAppendBlob snapshotImpl = new CloudAppendBlob(this.Name, new DateTimeOffset?(NavigationHelper.ParseSnapshotTime(BlobHttpResponseParsers.GetSnapshotTime(resp))), this.Container);
        snapshotImpl.attributes.Metadata = (IDictionary<string, string>) new Dictionary<string, string>(metadata ?? this.Metadata, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        snapshotImpl.attributes.Properties = new BlobProperties(this.Properties);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(snapshotImpl.attributes, resp, false);
        return snapshotImpl;
      });
      return cmd1;
    }

    public CloudAppendBlob(Uri blobAbsoluteUri)
      : this(blobAbsoluteUri, (StorageCredentials) null)
    {
    }

    public CloudAppendBlob(Uri blobAbsoluteUri, StorageCredentials credentials)
      : this(blobAbsoluteUri, new DateTimeOffset?(), credentials)
    {
    }

    public CloudAppendBlob(Uri blobAbsoluteUri, CloudBlobClient client)
      : this(blobAbsoluteUri, new DateTimeOffset?(), client)
    {
    }

    public CloudAppendBlob(
      Uri blobAbsoluteUri,
      DateTimeOffset? snapshotTime,
      StorageCredentials credentials)
      : this(new StorageUri(blobAbsoluteUri), snapshotTime, credentials)
    {
    }

    public CloudAppendBlob(
      Uri blobAbsoluteUri,
      DateTimeOffset? snapshotTime,
      CloudBlobClient client)
      : this(new StorageUri(blobAbsoluteUri), snapshotTime, client)
    {
    }

    public CloudAppendBlob(
      StorageUri blobAbsoluteUri,
      DateTimeOffset? snapshotTime,
      StorageCredentials credentials)
      : base(blobAbsoluteUri, snapshotTime, credentials)
    {
      this.Properties.BlobType = BlobType.AppendBlob;
    }

    public CloudAppendBlob(
      StorageUri blobAbsoluteUri,
      DateTimeOffset? snapshotTime,
      CloudBlobClient client)
      : base(blobAbsoluteUri, snapshotTime, client)
    {
      this.Properties.BlobType = BlobType.AppendBlob;
    }

    internal CloudAppendBlob(
      string blobName,
      DateTimeOffset? snapshotTime,
      CloudBlobContainer container)
      : base(blobName, snapshotTime, container)
    {
      this.Properties.BlobType = BlobType.AppendBlob;
    }

    internal CloudAppendBlob(BlobAttributes attributes, CloudBlobClient serviceClient)
      : base(attributes, serviceClient)
    {
      this.Properties.BlobType = BlobType.AppendBlob;
    }

    public int StreamWriteSizeInBytes
    {
      get => this.streamWriteSizeInBytes;
      set
      {
        CommonUtility.AssertInBounds<long>(nameof (StreamWriteSizeInBytes), (long) value, 16384L, 4194304L);
        this.streamWriteSizeInBytes = value;
      }
    }
  }
}
