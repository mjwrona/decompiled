// Decompiled with JetBrains decompiler
// Type: HdrHistogram.ZigZagEncoding
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using HdrHistogram.Utilities;

namespace HdrHistogram
{
  internal static class ZigZagEncoding
  {
    public static void PutLong(ByteBuffer buffer, long value)
    {
      value = value << 1 ^ value >> 63;
      if (value >> 7 == 0L)
      {
        buffer.Put((byte) value);
      }
      else
      {
        buffer.Put((byte) ((ulong) (value & (long) sbyte.MaxValue) | 128UL));
        if (value >> 14 == 0L)
        {
          buffer.Put((byte) (value >> 7));
        }
        else
        {
          buffer.Put((byte) ((ulong) (value >> 7) | 128UL));
          if (value >> 21 == 0L)
          {
            buffer.Put((byte) (value >> 14));
          }
          else
          {
            buffer.Put((byte) ((ulong) (value >> 14) | 128UL));
            if (value >> 28 == 0L)
            {
              buffer.Put((byte) (value >> 21));
            }
            else
            {
              buffer.Put((byte) ((ulong) (value >> 21) | 128UL));
              if (value >> 35 == 0L)
              {
                buffer.Put((byte) (value >> 28));
              }
              else
              {
                buffer.Put((byte) ((ulong) (value >> 28) | 128UL));
                if (value >> 42 == 0L)
                {
                  buffer.Put((byte) (value >> 35));
                }
                else
                {
                  buffer.Put((byte) ((ulong) (value >> 35) | 128UL));
                  if (value >> 49 == 0L)
                  {
                    buffer.Put((byte) (value >> 42));
                  }
                  else
                  {
                    buffer.Put((byte) ((ulong) (value >> 42) | 128UL));
                    if (value >> 56 == 0L)
                    {
                      buffer.Put((byte) (value >> 49));
                    }
                    else
                    {
                      buffer.Put((byte) ((ulong) (value >> 49) | 128UL));
                      buffer.Put((byte) (value >> 56));
                    }
                  }
                }
              }
            }
          }
        }
      }
    }

    public static long GetLong(ByteBuffer buffer)
    {
      long num1 = (long) buffer.Get();
      long num2 = num1 & (long) sbyte.MaxValue;
      if ((num1 & 128L) != 0L)
      {
        long num3 = (long) buffer.Get();
        num2 |= (num3 & (long) sbyte.MaxValue) << 7;
        if ((num3 & 128L) != 0L)
        {
          long num4 = (long) buffer.Get();
          num2 |= (num4 & (long) sbyte.MaxValue) << 14;
          if ((num4 & 128L) != 0L)
          {
            long num5 = (long) buffer.Get();
            num2 |= (num5 & (long) sbyte.MaxValue) << 21;
            if ((num5 & 128L) != 0L)
            {
              long num6 = (long) buffer.Get();
              num2 |= (num6 & (long) sbyte.MaxValue) << 28;
              if ((num6 & 128L) != 0L)
              {
                long num7 = (long) buffer.Get();
                num2 |= (num7 & (long) sbyte.MaxValue) << 35;
                if ((num7 & 128L) != 0L)
                {
                  long num8 = (long) buffer.Get();
                  num2 |= (num8 & (long) sbyte.MaxValue) << 42;
                  if ((num8 & 128L) != 0L)
                  {
                    long num9 = (long) buffer.Get();
                    num2 |= (num9 & (long) sbyte.MaxValue) << 49;
                    if ((num9 & 128L) != 0L)
                    {
                      long num10 = (long) buffer.Get();
                      num2 |= num10 << 56;
                    }
                  }
                }
              }
            }
          }
        }
      }
      return num2 >>> 1 ^ -(num2 & 1L);
    }
  }
}
