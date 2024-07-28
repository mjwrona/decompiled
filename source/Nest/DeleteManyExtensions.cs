// Decompiled with JetBrains decompiler
// Type: Nest.DeleteManyExtensions
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
  public static class DeleteManyExtensions
  {
    public static BulkResponse DeleteMany<T>(
      this IElasticClient client,
      IEnumerable<T> objects,
      IndexName index = null)
      where T : class
    {
      BulkRequest deleteBulkRequest = DeleteManyExtensions.CreateDeleteBulkRequest<T>(objects, index);
      return client.Bulk((IBulkRequest) deleteBulkRequest);
    }

    public static Task<BulkResponse> DeleteManyAsync<T>(
      this IElasticClient client,
      IEnumerable<T> objects,
      IndexName index = null,
      CancellationToken cancellationToken = default (CancellationToken))
      where T : class
    {
      BulkRequest deleteBulkRequest = DeleteManyExtensions.CreateDeleteBulkRequest<T>(objects, index);
      return client.BulkAsync((IBulkRequest) deleteBulkRequest, cancellationToken);
    }

    private static BulkRequest CreateDeleteBulkRequest<T>(IEnumerable<T> objects, IndexName index) where T : class
    {
      objects.ThrowIfEmpty<T>(nameof (objects));
      BulkRequest deleteBulkRequest = new BulkRequest(index);
      List<IBulkOperation> list = objects.Select<T, BulkDeleteOperation<T>>((Func<T, BulkDeleteOperation<T>>) (o => new BulkDeleteOperation<T>(o))).Cast<IBulkOperation>().ToList<IBulkOperation>();
      deleteBulkRequest.Operations = new BulkOperationsCollection<IBulkOperation>((IEnumerable<IBulkOperation>) list);
      return deleteBulkRequest;
    }
  }
}
