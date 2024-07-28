// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.ResourceFeedReader`1
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Documents.Client
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

    public Task<FeedResponse<T>> ExecuteNextAsync(CancellationToken cancellationToken = default (CancellationToken)) => TaskHelper.InlineIfPossible<FeedResponse<T>>((Func<Task<FeedResponse<T>>>) (() => this.ExecuteNextAsyncInternal(cancellationToken)), (IRetryPolicy) null, cancellationToken);

    private Task<FeedResponse<T>> ExecuteNextAsyncInternal(CancellationToken cancellationToken) => this.documentQuery.ExecuteNextAsync<T>(cancellationToken);
  }
}
