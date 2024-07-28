// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Compression.Compressor
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Compression
{
  public static class Compressor
  {
    private const int ErrorInsufficientBuffer = 122;
    private static readonly byte[] s_emptyArray = Array.Empty<byte>();
    private static readonly ArraySegment<byte> s_emptySegment = new ArraySegment<byte>(Compressor.s_emptyArray);

    public static ArraySegment<byte> Compress(
      CompressorAlgorithm algorithm,
      byte[] uncompressedData)
    {
      return Compressor.Compress(algorithm, uncompressedData, 0, uncompressedData.Length);
    }

    public static unsafe ArraySegment<byte> Compress(
      CompressorAlgorithm algorithm,
      byte[] uncompressedData,
      int offset,
      int count)
    {
      if (uncompressedData.Length == 0)
        return Compressor.s_emptySegment;
      IntPtr handle = IntPtr.Zero;
      try
      {
        if (!NativeMethods.CreateCompressor(algorithm, IntPtr.Zero, out handle))
          throw new Win32Exception();
        fixed (byte* numPtr1 = &uncompressedData[offset])
        {
          IntPtr compressedDataSize1;
          if (!NativeMethods.Compress(handle, new IntPtr((void*) numPtr1), new IntPtr(count), IntPtr.Zero, IntPtr.Zero, out compressedDataSize1))
          {
            int lastWin32Error = Marshal.GetLastWin32Error();
            if (lastWin32Error != 122)
              throw new Win32Exception(lastWin32Error);
          }
          byte[] array = new byte[compressedDataSize1.ToInt64()];
          fixed (byte* numPtr2 = &array[0])
          {
            IntPtr compressedDataSize2;
            if (!NativeMethods.Compress(handle, new IntPtr((void*) numPtr1), new IntPtr(count), new IntPtr((void*) numPtr2), compressedDataSize1, out compressedDataSize2))
              throw new Win32Exception();
            return new ArraySegment<byte>(array, 0, compressedDataSize2.ToInt32());
          }
        }
      }
      finally
      {
        if (handle != IntPtr.Zero)
          NativeMethods.CloseCompressor(handle);
      }
    }

    public static byte[] Decompress(
      CompressorAlgorithm algorithm,
      ArraySegment<byte> compressedData)
    {
      return Compressor.Decompress(algorithm, compressedData.Array, compressedData.Offset);
    }

    public static byte[] Decompress(CompressorAlgorithm algorithm, byte[] compressedData) => Compressor.Decompress(algorithm, compressedData, 0);

    [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "offset")]
    public static unsafe byte[] Decompress(
      CompressorAlgorithm algorithm,
      byte[] compressedData,
      int offset)
    {
      if (compressedData.Length == 0)
        return Compressor.s_emptyArray;
      IntPtr handle = IntPtr.Zero;
      try
      {
        if (!NativeMethods.CreateDecompressor(algorithm, IntPtr.Zero, out handle))
          throw new Win32Exception();
        fixed (byte* numPtr1 = &compressedData[0])
        {
          IntPtr uncompressedDataSize;
          if (!NativeMethods.Decompress(handle, new IntPtr((void*) numPtr1), new IntPtr((long) compressedData.Length), IntPtr.Zero, IntPtr.Zero, out uncompressedDataSize))
          {
            int lastWin32Error = Marshal.GetLastWin32Error();
            if (lastWin32Error != 122)
              throw new Win32Exception(lastWin32Error);
          }
          byte[] numArray = new byte[uncompressedDataSize.ToInt64()];
          fixed (byte* numPtr2 = &numArray[0])
          {
            if (!NativeMethods.Decompress(handle, new IntPtr((void*) numPtr1), new IntPtr((long) compressedData.Length), new IntPtr((void*) numPtr2), uncompressedDataSize, out IntPtr _))
              throw new Win32Exception();
            return numArray;
          }
        }
      }
      finally
      {
        if (handle != IntPtr.Zero)
          NativeMethods.CloseDecompressor(handle);
      }
    }
  }
}
