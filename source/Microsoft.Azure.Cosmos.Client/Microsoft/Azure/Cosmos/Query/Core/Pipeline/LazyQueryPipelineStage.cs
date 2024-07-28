// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.LazyQueryPipelineStage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline
{
  internal sealed class LazyQueryPipelineStage : 
    IQueryPipelineStage,
    ITracingAsyncEnumerator<TryCatch<QueryPage>>,
    IAsyncDisposable
  {
    private readonly AsyncLazy<TryCatch<IQueryPipelineStage>> lazyTryCreateStage;
    private CancellationToken cancellationToken;

    public LazyQueryPipelineStage(
      AsyncLazy<TryCatch<IQueryPipelineStage>> lazyTryCreateStage,
      CancellationToken cancellationToken)
    {
      this.lazyTryCreateStage = lazyTryCreateStage ?? throw new ArgumentNullException(nameof (lazyTryCreateStage));
      this.cancellationToken = cancellationToken;
    }

    public TryCatch<QueryPage> Current { get; private set; }

    public ValueTask DisposeAsync()
    {
      if (this.lazyTryCreateStage.ValueInitialized)
      {
        TryCatch<IQueryPipelineStage> result = this.lazyTryCreateStage.Result;
        if (result.Succeeded)
          return result.Result.DisposeAsync();
      }
      return new ValueTask();
    }

    public async ValueTask<bool> MoveNextAsync(ITrace trace)
    {
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      TryCatch<IQueryPipelineStage> valueAsync = await this.lazyTryCreateStage.GetValueAsync(trace, this.cancellationToken);
      if (valueAsync.Failed)
      {
        this.Current = TryCatch<QueryPage>.FromException(valueAsync.Exception);
        return true;
      }
      IQueryPipelineStage stage = valueAsync.Result;
      stage.SetCancellationToken(this.cancellationToken);
      if (!await stage.MoveNextAsync(trace))
      {
        this.Current = new TryCatch<QueryPage>();
        return false;
      }
      this.Current = stage.Current;
      return true;
    }

    public void SetCancellationToken(CancellationToken cancellationToken) => this.cancellationToken = cancellationToken;
  }
}
