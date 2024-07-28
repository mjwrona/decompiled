// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.SinglePartitionKeyServerBatchRequest
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class SinglePartitionKeyServerBatchRequest : ServerBatchRequest
  {
    private SinglePartitionKeyServerBatchRequest(
      Microsoft.Azure.Cosmos.PartitionKey? partitionKey,
      CosmosSerializerCore serializerCore)
      : base(int.MaxValue, int.MaxValue, serializerCore)
    {
      this.PartitionKey = partitionKey;
    }

    public Microsoft.Azure.Cosmos.PartitionKey? PartitionKey { get; }

    public static async Task<SinglePartitionKeyServerBatchRequest> CreateAsync(
      Microsoft.Azure.Cosmos.PartitionKey? partitionKey,
      ArraySegment<ItemBatchOperation> operations,
      CosmosSerializerCore serializerCore,
      ITrace trace,
      CancellationToken cancellationToken)
    {
      SinglePartitionKeyServerBatchRequest async;
      using (trace.StartChild("Create Batch Request", TraceComponent.Batch, TraceLevel.Info))
      {
        SinglePartitionKeyServerBatchRequest request = new SinglePartitionKeyServerBatchRequest(partitionKey, serializerCore);
        ArraySegment<ItemBatchOperation> bodyStreamAsync = await request.CreateBodyStreamAsync(operations, cancellationToken);
        async = request;
      }
      return async;
    }
  }
}
