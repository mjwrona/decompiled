// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedIteratorInternal
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Tracing;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal abstract class FeedIteratorInternal : FeedIterator
  {
    public abstract CosmosElement GetCosmosElementContinuationToken();

    public static bool IsRetriableException(CosmosException cosmosException) => cosmosException.StatusCode == (HttpStatusCode) 429 || cosmosException.StatusCode == HttpStatusCode.RequestTimeout || cosmosException.StatusCode == HttpStatusCode.ServiceUnavailable;

    public abstract Task<ResponseMessage> ReadNextAsync(
      ITrace trace,
      CancellationToken cancellationToken);
  }
}
