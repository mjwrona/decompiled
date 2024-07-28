// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ServerBatchRequest
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.IO;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.RecordIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal abstract class ServerBatchRequest
  {
    private readonly int maxBodyLength;
    private readonly int maxOperationCount;
    private readonly CosmosSerializerCore serializerCore;
    private ArraySegment<ItemBatchOperation> operations;
    private MemorySpanResizer<byte> operationResizableWriteBuffer;
    private MemoryStream bodyStream;
    private long bodyStreamPositionBeforeWritingCurrentRecord;
    private bool shouldDeleteLastWrittenRecord;
    private int lastWrittenOperationIndex;

    protected ServerBatchRequest(
      int maxBodyLength,
      int maxOperationCount,
      CosmosSerializerCore serializerCore)
    {
      this.maxBodyLength = maxBodyLength;
      this.maxOperationCount = maxOperationCount;
      this.serializerCore = serializerCore;
    }

    public IReadOnlyList<ItemBatchOperation> Operations => (IReadOnlyList<ItemBatchOperation>) this.operations;

    public MemoryStream TransferBodyStream()
    {
      MemoryStream bodyStream = this.bodyStream;
      this.bodyStream = (MemoryStream) null;
      return bodyStream;
    }

    protected async Task<ArraySegment<ItemBatchOperation>> CreateBodyStreamAsync(
      ArraySegment<ItemBatchOperation> operations,
      CancellationToken cancellationToken,
      bool ensureContinuousOperationIndexes = false)
    {
      ServerBatchRequest serverBatchRequest = this;
      int estimatedMaxOperationLength = 0;
      int approximateTotalLength = 0;
      int num1 = -1;
      int materializedCount = 0;
      foreach (ItemBatchOperation operation1 in operations)
      {
        ItemBatchOperation operation = operation1;
        if (ensureContinuousOperationIndexes && num1 != -1)
        {
          if (operation.OperationIndex != num1 + 1)
            break;
        }
        await operation.MaterializeResourceAsync(serverBatchRequest.serializerCore, cancellationToken);
        ++materializedCount;
        num1 = operation.OperationIndex;
        int serializedLength = operation.GetApproximateSerializedLength();
        estimatedMaxOperationLength = Math.Max(serializedLength, estimatedMaxOperationLength);
        approximateTotalLength += serializedLength;
        if (approximateTotalLength <= serverBatchRequest.maxBodyLength)
        {
          if (materializedCount != serverBatchRequest.maxOperationCount)
            operation = (ItemBatchOperation) null;
          else
            break;
        }
        else
          break;
      }
      serverBatchRequest.operations = new ArraySegment<ItemBatchOperation>(operations.Array, operations.Offset, materializedCount);
      serverBatchRequest.bodyStream = new MemoryStream(approximateTotalLength + 200 * materializedCount);
      serverBatchRequest.operationResizableWriteBuffer = new MemorySpanResizer<byte>(estimatedMaxOperationLength + 200);
      int num2 = (int) await serverBatchRequest.bodyStream.WriteRecordIOAsync(new Segment(), new RecordIOStream.ProduceFunc(serverBatchRequest.WriteOperation));
      serverBatchRequest.bodyStream.Position = 0L;
      if (serverBatchRequest.shouldDeleteLastWrittenRecord)
      {
        serverBatchRequest.bodyStream.SetLength(serverBatchRequest.bodyStreamPositionBeforeWritingCurrentRecord);
        serverBatchRequest.operations = new ArraySegment<ItemBatchOperation>(operations.Array, operations.Offset, serverBatchRequest.lastWrittenOperationIndex);
      }
      else
        serverBatchRequest.operations = new ArraySegment<ItemBatchOperation>(operations.Array, operations.Offset, serverBatchRequest.lastWrittenOperationIndex + 1);
      int count = operations.Count - serverBatchRequest.operations.Count;
      return new ArraySegment<ItemBatchOperation>(operations.Array, serverBatchRequest.operations.Count + operations.Offset, count);
    }

    private Result WriteOperation(long index, out ReadOnlyMemory<byte> buffer)
    {
      if (this.bodyStream.Length > (long) this.maxBodyLength)
      {
        if (index > 1L)
          this.shouldDeleteLastWrittenRecord = true;
        buffer = new ReadOnlyMemory<byte>();
        return Result.Success;
      }
      this.bodyStreamPositionBeforeWritingCurrentRecord = this.bodyStream.Length;
      if (index >= (long) this.operations.Count)
      {
        buffer = new ReadOnlyMemory<byte>();
        return Result.Success;
      }
      ItemBatchOperation context = this.operations.Array[this.operations.Offset + (int) index];
      RowBuffer row = new RowBuffer(this.operationResizableWriteBuffer.Memory.Length, (ISpanResizer<byte>) this.operationResizableWriteBuffer);
      row.InitLayout(HybridRowVersion.V1, BatchSchemaProvider.BatchOperationLayout, (LayoutResolver) BatchSchemaProvider.BatchLayoutResolver);
      Result result = RowWriter.WriteBuffer<ItemBatchOperation>(ref row, context, new RowWriter.WriterFunc<ItemBatchOperation>(ItemBatchOperation.WriteOperation));
      if (result != Result.Success)
      {
        buffer = (ReadOnlyMemory<byte>) (byte[]) null;
        return result;
      }
      this.lastWrittenOperationIndex = (int) index;
      buffer = (ReadOnlyMemory<byte>) this.operationResizableWriteBuffer.Memory.Slice(0, row.Length);
      return Result.Success;
    }
  }
}
