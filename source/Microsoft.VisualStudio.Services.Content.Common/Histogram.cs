// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Histogram
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
  [Serializable]
  public class Histogram
  {
    [JsonProperty]
    protected readonly long[] counts;

    public Histogram(int buckets) => this.counts = new long[buckets];

    public void IncrementCount(double value) => this.AddToCounts(value, 1L);

    public void AddToCounts(double value, long count)
    {
      if (count < 0L)
        throw new ArgumentOutOfRangeException();
      Interlocked.Add(ref this.counts[this.GetBucketIndex(value)], count);
    }

    protected virtual int GetBucketIndex(double value) => Math.Min(this.counts.Length - 1, Math.Max(0, (int) value));

    protected virtual Tuple<double, double> GetBucketRange(int bucketIndex)
    {
      double num1 = (double) bucketIndex;
      double num2 = (double) (bucketIndex + 1);
      if (bucketIndex == 0)
        num1 = double.MinValue;
      else if (bucketIndex == this.counts.Length - 1)
        num2 = double.MaxValue;
      return Tuple.Create<double, double>(num1, num2);
    }

    public SortedDictionary<Tuple<double, double>, long> GetCounts()
    {
      SortedDictionary<Tuple<double, double>, long> counts = new SortedDictionary<Tuple<double, double>, long>();
      foreach (int bucketIndex in Enumerable.Range(0, this.counts.Length))
        counts.Add(this.GetBucketRange(bucketIndex), Volatile.Read(ref this.counts[bucketIndex]));
      return counts;
    }

    public void MergeFrom(Histogram other)
    {
      if (this.counts.Length != other.counts.Length)
        throw new ArgumentException("Histograms have different count of buckets.");
      if (this.GetType() != other.GetType())
        throw new ArgumentException("Histograms are of different types.");
      for (int index = 0; index < this.counts.Length; ++index)
        this.counts[index] += other.counts[index];
    }

    public void Scale(double factor)
    {
      for (int index = 0; index < this.counts.Length; ++index)
        this.counts[index] = (long) ((double) this.counts[index] * factor);
    }
  }
}
