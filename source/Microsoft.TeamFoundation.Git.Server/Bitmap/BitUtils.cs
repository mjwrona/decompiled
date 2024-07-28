// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.BitUtils
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server.Bitmap
{
  internal static class BitUtils
  {
    public const int BitsInUlong = 64;
    public const ulong UlongAllBitsOn = 18446744073709551615;

    public static int BitCount(ulong value)
    {
      value -= value >> 1 & 6148914691236517205UL;
      value = (ulong) (((long) (value >> 2) & 3689348814741910323L) + ((long) value & 3689348814741910323L));
      value = (ulong) ((long) (value >> 4) + (long) value & 1085102592571150095L);
      value *= 72340172838076673UL;
      return (int) (value >> 56);
    }

    public static int LeastSignificantBit(ulong value)
    {
      int num = 0;
      value &= (ulong) -(long) value;
      if (((long) value & -4294967296L) != 0L)
        num |= 32;
      if (((long) value & -281470681808896L) != 0L)
        num |= 16;
      if (((long) value & -71777214294589696L) != 0L)
        num |= 8;
      if (((long) value & -1085102592571150096L) != 0L)
        num |= 4;
      if (((long) value & -3689348814741910324L) != 0L)
        num |= 2;
      if (((long) value & -6148914691236517206L) != 0L)
        num |= 1;
      return num;
    }

    public static bool FillBitmapWithRun(ulong[] bitmap, int offset, int start, int end)
    {
      int index1 = start / 64 - offset;
      if (index1 < 0)
        return false;
      int index2 = end / 64 - offset;
      if (index2 >= bitmap.Length || index2 < index1)
        return false;
      ulong num1 = ulong.MaxValue << start % 64;
      ulong num2 = ulong.MaxValue >> 64 - (end + 1) % 64;
      if (index1 == index2)
      {
        bitmap[index1] |= num1 & num2;
      }
      else
      {
        bitmap[index1] |= num1;
        for (int index3 = index1 + 1; index3 < index2; ++index3)
          bitmap[index3] = ulong.MaxValue;
        bitmap[index2] |= num2;
      }
      return true;
    }

    public static bool RemoveBitsFromRun(ulong[] bitmap, int offset, int start, int end)
    {
      int index1 = start / 64 - offset;
      if (index1 < 0)
        return false;
      int index2 = end / 64 - offset;
      if (index2 >= bitmap.Length || index2 < index1)
        return false;
      ulong num1 = ulong.MaxValue << start % 64;
      ulong num2 = ulong.MaxValue >> 64 - (end + 1) % 64;
      if (index1 == index2)
      {
        bitmap[index1] &= (ulong) ~((long) num1 & (long) num2);
      }
      else
      {
        bitmap[index1] &= ~num1;
        for (int index3 = index1 + 1; index3 < index2; ++index3)
          bitmap[index3] = 0UL;
        bitmap[index2] &= ~num2;
      }
      return true;
    }

    public static int Join(ushort highBits, ushort lowBits) => (int) highBits << 16 | (int) lowBits;

    public static void Split(int value, out ushort highBits, out ushort lowBits)
    {
      highBits = (ushort) (value >> 16);
      lowBits = (ushort) value;
    }
  }
}
