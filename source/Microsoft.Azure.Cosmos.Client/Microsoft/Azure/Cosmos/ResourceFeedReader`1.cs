// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ResourceFeedReader`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Azure.Documents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class ResourceFeedReader<T> : IEnumerable<T>, IEnumerable where T : JsonSerializable, new()
  {
    private readonly DocumentQuery<T> documentQuery;

    internal ResourceFeedReader(
      DocumentClient client,
      ResourceType resourceType,
      FeedOptions options,
      string resourceLink,
      object partitionKey = null)
    {
      this.documentQuery = new DocumentQuery<T>(client, resourceType, typeof (T), resourceLink, options, partitionKey);
    }

    public bool HasMoreResults => this.documentQuery.HasMoreResults;

    public IEnumerator<T> GetEnumerator() => this.documentQuery.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.documentQuery.GetEnumerator();

    public Task<DocumentFeedResponse<T>> ExecuteNextAsync(CancellationToken cancellationToken = default (CancellationToken)) => TaskHelper.InlineIfPossible<DocumentFeedResponse<T>>((Func<Task<DocumentFeedResponse<T>>>) (() => this.InternalExecuteNextAsync(cancellationToken)), (IRetryPolicy) null, cancellationToken);

    private async Task<DocumentFeedResponse<T>> InternalExecuteNextAsync(
      CancellationToken cancellationToken)
    {
      return await this.documentQuery.ExecuteNextAsync<T>(cancellationToken);
    }
  }
}
