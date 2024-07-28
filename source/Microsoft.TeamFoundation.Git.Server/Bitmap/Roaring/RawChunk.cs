// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring.RawChunk
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring
{
  internal class RawChunk : AbstractChunk
  {
    private int? m_count;
    private ulong[] m_bitmap;
    private int m_bitmapOffset;
    private int? m_firstNonZeroEntry;
    private int? m_minTrailingZero;
    private const int c_maskBits = 63;
    public const int FullLength = 1024;

    public RawChunk()
    {
      this.m_bitmap = new ulong[1024];
      this.m_count = new int?(0);
    }

    public RawChunk(ulong[] bitmap, int offset = 0, bool readOnly = false)
    {
      this.m_bitmap = bitmap;
      this.m_bitmapOffset = offset;
      this.IsReadOnly = readOnly;
    }

    public RawChunk(IEnumerable<ushort> values)
      : this()
    {
      foreach (ushort index in values)
        this.Add(index);
    }

    public override ushort LowerBound
    {
      get
      {
        this.EnsureZeroesComputed();
        return (ushort) ((this.Offset + this.m_firstNonZeroEntry.Value) * 64);
      }
    }

    public override ushort UpperBound
    {
      get
      {
        this.EnsureZeroesComputed();
        return (ushort) (63 + (this.Offset + this.m_minTrailingZero.Value - 1) * 64);
      }
    }

    public override int Count
    {
      get
      {
        this.EnsureCountComputed();
        return this.m_count.Value;
      }
    }

    public override int CountRuns
    {
      get
      {
        if (this.m_bitmap.Length == 0)
          return 0;
        ulong num = 0;
        for (int index = 0; index < this.m_bitmap.Length - 1; ++index)
          num += (ulong) BitUtils.BitCount((ulong) ((long) this.m_bitmap[index] << 1 & ~(long) this.m_bitmap[index])) + (this.m_bitmap[index] >> 63 & ~this.m_bitmap[index + 1]);
        int index1 = this.m_bitmap.Length - 1;
        return checked ((int) unchecked (num + (ulong) BitUtils.BitCount((ulong) ((long) this.m_bitmap[index1] << 1 & ~(long) this.m_bitmap[index1])) + (this.m_bitmap[index1] >> 63)));
      }
    }

    public ulong[] Bitmap => this.m_bitmap;

    public int Offset => this.m_bitmapOffset;

    public override bool Add(ushort index)
    {
      this.EnsureMutable();
      int offset;
      ulong mask;
      if (!this.GetOffsetAndMask(index, out offset, out mask))
        throw new InvalidOperationException("offset");
      if (((long) this.m_bitmap[offset] & (long) mask) != 0L)
        return false;
      this.EnsureCountComputed();
      this.m_bitmap[offset] |= mask;
      if (this.m_firstNonZeroEntry.HasValue && offset <= this.m_firstNonZeroEntry.Value)
        this.m_firstNonZeroEntry = new int?(offset);
      if (this.m_minTrailingZero.HasValue && offset >= this.m_minTrailingZero.Value)
        this.m_minTrailingZero = new int?(offset + 1);
      int? count = this.m_count;
      this.m_count = count.HasValue ? new int?(count.GetValueOrDefault() + 1) : new int?();
      return true;
    }

    public bool Remove(ushort index)
    {
      this.EnsureMutable();
      this.EnsureCountComputed();
      int offset;
      ulong mask;
      if (!this.GetOffsetAndMask(index, out offset, out mask))
        throw new InvalidOperationException("offset");
      if (((long) this.m_bitmap[offset] & (long) mask) == 0L)
        return false;
      this.m_bitmap[offset] ^= mask;
      if (this.m_bitmap[offset] == 0UL)
      {
        if (this.m_firstNonZeroEntry.HasValue && offset <= this.m_firstNonZeroEntry.Value)
          this.m_firstNonZeroEntry = new int?();
        if (this.m_minTrailingZero.HasValue && offset == this.m_minTrailingZero.Value - 1)
          this.m_minTrailingZero = new int?();
      }
      if (this.m_count.HasValue)
      {
        int? count = this.m_count;
        this.m_count = count.HasValue ? new int?(count.GetValueOrDefault() - 1) : new int?();
      }
      return true;
    }

    public bool Toggle(ushort index)
    {
      this.EnsureMutable();
      int offset;
      ulong mask;
      if (!this.GetOffsetAndMask(index, out offset, out mask))
        throw new InvalidOperationException("offset");
      this.m_bitmap[offset] ^= mask;
      this.ClearCountsAndZeroes();
      return true;
    }

    public override bool Contains(ushort index)
    {
      int offset;
      ulong mask;
      return this.GetOffsetAndMask(index, out offset, out mask) && (this.m_bitmap[offset] & mask) > 0UL;
    }

    public override IEnumerator<ushort> GetEnumerator()
    {
      ushort medBits = (ushort) (this.Offset * 64);
      for (int i = 0; i < this.m_bitmap.Length; ++i)
      {
        if (this.m_bitmap[i] != 0UL)
        {
          ulong mask = 1;
          for (ushort j = 0; j < (ushort) 64; ++j)
          {
            if (((long) this.m_bitmap[i] & (long) mask) != 0L)
              yield return (ushort) ((uint) medBits | (uint) j);
            mask <<= 1;
          }
        }
        medBits += (ushort) 64;
      }
    }

    public override IChunk Optimize() => (IChunk) this;

    public void Absorb(IChunk chunk)
    {
      switch (chunk)
      {
        case ArrayChunk _:
          this.AbsorbArray((ArrayChunk) chunk);
          break;
        case RawChunk _:
          this.AbsorbRaw((RawChunk) chunk);
          break;
        case RunChunk _:
          this.AbsorbRun((RunChunk) chunk);
          break;
        default:
          throw new ArgumentException(nameof (chunk));
      }
    }

    public void Remove(IChunk chunk)
    {
      switch (chunk)
      {
        case ArrayChunk _:
          this.RemoveArray((ArrayChunk) chunk);
          break;
        case RawChunk _:
          this.RemoveRaw((RawChunk) chunk);
          break;
        case RunChunk _:
          this.RemoveRun((RunChunk) chunk);
          break;
        default:
          throw new ArgumentException(nameof (chunk));
      }
    }

    private void AbsorbArray(ArrayChunk chunk)
    {
      foreach (ushort index in (AbstractChunk) chunk)
        this.Add(index);
    }

    private void RemoveArray(ArrayChunk chunk)
    {
      foreach (ushort index in (AbstractChunk) chunk)
        this.Remove(index);
    }

    private void AbsorbRaw(RawChunk chunk)
    {
      for (int index1 = 0; index1 < chunk.Bitmap.Length; ++index1)
      {
        int index2 = index1 + chunk.Offset - this.m_bitmapOffset;
        if (index2 >= 0 && index2 < this.m_bitmap.Length)
          this.m_bitmap[index2] |= chunk.Bitmap[index1];
      }
      this.ClearCountsAndZeroes();
    }

    private void RemoveRaw(RawChunk chunk)
    {
      for (int index1 = 0; index1 < chunk.Bitmap.Length; ++index1)
      {
        int index2 = index1 + chunk.Offset - this.m_bitmapOffset;
        if (index2 >= 0 && index2 < this.m_bitmap.Length)
          this.m_bitmap[index2] &= ~chunk.Bitmap[index1];
      }
      this.ClearCountsAndZeroes();
    }

    private void AbsorbRun(RunChunk chunk)
    {
      for (int index = 0; index < 2 * chunk.CountRuns; index += 2)
        BitUtils.FillBitmapWithRun(this.m_bitmap, this.m_bitmapOffset, (int) chunk.Runs[index], (int) chunk.Runs[index] + (int) chunk.Runs[index + 1]);
      this.ClearCountsAndZeroes();
    }

    private void RemoveRun(RunChunk chunk)
    {
      for (int index = 0; index < 2 * chunk.CountRuns; index += 2)
        BitUtils.RemoveBitsFromRun(this.m_bitmap, this.m_bitmapOffset, (int) chunk.Runs[index], (int) chunk.Runs[index] + (int) chunk.Runs[index + 1]);
      this.ClearCountsAndZeroes();
    }

    public override int GetSize() => this.m_bitmap.Length * 8 + 16;

    public static int EstimateSize(int minElement, int maxElement) => RawChunk.EstimateSizeFromOffsets(minElement / 64, (maxElement + 64 - 1) / 64);

    private static int EstimateSizeFromOffsets(int minOffset, int maxOffset) => (maxOffset + 1 - minOffset) * 8 + 16;

    public static int UnoptimizedSize => 8208;

    public bool GetOffsetAndMask(ushort index, out int offset, out ulong mask)
    {
      offset = (int) index / 64 - this.m_bitmapOffset;
      mask = (ulong) (1L << (int) index);
      return offset >= 0 && offset < this.m_bitmap.Length;
    }

    private void EnsureCountComputed()
    {
      if (this.m_count.HasValue)
        return;
      this.m_count = new int?(0);
      for (int index = 0; index < this.m_bitmap.Length; ++index)
      {
        if (this.m_bitmap[index] != 0UL)
        {
          int? count = this.m_count;
          int num = BitUtils.BitCount(this.m_bitmap[index]);
          this.m_count = count.HasValue ? new int?(count.GetValueOrDefault() + num) : new int?();
        }
      }
    }

    private void EnsureZeroesComputed()
    {
      if (this.m_firstNonZeroEntry.HasValue && this.m_minTrailingZero.HasValue)
        return;
      this.m_firstNonZeroEntry = new int?(0);
      bool flag = true;
      this.m_minTrailingZero = new int?(this.m_bitmap.Length);
      for (int index = 0; index < this.m_bitmap.Length; ++index)
      {
        if (this.m_bitmap[index] == 0UL)
        {
          if (flag)
            this.m_firstNonZeroEntry = new int?(index + 1);
        }
        else
        {
          flag = false;
          this.m_minTrailingZero = new int?(index + 1);
        }
      }
    }

    private void ClearCountsAndZeroes()
    {
      this.m_firstNonZeroEntry = new int?();
      this.m_minTrailingZero = new int?();
      this.m_count = new int?();
    }

    public override IChunk Duplicate()
    {
      ulong[] numArray = new ulong[this.m_bitmap.Length];
      Array.Copy((Array) this.m_bitmap, (Array) numArray, numArray.Length);
      return (IChunk) new RawChunk(numArray, this.m_bitmapOffset);
    }
  }
}
