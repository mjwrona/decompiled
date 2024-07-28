// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serialization.HybridRow.Internal.MurmurHash3
// Assembly: Microsoft.Azure.Cosmos.Serialization.HybridRow, Version=1.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 77F52C47-A4AE-4843-8DF5-462472B35FB8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Serialization.HybridRow.dll

using Microsoft.Azure.Cosmos.Core;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Azure.Cosmos.Serialization.HybridRow.Internal
{
  internal static class MurmurHash3
  {
    public static unsafe (ulong low, ulong high) Hash128(string value, (ulong high, ulong low) seed)
    {
      Contract.Requires(value != null);
      int maxByteCount = Encoding.UTF8.GetMaxByteCount(value.Length);
      Span<byte> span1;
      if (maxByteCount <= 256)
      {
        int length = maxByteCount;
        // ISSUE: untyped stack allocation
        span1 = new Span<byte>((void*) __untypedstackalloc((IntPtr) (uint) length), length);
      }
      else
        span1 = (Span<byte>) new byte[maxByteCount];
      Span<byte> span2 = span1;
      int bytes = SpanExtensions.GetBytes(Encoding.UTF8, value.AsSpan(), span2);
      return MurmurHash3.Hash128((ReadOnlySpan<byte>) span2.Slice(0, bytes), seed);
    }

    public static (ulong low, ulong high) Hash128(bool value, (ulong high, ulong low) seed) => MurmurHash3.Hash128<byte>(value ? (byte) 1 : (byte) 0, seed);

    public static unsafe (ulong low, ulong high) Hash128<T>(T value, (ulong high, ulong low) seed) where T : unmanaged => MurmurHash3.Hash128(MemoryMarshal.AsBytes<T>(new ReadOnlySpan<T>((void*) &value, 1)), seed);

    public static unsafe (ulong low, ulong high) Hash128(
      ReadOnlySpan<byte> span,
      (ulong high, ulong low) seed)
    {
      (ulong high, ulong low) = seed;
      ulong num1;
      ulong num2;
      fixed (byte* numPtr = &span.GetPinnableReference())
      {
        int index;
        for (index = 0; index < span.Length - 15; index += 16)
        {
          ulong num3 = (ulong) *(long*) (numPtr + index);
          ulong num4 = (ulong) *(long*) (numPtr + index + 8);
          ulong num5 = MurmurHash3.RotateLeft64(num3 * 9782798678568883157UL, 31) * 5545529020109919103UL;
          high = (MurmurHash3.RotateLeft64(high ^ num5, 27) + low) * 5UL + 1390208809UL;
          ulong num6 = MurmurHash3.RotateLeft64(num4 * 5545529020109919103UL, 33) * 9782798678568883157UL;
          low = (MurmurHash3.RotateLeft64(low ^ num6, 31) + high) * 5UL + 944331445UL;
        }
        ulong num7 = 0;
        ulong num8 = 0;
        int num9 = span.Length & 15;
        if (num9 >= 15)
          num8 ^= (ulong) numPtr[index + 14] << 48;
        if (num9 >= 14)
          num8 ^= (ulong) numPtr[index + 13] << 40;
        if (num9 >= 13)
          num8 ^= (ulong) numPtr[index + 12] << 32;
        if (num9 >= 12)
          num8 ^= (ulong) numPtr[index + 11] << 24;
        if (num9 >= 11)
          num8 ^= (ulong) numPtr[index + 10] << 16;
        if (num9 >= 10)
          num8 ^= (ulong) numPtr[index + 9] << 8;
        if (num9 >= 9)
          num8 ^= (ulong) numPtr[index + 8];
        ulong num10 = MurmurHash3.RotateLeft64(num8 * 5545529020109919103UL, 33) * 9782798678568883157UL;
        num1 = low ^ num10;
        if (num9 >= 8)
          num7 ^= (ulong) numPtr[index + 7] << 56;
        if (num9 >= 7)
          num7 ^= (ulong) numPtr[index + 6] << 48;
        if (num9 >= 6)
          num7 ^= (ulong) numPtr[index + 5] << 40;
        if (num9 >= 5)
          num7 ^= (ulong) numPtr[index + 4] << 32;
        if (num9 >= 4)
          num7 ^= (ulong) numPtr[index + 3] << 24;
        if (num9 >= 3)
          num7 ^= (ulong) numPtr[index + 2] << 16;
        if (num9 >= 2)
          num7 ^= (ulong) numPtr[index + 1] << 8;
        if (num9 >= 1)
          num7 ^= (ulong) numPtr[index];
        ulong num11 = MurmurHash3.RotateLeft64(num7 * 9782798678568883157UL, 31) * 5545529020109919103UL;
        num2 = high ^ num11;
      }
      ulong num12 = num2 ^ (ulong) span.Length;
      ulong num13 = num1 ^ (ulong) span.Length;
      ulong h1 = num12 + num13;
      ulong h2 = num13 + h1;
      ulong num14 = MurmurHash3.Mix(h1);
      ulong num15 = MurmurHash3.Mix(h2);
      ulong num16 = num14 + num15;
      ulong num17 = num15 + num16;
      return (num16, num17);
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
    private static ulong RotateLeft64(ulong n, int numBits) => n << numBits | n >> checked (64 - numBits);
  }
}
