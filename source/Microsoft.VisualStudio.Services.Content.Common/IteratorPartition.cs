// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.IteratorPartition
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class IteratorPartition
  {
    private readonly int partitionId;
    private readonly int totalPartitions;
    private readonly PartitionStrategy partitionStrategy;

    public IteratorPartition(
      int partitionId,
      int totalPartitions,
      PartitionStrategy partitionStrategy = PartitionStrategy.ExactMultiple)
    {
      if (totalPartitions < 1)
        throw new ArgumentException(string.Format("Total partitions size: ({0}) should be greater than 1", (object) totalPartitions));
      this.partitionId = partitionId < totalPartitions ? partitionId : throw new ArgumentException(string.Format("Partition number: ({0}) should be less than total partition size: ({1})", (object) partitionId, (object) totalPartitions));
      this.totalPartitions = totalPartitions;
      this.partitionStrategy = partitionStrategy;
    }

    private int GetPartitionSize(int iteratorCount)
    {
      switch (this.partitionStrategy)
      {
        case PartitionStrategy.ExactMultiple:
          if (iteratorCount % this.totalPartitions != 0)
            throw new ArgumentException(string.Format("Invalid number of partitions, The total number of iterators {0} is not exact multiple of number of partitions: {1}", (object) iteratorCount, (object) this.totalPartitions));
          break;
        case PartitionStrategy.ExactOneToOne:
          if (iteratorCount != this.totalPartitions)
            throw new ArgumentException(string.Format("Invalid number of partitions, The total number of iterators {0} is not same as number of partitions: {1}", (object) iteratorCount, (object) this.totalPartitions));
          break;
        case PartitionStrategy.All:
          return iteratorCount;
        default:
          throw new InvalidEnumArgumentException("Invalid partitioning strategy specified.");
      }
      int num = (iteratorCount + this.totalPartitions - 1) / this.totalPartitions;
      return num >= 1 ? num : throw new ArgumentException(string.Format("Invalid number of partitions, Count: {0}, PartitionCount: {1}", (object) iteratorCount, (object) this.totalPartitions));
    }

    public override string ToString() => string.Format("IteratorPartition({0}, {1})", (object) this.partitionId, (object) this.totalPartitions);

    public IEnumerable<T> SelectIterators<T>(IEnumerable<T> iterators)
    {
      if (this.partitionStrategy == PartitionStrategy.All || this.totalPartitions <= 1)
        return iterators;
      int partitionSize = this.GetPartitionSize(iterators.Count<T>());
      iterators = iterators.Skip<T>(partitionSize * this.partitionId).Take<T>(partitionSize);
      return iterators;
    }
  }
}
