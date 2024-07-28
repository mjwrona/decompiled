// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.BloomFilter`1
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using System;
using System.Threading;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [CLSCompliant(false)]
  public class BloomFilter<T> where T : ILongHash
  {
    private readonly long[] elements;
    public readonly ulong BitCount;
    private const int bitsPerElement = 64;
    private long bitsSet;

    public long BitsSet => Interlocked.Read(ref this.bitsSet);

    private static ulong DivideRoundUp(ulong dividend, ulong divisor) => (dividend + divisor - 1UL) / divisor;

    public BloomFilter(ulong size)
    {
      this.BitCount = size;
      this.elements = new long[BloomFilter<T>.DivideRoundUp(size, 64UL)];
    }

    public void Insert(T id)
    {
      if (!this.SetBit(id))
        return;
      Interlocked.Increment(ref this.bitsSet);
    }

    public BloomFilterCheckResult Check(T id) => !this.CheckBit(id) ? BloomFilterCheckResult.DefinitelyNotInserted : BloomFilterCheckResult.MaybeInserted;

    private void GetBitIndices(T id, out ulong elementIndex, out long bitMask)
    {
      ulong num1 = (ulong) id.GetLongHashCode() % this.BitCount;
      elementIndex = num1 / 64UL;
      ulong num2;
      bitMask = 1L << (int) (num2 = num1 % 64UL);
    }

    private bool CheckBit(T id)
    {
      ulong elementIndex;
      long bitMask;
      this.GetBitIndices(id, out elementIndex, out bitMask);
      return (Interlocked.Read(ref this.elements[elementIndex]) & bitMask) != 0L;
    }

    private bool SetBit(T id)
    {
      ulong elementIndex;
      long bitMask;
      this.GetBitIndices(id, out elementIndex, out bitMask);
      long comparand;
      long num;
      do
      {
        comparand = Interlocked.Read(ref this.elements[elementIndex]);
        if ((comparand & bitMask) != 0L)
          return false;
        num = comparand | bitMask;
      }
      while (comparand != Interlocked.CompareExchange(ref this.elements[elementIndex], num, comparand));
      return true;
    }
  }
}
