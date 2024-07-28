// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.BloomFilter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class BloomFilter : IBloomFilter, IReadOnlyBloomFilter
  {
    private readonly long[] m_bitArray;
    private readonly uint m_start;
    private readonly uint m_size;
    private readonly uint m_modulo;
    private readonly BloomFilterSettings m_settings;
    private const int c_bitsInLong = 64;

    public BloomFilter(uint size)
      : this(new long[(int) size], 0U, size)
    {
    }

    internal BloomFilter(long[] bitArray, uint offset, uint length, BloomFilterSettings settings = null)
    {
      this.m_bitArray = bitArray;
      this.m_start = offset;
      this.m_size = length;
      this.m_modulo = length * 64U;
      this.m_settings = settings ?? BloomFilterSettings.Default;
    }

    public void Add(BloomKey key)
    {
      for (int index = 0; index < key.BaseHashes.Count; ++index)
      {
        uint arrayIndex;
        long bitMask;
        this.GetBitPosition(key.BaseHashes[index], out arrayIndex, out bitMask);
        this.m_bitArray[(int) arrayIndex] |= bitMask;
      }
    }

    public bool ProbablyContains(BloomKey key)
    {
      if (this.m_size == 0U)
        return false;
      for (int index = 0; index < key.BaseHashes.Count; ++index)
      {
        uint arrayIndex;
        long bitMask;
        this.GetBitPosition(key.BaseHashes[index], out arrayIndex, out bitMask);
        if ((this.m_bitArray[(int) arrayIndex] & bitMask) == 0L)
          return false;
      }
      return true;
    }

    private void GetBitPosition(uint baseHash, out uint arrayIndex, out long bitMask)
    {
      int num1 = (int) (baseHash % this.m_modulo);
      uint num2 = (uint) num1 / 64U;
      int num3 = num1 & 63;
      arrayIndex = this.m_start + num2;
      bitMask = 1L << num3;
    }

    public BloomKey EncodeKey(string key) => new BloomKey(key, this.m_settings);

    internal uint Start => this.m_start;

    internal uint Size => this.m_size;

    internal long[] BitArray => this.m_bitArray;
  }
}
