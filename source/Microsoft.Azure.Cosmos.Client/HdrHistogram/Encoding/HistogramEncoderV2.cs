// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Encoding.HistogramEncoderV2
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using HdrHistogram.Utilities;
using System;

namespace HdrHistogram.Encoding
{
  internal class HistogramEncoderV2 : IEncoder
  {
    public static readonly HistogramEncoderV2 Instance = new HistogramEncoderV2();

    public int Encode(IRecordedData data, ByteBuffer buffer)
    {
      int position1 = buffer.Position;
      buffer.PutInt(data.Cookie);
      int position2 = buffer.Position;
      buffer.PutInt(0);
      buffer.PutInt(data.NormalizingIndexOffset);
      buffer.PutInt(data.NumberOfSignificantValueDigits);
      buffer.PutLong(data.LowestDiscernibleValue);
      buffer.PutLong(data.HighestTrackableValue);
      buffer.PutDouble(data.IntegerToDoubleValueConversionRatio);
      int num = HistogramEncoderV2.FillBufferFromCountsArray(buffer, data);
      buffer.PutInt(position2, num);
      return buffer.Position - position1;
    }

    private static int FillBufferFromCountsArray(ByteBuffer buffer, IRecordedData data)
    {
      int position = buffer.Position;
      int idx = 0;
      while (idx < data.Counts.Length)
      {
        long countAtIndex = HistogramEncoderV2.GetCountAtIndex(idx++, data);
        if (countAtIndex < 0L)
          throw new InvalidOperationException(string.Format("Cannot encode histogram containing negative counts ({0}) at index {1}", (object) countAtIndex, (object) idx));
        long num = 0;
        if (countAtIndex == 0L)
        {
          num = 1L;
          for (; idx < data.Counts.Length && HistogramEncoderV2.GetCountAtIndex(idx, data) == 0L; ++idx)
            ++num;
        }
        if (num > 1L)
          ZigZagEncoding.PutLong(buffer, -num);
        else
          ZigZagEncoding.PutLong(buffer, countAtIndex);
      }
      return buffer.Position - position;
    }

    private static long GetCountAtIndex(int idx, IRecordedData data) => data.Counts[idx];
  }
}
