// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ChunkCompression
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [CLSCompliant(false)]
  public static class ChunkCompression
  {
    private static readonly bool IsWindows = true;
    private static readonly Version MinSupportedNativeWindowsVersion = new Version(6, 2);
    private static readonly bool UseNative = ChunkCompression.DetermineUseNativeCompression();

    private static bool DetermineUseNativeCompression()
    {
      string environmentVariable = Environment.GetEnvironmentVariable("VSTS_XPRESS_COMPRESSION");
      switch (environmentVariable)
      {
        case null:
          return ChunkCompression.IsWindows && Environment.OSVersion.Version >= ChunkCompression.MinSupportedNativeWindowsVersion;
        case "NATIVE":
          return true;
        case "MANAGED":
          return false;
        default:
          throw new ArgumentException("Unknown VSTS_XPRESS_COMPRESSION value: " + environmentVariable);
      }
    }

    public static uint? TryCompressChunk(
      byte[] uncompressedChunk,
      uint uncompressedChunkSize,
      byte[] compressedChunk)
    {
      uint compressedChunkSize;
      uint num = !ChunkCompression.UseNative ? XpressManaged.TryCompressChunk(uncompressedChunk, uncompressedChunkSize, compressedChunk, out compressedChunkSize) : XpressNative.TryCompressChunk(uncompressedChunk, uncompressedChunkSize, compressedChunk, out compressedChunkSize);
      switch (num)
      {
        case 0:
          return compressedChunkSize >= uncompressedChunkSize ? new uint?() : new uint?(compressedChunkSize);
        case 3221225507:
          return new uint?();
        default:
          throw new Exception(string.Format("RtlCompressBuffer 0x{0:X}", (object) num));
      }
    }

    public static uint DecompressChunk(byte[] compressedChunk, byte[] uncompressedChunk) => ChunkCompression.DecompressChunk(compressedChunk, compressedChunk.Length, uncompressedChunk);

    public static uint DecompressChunk(
      byte[] compressedChunk,
      int compressedCount,
      byte[] uncompressedChunk)
    {
      uint num1;
      uint num2 = !ChunkCompression.UseNative ? XpressManaged.RtlDecompressBufferXpressLz(uncompressedChunk, compressedChunk, (uint) compressedCount, out num1) : XpressNative.DecompressChunk(compressedChunk, (uint) compressedCount, uncompressedChunk, out num1);
      if (num2 != 0U)
        throw new Exception(string.Format("RtlDecompressBuffer 0x{0:X}", (object) num2));
      return num1;
    }
  }
}
