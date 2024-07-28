// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.SkipEmptyPageQueryPipelineStage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Query.Core.QueryClient;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline
{
  internal sealed class SkipEmptyPageQueryPipelineStage : 
    IQueryPipelineStage,
    ITracingAsyncEnumerator<TryCatch<QueryPage>>,
    IAsyncDisposable
  {
    private static readonly IReadOnlyList<CosmosElement> EmptyPage = (IReadOnlyList<CosmosElement>) new List<CosmosElement>();
    private readonly IQueryPipelineStage inputStage;
    private double cumulativeRequestCharge;
    private long cumulativeResponseLengthInBytes;
    private IReadOnlyDictionary<string, string> cumulativeAdditionalHeaders;
    private CancellationToken cancellationToken;
    private bool returnedFinalStats;

    public SkipEmptyPageQueryPipelineStage(
      IQueryPipelineStage inputStage,
      CancellationToken cancellationToken)
    {
      this.inputStage = inputStage ?? throw new ArgumentNullException(nameof (inputStage));
      this.cancellationToken = cancellationToken;
    }

    public TryCatch<QueryPage> Current { get; private set; }

    public ValueTask DisposeAsync() => this.inputStage.DisposeAsync();

    public async ValueTask<bool> MoveNextAsync(ITrace trace)
    {
      this.cancellationToken.ThrowIfCancellationRequested();
      if (trace == null)
        throw new ArgumentNullException(nameof (trace));
      int num = 0;
      while (num == 0)
      {
        if (!await this.inputStage.MoveNextAsync(trace))
        {
          if (!this.returnedFinalStats)
          {
            QueryPage result = new QueryPage(SkipEmptyPageQueryPipelineStage.EmptyPage, this.cumulativeRequestCharge, Guid.Empty.ToString(), this.cumulativeResponseLengthInBytes, (Lazy<CosmosQueryExecutionInfo>) null, (string) null, this.cumulativeAdditionalHeaders, (QueryState) null);
            this.cumulativeRequestCharge = 0.0;
            this.cumulativeResponseLengthInBytes = 0L;
            this.cumulativeAdditionalHeaders = (IReadOnlyDictionary<string, string>) null;
            this.returnedFinalStats = true;
            this.Current = TryCatch<QueryPage>.FromResult(result);
            return true;
          }
          this.Current = new TryCatch<QueryPage>();
          return false;
        }
        TryCatch<QueryPage> current = this.inputStage.Current;
        if (current.Failed)
        {
          this.Current = current;
          return true;
        }
        QueryPage result1 = current.Result;
        num = result1.Documents.Count;
        if (num == 0)
        {
          if (result1.State == null)
          {
            QueryPage result2 = new QueryPage(SkipEmptyPageQueryPipelineStage.EmptyPage, result1.RequestCharge + this.cumulativeRequestCharge, result1.ActivityId, result1.ResponseLengthInBytes + this.cumulativeResponseLengthInBytes, result1.CosmosQueryExecutionInfo, result1.DisallowContinuationTokenMessage, result1.AdditionalHeaders, (QueryState) null);
            this.cumulativeRequestCharge = 0.0;
            this.cumulativeResponseLengthInBytes = 0L;
            this.cumulativeAdditionalHeaders = (IReadOnlyDictionary<string, string>) null;
            this.Current = TryCatch<QueryPage>.FromResult(result2);
            return true;
          }
          this.cumulativeRequestCharge += result1.RequestCharge;
          this.cumulativeResponseLengthInBytes += result1.ResponseLengthInBytes;
          this.cumulativeAdditionalHeaders = result1.AdditionalHeaders;
        }
        else
        {
          QueryPage result3;
          if (this.cumulativeRequestCharge != 0.0)
          {
            result3 = new QueryPage(result1.Documents, result1.RequestCharge + this.cumulativeRequestCharge, result1.ActivityId, result1.ResponseLengthInBytes + this.cumulativeResponseLengthInBytes, result1.CosmosQueryExecutionInfo, result1.DisallowContinuationTokenMessage, result1.AdditionalHeaders, result1.State);
            this.cumulativeRequestCharge = 0.0;
            this.cumulativeResponseLengthInBytes = 0L;
            this.cumulativeAdditionalHeaders = (IReadOnlyDictionary<string, string>) null;
          }
          else
            result3 = result1;
          this.Current = TryCatch<QueryPage>.FromResult(result3);
        }
      }
      return true;
    }

    public void SetCancellationToken(CancellationToken cancellationToken) => this.cancellationToken = cancellationToken;
  }
}
