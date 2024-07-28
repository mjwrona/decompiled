// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.CollectionCodeGitRepositoryAcesSyncOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class CollectionCodeGitRepositoryAcesSyncOperation : AbstractIndexingOperation
  {
    [StaticSafe]
    private static TraceMetaData s_traceMetaData = new TraceMetaData(1080633, "Indexing Pipeline", "IndexingOperation");

    public CollectionCodeGitRepositoryAcesSyncOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(CoreIndexingExecutionContext executionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(CollectionCodeGitRepositoryAcesSyncOperation.s_traceMetaData, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder stringBuilder = new StringBuilder();
      GitRepoSyncAnalyzer repoSyncAnalyzer = new GitRepoSyncAnalyzerFactory().GetGitRepoSyncAnalyzer((ExecutionContext) executionContext, CollectionCodeGitRepositoryAcesSyncOperation.s_traceMetaData, this.IndexingUnitChangeEventHandler, this.IndexingUnit.EntityType);
      try
      {
        int num = repoSyncAnalyzer.SyncReposSecHashInCollection();
        operationResult.Status = OperationStatus.Succeeded;
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "CollectionCodeGitRepositoryAcesSyncOperation updated security hash for {0} GIT repositories. ", (object) num);
      }
      finally
      {
        operationResult.Message = stringBuilder.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(CollectionCodeGitRepositoryAcesSyncOperation.s_traceMetaData, nameof (RunOperation));
      }
      return operationResult;
    }

    protected internal override void HandleOperationFailure(
      IndexingExecutionContext indexingExecutionContext,
      OperationResult result,
      Exception e)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("PeriodicCatchUpJobFailedAtGitSecurityHashSync", "Indexing Pipeline", 1.0);
      result.Message = e.ToString();
      result.Status = OperationStatus.Failed;
    }
  }
}
