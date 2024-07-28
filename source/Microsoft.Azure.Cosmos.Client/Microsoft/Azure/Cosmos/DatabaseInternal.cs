// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.DatabaseInternal
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal abstract class DatabaseInternal : Database
  {
    internal abstract string LinkUri { get; }

    internal abstract CosmosClientContext ClientContext { get; }

    internal abstract Task<ThroughputResponse> ReadThroughputIfExistsAsync(
      RequestOptions requestOptions,
      CancellationToken cancellationToken = default (CancellationToken));

    internal abstract Task<ThroughputResponse> ReplaceThroughputIfExistsAsync(
      int throughput,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    internal abstract Task<ThroughputResponse> ReplaceThroughputPropertiesIfExistsAsync(
      ThroughputProperties throughputProperties,
      RequestOptions requestOptions = null,
      CancellationToken cancellationToken = default (CancellationToken));

    internal abstract Task<string> GetRIDAsync(CancellationToken cancellationToken = default (CancellationToken));

    public abstract FeedIterator GetUserQueryStreamIterator(
      QueryDefinition queryDefinition,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);

    public abstract FeedIterator GetUserQueryStreamIterator(
      string queryText = null,
      string continuationToken = null,
      QueryRequestOptions requestOptions = null);
  }
}
