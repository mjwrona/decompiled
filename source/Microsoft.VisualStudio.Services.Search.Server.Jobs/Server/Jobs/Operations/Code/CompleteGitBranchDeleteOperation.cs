// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.CompleteGitBranchDeleteOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common.Git;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class CompleteGitBranchDeleteOperation : AbstractIndexingOperation
  {
    public CompleteGitBranchDeleteOperation(
      CoreIndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base((ExecutionContext) indexingExecutionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Tracer.TraceEnter(1083048, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      bool currentHostConfigValue = coreIndexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/IsGitBranchDeleteOperationDisabled", true);
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (!this.ValidateCIFeatureFlags(executionContext))
        {
          operationResult.Status = OperationStatus.Succeeded;
          return operationResult;
        }
        if (!currentHostConfigValue)
        {
          CompleteGitBranchDeleteEventData changeData = this.IndexingUnitChangeEvent.ChangeData as CompleteGitBranchDeleteEventData;
          List<string> stringList = new List<string>();
          if (changeData != null)
          {
            List<string> branches = changeData.Branches;
            if (branches.Count > 0)
            {
              operationResult.Status = this.CompleteBranchDeletion(executionContext, branches, changeData.CleanUpIndexingUnitData, resultMessage);
            }
            else
            {
              operationResult.Status = OperationStatus.Succeeded;
              resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("There are no branches to be deleted from IndexingUnit {0} CollectionId {1}.", (object) this.IndexingUnit.ToString(), (object) coreIndexingExecutionContext.RequestContext.GetCollectionID())));
            }
          }
          else
          {
            operationResult.Status = OperationStatus.Succeeded;
            resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("There is no CompleteGitBranchDeleteEventData present for IndexingUnit {0} CollectionId {1}.", (object) this.IndexingUnit.ToString(), (object) coreIndexingExecutionContext.RequestContext.GetCollectionID())));
          }
        }
        else
        {
          operationResult.Status = OperationStatus.Succeeded;
          resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Complete Branch Deletion Operation is disabled as GitBranchDeletion is disabled for {0}", (object) this.IndexingUnit.ToString())));
        }
      }
      finally
      {
        operationResult.Message = resultMessage.ToString();
        Tracer.TraceLeave(1083048, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual OperationStatus CompleteBranchDeletion(
      IndexingExecutionContext indexingExecutionContext,
      List<string> branchesList,
      bool cleanUpIndexingUnitData,
      StringBuilder resultMessage)
    {
      IndexIdentity index = indexingExecutionContext.GetIndex();
      return GitBranchDeleterFactory.GetGitBranchDeleter(indexingExecutionContext).CompleteBranchDeletion(indexingExecutionContext, this.IndexingUnitDataAccess, branchesList, index, this.IndexingUnit, cleanUpIndexingUnitData, resultMessage);
    }
  }
}
