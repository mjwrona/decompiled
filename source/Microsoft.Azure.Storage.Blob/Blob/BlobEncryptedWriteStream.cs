// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.BlobEncryptedWriteStream
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Core.Util;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Storage.Blob
{
  internal sealed class BlobEncryptedWriteStream : CloudBlobStream
  {
    private bool disposed;
    private BlobWriteStream writeStream;
    private CryptoStream cryptoStream;
    private ICryptoTransform transform;

    internal BlobEncryptedWriteStream(
      CloudBlockBlob blockBlob,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      ICryptoTransform transform)
    {
      CommonUtility.AssertNotNull(nameof (transform), (object) transform);
      if (options.EncryptionPolicy.EncryptionMode != BlobEncryptionMode.FullBlob)
        throw new InvalidOperationException("Invalid BlobEncryptionMode set on the policy. Please set it to FullBlob when the policy is used with UploadFromStream.", (Exception) null);
      options.SkipEncryptionPolicyValidation = true;
      this.transform = transform;
      this.writeStream = new BlobWriteStream(blockBlob, accessCondition, options, operationContext)
      {
        IgnoreFlush = true
      };
      this.cryptoStream = new CryptoStream((Stream) this.writeStream, transform, CryptoStreamMode.Write);
    }

    internal BlobEncryptedWriteStream(
      CloudPageBlob pageBlob,
      long pageBlobSize,
      bool createNew,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      ICryptoTransform transform)
    {
      CommonUtility.AssertNotNull(nameof (transform), (object) transform);
      if (options.EncryptionPolicy.EncryptionMode != BlobEncryptionMode.FullBlob)
        throw new InvalidOperationException("Invalid BlobEncryptionMode set on the policy. Please set it to FullBlob when the policy is used with UploadFromStream.", (Exception) null);
      options.SkipEncryptionPolicyValidation = true;
      this.transform = transform;
      this.writeStream = new BlobWriteStream(pageBlob, pageBlobSize, createNew, accessCondition, options, operationContext)
      {
        IgnoreFlush = true
      };
      this.cryptoStream = new CryptoStream((Stream) this.writeStream, transform, CryptoStreamMode.Write);
    }

    internal BlobEncryptedWriteStream(
      CloudAppendBlob appendBlob,
      AccessCondition accessCondition,
      BlobRequestOptions options,
      OperationContext operationContext,
      ICryptoTransform transform)
    {
      CommonUtility.AssertNotNull(nameof (transform), (object) transform);
      if (options.EncryptionPolicy.EncryptionMode != BlobEncryptionMode.FullBlob)
        throw new InvalidOperationException("Invalid BlobEncryptionMode set on the policy. Please set it to FullBlob when the policy is used with UploadFromStream.", (Exception) null);
      options.SkipEncryptionPolicyValidation = true;
      this.transform = transform;
      this.writeStream = new BlobWriteStream(appendBlob, accessCondition, options, operationContext)
      {
        IgnoreFlush = true
      };
      this.cryptoStream = new CryptoStream((Stream) this.writeStream, transform, CryptoStreamMode.Write);
    }

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
      get => throw new NotSupportedException();
      set => throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override void SetLength(long value) => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count) => this.EndWrite(this.BeginWrite(buffer, offset, count, (AsyncCallback) null, (object) null));

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      CommonUtility.AssertNotNull(nameof (buffer), (object) buffer);
      CommonUtility.AssertInBounds<int>(nameof (offset), offset, 0, buffer.Length);
      CommonUtility.AssertInBounds<int>(nameof (count), count, 0, buffer.Length - offset);
      return this.cryptoStream.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void EndWrite(IAsyncResult asyncResult) => this.cryptoStream.EndWrite(asyncResult);

    public override void Flush() => throw new NotSupportedException();

    public override ICancellableAsyncResult BeginFlush(AsyncCallback callback, object state) => throw new NotSupportedException();

    public override void EndFlush(IAsyncResult asyncResult) => throw new NotSupportedException();

    public override Task FlushAsync(CancellationToken token) => throw new NotSupportedException();

    public override void Commit()
    {
      this.cryptoStream.FlushFinalBlock();
      this.writeStream.IgnoreFlush = false;
      this.writeStream.Commit();
    }

    public override ICancellableAsyncResult BeginCommit(AsyncCallback callback, object state) => CancellableAsyncResultTaskWrapper.Create((Func<CancellationToken, Task>) (token => this.CommitAsync()), callback, state);

    public override void EndCommit(IAsyncResult asyncResult) => ((CancellableAsyncResultTaskWrapper) asyncResult).GetAwaiter().GetResult();

    public override Task CommitAsync()
    {
      this.cryptoStream.FlushFinalBlock();
      this.writeStream.IgnoreFlush = false;
      return this.writeStream.CommitAsync();
    }

    protected override void Dispose(bool disposing)
    {
      if (this.disposed)
        return;
      this.disposed = true;
      if (!disposing)
        return;
      this.writeStream.IgnoreFlush = false;
      this.cryptoStream.Dispose();
      if (this.transform == null)
        return;
      this.transform.Dispose();
    }
  }
}
