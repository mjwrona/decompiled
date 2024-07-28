// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ItemBatchOperation`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal class ItemBatchOperation<T> : ItemBatchOperation
  {
    public ItemBatchOperation(
      OperationType operationType,
      int operationIndex,
      Microsoft.Azure.Cosmos.PartitionKey partitionKey,
      T resource,
      string id = null,
      TransactionalBatchItemRequestOptions requestOptions = null,
      CosmosClientContext cosmosClientContext = null)
      : base(operationType, operationIndex, partitionKey, id, requestOptions: requestOptions, cosmosClientContext: cosmosClientContext)
    {
      this.Resource = resource;
    }

    public ItemBatchOperation(
      OperationType operationType,
      int operationIndex,
      T resource,
      ContainerInternal containerCore,
      string id = null,
      TransactionalBatchItemRequestOptions requestOptions = null)
      : base(operationType, operationIndex, containerCore, id, requestOptions: requestOptions)
    {
      this.Resource = resource;
    }

    public T Resource { get; private set; }

    internal override Task MaterializeResourceAsync(
      CosmosSerializerCore serializerCore,
      CancellationToken cancellationToken)
    {
      if (!this.body.IsEmpty || (object) this.Resource == null)
        return Task.CompletedTask;
      this.ResourceStream = serializerCore.ToStream<T>(this.Resource);
      return base.MaterializeResourceAsync(serializerCore, cancellationToken);
    }
  }
}
