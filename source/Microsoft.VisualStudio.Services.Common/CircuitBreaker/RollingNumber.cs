// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CircuitBreaker.RollingNumber
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Threading;

namespace Microsoft.VisualStudio.Services.CircuitBreaker
{
  public class RollingNumber : IRollingNumber
  {
    private readonly object newBucketLock = new object();
    private readonly ITime time;
    private readonly int bucketSizeInMilliseconds;
    private readonly int numberOfBuckets;
    private RollingNumber.Bucket[] buckets;
    private int CurrentBucketIndex;

    internal RollingNumber(ITime time, int timeInMilliseconds, int numberOfBuckets)
    {
      if (timeInMilliseconds % numberOfBuckets != 0)
        throw new ArgumentException("The timeInMilliseconds must divide equally into numberOfBuckets. For example 1000/10 is ok, 1000/11 is not.");
      this.time = time;
      this.bucketSizeInMilliseconds = timeInMilliseconds / numberOfBuckets;
      this.numberOfBuckets = numberOfBuckets;
      this.buckets = new RollingNumber.Bucket[numberOfBuckets];
    }

    public int GetTotalWindowTimeInMilliseconds() => this.numberOfBuckets * this.bucketSizeInMilliseconds;

    public int GetNumberOfBuckets() => this.numberOfBuckets;

    public int GetBucketSizeInMilliseconds() => this.bucketSizeInMilliseconds;

    internal RollingNumber.Bucket[] Buckets => this.buckets;

    public void Reset()
    {
      long currentTimeInMillis = this.time.GetCurrentTimeInMillis();
      for (int index = 0; index < this.numberOfBuckets; ++index)
      {
        this.buckets[index].windowStart = currentTimeInMillis;
        this.buckets[index].count = 0L;
        currentTimeInMillis += (long) this.bucketSizeInMilliseconds;
      }
    }

    public long GetRollingSum()
    {
      this.GetCurrentBucketIndex();
      long rollingSum = 0;
      for (int index = 0; index < this.numberOfBuckets; ++index)
        rollingSum += this.buckets[index].count;
      return rollingSum;
    }

    public void Increment() => Interlocked.Increment(ref this.buckets[this.GetCurrentBucketIndex()].count);

    internal int GetCurrentBucketIndex()
    {
      long currentTimeInMillis = this.time.GetCurrentTimeInMillis();
      if (currentTimeInMillis < this.buckets[this.CurrentBucketIndex].windowStart + (long) this.bucketSizeInMilliseconds)
        return this.CurrentBucketIndex;
      if (!Monitor.TryEnter(this.newBucketLock))
        return this.CurrentBucketIndex;
      try
      {
        if (currentTimeInMillis - (this.buckets[this.CurrentBucketIndex].windowStart + (long) (this.bucketSizeInMilliseconds * this.numberOfBuckets)) > (long) (this.bucketSizeInMilliseconds * this.numberOfBuckets))
        {
          long num = currentTimeInMillis;
          for (int index = 0; index < this.numberOfBuckets; ++index)
          {
            this.buckets[index].windowStart = num;
            this.buckets[index].count = 0L;
            num += (long) this.bucketSizeInMilliseconds;
          }
          this.CurrentBucketIndex = 0;
          return this.CurrentBucketIndex;
        }
        int num1 = (int) ((currentTimeInMillis - this.buckets[this.CurrentBucketIndex].windowStart) / (long) this.bucketSizeInMilliseconds);
        long windowStart = this.buckets[this.CurrentBucketIndex].windowStart;
        int index1 = this.CurrentBucketIndex;
        for (int index2 = 0; index2 < num1; ++index2)
        {
          windowStart += (long) this.bucketSizeInMilliseconds;
          int num2;
          index1 = (num2 = index1 + 1) % this.numberOfBuckets;
          this.buckets[index1].windowStart = windowStart;
          this.buckets[index1].count = 0L;
        }
        this.CurrentBucketIndex = index1;
        return this.CurrentBucketIndex;
      }
      finally
      {
        Monitor.Exit(this.newBucketLock);
      }
    }

    internal struct Bucket
    {
      internal long windowStart;
      internal long count;
    }
  }
}
