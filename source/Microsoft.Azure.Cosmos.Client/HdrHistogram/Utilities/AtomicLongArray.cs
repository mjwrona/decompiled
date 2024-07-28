// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Utilities.AtomicLongArray
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Threading;

namespace HdrHistogram.Utilities
{
  internal sealed class AtomicLongArray
  {
    private readonly long[] _counts;

    public AtomicLongArray(int arrayLength) => this._counts = new long[arrayLength];

    public int Length => this._counts.Length;

    public long this[int index]
    {
      get => Interlocked.Read(ref this._counts[index]);
      set => this.LazySet(index, value);
    }

    public long IncrementAndGet(int index) => Interlocked.Add(ref this._counts[index], 1L);

    public long AddAndGet(int index, long value) => Interlocked.Add(ref this._counts[index], value);

    private void LazySet(int index, long value)
    {
      this._counts[index] = value;
      Interlocked.MemoryBarrier();
    }
  }
}
