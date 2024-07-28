// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Murmur3HashFunction
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class Murmur3HashFunction : IBloomFilterHashFunction
  {
    public static readonly Murmur3HashFunction Instance = new Murmur3HashFunction();

    private Murmur3HashFunction()
    {
    }

    public BloomFilterHashVersion HashVersion => BloomFilterHashVersion.Murmur3;

    public uint[] GetHashValues(string key, uint numValues)
    {
      byte[] bytes = GitEncodingUtil.SafeUtf8NoBom.GetBytes(key);
      uint[] hashValues = new uint[(int) numValues];
      uint hash1 = 691726191;
      uint hash2 = 2120511020;
      Murmur3HashFunction.Murmur3(bytes, bytes.Length, ref hash1);
      Murmur3HashFunction.Murmur3(bytes, bytes.Length, ref hash2);
      for (uint index = 0; index < numValues; ++index)
        hashValues[(int) index] = hash1 + (index + 1U) * hash2;
      return hashValues;
    }

    internal static unsafe void Murmur3(byte[] bytes, int count, ref uint hash)
    {
      fixed (byte* numPtr1 = bytes)
      {
        int num1 = count / 4;
        byte* numPtr2 = numPtr1;
        uint* numPtr3 = (uint*) numPtr2;
        for (int index = 0; index < num1; ++index)
        {
          uint num2 = Murmur3HashFunction.RotateRight32(numPtr3[index] * 3432918353U, 15) * 461845907U;
          hash ^= num2;
          hash = (uint) ((int) Murmur3HashFunction.RotateRight32(hash, 13) * 5 - 430675100);
        }
        byte* numPtr4 = numPtr2 + num1 * 4;
        uint num3 = 0;
        switch (count & 3)
        {
          case 1:
            uint num4 = Murmur3HashFunction.RotateRight32((num3 ^ (uint) *numPtr4) * 3432918353U, 15) * 461845907U;
            hash ^= num4;
            break;
          case 2:
            num3 ^= (uint) numPtr4[1] << 8;
            goto case 1;
          case 3:
            num3 ^= (uint) numPtr4[2] << 16;
            goto case 2;
        }
        hash ^= (uint) count;
        hash ^= hash >> 16;
        hash *= 2246822507U;
        hash ^= hash >> 13;
        hash *= 3266489909U;
        hash ^= hash >> 16;
      }
    }

    public static uint RotateLeft32(uint value, int count)
    {
      int num = 31;
      count &= num;
      value = value << count | value >> (-count & num);
      return value;
    }

    public static uint RotateRight32(uint value, int count)
    {
      int num = 31;
      count &= num;
      value = value >> count | value << (-count & num);
      return value;
    }
  }
}
