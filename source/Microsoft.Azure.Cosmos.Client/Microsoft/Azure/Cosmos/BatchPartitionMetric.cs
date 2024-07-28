// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.BatchPartitionMetric
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class BatchPartitionMetric
  {
    public BatchPartitionMetric()
      : this(0L, 0L, 0L)
    {
    }

    public BatchPartitionMetric(
      long numberOfItemsOperatedOn,
      long timeTakenInMilliseconds,
      long numberOfThrottles)
    {
      if (numberOfItemsOperatedOn < 0L)
        throw new ArgumentException("numberOfItemsOperatedOn must be non negative");
      if (timeTakenInMilliseconds < 0L)
        throw new ArgumentException("timeTakenInMilliseconds must be non negative");
      if (numberOfThrottles < 0L)
        throw new ArgumentException("numberOfThrottles must be non negative");
      this.NumberOfItemsOperatedOn = numberOfItemsOperatedOn;
      this.TimeTakenInMilliseconds = timeTakenInMilliseconds;
      this.NumberOfThrottles = numberOfThrottles;
    }

    public long NumberOfItemsOperatedOn { get; private set; }

    public long TimeTakenInMilliseconds { get; private set; }

    public long NumberOfThrottles { get; private set; }

    public void Add(
      long numberOfDocumentsOperatedOn,
      long timeTakenInMilliseconds,
      long numberOfThrottles)
    {
      if (numberOfDocumentsOperatedOn < 0L)
        throw new ArgumentException("numberOfDocumentsOperatedOn must be non negative");
      if (timeTakenInMilliseconds < 0L)
        throw new ArgumentException("timeTakenInMilliseconds must be non negative");
      if (numberOfThrottles < 0L)
        throw new ArgumentException("numberOfThrottles must be non negative");
      this.NumberOfItemsOperatedOn += numberOfDocumentsOperatedOn;
      this.TimeTakenInMilliseconds += timeTakenInMilliseconds;
      this.NumberOfThrottles += numberOfThrottles;
    }
  }
}
