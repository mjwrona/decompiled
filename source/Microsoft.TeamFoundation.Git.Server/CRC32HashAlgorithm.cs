// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.CRC32HashAlgorithm
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Security.Cryptography;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class CRC32HashAlgorithm : HashAlgorithm
  {
    private static readonly uint[] table = new uint[256];
    private uint value = uint.MaxValue;

    static CRC32HashAlgorithm()
    {
      for (int index1 = 0; index1 < 256; ++index1)
      {
        uint num = (uint) index1;
        for (int index2 = 8; index2 > 0; --index2)
        {
          if (((int) num & 1) == 1)
            num = num >> 1 ^ 3988292384U;
          else
            num >>= 1;
        }
        CRC32HashAlgorithm.table[index1] = num;
      }
    }

    public override void Initialize() => this.value = uint.MaxValue;

    protected override void HashCore(byte[] buffer, int offset, int count)
    {
      for (int index1 = 0; index1 < count; ++index1)
      {
        ulong index2 = (ulong) (this.value & (uint) byte.MaxValue ^ (uint) buffer[offset + index1]);
        this.value >>= 8;
        this.value ^= CRC32HashAlgorithm.table[index2];
      }
    }

    protected override byte[] HashFinal()
    {
      ulong num = (ulong) (this.value ^ uint.MaxValue);
      return new byte[4]
      {
        (byte) ((num & 4278190080UL) >> 24),
        (byte) ((num & 16711680UL) >> 16),
        (byte) ((num & 65280UL) >> 8),
        (byte) (num & (ulong) byte.MaxValue)
      };
    }
  }
}
