// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Compression.NativeMethods
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using System;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Compression
{
  internal class NativeMethods
  {
    [DllImport("cabinet.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool CloseCompressor(IntPtr handle);

    [DllImport("cabinet.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool CloseDecompressor(IntPtr handle);

    [DllImport("cabinet.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool Compress(
      IntPtr handle,
      IntPtr uncompressData,
      IntPtr uncompressedDataSize,
      IntPtr compressedBuffer,
      IntPtr compressedbufferSize,
      out IntPtr compressedDataSize);

    [DllImport("cabinet.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool CreateCompressor(
      CompressorAlgorithm algorithm,
      IntPtr allocationRoutines,
      out IntPtr handle);

    [DllImport("cabinet.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool CreateDecompressor(
      CompressorAlgorithm algorithm,
      IntPtr allocationRoutines,
      out IntPtr handle);

    [DllImport("cabinet.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool Decompress(
      IntPtr handle,
      IntPtr compressedData,
      IntPtr compressedDataSize,
      IntPtr uncompressedBuffer,
      IntPtr uncompressedbufferSize,
      out IntPtr uncompressedDataSize);
  }
}
