// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Utilities.AtomicIntArray
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Threading;

namespace HdrHistogram.Utilities
{
  internal sealed class AtomicIntArray
  {
    private readonly int[] _counts;

    public AtomicIntArray(int arrayLength) => this._counts = new int[arrayLength];

    public int Length => this._counts.Length;

    public int this[int index]
    {
      get => this._counts[index];
      set => this.LazySet(index, value);
    }

    public int IncrementAndGet(int index) => Interlocked.Add(ref this._counts[index], 1);

    public int AddAndGet(int index, int value) => Interlocked.Add(ref this._counts[index], value);

    private void LazySet(int index, int value)
    {
      this._counts[index] = value;
      Interlocked.MemoryBarrier();
    }
  }
}
