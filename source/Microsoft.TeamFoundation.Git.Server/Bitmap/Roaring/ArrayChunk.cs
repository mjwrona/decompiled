// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring.ArrayChunk
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring
{
  internal class ArrayChunk : AbstractChunk
  {
    private int m_count;
    private ushort[] m_values;
    private const int c_initialArraySize = 32;

    public ArrayChunk(int? capacity = null)
    {
      if (capacity.HasValue)
        this.m_values = new ushort[capacity.Value];
      else
        this.m_values = Array.Empty<ushort>();
    }

    public ArrayChunk(IEnumerable<ushort> values, int? count = null, bool readOnly = false)
      : this(count)
    {
      foreach (ushort index in values)
        this.Add(index);
      this.IsReadOnly = readOnly;
    }

    public ushort[] Values => this.m_values;

    public override int Count => this.m_count;

    public override int CountRuns
    {
      get
      {
        if (this.m_count == 0)
          return 0;
        int countRuns = 1;
        for (int index = 1; index < this.m_count; ++index)
        {
          if ((int) this.m_values[index] > (int) this.m_values[index - 1] + 1)
            ++countRuns;
        }
        return countRuns;
      }
    }

    public override ushort LowerBound => this.m_count == 0 ? (ushort) 0 : this.m_values[0];

    public override ushort UpperBound => this.m_count == 0 ? (ushort) 0 : this.m_values[this.m_count - 1];

    public override bool Add(ushort value)
    {
      this.EnsureMutable();
      int index;
      if (this.TryFindLocation(value, out index))
        return false;
      if (this.m_count >= this.m_values.Length)
      {
        ushort[] destinationArray = new ushort[Math.Max(this.m_values.Length * 2, 32)];
        Array.Copy((Array) this.m_values, (Array) destinationArray, this.m_values.Length);
        this.m_values = destinationArray;
      }
      for (int count = this.m_count; count > index; --count)
        this.m_values[count] = this.m_values[count - 1];
      this.m_values[index] = value;
      ++this.m_count;
      return true;
    }

    public override bool Contains(ushort value) => this.TryFindLocation(value, out int _);

    public override IEnumerator<ushort> GetEnumerator()
    {
      for (int i = 0; i < this.m_count; ++i)
        yield return this.m_values[i];
    }

    public override IChunk Optimize() => this.GetSize() >= RawChunk.UnoptimizedSize ? (IChunk) new RawChunk((IEnumerable<ushort>) this) : (IChunk) this;

    private bool TryFindLocation(ushort value, out int index)
    {
      if (this.m_count == 0 || (int) this.m_values[this.m_count - 1] < (int) value)
      {
        index = this.m_count;
        return false;
      }
      index = Array.BinarySearch<ushort>(this.m_values, 0, this.m_count, value);
      if (index < 0)
        index = ~index;
      return index < this.m_count && (int) this.m_values[index] == (int) value;
    }

    public override int GetSize() => ArrayChunk.EstimateSize(this.m_values.Length);

    public static int EstimateSize(int count) => count * 2 + 2 + 1;

    public override IChunk Duplicate() => (IChunk) new ArrayChunk((IEnumerable<ushort>) this, new int?(this.Count));
  }
}
