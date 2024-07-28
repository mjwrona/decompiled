// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.EmptyQueryPipelineStage
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
  internal sealed class EmptyQueryPipelineStage : 
    IQueryPipelineStage,
    ITracingAsyncEnumerator<TryCatch<QueryPage>>,
    IAsyncDisposable
  {
    public static readonly EmptyQueryPipelineStage Singleton = new EmptyQueryPipelineStage();
    private readonly EmptyAsyncEnumerator<TryCatch<QueryPage>> emptyAsyncEnumerator;

    public EmptyQueryPipelineStage() => this.emptyAsyncEnumerator = new EmptyAsyncEnumerator<TryCatch<QueryPage>>();

    public TryCatch<QueryPage> Current => this.emptyAsyncEnumerator.Current;

    public ValueTask DisposeAsync() => this.emptyAsyncEnumerator.DisposeAsync();

    public ValueTask<bool> MoveNextAsync(ITrace trace) => this.emptyAsyncEnumerator.MoveNextAsync(trace);

    public void SetCancellationToken(CancellationToken cancellationToken)
    {
    }
  }
}
