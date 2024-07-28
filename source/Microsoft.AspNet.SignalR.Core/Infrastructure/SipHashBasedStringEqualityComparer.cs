// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Infrastructure.SipHashBasedStringEqualityComparer
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Microsoft.AspNet.SignalR.Infrastructure
{
  internal sealed class SipHashBasedStringEqualityComparer : IEqualityComparer<string>
  {
    private static readonly RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();
    private readonly ulong _k0;
    private readonly ulong _k1;

    public SipHashBasedStringEqualityComparer()
      : this(SipHashBasedStringEqualityComparer.GenerateRandomKeySegment(), SipHashBasedStringEqualityComparer.GenerateRandomKeySegment())
    {
    }

    internal SipHashBasedStringEqualityComparer(ulong k0, ulong k1)
    {
      this._k0 = k0;
      this._k1 = k1;
    }

    public bool Equals(string x, string y) => string.Equals(x, y);

    private static ulong GenerateRandomKeySegment()
    {
      byte[] data = new byte[8];
      SipHashBasedStringEqualityComparer._rng.GetBytes(data);
      return (ulong) BitConverter.ToInt64(data, 0);
    }

    public unsafe int GetHashCode(string obj)
    {
      if (obj == null)
        return 0;
      fixed (char* bytes = obj)
        return this.GetHashCode((byte*) bytes, checked ((uint) obj.Length * 2U));
    }

    internal unsafe int GetHashCode(byte* bytes, uint len) => (int) SipHashBasedStringEqualityComparer.SipHash_2_4_UlongCast_ForcedInline(bytes, len, this._k0, this._k1);

    private static unsafe ulong SipHash_2_4_UlongCast_ForcedInline(
      byte* finb,
      uint inlen,
      ulong k0,
      ulong k1)
    {
      ulong num1 = 8317987319222330741UL ^ k0;
      ulong num2 = 7237128888997146477UL ^ k1;
      ulong num3 = 7816392313619706465UL ^ k0;
      ulong num4 = 8387220255154660723UL ^ k1;
      ulong num5 = (ulong) inlen << 56;
      if (inlen > 0U)
      {
        byte* numPtr1 = finb;
        uint num6 = inlen & 7U;
        uint num7 = inlen;
        byte* numPtr2 = numPtr1 + num7 - num6;
        ulong* numPtr3 = (ulong*) finb;
        for (ulong* numPtr4 = (ulong*) numPtr2; numPtr3 < numPtr4; ++numPtr3)
        {
          ulong num8 = num4 ^ *numPtr3;
          ulong num9 = num1 + num2;
          ulong num10 = (num2 << 13 | num2 >> 51) ^ num9;
          ulong num11 = num9 << 32 | num9 >> 32;
          ulong num12 = num3 + num8;
          ulong num13 = (num8 << 16 | num8 >> 48) ^ num12;
          ulong num14 = num11 + num13;
          ulong num15 = (num13 << 21 | num13 >> 43) ^ num14;
          ulong num16 = num12 + num10;
          ulong num17 = (num10 << 17 | num10 >> 47) ^ num16;
          ulong num18 = num16 << 32 | num16 >> 32;
          ulong num19 = num14 + num17;
          ulong num20 = (num17 << 13 | num17 >> 51) ^ num19;
          ulong num21 = num19 << 32 | num19 >> 32;
          ulong num22 = num18 + num15;
          ulong num23 = (num15 << 16 | num15 >> 48) ^ num22;
          ulong num24 = num21 + num23;
          num4 = (num23 << 21 | num23 >> 43) ^ num24;
          ulong num25 = num22 + num20;
          num2 = (num20 << 17 | num20 >> 47) ^ num25;
          num3 = num25 << 32 | num25 >> 32;
          num1 = num24 ^ *numPtr3;
        }
        for (int index = 0; (long) index < (long) num6; ++index)
          num5 |= (ulong) numPtr2[index] << 8 * index;
      }
      ulong num26 = num4 ^ num5;
      ulong num27 = num1 + num2;
      ulong num28 = (num2 << 13 | num2 >> 51) ^ num27;
      ulong num29 = num27 << 32 | num27 >> 32;
      ulong num30 = num3 + num26;
      ulong num31 = (num26 << 16 | num26 >> 48) ^ num30;
      ulong num32 = num29 + num31;
      ulong num33 = (num31 << 21 | num31 >> 43) ^ num32;
      ulong num34 = num30 + num28;
      ulong num35 = (num28 << 17 | num28 >> 47) ^ num34;
      ulong num36 = num34 << 32 | num34 >> 32;
      ulong num37 = num32 + num35;
      ulong num38 = (num35 << 13 | num35 >> 51) ^ num37;
      ulong num39 = num37 << 32 | num37 >> 32;
      ulong num40 = num36 + num33;
      ulong num41 = (num33 << 16 | num33 >> 48) ^ num40;
      ulong num42 = num39 + num41;
      ulong num43 = (num41 << 21 | num41 >> 43) ^ num42;
      ulong num44 = num40 + num38;
      ulong num45 = (num38 << 17 | num38 >> 47) ^ num44;
      ulong num46 = num44 << 32 | num44 >> 32;
      ulong num47 = num42 ^ num5;
      ulong num48 = num46 ^ (ulong) byte.MaxValue;
      ulong num49 = num47 + num45;
      ulong num50 = (num45 << 13 | num45 >> 51) ^ num49;
      ulong num51 = num49 << 32 | num49 >> 32;
      ulong num52 = num48 + num43;
      ulong num53 = (num43 << 16 | num43 >> 48) ^ num52;
      ulong num54 = num51 + num53;
      ulong num55 = (num53 << 21 | num53 >> 43) ^ num54;
      ulong num56 = num52 + num50;
      ulong num57 = (num50 << 17 | num50 >> 47) ^ num56;
      ulong num58 = num56 << 32 | num56 >> 32;
      ulong num59 = num54 + num57;
      ulong num60 = (num57 << 13 | num57 >> 51) ^ num59;
      ulong num61 = num59 << 32 | num59 >> 32;
      ulong num62 = num58 + num55;
      ulong num63 = (num55 << 16 | num55 >> 48) ^ num62;
      ulong num64 = num61 + num63;
      ulong num65 = (num63 << 21 | num63 >> 43) ^ num64;
      ulong num66 = num62 + num60;
      ulong num67 = (num60 << 17 | num60 >> 47) ^ num66;
      ulong num68 = num66 << 32 | num66 >> 32;
      ulong num69 = num64 + num67;
      ulong num70 = (num67 << 13 | num67 >> 51) ^ num69;
      ulong num71 = num69 << 32 | num69 >> 32;
      ulong num72 = num68 + num65;
      ulong num73 = (num65 << 16 | num65 >> 48) ^ num72;
      ulong num74 = num71 + num73;
      ulong num75 = (num73 << 21 | num73 >> 43) ^ num74;
      ulong num76 = num72 + num70;
      ulong num77 = (num70 << 17 | num70 >> 47) ^ num76;
      ulong num78 = num76 << 32 | num76 >> 32;
      ulong num79 = num74 + num77;
      ulong num80 = (num77 << 13 | num77 >> 51) ^ num79;
      ulong num81 = num79 << 32 | num79 >> 32;
      ulong num82 = num78 + num75;
      ulong num83 = (num75 << 16 | num75 >> 48) ^ num82;
      ulong num84 = num81 + num83;
      ulong num85 = (num83 << 21 | num83 >> 43) ^ num84;
      ulong num86 = num82 + num80;
      ulong num87 = (num80 << 17 | num80 >> 47) ^ num86;
      ulong num88 = num86 << 32 | num86 >> 32;
      return num84 ^ num87 ^ num88 ^ num85;
    }
  }
}
