// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.GitBranchDeleteOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common.Git;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class GitBranchDeleteOperation : AbstractIndexingOperation
  {
    public GitBranchDeleteOperation(
      CoreIndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base((ExecutionContext) indexingExecutionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Tracer.TraceEnter(1080624, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      bool currentHostConfigValue = coreIndexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/IsGitBranchDeleteOperationDisabled", true);
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (!currentHostConfigValue && this.ValidateCIFeatureFlags(executionContext))
        {
          GitBranchDeleteEventData changeData = (GitBranchDeleteEventData) this.IndexingUnitChangeEvent.ChangeData;
          List<string> branchesToDeleteList = new List<string>();
          GitCodeRepoTFSAttributes repoAttributes = this.IndexingUnit.TFSEntityAttributes as GitCodeRepoTFSAttributes;
          GitCodeRepoIndexingProperties properties = this.IndexingUnit.Properties as GitCodeRepoIndexingProperties;
          if (changeData.Branches != null)
            branchesToDeleteList.AddRange((IEnumerable<string>) changeData.Branches.ToList<string>().FindAll((Predicate<string>) (x => !string.IsNullOrWhiteSpace(x))));
          else
            branchesToDeleteList.AddRange((IEnumerable<string>) properties.BranchIndexInfo.Keys.ToList<string>().FindAll((Predicate<string>) (x => !repoAttributes.BranchesToIndex.Contains(x))));
          if (branchesToDeleteList.Count > 0)
          {
            operationResult.Status = this.DeleteBranches(executionContext, branchesToDeleteList, changeData.CleanUpIndexingUnitData, resultMessage);
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
          resultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Branch Deletion Operation is disabled for {0}", (object) this.IndexingUnit.ToString())));
        }
      }
      finally
      {
        operationResult.Message = resultMessage.ToString();
        Tracer.TraceLeave(1080624, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual OperationStatus DeleteBranches(
      IndexingExecutionContext indexingExecutionContext,
      List<string> branchesToDeleteList,
      bool cleanUpIndexingUnitData,
      StringBuilder resultMessage)
    {
      IndexIdentity index = indexingExecutionContext.GetIndex();
      return GitBranchDeleterFactory.GetGitBranchDeleter(indexingExecutionContext).PerformBranchDeletion(indexingExecutionContext, this.IndexingUnitChangeEventHandler, branchesToDeleteList, index, this.IndexingUnit, cleanUpIndexingUnitData, this.IndexingUnitChangeEvent.LeaseId, resultMessage);
    }
  }
}
