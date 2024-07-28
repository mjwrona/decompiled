// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.MurmurHash3
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Azure.Cosmos
{
  internal static class MurmurHash3
  {
    public static unsafe uint Hash32(string value, uint seed)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      int maxByteCount = Encoding.UTF8.GetMaxByteCount(value.Length);
      Span<byte> span;
      if (maxByteCount <= 256)
      {
        int length = maxByteCount;
        // ISSUE: untyped stack allocation
        span = new Span<byte>((void*) __untypedstackalloc((IntPtr) (uint) length), length);
      }
      else
        span = (Span<byte>) new byte[maxByteCount];
      Span<byte> dest = span;
      int bytes = Encoding.UTF8.GetBytes(value, dest);
      return MurmurHash3.Hash32((ReadOnlySpan<byte>) dest.Slice(0, bytes), seed);
    }

    public static uint Hash32(bool value, uint seed) => MurmurHash3.Hash32<byte>(value ? (byte) 1 : (byte) 0, seed);

    public static unsafe uint Hash32<T>(T value, uint seed) where T : unmanaged => MurmurHash3.Hash32(MemoryMarshal.AsBytes<T>(new ReadOnlySpan<T>((void*) &value, 1)), seed);

    public static unsafe uint Hash32(ReadOnlySpan<byte> span, uint seed)
    {
      if (!BitConverter.IsLittleEndian)
        throw new InvalidOperationException("Host machine needs to be little endian.");
      uint num1 = seed;
      uint num2;
      fixed (byte* numPtr = &span.GetPinnableReference())
      {
        for (int index = 0; index < span.Length - 3; index += 4)
        {
          uint num3 = MurmurHash3.RotateLeft32(*(uint*) (numPtr + index) * 3432918353U, 15) * 461845907U;
          num1 = (uint) ((int) MurmurHash3.RotateLeft32(num1 ^ num3, 13) * 5 - 430675100);
        }
        uint num4 = 0;
        switch (span.Length & 3)
        {
          case 1:
            num4 ^= (uint) numPtr[span.Length - 1];
            break;
          case 2:
            num4 = num4 ^ (uint) numPtr[span.Length - 1] << 8 ^ (uint) numPtr[span.Length - 2];
            break;
          case 3:
            num4 = num4 ^ (uint) numPtr[span.Length - 1] << 16 ^ (uint) numPtr[span.Length - 2] << 8 ^ (uint) numPtr[span.Length - 3];
            break;
        }
        uint num5 = MurmurHash3.RotateLeft32(num4 * 3432918353U, 15) * 461845907U;
        num2 = num1 ^ num5;
      }
      uint num6 = num2 ^ (uint) span.Length;
      uint num7 = (num6 ^ num6 >> 16) * 2246822507U;
      uint num8 = (num7 ^ num7 >> 13) * 3266489909U;
      return num8 ^ num8 >> 16;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint RotateLeft32(uint n, int numBits)
    {
      if (numBits >= 32)
        throw new ArgumentOutOfRangeException("numBits must be less than 32");
      return n << numBits | n >> 32 - numBits;
    }

    public static unsafe UInt128 Hash128(string value, UInt128 seed)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      int maxByteCount = Encoding.UTF8.GetMaxByteCount(value.Length);
      Span<byte> span;
      if (maxByteCount <= 256)
      {
        int length = maxByteCount;
        // ISSUE: untyped stack allocation
        span = new Span<byte>((void*) __untypedstackalloc((IntPtr) (uint) length), length);
      }
      else
        span = (Span<byte>) new byte[maxByteCount];
      Span<byte> dest = span;
      int bytes = Encoding.UTF8.GetBytes(value, dest);
      return MurmurHash3.Hash128((ReadOnlySpan<byte>) dest.Slice(0, bytes), seed);
    }

    public static UInt128 Hash128(bool value, UInt128 seed) => MurmurHash3.Hash128<byte>(value ? (byte) 1 : (byte) 0, seed);

    public static unsafe UInt128 Hash128<T>(T value, UInt128 seed) where T : unmanaged => MurmurHash3.Hash128(MemoryMarshal.AsBytes<T>(new ReadOnlySpan<T>((void*) &value, 1)), seed);

    public static unsafe UInt128 Hash128(ReadOnlySpan<byte> span, UInt128 seed)
    {
      if (!BitConverter.IsLittleEndian)
        throw new InvalidOperationException("Host machine needs to be little endian.");
      ulong low1 = seed.GetLow();
      long high1 = (long) seed.GetHigh();
      ulong num1 = low1;
      ulong num2 = (ulong) high1;
      ulong num3;
      ulong num4;
      fixed (byte* numPtr = &span.GetPinnableReference())
      {
        int index;
        for (index = 0; index < span.Length - 15; index += 16)
        {
          ulong num5 = (ulong) *(long*) (numPtr + index);
          ulong num6 = (ulong) *(long*) (numPtr + index + 8);
          ulong num7 = MurmurHash3.RotateLeft64(num5 * 9782798678568883157UL, 31) * 5545529020109919103UL;
          num1 = (MurmurHash3.RotateLeft64(num1 ^ num7, 27) + num2) * 5UL + 1390208809UL;
          ulong num8 = MurmurHash3.RotateLeft64(num6 * 5545529020109919103UL, 33) * 9782798678568883157UL;
          num2 = (MurmurHash3.RotateLeft64(num2 ^ num8, 31) + num1) * 5UL + 944331445UL;
        }
        ulong num9 = 0;
        ulong num10 = 0;
        int num11 = span.Length & 15;
        if (num11 >= 15)
          num10 ^= (ulong) numPtr[index + 14] << 48;
        if (num11 >= 14)
          num10 ^= (ulong) numPtr[index + 13] << 40;
        if (num11 >= 13)
          num10 ^= (ulong) numPtr[index + 12] << 32;
        if (num11 >= 12)
          num10 ^= (ulong) numPtr[index + 11] << 24;
        if (num11 >= 11)
          num10 ^= (ulong) numPtr[index + 10] << 16;
        if (num11 >= 10)
          num10 ^= (ulong) numPtr[index + 9] << 8;
        if (num11 >= 9)
          num10 ^= (ulong) numPtr[index + 8];
        ulong num12 = MurmurHash3.RotateLeft64(num10 * 5545529020109919103UL, 33) * 9782798678568883157UL;
        num3 = num2 ^ num12;
        if (num11 >= 8)
          num9 ^= (ulong) numPtr[index + 7] << 56;
        if (num11 >= 7)
          num9 ^= (ulong) numPtr[index + 6] << 48;
        if (num11 >= 6)
          num9 ^= (ulong) numPtr[index + 5] << 40;
        if (num11 >= 5)
          num9 ^= (ulong) numPtr[index + 4] << 32;
        if (num11 >= 4)
          num9 ^= (ulong) numPtr[index + 3] << 24;
        if (num11 >= 3)
          num9 ^= (ulong) numPtr[index + 2] << 16;
        if (num11 >= 2)
          num9 ^= (ulong) numPtr[index + 1] << 8;
        if (num11 >= 1)
          num9 ^= (ulong) numPtr[index];
        ulong num13 = MurmurHash3.RotateLeft64(num9 * 9782798678568883157UL, 31) * 5545529020109919103UL;
        num4 = num1 ^ num13;
      }
      ulong num14 = num4 ^ (ulong) span.Length;
      ulong num15 = num3 ^ (ulong) span.Length;
      ulong h1 = num14 + num15;
      ulong h2 = num15 + h1;
      ulong num16 = MurmurHash3.Mix(h1);
      ulong num17 = MurmurHash3.Mix(h2);
      ulong low2 = num16 + num17;
      ulong high2 = num17 + low2;
      return UInt128.Create(low2, high2);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong Mix(ulong h)
    {
      h ^= h >> 33;
      h *= 18397679294719823053UL;
      h ^= h >> 33;
      h *= 14181476777654086739UL;
      h ^= h >> 33;
      return h;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static ulong RotateLeft64(ulong n, int numBits)
    {
      if (numBits >= 64)
        throw new ArgumentOutOfRangeException("numBits must be less than 64");
      return n << numBits | n >> 64 - numBits;
    }
  }
}
