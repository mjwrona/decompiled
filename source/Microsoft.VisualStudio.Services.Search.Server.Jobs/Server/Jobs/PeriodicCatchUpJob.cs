// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.PeriodicCatchUpJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Common.Constants;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Analyzers;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class PeriodicCatchUpJob : AbstractPeriodicCatchUpJob
  {
    private const string NextIndexingUnitIdToHeal = "NextIndexingUnitIdToHeal";
    private IndexOperations m_indexOperations;

    protected override IEntityType EntityType => (IEntityType) CodeEntityType.GetInstance();

    protected override int TracePoint => 1080336;

    public PeriodicCatchUpJob()
      : base(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance())
    {
    }

    internal PeriodicCatchUpJob(IDataAccessFactory dataAccessFactory)
      : base(dataAccessFactory)
    {
    }

    internal override bool IsIndexingEnabled(IVssRequestContext requestContext) => requestContext.IsSearchConfigured() && requestContext.IsCodeIndexingEnabled();

    internal bool IsContinuousIndexingEnabled(IVssRequestContext requestContext) => requestContext.IsCodeCrudOperationsEnabled();

    internal PeriodicCatchUpJob(
      IDataAccessFactory dataAccessFactory,
      IndexOperations indexOperations)
      : base(dataAccessFactory)
    {
      this.m_indexOperations = indexOperations;
    }

    internal override bool PreRunCatchUp(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      StringBuilder resultMessageBuilder)
    {
      IndexingExecutionContext indexingExecutionContext = new IndexingExecutionContext(executionContext.RequestContext, collectionIndexingUnit, executionContext.ExecutionTracerContext.TracerCICorrelationDetails);
      indexingExecutionContext.FaultService = executionContext.FaultService;
      indexingExecutionContext.InitializeNameAndIds(this.DataAccessFactory);
      if (!executionContext.RequestContext.IsJobEnabled(JobConstants.PeriodicCatchUpJobId))
      {
        resultMessageBuilder.Append("Job is disabled for this host.");
        return false;
      }
      executionContext.IsReindexingFailedOrInProgress(this.DataAccessFactory, this.EntityType);
      if (this.IsContinuousIndexingEnabled(executionContext.RequestContext))
      {
        if (executionContext.RequestContext.IsFeatureEnabled("Search.Server.Code.OrphanRepoCleanUp"))
          this.HandleOrphanRepos(indexingExecutionContext, executionContext.RequestContext, resultMessageBuilder);
        this.HandleMissingProjectRenameAndDeleteNotifications(executionContext, resultMessageBuilder);
        this.SyncGitRepositories(indexingExecutionContext, resultMessageBuilder);
        this.ProcessMissingTFVCReposAndChangesets(indexingExecutionContext, resultMessageBuilder);
        this.ProcessFailedFiles(indexingExecutionContext, indexingUnitDataAccess);
        if (!executionContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          this.AddRepositoryHealPatchOperations(executionContext, indexingUnitDataAccess);
        if (!this.ValidateRoutingValues(executionContext.RequestContext, collectionIndexingUnit))
          this.AddCollectionFinalizeOperation(executionContext, collectionIndexingUnit, resultMessageBuilder);
        this.CreatePatchEvents(indexingExecutionContext, resultMessageBuilder);
        return true;
      }
      resultMessageBuilder.Append("CodeCrudOperations are disabled, so bailing out.");
      return false;
    }

    internal virtual int AddRepositoryHealPatchOperationForRepos(
      ExecutionContext executionContext,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      int repoHealerBatchSize,
      int indexingUnitIdToHeal)
    {
      if (repoHealerBatchSize <= 0)
        return indexingUnitIdToHeal;
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> tfvcRepositories = this.GetIndexingUnitsForGitAndTfvcRepositories(executionContext, indexingUnitDataAccess);
      if (tfvcRepositories == null || tfvcRepositories.Count <= 0)
        return 0;
      tfvcRepositories.Sort((Comparison<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) ((indexingUnit1, indexingUnit2) => indexingUnit1.IndexingUnitId.CompareTo(indexingUnit2.IndexingUnitId)));
      if (tfvcRepositories.Last<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>().IndexingUnitId < indexingUnitIdToHeal)
        indexingUnitIdToHeal = 0;
      int count = tfvcRepositories.Where<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, bool>) (q => q.IndexingUnitId < indexingUnitIdToHeal)).Count<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> list = tfvcRepositories.Skip<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>(count).Take<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>(repoHealerBatchSize).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexUnit in list)
        indexingUnitChangeEventList.Add(this.GetRepositoryHealPatchChangeEvent(executionContext, indexUnit));
      this.IndexingUnitChangeEventHandler.HandleEvents(executionContext, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) indexingUnitChangeEventList);
      return list.Count <= 0 ? 0 : list.LastOrDefault<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>().IndexingUnitId + 1;
    }

    internal virtual List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetIndexingUnitsForGitAndTfvcRepositories(
      ExecutionContext executionContext,
      IIndexingUnitDataAccess indexingUnitDataAccess)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> tfvcRepositories = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      if (executionContext.RequestContext.IsFeatureEnabled("Search.Server.GitRepositoryHealer"))
      {
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexedRepositories = this.GetAllIndexedRepositories(executionContext, indexingUnitDataAccess, "Git_Repository");
        tfvcRepositories.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) indexedRepositories);
      }
      if (executionContext.RequestContext.IsFeatureEnabled("Search.Server.TfvcRepositoryHealer"))
      {
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexedRepositories = this.GetAllIndexedRepositories(executionContext, indexingUnitDataAccess, "TFVC_Repository");
        tfvcRepositories.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) indexedRepositories);
      }
      return tfvcRepositories;
    }

    private List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetAllIndexedRepositories(
      ExecutionContext executionContext,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      string indexingUnitType)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits1 = indexingUnitDataAccess.GetIndexingUnits(executionContext.RequestContext, indexingUnitType, this.EntityType, -1);
      if (executionContext.RequestContext.IsCodeReindexingWithZeroStalenessFeatureEnabled() && executionContext.IsReindexingFailedOrInProgress(this.DataAccessFactory, this.EntityType))
      {
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits2 = indexingUnitDataAccess.GetIndexingUnits(executionContext.RequestContext, indexingUnitType, true, this.EntityType, -1);
        indexingUnits1.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) indexingUnits2);
      }
      return indexingUnits1;
    }

    internal virtual void ProcessMissingTFVCReposAndChangesets(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessageBuilder)
    {
      CodeIndexMetadataStateAnalyser metadataStateAnalyser = this.GetSearchIndexMetadataStateAnalyser();
      try
      {
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Processed {0} missing TFVC Repo Indexing Units. ", (object) metadataStateAnalyser.ProcessMissingTfvcRepos(indexingExecutionContext));
      }
      catch (Exception ex) when (!IndexFaultMapManager.GetFaultMapper(typeof (HostStoppedFaultMapper)).IsMatch(ex))
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("PeriodicCatchUpJobFailedAtTFVCMissedRepositoriesSync", "Indexing Pipeline", 1.0);
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "TFVC repositories missed repositories sync failed with exception. ");
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TFVC repositories missed repositories sync failed with exception: {0}", (object) ex));
      }
      try
      {
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Processed missing changesets {0} for TFVC Repo Indexing Units. ", (object) metadataStateAnalyser.ProcessMissingChangesetsInCollection((ExecutionContext) indexingExecutionContext));
      }
      catch (Exception ex) when (!IndexFaultMapManager.GetFaultMapper(typeof (HostStoppedFaultMapper)).IsMatch(ex))
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("PeriodicCatchUpJobFailedAtTFVCMissedChangeSetsSync", "Indexing Pipeline", 1.0);
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "TFVC repositories missed notification sync failed with exception. ");
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TFVC repositories missed notification sync failed with exception: {0} ", (object) ex));
      }
    }

    internal virtual void SyncGitRepositories(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessageBuilder)
    {
      GitRepoSyncAnalyzer repoSyncAnalyzer = this.CreateGitRepoSyncAnalyzer((ExecutionContext) indexingExecutionContext);
      try
      {
        int num = repoSyncAnalyzer.SyncDefaultBranchChangeAndDeletedReposInCollection(resultMessageBuilder);
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "{0} repository indexing units got default branch changed or deleted. ", (object) num);
      }
      catch (Exception ex) when (!IndexFaultMapManager.GetFaultMapper(typeof (HostStoppedFaultMapper)).IsMatch(ex))
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("PeriodicCatchUpJobFailedAtGitDefaultBranchChangeOrDeletionSync", "Indexing Pipeline", 1.0);
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Git repositories default branch change or deletion sync failed with exception. ");
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Git repositories default branch change or deletion sync failed with exception: {0} ", (object) ex));
      }
      if (this.IsContinuousIndexingEnabled(indexingExecutionContext.RequestContext))
      {
        try
        {
          int num = repoSyncAnalyzer.SyncMissedCommitsInCollection(indexingExecutionContext);
          resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Processed {0} GIT repository indexing units for missed notification sync. ", (object) num);
        }
        catch (Exception ex) when (!IndexFaultMapManager.GetFaultMapper(typeof (HostStoppedFaultMapper)).IsMatch(ex))
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("PeriodicCatchUpJobFailedAtGitMissedCommitSync", "Indexing Pipeline", 1.0);
          resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Git repositories missed notification sync failed with exception. ");
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Git repositories missed notification sync failed with exception: {0} ", (object) ex));
        }
        try
        {
          int num = repoSyncAnalyzer.SyncMissingGitReposInCollection();
          resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Processed {0} new GIT repository indexing units", (object) num);
        }
        catch (Exception ex) when (!IndexFaultMapManager.GetFaultMapper(typeof (HostStoppedFaultMapper)).IsMatch(ex))
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("PeriodicCatchUpJobFailedAtMissingGitRepositoriesSync", "Indexing Pipeline", 1.0);
          resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Sync for missing git repositories failed with exception. ");
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Sync for missing git repositories failed with exception: {0} ", (object) ex));
        }
      }
      try
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = this.DataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(indexingExecutionContext.RequestContext, indexingExecutionContext.RequestContext.GetCollectionID(), "Collection", this.EntityType);
        this.AddCollectionCodeGitRepositoryAcesSyncOperation((ExecutionContext) indexingExecutionContext, indexingUnit.IndexingUnitId);
      }
      catch (Exception ex) when (!IndexFaultMapManager.GetFaultMapper(typeof (HostStoppedFaultMapper)).IsMatch(ex))
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("PeriodicCatchUpJobFailedAtGitSecurityHashSync", "Indexing Pipeline", 1.0);
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Git repositories security hash sync failed with exception. ");
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Git repositories security hash sync failed with exception: {0} ", (object) ex));
      }
    }

    internal virtual void HandleMissingProjectRenameAndDeleteNotifications(
      ExecutionContext executionContext,
      StringBuilder resultMessageBuilder)
    {
      CodeIndexMetadataStateAnalyser metadataStateAnalyser = this.GetSearchIndexMetadataStateAnalyser();
      try
      {
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Processed {0} projects for missing rename and delete notifications. ", (object) metadataStateAnalyser.ProcessMissingProjectRenameAndDeleteNotifications(executionContext));
      }
      catch (Exception ex) when (!IndexFaultMapManager.GetFaultMapper(typeof (HostStoppedFaultMapper)).IsMatch(ex))
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("PeriodicCatchUpJobFailedAtHandleMissingProjectRenameNotifications", "Indexing Pipeline", 1.0);
        string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "HandleMissingProjectRenameAndDeleteNotifications failed with exception: {0} ", (object) ex);
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, str);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, str);
      }
    }

    internal virtual CodeIndexMetadataStateAnalyser GetSearchIndexMetadataStateAnalyser() => new CodeIndexMetadataStateAnalyser();

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent GetRepositoryHealPatchChangeEvent(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexUnit)
    {
      return new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = indexUnit.IndexingUnitId,
        ChangeData = (ChangeEventData) new RepositoryPatchEventData(executionContext)
        {
          Patch = Patch.RepositoryHeal
        },
        ChangeType = "Patch",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
    }

    internal virtual GitRepoSyncAnalyzer CreateGitRepoSyncAnalyzer(ExecutionContext executionContext) => new GitRepoSyncAnalyzerFactory().GetGitRepoSyncAnalyzer(executionContext, new TraceMetaData(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer), (IIndexingUnitChangeEventHandler) this.IndexingUnitChangeEventHandler, this.EntityType);

    internal virtual void AddRepositoryHealPatchOperations(
      ExecutionContext executionContext,
      IIndexingUnitDataAccess indexingUnitDataAccess)
    {
      try
      {
        int configValue = executionContext.RequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/RepositoryHealerBatchSize");
        int result = 0;
        RegistryManager registryManager = new RegistryManager(executionContext.RequestContext, AbstractPeriodicCatchUpJob.s_TraceLayer);
        List<RegistryEntry> entriesWithPathPattern = registryManager.GetRegistryEntriesWithPathPattern(registryManager.GetRegistryPathPattern("NextIndexingUnitIdToHeal"));
        if (entriesWithPathPattern != null && entriesWithPathPattern.Any<RegistryEntry>() && !int.TryParse(entriesWithPathPattern.First<RegistryEntry>().Value, out result))
          result = 0;
        int num = this.AddRepositoryHealPatchOperationForRepos(executionContext, indexingUnitDataAccess, configValue, result);
        registryManager.AddOrUpdateRegistryValue("NextIndexingUnitIdToHeal", executionContext.RequestContext.GetCollectionID().ToString(), num.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      }
      catch (Exception ex) when (!IndexFaultMapManager.GetFaultMapper(typeof (HostStoppedFaultMapper)).IsMatch(ex))
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Periodic Catchup Job 'AddRepositoryHealPatchOperationForRepos' failed with exception {0}. These repos will be retried for healing in next run.", (object) ex);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, message);
      }
    }

    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent AddCollectionCodeGitRepositoryAcesSyncOperation(
      ExecutionContext executionContext,
      int indexingUnitId)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = indexingUnitId,
        ChangeData = (ChangeEventData) new GitRepositoryAcesSyncEventData(executionContext),
        ChangeType = "GitRepositorySecurityAcesSync",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      return this.IndexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded(executionContext, indexingUnitChangeEvent);
    }

    internal virtual void ProcessFailedFiles(
      IndexingExecutionContext indexingExecutionContext,
      IIndexingUnitDataAccess indexingUnitDataAccess)
    {
      int itemsMaxRetryCount = indexingExecutionContext.ServiceSettings.JobSettings.FailedItemsMaxRetryCount;
      int failedItemsToProcess = indexingExecutionContext.ServiceSettings.JobSettings.MaxNumberOfFailedItemsToProcess;
      IDictionary<int, int> recordsByIndexingUnit = indexingExecutionContext.ItemLevelFailureDataAccess.GetCountOfRecordsByIndexingUnit(indexingExecutionContext.RequestContext, (IEntityType) CodeEntityType.GetInstance(), itemsMaxRetryCount, indexingUnitDataAccess);
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> source = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      foreach (KeyValuePair<int, int> keyValuePair in (IEnumerable<KeyValuePair<int, int>>) recordsByIndexingUnit)
      {
        int key = keyValuePair.Key;
        if (keyValuePair.Value > 0)
        {
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnitDataAccess.GetIndexingUnit(indexingExecutionContext.RequestContext, key);
          if (indexingUnit != null)
          {
            if (indexingUnit.IndexingUnitType == "Git_Repository" || indexingUnit.IndexingUnitType == "TFVC_Repository")
              source.Add(this.CreateFailedItemsPatchEvent(indexingExecutionContext, key, Patch.ReIndexFailedItems));
          }
          else
          {
            IEnumerable<ItemLevelFailureRecord> withMaxAttemptCount = indexingExecutionContext.ItemLevelFailureDataAccess.GetItemsWithMaxAttemptCount(indexingExecutionContext.RequestContext, key, int.MaxValue, failedItemsToProcess);
            indexingExecutionContext.ItemLevelFailureDataAccess.DeleteRecordsByIds(indexingExecutionContext.RequestContext, key, (IList<long>) withMaxAttemptCount.Select<ItemLevelFailureRecord, long>((Func<ItemLevelFailureRecord, long>) (x => x.Id)).ToList<long>());
          }
        }
      }
      if (!source.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>())
        return;
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, FormattableString.Invariant(FormattableStringFactory.Create("Queuing {0} change events to process failed failes.", (object) source.Count)));
      this.IndexingUnitChangeEventHandler.HandleEvents((ExecutionContext) indexingExecutionContext, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) source);
    }

    internal virtual void CreatePatchEvents(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessageBuilder)
    {
      string currentHostConfigValue1 = indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<string>("/Service/ALMSearch/Settings/Code/IdsToBePatchedByPeriodicCatchUp");
      string currentHostConfigValue2 = indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<string>("/Service/ALMSearch/Settings/Code/PatchTypeToBeAppliedByPeriodicCatchUp");
      if (string.IsNullOrWhiteSpace(currentHostConfigValue1) || string.IsNullOrWhiteSpace(currentHostConfigValue2))
        return;
      IEnumerable<int> ints = ((IEnumerable<string>) currentHostConfigValue1.Split(new char[1]
      {
        ','
      }, StringSplitOptions.RemoveEmptyEntries)).Select<string, int>((Func<string, int>) (x => int.Parse(x.Trim(), (IFormatProvider) CultureInfo.InvariantCulture)));
      Patch patch = (Patch) Enum.Parse(typeof (Patch), currentHostConfigValue2, true);
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> indexingUnitChangeEventList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
      foreach (int indexingUnitId in ints)
      {
        if (indexingUnitId > 0)
          indexingUnitChangeEventList.Add(this.CreateFailedItemsPatchEvent(indexingExecutionContext, indexingUnitId, patch));
        else
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, FormattableString.Invariant(FormattableStringFactory.Create("Invalid value of Indexing Unit Id {0} present in registry, skipping it.", (object) indexingUnitId)));
      }
      if (indexingUnitChangeEventList.Count <= 0)
        return;
      this.IndexingUnitChangeEventHandler.HandleEvents((ExecutionContext) indexingExecutionContext, (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) indexingUnitChangeEventList);
      resultMessageBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Queued {0} patch events of type {1}.", (object) indexingUnitChangeEventList.Count, (object) currentHostConfigValue2)));
      indexingExecutionContext.RequestContext.DeleteCurrentHostConfigValue("/Service/ALMSearch/Settings/Code/IdsToBePatchedByPeriodicCatchUp");
      indexingExecutionContext.RequestContext.DeleteCurrentHostConfigValue("/Service/ALMSearch/Settings/Code/PatchTypeToBeAppliedByPeriodicCatchUp");
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent CreateFailedItemsPatchEvent(
      IndexingExecutionContext indexingExecutionContext,
      int indexingUnitId,
      Patch patch)
    {
      return new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = indexingUnitId,
        ChangeData = (ChangeEventData) new RepositoryPatchEventData((ExecutionContext) indexingExecutionContext)
        {
          Patch = patch
        },
        ChangeType = "Patch",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
    }

    internal override Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent AddCollectionPatchOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit,
      StringBuilder resultMessageBuilder)
    {
      return (Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent) null;
    }

    internal virtual bool ValidateRoutingValues(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit)
    {
      IAnalyzer routingInfoAnalyzer = (IAnalyzer) this.GetRoutingInfoAnalyzer();
      Dictionary<DataType, ProviderContext> contextDataSet = new Dictionary<DataType, ProviderContext>();
      List<HealthData> healthData = this.GetHealthData(requestContext, collectionIndexingUnit, routingInfoAnalyzer, contextDataSet);
      string result;
      List<ActionData> source = routingInfoAnalyzer.Analyze(healthData, contextDataSet, out result);
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, result);
      return !source.Where<ActionData>((Func<ActionData, bool>) (x => x.ActionType.Equals((object) ActionType.CollectionFinalize))).ToList<ActionData>().Any<ActionData>();
    }

    internal virtual List<HealthData> GetHealthData(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit,
      IAnalyzer routesAnalyzer,
      Dictionary<DataType, ProviderContext> contextDataSet)
    {
      List<HealthData> healthData = new List<HealthData>();
      AbstractContextBuilderFactory contextBuilderFactory = (AbstractContextBuilderFactory) new ContextBuilderFactory();
      HealthStatusJobData healthInputData = new HealthStatusJobData();
      healthInputData.EntityType = this.EntityType;
      CollectionIndexingProperties properties = collectionIndexingUnit?.Properties as CollectionIndexingProperties;
      foreach (DataType dataType in routesAnalyzer.GetDataTypes())
      {
        ProviderContext providerContext = contextBuilderFactory.GetProviderContextBuilder(dataType).BuildContext(requestContext, Scenario.StaleSearchResults, healthInputData, properties);
        contextDataSet.Add(dataType, providerContext);
        IDataProvider dataProvider = new DataProviderFactory().GetDataProvider(dataType);
        healthData.AddRange((IEnumerable<HealthData>) dataProvider.GetData(providerContext));
      }
      return healthData;
    }

    internal virtual RoutingInfoAnalyzer GetRoutingInfoAnalyzer() => new RoutingInfoAnalyzer();

    internal virtual void QueueDeleteOperationForShadowIndexingUnitsIfApplicable(
      IndexingExecutionContext executionContext,
      IIndexingUnitDataAccess indexingUnitDataAccess,
      IEntityType entityType)
    {
      throw new NotImplementedException();
    }

    internal void HandleOrphanRepos(
      IndexingExecutionContext indexingExecutionContext,
      IVssRequestContext requestContext,
      StringBuilder resultMessageBuilder)
    {
      try
      {
        IIndexingUnitDataAccess indexingUnitDataAccess = this.DataAccessFactory.GetIndexingUnitDataAccess();
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> unitsOfOrphanRepos = indexingUnitDataAccess.GetIndexingUnitsOfOrphanRepos(requestContext);
        if (unitsOfOrphanRepos == null || unitsOfOrphanRepos.Count <= 0)
          return;
        string message = unitsOfOrphanRepos.Count.ToString() + " orphan repos found in PeriodicCatchUpJob. Performing soft delete and delete from ES. Permanent delete from SQL will happen when PeriodicMaintenanceJob runs";
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, message);
        if (this.m_indexOperations == null)
          this.m_indexOperations = new IndexOperations();
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in unitsOfOrphanRepos)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, "deleting repo with id: " + (object) indexingUnit.TFSEntityId);
          string normalizedString = indexingUnit.GetTfsEntityIdAsNormalizedString();
          IExpression queryExpression = new IndexOperationsMetadata()
          {
            RepositoryId = normalizedString
          }.GetQueryExpression();
          this.m_indexOperations.PerformBulkDelete(indexingExecutionContext, indexingExecutionContext.GetIndex(), queryExpression);
        }
        List<int> list = unitsOfOrphanRepos.Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>) (item => item.IndexingUnitId)).ToList<int>();
        indexingUnitDataAccess.SoftDeleteIndexingUnits(requestContext, (IEnumerable<int>) list);
      }
      catch (Exception ex)
      {
        resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Failed to perform HandleOrphanRepos function. Orphan repos may or may not be present.");
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TracePoint, AbstractPeriodicCatchUpJob.s_TraceArea, AbstractPeriodicCatchUpJob.s_TraceLayer, FormattableString.Invariant(FormattableStringFactory.Create("HandleOrphanRepos failed with exception: {0}", (object) ex)));
      }
    }
  }
}
