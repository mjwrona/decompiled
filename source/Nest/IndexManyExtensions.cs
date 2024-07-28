// Decompiled with JetBrains decompiler
// Type: Nest.IndexManyExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nest
{
  public static class IndexManyExtensions
  {
    public static BulkResponse IndexMany<T>(
      this IElasticClient client,
      IEnumerable<T> objects,
      IndexName index = null)
      where T : class
    {
      BulkRequest indexBulkRequest = IndexManyExtensions.CreateIndexBulkRequest<T>(objects, index);
      return client.Bulk((IBulkRequest) indexBulkRequest);
    }

    public static Task<BulkResponse> IndexManyAsync<T>(
      this IElasticClient client,
      IEnumerable<T> objects,
      IndexName index = null,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : class
    {
      BulkRequest indexBulkRequest = IndexManyExtensions.CreateIndexBulkRequest<T>(objects, index);
      return client.BulkAsync((IBulkRequest) indexBulkRequest, cancellationToken);
    }

    private static BulkRequest CreateIndexBulkRequest<T>(IEnumerable<T> objects, IndexName index) where T : class
    {
      objects.ThrowIfEmpty<T>(nameof (objects));
      BulkRequest indexBulkRequest = new BulkRequest(index);
      List<IBulkOperation> list = objects.Select<T, BulkIndexOperation<T>>((Func<T, BulkIndexOperation<T>>) (o => new BulkIndexOperation<T>(o))).Cast<IBulkOperation>().ToList<IBulkOperation>();
      indexBulkRequest.Operations = new BulkOperationsCollection<IBulkOperation>((IEnumerable<IBulkOperation>) list);
      return indexBulkRequest;
    }
  }
}
