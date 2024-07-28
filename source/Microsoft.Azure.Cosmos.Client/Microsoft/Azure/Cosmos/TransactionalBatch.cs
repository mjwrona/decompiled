// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.TransactionalBatch
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  public abstract class TransactionalBatch
  {
    public abstract TransactionalBatch CreateItem<T>(
      T item,
      TransactionalBatchItemRequestOptions requestOptions = null);

    public abstract TransactionalBatch CreateItemStream(
      Stream streamPayload,
      TransactionalBatchItemRequestOptions requestOptions = null);

    public abstract TransactionalBatch ReadItem(
      string id,
      TransactionalBatchItemRequestOptions requestOptions = null);

    public abstract TransactionalBatch UpsertItem<T>(
      T item,
      TransactionalBatchItemRequestOptions requestOptions = null);

    public abstract TransactionalBatch UpsertItemStream(
      Stream streamPayload,
      TransactionalBatchItemRequestOptions requestOptions = null);

    public abstract TransactionalBatch ReplaceItem<T>(
      string id,
      T item,
      TransactionalBatchItemRequestOptions requestOptions = null);

    public abstract TransactionalBatch ReplaceItemStream(
      string id,
      Stream streamPayload,
      TransactionalBatchItemRequestOptions requestOptions = null);

    public abstract TransactionalBatch DeleteItem(
      string id,
      TransactionalBatchItemRequestOptions requestOptions = null);

    public abstract TransactionalBatch PatchItem(
      string id,
      IReadOnlyList<PatchOperation> patchOperations,
      TransactionalBatchPatchItemRequestOptions requestOptions = null);

    public abstract Task<TransactionalBatchResponse> ExecuteAsync(
      CancellationToken cancellationToken = default (CancellationToken));

    public abstract Task<TransactionalBatchResponse> ExecuteAsync(
      TransactionalBatchRequestOptions requestOptions,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
