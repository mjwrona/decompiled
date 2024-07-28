// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.XpressNative
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [CLSCompliant(false)]
  public static class XpressNative
  {
    private const int UncompressedChunkSize = 4096;
    public const uint STATUS_BUFFER_TOO_SMALL = 3221225507;
    internal static readonly Lazy<ByteArrayPool> NativeWorkspacePool = new Lazy<ByteArrayPool>((Func<ByteArrayPool>) (() => new ByteArrayPool((int) XpressNative.NativeMethods.CompressBufferWorkSpaceSize.Value, 4 * Environment.ProcessorCount)));

    public static uint TryCompressChunk(
      byte[] uncompressedChunk,
      uint uncompressedChunkSize,
      byte[] compressedChunk,
      out uint compressedChunkSize)
    {
      if (uncompressedChunkSize < 16U)
      {
        compressedChunkSize = 0U;
        return 3221225507;
      }
      using (Pool<byte[]>.PoolHandle poolHandle = XpressNative.NativeWorkspacePool.Value.Get())
        return XpressNative.NativeMethods.RtlCompressBuffer((ushort) 259, uncompressedChunk, uncompressedChunkSize, compressedChunk, (uint) compressedChunk.Length, 4096U, out compressedChunkSize, poolHandle.Value);
    }

    public static uint DecompressChunk(
      byte[] compressedChunk,
      uint compressedCount,
      byte[] uncompressedChunk,
      out uint uncompressedChunkSize)
    {
      using (Pool<byte[]>.PoolHandle poolHandle = XpressNative.NativeWorkspacePool.Value.Get())
        return XpressNative.NativeMethods.RtlDecompressBufferEx((ushort) 259, uncompressedChunk, (uint) uncompressedChunk.Length, compressedChunk, compressedCount, out uncompressedChunkSize, poolHandle.Value);
    }

    private static class NativeMethods
    {
      public const ushort COMPRESSION_ENGINE_MAXIMUM = 256;
      public const ushort COMPRESSION_FORMAT_XPRESS = 3;
      public static readonly Lazy<uint> CompressBufferWorkSpaceSize = new Lazy<uint>((Func<uint>) (() =>
      {
        uint compressBufferWorkSpaceSize;
        uint compressionWorkSpaceSize = XpressNative.NativeMethods.RtlGetCompressionWorkSpaceSize((ushort) 259, out compressBufferWorkSpaceSize, out uint _);
        if (compressionWorkSpaceSize != 0U)
          throw new Exception(string.Format("RtlGetCompressionWorkSpaceSize failed 0x{0:X}", (object) compressionWorkSpaceSize));
        return compressBufferWorkSpaceSize;
      }), LazyThreadSafetyMode.PublicationOnly);

      [DllImport("ntdll.dll")]
      private static extern uint RtlGetCompressionWorkSpaceSize(
        ushort compressionFormat,
        out uint compressBufferWorkSpaceSize,
        out uint compressFragmentWorkSpaceSize);

      [DllImport("ntdll.dll")]
      public static extern uint RtlDecompressBufferEx(
        ushort compressionFormat,
        byte[] uncompressedBuffer,
        uint uncompressedBufferSize,
        byte[] compressedBuffer,
        uint compressedBufferSize,
        out uint finalUncompressedSize,
        byte[] workSpace);

      [DllImport("ntdll.dll")]
      public static extern uint RtlCompressBuffer(
        ushort compressionFormatAndEngine,
        byte[] uncompressedBuffer,
        uint uncompressedBufferSize,
        byte[] compressedBuffer,
        uint compressedBufferSize,
        uint uncompressedChunkSize,
        out uint finalCompressedSize,
        byte[] workSpace);
    }
  }
}
