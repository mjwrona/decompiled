// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.FaultedQueryPipelineStage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Reactive;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline
{
  internal sealed class FaultedQueryPipelineStage : 
    IQueryPipelineStage,
    ITracingAsyncEnumerator<TryCatch<QueryPage>>,
    IAsyncDisposable
  {
    private readonly JustAsyncEnumerator<TryCatch<QueryPage>> justAsyncEnumerator;

    public FaultedQueryPipelineStage(Exception exception) => this.justAsyncEnumerator = exception != null ? new JustAsyncEnumerator<TryCatch<QueryPage>>(new TryCatch<QueryPage>[1]
    {
      TryCatch<QueryPage>.FromException(exception)
    }) : throw new ArgumentNullException(nameof (exception));

    public TryCatch<QueryPage> Current => this.justAsyncEnumerator.Current;

    public ValueTask DisposeAsync() => this.justAsyncEnumerator.DisposeAsync();

    public ValueTask<bool> MoveNextAsync(ITrace trace) => this.justAsyncEnumerator.MoveNextAsync(trace);

    public void SetCancellationToken(CancellationToken cancellationToken)
    {
    }
  }
}
