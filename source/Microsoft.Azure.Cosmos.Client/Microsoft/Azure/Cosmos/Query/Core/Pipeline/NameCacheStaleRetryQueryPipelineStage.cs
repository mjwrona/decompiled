// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.NameCacheStaleRetryQueryPipelineStage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.QueryClient;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline
{
  internal sealed class NameCacheStaleRetryQueryPipelineStage : 
    IQueryPipelineStage,
    ITracingAsyncEnumerator<TryCatch<QueryPage>>,
    IAsyncDisposable
  {
    private readonly CosmosQueryContext cosmosQueryContext;
    private readonly Func<IQueryPipelineStage> queryPipelineStageFactory;
    private IQueryPipelineStage currentQueryPipelineStage;
    private bool alreadyRetried;

    public NameCacheStaleRetryQueryPipelineStage(
      CosmosQueryContext cosmosQueryContext,
      Func<IQueryPipelineStage> queryPipelineStageFactory)
    {
      this.cosmosQueryContext = cosmosQueryContext ?? throw new ArgumentNullException(nameof (cosmosQueryContext));
      this.queryPipelineStageFactory = queryPipelineStageFactory ?? throw new ArgumentNullException(nameof (queryPipelineStageFactory));
      this.currentQueryPipelineStage = queryPipelineStageFactory();
    }

    public TryCatch<QueryPage> Current { get; private set; }

    public ValueTask DisposeAsync() => this.currentQueryPipelineStage.DisposeAsync();

    public async ValueTask<bool> MoveNextAsync(ITrace trace)
    {
      if (!await this.currentQueryPipelineStage.MoveNextAsync(trace))
        return false;
      TryCatch<QueryPage> current = this.currentQueryPipelineStage.Current;
      this.Current = current;
      if (!current.Failed || (!(current.InnerMostException is CosmosException innerMostException) || innerMostException.StatusCode != HttpStatusCode.Gone || innerMostException.SubStatusCode != 1000 ? 0 : (!this.alreadyRetried ? 1 : 0)) == 0)
        return true;
      await this.cosmosQueryContext.QueryClient.ForceRefreshCollectionCacheAsync(this.cosmosQueryContext.ResourceLink, new CancellationToken());
      this.alreadyRetried = true;
      await this.currentQueryPipelineStage.DisposeAsync();
      this.currentQueryPipelineStage = this.queryPipelineStageFactory();
      return await this.MoveNextAsync(trace);
    }

    public void SetCancellationToken(CancellationToken cancellationToken) => this.currentQueryPipelineStage.SetCancellationToken(cancellationToken);
  }
}
