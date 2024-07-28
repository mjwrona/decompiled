// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.TfvcRepositoryCodeBulkIndexOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class TfvcRepositoryCodeBulkIndexOperation : RepositoryCodeIndexingOperation
  {
    public TfvcRepositoryCodeBulkIndexOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, new RegistryManager(executionContext.RequestContext, "IndexingOperation"))
    {
    }

    public TfvcRepositoryCodeBulkIndexOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      RegistryManager registryManager)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, new TraceMetaData(1080613, "Indexing Pipeline", "IndexingOperation"))
    {
      this.RegistryManager = registryManager;
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Tracer.TraceEnter(this.TraceMetaData.TracePoint, this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (this.ValidateIndexingFeatureFlag(executionContext))
        {
          this.ValidateRepositoryName(executionContext);
          if (OperationStatus.PartiallySucceeded == this.ExecuteCrawlerParserAndFeeder(executionContext, string.Empty, new List<string>()).OperationStatus)
          {
            operationResult.Status = OperationStatus.Succeeded;
            executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Partially indexed {0} with Id {1}", (object) this.IndexingUnit.IndexingUnitType, (object) this.IndexingUnit.TFSEntityId)));
          }
          executionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Successfully indexed {0} with Id {1}", (object) this.IndexingUnit.IndexingUnitType, (object) this.IndexingUnit.TFSEntityId)));
        }
        else
          coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Indexing is disabled for this {0}.", (object) this.IndexingUnit.ToString())));
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        Tracer.TraceLeave(this.TraceMetaData.TracePoint, this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual bool ValidateIndexingFeatureFlag(
      IndexingExecutionContext indexingExecutionContext)
    {
      return indexingExecutionContext.IsIndexingEnabled() && (((TfvcCodeRepoIndexingProperties) this.IndexingUnit.Properties).LastIndexedChangeSetId <= 0 || indexingExecutionContext.IsCrudOperationsFeatureEnabled());
    }

    internal virtual void ValidateRepositoryName(IndexingExecutionContext indexingExecutionContext)
    {
      string str1 = "$/" + indexingExecutionContext.ProjectName;
      if (str1.Equals(indexingExecutionContext.RepositoryName))
        return;
      TfvcCodeRepoIndexingProperties properties = (TfvcCodeRepoIndexingProperties) this.IndexingUnit.Properties;
      TfvcCodeRepoTFSAttributes entityAttributes = (TfvcCodeRepoTFSAttributes) this.IndexingUnit.TFSEntityAttributes;
      properties.Name = str1;
      string str2 = str1;
      entityAttributes.RepositoryName = str2;
      this.IndexingUnit = this.IndexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, this.IndexingUnit);
      indexingExecutionContext.RepositoryName = str1;
    }

    internal override CodeCrawlSpec CreateCrawlSpec(
      IndexingExecutionContext iexContext,
      ref string branchName,
      in List<string> branches)
    {
      if (this.IndexingUnit.IndexingUnitType == "TFVC_Repository")
        return (CodeCrawlSpec) BulkIndexTfvcCrawlSpec.Create(iexContext, this.IndexingUnit.TFSEntityAttributes as TfvcCodeRepoTFSAttributes, this.IndexingUnit.TFSEntityId);
      throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported IndexingUnitType {0} for TfvcRepositoryCodeBulkIndexOperation.", (object) this.IndexingUnit.IndexingUnitType)));
    }

    internal override CodeIndexingPipelineContext GetPipelineContext(
      IndexingExecutionContext iexContext,
      string branchName,
      List<string> branches)
    {
      TfvcCodeRepoIndexingProperties properties = (TfvcCodeRepoIndexingProperties) this.IndexingUnit.Properties;
      Tracer.TraceInfo(this.TraceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("TfvcRepositoryCodeBulkIndexOperation: Created continuation token based CPF pipeline for BI.")));
      string str = properties.TfvcIndexJobYieldData != null ? properties.TfvcIndexJobYieldData.LastAttemptedTargetChangesetId : "-1";
      TfvcIndexCrawlSpec crawlSpec = new TfvcIndexCrawlSpec(iexContext, this.IndexingUnit.TFSEntityId, properties.LastIndexedChangeSetId.ToString((IFormatProvider) CultureInfo.InvariantCulture), str);
      return new CodeIndexingPipelineContext(iexContext.IndexingUnit, iexContext, (CodeCrawlSpec) crawlSpec, this.IndexingUnitChangeEvent, this.IndexingUnitChangeEventHandler, branchName, branches, false, false);
    }

    internal override CodeIndexingPipeline GetPipeline(CodeIndexingPipelineContext pipelineContext) => this.WorkerPipeline = (CodeIndexingPipeline) new TfvcRepositoryIndexingPipeline(pipelineContext);

    protected internal override TimeSpan GetChangeEventDelay(
      CoreIndexingExecutionContext executionContext,
      Exception e)
    {
      return e != null && IndexFaultMapManager.GetFaultMapper(typeof (VssTimeOutFaultMapper)).IsMatch(e) ? TimeSpan.FromSeconds(executionContext.RequestContext.GetCurrentHostConfigValue<double>("/Service/ALMSearch/Settings/ChangeEventDelayForThrottlingFailureInSeconds", true, 900.0)) : base.GetChangeEventDelay(executionContext, e);
    }
  }
}
