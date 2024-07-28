// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.GitRepoRenameOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class GitRepoRenameOperation : AbstractIndexingOperation
  {
    public GitRepoRenameOperation(
      CoreIndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base((ExecutionContext) executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080628, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (!this.ValidateCIFeatureFlags(executionContext))
        {
          operationResult.Status = OperationStatus.Succeeded;
          return operationResult;
        }
        operationResult.Status = this.CreateDependantOperations(executionContext, resultMessage);
      }
      finally
      {
        operationResult.Message = resultMessage.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080628, "Indexing Pipeline", "IndexingOperation", nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual OperationStatus CreateDependantOperations(
      IndexingExecutionContext executionContext,
      StringBuilder resultMessage)
    {
      if (this.ValidateRenameOperationShouldProceed(executionContext))
      {
        DocumentContractType contractType = executionContext.ProvisioningContext.ContractType;
        if (AbstractSearchDocumentContract.CreateContract(contractType).IsSourceEnabled(executionContext.RequestContext) && (contractType.IsDedupeFileContract() || contractType.IsSourceNoDedupeFileContract() && executionContext.RequestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/SourceEnabledInNoPayloadMapping", TeamFoundationHostType.Deployment, true)))
          this.QueueDependantOperationsForRepositoryFieldUpdate(executionContext, resultMessage);
        else
          this.CreateDependentOperationsForRepositoryRename(executionContext, resultMessage);
      }
      return OperationStatus.Succeeded;
    }

    internal virtual bool ValidateRenameOperationShouldProceed(
      IndexingExecutionContext executionContext)
    {
      string str = this.FetchGitRepositoryNameFromTfs(executionContext);
      string repositoryName = this.IndexingUnit.TFSEntityAttributes is GitCodeRepoTFSAttributes entityAttributes ? entityAttributes.RepositoryName : (string) null;
      string name = this.IndexingUnit.Properties?.Name;
      bool? nullable1 = repositoryName?.Equals(str, StringComparison.Ordinal);
      bool? nullable2 = name?.Equals(str, StringComparison.Ordinal);
      if (!nullable1.HasValue || !nullable1.Value || !nullable2.HasValue || !nullable2.Value)
        return true;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080628, "Indexing Pipeline", "IndexingOperation", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Repo name '{0}' is already processed. Doing Nothing", (object) repositoryName));
      return false;
    }

    internal virtual void CreateDependentOperationsForRepositoryRename(
      IndexingExecutionContext executionContext,
      StringBuilder resultMessage)
    {
      string newRepoName = this.FetchGitRepositoryNameFromTfs(executionContext);
      string repositoryName = this.IndexingUnit.TFSEntityAttributes is GitCodeRepoTFSAttributes entityAttributes ? entityAttributes.RepositoryName : (string) null;
      this.IndexingUnit.Properties.Name = newRepoName;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080628, "Indexing Pipeline", "IndexingOperation", FormattableString.Invariant(FormattableStringFactory.Create("Erasing Indexing watermarks for {0}", (object) this.IndexingUnit)));
      if (!this.IndexingUnit.EraseIndexingWatermarksOfTree(executionContext, this.IndexingUnitDataAccess))
        this.IndexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, this.IndexingUnit);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent repoBulkIndexEvent1 = this.CreateRepoBulkIndexEvent(executionContext);
      resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Successfully created BulkIndex operation with Id {0} for repository with {1} because of Repository rename from {2} to {3}.", (object) repoBulkIndexEvent1.Id, (object) this.IndexingUnit.ToString(), (object) repositoryName, (object) newRepoName);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent repoBulkIndexEvent2 = this.CreateDeleteOrphanFilesEventDependingOnStatusOfRepoBulkIndexEvent(executionContext, repoBulkIndexEvent1, repositoryName);
      resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Created deleteOrphanFilesChangeEvent with Id {0} operation for repository with {1} because of Repository rename.", (object) repoBulkIndexEvent2.Id, (object) this.IndexingUnit.ToString());
      this.QueueRepoRenameFinalizeOperation(executionContext, resultMessage, repoBulkIndexEvent1, repositoryName, newRepoName);
    }

    private void QueueRepoRenameFinalizeOperation(
      IndexingExecutionContext executionContext,
      StringBuilder resultMessage,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent ChangeEvent,
      string oldRepoName,
      string newRepoName)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent renameFinalizeEvent = this.CreateRepoRenameFinalizeEvent(executionContext, ChangeEvent);
      resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Successfully created rename finalize operation with Id {0} for repository with {1} because of Repository rename from {2} to {3}.", (object) renameFinalizeEvent.Id, (object) this.IndexingUnit.ToString(), (object) oldRepoName, (object) newRepoName);
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent CreateDeleteOrphanFilesEventDependingOnStatusOfRepoBulkIndexEvent(
      IndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent repoBulkIndexEvent,
      string oldRepoName)
    {
      IndexingUnitChangeEventPrerequisites eventPrerequisites = new IndexingUnitChangeEventPrerequisites();
      eventPrerequisites.Add(new IndexingUnitChangeEventPrerequisitesFilter()
      {
        Id = repoBulkIndexEvent.Id,
        Operator = IndexingUnitChangeEventFilterOperator.Contains,
        PossibleStates = new List<IndexingUnitChangeEventState>()
        {
          IndexingUnitChangeEventState.Succeeded
        }
      });
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = repoBulkIndexEvent.IndexingUnitId,
        ChangeType = "DeleteOrphanFiles",
        ChangeData = (ChangeEventData) new CodeDeleteOrphanFilesEventData((ExecutionContext) executionContext)
        {
          OldEntityName = oldRepoName,
          UnitType = "Git_Repository"
        },
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0,
        Prerequisites = eventPrerequisites
      };
      return this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) executionContext, indexingUnitChangeEvent);
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent CreateRepoBulkIndexEvent(
      IndexingExecutionContext executionContext)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.IndexingUnitChangeEvent.LeaseId)
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeType = this.IndexingUnit.IsLargeRepository(executionContext.RequestContext) ? "UpdateIndex" : "BeginBulkIndex",
        ChangeData = (ChangeEventData) new CodeBulkIndexEventData((ExecutionContext) executionContext),
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      return this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) executionContext, indexingUnitChangeEvent);
    }

    internal virtual void QueueDependantOperationsForRepositoryFieldUpdate(
      IndexingExecutionContext executionContext,
      StringBuilder resultMessage)
    {
      string str = "UpdateField";
      string newRepoName = this.FetchGitRepositoryNameFromTfs(executionContext);
      string repositoryName = this.IndexingUnit.TFSEntityAttributes is GitCodeRepoTFSAttributes entityAttributes ? entityAttributes.RepositoryName : (string) null;
      this.IndexingUnit.Properties.Name = newRepoName;
      this.IndexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, this.IndexingUnit);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(this.IndexingUnitChangeEvent.LeaseId)
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeType = str,
        ChangeData = (ChangeEventData) new EntityRenameEventData((ExecutionContext) executionContext)
        {
          FieldUpdateEventIndexingUnitType = "Git_Repository"
        },
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent ChangeEvent1 = this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) executionContext, indexingUnitChangeEvent);
      if (executionContext.IndexingUnit.IsLargeRepository(executionContext.RequestContext))
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent ChangeEvent2 = this.FinalizeRepositoryIndexing(executionContext, ChangeEvent1, resultMessage);
        this.QueueRepoRenameFinalizeOperation(executionContext, resultMessage, ChangeEvent2, repositoryName, newRepoName);
      }
      else
        this.QueueRepoRenameFinalizeOperation(executionContext, resultMessage, ChangeEvent1, repositoryName, newRepoName);
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent FinalizeRepositoryIndexing(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent ChangeEvent,
      StringBuilder resultMessage)
    {
      IndexingUnitChangeEventPrerequisites eventPrerequisites = new IndexingUnitChangeEventPrerequisites();
      eventPrerequisites.Add(new IndexingUnitChangeEventPrerequisitesFilter()
      {
        Id = ChangeEvent.Id,
        Operator = IndexingUnitChangeEventFilterOperator.Contains,
        PossibleStates = new List<IndexingUnitChangeEventState>()
        {
          IndexingUnitChangeEventState.Succeeded
        }
      });
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = indexingExecutionContext.CollectionIndexingUnit.IndexingUnitId,
        ChangeData = (ChangeEventData) new CodeIndexPublishData((ExecutionContext) indexingExecutionContext),
        ChangeType = "CompleteBulkIndex",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0,
        Prerequisites = eventPrerequisites
      };
      return this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent);
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent CreateRepoRenameFinalizeEvent(
      IndexingExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent repoBulkIndexEvent)
    {
      IndexingUnitChangeEventPrerequisites eventPrerequisites = new IndexingUnitChangeEventPrerequisites();
      eventPrerequisites.Add(new IndexingUnitChangeEventPrerequisitesFilter()
      {
        Id = repoBulkIndexEvent.Id,
        Operator = IndexingUnitChangeEventFilterOperator.Contains,
        PossibleStates = new List<IndexingUnitChangeEventState>()
        {
          IndexingUnitChangeEventState.Succeeded,
          IndexingUnitChangeEventState.Failed
        }
      });
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeType = "CompleteEntityRename",
        ChangeData = new ChangeEventData((ExecutionContext) executionContext),
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0,
        Prerequisites = eventPrerequisites
      };
      return this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) executionContext, indexingUnitChangeEvent);
    }

    internal virtual string FetchGitRepositoryNameFromTfs(
      IndexingExecutionContext indexingExecutionContext)
    {
      return new GitHttpClientWrapper((ExecutionContext) indexingExecutionContext, new TraceMetaData(1080628, "Indexing Pipeline", "IndexingOperation")).GetRepositoryAsync(this.IndexingUnit.TFSEntityId).Name;
    }
  }
}
