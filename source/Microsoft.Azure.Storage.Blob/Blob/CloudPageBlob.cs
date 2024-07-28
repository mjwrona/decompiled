// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.CloudPageBlob
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
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Blob
{
  public class CloudPageBlob : CloudBlob, ICloudBlob, IListBlobItem
  {
    private int streamWriteSizeInBytes = 4194304;

    [DoesServiceRequest]
    public virtual CloudBlobStream OpenWrite(
      long? size,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return this.OpenWrite(size, new PremiumPageBlobTier?(), accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    internal virtual CloudBlobStream OpenWrite(
      long? size,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, this.BlobType, this.ServiceClient, false);
      bool hasValue = size.HasValue;
      ICryptoTransform transform = (ICryptoTransform) null;
      options1.AssertPolicyIfRequired();
      if (options1.EncryptionPolicy != null)
        transform = options1.EncryptionPolicy.CreateAndSetEncryptionContext(this.Metadata, true);
      if (hasValue)
      {
        this.Create(size.Value, premiumPageBlobTier, accessCondition, options, operationContext);
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
        size = new long?(this.Properties.Length);
      }
      if (accessCondition != null)
        accessCondition = AccessCondition.GenerateLeaseCondition(accessCondition.LeaseId);
      return options1.EncryptionPolicy != null ? (CloudBlobStream) new BlobEncryptedWriteStream(this, size.Value, hasValue, accessCondition, options1, operationContext, transform) : (CloudBlobStream) new BlobWriteStream(this, size.Value, hasValue, accessCondition, options1, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginOpenWrite(
      long? size,
      AsyncCallback callback,
      object state)
    {
      return this.BeginOpenWrite(size, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginOpenWrite(
      long? size,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginOpenWrite(size, new PremiumPageBlobTier?(), accessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    internal virtual ICancellableAsyncResult BeginOpenWrite(
      long? size,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<CloudBlobStream>((Func<CancellationToken, Task<CloudBlobStream>>) (token => this.OpenWriteAsync(size, premiumPageBlobTier, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual CloudBlobStream EndOpenWrite(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<CloudBlobStream>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<CloudBlobStream> OpenWriteAsync(long? size) => this.OpenWriteAsync(size, new PremiumPageBlobTier?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<CloudBlobStream> OpenWriteAsync(
      long? size,
      CancellationToken cancellationToken)
    {
      return this.OpenWriteAsync(size, new PremiumPageBlobTier?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<CloudBlobStream> OpenWriteAsync(
      long? size,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.OpenWriteAsync(size, new PremiumPageBlobTier?(), accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<CloudBlobStream> OpenWriteAsync(
      long? size,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.OpenWriteAsync(size, new PremiumPageBlobTier?(), accessCondition, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task<CloudBlobStream> OpenWriteAsync(
      long? size,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CloudPageBlob pageBlob = this;
      pageBlob.attributes.AssertNoSnapshot();
      // ISSUE: explicit non-virtual call
      // ISSUE: explicit non-virtual call
      BlobRequestOptions modifiedOptions = BlobRequestOptions.ApplyDefaults(options, __nonvirtual (pageBlob.BlobType), __nonvirtual (pageBlob.ServiceClient), false);
      bool createNew = size.HasValue;
      ICryptoTransform transform = (ICryptoTransform) null;
      modifiedOptions.AssertPolicyIfRequired();
      if (modifiedOptions.EncryptionPolicy != null)
      {
        // ISSUE: explicit non-virtual call
        transform = modifiedOptions.EncryptionPolicy.CreateAndSetEncryptionContext(__nonvirtual (pageBlob.Metadata), true);
      }
      if (createNew)
      {
        await pageBlob.CreateAsync(size.Value, premiumPageBlobTier, accessCondition, options, operationContext, cancellationToken).ConfigureAwait(false);
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
        await pageBlob.FetchAttributesAsync(accessCondition, options, operationContext, cancellationToken).ConfigureAwait(false);
        // ISSUE: explicit non-virtual call
        size = new long?(__nonvirtual (pageBlob.Properties).Length);
      }
      if (accessCondition != null)
        accessCondition = AccessCondition.GenerateLeaseCondition(accessCondition.LeaseId);
      CloudBlobStream cloudBlobStream = modifiedOptions.EncryptionPolicy == null ? (CloudBlobStream) new BlobWriteStream(pageBlob, size.Value, createNew, accessCondition, modifiedOptions, operationContext) : (CloudBlobStream) new BlobEncryptedWriteStream(pageBlob, size.Value, createNew, accessCondition, modifiedOptions, operationContext, transform);
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
      this.UploadFromStreamHelper(source, new long?(), new PremiumPageBlobTier?(), accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual void UploadFromStream(
      Stream source,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.UploadFromStreamHelper(source, new long?(), premiumPageBlobTier, accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual void UploadFromStream(
      Stream source,
      long length,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.UploadFromStreamHelper(source, new long?(length), new PremiumPageBlobTier?(), accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual void UploadFromStream(
      Stream source,
      long length,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.UploadFromStreamHelper(source, new long?(length), premiumPageBlobTier, accessCondition, options, operationContext);
    }

    internal void UploadFromStreamHelper(
      Stream source,
      long? length,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      CommonUtility.AssertNotNull(nameof (source), (object) source);
      if (!source.CanSeek)
        throw new InvalidOperationException();
      if (length.HasValue)
        CommonUtility.AssertInBounds<long>(nameof (length), length.Value, 1L, source.Length - source.Position);
      else
        length = new long?(source.Length - source.Position);
      long? nullable1 = length;
      long num1 = 512;
      long? nullable2 = nullable1.HasValue ? new long?(nullable1.GetValueOrDefault() % num1) : new long?();
      long num2 = 0;
      if (!(nullable2.GetValueOrDefault() == num2 & nullable2.HasValue))
        throw new ArgumentException("Page data must be a multiple of 512 bytes.", nameof (source));
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      using (CloudBlobStream toStream = this.OpenWrite(length, premiumPageBlobTier, accessCondition, options1, operationContext))
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
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsync(source, new PremiumPageBlobTier?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, token)), callback, state);
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
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsync(source, new PremiumPageBlobTier?(), accessCondition, options, operationContext, (IProgress<StorageProgress>) null, token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsync(source, premiumPageBlobTier, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, token)), callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsync(source, premiumPageBlobTier, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      long length,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsync(source, length, new PremiumPageBlobTier?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, token)), callback, state);
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
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsync(source, length, new PremiumPageBlobTier?(), accessCondition, options, operationContext, token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      long length,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsync(source, length, premiumPageBlobTier, accessCondition, options, operationContext, token)), callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      long length,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsync(source, length, premiumPageBlobTier, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual void EndUploadFromStream(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(Stream source) => this.UploadFromStreamAsyncHelper(source, new long?(), new PremiumPageBlobTier?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(Stream source, CancellationToken cancellationToken) => this.UploadFromStreamAsyncHelper(source, new long?(), new PremiumPageBlobTier?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(), new PremiumPageBlobTier?(), accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(), new PremiumPageBlobTier?(), accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(), premiumPageBlobTier, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(), premiumPageBlobTier, accessCondition, options, operationContext, progressHandler, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(Stream source, long length) => this.UploadFromStreamAsync(source, length, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      long length,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(length), new PremiumPageBlobTier?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(length), new PremiumPageBlobTier?(), accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
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
      return this.UploadFromStreamAsyncHelper(source, new long?(length), new PremiumPageBlobTier?(), accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      long length,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(length), premiumPageBlobTier, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      long length,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(length), premiumPageBlobTier, accessCondition, options, operationContext, progressHandler, cancellationToken);
    }

    [DoesServiceRequest]
    private async Task UploadFromStreamAsyncHelper(
      Stream source,
      long? length,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      CloudPageBlob cloudPageBlob = this;
      CommonUtility.AssertNotNull(nameof (source), (object) source);
      if (!source.CanSeek)
        throw new InvalidOperationException();
      if (length.HasValue)
        CommonUtility.AssertInBounds<long>(nameof (length), length.Value, 1L, source.Length - source.Position);
      else
        length = new long?(source.Length - source.Position);
      long? nullable1 = length;
      long num1 = 512;
      long? nullable2 = nullable1.HasValue ? new long?(nullable1.GetValueOrDefault() % num1) : new long?();
      long num2 = 0;
      if (!(nullable2.GetValueOrDefault() == num2 & nullable2.HasValue))
        throw new ArgumentException("Page data must be a multiple of 512 bytes.", nameof (source));
      cloudPageBlob.attributes.AssertNoSnapshot();
      // ISSUE: explicit non-virtual call
      BlobRequestOptions modifiedOptions = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, __nonvirtual (cloudPageBlob.ServiceClient));
      operationContext = operationContext ?? new OperationContext();
      using (CloudBlobStream blobStream = await cloudPageBlob.OpenWriteAsync(length, premiumPageBlobTier, accessCondition, modifiedOptions, operationContext, cancellationToken).ConfigureAwait(false))
      {
        using (ExecutionState<NullType> tempExecutionState = BlobCommonUtility.CreateTemporaryExecutionState(modifiedOptions))
        {
          Stream stream = source;
          Stream incrementingStream = new AggregatingProgressIncrementer(progressHandler).CreateProgressIncrementingStream((Stream) blobStream);
          // ISSUE: explicit non-virtual call
          IBufferManager bufferManager = __nonvirtual (cloudPageBlob.ServiceClient).BufferManager;
          long? copyLength = length;
          nullable2 = new long?();
          long? maxLength = nullable2;
          ChecksumRequested none = ChecksumRequested.None;
          ExecutionState<NullType> executionState = tempExecutionState;
          CancellationToken cancellationToken1 = cancellationToken;
          ConfiguredTaskAwaitable configuredTaskAwaitable = stream.WriteToAsync<NullType>(incrementingStream, bufferManager, copyLength, maxLength, none, executionState, (StreamDescriptor) null, cancellationToken1).ConfigureAwait(false);
          await configuredTaskAwaitable;
          configuredTaskAwaitable = blobStream.CommitAsync().ConfigureAwait(false);
          await configuredTaskAwaitable;
        }
      }
      modifiedOptions = (BlobRequestOptions) null;
    }

    [DoesServiceRequest]
    public virtual void UploadFromFile(
      string path,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.UploadFromFile(path, new PremiumPageBlobTier?(), accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual void UploadFromFile(
      string path,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (path), (object) path);
      using (FileStream source = new FileStream(path, FileMode.Open, FileAccess.Read))
        this.UploadFromStream((Stream) source, premiumPageBlobTier, accessCondition, options, operationContext);
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
      return this.BeginUploadFromFile(path, new PremiumPageBlobTier?(), accessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromFile(
      string path,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginUploadFromFile(path, premiumPageBlobTier, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginUploadFromFile(
      string path,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromFileAsync(path, premiumPageBlobTier, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual void EndUploadFromFile(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task UploadFromFileAsync(string path) => this.UploadFromFileAsync(path, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task UploadFromFileAsync(string path, CancellationToken cancellationToken) => this.UploadFromFileAsync(path, new PremiumPageBlobTier?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task UploadFromFileAsync(
      string path,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.UploadFromFileAsync(path, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromFileAsync(
      string path,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadFromFileAsync(path, new PremiumPageBlobTier?(), accessCondition, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromFileAsync(
      string path,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadFromFileAsync(path, premiumPageBlobTier, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task UploadFromFileAsync(
      string path,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (path), (object) path);
      using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
        await this.UploadFromStreamAsync((Stream) fileStream, premiumPageBlobTier, accessCondition, options, operationContext, progressHandler, cancellationToken).ConfigureAwait(false);
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
      this.UploadFromByteArray(buffer, index, count, new PremiumPageBlobTier?(), accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual void UploadFromByteArray(
      byte[] buffer,
      int index,
      int count,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (buffer), (object) buffer);
      using (SyncMemoryStream source = new SyncMemoryStream(buffer, index, count))
        this.UploadFromStream((Stream) source, premiumPageBlobTier, accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromByteArray(
      byte[] buffer,
      int index,
      int count,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromByteArrayAsync(buffer, index, count, new PremiumPageBlobTier?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, token)), callback, state);
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
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromByteArrayAsync(buffer, index, count, new PremiumPageBlobTier?(), accessCondition, options, operationContext, (IProgress<StorageProgress>) null, token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromByteArray(
      byte[] buffer,
      int index,
      int count,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromByteArrayAsync(buffer, index, count, premiumPageBlobTier, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, token)), callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginUploadFromByteArray(
      byte[] buffer,
      int index,
      int count,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromByteArrayAsync(buffer, index, count, premiumPageBlobTier, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual void EndUploadFromByteArray(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task UploadFromByteArrayAsync(byte[] buffer, int index, int count) => this.UploadFromByteArrayAsync(buffer, index, count, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task UploadFromByteArrayAsync(
      byte[] buffer,
      int index,
      int count,
      CancellationToken cancellationToken)
    {
      return this.UploadFromByteArrayAsync(buffer, index, count, new PremiumPageBlobTier?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);
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
      return this.UploadFromByteArrayAsync(buffer, index, count, new PremiumPageBlobTier?(), accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
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
      return this.UploadFromByteArrayAsync(buffer, index, count, new PremiumPageBlobTier?(), accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromByteArrayAsync(
      byte[] buffer,
      int index,
      int count,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadFromByteArrayAsync(buffer, index, count, premiumPageBlobTier, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task UploadFromByteArrayAsync(
      byte[] buffer,
      int index,
      int count,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (buffer), (object) buffer);
      using (SyncMemoryStream stream = new SyncMemoryStream(buffer, index, count))
        await this.UploadFromStreamAsync((Stream) stream, premiumPageBlobTier, accessCondition, options, operationContext, progressHandler, cancellationToken).ConfigureAwait(false);
    }

    [DoesServiceRequest]
    public virtual void Create(
      long size,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.Create(size, new PremiumPageBlobTier?(), accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual void Create(
      long size,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.CreateImpl(size, premiumPageBlobTier, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreate(
      long size,
      AsyncCallback callback,
      object state)
    {
      return this.BeginCreate(size, new PremiumPageBlobTier?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreate(
      long size,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginCreate(size, new PremiumPageBlobTier?(), accessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreate(
      long size,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.CreateAsync(size, premiumPageBlobTier, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndCreate(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task CreateAsync(long size) => this.CreateAsync(size, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task CreateAsync(long size, CancellationToken cancellationToken) => this.CreateAsync(size, new PremiumPageBlobTier?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task CreateAsync(
      long size,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.CreateAsync(size, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task CreateAsync(
      long size,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.CreateAsync(size, new PremiumPageBlobTier?(), accessCondition, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task CreateAsync(
      long size,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.CreateImpl(size, premiumPageBlobTier, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void Resize(
      long size,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.ResizeImpl(size, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginResize(
      long size,
      AsyncCallback callback,
      object state)
    {
      return this.BeginResize(size, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginResize(
      long size,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.ResizeAsync(size, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndResize(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task ResizeAsync(long size) => this.ResizeAsync(size, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task ResizeAsync(long size, CancellationToken cancellationToken) => this.ResizeAsync(size, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task ResizeAsync(
      long size,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.ResizeAsync(size, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task ResizeAsync(
      long size,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.ResizeImpl(size, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void SetSequenceNumber(
      SequenceNumberAction sequenceNumberAction,
      long? sequenceNumber,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.SetSequenceNumberImpl(sequenceNumberAction, sequenceNumber, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetSequenceNumber(
      SequenceNumberAction sequenceNumberAction,
      long? sequenceNumber,
      AsyncCallback callback,
      object state)
    {
      return this.BeginSetSequenceNumber(sequenceNumberAction, sequenceNumber, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetSequenceNumber(
      SequenceNumberAction sequenceNumberAction,
      long? sequenceNumber,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.SetSequenceNumberAsync(sequenceNumberAction, sequenceNumber, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndSetSequenceNumber(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task SetSequenceNumberAsync(
      SequenceNumberAction sequenceNumberAction,
      long? sequenceNumber)
    {
      return this.SetSequenceNumberAsync(sequenceNumberAction, sequenceNumber, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task SetSequenceNumberAsync(
      SequenceNumberAction sequenceNumberAction,
      long? sequenceNumber,
      CancellationToken cancellationToken)
    {
      return this.SetSequenceNumberAsync(sequenceNumberAction, sequenceNumber, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task SetSequenceNumberAsync(
      SequenceNumberAction sequenceNumberAction,
      long? sequenceNumber,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.SetSequenceNumberAsync(sequenceNumberAction, sequenceNumber, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task SetSequenceNumberAsync(
      SequenceNumberAction sequenceNumberAction,
      long? sequenceNumber,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.SetSequenceNumberImpl(sequenceNumberAction, sequenceNumber, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual IEnumerable<PageRange> GetPageRanges(
      long? offset = null,
      long? length = null,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<IEnumerable<PageRange>>(this.GetPageRangesImpl(offset, length, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetPageRanges(AsyncCallback callback, object state) => this.BeginGetPageRanges(new long?(), new long?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetPageRanges(
      long? offset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<IEnumerable<PageRange>>((Func<CancellationToken, Task<IEnumerable<PageRange>>>) (token => this.GetPageRangesAsync(offset, length, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual IEnumerable<PageRange> EndGetPageRanges(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<IEnumerable<PageRange>>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<IEnumerable<PageRange>> GetPageRangesAsync() => this.GetPageRangesAsync(new long?(), new long?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<IEnumerable<PageRange>> GetPageRangesAsync(
      CancellationToken cancellationToken)
    {
      return this.GetPageRangesAsync(new long?(), new long?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<IEnumerable<PageRange>> GetPageRangesAsync(
      long? offset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.GetPageRangesAsync(offset, length, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<IEnumerable<PageRange>> GetPageRangesAsync(
      long? offset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<IEnumerable<PageRange>>(this.GetPageRangesImpl(offset, length, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual IEnumerable<PageDiffRange> GetPageRangesDiff(
      DateTimeOffset previousSnapshotTime,
      long? offset = null,
      long? length = null,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<IEnumerable<PageDiffRange>>(this.GetPageRangesDiffImpl(previousSnapshotTime, offset, length, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetPageRangesDiff(
      DateTimeOffset previousSnapshotTime,
      AsyncCallback callback,
      object state)
    {
      return this.BeginGetPageRangesDiff(previousSnapshotTime, new long?(), new long?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginGetPageRangesDiff(
      DateTimeOffset previousSnapshotTime,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<IEnumerable<PageDiffRange>>((Func<CancellationToken, Task<IEnumerable<PageDiffRange>>>) (token => this.GetPageRangesDiffAsync(previousSnapshotTime, offset, length, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual IEnumerable<PageDiffRange> EndGetPageRangesDiff(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<IEnumerable<PageDiffRange>>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<IEnumerable<PageDiffRange>> GetPageRangesDiffAsync(
      DateTimeOffset previousSnapshotTime)
    {
      return this.GetPageRangesDiffAsync(previousSnapshotTime, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<IEnumerable<PageDiffRange>> GetPageRangesDiffAsync(
      DateTimeOffset previousSnapshotTime,
      CancellationToken cancellationToken)
    {
      return this.GetPageRangesDiffAsync(previousSnapshotTime, new long?(), new long?(), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<IEnumerable<PageDiffRange>> GetPageRangesDiffAsync(
      DateTimeOffset previousSnapshotTime,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.GetPageRangesDiffAsync(previousSnapshotTime, offset, length, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<IEnumerable<PageDiffRange>> GetPageRangesDiffAsync(
      DateTimeOffset previousSnapshotTime,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<IEnumerable<PageDiffRange>>(this.GetPageRangesDiffImpl(previousSnapshotTime, offset, length, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual CloudPageBlob CreateSnapshot(
      IDictionary<string, string> metadata = null,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<CloudPageBlob>(this.CreateSnapshotImpl(metadata, accessCondition, options1), options1.RetryPolicy, operationContext);
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
      return CancellableAsyncResultTaskWrapper.Create<CloudPageBlob>((Func<CancellationToken, Task<CloudPageBlob>>) (token => this.CreateSnapshotAsync(metadata, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual CloudPageBlob EndCreateSnapshot(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<CloudPageBlob>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<CloudPageBlob> CreateSnapshotAsync() => this.CreateSnapshotAsync((IDictionary<string, string>) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<CloudPageBlob> CreateSnapshotAsync(CancellationToken cancellationToken) => this.CreateSnapshotAsync((IDictionary<string, string>) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<CloudPageBlob> CreateSnapshotAsync(
      IDictionary<string, string> metadata,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.CreateSnapshotAsync(metadata, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<CloudPageBlob> CreateSnapshotAsync(
      IDictionary<string, string> metadata,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<CloudPageBlob>(this.CreateSnapshotImpl(metadata, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void WritePages(
      Stream pageData,
      long startOffset,
      Checksum contentChecksum = null,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (pageData), (object) pageData);
      contentChecksum = contentChecksum ?? Checksum.None;
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      ChecksumRequested calculateChecksum = new ChecksumRequested((contentChecksum == null || contentChecksum.MD5 == null) && options1.ChecksumOptions.UseTransactionalMD5.Value, (contentChecksum == null || contentChecksum.CRC64 == null) && options1.ChecksumOptions.UseTransactionalCRC64.Value);
      operationContext = operationContext ?? new OperationContext();
      Stream pageData1 = pageData;
      bool flag = false;
      try
      {
        if (!pageData.CanSeek || calculateChecksum.HasAny)
        {
          ExecutionState<NullType> temporaryExecutionState = BlobCommonUtility.CreateTemporaryExecutionState(options1);
          Stream toStream;
          if (pageData.CanSeek)
          {
            toStream = Stream.Null;
          }
          else
          {
            pageData1 = (Stream) new MultiBufferMemoryStream(this.ServiceClient.BufferManager);
            flag = true;
            toStream = pageData1;
          }
          long position = pageData1.Position;
          StreamDescriptor streamCopyState = new StreamDescriptor();
          pageData.WriteToSync<NullType>(toStream, new long?(), new long?(104857600L), calculateChecksum, true, temporaryExecutionState, streamCopyState);
          pageData1.Position = position;
          contentChecksum = new Checksum(calculateChecksum.MD5 ? streamCopyState.Md5 : (string) null, calculateChecksum.CRC64 ? streamCopyState.Crc64 : (string) null);
        }
        Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.PutPageImpl(pageData1, startOffset, contentChecksum, accessCondition, options1), options1.RetryPolicy, operationContext);
      }
      finally
      {
        if (flag)
          pageData1.Dispose();
      }
    }

    [DoesServiceRequest]
    public virtual void WritePages(
      Uri sourceUri,
      long offset,
      long count,
      long startOffset,
      Checksum sourceContentChecksum = null,
      AccessCondition sourceAccessCondition = null,
      AccessCondition destAccessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (sourceUri), (object) sourceUri);
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.PutPageImpl(sourceUri, offset, count, startOffset, sourceContentChecksum, sourceAccessCondition, destAccessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginWritePages(
      Stream pageData,
      long startOffset,
      Checksum contentChecksum,
      AsyncCallback callback,
      object state)
    {
      return this.BeginWritePages(pageData, startOffset, contentChecksum, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginWritePages(
      Stream pageData,
      long startOffset,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginWritePages(pageData, startOffset, contentChecksum, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginWritePages(
      Uri sourceUri,
      long offset,
      long count,
      long startOffset,
      Checksum sourceContentChecksum,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.WritePagesAsync(sourceUri, offset, count, startOffset, sourceContentChecksum, sourceAccessCondition, destAccessCondition, options, operationContext, token)), callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginWritePages(
      Stream pageData,
      long startOffset,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.WritePagesAsync(pageData, startOffset, contentChecksum, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual void EndWritePages(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task WritePagesAsync(
      Stream pageData,
      long startOffset,
      Checksum contentChecksum)
    {
      return this.WritePagesAsync(pageData, startOffset, contentChecksum, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task WritePagesAsync(
      Stream pageData,
      long startOffset,
      Checksum contentChecksum,
      CancellationToken cancellationToken)
    {
      return this.WritePagesAsync(pageData, startOffset, contentChecksum, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task WritePagesAsync(
      Stream pageData,
      long startOffset,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.WritePagesAsync(pageData, startOffset, contentChecksum, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task WritePagesAsync(
      Stream pageData,
      long startOffset,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.WritePagesAsync(pageData, startOffset, contentChecksum, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task WritePagesAsync(
      Stream pageData,
      long startOffset,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      CloudPageBlob cloudPageBlob = this;
      CommonUtility.AssertNotNull(nameof (pageData), (object) pageData);
      contentChecksum = contentChecksum ?? Checksum.None;
      // ISSUE: explicit non-virtual call
      BlobRequestOptions modifiedOptions = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, __nonvirtual (cloudPageBlob.ServiceClient));
      bool? nullable;
      int num1;
      if (contentChecksum.MD5 == null)
      {
        nullable = modifiedOptions.ChecksumOptions.UseTransactionalMD5;
        num1 = nullable.Value ? 1 : 0;
      }
      else
        num1 = 0;
      int num2;
      if (contentChecksum.CRC64 == null)
      {
        nullable = modifiedOptions.ChecksumOptions.UseTransactionalCRC64;
        num2 = nullable.Value ? 1 : 0;
      }
      else
        num2 = 0;
      ChecksumRequested requiresContentChecksum = new ChecksumRequested(num1 != 0, num2 != 0);
      operationContext = operationContext ?? new OperationContext();
      Stream seekableStream = pageData;
      bool seekableStreamCreated = false;
      try
      {
        if (!pageData.CanSeek || requiresContentChecksum.HasAny)
        {
          ExecutionState<NullType> temporaryExecutionState = BlobCommonUtility.CreateTemporaryExecutionState(modifiedOptions);
          Stream toStream;
          if (pageData.CanSeek)
          {
            toStream = Stream.Null;
          }
          else
          {
            // ISSUE: explicit non-virtual call
            seekableStream = (Stream) new MultiBufferMemoryStream(__nonvirtual (cloudPageBlob.ServiceClient).BufferManager);
            seekableStreamCreated = true;
            toStream = seekableStream;
          }
          long startPosition = seekableStream.Position;
          StreamDescriptor streamCopyState = new StreamDescriptor();
          // ISSUE: explicit non-virtual call
          await pageData.WriteToAsync<NullType>(toStream, __nonvirtual (cloudPageBlob.ServiceClient).BufferManager, new long?(), new long?(104857600L), requiresContentChecksum, temporaryExecutionState, streamCopyState, cancellationToken).ConfigureAwait(false);
          seekableStream.Position = startPosition;
          contentChecksum = new Checksum(requiresContentChecksum.MD5 ? streamCopyState.Md5 : (string) null, requiresContentChecksum.CRC64 ? streamCopyState.Crc64 : (string) null);
          streamCopyState = (StreamDescriptor) null;
        }
        NullType nullType = await Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(cloudPageBlob.PutPageImpl(new AggregatingProgressIncrementer(progressHandler).CreateProgressIncrementingStream(seekableStream), startOffset, contentChecksum, accessCondition, modifiedOptions), modifiedOptions.RetryPolicy, operationContext, cancellationToken).ConfigureAwait(false);
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
    public virtual async Task WritePagesAsync(
      Uri sourceUri,
      long offset,
      long count,
      long startOffset,
      Checksum sourceContentChecksum,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CloudPageBlob cloudPageBlob = this;
      CommonUtility.AssertNotNull(nameof (sourceUri), (object) sourceUri);
      // ISSUE: explicit non-virtual call
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, __nonvirtual (cloudPageBlob.ServiceClient));
      operationContext = operationContext ?? new OperationContext();
      NullType nullType = await Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(cloudPageBlob.PutPageImpl(sourceUri, offset, count, startOffset, sourceContentChecksum, sourceAccessCondition, destAccessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken).ConfigureAwait(false);
    }

    [DoesServiceRequest]
    public virtual void ClearPages(
      long startOffset,
      long length,
      AccessCondition accessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.ClearPageImpl(startOffset, length, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginClearPages(
      long startOffset,
      long length,
      AsyncCallback callback,
      object state)
    {
      return this.BeginClearPages(startOffset, length, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginClearPages(
      long startOffset,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.ClearPagesAsync(startOffset, length, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndClearPages(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task ClearPagesAsync(long startOffset, long length) => this.ClearPagesAsync(startOffset, length, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task ClearPagesAsync(
      long startOffset,
      long length,
      CancellationToken cancellationToken)
    {
      return this.ClearPagesAsync(startOffset, length, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task ClearPagesAsync(
      long startOffset,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.ClearPagesAsync(startOffset, length, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task ClearPagesAsync(
      long startOffset,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.ClearPageImpl(startOffset, length, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual string StartCopy(
      CloudPageBlob source,
      AccessCondition sourceAccessCondition = null,
      AccessCondition destAccessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return this.StartCopy(CloudBlob.SourceBlobToUri((CloudBlob) source), sourceAccessCondition, destAccessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual string StartCopy(
      CloudPageBlob source,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition sourceAccessCondition = null,
      AccessCondition destAccessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return this.StartCopy(CloudBlob.SourceBlobToUri((CloudBlob) source), premiumPageBlobTier, new StandardBlobTier?(), new RehydratePriority?(), sourceAccessCondition, destAccessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual string StartIncrementalCopy(
      CloudPageBlob sourceSnapshot,
      AccessCondition destAccessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (sourceSnapshot), (object) sourceSnapshot);
      return this.StartIncrementalCopy(CloudBlob.SourceBlobToUri((CloudBlob) sourceSnapshot), destAccessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual string StartIncrementalCopy(
      Uri sourceSnapshotUri,
      AccessCondition destAccessCondition = null,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (sourceSnapshotUri), (object) sourceSnapshotUri);
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<string>(this.StartCopyImpl(this.attributes, sourceSnapshotUri, Checksum.None, true, false, new PremiumPageBlobTier?(), new StandardBlobTier?(), new RehydratePriority?(), (AccessCondition) null, destAccessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginStartCopy(
      CloudPageBlob source,
      AsyncCallback callback,
      object state)
    {
      return this.BeginStartCopy(CloudBlob.SourceBlobToUri((CloudBlob) source), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginStartIncrementalCopy(
      CloudPageBlob sourceSnapshot,
      AsyncCallback callback,
      object state)
    {
      return this.BeginStartIncrementalCopy(CloudBlob.SourceBlobToUri((CloudBlob) sourceSnapshot), (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginStartCopy(
      CloudPageBlob source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginStartCopy(source, new PremiumPageBlobTier?(), sourceAccessCondition, destAccessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginStartCopy(
      CloudPageBlob source,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginStartCopy(CloudBlob.SourceBlobToUri((CloudBlob) source), premiumPageBlobTier, new StandardBlobTier?(), sourceAccessCondition, destAccessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginStartIncrementalCopy(
      CloudPageBlob sourceSnapshot,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginStartIncrementalCopy(CloudBlob.SourceBlobToUri((CloudBlob) sourceSnapshot), destAccessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginStartIncrementalCopy(
      Uri sourceSnapshot,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<string>((Func<CancellationToken, Task<string>>) (token => this.StartIncrementalCopyAsync(sourceSnapshot, destAccessCondition, options, operationContext, token)), callback, state);
    }

    public virtual string EndStartIncrementalCopy(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<string>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(CloudPageBlob source) => this.StartCopyAsync(source, new PremiumPageBlobTier?(), (AccessCondition) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      CloudPageBlob source,
      CancellationToken cancellationToken)
    {
      return this.StartCopyAsync(source, new PremiumPageBlobTier?(), (AccessCondition) null, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      CloudPageBlob source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.StartCopyAsync(source, new PremiumPageBlobTier?(), sourceAccessCondition, destAccessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      CloudPageBlob source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.StartCopyAsync(source, new PremiumPageBlobTier?(), sourceAccessCondition, destAccessCondition, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      CloudPageBlob source,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.StartCopyAsync(CloudBlob.SourceBlobToUri((CloudBlob) source), premiumPageBlobTier, sourceAccessCondition, destAccessCondition, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartIncrementalCopyAsync(CloudPageBlob source) => this.StartIncrementalCopyAsync(source, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<string> StartIncrementalCopyAsync(
      CloudPageBlob source,
      CancellationToken cancellationToken)
    {
      return this.StartIncrementalCopyAsync(source, (AccessCondition) null, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartIncrementalCopyAsync(
      CloudPageBlob sourceSnapshot,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (sourceSnapshot), (object) sourceSnapshot);
      return this.StartIncrementalCopyAsync(CloudBlob.SourceBlobToUri((CloudBlob) sourceSnapshot), destAccessCondition, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartIncrementalCopyAsync(
      Uri sourceSnapshotUri,
      AccessCondition destAccessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (sourceSnapshotUri), (object) sourceSnapshotUri);
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.Unspecified, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<string>(this.StartCopyImpl(this.attributes, sourceSnapshotUri, Checksum.None, true, false, new PremiumPageBlobTier?(), new StandardBlobTier?(), new RehydratePriority?(), (AccessCondition) null, destAccessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void SetPremiumBlobTier(
      PremiumPageBlobTier premiumPageBlobTier,
      BlobRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.SetBlobTierImpl(premiumPageBlobTier, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetPremiumBlobTier(
      PremiumPageBlobTier premiumPageBlobTier,
      AsyncCallback callback,
      object state)
    {
      return this.BeginSetPremiumBlobTier(premiumPageBlobTier, (BlobRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetPremiumBlobTier(
      PremiumPageBlobTier premiumPageBlobTier,
      BlobRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.SetPremiumBlobTierAsync(premiumPageBlobTier, options, operationContext, token)), callback, state);
    }

    public virtual void EndSetPremiumBlobTier(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task SetPremiumBlobTierAsync(PremiumPageBlobTier premiumPageBlobTier) => this.SetPremiumBlobTierAsync(premiumPageBlobTier, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task SetPremiumBlobTierAsync(
      PremiumPageBlobTier premiumPageBlobTier,
      CancellationToken cancellationToken)
    {
      return this.SetPremiumBlobTierAsync(premiumPageBlobTier, (BlobRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task SetPremiumBlobTierAsync(
      PremiumPageBlobTier premiumPageBlobTier,
      BlobRequestOptions options,
      OperationContext operationContext)
    {
      return this.SetPremiumBlobTierAsync(premiumPageBlobTier, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task SetPremiumBlobTierAsync(
      PremiumPageBlobTier premiumPageBlobTier,
      BlobRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.attributes.AssertNoSnapshot();
      BlobRequestOptions options1 = BlobRequestOptions.ApplyDefaults(options, BlobType.PageBlob, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.SetBlobTierImpl(premiumPageBlobTier, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    private RESTCommand<NullType> CreateImpl(
      long sizeInBytes,
      PremiumPageBlobTier? premiumPageBlobTier,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        BlobRequest.VerifyHttpsCustomerProvidedKey(uri, options);
        StorageRequestMessage request = BlobHttpRequestMessageFactory.Put(uri, serverTimeout, this.Properties, BlobType.PageBlob, sizeInBytes, premiumPageBlobTier, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options);
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
        this.Properties.Length = sizeInBytes;
        this.attributes.Properties.PremiumPageBlobTier = premiumPageBlobTier;
        if (premiumPageBlobTier.HasValue)
          this.attributes.Properties.BlobTierInferred = new bool?(false);
        BlobResponse.ValidateCPKHeaders(resp, options, true);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<NullType> ResizeImpl(
      long sizeInBytes,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.Resize(uri, serverTimeout, sizeInBytes, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(this.attributes, resp, false);
        this.Properties.Length = sizeInBytes;
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<NullType> SetSequenceNumberImpl(
      SequenceNumberAction sequenceNumberAction,
      long? sequenceNumber,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.SetSequenceNumber(uri, serverTimeout, sequenceNumberAction, sequenceNumber, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(this.attributes, resp, false);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<IEnumerable<PageRange>> GetPageRangesImpl(
      long? offset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<IEnumerable<PageRange>> cmd1 = new RESTCommand<IEnumerable<PageRange>>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<IEnumerable<PageRange>>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<IEnumerable<PageRange>>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.GetPageRanges(uri, serverTimeout, this.SnapshotTime, offset, length, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<IEnumerable<PageRange>>, HttpResponseMessage, Exception, OperationContext, IEnumerable<PageRange>>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<IEnumerable<PageRange>>(HttpStatusCode.OK, resp, (IEnumerable<PageRange>) null, (StorageCommandBase<IEnumerable<PageRange>>) cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<IEnumerable<PageRange>>, HttpResponseMessage, OperationContext, CancellationToken, Task<IEnumerable<PageRange>>>) ((cmd, resp, ctx, ct) =>
      {
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(this.attributes, resp, true);
        return GetPageRangesResponse.ParseAsync(cmd.ResponseStream, ct);
      });
      return cmd1;
    }

    private RESTCommand<IEnumerable<PageDiffRange>> GetPageRangesDiffImpl(
      DateTimeOffset previousSnapshotTime,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<IEnumerable<PageDiffRange>> cmd1 = new RESTCommand<IEnumerable<PageDiffRange>>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<IEnumerable<PageDiffRange>>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<IEnumerable<PageDiffRange>>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.GetPageRangesDiff(uri, serverTimeout, this.SnapshotTime, previousSnapshotTime, offset, length, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<IEnumerable<PageDiffRange>>, HttpResponseMessage, Exception, OperationContext, IEnumerable<PageDiffRange>>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<IEnumerable<PageDiffRange>>(HttpStatusCode.OK, resp, (IEnumerable<PageDiffRange>) null, (StorageCommandBase<IEnumerable<PageDiffRange>>) cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<IEnumerable<PageDiffRange>>, HttpResponseMessage, OperationContext, CancellationToken, Task<IEnumerable<PageDiffRange>>>) ((cmd, resp, ctx, ct) =>
      {
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(this.attributes, resp, true);
        return GetPageDiffRangesResponse.ParseAsync(cmd.ResponseStream, ct);
      });
      return cmd1;
    }

    private RESTCommand<NullType> PutPageImpl(
      Stream pageData,
      long startOffset,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      options.AssertNoEncryptionPolicyOrStrictMode();
      if (startOffset % 512L != 0L)
        CommonUtility.ArgumentOutOfRange(nameof (startOffset), (object) startOffset);
      long offset = pageData.Position;
      long length = pageData.Length - offset;
      PageRange pageRange = new PageRange(startOffset, startOffset + length - 1L);
      PageWrite pageWrite = PageWrite.Update;
      if ((1L + pageRange.EndOffset - pageRange.StartOffset) % 512L != 0L || 1L + pageRange.EndOffset - pageRange.StartOffset == 0L)
        CommonUtility.ArgumentOutOfRange(nameof (pageData), (object) pageData);
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildContent = (Func<RESTCommand<NullType>, OperationContext, HttpContent>) ((cmd, ctx) => HttpContentFactory.BuildContentFromStream<NullType>(pageData, offset, new long?(length), contentChecksum, cmd, ctx));
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        BlobRequest.VerifyHttpsCustomerProvidedKey(uri, options);
        return BlobHttpRequestMessageFactory.PutPage(uri, serverTimeout, pageRange, pageWrite, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options);
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Created, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(this.attributes, resp, false);
        cmd.CurrentResult.IsRequestServerEncrypted = HttpResponseParsers.ParseServerRequestEncrypted(resp);
        cmd.CurrentResult.EncryptionKeySHA256 = HttpResponseParsers.ParseEncryptionKeySHA256(resp);
        BlobResponse.ValidateCPKHeaders(resp, options, true);
        cmd.CurrentResult.EncryptionScope = HttpResponseParsers.ParseEncryptionScope(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<NullType> PutPageImpl(
      Uri sourceUri,
      long offset,
      long count,
      long startOffset,
      Checksum sourceContentChecksum,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      BlobRequestOptions options)
    {
      options.AssertNoEncryptionPolicyOrStrictMode();
      if (startOffset % 512L != 0L)
        CommonUtility.ArgumentOutOfRange(nameof (startOffset), (object) startOffset);
      PageRange pageRange = new PageRange(startOffset, startOffset + count - 1L);
      if ((1L + pageRange.EndOffset - pageRange.StartOffset) % 512L != 0L || 1L + pageRange.EndOffset - pageRange.StartOffset == 0L)
        CommonUtility.ArgumentOutOfRange("EndOffset", (object) pageRange.EndOffset);
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        BlobRequest.VerifyHttpsCustomerProvidedKey(uri, options);
        return BlobHttpRequestMessageFactory.PutPage(uri, sourceUri, new long?(offset), new long?(count), sourceContentChecksum, serverTimeout, pageRange, sourceAccessCondition, destAccessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options);
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Created, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(this.attributes, resp, false);
        cmd.CurrentResult.IsRequestServerEncrypted = HttpResponseParsers.ParseServerRequestEncrypted(resp);
        cmd.CurrentResult.EncryptionKeySHA256 = HttpResponseParsers.ParseEncryptionKeySHA256(resp);
        BlobResponse.ValidateCPKHeaders(resp, options, true);
        cmd.CurrentResult.EncryptionScope = HttpResponseParsers.ParseEncryptionScope(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<NullType> ClearPageImpl(
      long startOffset,
      long length,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      CommonUtility.AssertNotNull(nameof (options), (object) options);
      options.AssertNoEncryptionPolicyOrStrictMode();
      if (startOffset < 0L || startOffset % 512L != 0L)
        CommonUtility.ArgumentOutOfRange(nameof (startOffset), (object) startOffset);
      if (length <= 0L || length % 512L != 0L)
        CommonUtility.ArgumentOutOfRange(nameof (length), (object) length);
      PageRange pageRange = new PageRange(startOffset, startOffset + length - 1L);
      PageWrite pageWrite = PageWrite.Clear;
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        BlobRequest.VerifyHttpsCustomerProvidedKey(uri, options);
        return BlobHttpRequestMessageFactory.PutPage(uri, serverTimeout, pageRange, pageWrite, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options);
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Created, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(this.attributes, resp, false);
        return NullType.Value;
      });
      return cmd1;
    }

    internal RESTCommand<CloudPageBlob> CreateSnapshotImpl(
      IDictionary<string, string> metadata,
      AccessCondition accessCondition,
      BlobRequestOptions options)
    {
      RESTCommand<CloudPageBlob> cmd1 = new RESTCommand<CloudPageBlob>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<CloudPageBlob>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<CloudPageBlob>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = BlobHttpRequestMessageFactory.Snapshot(uri, serverTimeout, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials, options);
        if (metadata != null)
          BlobHttpRequestMessageFactory.AddMetadata(request, metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<CloudPageBlob>, HttpResponseMessage, Exception, OperationContext, CloudPageBlob>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<CloudPageBlob>(HttpStatusCode.Created, resp, (CloudPageBlob) null, (StorageCommandBase<CloudPageBlob>) cmd, ex);
        CloudPageBlob snapshotImpl = new CloudPageBlob(this.Name, new DateTimeOffset?(NavigationHelper.ParseSnapshotTime(BlobHttpResponseParsers.GetSnapshotTime(resp))), this.Container);
        snapshotImpl.attributes.Metadata = (IDictionary<string, string>) new Dictionary<string, string>(metadata ?? this.Metadata, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        snapshotImpl.attributes.Properties = new BlobProperties(this.Properties);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(snapshotImpl.attributes, resp, false);
        return snapshotImpl;
      });
      return cmd1;
    }

    internal RESTCommand<NullType> SetBlobTierImpl(
      PremiumPageBlobTier premiumPageBlobTier,
      BlobRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => BlobHttpRequestMessageFactory.SetBlobTier(uri, serverTimeout, premiumPageBlobTier.ToString(), new RehydratePriority?(), cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        CloudBlob.UpdateETagLMTLengthAndSequenceNumber(this.attributes, resp, false);
        this.attributes.Properties.PremiumPageBlobTier = new PremiumPageBlobTier?(premiumPageBlobTier);
        this.attributes.Properties.BlobTierInferred = new bool?(false);
        return NullType.Value;
      });
      return cmd1;
    }

    public CloudPageBlob(Uri blobAbsoluteUri)
      : this(blobAbsoluteUri, (StorageCredentials) null)
    {
    }

    public CloudPageBlob(Uri blobAbsoluteUri, StorageCredentials credentials)
      : this(blobAbsoluteUri, new DateTimeOffset?(), credentials)
    {
    }

    public CloudPageBlob(Uri blobAbsoluteUri, CloudBlobClient client)
      : this(blobAbsoluteUri, new DateTimeOffset?(), client)
    {
    }

    public CloudPageBlob(
      Uri blobAbsoluteUri,
      DateTimeOffset? snapshotTime,
      StorageCredentials credentials)
      : this(new StorageUri(blobAbsoluteUri), snapshotTime, credentials)
    {
    }

    public CloudPageBlob(Uri blobAbsoluteUri, DateTimeOffset? snapshotTime, CloudBlobClient client)
      : this(new StorageUri(blobAbsoluteUri), snapshotTime, client)
    {
    }

    public CloudPageBlob(
      StorageUri blobAbsoluteUri,
      DateTimeOffset? snapshotTime,
      StorageCredentials credentials)
      : base(blobAbsoluteUri, snapshotTime, credentials)
    {
      this.Properties.BlobType = BlobType.PageBlob;
    }

    public CloudPageBlob(
      StorageUri blobAbsoluteUri,
      DateTimeOffset? snapshotTime,
      CloudBlobClient client)
      : base(blobAbsoluteUri, snapshotTime, client)
    {
      this.Properties.BlobType = BlobType.PageBlob;
    }

    internal CloudPageBlob(
      string blobName,
      DateTimeOffset? snapshotTime,
      CloudBlobContainer container)
      : base(blobName, snapshotTime, container)
    {
      this.Properties.BlobType = BlobType.PageBlob;
    }

    internal CloudPageBlob(BlobAttributes attributes, CloudBlobClient serviceClient)
      : base(attributes, serviceClient)
    {
      this.Properties.BlobType = BlobType.PageBlob;
    }

    public int StreamWriteSizeInBytes
    {
      get => this.streamWriteSizeInBytes;
      set
      {
        CommonUtility.AssertInBounds<int>(nameof (StreamWriteSizeInBytes), value, 512, 104857600);
        this.streamWriteSizeInBytes = value;
      }
    }
  }
}
