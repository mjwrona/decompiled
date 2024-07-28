// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.MurmurHash3
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using System;

namespace Microsoft.Azure.Documents.Routing
{
  internal static class MurmurHash3
  {
    public static uint Hash32(byte[] bytes, long length, uint seed = 0)
    {
      uint num1 = 3432918353;
      uint num2 = 461845907;
      uint num3 = seed;
      for (int startIndex = 0; (long) startIndex < length - 3L; startIndex += 4)
      {
        uint num4 = MurmurHash3.RotateLeft32(BitConverter.ToUInt32(bytes, startIndex) * num1, 15) * num2;
        num3 = (uint) ((int) MurmurHash3.RotateLeft32(num3 ^ num4, 13) * 5 - 430675100);
      }
      uint num5 = 0;
      switch ((length & 3L) - 1L)
      {
        case 0:
          num5 ^= (uint) bytes[length - 1L];
          break;
        case 1:
          num5 = num5 ^ (uint) bytes[length - 1L] << 8 ^ (uint) bytes[length - 2L];
          break;
        case 2:
          num5 = num5 ^ (uint) bytes[length - 1L] << 16 ^ (uint) bytes[length - 2L] << 8 ^ (uint) bytes[length - 3L];
          break;
      }
      uint num6 = MurmurHash3.RotateLeft32(num5 * num1, 15) * num2;
      uint num7 = num3 ^ num6 ^ (uint) length;
      uint num8 = (num7 ^ num7 >> 16) * 2246822507U;
      uint num9 = (num8 ^ num8 >> 13) * 3266489909U;
      return num9 ^ num9 >> 16;
    }

    public static ulong Hash64(byte[] bytes, int length, ulong seed = 0)
    {
      int num1 = length / 8;
      ulong num2 = seed;
      int startIndex;
      for (startIndex = 0; startIndex < length - 7; startIndex += 8)
      {
        ulong num3 = MurmurHash3.RotateLeft64(BitConverter.ToUInt64(bytes, startIndex) * 9782798678568883157UL, 31) * 5545529020109919103UL;
        num2 = MurmurHash3.RotateLeft64(num2 ^ num3, 27) * 5UL + 1390208809UL;
      }
      ulong num4 = 0;
      switch (length & 7)
      {
        case 1:
          num4 ^= (ulong) bytes[startIndex];
          break;
        case 2:
          num4 ^= (ulong) bytes[startIndex + 1] << 8;
          break;
        case 3:
          num4 ^= (ulong) bytes[startIndex + 2] << 16;
          break;
        case 4:
          num4 ^= (ulong) bytes[startIndex + 3] << 24;
          break;
        case 5:
          num4 ^= (ulong) bytes[startIndex + 4] << 32;
          break;
        case 6:
          num4 ^= (ulong) bytes[startIndex + 5] << 40;
          break;
        case 7:
          num4 ^= (ulong) bytes[startIndex + 6] << 48;
          break;
      }
      ulong num5 = MurmurHash3.RotateLeft64(num4 * 9782798678568883157UL, 31) * 5545529020109919103UL;
      ulong num6 = num2 ^ num5 ^ (ulong) length;
      ulong num7 = (num6 ^ num6 >> 33) * 18397679294719823053UL;
      ulong num8 = (num7 ^ num7 >> 33) * 14181476777654086739UL;
      return num8 ^ num8 >> 33;
    }

    public static UInt128 Hash128(byte[] bytes, int length, UInt128 seed)
    {
      ulong num1 = seed.GetLow();
      ulong num2 = seed.GetHigh();
      int startIndex;
      for (startIndex = 0; startIndex < length - 15; startIndex += 16)
      {
        ulong uint64_1 = BitConverter.ToUInt64(bytes, startIndex);
        ulong uint64_2 = BitConverter.ToUInt64(bytes, startIndex + 8);
        ulong num3 = MurmurHash3.RotateLeft64(uint64_1 * 9782798678568883157UL, 31) * 5545529020109919103UL;
        num1 = (MurmurHash3.RotateLeft64(num1 ^ num3, 27) + num2) * 5UL + 1390208809UL;
        ulong num4 = MurmurHash3.RotateLeft64(uint64_2 * 5545529020109919103UL, 33) * 9782798678568883157UL;
        num2 = (MurmurHash3.RotateLeft64(num2 ^ num4, 31) + num1) * 5UL + 944331445UL;
      }
      ulong num5 = 0;
      ulong num6 = 0;
      int num7 = length & 15;
      if (num7 >= 15)
        num6 ^= (ulong) bytes[startIndex + 14] << 48;
      if (num7 >= 14)
        num6 ^= (ulong) bytes[startIndex + 13] << 40;
      if (num7 >= 13)
        num6 ^= (ulong) bytes[startIndex + 12] << 32;
      if (num7 >= 12)
        num6 ^= (ulong) bytes[startIndex + 11] << 24;
      if (num7 >= 11)
        num6 ^= (ulong) bytes[startIndex + 10] << 16;
      if (num7 >= 10)
        num6 ^= (ulong) bytes[startIndex + 9] << 8;
      if (num7 >= 9)
        num6 ^= (ulong) bytes[startIndex + 8];
      ulong num8 = MurmurHash3.RotateLeft64(num6 * 5545529020109919103UL, 33) * 9782798678568883157UL;
      ulong num9 = num2 ^ num8;
      if (num7 >= 8)
        num5 ^= (ulong) bytes[startIndex + 7] << 56;
      if (num7 >= 7)
        num5 ^= (ulong) bytes[startIndex + 6] << 48;
      if (num7 >= 6)
        num5 ^= (ulong) bytes[startIndex + 5] << 40;
      if (num7 >= 5)
        num5 ^= (ulong) bytes[startIndex + 4] << 32;
      if (num7 >= 4)
        num5 ^= (ulong) bytes[startIndex + 3] << 24;
      if (num7 >= 3)
        num5 ^= (ulong) bytes[startIndex + 2] << 16;
      if (num7 >= 2)
        num5 ^= (ulong) bytes[startIndex + 1] << 8;
      if (num7 >= 1)
        num5 ^= (ulong) bytes[startIndex];
      ulong num10 = MurmurHash3.RotateLeft64(num5 * 9782798678568883157UL, 31) * 5545529020109919103UL;
      ulong num11 = num1 ^ num10 ^ (ulong) length;
      ulong num12 = num9 ^ (ulong) length;
      ulong num13 = num11 + num12;
      ulong num14 = num12 + num13;
      ulong num15 = (num13 ^ num13 >> 33) * 18397679294719823053UL;
      ulong num16 = (num15 ^ num15 >> 33) * 14181476777654086739UL;
      ulong num17 = num16 ^ num16 >> 33;
      ulong num18 = (num14 ^ num14 >> 33) * 18397679294719823053UL;
      ulong num19 = (num18 ^ num18 >> 33) * 14181476777654086739UL;
      ulong num20 = num19 ^ num19 >> 33;
      ulong low = num17 + num20;
      ulong high = num20 + low;
      if (!BitConverter.IsLittleEndian)
      {
        low = MurmurHash3.Reverse(low);
        high = MurmurHash3.Reverse(high);
      }
      return UInt128.Create(low, high);
    }

    public static ulong Reverse(ulong value) => (ulong) (((long) value & (long) byte.MaxValue) << 56 | (long) (value >> 8 & (ulong) byte.MaxValue) << 48 | (long) (value >> 16 & (ulong) byte.MaxValue) << 40 | (long) (value >> 24 & (ulong) byte.MaxValue) << 32 | (long) (value >> 32 & (ulong) byte.MaxValue) << 24 | (long) (value >> 40 & (ulong) byte.MaxValue) << 16 | (long) (value >> 48 & (ulong) byte.MaxValue) << 8) | value >> 56 & (ulong) byte.MaxValue;

    private static uint RotateLeft32(uint n, int numBits) => n << numBits | n >> 32 - numBits;

    private static ulong RotateLeft64(ulong n, int numBits) => n << numBits | n >> 64 - numBits;
  }
}
