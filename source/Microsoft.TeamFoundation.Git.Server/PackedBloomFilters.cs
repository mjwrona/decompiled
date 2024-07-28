// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.PackedBloomFilters
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class PackedBloomFilters : IPackedBloomFilters
  {
    private int m_curMaxSetLabel;
    private uint m_curEndOfFilters;
    private long[] m_packedFilters;
    private const int c_bitsInLong = 64;
    private byte[] m_packedStartsAndSizes;
    private const int c_bytesInStartAndSize = 5;
    private const int c_maxFilterSize = 255;
    private static PackedBloomFilters.LabelStatus NotComputedStatus = new PackedBloomFilters.LabelStatus()
    {
      Size = 0,
      Start = 0,
      Status = BloomFilterStatus.NotComputed
    };

    public PackedBloomFilters(int capacity = 0)
      : this(capacity, BloomFilterSettings.Default)
    {
    }

    public PackedBloomFilters(int capacity, BloomFilterSettings settings)
    {
      GitServerUtils.CheckIsLittleEndian();
      this.m_curEndOfFilters = 0U;
      this.m_curMaxSetLabel = 0;
      this.m_packedFilters = new long[capacity / 64 + 1];
      this.m_packedStartsAndSizes = new byte[5 * capacity];
      this.Settings = settings;
    }

    internal PackedBloomFilters(
      byte[] startsAndSizes,
      long[] packedFilters,
      BloomFilterSettings settings)
    {
      this.m_curMaxSetLabel = startsAndSizes.Length / 5;
      this.m_curEndOfFilters = (uint) packedFilters.Length;
      this.m_packedFilters = packedFilters;
      this.m_packedStartsAndSizes = startsAndSizes;
      this.Settings = settings;
    }

    public BloomFilterStatus GetFilterStatus(int label) => this.GetLabelStatus(label).Status;

    public void SetFilterStatus(int label, BloomFilterStatus status)
    {
      PackedBloomFilters.LabelStatus labelStatus = this.GetLabelStatus(label, true) with
      {
        Status = status
      };
      this.StoreLabelStatus(label, labelStatus);
    }

    public void SetFilter(int label, BloomFilter filter)
    {
      if (this.GetLabelStatus(label, true).Status != BloomFilterStatus.NotComputed)
        return;
      uint size = filter.Size;
      if (size > (uint) byte.MaxValue)
        throw new InvalidBloomFilterValue("size", size, (uint) byte.MaxValue);
      if ((long) (this.m_curEndOfFilters + size) >= (long) this.m_packedFilters.Length)
      {
        long[] destinationArray = new long[(int) Math.Max((long) (uint) ((int) this.m_curEndOfFilters + (int) size + 1), (long) (int) (1.5 * (double) this.m_packedFilters.Length))];
        Array.Copy((Array) this.m_packedFilters, (Array) destinationArray, this.m_packedFilters.Length);
        this.m_packedFilters = destinationArray;
      }
      uint curEndOfFilters = this.m_curEndOfFilters;
      this.StoreLabelStatus(label, size, curEndOfFilters, BloomFilterStatus.Computing);
      if (size > 0U)
        Array.Copy((Array) filter.BitArray, (long) filter.Start, (Array) this.m_packedFilters, (long) curEndOfFilters, (long) size);
      this.m_curEndOfFilters += size;
      this.SetFilterStatus(label, BloomFilterStatus.Computed);
    }

    public void ComputeFilter(int label, IList<string> entries)
    {
      if (this.GetLabelStatus(label, true).Status != BloomFilterStatus.NotComputed)
        return;
      uint num = 0;
      uint count = (uint) entries.Count;
      if (count > 0U)
        num = (uint) ((int) count * (int) this.Settings.NumBitsPerEntry - 1) / 64U + 1U;
      if (num > (uint) byte.MaxValue || count > uint.MaxValue / (uint) this.Settings.NumBitsPerEntry)
        throw new InvalidBloomFilterValue("size", num, (uint) byte.MaxValue);
      if ((long) (this.m_curEndOfFilters + num) >= (long) this.m_packedFilters.Length)
      {
        long[] destinationArray = new long[(int) Math.Max((long) (uint) ((int) this.m_curEndOfFilters + (int) num + 1), (long) (int) (1.5 * (double) this.m_packedFilters.Length))];
        Array.Copy((Array) this.m_packedFilters, (Array) destinationArray, (long) this.m_curEndOfFilters);
        this.m_packedFilters = destinationArray;
      }
      uint curEndOfFilters = this.m_curEndOfFilters;
      this.StoreLabelStatus(label, num, curEndOfFilters, BloomFilterStatus.Computing);
      this.m_curEndOfFilters += num;
      BloomFilter bloomFilter = new BloomFilter(this.m_packedFilters, curEndOfFilters, num, this.Settings);
      for (int index = 0; index < entries.Count; ++index)
        bloomFilter.Add(this.EncodeKey(entries[index]));
      this.SetFilterStatus(label, BloomFilterStatus.Computed);
    }

    public IReadOnlyBloomFilter GetReadOnlyFilter(int label)
    {
      if (this.GetLabelStatus(label, true).Status != BloomFilterStatus.Computed)
        return (IReadOnlyBloomFilter) null;
      PackedBloomFilters.LabelStatus labelStatus = this.GetLabelStatus(label);
      return (IReadOnlyBloomFilter) new BloomFilter(this.m_packedFilters, labelStatus.Start, labelStatus.Size, this.Settings);
    }

    public long GetSize() => (long) (IntPtr.Size + 4 + IntPtr.Size + 8 * this.m_packedFilters.Length + IntPtr.Size + this.m_packedStartsAndSizes.Length);

    public BloomFilterSettings Settings { get; }

    internal byte[] Statuses => this.m_packedStartsAndSizes;

    internal long[] FilterData => this.m_packedFilters;

    internal int CurNumLabels => this.m_curMaxSetLabel;

    internal uint CurFilterSize => this.m_curEndOfFilters;

    private void StoreLabelStatus(int label, PackedBloomFilters.LabelStatus labelStatus) => this.StoreLabelStatus(label, labelStatus.Size, labelStatus.Start, labelStatus.Status);

    private void StoreLabelStatus(int label, uint size, uint start, BloomFilterStatus status)
    {
      if (size > (uint) byte.MaxValue)
        throw new InvalidBloomFilterValue(nameof (size), size, (uint) byte.MaxValue);
      if (start >= 2147483646U)
        throw new InvalidBloomFilterValue(nameof (start), start, 2147483645U);
      this.ExtendStatus(label);
      byte[] bytes = BitConverter.GetBytes(start);
      switch (status)
      {
        case BloomFilterStatus.NotComputed:
          size = 0U;
          for (int index = 0; index < 4; ++index)
            bytes[index] = (byte) 0;
          break;
        case BloomFilterStatus.Computing:
          bytes[3] |= (byte) 128;
          break;
        case BloomFilterStatus.TooLarge:
          for (int index = 0; index < 4; ++index)
            bytes[index] = byte.MaxValue;
          break;
      }
      if (size == 0U)
      {
        bytes[3] |= (byte) 127;
        bytes[2] |= byte.MaxValue;
        bytes[1] |= byte.MaxValue;
        bytes[0] |= (byte) 254;
      }
      this.m_packedStartsAndSizes[5 * label] = (byte) size;
      Array.Copy((Array) bytes, 0, (Array) this.m_packedStartsAndSizes, 5 * label + 1, 4);
    }

    private PackedBloomFilters.LabelStatus GetLabelStatus(int label, bool extendStatus = false)
    {
      if (extendStatus)
        this.ExtendStatus(label);
      else if (5 * (label + 1) > this.m_packedStartsAndSizes.Length)
        return PackedBloomFilters.NotComputedStatus;
      uint uint32 = BitConverter.ToUInt32(this.m_packedStartsAndSizes, 5 * label + 1);
      uint num1 = (uint) this.m_packedStartsAndSizes[5 * label];
      uint num2 = 0;
      BloomFilterStatus bloomFilterStatus = uint32 != 0U || num1 != 0U ? (uint32 != uint.MaxValue ? (((int) uint32 & int.MinValue) == 0 ? BloomFilterStatus.Computed : BloomFilterStatus.Computing) : BloomFilterStatus.TooLarge) : BloomFilterStatus.NotComputed;
      if (bloomFilterStatus == BloomFilterStatus.Computing || bloomFilterStatus == BloomFilterStatus.Computed)
        num2 = uint32 & (uint) int.MaxValue;
      if (num2 == 2147483646U)
      {
        num1 = 0U;
        num2 = 0U;
      }
      return new PackedBloomFilters.LabelStatus()
      {
        Size = num1,
        Start = num2,
        Status = bloomFilterStatus
      };
    }

    private void ExtendStatus(int label)
    {
      if (label >= this.m_curMaxSetLabel)
        this.m_curMaxSetLabel = label + 1;
      if (5 * (label + 1) < this.m_packedStartsAndSizes.Length)
        return;
      byte[] destinationArray = new byte[Math.Max(5 * (label + 1) + 1, (int) (1.5 * (double) this.m_packedStartsAndSizes.Length))];
      Array.Copy((Array) this.m_packedStartsAndSizes, (Array) destinationArray, this.m_packedStartsAndSizes.Length);
      this.m_packedStartsAndSizes = destinationArray;
    }

    public BloomKey EncodeKey(string key) => new BloomKey(key, this.Settings);

    private struct LabelStatus
    {
      public uint Start { get; set; }

      public uint Size { get; set; }

      public BloomFilterStatus Status { get; set; }
    }
  }
}
