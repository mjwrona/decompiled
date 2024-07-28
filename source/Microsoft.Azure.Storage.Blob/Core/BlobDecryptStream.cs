// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Core.BlobDecryptStream
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Core.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace Microsoft.Azure.Storage.Core
{
  internal class BlobDecryptStream : Stream
  {
    private readonly Stream userStream;
    private readonly IDictionary<string, string> metadata;
    private long position;
    private long? userProvidedLength;
    private byte[] iv = new byte[16];
    private BlobEncryptionPolicy encryptionPolicy;
    private int discardFirst;
    private Stream cryptoStream;
    private bool bufferIV;
    private bool noPadding;
    private bool disposed;
    private bool? requireEncryption;
    private ICryptoTransform transform;

    public BlobDecryptStream(
      Stream userStream,
      IDictionary<string, string> metadata,
      long? userProvidedLength,
      int discardFirst,
      bool bufferIV,
      bool noPadding,
      BlobEncryptionPolicy policy,
      bool? requireEncryption)
    {
      this.userStream = userStream;
      this.metadata = metadata;
      this.userProvidedLength = userProvidedLength;
      this.discardFirst = discardFirst;
      this.encryptionPolicy = policy;
      this.bufferIV = bufferIV;
      this.noPadding = noPadding;
      this.requireEncryption = requireEncryption;
    }

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
      get => this.position;
      set => throw new NotSupportedException();
    }

    public override void Flush() => this.userStream.Flush();

    public override void SetLength(long value)
    {
    }

    public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

    public override int Read(byte[] buffer, int offset, int count) => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
    {
      if (this.bufferIV && this.position < 16L)
      {
        int num = 16 - (int) this.position;
        int length = count > num ? num : count;
        Array.Copy((Array) buffer, offset, (Array) this.iv, (int) this.position, length);
        this.position += (long) length;
        offset += length;
        count -= length;
        if (this.position < 16L)
          return;
      }
      if (this.cryptoStream == null)
        this.cryptoStream = this.encryptionPolicy.DecryptBlob((Stream) new LengthLimitingStream(this.userStream, (long) this.discardFirst, this.userProvidedLength), this.metadata, out this.transform, this.requireEncryption, !this.bufferIV ? (byte[]) null : this.iv, this.noPadding);
      if (count <= 0)
        return;
      this.cryptoStream.Write(buffer, offset, count);
      this.position += (long) count;
    }

    public override IAsyncResult BeginWrite(
      byte[] buffer,
      int offset,
      int count,
      AsyncCallback callback,
      object state)
    {
      StorageAsyncResult<NullType> state1 = new StorageAsyncResult<NullType>(callback, state);
      if (this.bufferIV && this.position < 16L)
      {
        int num = 16 - (int) this.position;
        int length = count > num ? num : count;
        Array.Copy((Array) buffer, offset, (Array) this.iv, (int) this.position, length);
        this.position += (long) length;
        offset += length;
        count -= length;
        if (this.position < 16L)
        {
          state1.OnComplete();
          return (IAsyncResult) state1;
        }
      }
      if (this.cryptoStream == null)
        this.cryptoStream = this.encryptionPolicy.DecryptBlob((Stream) new LengthLimitingStream(this.userStream, (long) this.discardFirst, this.userProvidedLength), this.metadata, out this.transform, this.requireEncryption, !this.bufferIV ? (byte[]) null : this.iv, this.noPadding);
      if (count <= 0)
      {
        state1.OnComplete();
      }
      else
      {
        state1.OperationState = (object) count;
        this.cryptoStream.BeginWrite(buffer, offset, count, new AsyncCallback(this.WriteStreamCallback), (object) state1);
      }
      return (IAsyncResult) state1;
    }

    private void WriteStreamCallback(IAsyncResult ar)
    {
      StorageAsyncResult<NullType> asyncState = (StorageAsyncResult<NullType>) ar.AsyncState;
      asyncState.UpdateCompletedSynchronously(ar.CompletedSynchronously);
      Exception exception = (Exception) null;
      try
      {
        this.cryptoStream.EndWrite(ar);
        this.position += (long) (int) asyncState.OperationState;
      }
      catch (Exception ex)
      {
        exception = ex;
      }
      asyncState.OnComplete(exception);
    }

    public override void EndWrite(IAsyncResult asyncResult) => ((StorageCommandAsyncResult) asyncResult).End();

    protected override void Dispose(bool disposing)
    {
      if (!this.disposed)
      {
        this.disposed = true;
        if (disposing && this.cryptoStream != null)
          this.cryptoStream.Close();
        if (this.transform != null)
          this.transform.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}
