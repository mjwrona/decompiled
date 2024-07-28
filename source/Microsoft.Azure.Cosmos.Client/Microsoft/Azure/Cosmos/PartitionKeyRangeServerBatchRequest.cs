// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.PartitionKeyRangeServerBatchRequest
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class PartitionKeyRangeServerBatchRequest : ServerBatchRequest
  {
    public PartitionKeyRangeServerBatchRequest(
      string partitionKeyRangeId,
      bool isClientEncrypted,
      string intendedCollectionRidValue,
      int maxBodyLength,
      int maxOperationCount,
      CosmosSerializerCore serializerCore)
      : base(maxBodyLength, maxOperationCount, serializerCore)
    {
      this.PartitionKeyRangeId = partitionKeyRangeId;
      this.IsClientEncrypted = isClientEncrypted;
      this.IntendedCollectionRidValue = intendedCollectionRidValue;
    }

    public string PartitionKeyRangeId { get; }

    public bool IsClientEncrypted { get; }

    public string IntendedCollectionRidValue { get; }

    public static async Task<Tuple<PartitionKeyRangeServerBatchRequest, ArraySegment<ItemBatchOperation>>> CreateAsync(
      string partitionKeyRangeId,
      ArraySegment<ItemBatchOperation> operations,
      int maxBodyLength,
      int maxOperationCount,
      bool ensureContinuousOperationIndexes,
      CosmosSerializerCore serializerCore,
      bool isClientEncrypted,
      string intendedCollectionRidValue,
      CancellationToken cancellationToken)
    {
      PartitionKeyRangeServerBatchRequest request = new PartitionKeyRangeServerBatchRequest(partitionKeyRangeId, isClientEncrypted, intendedCollectionRidValue, maxBodyLength, maxOperationCount, serializerCore);
      Tuple<PartitionKeyRangeServerBatchRequest, ArraySegment<ItemBatchOperation>> async = new Tuple<PartitionKeyRangeServerBatchRequest, ArraySegment<ItemBatchOperation>>(request, await request.CreateBodyStreamAsync(operations, cancellationToken, ensureContinuousOperationIndexes));
      request = (PartitionKeyRangeServerBatchRequest) null;
      return async;
    }
  }
}
