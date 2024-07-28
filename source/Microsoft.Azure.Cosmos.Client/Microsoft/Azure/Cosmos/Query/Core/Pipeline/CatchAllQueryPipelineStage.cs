// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.CatchAllQueryPipelineStage
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Query.Core.Pipeline.Pagination;
using Microsoft.Azure.Cosmos.Tracing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline
{
  internal sealed class CatchAllQueryPipelineStage : QueryPipelineStageBase
  {
    public CatchAllQueryPipelineStage(
      IQueryPipelineStage inputStage,
      CancellationToken cancellationToken)
      : base(inputStage, cancellationToken)
    {
    }

    public override async ValueTask<bool> MoveNextAsync(ITrace trace)
    {
      CatchAllQueryPipelineStage queryPipelineStage = this;
      int num;
      if (num != 0 && trace == null)
        throw new ArgumentNullException(nameof (trace));
      try
      {
        if (!await queryPipelineStage.inputStage.MoveNextAsync(trace))
        {
          queryPipelineStage.Current = new TryCatch<QueryPage>();
          return false;
        }
        queryPipelineStage.Current = queryPipelineStage.inputStage.Current;
        return true;
      }
      catch (Exception ex)
      {
        ITrace trace1 = trace;
        CosmosException cosmosException;
        ref CosmosException local = ref cosmosException;
        if (!ExceptionToCosmosException.TryCreateFromException(ex, trace1, out local))
        {
          throw;
        }
        else
        {
          queryPipelineStage.Current = TryCatch<QueryPage>.FromException((Exception) cosmosException);
          return true;
        }
      }
    }
  }
}
