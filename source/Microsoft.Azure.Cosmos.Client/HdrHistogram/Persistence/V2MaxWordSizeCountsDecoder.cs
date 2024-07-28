// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Persistence.V2MaxWordSizeCountsDecoder
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using HdrHistogram.Utilities;
using System;

namespace HdrHistogram.Persistence
{
  internal sealed class V2MaxWordSizeCountsDecoder : ICountsDecoder
  {
    public int WordSize => 9;

    public int ReadCounts(
      ByteBuffer sourceBuffer,
      int lengthInBytes,
      int maxIndex,
      Action<int, long> setCount)
    {
      int num1 = 0;
      int num2 = sourceBuffer.Position + lengthInBytes;
      while (sourceBuffer.Position < num2 && num1 < maxIndex)
      {
        long num3 = ZigZagEncoding.GetLong(sourceBuffer);
        if (num3 < 0L)
        {
          long num4 = -num3;
          if (num4 > (long) int.MaxValue)
            throw new ArgumentException("An encoded zero count of > int.MaxValue was encountered in the source");
          num1 += (int) num4;
        }
        else
          setCount(num1++, num3);
      }
      return num1;
    }
  }
}
