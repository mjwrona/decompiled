// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring.ChunkOptimizer
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Bitmap.Roaring
{
  internal static class ChunkOptimizer
  {
    public static IChunk OptimizeAsReadOnly(IChunk chunk)
    {
      int count = chunk.Count;
      if (count == 0)
        return (IChunk) new ArrayChunk((IEnumerable<ushort>) Array.Empty<ushort>(), new int?(0), true);
      int lowerBound = (int) chunk.LowerBound;
      int upperBound = (int) chunk.UpperBound;
      int countRuns = chunk.CountRuns;
      int num1 = ArrayChunk.EstimateSize(count);
      int num2 = RawChunk.EstimateSize(lowerBound, upperBound);
      int num3 = RunChunk.EstimateSize(countRuns);
      if (num1 <= num2 && num1 <= num3)
        return (IChunk) ChunkOptimizer.CreateOptimizedArrayChunk(count, chunk);
      return num2 <= num3 ? (IChunk) ChunkOptimizer.CreateOptimizedRawChunk(lowerBound, upperBound, chunk) : (IChunk) ChunkOptimizer.CreateOptimizedRunChunk(countRuns, chunk);
    }

    public static ArrayChunk CreateOptimizedArrayChunk(int count, IChunk chunk)
    {
      ushort[] values = new ushort[count];
      int num1 = 0;
      foreach (ushort num2 in (IEnumerable<ushort>) chunk)
        values[num1++] = num2;
      return new ArrayChunk((IEnumerable<ushort>) values, new int?(count), true);
    }

    public static RawChunk CreateOptimizedRawChunk(int minElement, int maxElement, IChunk chunk)
    {
      if (chunk.Count == 0)
        return new RawChunk(Array.Empty<ulong>(), readOnly: true);
      switch (chunk)
      {
        case RawChunk _:
          return ChunkOptimizer.CreateOptimizedRawChunk(minElement, maxElement, (RawChunk) chunk);
        case RunChunk _:
          return ChunkOptimizer.CreateOptimizedRawChunk(minElement, maxElement, (RunChunk) chunk);
        default:
          int offset = minElement / 64;
          RawChunk optimizedRawChunk = new RawChunk(new ulong[(maxElement + 64 - 1) / 64 + 1 - offset], offset);
          foreach (ushort index in (IEnumerable<ushort>) chunk)
            optimizedRawChunk.Add(index);
          optimizedRawChunk.IsReadOnly = true;
          return optimizedRawChunk;
      }
    }

    private static RawChunk CreateOptimizedRawChunk(
      int minElement,
      int maxElement,
      RawChunk rawChunk)
    {
      if (rawChunk.Count == 0)
        return new RawChunk(Array.Empty<ulong>(), readOnly: true);
      int offset = minElement / 64;
      int length = maxElement / 64 - offset + 1;
      ulong[] numArray = new ulong[length];
      Array.Copy((Array) rawChunk.Bitmap, offset - rawChunk.Offset, (Array) numArray, 0, length);
      return new RawChunk(numArray, offset, true);
    }

    private static RawChunk CreateOptimizedRawChunk(
      int minElement,
      int maxElement,
      RunChunk runChunk)
    {
      if (runChunk.Count == 0)
        return new RawChunk(Array.Empty<ulong>(), readOnly: true);
      int offset = minElement / 64;
      ulong[] bitmap = new ulong[maxElement / 64 - offset + 1];
      for (int index = 0; index < runChunk.Runs.Length; index += 2)
        BitUtils.FillBitmapWithRun(bitmap, offset, (int) runChunk.Runs[index], (int) runChunk.Runs[index] + (int) runChunk.Runs[index + 1]);
      return new RawChunk(bitmap, offset, true);
    }

    public static RunChunk CreateOptimizedRunChunk(int numRuns, IChunk chunk)
    {
      if (numRuns == 0)
        return new RunChunk(0, true);
      switch (chunk)
      {
        case ArrayChunk _:
          return ChunkOptimizer.CreateOptimizedRunChunk(numRuns, (ArrayChunk) chunk);
        case RawChunk _:
          return ChunkOptimizer.CreateOptimizedRunChunk(numRuns, (RawChunk) chunk);
        case RunChunk _:
          return ChunkOptimizer.CreateOptimizedRunChunk(numRuns, (RunChunk) chunk);
        default:
          RunChunk optimizedRunChunk = new RunChunk(numRuns);
          foreach (ushort index in (IEnumerable<ushort>) chunk)
            optimizedRunChunk.Add(index);
          optimizedRunChunk.IsReadOnly = true;
          return optimizedRunChunk;
      }
    }

    private static RunChunk CreateOptimizedRunChunk(int numRuns, ArrayChunk arrChunk)
    {
      ushort[] runs = new ushort[2 * numRuns];
      ushort num1 = 0;
      int num2 = 0;
      runs[0] = arrChunk.Values[0];
      for (int index = 1; index < arrChunk.Count; ++index)
      {
        if ((int) arrChunk.Values[index] > (int) arrChunk.Values[index - 1] + 1)
        {
          runs[num2 + 1] = num1;
          runs[num2 + 2] = arrChunk.Values[index];
          num1 = (ushort) 0;
          num2 += 2;
        }
        else
          ++num1;
      }
      runs[num2 + 1] = num1;
      return new RunChunk(runs, true);
    }

    private static RunChunk CreateOptimizedRunChunk(int numRuns, RawChunk rawChunk)
    {
      int index1 = 0;
      ushort[] runs = new ushort[2 * numRuns];
      int index2 = 0;
      ulong num1 = rawChunk.Bitmap[index2];
      while (true)
      {
        ulong num2;
        for (; num1 != 0UL; num1 = num2 & num2 + 1UL)
        {
          ushort num3 = checked ((ushort) ((long) BitUtils.LeastSignificantBit(num1) + (long) (uint) (64 * (index2 + rawChunk.Offset))));
          for (num2 = num1 | num1 - 1UL; index2 + 1 < rawChunk.Bitmap.Length && num2 == ulong.MaxValue; num2 = rawChunk.Bitmap[index2])
            ++index2;
          ushort num4 = num2 != ulong.MaxValue ? (ushort) (BitUtils.LeastSignificantBit(~num2) + 64 * (index2 + rawChunk.Offset) - (int) num3 - 1) : (ushort) (64 * (index2 + 1 + rawChunk.Offset) - (int) num3 - 1);
          runs[index1] = num3;
          runs[index1 + 1] = num4;
          index1 += 2;
        }
        ++index2;
        if (index2 < rawChunk.Bitmap.Length)
          num1 = rawChunk.Bitmap[index2];
        else
          break;
      }
      return new RunChunk(runs, true);
    }

    private static RunChunk CreateOptimizedRunChunk(int numRuns, RunChunk runChunk)
    {
      ushort[] runs = new ushort[2 * numRuns];
      for (int index = 0; index < 2 * numRuns; ++index)
        runs[index] = runChunk.Runs[index];
      return new RunChunk(runs, true);
    }
  }
}
