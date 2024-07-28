// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Compression.Inflater
// Assembly: Microsoft.TeamFoundation.Server.Compression, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E666AAE4-36CD-4581-80AF-1B631308AB46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.Compression.dll

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.TeamFoundation.Server.Compression
{
  internal sealed class Inflater : IDisposable
  {
    private bool _finished;
    private bool _isDisposed;
    private ZLibNative.ZLibStreamHandle _zlibStream;
    private GCHandle _inputBufferHandle;
    private readonly object _syncLock = new object();
    private const int minWindowBits = -15;
    private const int maxWindowBits = 47;

    internal Inflater(int windowBits)
    {
      this._finished = false;
      this._isDisposed = false;
      this.InflateInit(windowBits);
    }

    public int AvailableOutput => (int) this._zlibStream.AvailOut;

    public int AvailableInput => (int) this._zlibStream.AvailIn;

    public bool Finished() => this._finished;

    public unsafe bool Inflate(out byte b)
    {
      if (!this.NeedsInput())
      {
        GCHandle inputBufferHandle = this._inputBufferHandle;
        if (this._inputBufferHandle.IsAllocated)
        {
          fixed (byte* bufPtr = &b)
            return this.InflateVerified(bufPtr, 1) != 0;
        }
      }
      b = (byte) 0;
      return false;
    }

    public unsafe int Inflate(byte[] bytes, int offset, int length)
    {
      if (!this.NeedsInput())
      {
        GCHandle inputBufferHandle = this._inputBufferHandle;
        if (this._inputBufferHandle.IsAllocated && length != 0)
        {
          fixed (byte* numPtr = bytes)
            return this.InflateVerified(numPtr + offset, length);
        }
      }
      return 0;
    }

    public unsafe int InflateVerified(byte* bufPtr, int length)
    {
      try
      {
        int bytesRead;
        if (this.ReadInflateOutput(bufPtr, length, ZLibNative.FlushCode.NoFlush, out bytesRead) == ZLibNative.ErrorCode.StreamEnd)
          this._finished = true;
        return bytesRead;
      }
      finally
      {
        if (this._zlibStream.AvailIn == 0U && this._inputBufferHandle.IsAllocated)
          this.DeallocateInputBufferHandle();
      }
    }

    public bool NeedsInput() => this._zlibStream.AvailIn == 0U;

    public void SetInput(byte[] inputBuffer, int startIndex, int count)
    {
      if (count == 0)
        return;
      lock (this._syncLock)
      {
        this._inputBufferHandle = GCHandle.Alloc((object) inputBuffer, GCHandleType.Pinned);
        this._zlibStream.NextIn = this._inputBufferHandle.AddrOfPinnedObject() + startIndex;
        this._zlibStream.AvailIn = (uint) count;
        this._finished = false;
      }
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

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    ~Inflater() => this.Dispose(false);

    [SecuritySafeCritical]
    private void InflateInit(int windowBits)
    {
      ZLibNative.ErrorCode streamForInflate;
      try
      {
        streamForInflate = ZLibNative.CreateZLibStreamForInflate(out this._zlibStream, windowBits);
      }
      catch (Exception ex)
      {
        throw new ZLibException(SR.ZLibErrorDLLLoadError, ex);
      }
      switch (streamForInflate)
      {
        case ZLibNative.ErrorCode.VersionError:
          throw new ZLibException(SR.ZLibErrorVersionMismatch, "inflateInit2_", (int) streamForInflate, this._zlibStream.GetErrorMessage());
        case ZLibNative.ErrorCode.MemError:
          throw new ZLibException(SR.ZLibErrorNotEnoughMemory, "inflateInit2_", (int) streamForInflate, this._zlibStream.GetErrorMessage());
        case ZLibNative.ErrorCode.StreamError:
          throw new ZLibException(SR.ZLibErrorIncorrectInitParameters, "inflateInit2_", (int) streamForInflate, this._zlibStream.GetErrorMessage());
        case ZLibNative.ErrorCode.Ok:
          break;
        default:
          throw new ZLibException(SR.ZLibErrorUnexpected, "inflateInit2_", (int) streamForInflate, this._zlibStream.GetErrorMessage());
      }
    }

    private unsafe ZLibNative.ErrorCode ReadInflateOutput(
      byte* bufPtr,
      int length,
      ZLibNative.FlushCode flushCode,
      out int bytesRead)
    {
      lock (this._syncLock)
      {
        this._zlibStream.NextOut = (IntPtr) (void*) bufPtr;
        this._zlibStream.AvailOut = (uint) length;
        int num = (int) this.Inflate(flushCode);
        bytesRead = length - (int) this._zlibStream.AvailOut;
        return (ZLibNative.ErrorCode) num;
      }
    }

    [SecuritySafeCritical]
    private ZLibNative.ErrorCode Inflate(ZLibNative.FlushCode flushCode)
    {
      ZLibNative.ErrorCode zlibErrorCode;
      try
      {
        zlibErrorCode = this._zlibStream.Inflate(flushCode);
      }
      catch (Exception ex)
      {
        throw new ZLibException(SR.ZLibErrorDLLLoadError, ex);
      }
      switch (zlibErrorCode)
      {
        case ZLibNative.ErrorCode.BufError:
          return zlibErrorCode;
        case ZLibNative.ErrorCode.MemError:
          throw new ZLibException(SR.ZLibErrorNotEnoughMemory, "inflate_", (int) zlibErrorCode, this._zlibStream.GetErrorMessage());
        case ZLibNative.ErrorCode.DataError:
          throw new InvalidDataException(SR.UnsupportedCompression);
        case ZLibNative.ErrorCode.StreamError:
          throw new ZLibException(SR.ZLibErrorInconsistentStream, "inflate_", (int) zlibErrorCode, this._zlibStream.GetErrorMessage());
        case ZLibNative.ErrorCode.Ok:
        case ZLibNative.ErrorCode.StreamEnd:
          return zlibErrorCode;
        default:
          throw new ZLibException(SR.ZLibErrorUnexpected, "inflate_", (int) zlibErrorCode, this._zlibStream.GetErrorMessage());
      }
    }

    private void DeallocateInputBufferHandle()
    {
      lock (this._syncLock)
      {
        this._zlibStream.AvailIn = 0U;
        this._zlibStream.NextIn = ZLibNative.ZNullPtr;
        this._inputBufferHandle.Free();
      }
    }
  }
}
