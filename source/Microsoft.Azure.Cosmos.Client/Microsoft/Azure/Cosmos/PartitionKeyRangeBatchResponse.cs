// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.PartitionKeyRangeBatchResponse
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Microsoft.Azure.Cosmos
{
  internal class PartitionKeyRangeBatchResponse : TransactionalBatchResponse
  {
    private readonly TransactionalBatchOperationResult[] resultsByOperationIndex;
    private readonly TransactionalBatchResponse serverResponse;
    private bool isDisposed;

    internal PartitionKeyRangeBatchResponse(
      int originalOperationsCount,
      TransactionalBatchResponse serverResponse,
      CosmosSerializerCore serializerCore)
    {
      this.StatusCode = serverResponse.StatusCode;
      this.serverResponse = serverResponse;
      this.resultsByOperationIndex = new TransactionalBatchOperationResult[originalOperationsCount];
      StringBuilder stringBuilder = new StringBuilder();
      List<ItemBatchOperation> itemBatchOperationList = new List<ItemBatchOperation>();
      for (int index = 0; index < serverResponse.Operations.Count; ++index)
      {
        int operationIndex = serverResponse.Operations[index].OperationIndex;
        if (this.resultsByOperationIndex[operationIndex] == null || this.resultsByOperationIndex[operationIndex].StatusCode == (HttpStatusCode) 429)
          this.resultsByOperationIndex[operationIndex] = serverResponse[index];
      }
      itemBatchOperationList.AddRange((IEnumerable<ItemBatchOperation>) serverResponse.Operations);
      this.Headers = serverResponse.Headers;
      if (!string.IsNullOrEmpty(serverResponse.ErrorMessage))
        stringBuilder.AppendFormat("{0}; ", (object) serverResponse.ErrorMessage);
      this.ErrorMessage = stringBuilder.Length > 2 ? stringBuilder.ToString(0, stringBuilder.Length - 2) : (string) null;
      this.Operations = (IReadOnlyList<ItemBatchOperation>) itemBatchOperationList;
      this.SerializerCore = serializerCore;
    }

    public override string ActivityId => this.serverResponse.ActivityId;

    public override CosmosDiagnostics Diagnostics => this.serverResponse.Diagnostics;

    internal override CosmosSerializerCore SerializerCore { get; }

    public override int Count => this.resultsByOperationIndex.Length;

    public override TransactionalBatchOperationResult this[int index] => this.resultsByOperationIndex[index];

    public override TransactionalBatchOperationResult<T> GetOperationResultAtIndex<T>(int index)
    {
      TransactionalBatchOperationResult result = index < this.Count ? this.resultsByOperationIndex[index] : throw new IndexOutOfRangeException();
      T resource = default (T);
      if (result.ResourceStream != null)
        resource = this.SerializerCore.FromStream<T>(result.ResourceStream);
      return new TransactionalBatchOperationResult<T>(result, resource);
    }

    public override IEnumerator<TransactionalBatchOperationResult> GetEnumerator()
    {
      TransactionalBatchOperationResult[] batchOperationResultArray = this.resultsByOperationIndex;
      for (int index = 0; index < batchOperationResultArray.Length; ++index)
        yield return batchOperationResultArray[index];
      batchOperationResultArray = (TransactionalBatchOperationResult[]) null;
    }

    internal override IEnumerable<string> GetActivityIds() => (IEnumerable<string>) new string[1]
    {
      this.ActivityId
    };

    protected override void Dispose(bool disposing)
    {
      if (disposing && !this.isDisposed)
      {
        this.isDisposed = true;
        this.serverResponse?.Dispose();
      }
      base.Dispose(disposing);
    }
  }
}
