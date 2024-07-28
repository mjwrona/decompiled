// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.QueryPipelineStageBase
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
  internal abstract class QueryPipelineStageBase : 
    IQueryPipelineStage,
    ITracingAsyncEnumerator<TryCatch<QueryPage>>,
    IAsyncDisposable
  {
    protected readonly IQueryPipelineStage inputStage;
    protected CancellationToken cancellationToken;

    protected QueryPipelineStageBase(
      IQueryPipelineStage inputStage,
      CancellationToken cancellationToken)
    {
      this.inputStage = inputStage ?? throw new ArgumentNullException(nameof (inputStage));
      this.cancellationToken = cancellationToken;
    }

    public TryCatch<QueryPage> Current { get; protected set; }

    public ValueTask DisposeAsync() => this.inputStage.DisposeAsync();

    public abstract ValueTask<bool> MoveNextAsync(ITrace trace);

    public void SetCancellationToken(CancellationToken cancellationToken)
    {
      this.cancellationToken = cancellationToken;
      this.inputStage.SetCancellationToken(cancellationToken);
    }
  }
}
