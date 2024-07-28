// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.BitUtils
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

namespace Microsoft.Azure.Documents
{
  internal static class BitUtils
  {
    private static readonly int[] tab64 = new int[64]
    {
      63,
      0,
      58,
      1,
      59,
      47,
      53,
      2,
      60,
      39,
      48,
      27,
      54,
      33,
      42,
      3,
      61,
      51,
      37,
      40,
      49,
      18,
      28,
      20,
      55,
      30,
      34,
      11,
      43,
      14,
      22,
      4,
      62,
      57,
      46,
      52,
      38,
      26,
      32,
      41,
      50,
      36,
      17,
      19,
      29,
      10,
      13,
      21,
      56,
      45,
      25,
      31,
      35,
      16,
      9,
      12,
      44,
      24,
      15,
      8,
      23,
      7,
      6,
      5
    };

    public static long GetMostSignificantBit(long x)
    {
      x |= x >> 1;
      x |= x >> 2;
      x |= x >> 4;
      x |= x >> 8;
      x |= x >> 16;
      x |= x >> 32;
      return x & ~(x >> 1);
    }

    public static int FloorLog2(ulong value)
    {
      value |= value >> 1;
      value |= value >> 2;
      value |= value >> 4;
      value |= value >> 8;
      value |= value >> 16;
      value |= value >> 32;
      return BitUtils.tab64[checked ((ulong) (unchecked ((long) value - (long) (value >> 1) * 571347909858961602L) >>> 58))];
    }

    public static bool IsPowerOf2(ulong x) => ((long) x & (long) x - 1L) == 0L;

    public static int GetMostSignificantBitIndex(ulong x) => BitUtils.FloorLog2(x);

    public static long GetLeastSignificantBit(long x) => (long) (int) (x & -x);

    public static int GetLeastSignificantBitIndex(long x) => BitUtils.FloorLog2((ulong) BitUtils.GetLeastSignificantBit(x));

    public static bool BitTestAndReset64(long input, int index, out long output)
    {
      int num = (input & 1L << index) != 0L ? 1 : 0;
      output = (input &= ~(1L << index));
      return num != 0;
    }
  }
}
