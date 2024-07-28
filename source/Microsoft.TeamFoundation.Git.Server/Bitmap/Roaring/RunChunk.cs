// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring.RunChunk
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring
{
  internal class RunChunk : AbstractChunk
  {
    private int m_count;
    private int m_curRunLength;
    private ushort[] m_runs;

    public RunChunk(int capacity = 16, bool readOnly = false)
    {
      this.m_curRunLength = 0;
      this.m_runs = new ushort[capacity * 2];
      this.IsReadOnly = readOnly;
    }

    public RunChunk(ushort[] runs, bool readOnly = false)
    {
      this.m_runs = runs.Length % 2 == 0 ? runs : throw new ArgumentException(nameof (runs));
      this.m_curRunLength = runs.Length;
      this.IsReadOnly = readOnly;
      this.Recount();
    }

    public RunChunk(IEnumerable<ushort> values)
      : this()
    {
      foreach (ushort index in values)
        this.Add(index);
    }

    public override int Count => this.m_count;

    public override int CountRuns => this.m_curRunLength / 2;

    public override ushort LowerBound => this.m_curRunLength == 0 ? (ushort) 0 : this.m_runs[0];

    public override ushort UpperBound => this.m_curRunLength == 0 ? (ushort) 0 : (ushort) ((uint) this.m_runs[this.m_curRunLength - 2] + (uint) this.m_runs[this.m_curRunLength - 1]);

    public ushort[] Runs => this.m_runs;

    public override bool Add(ushort value)
    {
      this.EnsureMutable();
      int indexForInsert;
      if (this.TryFindLocation(value, out indexForInsert))
        return false;
      if (indexForInsert + 1 >= this.m_runs.Length)
        this.ExtendArrays();
      if (indexForInsert == this.m_curRunLength)
      {
        this.m_runs[indexForInsert] = value;
        this.m_runs[indexForInsert + 1] = (ushort) 0;
        this.m_curRunLength += 2;
      }
      else
      {
        RunChunk.Run run = new RunChunk.Run(this.m_runs, indexForInsert);
        if ((int) value < (int) run.Min - 1)
        {
          if (this.m_curRunLength + 1 >= this.m_runs.Length)
            this.ExtendArrays();
          for (int index = this.m_curRunLength - 2; index >= indexForInsert; index -= 2)
          {
            this.m_runs[index + 2] = this.m_runs[index];
            this.m_runs[index + 3] = this.m_runs[index + 1];
          }
          this.m_runs[indexForInsert] = value;
          this.m_runs[indexForInsert + 1] = (ushort) 0;
          this.m_curRunLength += 2;
        }
        else if ((int) value == (int) run.Min - 1)
        {
          --this.m_runs[indexForInsert];
          ++this.m_runs[indexForInsert + 1];
          if (indexForInsert >= 2 && (int) this.m_runs[indexForInsert - 2] + (int) this.m_runs[indexForInsert - 1] == (int) value - 1)
            this.MergeConsecutiveIntervals(indexForInsert - 2);
        }
        else
        {
          if ((int) value != (int) run.Max + 1)
            throw new InvalidOperationException("TryFindLocation returned a bad interval!");
          ++this.m_runs[indexForInsert + 1];
          if (indexForInsert + 2 < this.m_curRunLength && (int) this.m_runs[indexForInsert + 2] == (int) value + 1)
            this.MergeConsecutiveIntervals(indexForInsert);
        }
      }
      ++this.m_count;
      return true;
    }

    public override bool Contains(ushort index) => this.TryFindLocation(index, out int _);

    public override IEnumerator<ushort> GetEnumerator()
    {
      int i = 0;
      while (i < this.m_curRunLength)
      {
        int iteration = 0;
        while (iteration <= (int) this.m_runs[checked (i + 1)])
        {
          yield return checked ((ushort) ((int) this.m_runs[i] + iteration));
          checked { ++iteration; }
        }
        checked { i += 2; }
      }
    }

    public override IChunk Optimize()
    {
      int num1 = this.m_curRunLength * 2;
      int num2 = this.m_count * 2;
      int num3 = 8192;
      if (num1 < num2 && num1 < num3)
        return (IChunk) this;
      return 5 * num2 < 2 * num3 ? (IChunk) new ArrayChunk((IEnumerable<ushort>) this) : (IChunk) new RawChunk((IEnumerable<ushort>) this);
    }

    public override int GetSize() => RunChunk.EstimateSize(this.m_runs.Length / 2);

    public static int EstimateSize(int numRuns) => 9 + 2 * numRuns * 2;

    private void Recount()
    {
      this.m_count = 0;
      for (int index = 0; index < this.m_curRunLength; index += 2)
        this.m_count += (int) this.m_runs[index + 1] + 1;
    }

    private bool TryFindLocation(ushort value, out int indexForInsert)
    {
      if (this.m_curRunLength == 0)
      {
        indexForInsert = 0;
        return false;
      }
      RunChunk.Run run = new RunChunk.Run(this.m_runs, this.m_curRunLength - 2);
      if ((int) run.Min - 1 <= (int) value)
      {
        if ((int) run.Max + 1 < (int) value)
        {
          indexForInsert = this.m_curRunLength;
          return false;
        }
        indexForInsert = this.m_curRunLength - 2;
        return (int) run.Min <= (int) value && (int) run.Max >= (int) value;
      }
      int num1 = 0;
      int num2 = this.m_curRunLength - 2;
      while (num1 < num2)
      {
        int index = (num1 + num2) / 2 & -2;
        run = new RunChunk.Run(this.m_runs, index);
        if ((int) run.Min - 1 <= (int) value && (int) run.Max + 1 >= (int) value)
        {
          indexForInsert = index;
          return (int) run.Min <= (int) value && (int) run.Max >= (int) value;
        }
        if ((int) run.Min - 1 > (int) value)
          num2 = index;
        else
          num1 = index + 2;
      }
      indexForInsert = num1;
      return false;
    }

    private void MergeConsecutiveIntervals(int atIndex)
    {
      this.m_runs[checked (atIndex + 1)] = checked ((ushort) ((int) this.m_runs[atIndex + 1] + (int) this.m_runs[atIndex + 3] + 1));
      for (int index = atIndex + 2; index < this.m_curRunLength - 2; index += 2)
      {
        this.m_runs[index] = this.m_runs[index + 2];
        this.m_runs[index + 1] = this.m_runs[index + 3];
      }
      this.m_curRunLength -= 2;
    }

    private void ExtendArrays()
    {
      ushort[] destinationArray = new ushort[this.m_runs.Length * 2];
      Array.Copy((Array) this.m_runs, (Array) destinationArray, this.m_runs.Length);
      this.m_runs = destinationArray;
    }

    public override IChunk Duplicate()
    {
      ushort[] numArray = new ushort[2 * this.CountRuns];
      Array.Copy((Array) this.m_runs, (Array) numArray, numArray.Length);
      return (IChunk) new RunChunk(numArray);
    }

    private struct Run
    {
      public readonly ushort Min;
      public readonly ushort LengthMinus1;

      public Run(ushort[] runs, int index)
      {
        this.Min = runs[index];
        this.LengthMinus1 = runs[index + 1];
      }

      public ushort Max => (ushort) ((uint) this.Min + (uint) this.LengthMinus1);
    }
  }
}
