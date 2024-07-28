// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ItemBatchOperation
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.Handlers;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.IO;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal class ItemBatchOperation : IDisposable
  {
    protected Memory<byte> body;
    private bool isDisposed;
    private readonly CosmosClientContext ClientContext;

    public ItemBatchOperation(
      OperationType operationType,
      int operationIndex,
      Microsoft.Azure.Cosmos.PartitionKey partitionKey,
      string id = null,
      Stream resourceStream = null,
      TransactionalBatchItemRequestOptions requestOptions = null,
      CosmosClientContext cosmosClientContext = null)
    {
      this.OperationType = operationType;
      this.OperationIndex = operationIndex;
      this.PartitionKey = new Microsoft.Azure.Cosmos.PartitionKey?(partitionKey);
      this.Id = id;
      this.ResourceStream = resourceStream;
      this.RequestOptions = requestOptions;
      this.ClientContext = cosmosClientContext;
    }

    public ItemBatchOperation(
      OperationType operationType,
      int operationIndex,
      ContainerInternal containerCore,
      string id = null,
      Stream resourceStream = null,
      TransactionalBatchItemRequestOptions requestOptions = null)
    {
      this.OperationType = operationType;
      this.OperationIndex = operationIndex;
      this.ContainerInternal = containerCore;
      this.Id = id;
      this.ResourceStream = resourceStream;
      this.RequestOptions = requestOptions;
      this.ClientContext = containerCore.ClientContext;
    }

    public Microsoft.Azure.Cosmos.PartitionKey? PartitionKey { get; internal set; }

    public string Id { get; }

    public OperationType OperationType { get; }

    public Stream ResourceStream { get; protected set; }

    public TransactionalBatchItemRequestOptions RequestOptions { get; }

    public int OperationIndex { get; internal set; }

    internal ContainerInternal ContainerInternal { get; }

    internal string PartitionKeyJson { get; set; }

    internal Microsoft.Azure.Documents.PartitionKey ParsedPartitionKey { get; set; }

    internal Memory<byte> ResourceBody
    {
      get => this.body;
      set => this.body = value;
    }

    internal ItemBatchOperationContext Context { get; private set; }

    internal ITrace Trace { get; set; }

    public void Dispose() => this.Dispose(true);

    internal static Result WriteOperation(
      ref RowWriter writer,
      TypeArgument typeArg,
      ItemBatchOperation operation)
    {
      bool pkWritten = false;
      Result result1 = writer.WriteInt32(UtfAnyString.op_Implicit("operationType"), (int) operation.OperationType);
      if (result1 != Result.Success)
        return result1;
      Result result2 = writer.WriteInt32(UtfAnyString.op_Implicit("resourceType"), 2);
      if (result2 != Result.Success)
        return result2;
      if (operation.PartitionKeyJson != null)
      {
        Result result3 = writer.WriteString(UtfAnyString.op_Implicit("partitionKey"), operation.PartitionKeyJson);
        if (result3 != Result.Success)
          return result3;
        pkWritten = true;
      }
      if (operation.Id != null)
      {
        Result result4 = writer.WriteString(UtfAnyString.op_Implicit("id"), operation.Id);
        if (result4 != Result.Success)
          return result4;
      }
      Memory<byte> resourceBody = operation.ResourceBody;
      if (!resourceBody.IsEmpty)
      {
        ref RowWriter local = ref writer;
        UtfAnyString path = UtfAnyString.op_Implicit("resourceBody");
        resourceBody = operation.ResourceBody;
        ReadOnlySpan<byte> span = (ReadOnlySpan<byte>) resourceBody.Span;
        Result result5 = local.WriteBinary(path, span);
        if (result5 != Result.Success)
          return result5;
      }
      if (operation.RequestOptions != null)
      {
        TransactionalBatchItemRequestOptions requestOptions = operation.RequestOptions;
        if (requestOptions.IndexingDirective.HasValue)
        {
          string str = IndexingDirectiveStrings.FromIndexingDirective(requestOptions.IndexingDirective.Value);
          Result result6 = writer.WriteString(UtfAnyString.op_Implicit("indexingDirective"), str);
          if (result6 != Result.Success)
            return result6;
        }
        if (requestOptions.IfMatchEtag != null)
        {
          Result result7 = writer.WriteString(UtfAnyString.op_Implicit("ifMatch"), requestOptions.IfMatchEtag);
          if (result7 != Result.Success)
            return result7;
        }
        else if (requestOptions.IfNoneMatchEtag != null)
        {
          Result result8 = writer.WriteString(UtfAnyString.op_Implicit("ifNoneMatch"), requestOptions.IfNoneMatchEtag);
          if (result8 != Result.Success)
            return result8;
        }
        Result result9 = requestOptions.WriteRequestProperties(ref writer, pkWritten);
        if (result9 != Result.Success)
          return result9;
      }
      if (RequestInvokerHandler.ShouldSetNoContentResponseHeaders((Microsoft.Azure.Cosmos.RequestOptions) operation.RequestOptions, operation.ClientContext?.ClientOptions, operation.OperationType, ResourceType.Document))
      {
        Result result10 = writer.WriteBool(UtfAnyString.op_Implicit("minimalReturnPreference"), true);
        if (result10 != Result.Success)
          return result10;
      }
      return Result.Success;
    }

    internal int GetApproximateSerializedLength()
    {
      int num = 0;
      if (this.PartitionKeyJson != null)
        num += this.PartitionKeyJson.Length;
      if (this.Id != null)
        num += this.Id.Length;
      int serializedLength = num + this.body.Length;
      if (this.RequestOptions != null)
      {
        if (this.RequestOptions.IfMatchEtag != null)
          serializedLength += this.RequestOptions.IfMatchEtag.Length;
        if (this.RequestOptions.IfNoneMatchEtag != null)
          serializedLength += this.RequestOptions.IfNoneMatchEtag.Length;
        if (this.RequestOptions.IndexingDirective.HasValue)
          serializedLength += 7;
        serializedLength += this.RequestOptions.GetRequestPropertiesSerializationLength();
      }
      return serializedLength;
    }

    internal virtual async Task MaterializeResourceAsync(
      CosmosSerializerCore serializerCore,
      CancellationToken cancellationToken)
    {
      if (!this.body.IsEmpty || this.ResourceStream == null)
        return;
      this.body = await BatchExecUtils.StreamToMemoryAsync(this.ResourceStream, cancellationToken);
    }

    internal void AttachContext(ItemBatchOperationContext context) => this.Context = this.Context == null ? context : throw new InvalidOperationException("Cannot modify the current context of an operation.");

    protected virtual void Dispose(bool disposing)
    {
      if (!disposing || this.isDisposed)
        return;
      this.isDisposed = true;
      if (this.ResourceStream == null)
        return;
      this.ResourceStream.Dispose();
      this.ResourceStream = (Stream) null;
    }
  }
}
