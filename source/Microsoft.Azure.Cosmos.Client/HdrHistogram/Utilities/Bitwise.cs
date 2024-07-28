// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Utilities.Bitwise
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace HdrHistogram.Utilities
{
  internal static class Bitwise
  {
    private static readonly int[] Lookup = new int[256];

    static Bitwise()
    {
      for (int d = 1; d < 256; ++d)
        Bitwise.Lookup[d] = (int) (Math.Log((double) d) / Math.Log(2.0));
    }

    public static int NumberOfLeadingZeros(long value) => value < (long) int.MaxValue ? 63 - Bitwise.Log2((int) value) : Bitwise.NumberOfLeadingZerosLong(value);

    private static int NumberOfLeadingZerosLong(long value)
    {
      int num1 = 1;
      uint num2 = (uint) (value >> 32);
      if (num2 == 0U)
      {
        num1 += 32;
        num2 = (uint) value;
      }
      if (num2 >> 16 == 0U)
      {
        num1 += 16;
        num2 <<= 16;
      }
      if (num2 >> 24 == 0U)
      {
        num1 += 8;
        num2 <<= 8;
      }
      if (num2 >> 28 == 0U)
      {
        num1 += 4;
        num2 <<= 4;
      }
      if (num2 >> 30 == 0U)
      {
        num1 += 2;
        num2 <<= 2;
      }
      return num1 - (int) (num2 >> 31);
    }

    private static int Log2(int i)
    {
      if (i >= 16777216)
        return Bitwise.Lookup[i >> 24] + 24;
      if (i >= 65536)
        return Bitwise.Lookup[i >> 16] + 16;
      return i >= 256 ? Bitwise.Lookup[i >> 8] + 8 : Bitwise.Lookup[i];
    }
  }
}
