// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.CompressionHelper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public class CompressionHelper
  {
    public static byte[] DeflateByteArray(byte[] uncompressedBytes, bool withZLibHeader = false) => CompressionHelper.DeflateByteArray(uncompressedBytes, 0, uncompressedBytes.Length, withZLibHeader);

    public static byte[] DeflateByteArray(ArraySegment<byte> arraySegment, bool withZLibHeader = false) => CompressionHelper.DeflateByteArray(arraySegment.Array, arraySegment.Offset, arraySegment.Count, withZLibHeader);

    public static byte[] DeflateByteArray(
      byte[] uncompressedBytes,
      int offset,
      int count,
      bool withZLibHeader = false)
    {
      using (MemoryStream baseOutputStream = new MemoryStream())
      {
        using (DeflaterOutputStream destination = new DeflaterOutputStream((Stream) baseOutputStream, new Deflater(-1, !withZLibHeader)))
        {
          using (MemoryStream memoryStream = new MemoryStream(uncompressedBytes, offset, count, false))
          {
            memoryStream.CopyTo((Stream) destination);
            destination.Finish();
            return baseOutputStream.ToArray();
          }
        }
      }
    }

    public static byte[] InflateByteArray(byte[] uncompressedBytes, bool withZLibHeader = false) => CompressionHelper.InflateByteArray(uncompressedBytes, 0, uncompressedBytes.Length, withZLibHeader);

    public static byte[] InflateByteArray(ArraySegment<byte> arraySegment, bool withZLibHeader = false) => CompressionHelper.InflateByteArray(arraySegment.Array, arraySegment.Offset, arraySegment.Count, withZLibHeader);

    public static byte[] InflateByteArray(
      byte[] compressedBytes,
      int offset,
      int count,
      bool withZLibHeader = false)
    {
      using (MemoryStream baseInputStream = new MemoryStream(compressedBytes, offset, count, false))
      {
        using (MemoryStream destination = new MemoryStream())
        {
          using (InflaterInputStream inflaterInputStream = new InflaterInputStream((Stream) baseInputStream, new Inflater(!withZLibHeader)))
          {
            inflaterInputStream.IsStreamOwner = false;
            inflaterInputStream.CopyTo((Stream) destination);
          }
          return destination.ToArray();
        }
      }
    }
  }
}
