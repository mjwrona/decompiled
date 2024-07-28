// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.GitRepositoryCodeCustomBulkIndexingOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.ChangeEventData;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Api;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class GitRepositoryCodeCustomBulkIndexingOperation : AbstractIndexingOperation
  {
    internal GitRepositoryCodeCustomBulkIndexingOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Tracer.TraceEnter(1080613, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (!executionContext.IsIndexingEnabled())
        {
          executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Indexing is disabled, bailing out.")));
          operationResult.Status = OperationStatus.Succeeded;
          return operationResult;
        }
        GitRepositoryCustomBulkIndexingEventData changeEventData = this.IndexingUnitChangeEvent.ChangeData as GitRepositoryCustomBulkIndexingEventData;
        GitCodeRepoTFSAttributes entityAttributes = executionContext.RepositoryIndexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes;
        if (changeEventData != null)
        {
          List<string> stringList = entityAttributes.Branches ?? new List<string>();
          bool flag = false;
          if (changeEventData.BranchesToIndex != null && changeEventData.BranchesToIndex.Any<string>())
          {
            stringList.AddRange((IEnumerable<string>) changeEventData.BranchesToIndex);
            flag = true;
          }
          if (changeEventData.BranchesToDelete != null && changeEventData.BranchesToDelete.Any<string>())
          {
            stringList.RemoveAll((Predicate<string>) (x => changeEventData.BranchesToDelete.Contains(x)));
            flag = true;
          }
          if (flag)
          {
            entityAttributes.Branches = stringList;
            executionContext.RepositoryIndexingUnit = this.IndexingUnitDataAccess.UpdateIndexingUnit(coreIndexingExecutionContext.RequestContext, executionContext.RepositoryIndexingUnit);
            this.GetGitRepoSyncAnalyzer((ExecutionContext) coreIndexingExecutionContext).QueueGitRepoPatchOperation((ExecutionContext) coreIndexingExecutionContext, executionContext.RepositoryIndexingUnit, changeEventData.BranchesToIndex);
          }
          if (changeEventData.DefaultBranchChanged)
            coreIndexingExecutionContext.RequestContext.QueuePeriodicCatchUpJob(0);
        }
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        Tracer.TraceLeave(1080613, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual GitRepoSyncAnalyzer GetGitRepoSyncAnalyzer(ExecutionContext executionContext) => new GitRepoSyncAnalyzerFactory().GetGitRepoSyncAnalyzer(executionContext, new TraceMetaData(1080613, "Indexing Pipeline", "IndexingOperation"), this.IndexingUnitChangeEventHandler, (IEntityType) CodeEntityType.GetInstance());
  }
}
