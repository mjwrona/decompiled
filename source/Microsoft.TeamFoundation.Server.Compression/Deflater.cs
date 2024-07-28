// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Compression.Deflater
// Assembly: Microsoft.TeamFoundation.Server.Compression, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E666AAE4-36CD-4581-80AF-1B631308AB46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.Compression.dll

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.TeamFoundation.Server.Compression
{
  internal sealed class Deflater : IDisposable
  {
    private ZLibNative.ZLibStreamHandle _zlibStream;
    private GCHandle _inputBufferHandle;
    private bool _isDisposed;
    private const int minWindowBits = -15;
    private const int maxWindowBits = 31;
    private readonly object _syncLock = new object();

    internal Deflater(CompressionLevel compressionLevel, int windowBits)
    {
      ZLibNative.CompressionLevel compressionLevel1;
      int memLevel;
      switch (compressionLevel)
      {
        case CompressionLevel.Optimal:
          compressionLevel1 = ZLibNative.CompressionLevel.DefaultCompression;
          memLevel = 8;
          break;
        case CompressionLevel.Fastest:
          compressionLevel1 = ZLibNative.CompressionLevel.BestSpeed;
          memLevel = 8;
          break;
        case CompressionLevel.NoCompression:
          compressionLevel1 = ZLibNative.CompressionLevel.NoCompression;
          memLevel = 7;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (compressionLevel));
      }
      ZLibNative.CompressionStrategy strategy = ZLibNative.CompressionStrategy.DefaultStrategy;
      this.DeflateInit(compressionLevel1, windowBits, memLevel, strategy);
    }

    ~Deflater() => this.Dispose(false);

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    [SecuritySafeCritical]
    private void Dispose(bool disposing)
    {
      if (this._isDisposed)
        return;
      if (disposing)
        this._zlibStream.Dispose();
      if (this._inputBufferHandle.IsAllocated)
        this.DeallocateInputBufferHandle();
      this._isDisposed = true;
    }

    public bool NeedsInput() => this._zlibStream.AvailIn == 0U;

    internal void SetInput(byte[] inputBuffer, int startIndex, int count)
    {
      if (count == 0)
        return;
      lock (this._syncLock)
      {
        this._inputBufferHandle = GCHandle.Alloc((object) inputBuffer, GCHandleType.Pinned);
        this._zlibStream.NextIn = this._inputBufferHandle.AddrOfPinnedObject() + startIndex;
        this._zlibStream.AvailIn = (uint) count;
      }
    }

    internal int GetDeflateOutput(byte[] outputBuffer)
    {
      try
      {
        int bytesRead;
        int num = (int) this.ReadDeflateOutput(outputBuffer, ZLibNative.FlushCode.NoFlush, out bytesRead);
        return bytesRead;
      }
      finally
      {
        if (this._zlibStream.AvailIn == 0U && this._inputBufferHandle.IsAllocated)
          this.DeallocateInputBufferHandle();
      }
    }

    private unsafe ZLibNative.ErrorCode ReadDeflateOutput(
      byte[] outputBuffer,
      ZLibNative.FlushCode flushCode,
      out int bytesRead)
    {
      lock (this._syncLock)
      {
        fixed (byte* numPtr = outputBuffer)
        {
          this._zlibStream.NextOut = (IntPtr) (void*) numPtr;
          this._zlibStream.AvailOut = (uint) outputBuffer.Length;
          int num = (int) this.Deflate(flushCode);
          bytesRead = outputBuffer.Length - (int) this._zlibStream.AvailOut;
          return (ZLibNative.ErrorCode) num;
        }
      }
    }

    internal bool Finish(byte[] outputBuffer, out int bytesRead) => this.ReadDeflateOutput(outputBuffer, ZLibNative.FlushCode.Finish, out bytesRead) == ZLibNative.ErrorCode.StreamEnd;

    internal bool Flush(byte[] outputBuffer, out int bytesRead) => this.ReadDeflateOutput(outputBuffer, ZLibNative.FlushCode.SyncFlush, out bytesRead) == ZLibNative.ErrorCode.Ok;

    private void DeallocateInputBufferHandle()
    {
      lock (this._syncLock)
      {
        this._zlibStream.AvailIn = 0U;
        this._zlibStream.NextIn = ZLibNative.ZNullPtr;
        this._inputBufferHandle.Free();
      }
    }

    [SecuritySafeCritical]
    private void DeflateInit(
      ZLibNative.CompressionLevel compressionLevel,
      int windowBits,
      int memLevel,
      ZLibNative.CompressionStrategy strategy)
    {
      ZLibNative.ErrorCode streamForDeflate;
      try
      {
        streamForDeflate = ZLibNative.CreateZLibStreamForDeflate(out this._zlibStream, compressionLevel, windowBits, memLevel, strategy);
      }
      catch (Exception ex)
      {
        throw new ZLibException(SR.ZLibErrorDLLLoadError, ex);
      }
      switch (streamForDeflate)
      {
        case ZLibNative.ErrorCode.VersionError:
          throw new ZLibException(SR.ZLibErrorVersionMismatch, "deflateInit2_", (int) streamForDeflate, this._zlibStream.GetErrorMessage());
        case ZLibNative.ErrorCode.MemError:
          throw new ZLibException(SR.ZLibErrorNotEnoughMemory, "deflateInit2_", (int) streamForDeflate, this._zlibStream.GetErrorMessage());
        case ZLibNative.ErrorCode.StreamError:
          throw new ZLibException(SR.ZLibErrorIncorrectInitParameters, "deflateInit2_", (int) streamForDeflate, this._zlibStream.GetErrorMessage());
        case ZLibNative.ErrorCode.Ok:
          break;
        default:
          throw new ZLibException(SR.ZLibErrorUnexpected, "deflateInit2_", (int) streamForDeflate, this._zlibStream.GetErrorMessage());
      }
    }

    [SecuritySafeCritical]
    private ZLibNative.ErrorCode Deflate(ZLibNative.FlushCode flushCode)
    {
      ZLibNative.ErrorCode zlibErrorCode;
      try
      {
        zlibErrorCode = this._zlibStream.Deflate(flushCode);
      }
      catch (Exception ex)
      {
        throw new ZLibException(SR.ZLibErrorDLLLoadError, ex);
      }
      switch (zlibErrorCode)
      {
        case ZLibNative.ErrorCode.BufError:
          return zlibErrorCode;
        case ZLibNative.ErrorCode.StreamError:
          throw new ZLibException(SR.ZLibErrorInconsistentStream, "deflate", (int) zlibErrorCode, this._zlibStream.GetErrorMessage());
        case ZLibNative.ErrorCode.Ok:
        case ZLibNative.ErrorCode.StreamEnd:
          return zlibErrorCode;
        default:
          throw new ZLibException(SR.ZLibErrorUnexpected, "deflate", (int) zlibErrorCode, this._zlibStream.GetErrorMessage());
      }
    }
  }
}
