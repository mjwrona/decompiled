// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Compression.ZLibNative
// Assembly: Microsoft.TeamFoundation.Server.Compression, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E666AAE4-36CD-4581-80AF-1B631308AB46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.Compression.dll

using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.TeamFoundation.Server.Compression
{
  internal static class ZLibNative
  {
    internal static readonly IntPtr ZNullPtr = (IntPtr) 0;
    public const int Deflate_DefaultWindowBits = -15;
    public const int GZip_DefaultWindowBits = 31;
    public const int Deflate_DefaultMemLevel = 8;
    public const int Deflate_NoCompressionMemLevel = 7;

    [SecurityCritical]
    public static ZLibNative.ErrorCode CreateZLibStreamForDeflate(
      out ZLibNative.ZLibStreamHandle zLibStreamHandle,
      ZLibNative.CompressionLevel level,
      int windowBits,
      int memLevel,
      ZLibNative.CompressionStrategy strategy)
    {
      zLibStreamHandle = new ZLibNative.ZLibStreamHandle();
      return zLibStreamHandle.DeflateInit2_(level, windowBits, memLevel, strategy);
    }

    [SecurityCritical]
    public static ZLibNative.ErrorCode CreateZLibStreamForInflate(
      out ZLibNative.ZLibStreamHandle zLibStreamHandle,
      int windowBits)
    {
      zLibStreamHandle = new ZLibNative.ZLibStreamHandle();
      return zLibStreamHandle.InflateInit2_(windowBits);
    }

    public enum FlushCode
    {
      NoFlush = 0,
      SyncFlush = 2,
      Finish = 4,
    }

    public enum ErrorCode
    {
      VersionError = -6, // 0xFFFFFFFA
      BufError = -5, // 0xFFFFFFFB
      MemError = -4, // 0xFFFFFFFC
      DataError = -3, // 0xFFFFFFFD
      StreamError = -2, // 0xFFFFFFFE
      Ok = 0,
      StreamEnd = 1,
    }

    public enum CompressionLevel
    {
      DefaultCompression = -1, // 0xFFFFFFFF
      NoCompression = 0,
      BestSpeed = 1,
    }

    public enum CompressionStrategy
    {
      DefaultStrategy,
    }

    public enum CompressionMethod
    {
      Deflated = 8,
    }

    [SecurityCritical]
    public sealed class ZLibStreamHandle : SafeHandle
    {
      private ZLibNative.ZStream _zStream;
      [SecurityCritical]
      private volatile ZLibNative.ZLibStreamHandle.State _initializationState;

      public ZLibStreamHandle()
        : base(new IntPtr(-1), true)
      {
        this._zStream = new ZLibNative.ZStream();
        this._zStream.Init();
        this._initializationState = ZLibNative.ZLibStreamHandle.State.NotInitialized;
        this.SetHandle(IntPtr.Zero);
      }

      public override bool IsInvalid
      {
        [SecurityCritical] get => this.handle == new IntPtr(-1);
      }

      public ZLibNative.ZLibStreamHandle.State InitializationState
      {
        [SecurityCritical] get => this._initializationState;
      }

      [SecurityCritical]
      protected override bool ReleaseHandle()
      {
        switch (this.InitializationState)
        {
          case ZLibNative.ZLibStreamHandle.State.NotInitialized:
            return true;
          case ZLibNative.ZLibStreamHandle.State.InitializedForDeflate:
            return this.DeflateEnd() == ZLibNative.ErrorCode.Ok;
          case ZLibNative.ZLibStreamHandle.State.InitializedForInflate:
            return this.InflateEnd() == ZLibNative.ErrorCode.Ok;
          case ZLibNative.ZLibStreamHandle.State.Disposed:
            return true;
          default:
            return false;
        }
      }

      public IntPtr NextIn
      {
        [SecurityCritical] get => this._zStream.nextIn;
        [SecurityCritical] set => this._zStream.nextIn = value;
      }

      public uint AvailIn
      {
        [SecurityCritical] get => this._zStream.availIn;
        [SecurityCritical] set => this._zStream.availIn = value;
      }

      public IntPtr NextOut
      {
        [SecurityCritical] get => this._zStream.nextOut;
        [SecurityCritical] set => this._zStream.nextOut = value;
      }

      public uint AvailOut
      {
        [SecurityCritical] get => this._zStream.availOut;
        [SecurityCritical] set => this._zStream.availOut = value;
      }

      [SecurityCritical]
      private void EnsureNotDisposed()
      {
        if (this.InitializationState == ZLibNative.ZLibStreamHandle.State.Disposed)
          throw new ObjectDisposedException(this.GetType().ToString());
      }

      [SecurityCritical]
      private void EnsureState(ZLibNative.ZLibStreamHandle.State requiredState)
      {
        if (this.InitializationState != requiredState)
          throw new InvalidOperationException("InitializationState != " + requiredState.ToString());
      }

      [SecurityCritical]
      public ZLibNative.ErrorCode DeflateInit2_(
        ZLibNative.CompressionLevel level,
        int windowBits,
        int memLevel,
        ZLibNative.CompressionStrategy strategy)
      {
        this.EnsureNotDisposed();
        this.EnsureState(ZLibNative.ZLibStreamHandle.State.NotInitialized);
        int num = (int) Interop.zlib.DeflateInit2_(ref this._zStream, level, ZLibNative.CompressionMethod.Deflated, windowBits, memLevel, strategy);
        this._initializationState = ZLibNative.ZLibStreamHandle.State.InitializedForDeflate;
        return (ZLibNative.ErrorCode) num;
      }

      [SecurityCritical]
      public ZLibNative.ErrorCode Deflate(ZLibNative.FlushCode flush)
      {
        this.EnsureNotDisposed();
        this.EnsureState(ZLibNative.ZLibStreamHandle.State.InitializedForDeflate);
        return Interop.zlib.Deflate(ref this._zStream, flush);
      }

      [SecurityCritical]
      public ZLibNative.ErrorCode DeflateEnd()
      {
        this.EnsureNotDisposed();
        this.EnsureState(ZLibNative.ZLibStreamHandle.State.InitializedForDeflate);
        int num = (int) Interop.zlib.DeflateEnd(ref this._zStream);
        this._initializationState = ZLibNative.ZLibStreamHandle.State.Disposed;
        return (ZLibNative.ErrorCode) num;
      }

      [SecurityCritical]
      public ZLibNative.ErrorCode InflateInit2_(int windowBits)
      {
        this.EnsureNotDisposed();
        this.EnsureState(ZLibNative.ZLibStreamHandle.State.NotInitialized);
        int num = (int) Interop.zlib.InflateInit2_(ref this._zStream, windowBits);
        this._initializationState = ZLibNative.ZLibStreamHandle.State.InitializedForInflate;
        return (ZLibNative.ErrorCode) num;
      }

      [SecurityCritical]
      public ZLibNative.ErrorCode Inflate(ZLibNative.FlushCode flush)
      {
        this.EnsureNotDisposed();
        this.EnsureState(ZLibNative.ZLibStreamHandle.State.InitializedForInflate);
        return Interop.zlib.Inflate(ref this._zStream, flush);
      }

      [SecurityCritical]
      public ZLibNative.ErrorCode InflateEnd()
      {
        this.EnsureNotDisposed();
        this.EnsureState(ZLibNative.ZLibStreamHandle.State.InitializedForInflate);
        int num = (int) Interop.zlib.InflateEnd(ref this._zStream);
        this._initializationState = ZLibNative.ZLibStreamHandle.State.Disposed;
        return (ZLibNative.ErrorCode) num;
      }

      [SecurityCritical]
      public ZLibNative.ErrorCode InflateReset(int windowBits)
      {
        this.EnsureNotDisposed();
        this.EnsureState(ZLibNative.ZLibStreamHandle.State.InitializedForInflate);
        ZLibNative.ErrorCode errorCode1 = Interop.zlib.InflateEnd(ref this._zStream);
        if (errorCode1 != ZLibNative.ErrorCode.Ok)
        {
          this._initializationState = ZLibNative.ZLibStreamHandle.State.Disposed;
          return errorCode1;
        }
        ZLibNative.ErrorCode errorCode2 = Interop.zlib.InflateInit2_(ref this._zStream, windowBits);
        this._initializationState = ZLibNative.ZLibStreamHandle.State.InitializedForInflate;
        return errorCode2;
      }

      [SecurityCritical]
      public string GetErrorMessage() => !(this._zStream.msg != ZLibNative.ZNullPtr) ? string.Empty : Marshal.PtrToStringAnsi(this._zStream.msg);

      public enum State
      {
        NotInitialized,
        InitializedForDeflate,
        InitializedForInflate,
        Disposed,
      }
    }

    internal struct ZStream
    {
      internal IntPtr nextIn;
      internal uint availIn;
      internal uint totalIn;
      internal IntPtr nextOut;
      internal uint availOut;
      internal uint totalOut;
      internal IntPtr msg;
      internal IntPtr state;
      internal IntPtr zalloc;
      internal IntPtr zfree;
      internal IntPtr opaque;
      internal int dataType;
      internal uint adler;
      internal uint reserved;

      internal void Init()
      {
        this.zalloc = ZLibNative.ZNullPtr;
        this.zfree = ZLibNative.ZNullPtr;
        this.opaque = ZLibNative.ZNullPtr;
      }
    }
  }
}
