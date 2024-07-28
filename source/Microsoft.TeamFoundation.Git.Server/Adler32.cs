// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Adler32
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class Adler32 : HashAlgorithm
  {
    private uint m_s1;
    private uint m_s2;
    private int m_iSinceMod;
    internal const int ModBase = 65521;
    internal const int ModInterval = 5552;

    public Adler32()
    {
      this.HashSizeValue = 32;
      this.Initialize();
    }

    public override void Initialize()
    {
      this.m_s1 = 1U;
      this.m_s2 = 0U;
      this.m_iSinceMod = 0;
    }

    protected override void HashCore(byte[] array, int ibStart, int cbSize)
    {
      uint num1 = this.m_s1;
      uint num2 = this.m_s2;
      int num3 = this.m_iSinceMod;
      int index;
      for (index = ibStart; index < ibStart + cbSize - 7; index += 8)
      {
        num3 += 8;
        if (num3 > 5552)
        {
          num1 %= 65521U;
          num2 %= 65521U;
          num3 = 8;
        }
        uint num4 = num1 + (uint) array[index];
        uint num5 = num2 + num4;
        uint num6 = num4 + (uint) array[index + 1];
        uint num7 = num5 + num6;
        uint num8 = num6 + (uint) array[index + 2];
        uint num9 = num7 + num8;
        uint num10 = num8 + (uint) array[index + 3];
        uint num11 = num9 + num10;
        uint num12 = num10 + (uint) array[index + 4];
        uint num13 = num11 + num12;
        uint num14 = num12 + (uint) array[index + 5];
        uint num15 = num13 + num14;
        uint num16 = num14 + (uint) array[index + 6];
        uint num17 = num15 + num16;
        num1 = num16 + (uint) array[index + 7];
        num2 = num17 + num1;
      }
      for (; index < ibStart + cbSize; ++index)
      {
        ++num3;
        if (num3 > 5552)
        {
          num1 %= 65521U;
          num2 %= 65521U;
          num3 = 1;
        }
        num1 += (uint) array[index];
        num2 += num1;
      }
      this.m_s1 = num1;
      this.m_s2 = num2;
      this.m_iSinceMod = num3;
    }

    protected override byte[] HashFinal()
    {
      this.m_s1 %= 65521U;
      this.m_s2 %= 65521U;
      return new byte[4]
      {
        (byte) ((this.m_s2 & 65280U) >> 8),
        (byte) (this.m_s2 & (uint) byte.MaxValue),
        (byte) ((this.m_s1 & 65280U) >> 8),
        (byte) (this.m_s1 & (uint) byte.MaxValue)
      };
    }
  }
}
