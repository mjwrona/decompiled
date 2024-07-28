// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.BlobIdsForPartition
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class BlobIdsForPartition
  {
    public readonly byte Partition;
    public readonly BlobIdentifier MinValue;
    public readonly BlobIdentifier MaxValue;
    public readonly BlobIdentifier StartValue;
    public static readonly IEnumerable<int> SupportedNumberOfBlobIdPartitions = (IEnumerable<int>) new int[9]
    {
      1,
      2,
      4,
      8,
      16,
      32,
      64,
      128,
      256
    };

    public static void ValidateBlobIdPartitionSize(int blobIdPartitions, int totalPartitions)
    {
      if (blobIdPartitions == 0)
        return;
      int val2 = BlobIdsForPartition.SupportedNumberOfBlobIdPartitions.Last<int>();
      if (blobIdPartitions < 0)
        throw new InvalidPartitionSizeException(string.Format("Invalid BlobIdPartitions: {0}. It should be non-negative.", (object) blobIdPartitions));
      if (!BlobIdsForPartition.SupportedNumberOfBlobIdPartitions.Contains<int>(blobIdPartitions))
        throw new InvalidPartitionSizeException(string.Format("Invalid BlobIdPartitions: {0}. It should be one of {1}", (object) blobIdPartitions, (object) string.Join<int>(", ", BlobIdsForPartition.SupportedNumberOfBlobIdPartitions)));
      if (blobIdPartitions > Math.Min(totalPartitions, val2))
        throw new InvalidPartitionSizeException(string.Format("Invalid BlobIdPartitions: {0}. ", (object) blobIdPartitions) + string.Format("It should be less than or equal to both TotalPartitions: {0} and maxSubPartitionsPerIterator: {1}", (object) totalPartitions, (object) val2));
      if (totalPartitions % blobIdPartitions != 0)
        throw new InvalidPartitionSizeException(string.Format("Invalid BlobIdPartitions: {0}. ", (object) blobIdPartitions) + string.Format("TotalPartitions: {0} should be divisible by BlobIdPartitions.", (object) totalPartitions));
    }

    private BlobIdsForPartition(
      byte partition,
      BlobIdentifier minValue,
      BlobIdentifier maxValue,
      BlobIdentifier startValue)
    {
      this.Partition = partition;
      this.MinValue = minValue;
      this.MaxValue = maxValue;
      if (startValue == (BlobIdentifier) null || string.Compare(startValue.ValueString, this.MinValue.ValueString, StringComparison.OrdinalIgnoreCase) < 0 || string.Compare(startValue.ValueString, this.MaxValue.ValueString, StringComparison.OrdinalIgnoreCase) > 0)
        throw new ArgumentException(string.Format("Invalid start blobId: {0}, MinValue: {1}, MaxValue: {2}", (object) startValue, (object) this.MinValue.ValueString, (object) this.MaxValue.ValueString));
      this.StartValue = startValue;
    }

    public static BlobIdsForPartition Create(
      byte partition,
      int totalPartitions,
      BlobIdentifier startId)
    {
      if ((int) partition > totalPartitions)
        throw new InvalidPartitionSizeException(string.Format("Blob partition ({0}) should be within range 0 to {1}", (object) partition, (object) totalPartitions));
      byte[] algorithmResultBytes1 = BlobIdentifier.MinValue.AlgorithmResultBytes;
      byte[] algorithmResultBytes2 = BlobIdentifier.MaxValue.AlgorithmResultBytes;
      BlobIdentifier blobIdentifier = startId;
      if ((object) blobIdentifier == null)
        blobIdentifier = BlobIdentifier.Random();
      byte[] algorithmResultBytes3 = blobIdentifier.AlgorithmResultBytes;
      int num = 256 / totalPartitions;
      algorithmResultBytes1[0] = (byte) ((uint) partition * (uint) num);
      algorithmResultBytes2[0] = (byte) ((int) partition * num | num - 1);
      if (startId == (BlobIdentifier) null || startId == BlobIdentifier.MinValue)
        algorithmResultBytes3[0] = (byte) ((int) partition * num | (int) algorithmResultBytes3[0] & num - 1);
      return new BlobIdsForPartition(partition, BlobIdentifier.CreateFromAlgorithmResult(algorithmResultBytes1), BlobIdentifier.CreateFromAlgorithmResult(algorithmResultBytes2), BlobIdentifier.CreateFromAlgorithmResult(algorithmResultBytes3));
    }

    public override string ToString() => string.Format("Partition: {0}, Range: {1} - {2}, StartValue: {3}", (object) this.Partition, (object) this.MinValue.ValueString.Substring(0, 12), (object) this.MaxValue.ValueString.Substring(0, 12), (object) this.StartValue.ValueString.Substring(0, 12));

    public int CalculateJobPerMille(BlobIdentifier currentId)
    {
      if (currentId == (BlobIdentifier) null || this.MinValue == this.MaxValue)
        return 0;
      long num1 = (long) (this.GetBlobIdNumericValue(currentId) - this.GetBlobIdNumericValue(this.StartValue));
      long num2 = (long) (this.GetBlobIdNumericValue(this.MaxValue) - this.GetBlobIdNumericValue(this.MinValue));
      return (int) ((num1 * 1000L + num2 / 2L) / num2) + (currentId.CompareTo((object) this.StartValue) < 0 ? 1000 : 0);
    }

    private int GetBlobIdNumericValue(BlobIdentifier blobId)
    {
      byte[] bytes = blobId.Bytes;
      return ((int) bytes[0] << 16) + ((int) bytes[1] << 8) + (int) bytes[2];
    }
  }
}
