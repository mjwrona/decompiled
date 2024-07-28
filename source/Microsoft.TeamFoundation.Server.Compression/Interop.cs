// Decompiled with JetBrains decompiler
// Type: Interop
// Assembly: Microsoft.TeamFoundation.Server.Compression, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E666AAE4-36CD-4581-80AF-1B631308AB46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.Compression.dll

using Microsoft.TeamFoundation.Server.Compression;
using System;
using System.IO;
using System.Runtime.InteropServices;

internal static class Interop
{
  internal static class zlib
  {
    private static readonly byte[] ZLibVersion = new byte[6]
    {
      (byte) 49,
      (byte) 46,
      (byte) 50,
      (byte) 46,
      (byte) 51,
      (byte) 0
    };
    private const string c_NativeBinaries = "NativeBinaries";

    [DllImport("tfszlib.dll")]
    private static extern unsafe int deflateInit2_(
      byte* stream,
      int level,
      int method,
      int windowBits,
      int memLevel,
      int strategy,
      byte* version,
      int stream_size);

    [DllImport("tfszlib.dll")]
    private static extern unsafe int deflate(byte* stream, int flush);

    [DllImport("tfszlib.dll")]
    private static extern unsafe int deflateEnd(byte* strm);

    [DllImport("tfszlib.dll")]
    internal static extern unsafe uint crc32(uint crc, byte* buffer, int len);

    [DllImport("tfszlib.dll")]
    private static extern unsafe int inflateInit2_(
      byte* stream,
      int windowBits,
      byte* version,
      int stream_size);

    [DllImport("tfszlib.dll")]
    private static extern unsafe int inflate(byte* stream, int flush);

    [DllImport("tfszlib.dll")]
    private static extern unsafe int inflateEnd(byte* stream);

    internal static unsafe ZLibNative.ErrorCode DeflateInit2_(
      ref ZLibNative.ZStream stream,
      ZLibNative.CompressionLevel level,
      ZLibNative.CompressionMethod method,
      int windowBits,
      int memLevel,
      ZLibNative.CompressionStrategy strategy)
    {
      fixed (byte* version = Interop.zlib.ZLibVersion)
        fixed (ZLibNative.ZStream* stream1 = &stream)
          return (ZLibNative.ErrorCode) Interop.zlib.deflateInit2_((byte*) stream1, (int) level, (int) method, windowBits, memLevel, (int) strategy, version, sizeof (ZLibNative.ZStream));
    }

    internal static unsafe uint crc32(uint crc, byte[] buffer, int offset, int len)
    {
      fixed (byte* buffer1 = &buffer[offset])
        return Interop.zlib.crc32(crc, buffer1, len);
    }

    internal static unsafe ZLibNative.ErrorCode Deflate(
      ref ZLibNative.ZStream stream,
      ZLibNative.FlushCode flush)
    {
      fixed (ZLibNative.ZStream* stream1 = &stream)
        return (ZLibNative.ErrorCode) Interop.zlib.deflate((byte*) stream1, (int) flush);
    }

    internal static unsafe ZLibNative.ErrorCode DeflateEnd(ref ZLibNative.ZStream stream)
    {
      fixed (ZLibNative.ZStream* strm = &stream)
        return (ZLibNative.ErrorCode) Interop.zlib.deflateEnd((byte*) strm);
    }

    internal static unsafe ZLibNative.ErrorCode InflateInit2_(
      ref ZLibNative.ZStream stream,
      int windowBits)
    {
      fixed (byte* version = Interop.zlib.ZLibVersion)
        fixed (ZLibNative.ZStream* stream1 = &stream)
          return (ZLibNative.ErrorCode) Interop.zlib.inflateInit2_((byte*) stream1, windowBits, version, sizeof (ZLibNative.ZStream));
    }

    internal static unsafe ZLibNative.ErrorCode Inflate(
      ref ZLibNative.ZStream stream,
      ZLibNative.FlushCode flush)
    {
      fixed (ZLibNative.ZStream* stream1 = &stream)
        return (ZLibNative.ErrorCode) Interop.zlib.inflate((byte*) stream1, (int) flush);
    }

    internal static unsafe ZLibNative.ErrorCode InflateEnd(ref ZLibNative.ZStream stream)
    {
      fixed (ZLibNative.ZStream* stream1 = &stream)
        return (ZLibNative.ErrorCode) Interop.zlib.inflateEnd((byte*) stream1);
    }

    [DllImport("kernel32.dll")]
    public static extern IntPtr LoadLibrary(string dll);

    static zlib()
    {
      string path1 = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
      if (File.Exists(Path.Combine(path1, "tfszlib.dll")))
        return;
      string path3 = Environment.Is64BitProcess ? "amd64" : "x86";
      Interop.zlib.LoadLibrary(Path.Combine(path1, "NativeBinaries", path3, "tfszlib.dll"));
    }

    private static class Libraries
    {
      public const string Zlib = "tfszlib.dll";
      public const string Kernel32 = "kernel32.dll";
    }

    private static class Platforms
    {
      public const string x86 = "x86";
      public const string x64 = "amd64";
    }
  }
}
