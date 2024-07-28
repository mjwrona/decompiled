// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.File.CloudFile
// Assembly: Microsoft.Azure.Storage.File, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: C68E95B0-8DFB-410C-8E70-706406D1A279
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.File.dll

using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Core;
using Microsoft.Azure.Storage.Core.Auth;
using Microsoft.Azure.Storage.Core.Executor;
using Microsoft.Azure.Storage.Core.Util;
using Microsoft.Azure.Storage.File.Protocol;
using Microsoft.Azure.Storage.Shared.Protocol;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.File
{
  public class CloudFile : IListFileItem
  {
    private int streamWriteSizeInBytes = 4194304;
    private int streamMinimumReadSizeInBytes = 4194304;
    private string filePermission;
    private CloudFileShare share;
    private CloudFileDirectory parent;
    internal CloudFileAttributes attributes;

    [DoesServiceRequest]
    public virtual Stream OpenRead(
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      operationContext = operationContext ?? new OperationContext();
      this.FetchAttributes(accessCondition, options, operationContext);
      return (Stream) new FileReadStream(this, AccessCondition.CloneConditionWithETag(accessCondition, this.Properties.ETag), FileRequestOptions.ApplyDefaults(options, this.ServiceClient, false), operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginOpenRead(AsyncCallback callback, object state) => this.BeginOpenRead((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginOpenRead(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<Stream>((Func<CancellationToken, Task<Stream>>) (token => this.OpenReadAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual Stream EndOpenRead(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<Stream>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<Stream> OpenReadAsync() => this.OpenReadAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<Stream> OpenReadAsync(CancellationToken cancellationToken) => this.OpenReadAsync((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<Stream> OpenReadAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.OpenReadAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual async Task<Stream> OpenReadAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CloudFile file = this;
      operationContext = operationContext ?? new OperationContext();
      await file.FetchAttributesAsync(accessCondition, options, operationContext).ConfigureAwait(false);
      AccessCondition accessCondition1 = AccessCondition.CloneConditionWithETag(accessCondition, file.Properties.ETag);
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, file.ServiceClient, false);
      return (Stream) new FileReadStream(file, accessCondition1, options1, operationContext);
    }

    [DoesServiceRequest]
    public virtual CloudFileStream OpenWrite(
      long? size,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient, false);
      operationContext = operationContext ?? new OperationContext();
      bool hasValue = size.HasValue;
      if (hasValue)
      {
        this.Create(size.Value, accessCondition, options, operationContext);
      }
      else
      {
        if (options1.ChecksumOptions.StoreContentMD5.Value)
          throw new ArgumentException("MD5 cannot be calculated for an existing blob because it would require reading the existing data. Please disable StoreBlobContentMD5.");
        if (options1.ChecksumOptions.StoreContentCRC64.Value)
          throw new ArgumentException("CRC64 cannot be calculated for an existing blob because it would require reading the existing data. Please disable StoreBlobContentCRC64.");
        this.FetchAttributes(accessCondition, options, operationContext);
        size = new long?(this.Properties.Length);
      }
      if (accessCondition != null)
        accessCondition = AccessCondition.GenerateLeaseCondition(accessCondition.LeaseId);
      return (CloudFileStream) new FileWriteStream(this, size.Value, hasValue, accessCondition, options1, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginOpenWrite(
      long? size,
      AsyncCallback callback,
      object state)
    {
      return this.BeginOpenWrite(size, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginOpenWrite(
      long? size,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<CloudFileStream>((Func<CancellationToken, Task<CloudFileStream>>) (token => this.OpenWriteAsync(size, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual CloudFileStream EndOpenWrite(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<CloudFileStream>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<CloudFileStream> OpenWriteAsync(long? size) => this.OpenWriteAsync(size, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<CloudFileStream> OpenWriteAsync(
      long? size,
      CancellationToken cancellationToken)
    {
      return this.OpenWriteAsync(size, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<CloudFileStream> OpenWriteAsync(
      long? size,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.OpenWriteAsync(size, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual async Task<CloudFileStream> OpenWriteAsync(
      long? size,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      CloudFile file = this;
      file.AssertNoSnapshot();
      FileRequestOptions modifiedOptions = FileRequestOptions.ApplyDefaults(options, file.ServiceClient, false);
      operationContext = operationContext ?? new OperationContext();
      bool createNew = size.HasValue;
      bool? nullable;
      if (!createNew)
      {
        nullable = modifiedOptions.ChecksumOptions.StoreContentMD5;
        if (nullable.Value)
          throw new ArgumentException("MD5 cannot be calculated for an existing blob because it would require reading the existing data. Please disable StoreBlobContentMD5.");
      }
      if (!createNew)
      {
        nullable = modifiedOptions.ChecksumOptions.StoreContentCRC64;
        if (nullable.Value)
          throw new ArgumentException("CRC64 cannot be calculated for an existing blob because it would require reading the existing data. Please disable StoreBlobContentCRC64.");
      }
      if (createNew)
      {
        await file.CreateAsync(size.Value, accessCondition, options, operationContext, cancellationToken).ConfigureAwait(false);
      }
      else
      {
        await file.FetchAttributesAsync(accessCondition, options, operationContext, cancellationToken).ConfigureAwait(false);
        size = new long?(file.Properties.Length);
      }
      if (accessCondition != null)
        accessCondition = AccessCondition.GenerateLeaseCondition(accessCondition.LeaseId);
      CloudFileStream cloudFileStream = (CloudFileStream) new FileWriteStream(file, size.Value, createNew, accessCondition, modifiedOptions, operationContext);
      modifiedOptions = (FileRequestOptions) null;
      return cloudFileStream;
    }

    [DoesServiceRequest]
    public virtual void DownloadToStream(
      Stream target,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
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
      return this.BeginDownloadToStream(target, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadToStream(
      Stream target,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginDownloadRangeToStream(target, new long?(), new long?(), accessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginDownloadToStream(
      Stream target,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.DownloadToStreamAsync(target, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual void EndDownloadToStream(IAsyncResult asyncResult) => this.EndDownloadRangeToStream(asyncResult);

    [DoesServiceRequest]
    public virtual Task DownloadToStreamAsync(Stream target) => this.DownloadToStreamAsync(target, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task DownloadToStreamAsync(Stream target, CancellationToken cancellationToken) => this.DownloadToStreamAsync(target, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task DownloadToStreamAsync(
      Stream target,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.DownloadToStreamAsync(target, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task DownloadToStreamAsync(
      Stream target,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.DownloadRangeToStreamAsync(target, new long?(), new long?(), accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task DownloadToStreamAsync(
      Stream target,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.DownloadRangeToStreamAsync(target, new long?(), new long?(), accessCondition, options, operationContext, progressHandler, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void DownloadToFile(
      string path,
      FileMode mode,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
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
      return this.BeginDownloadToFile(path, mode, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadToFile(
      string path,
      FileMode mode,
      AccessCondition accessCondition,
      FileRequestOptions options,
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
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.DownloadToFileAsync(path, mode, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndDownloadToFile(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task DownloadToFileAsync(string path, FileMode mode) => this.DownloadToFileAsync(path, mode, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task DownloadToFileAsync(
      string path,
      FileMode mode,
      CancellationToken cancellationToken)
    {
      return this.DownloadToFileAsync(path, mode, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task DownloadToFileAsync(
      string path,
      FileMode mode,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.DownloadToFileAsync(path, mode, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task DownloadToFileAsync(
      string path,
      FileMode mode,
      AccessCondition accessCondition,
      FileRequestOptions options,
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
      FileRequestOptions options,
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
    public virtual int DownloadToByteArray(
      byte[] target,
      int index,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
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
      return this.BeginDownloadToByteArray(target, index, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadToByteArray(
      byte[] target,
      int index,
      AccessCondition accessCondition,
      FileRequestOptions options,
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
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<int>((Func<CancellationToken, Task<int>>) (token => this.DownloadToByteArrayAsync(target, index, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual int EndDownloadToByteArray(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<int>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<int> DownloadToByteArrayAsync(byte[] target, int index) => this.DownloadToByteArrayAsync(target, index, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<int> DownloadToByteArrayAsync(
      byte[] target,
      int index,
      CancellationToken cancellationToken)
    {
      return this.DownloadToByteArrayAsync(target, index, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<int> DownloadToByteArrayAsync(
      byte[] target,
      int index,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.DownloadToByteArrayAsync(target, index, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<int> DownloadToByteArrayAsync(
      byte[] target,
      int index,
      AccessCondition accessCondition,
      FileRequestOptions options,
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
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.DownloadRangeToByteArrayAsync(target, index, new long?(), new long?(), accessCondition, options, operationContext, progressHandler, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual string DownloadText(
      Encoding encoding = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
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
    public virtual ICancellableAsyncResult BeginDownloadText(AsyncCallback callback, object state) => this.BeginDownloadText((Encoding) null, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadText(
      Encoding encoding,
      AccessCondition accessCondition,
      FileRequestOptions options,
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
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<string>((Func<CancellationToken, Task<string>>) (token => this.DownloadTextAsync(encoding, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual string EndDownloadText(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<string>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<string> DownloadTextAsync() => this.DownloadTextAsync(CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<string> DownloadTextAsync(CancellationToken cancellationToken) => this.DownloadTextAsync((Encoding) null, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<string> DownloadTextAsync(
      Encoding encoding,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.DownloadTextAsync(encoding, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<string> DownloadTextAsync(
      Encoding encoding,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.DownloadTextAsync(encoding, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task<string> DownloadTextAsync(
      Encoding encoding,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      string str;
      using (SyncMemoryStream stream = new SyncMemoryStream())
      {
        await this.DownloadToStreamAsync((Stream) stream, accessCondition, options, operationContext, progressHandler, cancellationToken).ConfigureAwait(false);
        byte[] array = stream.ToArray();
        str = (encoding ?? Encoding.UTF8).GetString(array, 0, array.Length);
      }
      return str;
    }

    [DoesServiceRequest]
    public virtual void DownloadRangeToStream(
      Stream target,
      long? offset,
      long? length,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (target), (object) target);
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.GetFileImpl(target, offset, length, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadRangeToStream(
      Stream target,
      long? offset,
      long? length,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.DownloadRangeToStreamAsync(target, offset, length, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadRangeToStream(
      Stream target,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.DownloadRangeToStreamAsync(target, offset, length, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, token)), callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginDownloadRangeToStream(
      Stream target,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.DownloadRangeToStreamAsync(target, offset, length, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual void EndDownloadRangeToStream(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task DownloadRangeToStreamAsync(Stream target, long? offset, long? length) => this.DownloadRangeToStreamAsync(target, offset, length, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task DownloadRangeToStreamAsync(
      Stream target,
      long? offset,
      long? length,
      CancellationToken cancellationToken)
    {
      return this.DownloadRangeToStreamAsync(target, offset, length, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task DownloadRangeToStreamAsync(
      Stream target,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.DownloadRangeToStreamAsync(target, offset, length, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task DownloadRangeToStreamAsync(
      Stream target,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.DownloadRangeToStreamAsync(target, offset, length, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task DownloadRangeToStreamAsync(
      Stream target,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (target), (object) target);
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      NullType nullType = await Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.GetFileImpl(new AggregatingProgressIncrementer(progressHandler).CreateProgressIncrementingStream(target), offset, length, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken).ConfigureAwait(false);
    }

    [DoesServiceRequest]
    public virtual int DownloadRangeToByteArray(
      byte[] target,
      int index,
      long? fileOffset,
      long? length,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      using (SyncMemoryStream target1 = new SyncMemoryStream(target, index))
      {
        this.DownloadRangeToStream((Stream) target1, fileOffset, length, accessCondition, options, operationContext);
        return (int) target1.Position;
      }
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadRangeToByteArray(
      byte[] target,
      int index,
      long? fileOffset,
      long? length,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<int>((Func<CancellationToken, Task<int>>) (token => this.DownloadRangeToByteArrayAsync(target, index, fileOffset, length, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDownloadRangeToByteArray(
      byte[] target,
      int index,
      long? fileOffset,
      long? length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<int>((Func<CancellationToken, Task<int>>) (token => this.DownloadRangeToByteArrayAsync(target, index, fileOffset, length, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, token)), callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginDownloadRangeToByteArray(
      byte[] target,
      int index,
      long? fileOffset,
      long? length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<int>((Func<CancellationToken, Task<int>>) (token => this.DownloadRangeToByteArrayAsync(target, index, fileOffset, length, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual int EndDownloadRangeToByteArray(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<int>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<int> DownloadRangeToByteArrayAsync(
      byte[] target,
      int index,
      long? fileOffset,
      long? length)
    {
      return this.DownloadRangeToByteArrayAsync(target, index, fileOffset, length, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<int> DownloadRangeToByteArrayAsync(
      byte[] target,
      int index,
      long? fileOffset,
      long? length,
      CancellationToken cancellationToken)
    {
      return this.DownloadRangeToByteArrayAsync(target, index, fileOffset, length, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<int> DownloadRangeToByteArrayAsync(
      byte[] target,
      int index,
      long? fileOffset,
      long? length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.DownloadRangeToByteArrayAsync(target, index, fileOffset, length, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<int> DownloadRangeToByteArrayAsync(
      byte[] target,
      int index,
      long? fileOffset,
      long? length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.DownloadRangeToByteArrayAsync(target, index, fileOffset, length, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task<int> DownloadRangeToByteArrayAsync(
      byte[] target,
      int index,
      long? fileOffset,
      long? length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      int position;
      using (SyncMemoryStream stream = new SyncMemoryStream(target, index))
      {
        await this.DownloadRangeToStreamAsync((Stream) stream, fileOffset, length, accessCondition, options, operationContext, progressHandler, cancellationToken).ConfigureAwait(false);
        position = (int) stream.Position;
      }
      return position;
    }

    [DoesServiceRequest]
    public virtual void UploadFromStream(
      Stream source,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.UploadFromStreamHelper(source, new long?(), accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual void UploadFromStream(
      Stream source,
      long length,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.UploadFromStreamHelper(source, new long?(length), accessCondition, options, operationContext);
    }

    internal void UploadFromStreamHelper(
      Stream source,
      long? length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      CommonUtility.AssertNotNull(nameof (source), (object) source);
      if (!source.CanSeek)
        throw new InvalidOperationException();
      if (length.HasValue)
        CommonUtility.AssertInBounds<long>(nameof (length), length.Value, 1L, source.Length - source.Position);
      else
        length = new long?(source.Length - source.Position);
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      using (CloudFileStream toStream = this.OpenWrite(length, accessCondition, options1, operationContext))
      {
        using (ExecutionState<NullType> temporaryExecutionState = FileCommonUtility.CreateTemporaryExecutionState(options1))
        {
          source.WriteToSync<NullType>((Stream) toStream, length, new long?(), ChecksumRequested.None, true, temporaryExecutionState, (StreamDescriptor) null);
          toStream.Commit();
        }
      }
    }

    [DoesServiceRequest]
    private async Task UploadFromStreamAsyncHelper(
      Stream source,
      long? length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (source), (object) source);
      if (!source.CanSeek)
        throw new InvalidOperationException();
      if (length.HasValue)
        CommonUtility.AssertInBounds<long>(nameof (length), length.Value, 1L, source.Length - source.Position);
      else
        length = new long?(source.Length - source.Position);
      this.AssertNoSnapshot();
      FileRequestOptions modifiedOptions = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      using (CloudFileStream fileStream = await this.OpenWriteAsync(length, accessCondition, options, operationContext, cancellationToken).ConfigureAwait(false))
      {
        using (ExecutionState<NullType> tempExecutionState = FileCommonUtility.CreateTemporaryExecutionState(modifiedOptions))
        {
          ConfiguredTaskAwaitable configuredTaskAwaitable = source.WriteToAsync<NullType>(new AggregatingProgressIncrementer(progressHandler).CreateProgressIncrementingStream((Stream) fileStream), this.ServiceClient.BufferManager, length, new long?(), ChecksumRequested.None, tempExecutionState, (StreamDescriptor) null, cancellationToken).ConfigureAwait(false);
          await configuredTaskAwaitable;
          configuredTaskAwaitable = fileStream.CommitAsync().ConfigureAwait(false);
          await configuredTaskAwaitable;
        }
      }
      modifiedOptions = (FileRequestOptions) null;
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsyncHelper(source, new long?(), (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsyncHelper(source, new long?(), accessCondition, options, operationContext, (IProgress<StorageProgress>) null, token)), callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsyncHelper(source, new long?(), accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      long length,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsyncHelper(source, new long?(length), (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      long length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsyncHelper(source, new long?(length), accessCondition, options, operationContext, (IProgress<StorageProgress>) null, token)), callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginUploadFromStream(
      Stream source,
      long length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromStreamAsyncHelper(source, new long?(length), accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual void EndUploadFromStream(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(Stream source) => this.UploadFromStreamAsyncHelper(source, new long?(), (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(Stream source, CancellationToken cancellationToken) => this.UploadFromStreamAsyncHelper(source, new long?(), (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(), accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(), accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(), accessCondition, options, operationContext, progressHandler, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(Stream source, long length) => this.UploadFromStreamAsyncHelper(source, new long?(length), (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      long length,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(length), (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      long length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(length), accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      long length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(length), accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromStreamAsync(
      Stream source,
      long length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.UploadFromStreamAsyncHelper(source, new long?(length), accessCondition, options, operationContext, progressHandler, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void UploadFromFile(
      string path,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (path), (object) path);
      using (FileStream source = new FileStream(path, FileMode.Open, FileAccess.Read))
        this.UploadFromStream((Stream) source, accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromFile(
      string path,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromFileAsync(path, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromFile(
      string path,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromFileAsync(path, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, token)), callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginUploadFromFile(
      string path,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromFileAsync(path, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual void EndUploadFromFile(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task UploadFromFileAsync(string path) => this.UploadFromFileAsync(path, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task UploadFromFileAsync(string path, CancellationToken cancellationToken) => this.UploadFromFileAsync(path, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task UploadFromFileAsync(
      string path,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.UploadFromFileAsync(path, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromFileAsync(
      string path,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadFromFileAsync(path, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual async Task UploadFromFileAsync(
      string path,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (path), (object) path);
      using (Stream stream = (Stream) new FileStream(path, FileMode.Open, FileAccess.Read))
        await this.UploadFromStreamAsync(stream, accessCondition, options, operationContext, progressHandler, cancellationToken).ConfigureAwait(false);
    }

    [DoesServiceRequest]
    public virtual void UploadFromByteArray(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
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
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromByteArrayAsync(buffer, index, count, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadFromByteArray(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromByteArrayAsync(buffer, index, count, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, token)), callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginUploadFromByteArray(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadFromByteArrayAsync(buffer, index, count, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual void EndUploadFromByteArray(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task UploadFromByteArrayAsync(byte[] buffer, int index, int count) => this.UploadFromByteArrayAsync(buffer, index, count, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task UploadFromByteArrayAsync(
      byte[] buffer,
      int index,
      int count,
      CancellationToken cancellationToken)
    {
      return this.UploadFromByteArrayAsync(buffer, index, count, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromByteArrayAsync(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition,
      FileRequestOptions options,
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
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadFromByteArrayAsync(buffer, index, count, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadFromByteArrayAsync(
      byte[] buffer,
      int index,
      int count,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (buffer), (object) buffer);
      SyncMemoryStream source = new SyncMemoryStream(buffer, index, count);
      return this.UploadFromStreamAsync((Stream) source, source.Length, accessCondition, options, operationContext, progressHandler, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void UploadText(
      string content,
      Encoding encoding = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
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
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadTextAsync(content, (Encoding) null, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginUploadText(
      string content,
      Encoding encoding,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadTextAsync(content, encoding, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, token)), callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginUploadText(
      string content,
      Encoding encoding,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.UploadTextAsync(content, encoding, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual void EndUploadText(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task UploadTextAsync(string content) => this.UploadTextAsync(content, (Encoding) null, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task UploadTextAsync(string content, CancellationToken cancellationToken) => this.UploadTextAsync(content, (Encoding) null, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task UploadTextAsync(
      string content,
      Encoding encoding,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.UploadTextAsync(content, encoding, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task UploadTextAsync(
      string content,
      Encoding encoding,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.UploadTextAsync(content, encoding, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task UploadTextAsync(
      string content,
      Encoding encoding,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (content), (object) content);
      byte[] bytes = (encoding ?? Encoding.UTF8).GetBytes(content);
      return this.UploadFromByteArrayAsync(bytes, 0, bytes.Length, accessCondition, options, operationContext, progressHandler, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void Create(
      long size,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.AssertNoSnapshot();
      this.AssertValidFilePermissionOrKey();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.CreateImpl(size, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreate(
      long size,
      AsyncCallback callback,
      object state)
    {
      return this.BeginCreate(size, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCreate(
      long size,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.CreateAsync(size, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndCreate(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task CreateAsync(long size) => this.CreateAsync(size, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task CreateAsync(long size, CancellationToken cancellationToken) => this.CreateAsync(size, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task CreateAsync(
      long size,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.CreateAsync(size, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task CreateAsync(
      long size,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.Share.AssertNoSnapshot();
      this.AssertValidFilePermissionOrKey();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.CreateImpl(size, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual bool Exists(FileRequestOptions options = null, OperationContext operationContext = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<bool>(this.ExistsImpl(options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginExists(AsyncCallback callback, object state) => this.BeginExists((FileRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginExists(
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<bool>((Func<CancellationToken, Task<bool>>) (token => this.ExistsAsync(options, operationContext, token)), callback, state);
    }

    public virtual bool EndExists(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<bool>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync() => this.ExistsAsync((FileRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync(CancellationToken cancellationToken) => this.ExistsAsync((FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync(
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.ExistsAsync(options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<bool> ExistsAsync(
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<bool>(this.ExistsImpl(options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void FetchAttributes(
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.FetchAttributesImpl(accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginFetchAttributes(
      AsyncCallback callback,
      object state)
    {
      return this.BeginFetchAttributes((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginFetchAttributes(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.FetchAttributesAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndFetchAttributes(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync() => this.FetchAttributesAsync((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync(CancellationToken cancellationToken) => this.FetchAttributesAsync((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.FetchAttributesAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task FetchAttributesAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.FetchAttributesImpl(accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void Delete(
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.DeleteFileImpl(accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDelete(AsyncCallback callback, object state) => this.BeginDelete((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDelete(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.DeleteAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndDelete(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task DeleteAsync() => this.DeleteAsync((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task DeleteAsync(CancellationToken cancellationToken) => this.DeleteAsync((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task DeleteAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.DeleteAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task DeleteAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.DeleteFileImpl(accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual bool DeleteIfExists(
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.Share.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      try
      {
        this.Delete(accessCondition, options1, operationContext);
        return true;
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 404)
        {
          if (ex.RequestInformation.ExtendedErrorInformation == null || ex.RequestInformation.ExtendedErrorInformation.ErrorCode == StorageErrorCodeStrings.ResourceNotFound)
            return false;
          throw;
        }
        else
          throw;
      }
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDeleteIfExists(AsyncCallback callback, object state) => this.BeginDeleteIfExists((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginDeleteIfExists(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<bool>((Func<CancellationToken, Task<bool>>) (token => this.DeleteIfExistsAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual bool EndDeleteIfExists(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<bool>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<bool> DeleteIfExistsAsync() => this.DeleteIfExistsAsync((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<bool> DeleteIfExistsAsync(CancellationToken cancellationToken) => this.DeleteIfExistsAsync((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<bool> DeleteIfExistsAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.DeleteIfExistsAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual async Task<bool> DeleteIfExistsAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.Share.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      try
      {
        await this.DeleteAsync(accessCondition, options1, operationContext, cancellationToken).ConfigureAwait(false);
        return true;
      }
      catch (Exception ex)
      {
        if (operationContext.LastResult.HttpStatusCode == 404)
        {
          StorageExtendedErrorInformation errorInformation = operationContext.LastResult.ExtendedErrorInformation;
          if (errorInformation == null || errorInformation.ErrorCode == StorageErrorCodeStrings.ResourceNotFound)
            return false;
          throw;
        }
        else
          throw;
      }
    }

    [DoesServiceRequest]
    public virtual IEnumerable<FileRange> ListRanges(
      long? offset = null,
      long? length = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<IEnumerable<FileRange>>(this.ListRangesImpl(offset, length, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginListRanges(AsyncCallback callback, object state) => this.BeginListRanges(new long?(), new long?(), (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginListRanges(
      long? offset,
      long? length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<IEnumerable<FileRange>>((Func<CancellationToken, Task<IEnumerable<FileRange>>>) (token => this.ListRangesAsync(offset, length, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual IEnumerable<FileRange> EndListRanges(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<IEnumerable<FileRange>>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<IEnumerable<FileRange>> ListRangesAsync() => this.ListRangesAsync(new long?(), new long?(), (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<IEnumerable<FileRange>> ListRangesAsync(CancellationToken cancellationToken) => this.ListRangesAsync(new long?(), new long?(), (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<IEnumerable<FileRange>> ListRangesAsync(
      long? offset,
      long? length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.ListRangesAsync(offset, length, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<IEnumerable<FileRange>> ListRangesAsync(
      long? offset,
      long? length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<IEnumerable<FileRange>>(this.ListRangesImpl(offset, length, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual FileHandleResultSegment ListHandlesSegmented(
      FileContinuationToken token = null,
      int? maxResults = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<FileHandleResultSegment>(this.ListHandlesImpl(token, maxResults, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginListHandlesSegmented(
      FileContinuationToken token,
      int? maxResults,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<FileHandleResultSegment>((Func<CancellationToken, Task<FileHandleResultSegment>>) (cancellationToken => this.ListHandlesSegmentedAsync(token, maxResults, accessCondition, options, operationContext, new CancellationToken?(cancellationToken))), callback, state);
    }

    public virtual FileHandleResultSegment EndListHandlesSegmented(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<FileHandleResultSegment>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<FileHandleResultSegment> ListHandlesSegmentedAsync(
      FileContinuationToken token = null,
      int? maxResults = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null,
      CancellationToken? cancellationToken = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<FileHandleResultSegment>(this.ListHandlesImpl(token, maxResults, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken ?? CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual CloseFileHandleResultSegment CloseAllHandlesSegmented(
      FileContinuationToken token = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<CloseFileHandleResultSegment>(this.CloseHandleImpl(token, "*", accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCloseAllHandlesSegmented(
      FileContinuationToken token,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<CloseFileHandleResultSegment>((Func<CancellationToken, Task<CloseFileHandleResultSegment>>) (cancellationToken => this.CloseAllHandlesSegmentedAsync(token, accessCondition, options, operationContext, new CancellationToken?(cancellationToken))), callback, state);
    }

    public virtual CloseFileHandleResultSegment EndCloseAllHandlesSegmented(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<CloseFileHandleResultSegment>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<CloseFileHandleResultSegment> CloseAllHandlesSegmentedAsync(
      FileContinuationToken token = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null,
      CancellationToken? cancellationToken = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<CloseFileHandleResultSegment>(this.CloseHandleImpl(token, "*", accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken ?? CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual CloseFileHandleResultSegment CloseHandleSegmented(
      string handleId,
      FileContinuationToken token = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<CloseFileHandleResultSegment>(this.CloseHandleImpl(token, handleId, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginCloseHandleSegmented(
      string handleId,
      FileContinuationToken token,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<CloseFileHandleResultSegment>((Func<CancellationToken, Task<CloseFileHandleResultSegment>>) (cancellationToken => this.CloseHandleSegmentedAsync(handleId, token, accessCondition, options, operationContext, new CancellationToken?(cancellationToken))), callback, state);
    }

    public virtual CloseFileHandleResultSegment EndCloseHandleSegmented(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<CloseFileHandleResultSegment>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<CloseFileHandleResultSegment> CloseHandleSegmentedAsync(
      string handleId,
      FileContinuationToken token = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null,
      CancellationToken? cancellationToken = null)
    {
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<CloseFileHandleResultSegment>(this.CloseHandleImpl(token, handleId, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken ?? CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual void SetProperties(
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.AssertNoSnapshot();
      this.AssertValidFilePermissionOrKey();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.SetPropertiesImpl(accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetProperties(AsyncCallback callback, object state) => this.BeginSetProperties((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetProperties(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.SetPropertiesAsync(accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndSetProperties(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task SetPropertiesAsync() => this.SetPropertiesAsync((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task SetPropertiesAsync(CancellationToken cancellationToken) => this.SetPropertiesAsync((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task SetPropertiesAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.SetPropertiesAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task SetPropertiesAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.AssertNoSnapshot();
      this.AssertValidFilePermissionOrKey();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.SetPropertiesImpl(accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void Resize(
      long size,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.ResizeImpl(size, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginResize(
      long size,
      AsyncCallback callback,
      object state)
    {
      return this.BeginResize(size, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginResize(
      long size,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.ResizeAsync(size, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndResize(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task ResizeAsync(long size) => this.ResizeAsync(size, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task ResizeAsync(long size, CancellationToken cancellationToken) => this.ResizeAsync(size, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task ResizeAsync(
      long size,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.ResizeAsync(size, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task ResizeAsync(
      long size,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.ResizeImpl(size, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void SetMetadata(
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.SetMetadataImpl(accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetMetadata(AsyncCallback callback, object state) => this.BeginSetMetadata((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginSetMetadata(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      this.AssertNoSnapshot();
      FileRequestOptions modifiedOptions = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.SetMetadataAsync(accessCondition, modifiedOptions, operationContext)), callback, state);
    }

    public virtual void EndSetMetadata(IAsyncResult asyncResult)
    {
      CommonUtility.AssertNotNull(nameof (asyncResult), (object) asyncResult);
      ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();
    }

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync() => this.SetMetadataAsync((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync(CancellationToken cancellationToken) => this.SetMetadataAsync((AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.SetMetadataAsync(accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task SetMetadataAsync(
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.Share.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.SetMetadataImpl(accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void WriteRange(
      Stream rangeData,
      long startOffset,
      string contentMD5 = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.WriteRange(rangeData, startOffset, new Checksum(contentMD5), accessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    private void WriteRange(
      Stream rangeData,
      long startOffset,
      Checksum contentChecksum = null,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      CommonUtility.AssertNotNull(nameof (rangeData), (object) rangeData);
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      ChecksumRequested calculateChecksum;
      ref ChecksumRequested local = ref calculateChecksum;
      bool? nullable;
      int num1;
      if (contentChecksum == null || contentChecksum.MD5 == null)
      {
        nullable = options1.ChecksumOptions.UseTransactionalMD5;
        num1 = nullable.Value ? 1 : 0;
      }
      else
        num1 = 0;
      int num2;
      if (contentChecksum == null || contentChecksum.CRC64 == null)
      {
        nullable = options1.ChecksumOptions.UseTransactionalCRC64;
        num2 = nullable.Value ? 1 : 0;
      }
      else
        num2 = 0;
      local = new ChecksumRequested(num1 != 0, num2 != 0);
      operationContext = operationContext ?? new OperationContext();
      Stream rangeData1 = rangeData;
      bool flag = false;
      try
      {
        if (!rangeData.CanSeek || calculateChecksum.HasAny)
        {
          ExecutionState<NullType> temporaryExecutionState = FileCommonUtility.CreateTemporaryExecutionState(options1);
          Stream toStream;
          if (rangeData.CanSeek)
          {
            toStream = Stream.Null;
          }
          else
          {
            rangeData1 = (Stream) new MultiBufferMemoryStream(this.ServiceClient.BufferManager);
            flag = true;
            toStream = rangeData1;
          }
          long position = rangeData1.Position;
          StreamDescriptor streamCopyState = new StreamDescriptor();
          rangeData.WriteToSync<NullType>(toStream, new long?(), new long?(104857600L), calculateChecksum, true, temporaryExecutionState, streamCopyState);
          rangeData1.Position = position;
          contentChecksum = new Checksum(calculateChecksum.MD5 ? streamCopyState.Md5 : (string) null, calculateChecksum.CRC64 ? streamCopyState.Crc64 : (string) null);
        }
        Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.PutRangeImpl(rangeData1, startOffset, contentChecksum, accessCondition, options1), options1.RetryPolicy, operationContext);
      }
      finally
      {
        if (flag)
          rangeData1.Dispose();
      }
    }

    [DoesServiceRequest]
    public virtual void WriteRange(
      Uri sourceUri,
      long sourceOffset,
      long count,
      long destOffset,
      Checksum sourceContentChecksum = null,
      AccessCondition sourceAccessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.AssertNoSnapshot();
      CommonUtility.AssertNotNull(nameof (sourceUri), (object) sourceUri);
      CommonUtility.AssertInBounds<long>(nameof (sourceOffset), sourceOffset, 0L);
      CommonUtility.AssertInBounds<long>(nameof (count), count, 0L, 4194304L);
      CommonUtility.AssertInBounds<long>(nameof (destOffset), destOffset, 0L);
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.PutRangeFromUriImpl(sourceUri, sourceOffset, count, destOffset, sourceContentChecksum, sourceAccessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginWriteRange(
      Stream rangeData,
      long startOffset,
      string contentMD5,
      AsyncCallback callback,
      object state)
    {
      return this.BeginWriteRange(rangeData, startOffset, contentMD5, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginWriteRange(
      Stream rangeData,
      long startOffset,
      string contentMD5,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginWriteRange(rangeData, startOffset, contentMD5, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginWriteRange(
      Stream rangeData,
      long startOffset,
      string contentMD5,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.WriteRangeAsync(rangeData, startOffset, new Checksum(contentMD5), accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    [DoesServiceRequest]
    private ICancellableAsyncResult BeginWriteRange(
      Stream rangeData,
      long startOffset,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.WriteRangeAsync(rangeData, startOffset, contentChecksum, accessCondition, options, operationContext, progressHandler, token)), callback, state);
    }

    public virtual void EndWriteRange(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task WriteRangeAsync(Stream rangeData, long startOffset, string contentMD5) => this.WriteRangeAsync(rangeData, startOffset, contentMD5, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task WriteRangeAsync(
      Stream rangeData,
      long startOffset,
      string contentMD5,
      CancellationToken cancellationToken)
    {
      return this.WriteRangeAsync(rangeData, startOffset, contentMD5, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task WriteRangeAsync(
      Stream rangeData,
      long startOffset,
      string contentMD5,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.WriteRangeAsync(rangeData, startOffset, contentMD5, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task WriteRangeAsync(
      Stream rangeData,
      long startOffset,
      string contentMD5,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.WriteRangeAsync(rangeData, startOffset, contentMD5, accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task WriteRangeAsync(
      Stream rangeData,
      long startOffset,
      string contentMD5,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      return this.WriteRangeAsync(rangeData, startOffset, new Checksum(contentMD5), accessCondition, options, operationContext, (IProgress<StorageProgress>) null, cancellationToken);
    }

    [DoesServiceRequest]
    internal async Task WriteRangeAsync(
      Stream rangeData,
      long startOffset,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      IProgress<StorageProgress> progressHandler,
      CancellationToken cancellationToken)
    {
      CommonUtility.AssertNotNull(nameof (rangeData), (object) rangeData);
      contentChecksum = contentChecksum ?? Checksum.None;
      this.AssertNoSnapshot();
      FileRequestOptions modifiedOptions = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
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
      DateTime streamCopyStartTime = DateTime.Now;
      Stream seekableStream = rangeData;
      bool seekableStreamCreated = false;
      try
      {
        if (!rangeData.CanSeek || requiresContentChecksum.HasAny)
        {
          ExecutionState<NullType> temporaryExecutionState = FileCommonUtility.CreateTemporaryExecutionState(modifiedOptions);
          Stream toStream;
          if (rangeData.CanSeek)
          {
            toStream = Stream.Null;
          }
          else
          {
            seekableStream = (Stream) new MultiBufferMemoryStream(this.ServiceClient.BufferManager);
            seekableStreamCreated = true;
            toStream = seekableStream;
          }
          StreamDescriptor streamCopyState = new StreamDescriptor();
          long startPosition = seekableStream.Position;
          await rangeData.WriteToAsync<NullType>(toStream, this.ServiceClient.BufferManager, new long?(), new long?(104857600L), requiresContentChecksum, temporaryExecutionState, streamCopyState, cancellationToken).ConfigureAwait(false);
          seekableStream.Position = startPosition;
          contentChecksum = new Checksum(requiresContentChecksum.MD5 ? streamCopyState.Md5 : (string) null, requiresContentChecksum.CRC64 ? streamCopyState.Crc64 : (string) null);
          if (modifiedOptions.MaximumExecutionTime.HasValue)
          {
            FileRequestOptions fileRequestOptions = modifiedOptions;
            TimeSpan? maximumExecutionTime = fileRequestOptions.MaximumExecutionTime;
            TimeSpan timeSpan = DateTime.Now.Subtract(streamCopyStartTime);
            fileRequestOptions.MaximumExecutionTime = maximumExecutionTime.HasValue ? new TimeSpan?(maximumExecutionTime.GetValueOrDefault() - timeSpan) : new TimeSpan?();
          }
          streamCopyState = (StreamDescriptor) null;
        }
        NullType nullType = await Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.PutRangeImpl(new AggregatingProgressIncrementer(progressHandler).CreateProgressIncrementingStream(seekableStream), startOffset, contentChecksum, accessCondition, modifiedOptions), modifiedOptions.RetryPolicy, operationContext, cancellationToken).ConfigureAwait(false);
      }
      finally
      {
        if (seekableStreamCreated)
          seekableStream.Dispose();
      }
      modifiedOptions = (FileRequestOptions) null;
      seekableStream = (Stream) null;
    }

    [DoesServiceRequest]
    public virtual async Task WriteRangeAsync(
      Uri sourceUri,
      long sourceOffset,
      long count,
      long destOffset,
      Checksum sourceContentChecksum = null,
      AccessCondition sourceAccessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null,
      CancellationToken? cancellationToken = null)
    {
      this.AssertNoSnapshot();
      CommonUtility.AssertNotNull(nameof (sourceUri), (object) sourceUri);
      CommonUtility.AssertInBounds<long>(nameof (sourceOffset), sourceOffset, 0L);
      CommonUtility.AssertInBounds<long>(nameof (count), count, 0L, 4194304L);
      CommonUtility.AssertInBounds<long>(nameof (destOffset), destOffset, 0L);
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      operationContext = operationContext ?? new OperationContext();
      cancellationToken = new CancellationToken?(cancellationToken ?? CancellationToken.None);
      NullType nullType = await Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.PutRangeFromUriImpl(sourceUri, sourceOffset, count, destOffset, sourceContentChecksum, sourceAccessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken.Value).ConfigureAwait(false);
    }

    [DoesServiceRequest]
    public virtual void ClearRange(
      long startOffset,
      long length,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.ClearRangeImpl(startOffset, length, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginClearRange(
      long startOffset,
      long length,
      AsyncCallback callback,
      object state)
    {
      return this.BeginClearRange(startOffset, length, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginClearRange(
      long startOffset,
      long length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.ClearRangeAsync(startOffset, length, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndClearRange(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task ClearRangeAsync(long startOffset, long length) => this.ClearRangeAsync(startOffset, length, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task ClearRangeAsync(
      long startOffset,
      long length,
      CancellationToken cancellationToken)
    {
      return this.ClearRangeAsync(startOffset, length, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task ClearRangeAsync(
      long startOffset,
      long length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.ClearRangeAsync(startOffset, length, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task ClearRangeAsync(
      long startOffset,
      long length,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.Share.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.ClearRangeImpl(startOffset, length, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual string StartCopy(
      Uri source,
      AccessCondition sourceAccessCondition = null,
      AccessCondition destAccessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return this.StartCopy(source, sourceAccessCondition, destAccessCondition, new FileCopyOptions(), options, operationContext);
    }

    [DoesServiceRequest]
    public virtual string StartCopy(
      Uri source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      FileCopyOptions copyOptions,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      CommonUtility.AssertNotNull(nameof (source), (object) source);
      this.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<string>(this.StartCopyImpl(source, sourceAccessCondition, destAccessCondition, copyOptions, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual string StartCopy(
      CloudFile source,
      AccessCondition sourceAccessCondition = null,
      AccessCondition destAccessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      return this.StartCopy(CloudFile.SourceFileToUri(source), sourceAccessCondition, destAccessCondition, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual string StartCopy(
      CloudFile source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      FileCopyOptions copyOptions,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.StartCopy(CloudFile.SourceFileToUri(source), sourceAccessCondition, destAccessCondition, copyOptions, options, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginStartCopy(
      Uri source,
      AsyncCallback callback,
      object state)
    {
      return this.BeginStartCopy(source, (AccessCondition) null, (AccessCondition) null, new FileCopyOptions(), (FileRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginStartCopy(
      CloudFile source,
      AsyncCallback callback,
      object state)
    {
      return this.BeginStartCopy(CloudFile.SourceFileToUri(source), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginStartCopy(
      Uri source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginStartCopy(source, sourceAccessCondition, destAccessCondition, new FileCopyOptions(), options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginStartCopy(
      Uri source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      FileCopyOptions copyOptions,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create<string>((Func<CancellationToken, Task<string>>) (token => this.StartCopyAsync(source, sourceAccessCondition, destAccessCondition, copyOptions, options, operationContext, token)), callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginStartCopy(
      CloudFile source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginStartCopy(CloudFile.SourceFileToUri(source), sourceAccessCondition, destAccessCondition, options, operationContext, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginStartCopy(
      CloudFile source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      FileCopyOptions copyOptions,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return this.BeginStartCopy(CloudFile.SourceFileToUri(source), sourceAccessCondition, destAccessCondition, copyOptions, options, operationContext, callback, state);
    }

    public virtual string EndStartCopy(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper<string>) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(Uri source) => this.StartCopyAsync(source, (AccessCondition) null, (AccessCondition) null, new FileCopyOptions(), (FileRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(Uri source, CancellationToken cancellationToken) => this.StartCopyAsync(source, (AccessCondition) null, (AccessCondition) null, new FileCopyOptions(), (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(CloudFile source) => this.StartCopyAsync(CloudFile.SourceFileToUri(source), (AccessCondition) null, (AccessCondition) null, new FileCopyOptions(), (FileRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      CloudFile source,
      CancellationToken cancellationToken)
    {
      return this.StartCopyAsync(CloudFile.SourceFileToUri(source), (AccessCondition) null, (AccessCondition) null, new FileCopyOptions(), (FileRequestOptions) null, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      Uri source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.StartCopyAsync(source, sourceAccessCondition, destAccessCondition, new FileCopyOptions(), options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      Uri source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      FileCopyOptions copyOptions,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.StartCopyAsync(source, sourceAccessCondition, destAccessCondition, copyOptions, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      Uri source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.StartCopyAsync(source, sourceAccessCondition, destAccessCondition, new FileCopyOptions(), options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      Uri source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      FileCopyOptions copyOptions,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.Share.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<string>(this.StartCopyImpl(source, sourceAccessCondition, destAccessCondition, copyOptions, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      CloudFile source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.StartCopyAsync(CloudFile.SourceFileToUri(source), sourceAccessCondition, destAccessCondition, new FileCopyOptions(), options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      CloudFile source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      FileCopyOptions copyOptions,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.StartCopyAsync(CloudFile.SourceFileToUri(source), sourceAccessCondition, destAccessCondition, copyOptions, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      CloudFile source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.StartCopyAsync(CloudFile.SourceFileToUri(source), sourceAccessCondition, destAccessCondition, new FileCopyOptions(), options, (OperationContext) null, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual Task<string> StartCopyAsync(
      CloudFile source,
      AccessCondition sourceAccessCondition,
      AccessCondition destAccessCondition,
      FileCopyOptions copyOptions,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      return this.StartCopyAsync(CloudFile.SourceFileToUri(source), sourceAccessCondition, destAccessCondition, copyOptions, options, operationContext, cancellationToken);
    }

    [DoesServiceRequest]
    public virtual void AbortCopy(
      string copyId,
      AccessCondition accessCondition = null,
      FileRequestOptions options = null,
      OperationContext operationContext = null)
    {
      this.Share.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteSync<NullType>(this.AbortCopyImpl(copyId, accessCondition, options1), options1.RetryPolicy, operationContext);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAbortCopy(
      string copyId,
      AsyncCallback callback,
      object state)
    {
      return this.BeginAbortCopy(copyId, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, callback, state);
    }

    [DoesServiceRequest]
    public virtual ICancellableAsyncResult BeginAbortCopy(
      string copyId,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      AsyncCallback callback,
      object state)
    {
      return CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.AbortCopyAsync(copyId, accessCondition, options, operationContext, token)), callback, state);
    }

    public virtual void EndAbortCopy(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    [DoesServiceRequest]
    public virtual Task AbortCopyAsync(string copyId) => this.AbortCopyAsync(copyId, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, CancellationToken.None);

    [DoesServiceRequest]
    public virtual Task AbortCopyAsync(string copyId, CancellationToken cancellationToken) => this.AbortCopyAsync(copyId, (AccessCondition) null, (FileRequestOptions) null, (OperationContext) null, cancellationToken);

    [DoesServiceRequest]
    public virtual Task AbortCopyAsync(
      string copyId,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext)
    {
      return this.AbortCopyAsync(copyId, accessCondition, options, operationContext, CancellationToken.None);
    }

    [DoesServiceRequest]
    public virtual Task AbortCopyAsync(
      string copyId,
      AccessCondition accessCondition,
      FileRequestOptions options,
      OperationContext operationContext,
      CancellationToken cancellationToken)
    {
      this.Share.AssertNoSnapshot();
      FileRequestOptions options1 = FileRequestOptions.ApplyDefaults(options, this.ServiceClient);
      return (Task) Microsoft.Azure.Storage.Core.Executor.Executor.ExecuteAsync<NullType>(this.AbortCopyImpl(copyId, accessCondition, options1), options1.RetryPolicy, operationContext, cancellationToken);
    }

    private RESTCommand<NullType> GetFileImpl(
      Stream destStream,
      long? offset,
      long? length,
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      string lockedETag = (string) null;
      AccessCondition lockedAccessCondition = (AccessCondition) null;
      bool arePropertiesPopulated = false;
      string storedMD5 = (string) null;
      string storedCRC64 = (string) null;
      long startingOffset = offset.HasValue ? offset.Value : 0L;
      long? startingLength = length;
      long? validateLength = new long?();
      RESTCommand<NullType> getCmd = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      bool? nullable1 = options.ChecksumOptions.UseTransactionalMD5;
      int num1 = nullable1.Value ? 1 : 0;
      nullable1 = options.ChecksumOptions.UseTransactionalCRC64;
      int num2 = nullable1.Value ? 1 : 0;
      ChecksumRequested checksumRequested = new ChecksumRequested(num1 != 0, num2 != 0);
      options.ApplyToStorageCommand<NullType>(getCmd);
      getCmd.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      getCmd.RetrieveResponseStream = true;
      getCmd.DestinationStream = destStream;
      RESTCommand<NullType> restCommand = getCmd;
      nullable1 = options.ChecksumOptions.DisableContentMD5Validation;
      int num3 = !nullable1.Value ? 1 : 0;
      nullable1 = options.ChecksumOptions.DisableContentCRC64Validation;
      int num4 = !nullable1.Value ? 1 : 0;
      ChecksumRequested checksumRequested1 = new ChecksumRequested(num3 != 0, num4 != 0);
      restCommand.ChecksumRequestedForResponseStream = checksumRequested1;
      getCmd.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => FileHttpRequestMessageFactory.Get(uri, serverTimeout, offset, length, checksumRequested, this.Share.SnapshotTime, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
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
        getCmd.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((command, uri, builder, cnt, serverTimeout, context) => FileHttpRequestMessageFactory.Get(uri, serverTimeout, offset, length, checksumRequestedAndNotPopulated, this.Share.SnapshotTime, accessCondition, cnt, context, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      });
      getCmd.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(offset.HasValue ? HttpStatusCode.PartialContent : HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        if (!arePropertiesPopulated)
        {
          this.UpdateAfterFetchAttributes(resp);
          storedMD5 = HttpResponseParsers.GetContentMD5(resp);
          storedCRC64 = HttpResponseParsers.GetContentCRC64(resp);
          bool? nullable2;
          if (!options.ChecksumOptions.DisableContentMD5Validation.Value)
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
          lockedETag = this.attributes.Properties.ETag;
          validateLength = resp.Content.Headers.ContentLength;
          arePropertiesPopulated = true;
        }
        else if (!resp.Headers.ETag.ToString().Equals(lockedETag, StringComparison.Ordinal))
          throw new StorageException(new RequestResult()
          {
            HttpStatusMessage = (string) null,
            HttpStatusCode = 412,
            ExtendedErrorInformation = (StorageExtendedErrorInformation) null
          }, "The condition specified using HTTP conditional header(s) is not met.", (Exception) null);
        return NullType.Value;
      });
      getCmd.PostProcessResponseAsync = (Func<RESTCommand<NullType>, HttpResponseMessage, OperationContext, CancellationToken, Task<NullType>>) ((cmd, resp, ctx, ct) =>
      {
        HttpResponseParsers.ValidateResponseStreamChecksumAndLength<NullType>(validateLength, storedMD5, storedCRC64, (StorageCommandBase<NullType>) cmd);
        return NullType.ValueTask;
      });
      return getCmd;
    }

    private RESTCommand<NullType> CreateImpl(
      long sizeInBytes,
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = FileHttpRequestMessageFactory.Create(uri, serverTimeout, this.Properties, this.filePermission, sizeInBytes, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        FileHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Created, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        FileHttpResponseParsers.UpdateSmbProperties(resp, this.Properties);
        this.UpdateETagLMTAndLength(resp, false);
        this.Properties.Length = sizeInBytes;
        this.filePermission = (string) null;
        cmd.CurrentResult.IsRequestServerEncrypted = HttpResponseParsers.ParseServerRequestEncrypted(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<NullType> FetchAttributesImpl(
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => FileHttpRequestMessageFactory.GetProperties(uri, serverTimeout, this.Share.SnapshotTime, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.UpdateAfterFetchAttributes(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<bool> ExistsImpl(FileRequestOptions options)
    {
      RESTCommand<bool> cmd1 = new RESTCommand<bool>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<bool>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<bool>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => FileHttpRequestMessageFactory.GetProperties(uri, serverTimeout, this.Share.SnapshotTime, (AccessCondition) null, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<bool>, HttpResponseMessage, Exception, OperationContext, bool>) ((cmd, resp, ex, ctx) =>
      {
        if (resp.StatusCode == HttpStatusCode.NotFound)
          return false;
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<bool>(HttpStatusCode.OK, resp, true, (StorageCommandBase<bool>) cmd, ex);
        this.UpdateAfterFetchAttributes(resp);
        return true;
      });
      return cmd1;
    }

    private RESTCommand<NullType> DeleteFileImpl(
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => FileHttpRequestMessageFactory.Delete(uri, serverTimeout, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Accepted, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex));
      return cmd1;
    }

    private RESTCommand<IEnumerable<FileRange>> ListRangesImpl(
      long? offset,
      long? length,
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      RESTCommand<IEnumerable<FileRange>> cmd1 = new RESTCommand<IEnumerable<FileRange>>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<IEnumerable<FileRange>>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<IEnumerable<FileRange>>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = FileHttpRequestMessageFactory.ListRanges(uri, serverTimeout, offset, length, this.Share.SnapshotTime, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        FileHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<IEnumerable<FileRange>>, HttpResponseMessage, Exception, OperationContext, IEnumerable<FileRange>>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<IEnumerable<FileRange>>(HttpStatusCode.OK, resp, (IEnumerable<FileRange>) null, (StorageCommandBase<IEnumerable<FileRange>>) cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<IEnumerable<FileRange>>, HttpResponseMessage, OperationContext, CancellationToken, Task<IEnumerable<FileRange>>>) ((cmd, resp, ctx, ct) =>
      {
        this.UpdateETagLMTAndLength(resp, true);
        return ListRangesResponse.ParseAsync(cmd.ResponseStream, ct);
      });
      return cmd1;
    }

    private RESTCommand<FileHandleResultSegment> ListHandlesImpl(
      FileContinuationToken token,
      int? maxResults,
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      RESTCommand<FileHandleResultSegment> cmd1 = new RESTCommand<FileHandleResultSegment>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<FileHandleResultSegment>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<FileHandleResultSegment>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = FileHttpRequestMessageFactory.ListHandles(uri, serverTimeout, this.Share.SnapshotTime, maxResults, new bool?(false), token, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        FileHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<FileHandleResultSegment>, HttpResponseMessage, Exception, OperationContext, FileHandleResultSegment>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<FileHandleResultSegment>(HttpStatusCode.OK, resp, (FileHandleResultSegment) null, (StorageCommandBase<FileHandleResultSegment>) cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<FileHandleResultSegment>, HttpResponseMessage, OperationContext, CancellationToken, Task<FileHandleResultSegment>>) ((cmd, resp, ctx, ct) =>
      {
        this.UpdateETagLMTAndLength(resp, true);
        ListHandlesResponse listHandlesResponse = new ListHandlesResponse(cmd.ResponseStream);
        return Task.FromResult<FileHandleResultSegment>(new FileHandleResultSegment()
        {
          Results = listHandlesResponse.Handles,
          ContinuationToken = new FileContinuationToken()
          {
            NextMarker = listHandlesResponse.NextMarker
          }
        });
      });
      return cmd1;
    }

    private RESTCommand<CloseFileHandleResultSegment> CloseHandleImpl(
      FileContinuationToken token,
      string handleId,
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      RESTCommand<CloseFileHandleResultSegment> cmd1 = new RESTCommand<CloseFileHandleResultSegment>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<CloseFileHandleResultSegment>(cmd1);
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.RetrieveResponseStream = true;
      cmd1.BuildRequest = (Func<RESTCommand<CloseFileHandleResultSegment>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = FileHttpRequestMessageFactory.CloseHandle(uri, serverTimeout, this.Share.SnapshotTime, handleId, new bool?(false), token, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        FileHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<CloseFileHandleResultSegment>, HttpResponseMessage, Exception, OperationContext, CloseFileHandleResultSegment>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<CloseFileHandleResultSegment>(HttpStatusCode.OK, resp, (CloseFileHandleResultSegment) null, (StorageCommandBase<CloseFileHandleResultSegment>) cmd, ex);
        int result;
        if (!int.TryParse(resp.Headers.GetHeaderSingleValueOrDefault("x-ms-number-of-handles-closed"), out result))
          result = -1;
        FileContinuationToken continuationToken = (FileContinuationToken) null;
        string singleValueOrDefault;
        if ((singleValueOrDefault = resp.Headers.GetHeaderSingleValueOrDefault("x-ms-marker")) != "")
          continuationToken = new FileContinuationToken()
          {
            NextMarker = singleValueOrDefault
          };
        return new CloseFileHandleResultSegment()
        {
          NumHandlesClosed = result,
          ContinuationToken = continuationToken
        };
      });
      return cmd1;
    }

    private RESTCommand<NullType> SetPropertiesImpl(
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = FileHttpRequestMessageFactory.SetProperties(uri, serverTimeout, this.Properties, this.FilePermission, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        FileHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        FileHttpResponseParsers.UpdateSmbProperties(resp, this.Properties);
        this.UpdateETagLMTAndLength(resp, false);
        this.filePermission = (string) null;
        cmd.CurrentResult.IsRequestServerEncrypted = HttpResponseParsers.ParseServerRequestEncrypted(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<NullType> ResizeImpl(
      long sizeInBytes,
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      RESTCommand<NullType> putCmd = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(putCmd);
      putCmd.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => FileHttpRequestMessageFactory.Resize(uri, serverTimeout, sizeInBytes, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      putCmd.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.UpdateETagLMTAndLength(resp, false);
        this.Properties.Length = sizeInBytes;
        putCmd.CurrentResult.IsRequestServerEncrypted = HttpResponseParsers.ParseServerRequestEncrypted(resp);
        return NullType.Value;
      });
      return putCmd;
    }

    private RESTCommand<NullType> SetMetadataImpl(
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request = FileHttpRequestMessageFactory.SetMetadata(uri, serverTimeout, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        FileHttpRequestMessageFactory.AddMetadata(request, this.Metadata);
        return request;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.OK, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.UpdateETagLMTAndLength(resp, false);
        cmd.CurrentResult.IsRequestServerEncrypted = HttpResponseParsers.ParseServerRequestEncrypted(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<NullType> PutRangeImpl(
      Stream rangeData,
      long startOffset,
      Checksum contentChecksum,
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      long offset = rangeData.Position;
      long length = rangeData.Length - offset;
      FileRange fileRange = new FileRange(startOffset, startOffset + length - 1L);
      FileRangeWrite fileRangeWrite = FileRangeWrite.Update;
      if (1L + fileRange.EndOffset - fileRange.StartOffset == 0L)
        CommonUtility.ArgumentOutOfRange(nameof (rangeData), (object) rangeData);
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildContent = (Func<RESTCommand<NullType>, OperationContext, HttpContent>) ((cmd, ctx) => HttpContentFactory.BuildContentFromStream<NullType>(rangeData, offset, new long?(length), contentChecksum, cmd, ctx));
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => FileHttpRequestMessageFactory.PutRange(uri, serverTimeout, fileRange, fileRangeWrite, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Created, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.UpdateETagLMTAndLength(resp, false);
        cmd.CurrentResult.IsRequestServerEncrypted = HttpResponseParsers.ParseServerRequestEncrypted(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<NullType> PutRangeFromUriImpl(
      Uri sourceUri,
      long sourceOffset,
      long count,
      long destOffset,
      Checksum sourceContentChecksum,
      AccessCondition sourceAccessCondition,
      FileRequestOptions options)
    {
      FileRange sourceFileRange = new FileRange(sourceOffset, sourceOffset + count - 1L);
      FileRange destFileRange = new FileRange(destOffset, destOffset + count - 1L);
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => FileHttpRequestMessageFactory.PutRangeFromUrl(uri, sourceUri, sourceFileRange, destFileRange, serverTimeout, sourceContentChecksum, sourceAccessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Created, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.UpdateETagLMTAndLength(resp, false);
        cmd.CurrentResult.IsRequestServerEncrypted = HttpResponseParsers.ParseServerRequestEncrypted(resp);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<NullType> ClearRangeImpl(
      long startOffset,
      long length,
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      CommonUtility.AssertNotNull(nameof (options), (object) options);
      if (startOffset < 0L)
        CommonUtility.ArgumentOutOfRange(nameof (startOffset), (object) startOffset);
      if (length <= 0L)
        CommonUtility.ArgumentOutOfRange(nameof (length), (object) length);
      FileRange range = new FileRange(startOffset, startOffset + length - 1L);
      FileRangeWrite fileWrite = FileRangeWrite.Clear;
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => FileHttpRequestMessageFactory.PutRange(uri, serverTimeout, range, fileWrite, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.Created, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex);
        this.UpdateETagLMTAndLength(resp, false);
        return NullType.Value;
      });
      return cmd1;
    }

    private RESTCommand<string> StartCopyImpl(
      Uri source,
      AccessCondition sourceAccessCondition = null,
      AccessCondition destAccessCondition = null,
      FileCopyOptions copyOptions = default (FileCopyOptions),
      FileRequestOptions options = null)
    {
      if (sourceAccessCondition != null && !string.IsNullOrEmpty(sourceAccessCondition.LeaseId))
        throw new ArgumentException("A lease condition cannot be specified on the source of a copy.", nameof (sourceAccessCondition));
      RESTCommand<string> cmd1 = new RESTCommand<string>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<string>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<string>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) =>
      {
        StorageRequestMessage request1 = FileHttpRequestMessageFactory.CopyFrom(uri, serverTimeout, source, sourceAccessCondition, destAccessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials);
        FileHttpRequestMessageFactory.AddMetadata(request1, this.attributes.Metadata);
        if (copyOptions.PreservePermissions)
          request1.AddOptionalHeader("x-ms-file-permission-copy-mode", nameof (source));
        else if (this.filePermission != null)
        {
          request1.AddOptionalHeader("x-ms-file-permission-copy-mode", "override");
          request1.AddOptionalHeader("x-ms-file-permission", this.filePermission);
        }
        else if (this.Properties?.FilePermissionKey != null)
        {
          request1.AddOptionalHeader("x-ms-file-permission-copy-mode", "override");
          request1.AddOptionalHeader("x-ms-file-permission-key", this.Properties.FilePermissionKey);
        }
        bool? setArchive = copyOptions.SetArchive;
        if (setArchive.HasValue)
        {
          StorageRequestMessage request2 = request1;
          setArchive = copyOptions.SetArchive;
          string str = setArchive.Value ? "true" : "false";
          request2.AddOptionalHeader("x-ms-file-copy-set-archive", str);
        }
        if (copyOptions.PreserveNtfsAttributes)
        {
          request1.AddOptionalHeader("x-ms-file-attributes", nameof (source));
        }
        else
        {
          FileProperties properties = this.Properties;
          if ((properties != null ? (properties.ntfsAttributesToSet.HasValue ? 1 : 0) : 0) != 0)
            request1.AddOptionalHeader("x-ms-file-attributes", CloudFileNtfsAttributesHelper.ToString(this.Properties.ntfsAttributesToSet.Value));
        }
        if (copyOptions.PreserveCreationTime)
        {
          request1.AddOptionalHeader("x-ms-file-creation-time", nameof (source));
        }
        else
        {
          FileProperties properties = this.Properties;
          if ((properties != null ? (properties.creationTimeToSet.HasValue ? 1 : 0) : 0) != 0)
            request1.AddOptionalHeader("x-ms-file-creation-time", Request.ConvertDateTimeToSnapshotString(this.Properties.creationTimeToSet.Value));
        }
        if (copyOptions.PreserveLastWriteTime)
        {
          request1.AddOptionalHeader("x-ms-file-last-write-time", nameof (source));
        }
        else
        {
          FileProperties properties = this.Properties;
          if ((properties != null ? (properties.lastWriteTimeToSet.HasValue ? 1 : 0) : 0) != 0)
            request1.AddOptionalHeader("x-ms-file-last-write-time", Request.ConvertDateTimeToSnapshotString(this.Properties.lastWriteTimeToSet.Value));
        }
        if (copyOptions.IgnoreReadOnly)
          request1.AddOptionalHeader("x-ms-file-copy-ignore-readonly", "true");
        return request1;
      });
      cmd1.PreProcessResponse = (Func<RESTCommand<string>, HttpResponseMessage, Exception, OperationContext, string>) ((cmd, resp, ex, ctx) =>
      {
        HttpResponseParsers.ProcessExpectedStatusCodeNoException<string>(HttpStatusCode.Accepted, resp, (string) null, (StorageCommandBase<string>) cmd, ex);
        CopyState copyAttributes = FileHttpResponseParsers.GetCopyAttributes(resp);
        this.attributes.Properties = FileHttpResponseParsers.GetProperties(resp);
        this.attributes.Metadata = FileHttpResponseParsers.GetMetadata(resp);
        this.attributes.CopyState = copyAttributes;
        return copyAttributes.CopyId;
      });
      return cmd1;
    }

    private RESTCommand<NullType> AbortCopyImpl(
      string copyId,
      AccessCondition accessCondition,
      FileRequestOptions options)
    {
      CommonUtility.AssertNotNull(nameof (copyId), (object) copyId);
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(this.ServiceClient.Credentials, this.attributes.StorageUri, this.ServiceClient.HttpClient);
      options.ApplyToStorageCommand<NullType>(cmd1);
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, builder, cnt, serverTimeout, ctx) => FileHttpRequestMessageFactory.AbortCopy(uri, serverTimeout, copyId, accessCondition, cnt, ctx, this.ServiceClient.GetCanonicalizer(), this.ServiceClient.Credentials));
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.NoContent, resp, NullType.Value, (StorageCommandBase<NullType>) cmd, ex));
      return cmd1;
    }

    internal static Uri SourceFileToUri(CloudFile source)
    {
      CommonUtility.AssertNotNull(nameof (source), (object) source);
      return source.ServiceClient.Credentials.TransformUri(source.SnapshotQualifiedUri);
    }

    private void UpdateAfterFetchAttributes(HttpResponseMessage response)
    {
      FileProperties properties = FileHttpResponseParsers.GetProperties(response);
      FileHttpResponseParsers.UpdateSmbProperties(response, properties);
      CopyState copyAttributes = FileHttpResponseParsers.GetCopyAttributes(response);
      this.attributes.Properties = properties;
      this.attributes.Metadata = FileHttpResponseParsers.GetMetadata(response);
      this.attributes.CopyState = copyAttributes;
    }

    private void UpdateETagLMTAndLength(HttpResponseMessage response, bool updateLength)
    {
      FileProperties properties = FileHttpResponseParsers.GetProperties(response);
      this.Properties.ETag = properties.ETag ?? this.Properties.ETag;
      this.Properties.LastModified = properties.LastModified ?? this.Properties.LastModified;
      if (!updateLength)
        return;
      this.Properties.Length = properties.Length;
    }

    public CloudFile(Uri fileAbsoluteUri)
      : this(fileAbsoluteUri, (StorageCredentials) null)
    {
    }

    public CloudFile(Uri fileAbsoluteUri, StorageCredentials credentials)
      : this(new StorageUri(fileAbsoluteUri), credentials)
    {
    }

    public CloudFile(StorageUri fileAbsoluteUri, StorageCredentials credentials)
    {
      this.attributes = new CloudFileAttributes();
      this.ParseQueryAndVerify(fileAbsoluteUri, credentials);
    }

    internal CloudFile(StorageUri uri, string fileName, CloudFileShare share)
    {
      CommonUtility.AssertNotNull(nameof (uri), (object) uri);
      CommonUtility.AssertNotNullOrEmpty(nameof (fileName), fileName);
      CommonUtility.AssertNotNull(nameof (share), (object) share);
      this.attributes = new CloudFileAttributes();
      this.attributes.StorageUri = uri;
      this.ServiceClient = share.ServiceClient;
      this.share = share;
      this.Name = fileName;
    }

    internal CloudFile(CloudFileAttributes attributes, CloudFileClient serviceClient)
    {
      this.attributes = attributes;
      this.ServiceClient = serviceClient;
      this.ParseQueryAndVerify(this.StorageUri, this.ServiceClient.Credentials);
    }

    public string FilePermission
    {
      get => this.filePermission;
      set
      {
        CommonUtility.AssertInBounds<long>(nameof (FilePermission), (long) Encoding.UTF8.GetBytes(value.ToCharArray()).Length, 0L, 8192L);
        this.filePermission = value;
      }
    }

    public CloudFileClient ServiceClient { get; private set; }

    public int StreamWriteSizeInBytes
    {
      get => this.streamWriteSizeInBytes;
      set
      {
        CommonUtility.AssertInBounds<int>(nameof (StreamWriteSizeInBytes), value, 512, 104857600);
        this.streamWriteSizeInBytes = value;
      }
    }

    public int StreamMinimumReadSizeInBytes
    {
      get => this.streamMinimumReadSizeInBytes;
      set
      {
        CommonUtility.AssertInBounds<long>(nameof (StreamMinimumReadSizeInBytes), (long) value, 16384L);
        this.streamMinimumReadSizeInBytes = value;
      }
    }

    public virtual FileProperties Properties
    {
      get => this.attributes.Properties;
      internal set => this.attributes.Properties = value;
    }

    public IDictionary<string, string> Metadata => this.attributes.Metadata;

    public Uri Uri => this.attributes.Uri;

    public StorageUri StorageUri => this.attributes.StorageUri;

    public Uri SnapshotQualifiedUri
    {
      get
      {
        if (!this.Share.SnapshotTime.HasValue)
          return this.Uri;
        UriQueryBuilder uriQueryBuilder = new UriQueryBuilder();
        uriQueryBuilder.Add("sharesnapshot", Request.ConvertDateTimeToSnapshotString(this.Share.SnapshotTime.Value));
        return uriQueryBuilder.AddToUri(this.Uri);
      }
    }

    public StorageUri SnapshotQualifiedStorageUri
    {
      get
      {
        if (!this.Share.SnapshotTime.HasValue)
          return this.StorageUri;
        UriQueryBuilder uriQueryBuilder = new UriQueryBuilder();
        uriQueryBuilder.Add("sharesnapshot", Request.ConvertDateTimeToSnapshotString(this.Share.SnapshotTime.Value));
        return uriQueryBuilder.AddToUri(this.StorageUri);
      }
    }

    internal void AssertNoSnapshot()
    {
      if (this.Share.IsSnapshot)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot perform this operation on a share representing a snapshot."));
    }

    internal void AssertValidFilePermissionOrKey()
    {
      if (this.filePermission != null && this.Properties?.filePermissionKeyToSet != null)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "File permission and file permission key cannot both be set"));
    }

    public CopyState CopyState => this.attributes.CopyState;

    public virtual string Name { get; private set; }

    public CloudFileShare Share
    {
      get
      {
        if (this.share == null)
          this.share = this.ServiceClient.GetShareReference(NavigationHelper.GetShareName(this.Uri, new bool?(this.ServiceClient.UsePathStyleUris)));
        return this.share;
      }
    }

    public CloudFileDirectory Parent
    {
      get
      {
        string parentName;
        StorageUri parentAddress;
        if (this.parent == null && NavigationHelper.GetFileParentNameAndAddress(this.StorageUri, new bool?(this.ServiceClient.UsePathStyleUris), out parentName, out parentAddress))
          this.parent = new CloudFileDirectory(parentAddress, parentName, this.Share);
        return this.parent;
      }
    }

    public string GetSharedAccessSignature(SharedAccessFilePolicy policy) => this.GetSharedAccessSignature(policy, (SharedAccessFileHeaders) null, (string) null);

    public string GetSharedAccessSignature(
      SharedAccessFilePolicy policy,
      string groupPolicyIdentifier)
    {
      return this.GetSharedAccessSignature(policy, (SharedAccessFileHeaders) null, groupPolicyIdentifier);
    }

    public string GetSharedAccessSignature(
      SharedAccessFilePolicy policy,
      SharedAccessFileHeaders headers)
    {
      return this.GetSharedAccessSignature(policy, headers, (string) null);
    }

    public string GetSharedAccessSignature(
      SharedAccessFilePolicy policy,
      SharedAccessFileHeaders headers,
      string groupPolicyIdentifier)
    {
      return this.GetSharedAccessSignature(policy, headers, groupPolicyIdentifier, new SharedAccessProtocol?(), (IPAddressOrRange) null);
    }

    public string GetSharedAccessSignature(
      SharedAccessFilePolicy policy,
      SharedAccessFileHeaders headers,
      string groupPolicyIdentifier,
      SharedAccessProtocol? protocols,
      IPAddressOrRange ipAddressOrRange)
    {
      if (!this.ServiceClient.Credentials.IsSharedKey)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Cannot create Shared Access Signature unless Account Key credentials are used."));
      string canonicalName = this.GetCanonicalName();
      StorageAccountKey key = this.ServiceClient.Credentials.Key;
      string hash = FileSharedAccessSignatureHelper.GetHash(policy, headers, groupPolicyIdentifier, canonicalName, "2019-07-07", protocols, ipAddressOrRange, key.KeyValue);
      return FileSharedAccessSignatureHelper.GetSignature(policy, headers, groupPolicyIdentifier, "f", hash, key.KeyName, "2019-07-07", protocols, ipAddressOrRange).ToString();
    }

    private string GetCanonicalName() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/{0}/{1}/{2}/{3}", (object) "file", (object) this.ServiceClient.Credentials.AccountName, (object) this.Share.Name, (object) NavigationHelper.GetFileAndDirectoryName(this.Uri, new bool?(this.ServiceClient.UsePathStyleUris)).Replace('\\', '/'));

    private void ParseQueryAndVerify(StorageUri address, StorageCredentials credentials)
    {
      StorageCredentials parsedCredentials;
      DateTimeOffset? parsedShareSnapshot;
      this.attributes.StorageUri = NavigationHelper.ParseFileQueryAndVerify(address, out parsedCredentials, out parsedShareSnapshot);
      if (parsedCredentials != null && credentials != null && !credentials.Equals(new StorageCredentials()))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot provide credentials as part of the address and as constructor parameter. Either pass in the address or use a different constructor."));
      if (this.ServiceClient == null)
        this.ServiceClient = new CloudFileClient(NavigationHelper.GetServiceClientBaseAddress(this.StorageUri, new bool?()), credentials ?? parsedCredentials);
      if (parsedShareSnapshot.HasValue)
        this.Share.SnapshotTime = parsedShareSnapshot;
      this.Name = NavigationHelper.GetFileName(this.Uri, new bool?(this.ServiceClient.UsePathStyleUris));
    }
  }
}
