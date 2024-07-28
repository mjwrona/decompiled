// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.SettingIndexUpdateJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Constants;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Api;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class SettingIndexUpdateJob : ITeamFoundationJobExtension
  {
    private const string TraceArea = "Indexing Pipeline";
    private const string TraceLayer = "Job";
    private const int TracePoint = 1083134;
    private IndexingExecutionContext m_indexingExecutionContext;
    private ISharedIndexingPropertyDataAccess m_sharedIndexingPropertyDataAccess;
    private IShardDetailsDataAccess m_shardDetailsDataAccess;

    internal IDataAccessFactory DataAccessFactoryObject { get; set; }

    protected IEntityType EntityType { get; }

    protected IIndexingUnitChangeEventHandler IndexingUnitChangeEventHandler { get; }

    public SettingIndexUpdateJob()
      : this(DataAccessFactory.GetInstance())
    {
    }

    internal SettingIndexUpdateJob(IDataAccessFactory dataAccessFactory)
    {
      this.DataAccessFactoryObject = dataAccessFactory;
      this.EntityType = (IEntityType) SettingEntityType.GetInstance();
      this.IndexingUnitChangeEventHandler = (IIndexingUnitChangeEventHandler) new Microsoft.VisualStudio.Services.Search.Server.EventHandler.IndexingUnitChangeEventHandler();
      this.m_sharedIndexingPropertyDataAccess = dataAccessFactory.GetSharedIndexingPropertyDataAccess();
      this.m_shardDetailsDataAccess = dataAccessFactory.GetShardDetailsDataAccess();
    }

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      this.ValidateRequestContext(requestContext);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1083134, "Indexing Pipeline", "Job", nameof (Run));
      StringBuilder resultMessageBuilder = new StringBuilder();
      Stopwatch stopwatch = Stopwatch.StartNew();
      ExecutionContext executionContext1 = requestContext.GetExecutionContext(TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext, jobDefinition.Name, 61));
      bool flag = false;
      int currentAttemptCount = 0;
      string countRegistryPath = this.GetAttemptCountRegistryPath(requestContext);
      TeamFoundationJobExecutionResult jobResult = TeamFoundationJobExecutionResult.Succeeded;
      try
      {
        currentAttemptCount = requestContext.GetCurrentHostConfigValue<int>(countRegistryPath) + 1;
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnitData = this.GetDefaultIndexingUnitData(this.GetIndexPropertiesFromRegistry(requestContext), executionContext1);
        IndexingExecutionContext executionContext2 = this.GetIndexingExecutionContext(executionContext1, indexingUnitData);
        string queryIndexName = executionContext2.IndexingUnit.GetQueryIndexName();
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = this.AssignOrProvisionIndex(executionContext2);
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Successfully created search settings with index name: {0}. ", (object) indexingUnit.GetIndexingIndexName());
        this.ExecutePipeline(executionContext2, resultMessageBuilder);
        this.DeleteOldIndex(executionContext2, resultMessageBuilder, queryIndexName);
        requestContext.DeleteCurrentHostConfigValue(countRegistryPath);
      }
      catch (Exception ex)
      {
        jobResult = TeamFoundationJobExecutionResult.Failed;
        requestContext.SetCurrentHostConfigValue<int>(countRegistryPath, currentAttemptCount);
        int updateJobMaxRetries = executionContext1.ServiceSettings.JobSettings.SettingsIndexUpdateJobMaxRetries;
        resultMessageBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Job failed with error : {0}. ", (object) ex)));
        if (currentAttemptCount >= updateJobMaxRetries)
        {
          resultMessageBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("All retries exhausted, Max Attempts possible {0}, Current Attempt Count {1}. Aborting retries now. ", (object) updateJobMaxRetries, (object) currentAttemptCount)));
          requestContext.DeleteCurrentHostConfigValue(countRegistryPath);
        }
        else
        {
          flag = true;
          resultMessageBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Max Attempts possible {0}, Current Attempt Count {1}. Requeuing this job. ", (object) updateJobMaxRetries, (object) currentAttemptCount)));
        }
      }
      finally
      {
        resultMessage = resultMessageBuilder.ToString();
        stopwatch.Stop();
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        if (flag)
          this.QueueSearchSettingsJob(executionContext1, currentAttemptCount);
        else
          this.PublishCIData(jobDefinition, jobResult, elapsedMilliseconds, currentAttemptCount, resultMessageBuilder);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1083134, "Indexing Pipeline", "Job", nameof (Run));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
      return jobResult;
    }

    protected internal FirstPartyPipelineContext<string, SettingDocument> GetPipelineContext(
      IndexingExecutionContext indexingExecutionContext)
    {
      return new FirstPartyPipelineContext<string, SettingDocument>(indexingExecutionContext.IndexingUnit, indexingExecutionContext, (CoreCrawlSpec) this.GetSettingCrawlSpec(), new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent(), this.IndexingUnitChangeEventHandler, false);
    }

    protected internal void ValidateRequestContext(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.CheckHostedDeployment();
    }

    protected internal void PublishCIData(
      TeamFoundationJobDefinition jobDefinition,
      TeamFoundationJobExecutionResult jobResult,
      long elapsedTime,
      int currentAttemptCount,
      StringBuilder resultMessageBuilder)
    {
      FriendlyDictionary<string, object> properties = new FriendlyDictionary<string, object>();
      properties.Add("OperationStatus", (object) jobResult.ToString());
      properties.Add("JobExecutionTime", (object) elapsedTime);
      properties.Add("JobName", (object) jobDefinition.Name);
      properties.Add("AttemptCount", (object) currentAttemptCount);
      if (jobResult != TeamFoundationJobExecutionResult.Succeeded)
        properties.Add("JobExecutionResultMessage", (object) resultMessageBuilder);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Indexing Pipeline", "Job", (IDictionary<string, object>) properties, true);
    }

    internal virtual SettingIndexingPipeline GetPipeline(
      FirstPartyPipelineContext<string, SettingDocument> pipelineContext)
    {
      return new SettingIndexingPipeline(pipelineContext);
    }

    protected internal virtual SettingCrawlSpec GetSettingCrawlSpec() => new SettingCrawlSpec();

    internal string GetAttemptCountRegistryPath(IVssRequestContext requestContext) => "/Service/ALMSearch/Settings/" + this.EntityType?.ToString() + "/" + requestContext.GetCurrentHostIdAsNormalizedString();

    internal virtual void QueueSearchSettingsJob(
      ExecutionContext executionContext,
      int currentAttemptCount)
    {
      int updateJobDelayInSec = executionContext.ServiceSettings.JobSettings.SettingsIndexUpdateJobDelayInSec;
      executionContext.RequestContext.QueueDelayedNamedJob(JobConstants.SettingsSearchIndexUpdateJobId, updateJobDelayInSec * currentAttemptCount, JobPriorityLevel.Normal);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1083134, "Indexing Pipeline", "Job", FormattableString.Invariant(FormattableStringFactory.Create("Queued next iteration of '{0}' to run after {1} seconds", (object) this.GetType().Name, (object) updateJobDelayInSec)));
    }

    protected internal IndexingExecutionContext GetIndexingExecutionContext(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit sharedIndexingUnit)
    {
      if (this.m_indexingExecutionContext == null)
      {
        IndexingExecutionContext executionContext1 = new IndexingExecutionContext(executionContext.RequestContext, sharedIndexingUnit, executionContext.ExecutionTracerContext.TracerCICorrelationDetails, ((CollectionIndexingProperties) sharedIndexingUnit.Properties).IndexESConnectionString);
        executionContext1.FaultService = executionContext.FaultService;
        this.m_indexingExecutionContext = executionContext1;
      }
      return this.m_indexingExecutionContext;
    }

    protected internal DeploymentIndexingProperties GetIndexPropertiesFromRegistry(
      IVssRequestContext requestContext)
    {
      return this.m_sharedIndexingPropertyDataAccess.GetIndexingProperties(requestContext);
    }

    protected internal Microsoft.VisualStudio.Services.Search.Common.IndexingUnit GetDefaultIndexingUnitData(
      DeploymentIndexingProperties indexingProperties,
      ExecutionContext executionContext)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnitData = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnit(Guid.Empty, string.Empty, this.EntityType, -1);
      DeploymentIndexingProperties indexingProperties1;
      if (indexingProperties != null)
      {
        indexingProperties1 = indexingProperties;
      }
      else
      {
        indexingProperties1 = new DeploymentIndexingProperties();
        indexingProperties1.IndexContractType = DocumentContractType.SettingContract;
        indexingProperties1.QueryContractType = DocumentContractType.SettingContract;
        indexingProperties1.IndexESConnectionString = executionContext.ServiceSettings.JobAgentSearchPlatformConnectionString;
        indexingProperties1.QueryESConnectionString = executionContext.ServiceSettings.ATSearchPlatformConnectionString;
      }
      indexingUnitData.Properties = (IndexingProperties) indexingProperties1;
      return indexingUnitData;
    }

    protected internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnit AssignOrProvisionIndex(
      IndexingExecutionContext iexContext)
    {
      List<ShardAssignmentDetails> source = new SettingEntityRoutingProvider(string.Empty).AssignIndex(iexContext, (List<IndexingUnitWithSize>) null);
      if (source == null)
        return (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit) null;
      HashSet<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = source.First<ShardAssignmentDetails>().IndexingUnits;
      return indexingUnits == null ? (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit) null : indexingUnits.First<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
    }

    protected internal virtual void ExecutePipeline(
      IndexingExecutionContext iexContext,
      StringBuilder resultMessageBuilder)
    {
      using (FirstPartyPipelineContext<string, SettingDocument> pipelineContext = this.GetPipelineContext(iexContext))
      {
        this.GetPipeline(pipelineContext).Run();
        resultMessageBuilder.Append(iexContext.Log.Content);
      }
      iexContext.IndexingUnit.Properties.QueryIndices = iexContext.IndexingUnit.Properties.IndexIndices;
      this.m_sharedIndexingPropertyDataAccess.UpdateIndexingProperties(iexContext.RequestContext, (DeploymentIndexingProperties) iexContext.IndexingUnit.Properties);
      resultMessageBuilder.Append("Successfully updated query index in indexing properties. ");
    }

    protected internal void DeleteOldIndex(
      IndexingExecutionContext iexContext,
      StringBuilder resultMessageBuilder,
      string previousQueryIndexName)
    {
      try
      {
        string queryIndexName = iexContext.IndexingUnit.GetQueryIndexName();
        if (string.IsNullOrWhiteSpace(previousQueryIndexName) || string.Equals(previousQueryIndexName, queryIndexName, StringComparison.OrdinalIgnoreCase))
          return;
        IndexOperationsResponse operationsResponse = this.PerformDeleteIndexAction(iexContext, previousQueryIndexName);
        if (operationsResponse.Success)
        {
          resultMessageBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Deleting index {0} of settings search completed successfully. ", (object) previousQueryIndexName)));
        }
        else
        {
          resultMessageBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Deleting index {0} of settings search failed with error: {1}. ", (object) previousQueryIndexName, (object) operationsResponse.ToString())));
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1083134, "Indexing Pipeline", "Job", FormattableString.Invariant(FormattableStringFactory.Create("Deleting previous index{0} encountered an error: {1}", (object) previousQueryIndexName, (object) operationsResponse.ToString())));
        }
        string clusterName = iexContext.ProvisioningContext.SearchClusterManagementService.GetClusterName();
        this.m_shardDetailsDataAccess.MarkShardsInactive(iexContext.RequestContext, clusterName, this.EntityType, previousQueryIndexName);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1083134, "Indexing Pipeline", "Job", FormattableString.Invariant(FormattableStringFactory.Create("Deleting previous index{0} encountered an error: {1}", (object) previousQueryIndexName, (object) ex.ToString())));
        resultMessageBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Deleting index {0} encountered an error: {1}. ", (object) previousQueryIndexName, (object) ex.ToString())));
      }
    }

    protected internal virtual IndexOperationsResponse PerformDeleteIndexAction(
      IndexingExecutionContext iexContext,
      string previousQueryIndexName)
    {
      ISearchIndex index = iexContext.ProvisioningContext.SearchPlatform.GetIndex(IndexIdentity.CreateIndexIdentity(previousQueryIndexName));
      return iexContext.ProvisioningContext.SearchPlatform.DeleteIndex((ExecutionContext) iexContext, index.IndexIdentity);
    }
  }
}
