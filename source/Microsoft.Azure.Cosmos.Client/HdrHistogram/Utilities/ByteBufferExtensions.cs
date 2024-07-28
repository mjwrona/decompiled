// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Utilities.ByteBufferExtensions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.IO;
using System.IO.Compression;

namespace HdrHistogram.Utilities
{
  internal static class ByteBufferExtensions
  {
    public static int CompressedCopy(this ByteBuffer target, ByteBuffer source, int targetOffset)
    {
      byte[] src;
      using (MemoryStream input = new MemoryStream(source.ToArray()))
        src = ByteBufferExtensions.Compress((Stream) input);
      target.BlockCopy((Array) src, 0, targetOffset, src.Length);
      return src.Length;
    }

    private static byte[] Compress(Stream input)
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        memoryStream.WriteByte((byte) 88);
        memoryStream.WriteByte((byte) 133);
        using (DeflateStream destination = new DeflateStream((Stream) memoryStream, CompressionMode.Compress, true))
          input.CopyTo((Stream) destination);
        return memoryStream.ToArray();
      }
    }
  }
}
