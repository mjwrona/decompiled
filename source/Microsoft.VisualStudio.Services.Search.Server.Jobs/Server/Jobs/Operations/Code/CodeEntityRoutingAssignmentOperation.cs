// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.CodeEntityRoutingAssignmentOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.WebApi;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class CodeEntityRoutingAssignmentOperation : BasicRoutingAssignmentOperation
  {
    private readonly TraceMetaData m_traceMetaData;

    public CodeEntityRoutingAssignmentOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.m_traceMetaData = new TraceMetaData(1080292, "Indexing Pipeline", "IndexingOperation");
    }

    protected override EntityFinalizerBase FinalizeHelper => (EntityFinalizerBase) new CollectionCodeFinalizeHelper();

    internal override IndexingUnitWithSize GetIndexingUnitWithSizeEstimates(
      IndexingExecutionContext indexingExecutionContext)
    {
      switch (indexingExecutionContext.IndexingUnit.IndexingUnitType)
      {
        case "Git_Repository":
          return this.GetGitRepoSizeEstimates(indexingExecutionContext);
        case "TFVC_Repository":
          return this.GetTfvcRepoSizeEstimates(indexingExecutionContext);
        case "CustomRepository":
          return this.GetCustomRepoSizeEstimates(indexingExecutionContext);
        case "ScopedIndexingUnit":
          return this.GetScopedPathFileEstimates(indexingExecutionContext);
        default:
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("{0} is supported only for {1} and IndexingUnitTypes :{2},", (object) "AssignRouting", (object) CodeEntityType.GetInstance(), (object) "Git_Repository")) + FormattableString.Invariant(FormattableStringFactory.Create(" {0}, {1}, {2}", (object) "TFVC_Repository", (object) "CustomRepository", (object) "ScopedIndexingUnit")));
      }
    }

    internal virtual IndexingUnitWithSize GetGitRepoSizeEstimates(
      IndexingExecutionContext indexingExecutionContext)
    {
      GitHttpClientWrapper httpClientWrapper = this.GetGitHttpClientWrapper(indexingExecutionContext);
      int shardDensity1 = indexingExecutionContext.ProvisioningContext.ContractType.GetShardDensity(indexingExecutionContext.RequestContext);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingExecutionContext.IndexingUnit;
      int branchCount = indexingUnit.GetBranchCountFromTFSAttributesIfGitRepo();
      if (branchCount <= 0)
        branchCount = 1;
      double factorForMultibranch = indexingExecutionContext.ProvisioningContext.ContractType.GetFileCountFactorForMultibranch(indexingExecutionContext.RequestContext, indexingUnit, branchCount);
      IVssRequestContext requestContext = indexingExecutionContext.RequestContext;
      int shardDensity2 = shardDensity1;
      int numberOfBranches = branchCount;
      double fileCountFactorForMultibranch = factorForMultibranch;
      int currentEstimatedDocumentCount;
      ref int local1 = ref currentEstimatedDocumentCount;
      int estimatedGrowth;
      ref int local2 = ref estimatedGrowth;
      long num1;
      ref long local3 = ref num1;
      long num2;
      ref long local4 = ref num2;
      long num3;
      ref long local5 = ref num3;
      bool repoSizeEstimates = httpClientWrapper.GetGitRepoSizeEstimates(requestContext, shardDensity2, numberOfBranches, fileCountFactorForMultibranch, out local1, out local2, out local3, out local4, out local5);
      return new IndexingUnitWithSize(indexingExecutionContext.IndexingUnit, currentEstimatedDocumentCount, estimatedGrowth, repoSizeEstimates)
      {
        ActualInitialSize = num3
      };
    }

    internal virtual IndexingUnitWithSize GetTfvcRepoSizeEstimates(
      IndexingExecutionContext indexingExecutionContext)
    {
      Guid tfsEntityId = indexingExecutionContext.IndexingUnit.TFSEntityId;
      TfvcHttpClientWrapper httpClientWrapper = this.GetTfvcHttpClientWrapper(indexingExecutionContext);
      TeamProject withCapabilities = this.GetProjectHttpClientWrapper(indexingExecutionContext).GetTeamProjectWithCapabilities(tfsEntityId.ToString());
      IVssRequestContext requestContext = indexingExecutionContext.RequestContext;
      int contractType = (int) indexingExecutionContext.ProvisioningContext.ContractType;
      Guid projectId = tfsEntityId;
      string scopePath = "$/" + withCapabilities.Name;
      int currentEstimatedDocumentCount;
      ref int local1 = ref currentEstimatedDocumentCount;
      int estimatedGrowth;
      ref int local2 = ref estimatedGrowth;
      httpClientWrapper.GetDocumentCountEstimates(requestContext, (DocumentContractType) contractType, projectId, scopePath, out local1, out local2);
      return new IndexingUnitWithSize(indexingExecutionContext.IndexingUnit, currentEstimatedDocumentCount, estimatedGrowth, true)
      {
        ActualInitialDocCount = currentEstimatedDocumentCount
      };
    }

    internal virtual IndexingUnitWithSize GetCustomRepoSizeEstimates(
      IndexingExecutionContext indexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingExecutionContext.IndexingUnit;
      int repositorySize = ((CustomRepoCodeIndexingProperties) indexingUnit.Properties).RepositorySize;
      int estimatedGrowth = (int) ((double) indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<float>("/Service/ALMSearch/Settings/Routing/Code/CodeEntityCustomRepositoryGrowthFactor", true, 0.1f) * (double) repositorySize);
      return new IndexingUnitWithSize(indexingUnit, repositorySize, estimatedGrowth, true)
      {
        ActualInitialDocCount = repositorySize
      };
    }

    internal virtual IndexingUnitWithSize GetScopedPathFileEstimates(
      IndexingExecutionContext indexingExecutionContext)
    {
      if (indexingExecutionContext.RepositoryIndexingUnit.IndexingUnitType != "CustomRepository")
        throw new NotImplementedException(FormattableString.Invariant(FormattableStringFactory.Create("{0} is not implemented for Scoped IU of large non_custom repository. The routing is assigned ", (object) "AssignRouting")) + FormattableString.Invariant(FormattableStringFactory.Create("from collection metadata crawl operation ")));
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingExecutionContext.IndexingUnit;
      int repositorySize = ((CustomRepoCodeIndexingProperties) indexingUnit.Properties).RepositorySize;
      SDRepositoryProperties properties = indexingExecutionContext.DataAccessFactory.GetCustomRepositoryDataAccess().GetRepository(indexingExecutionContext.RequestContext, indexingExecutionContext.CollectionId, indexingExecutionContext.ProjectName, indexingExecutionContext.RepositoryName).Properties as SDRepositoryProperties;
      int branchCount = 1;
      if (properties != null)
        branchCount = properties.BranchDetails.Count<SDBranchDetail>();
      double factorForMultibranch = indexingExecutionContext.ProvisioningContext.ContractType.GetFileCountFactorForMultibranch(indexingExecutionContext.RequestContext, indexingUnit, branchCount);
      int currentEstimatedDocumentCount = (int) ((double) repositorySize * factorForMultibranch);
      int estimatedGrowth = (int) ((double) indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<float>("/Service/ALMSearch/Settings/Routing/Code/CodeEntityCustomRepositoryGrowthFactor", true, 0.1f) * (double) currentEstimatedDocumentCount);
      return new IndexingUnitWithSize(indexingUnit, currentEstimatedDocumentCount, estimatedGrowth, true)
      {
        ActualInitialDocCount = currentEstimatedDocumentCount
      };
    }

    internal override bool CanAssignRouting(IndexingUnitWithSize indexingUnitWithSize) => !indexingUnitWithSize.IsEmpty();

    internal override bool ShouldReassignRouting(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      return indexingUnit.IndexingUnitType == "CustomRepository" && indexingUnit.IsLargeRepository(indexingExecutionContext.RequestContext);
    }

    internal override IndexingUnitWithSize GetIndexingUnitWithFallbackSizeEstimates(
      IndexingExecutionContext indexingExecutionContext)
    {
      int documentCount;
      if (!(indexingExecutionContext.IndexingUnit.IndexingUnitType == "Git_Repository") || this.GetGitHttpClientWrapper(indexingExecutionContext).IsEmpty(indexingExecutionContext.RequestContext, indexingExecutionContext.ProjectIndexingUnit, indexingExecutionContext.IndexingUnit, out documentCount))
        return (IndexingUnitWithSize) null;
      int shardDensity = indexingExecutionContext.ProvisioningContext.ContractType.GetShardDensity(indexingExecutionContext.RequestContext);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingExecutionContext.IndexingUnit;
      int branchCount = indexingUnit.GetBranchCountFromTFSAttributesIfGitRepo();
      if (branchCount <= 0)
        branchCount = 1;
      double factorForMultibranch = indexingExecutionContext.ProvisioningContext.ContractType.GetFileCountFactorForMultibranch(indexingExecutionContext.RequestContext, indexingUnit, branchCount);
      float currentHostConfigValue = indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<float>("/Service/ALMSearch/Settings/Routing/Code/CodeEntityGitRepositoryGrowthFactor", true, 0.3f);
      int currentEstimatedDocumentCount = (int) ((double) (documentCount * branchCount) * factorForMultibranch);
      double gitRepoGrowthFactor = (double) currentHostConfigValue;
      int estimatedDocCount = currentEstimatedDocumentCount;
      int estimatedGrowth;
      ref int local1 = ref estimatedGrowth;
      long num1;
      ref long local2 = ref num1;
      long num2;
      ref long local3 = ref num2;
      GitHttpClientWrapper.GetGitRepoSizeEstimates(shardDensity, (float) gitRepoGrowthFactor, estimatedDocCount, out local1, out local2, out local3);
      return new IndexingUnitWithSize(indexingExecutionContext.IndexingUnit, currentEstimatedDocumentCount, estimatedGrowth, true)
      {
        ActualInitialSize = 0
      };
    }

    internal override void PostRun(
      CoreIndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      CodeQueryScopingCacheUtil.SqlNotifyForRepoAddition(this.DataAccessFactory, indexingExecutionContext.RequestContext, indexingUnit);
      this.QueueBulkIndexingAndFinalizationEvents((IndexingExecutionContext) indexingExecutionContext);
    }

    protected internal override bool ShouldRetryToGetSizeEstimates(
      IndexingExecutionContext indexingExecutionContext)
    {
      return indexingExecutionContext.IndexingUnit.IndexingUnitType == "Git_Repository" && !this.GetGitHttpClientWrapper(indexingExecutionContext).IsEmpty(indexingExecutionContext.RequestContext, indexingExecutionContext.ProjectIndexingUnit, indexingExecutionContext.IndexingUnit, out int _) && base.ShouldRetryToGetSizeEstimates(indexingExecutionContext);
    }

    internal virtual GitHttpClientWrapper GetGitHttpClientWrapper(
      IndexingExecutionContext indexingExecutionContext)
    {
      IndexingExecutionContext executionContext = indexingExecutionContext;
      Guid? nullable = indexingExecutionContext.ProjectId;
      Guid projectId = nullable.Value;
      nullable = indexingExecutionContext.RepositoryId;
      Guid repositoryId = nullable.Value;
      TraceMetaData traceMetaData = this.m_traceMetaData;
      return new GitHttpClientWrapper((ExecutionContext) executionContext, projectId, repositoryId, traceMetaData);
    }

    internal virtual TfvcHttpClientWrapper GetTfvcHttpClientWrapper(
      IndexingExecutionContext indexingExecutionContext)
    {
      return new TfvcHttpClientWrapper((ExecutionContext) indexingExecutionContext, this.m_traceMetaData);
    }

    internal virtual ProjectHttpClientWrapper GetProjectHttpClientWrapper(
      IndexingExecutionContext indexingExecutionContext)
    {
      return new ProjectHttpClientWrapper((ExecutionContext) indexingExecutionContext, this.m_traceMetaData);
    }

    internal virtual void QueueBulkIndexingAndFinalizationEvents(
      IndexingExecutionContext indexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingExecutionContext.IndexingUnit;
      if (this.FinalizeHelper.ShouldFinalizeChildIndexingUnit(indexingExecutionContext, this.IndexingUnit) && this.IndexingUnit.IsRepository())
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
        {
          IndexingUnitId = indexingExecutionContext.CollectionIndexingUnit.IndexingUnitId,
          ChangeData = (ChangeEventData) new CodeIndexPublishData((ExecutionContext) indexingExecutionContext),
          ChangeType = "CompleteBulkIndex",
          State = IndexingUnitChangeEventState.Pending,
          AttemptCount = 0
        };
        indexingUnitChangeEvent1.LeaseId = this.IndexingUnitChangeEvent.LeaseId;
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = this.IndexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded((ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent1);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Queued {0} with LeaseId {1} to finalize.", (object) indexingUnitChangeEvent2.Id, (object) indexingUnitChangeEvent2.LeaseId)));
      }
      if (!(indexingExecutionContext.RepositoryIndexingUnit.IndexingUnitType != "CustomRepository"))
        return;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent3 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = indexingUnit.IndexingUnitId,
        ChangeData = (ChangeEventData) new CodeBulkIndexEventData((ExecutionContext) indexingExecutionContext),
        ChangeType = indexingUnit.IsLargeRepository(indexingExecutionContext.RequestContext) ? "UpdateIndex" : "BeginBulkIndex",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0,
        LeaseId = this.IndexingUnitChangeEvent.LeaseId
      };
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent4 = this.IndexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded((ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent3);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Queued {0} with LeaseId {1} to perform Bulk Indexing.", (object) indexingUnitChangeEvent4.Id, (object) indexingUnitChangeEvent4.LeaseId)));
    }
  }
}
