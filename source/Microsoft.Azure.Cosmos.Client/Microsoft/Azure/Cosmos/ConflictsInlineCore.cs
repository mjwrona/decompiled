// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ConflictsInlineCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Telemetry;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class ConflictsInlineCore : ConflictsCore
  {
    internal ConflictsInlineCore(CosmosClientContext clientContext, ContainerInternal container)
      : base(clientContext, container)
    {
    }

    public override Task<ResponseMessage> DeleteAsync(
      ConflictProperties conflict,
      PartitionKey partitionKey,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ResponseMessage>(nameof (DeleteAsync), (RequestOptions) null, (Func<ITrace, Task<ResponseMessage>>) (trace => this.DeleteAsync(conflict, partitionKey, trace, cancellationToken)), (Func<ResponseMessage, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse(response)));
    }

    public override FeedIterator GetConflictQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorInlineCore(base.GetConflictQueryStreamIterator(queryText, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator<T> GetConflictQueryIterator<T>(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(base.GetConflictQueryIterator<T>(queryText, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator GetConflictQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator) new FeedIteratorInlineCore(base.GetConflictQueryStreamIterator(queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }

    public override FeedIterator<T> GetConflictQueryIterator<T>(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null)
    {
      return (FeedIterator<T>) new FeedIteratorInlineCore<T>(base.GetConflictQueryIterator<T>(queryDefinition, continuationToken, requestOptions), this.ClientContext);
    }

    public override Task<ItemResponse<T>> ReadCurrentAsync<T>(
      ConflictProperties cosmosConflict,
      PartitionKey partitionKey,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return this.ClientContext.OperationHelperAsync<ItemResponse<T>>(nameof (ReadCurrentAsync), (RequestOptions) null, (Func<ITrace, Task<ItemResponse<T>>>) (trace => this.ReadCurrentAsync<T>(cosmosConflict, partitionKey, trace, cancellationToken)), (Func<ItemResponse<T>, OpenTelemetryAttributes>) (response => (OpenTelemetryAttributes) new OpenTelemetryResponse<T>((Response<T>) response)));
    }

    public override T ReadConflictContent<T>(ConflictProperties cosmosConflict) => base.ReadConflictContent<T>(cosmosConflict);
  }
}
