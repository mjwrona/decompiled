// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.AbstractAccountFaultInJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Extension;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Api;
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
  public abstract class AbstractAccountFaultInJob : ITeamFoundationJobExtension
  {
    private const string TraceArea = "Indexing Pipeline";
    private const string TraceLayer = "Job";
    private IndexingExecutionContext m_collectionIndexingExecutionContext;
    internal int Trigger = 1;
    internal readonly StringBuilder ResultMessage = new StringBuilder();

    internal IDataAccessFactory DataAccessFactory { get; set; }

    protected IIndexingUnitChangeEventHandler IndexingUnitChangeEventHandler { get; }

    protected Guid HostFaultInJobId { get; }

    protected IEntityType EntityType { get; }

    internal AbstractAccountFaultInJob(
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler,
      IEntityType entityType,
      Guid hostFaultInJobId)
    {
      this.DataAccessFactory = dataAccessFactory;
      this.IndexingUnitChangeEventHandler = indexingUnitChangeEventHandler;
      this.EntityType = entityType;
      this.HostFaultInJobId = hostFaultInJobId;
    }

    public TeamFoundationJobExecutionResult Run(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      DateTime queueTime,
      out string resultMessage)
    {
      this.ValidateRequestContext(requestContext);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080331, "Indexing Pipeline", "Job", nameof (Run));
      Stopwatch stopwatch = Stopwatch.StartNew();
      ExecutionContext executionContext = requestContext.GetExecutionContext(TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext, jobDefinition.Name, 1));
      bool flag = true;
      int currentAttemptCount = 0;
      string countRegistryPath = this.GetAttemptCountRegistryPath(requestContext);
      TeamFoundationJobExecutionResult jobResult = TeamFoundationJobExecutionResult.Failed;
      try
      {
        currentAttemptCount = requestContext.GetCurrentHostConfigValue<int>(countRegistryPath) + 1;
        if (this.ShouldRequeue(executionContext))
        {
          this.ResultMessage.Append("Requeue the Account Fault-In job since Extension Uninstall sequence is still in progress.");
          jobResult = TeamFoundationJobExecutionResult.Succeeded;
          return jobResult;
        }
        if (!this.PreRun(executionContext, out jobResult))
        {
          this.ResultMessage.Append("Pre-run criteria was not met. Bailing out.");
          flag = false;
          executionContext.ExecutionTracerContext.PublishCi("Indexing Pipeline", "Job", "RunCriteriaStatus", "Failed");
          executionContext.ExecutionTracerContext.PublishCi("Indexing Pipeline", "Job", "OperationStatus", jobResult.ToString());
          return jobResult;
        }
        if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          this.UpdateReIndexingStatusIfNeeded(requestContext);
        this.UpdateFeatureFlagsIfNeeded(requestContext);
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = this.OnboardHostForIndexing(executionContext);
        IndexingUnitChangeEventPrerequisites operationPrerequisites = this.GetMetaCrawlOperationPrerequisites(executionContext);
        this.AddMetadataCrawlOperation(executionContext, indexingUnit.IndexingUnitId, operationPrerequisites);
        requestContext.DeleteCurrentHostConfigValue(countRegistryPath);
        this.PostRun(executionContext);
        flag = false;
        jobResult = TeamFoundationJobExecutionResult.Succeeded;
      }
      catch (Exception ex)
      {
        jobResult = TeamFoundationJobExecutionResult.Failed;
        requestContext.SetCurrentHostConfigValue<int>(countRegistryPath, currentAttemptCount);
        int faultInJobMaxRetries = executionContext.ServiceSettings.JobSettings.AccountFaultInJobMaxRetries;
        string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "'{0}' failed with error : {1}. ", (object) this.GetType().Name, (object) ex);
        string str2;
        if (currentAttemptCount >= faultInJobMaxRetries)
        {
          flag = false;
          str2 = str1 + FormattableString.Invariant(FormattableStringFactory.Create("All retries exhausted, Max Attempts possible {0}, Current Attempt Count {1}. Aborting retries now.", (object) faultInJobMaxRetries, (object) currentAttemptCount));
          requestContext.DeleteCurrentHostConfigValue(countRegistryPath);
        }
        else
          str2 = str1 + FormattableString.Invariant(FormattableStringFactory.Create("Max Attempts possible {0}, Current Attempt Count {1}. Requeuing this job.", (object) faultInJobMaxRetries, (object) currentAttemptCount));
        this.ResultMessage.Append(str2);
      }
      finally
      {
        resultMessage = this.ResultMessage.ToString();
        stopwatch.Stop();
        long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
        if (flag)
          this.QueueAccountFaultInJob(executionContext, currentAttemptCount);
        else
          this.FinalizeJob(jobDefinition, jobResult, elapsedMilliseconds, currentAttemptCount);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080331, "Indexing Pipeline", "Job", nameof (Run));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
      return jobResult;
    }

    protected internal virtual IndexIdentity ProvisionIndexForAccount(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit,
      bool assignNewIndex)
    {
      IndexingExecutionContext executionContext1 = this.GetCollectionIndexingExecutionContext(executionContext, collectionIndexingUnit);
      IIndexProvisioner indexProvisioner = this.GetIndexProvisioner(executionContext1, collectionIndexingUnit);
      string indexingIndexName = collectionIndexingUnit.GetIndexingIndexName();
      IndexingExecutionContext indexingExecutionContext = executionContext1;
      ISearchPlatform searchPlatform = executionContext1.ProvisioningContext.SearchPlatform;
      IndexIdentity indexIdentity1 = string.IsNullOrWhiteSpace(indexingIndexName) ? (IndexIdentity) null : IndexIdentity.CreateIndexIdentity(indexingIndexName);
      IndexIdentity indexIdentity2 = indexProvisioner.ProvisionIndex(indexingExecutionContext, searchPlatform, indexIdentity1);
      if (assignNewIndex && !string.IsNullOrWhiteSpace(indexingIndexName) && indexIdentity2.Name.Equals(indexingIndexName))
        throw new SearchPlatformException(FormattableString.Invariant(FormattableStringFactory.Create("Could not provision different index for collection {0}. Current index [{1}], proposed index [{2}].", (object) executionContext.RequestContext.GetCollectionID(), (object) indexingIndexName, (object) indexIdentity2.Name)));
      this.ResultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, FormattableString.Invariant(FormattableStringFactory.Create("Index [{0}] provisioned for host [{1}] and EntityType [{2}] ", (object) indexIdentity2, (object) executionContext.RequestContext.ServiceHost.InstanceId, (object) this.EntityType)));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080342, "Indexing Pipeline", "Job", this.ResultMessage.ToString());
      return indexIdentity2;
    }

    protected internal virtual IndexIdentity ProvisionIndex(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit,
      bool assignNewIndex)
    {
      return this.ProvisionIndexForAccount(executionContext, collectionIndexingUnit, assignNewIndex);
    }

    internal virtual IndexMetadataStateAnalyser GetIndexMetadataStateAnalyser(
      ExecutionContext executionContext,
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
    {
      return new IndexMetadataStateAnalyserFactory().GetIndexMetadataStateAnalyser(this.DataAccessFactory, this.IndexingUnitChangeEventHandler, this.EntityType);
    }

    internal string GetAttemptCountRegistryPath(IVssRequestContext requestContext) => "/Service/ALMSearch/Settings/" + this.EntityType?.ToString() + "/" + requestContext.GetCurrentHostIdAsNormalizedString();

    internal IndexingUnitChangeEventPrerequisites GetMetaCrawlOperationPrerequisites(
      ExecutionContext executionContext)
    {
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList = (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      this.QueueCleanUpOperationsForCollection(executionContext, indexingUnitChangeEventList);
      return this.GetPrerequisitesFromChangeEvents(executionContext, indexingUnitChangeEventList);
    }

    internal virtual void QueueAccountFaultInJob(
      ExecutionContext executionContext,
      int currentAttemptCount)
    {
      int faultInJobDelayInSec = executionContext.ServiceSettings.JobSettings.AccountFaultInJobDelayInSec;
      executionContext.RequestContext.QueueDelayedNamedJob(this.HostFaultInJobId, faultInJobDelayInSec * currentAttemptCount, JobPriorityLevel.Normal);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1080331, "Indexing Pipeline", "Job", FormattableString.Invariant(FormattableStringFactory.Create("Queued next iteration of '{0}' to run after {1} seconds", (object) this.GetType().Name, (object) faultInJobDelayInSec)));
    }

    protected internal virtual bool ShouldRequeue(ExecutionContext executionContext)
    {
      IVssRequestContext requestContext = executionContext.RequestContext;
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return false;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string query = FormattableString.Invariant(FormattableStringFactory.Create("{0}/{1}/{2}", (object) "/Service/ALMSearch/Settings/IsExtensionOperationInProgress", (object) this.EntityType, (object) InstalledExtensionMessageChangeType.Uninstalled));
      return service.GetValue<bool>(requestContext, (RegistryQuery) query, false, false);
    }

    protected internal virtual bool PreRun(
      ExecutionContext executionContext,
      out TeamFoundationJobExecutionResult jobResult)
    {
      jobResult = TeamFoundationJobExecutionResult.Succeeded;
      return true;
    }

    protected internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnit OnboardHostForIndexing(
      ExecutionContext executionContext)
    {
      IIndexingUnitDataAccess indexingUnitDataAccess = this.DataAccessFactory.GetIndexingUnitDataAccess();
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 = indexingUnitDataAccess.GetIndexingUnit(executionContext.RequestContext, executionContext.RequestContext.GetCollectionID(), "Collection", this.EntityType);
      DocumentContractType documentContractType = DocumentContractTypeServiceHelper.GetDefaultDocumentContractType(executionContext.RequestContext, this.EntityType, indexingUnit1);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2;
      if (indexingUnit1 == null)
      {
        indexingUnit2 = this.CreateHostIndexingUnit(executionContext, indexingUnitDataAccess, documentContractType);
        this.Trigger = 1;
      }
      else
      {
        indexingUnit2 = this.UpdateHostIndexingUnit(executionContext, documentContractType, indexingUnit1);
        this.Trigger = executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment ? 1 : 33;
      }
      IndexingExecutionContext executionContext1 = this.GetCollectionIndexingExecutionContext(executionContext, indexingUnit2);
      IndexIdentity indexIdentity = this.ProvisionIndex(executionContext, indexingUnit2, this.Trigger == 33 && executionContext.RequestContext.ExecutionEnvironment.IsHostedDeployment);
      if (indexIdentity != null)
        indexingUnit2.Properties.IndexIndices = new List<IndexInfo>()
        {
          new IndexInfo()
          {
            IndexName = indexIdentity.Name,
            Version = new int?(executionContext1.GetIndexVersion(indexIdentity.Name)),
            DocumentContractType = documentContractType,
            Routing = this.GetRoutingDetails(executionContext1)
          }
        };
      return indexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, indexingUnit2);
    }

    protected virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnit CreateHostIndexingUnit(
      ExecutionContext executionContext,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      DocumentContractType indexContractType)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnit(executionContext.RequestContext.GetCollectionID(), "Collection", this.EntityType, -1);
      indexingUnit1.TFSEntityAttributes = (TFSEntityAttributes) new CollectionAttributes()
      {
        CollectionName = executionContext.RequestContext.GetCollectionName()
      };
      CollectionIndexingProperties indexingProperties = new CollectionIndexingProperties();
      indexingProperties.IndexContractType = indexContractType;
      indexingProperties.QueryContractType = indexContractType;
      indexingProperties.IndexESConnectionString = executionContext.ServiceSettings.JobAgentSearchPlatformConnectionString;
      indexingProperties.QueryESConnectionString = executionContext.ServiceSettings.ATSearchPlatformConnectionString;
      indexingProperties.Name = executionContext.RequestContext.GetCollectionName();
      indexingUnit1.Properties = (IndexingProperties) indexingProperties;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2 = indexingUnit1;
      return indexingUnitDataAccess.AddIndexingUnit(executionContext.RequestContext, indexingUnit2);
    }

    protected internal virtual void PostRun(ExecutionContext executionContext)
    {
    }

    protected internal void FinalizeJob(
      TeamFoundationJobDefinition jobDefinition,
      TeamFoundationJobExecutionResult jobResult,
      long elapsedTime,
      int currentAttemptCount)
    {
      FriendlyDictionary<string, object> properties = new FriendlyDictionary<string, object>();
      properties.Add("OperationStatus", (object) jobResult.ToString());
      properties.Add("JobExecutionTime", (object) elapsedTime);
      properties.Add("JobName", (object) jobDefinition.Name);
      properties.Add("AttemptCount", (object) currentAttemptCount);
      if (jobResult != TeamFoundationJobExecutionResult.Succeeded)
        properties.Add("JobExecutionResultMessage", (object) this.ResultMessage);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Indexing Pipeline", "Job", (IDictionary<string, object>) properties, true);
    }

    protected abstract void ValidateRequestContext(IVssRequestContext requestContext);

    protected abstract void AddMetadataCrawlOperation(
      ExecutionContext executionContext,
      int indexingUnitId,
      IndexingUnitChangeEventPrerequisites metadataCrawlPrerequisites);

    protected internal virtual void UpdateReIndexingStatusIfNeeded(IVssRequestContext requestContext)
    {
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
      Guid collectionId = requestContext.GetCollectionID();
      IReindexingStatusDataAccess statusDataAccess = this.DataAccessFactory.GetReindexingStatusDataAccess();
      ReindexingStatusEntry entry = statusDataAccess.GetReindexingStatusEntry(requestContext1, collectionId, this.EntityType);
      if (entry == null)
        entry = new ReindexingStatusEntry(collectionId, this.EntityType)
        {
          Status = Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.NotRequired
        };
      else if (entry.Status != Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.InProgress)
        entry.Status = Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.InProgress;
      statusDataAccess.AddOrUpdateReindexingStatusEntry(requestContext1, entry);
    }

    protected internal virtual void UpdateFeatureFlagsIfNeeded(IVssRequestContext requestContext)
    {
    }

    protected internal virtual IndexingExecutionContext GetCollectionIndexingExecutionContext(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit)
    {
      if (this.m_collectionIndexingExecutionContext == null)
      {
        IndexingExecutionContext executionContext1 = new IndexingExecutionContext(executionContext.RequestContext, collectionIndexingUnit, executionContext.ExecutionTracerContext.TracerCICorrelationDetails, ((CollectionIndexingProperties) collectionIndexingUnit.Properties).IndexESConnectionString);
        executionContext1.FaultService = executionContext.FaultService;
        this.m_collectionIndexingExecutionContext = executionContext1;
        this.m_collectionIndexingExecutionContext.InitializeNameAndIds(this.DataAccessFactory);
      }
      return this.m_collectionIndexingExecutionContext;
    }

    protected internal virtual string GetRoutingDetails(IndexingExecutionContext iexContext) => (string) null;

    [Info("InternalForTestPurpose")]
    protected internal virtual IIndexProvisioner GetIndexProvisioner(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit)
    {
      return indexingExecutionContext.RequestContext.GetIndexProvisionerFactory(collectionIndexingUnit.EntityType).GetIndexProvisioner(indexingExecutionContext, collectionIndexingUnit);
    }

    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnit UpdateHostIndexingUnit(
      ExecutionContext executionContext,
      DocumentContractType indexContractType,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit)
    {
      if (!(collectionIndexingUnit.Properties is CollectionIndexingProperties properties))
      {
        collectionIndexingUnit.Properties = (IndexingProperties) new CollectionIndexingProperties();
        properties = (CollectionIndexingProperties) collectionIndexingUnit.Properties;
      }
      string connectionString = properties.IndexESConnectionString;
      properties.IndexContractType = indexContractType;
      properties.IndexESConnectionString = executionContext.ServiceSettings.JobAgentSearchPlatformConnectionString;
      properties.Name = executionContext.RequestContext.GetCollectionName();
      this.ResultMessage.Append(FormattableString.Invariant(FormattableStringFactory.Create("Collection is being re-indexed. New connection string: {0}. Previous connection string: {1}. ", (object) properties.IndexESConnectionString, (object) connectionString)));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("ReindexingStarted", "Indexing Pipeline", 1.0);
      if (executionContext.IsReindexingFailedOrInProgress(this.DataAccessFactory, collectionIndexingUnit.EntityType))
        properties.SaveIndexingStatePreReindexing();
      executionContext.RequestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(executionContext.RequestContext, Microsoft.VisualStudio.Services.Search.Common.SqlNotificationEventClasses.DocumentContractTypeChanged, (string) null);
      return collectionIndexingUnit;
    }

    private void QueueCleanUpOperationsForCollection(
      ExecutionContext executionContext,
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> changeEventList)
    {
      if (!executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      IIndexingUnitDataAccess indexingUnitDataAccess = this.DataAccessFactory.GetIndexingUnitDataAccess();
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnitDataAccess.GetIndexingUnit(executionContext.RequestContext, executionContext.RequestContext.GetCollectionID(), "Collection", this.EntityType);
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> unitsWithGivenParent = indexingUnitDataAccess.GetIndexingUnitsWithGivenParent(executionContext.RequestContext, indexingUnit.IndexingUnitId, -1);
      if (unitsWithGivenParent == null || !unitsWithGivenParent.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
        return;
      IndexMetadataStateAnalyser metadataStateAnalyser = this.GetIndexMetadataStateAnalyser(executionContext, this.DataAccessFactory, this.IndexingUnitChangeEventHandler);
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit entityIndexingUnit in unitsWithGivenParent)
        changeEventList.Add(metadataStateAnalyser.CreateEntityDeleteOperationsIfRequired(executionContext, entityIndexingUnit, true));
    }

    private IndexingUnitChangeEventPrerequisites GetPrerequisitesFromChangeEvents(
      ExecutionContext executionContext,
      IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> prereqChangeEvents)
    {
      IndexingUnitChangeEventPrerequisites fromChangeEvents = new IndexingUnitChangeEventPrerequisites();
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent prereqChangeEvent in (IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) prereqChangeEvents)
      {
        IndexingUnitChangeEventPrerequisites eventPrerequisites = fromChangeEvents;
        IndexingUnitChangeEventPrerequisitesFilter prerequisitesFilter = new IndexingUnitChangeEventPrerequisitesFilter();
        prerequisitesFilter.Id = prereqChangeEvent.Id;
        prerequisitesFilter.Operator = IndexingUnitChangeEventFilterOperator.Contains;
        List<IndexingUnitChangeEventState> changeEventStateList;
        if (!executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        {
          changeEventStateList = new List<IndexingUnitChangeEventState>()
          {
            IndexingUnitChangeEventState.Succeeded
          };
        }
        else
        {
          changeEventStateList = new List<IndexingUnitChangeEventState>();
          changeEventStateList.Add(IndexingUnitChangeEventState.Succeeded);
          changeEventStateList.Add(IndexingUnitChangeEventState.Failed);
        }
        prerequisitesFilter.PossibleStates = changeEventStateList;
        eventPrerequisites.Add(prerequisitesFilter);
      }
      return fromChangeEvents;
    }
  }
}
