// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.AbstractShardSizeReductionJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common.Internal;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Constants;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearch;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Api;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public abstract class AbstractShardSizeReductionJob : AbstractElasticsearchJobs
  {
    protected string IndicesToBeMonitored { get; set; }

    protected internal IEntityType EntityType { get; set; }

    protected internal bool MonitoringMode { get; set; }

    [Info("InternalForTestPurpose")]
    internal AbstractShardSizeReductionJob(
      IDataAccessFactory dataAccessFactory,
      ISearchPlatformFactory searchPlatformFactory)
      : base(dataAccessFactory, searchPlatformFactory)
    {
    }

    [Info("InternalForTestPurpose")]
    protected internal override void Initialize(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition)
    {
      base.Initialize(requestContext, jobDefinition);
      this.TracePoint = 1083041;
      this.SearchEventIdentifier = SearchEventId.ElasticsearchShardReduction;
      this.ExecutionContext = requestContext.GetExecutionContext(TracerCICorrelationDetailsFactory.GetCICorrelationDetails(requestContext.ActivityId.ToString(), jobDefinition.Name, 52));
    }

    [Info("InternalForTestPurpose")]
    internal IDictionary<string, IEnumerable<string>> GetLargeShards(
      IVssRequestContext requestContext,
      IDictionary<string, IList<ShardDetailsActualInfo>> indexToShardDetailsMap)
    {
      IDictionary<string, IEnumerable<string>> largeShards = (IDictionary<string, IEnumerable<string>>) new Dictionary<string, IEnumerable<string>>();
      foreach (KeyValuePair<string, IList<ShardDetailsActualInfo>> indexToShardDetails in (IEnumerable<KeyValuePair<string, IList<ShardDetailsActualInfo>>>) indexToShardDetailsMap)
      {
        IEnumerable<KeyValuePair<string, double>> shardSizes = indexToShardDetails.Value.Select<ShardDetailsActualInfo, KeyValuePair<string, double>>((Func<ShardDetailsActualInfo, KeyValuePair<string, double>>) (x => new KeyValuePair<string, double>(x.ShardId.ToString((IFormatProvider) CultureInfo.InvariantCulture), (double) x.ActualSize)));
        IEnumerable<string> extremelyLargeShards = this.ElasticsearchFeedbackProcessor.GetExtremelyLargeShards(requestContext, this.EntityType, shardSizes);
        if (extremelyLargeShards.Any<string>())
        {
          largeShards.Add(indexToShardDetails.Key, extremelyLargeShards);
          this.PublishClientTrace(requestContext, indexToShardDetails.Key, extremelyLargeShards, shardSizes);
        }
      }
      return largeShards;
    }

    [Info("InternalForTestPurpose")]
    internal Set<Guid> GetCollectionsTobeMigratedForAnIndex(
      IVssRequestContext requestContext,
      DocumentContractType contractType,
      KeyValuePair<string, IEnumerable<string>> largeShards)
    {
      Set<Guid> migratedForAnIndex = new Set<Guid>();
      IRoutingLookupService service = requestContext.GetService<IRoutingLookupService>();
      string key = largeShards.Key;
      foreach (string s in largeShards.Value)
      {
        string routingKey = service.GetRoutingKey(requestContext, this._esConnectionString, key, int.Parse(s, (IFormatProvider) CultureInfo.InvariantCulture));
        Guid migratedFromTheShard = this.GetCollectionToBeMigratedFromTheShard(requestContext, key, contractType, routingKey);
        if (migratedFromTheShard != Guid.Empty)
          migratedForAnIndex.Add(migratedFromTheShard);
      }
      return migratedForAnIndex;
    }

    [Info("InternalForTestPurpose")]
    internal void MarkAccountsForReindexingAndTriggerAndMonitorReindexingJob(
      IVssRequestContext deploymentContext,
      List<Guid> collections,
      IEntityType entityType)
    {
      IReindexingStatusDataAccess statusDataAccess = this.DataAccessFactory.GetReindexingStatusDataAccess();
      List<KeyValuePair<Guid, IEntityType>> list1 = collections.Select<Guid, KeyValuePair<Guid, IEntityType>>((Func<Guid, KeyValuePair<Guid, IEntityType>>) (it => new KeyValuePair<Guid, IEntityType>(it, this.EntityType))).ToList<KeyValuePair<Guid, IEntityType>>();
      List<ReindexingStatusEntry> list2 = statusDataAccess.GetReindexingStatusEntries(deploymentContext, list1).Where<ReindexingStatusEntry>((Func<ReindexingStatusEntry, bool>) (it => it.Status == Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.Completed || it.Status == Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.NotRequired || it.Status == Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.Failed)).Select<ReindexingStatusEntry, Guid>((Func<ReindexingStatusEntry, Guid>) (it => it.CollectionId)).Select<Guid, ReindexingStatusEntry>((Func<Guid, ReindexingStatusEntry>) (it => new ReindexingStatusEntry(it, entityType)
      {
        Priority = (short) 0,
        Status = Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.ReindexingRequired
      })).ToList<ReindexingStatusEntry>();
      statusDataAccess.AddOrUpdateReindexingStatusEntries(deploymentContext, list2);
      deploymentContext.QueueDelayedNamedJob(JobConstants.TriggerAndMonitorReindexingJob, 30, JobPriorityLevel.Normal);
    }

    [Info("InternalForTestPurpose")]
    protected internal override TeamFoundationJobExecutionResult InvokeJob(
      IVssRequestContext requestContext,
      out string resultMessage)
    {
      resultMessage = "Running Shard Size Reduction job\n";
      try
      {
        IDictionary<string, IList<ShardDetailsActualInfo>> indexToShardDetailsMap = this.UpdateShardDetails(requestContext);
        IDictionary<string, IEnumerable<string>> largeShards = this.GetLargeShards(requestContext, indexToShardDetailsMap);
        resultMessage += "Updated Shard Size in the SQL table\n";
        requestContext.SetConfigValue<bool>("/Service/ALMSearch/Settings/ForceShardSizeReductionJob", false);
        if (!string.IsNullOrWhiteSpace(this.IndicesToBeMonitored))
        {
          IDictionary<string, IEnumerable<string>> actionableLargeShards = this.GetActionableLargeShards(largeShards);
          List<Guid> source = this.ProcessLargeShards(requestContext, actionableLargeShards);
          if ((source != null ? (source.Any<Guid>() ? 1 : 0) : 0) != 0)
          {
            this.UpdateMaintenanceJobStatus(requestContext, MaintenanceJobStatus.InProgress);
            resultMessage += "Account reindexing Queued for Shards Reduction\n";
          }
          else
          {
            this.UpdateMaintenanceJobStatus(requestContext, MaintenanceJobStatus.Succeeded);
            resultMessage += "No Account reindexing Queued for Shards Reduction\n";
          }
        }
        else
        {
          resultMessage += "No Indices configured for analysis and shard reduction\n";
          this.UpdateMaintenanceJobStatus(requestContext, MaintenanceJobStatus.Succeeded);
        }
        return TeamFoundationJobExecutionResult.Succeeded;
      }
      catch (Exception ex)
      {
        resultMessage += FormattableString.Invariant(FormattableStringFactory.Create("ShardSizeReductionjob failed with error : {0}", (object) ex));
        return TeamFoundationJobExecutionResult.Failed;
      }
    }

    internal virtual void UpdateShardDetails(
      IVssRequestContext requestContext,
      IDictionary<string, IList<ShardDetailsActualInfo>> indexToShardDetailsMap)
    {
      foreach (KeyValuePair<string, IList<ShardDetailsActualInfo>> indexToShardDetails in (IEnumerable<KeyValuePair<string, IList<ShardDetailsActualInfo>>>) indexToShardDetailsMap)
      {
        if (indexToShardDetails.Value.Any<ShardDetailsActualInfo>())
          this.DataAccessFactory.GetShardDetailsDataAccess().UpdateActualShardDetails(requestContext, indexToShardDetails.Value);
      }
    }

    [Info("InternalForTestPurpose")]
    protected internal virtual List<Guid> ProcessLargeShards(
      IVssRequestContext requestContext,
      IDictionary<string, IEnumerable<string>> largeShards)
    {
      List<Guid> guidList = (List<Guid>) null;
      if (largeShards != null && largeShards.Any<KeyValuePair<string, IEnumerable<string>>>())
      {
        DocumentContractType documentContractType = requestContext.GetConfigValue<DocumentContractType>("/Service/ALMSearch/Settings/DefaultCodeDocumentContractType", TeamFoundationHostType.Deployment);
        if (DocumentContractTypeExtension.IsValidCodeDocumentContractType(documentContractType))
        {
          guidList = largeShards.SelectMany<KeyValuePair<string, IEnumerable<string>>, Guid>((Func<KeyValuePair<string, IEnumerable<string>>, IEnumerable<Guid>>) (it => (IEnumerable<Guid>) this.GetCollectionsTobeMigratedForAnIndex(requestContext, documentContractType, it))).Distinct<Guid>().ToList<Guid>();
          string str = string.Join<Guid>(",", (IEnumerable<Guid>) guidList);
          requestContext.SetConfigValue<string>("/Service/ALMSearch/Settings/CollectionsMigrated", str);
          this.MarkAccountsForReindexingAndTriggerAndMonitorReindexingJob(requestContext, guidList, this.EntityType);
        }
        else
          throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("Unsupported contract type {0}", (object) documentContractType)));
      }
      return guidList;
    }

    [Info("InternalForTestPurpose")]
    protected internal override bool PreRunCheck(IVssRequestContext requestContext) => !this.MonitoringMode | requestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/ForceShardSizeReductionJob", TeamFoundationHostType.Deployment);

    [Info("InternalForTestPurpose")]
    protected internal override void MonitorOnGoingJob(IVssRequestContext requestContext)
    {
      if (this.m_lastJobStatus != MaintenanceJobStatus.InProgress)
        return;
      string str1 = requestContext.GetConfigValue<string>("/Service/ALMSearch/Settings/CollectionsMigrated")?.Trim();
      if (string.IsNullOrWhiteSpace(str1))
      {
        this.UpdateMaintenanceJobStatus(requestContext, MaintenanceJobStatus.Succeeded);
      }
      else
      {
        List<KeyValuePair<Guid, IEntityType>> list1 = ((IEnumerable<string>) str1.Split(',')).Select<string, KeyValuePair<Guid, IEntityType>>((Func<string, KeyValuePair<Guid, IEntityType>>) (it => new KeyValuePair<Guid, IEntityType>(new Guid(it), this.EntityType))).ToList<KeyValuePair<Guid, IEntityType>>();
        List<ReindexingStatusEntry> reindexingStatusEntries = this.DataAccessFactory.GetReindexingStatusDataAccess().GetReindexingStatusEntries(requestContext, list1);
        List<Guid> list2 = reindexingStatusEntries.Where<ReindexingStatusEntry>((Func<ReindexingStatusEntry, bool>) (it => it.Status == Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.InProgress || it.Status == Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.Queued || it.Status == Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.ReindexingRequired)).Select<ReindexingStatusEntry, Guid>((Func<ReindexingStatusEntry, Guid>) (it => it.CollectionId)).ToList<Guid>();
        IEnumerable<ReindexingStatusEntry> source = reindexingStatusEntries.Where<ReindexingStatusEntry>((Func<ReindexingStatusEntry, bool>) (it => it.Status == Microsoft.VisualStudio.Services.Search.Common.Enums.ReindexingStatus.Failed));
        if (source.Any<ReindexingStatusEntry>())
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(this.TracePoint, "Indexing Pipeline", "IndexingOperation", "Account reindexing failed for :" + source.Select<ReindexingStatusEntry, string>((Func<ReindexingStatusEntry, string>) (it => it.CollectionId.ToString())).Aggregate<string>((Func<string, string, string>) ((i, j) => i + "," + j)));
        string str2 = list2.Any<Guid>() ? string.Join<Guid>(",", (IEnumerable<Guid>) list2) : string.Empty;
        requestContext.SetConfigValue<string>("/Service/ALMSearch/Settings/CollectionsMigrated", str2);
        this.UpdateMaintenanceJobStatus(requestContext, list2.Any<Guid>() ? MaintenanceJobStatus.InProgress : MaintenanceJobStatus.Succeeded);
      }
    }

    internal virtual void PublishClientTrace(
      IVssRequestContext requestContext,
      string index,
      IEnumerable<string> largeShards,
      IEnumerable<KeyValuePair<string, double>> shardSizes)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishClientTrace("Health Manager", "Job", (IDictionary<string, object>) new FriendlyDictionary<string, object>()
      {
        {
          "IndexName",
          (object) index
        },
        {
          "SizeThreshold",
          (object) this.ElasticsearchFeedbackProcessor.GetExtremelyLargeShardSize(requestContext, this.EntityType)
        },
        {
          "Shards",
          (object) shardSizes.Where<KeyValuePair<string, double>>((Func<KeyValuePair<string, double>, bool>) (x => largeShards.Contains<string>(x.Key)))
        }
      });
    }

    internal abstract Guid GetCollectionToBeMigratedFromTheShard(
      IVssRequestContext requestContext,
      string indexName,
      DocumentContractType contractType,
      string routingId);

    private IDictionary<string, IList<ShardDetailsActualInfo>> UpdateShardDetails(
      IVssRequestContext requestContext)
    {
      IDictionary<string, IList<ShardDetailsActualInfo>> detailsForIndices = this.GetShardDetailsForIndices(this.SearchPlatform.GetIndices(this.ExecutionContext).Records.Select<CatIndicesRecord, string>((Func<CatIndicesRecord, string>) (it => it.Index)).Where<string>((Func<string, bool>) (it => !it.StartsWith(".", StringComparison.Ordinal))));
      this.UpdateShardDetails(requestContext, detailsForIndices);
      return detailsForIndices;
    }

    private IDictionary<string, IList<ShardDetailsActualInfo>> GetShardDetailsForIndices(
      IEnumerable<string> indices)
    {
      IDictionary<string, IList<ShardDetailsActualInfo>> detailsForIndices = (IDictionary<string, IList<ShardDetailsActualInfo>>) new FriendlyDictionary<string, IList<ShardDetailsActualInfo>>();
      foreach (string index in indices)
      {
        List<EsShardDetails> shardsDetails = this.SearchClusterStateService.GetShardsDetails(this.ExecutionContext, index);
        IList<ShardDetailsActualInfo> detailsActualInfoList = (IList<ShardDetailsActualInfo>) new List<ShardDetailsActualInfo>(shardsDetails.Count);
        foreach (EsShardDetails esShardDetails in (IEnumerable<EsShardDetails>) shardsDetails)
          detailsActualInfoList.Add(new ShardDetailsActualInfo(esShardDetails.EsClusterName, esShardDetails.IndexName, esShardDetails.ShardId, esShardDetails.ActualDocCount, esShardDetails.DeletedDocCount, esShardDetails.ActualSize));
        detailsForIndices.Add(index, detailsActualInfoList);
      }
      return detailsForIndices;
    }

    private IDictionary<string, IEnumerable<string>> GetActionableLargeShards(
      IDictionary<string, IEnumerable<string>> largeShards)
    {
      List<string> list = ((IEnumerable<string>) this.IndicesToBeMonitored.Split(',')).Where<string>((Func<string, bool>) (x => !string.IsNullOrWhiteSpace(x))).ToList<string>();
      if (list.Any<string>())
      {
        CatResponse<CatIndicesRecord> indices = this.SearchPlatform.GetIndices(this.ExecutionContext, list);
        if (indices != null && indices.Records.Any<CatIndicesRecord>())
        {
          HashSet<string> indexNames = indices.Records.Select<CatIndicesRecord, string>((Func<CatIndicesRecord, string>) (it => it.Index)).ToHashSet<string>();
          return (IDictionary<string, IEnumerable<string>>) largeShards.Where<KeyValuePair<string, IEnumerable<string>>>((Func<KeyValuePair<string, IEnumerable<string>>, bool>) (x => indexNames.Contains(x.Key))).ToDictionary<KeyValuePair<string, IEnumerable<string>>, string, IEnumerable<string>>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (d => d.Key), (Func<KeyValuePair<string, IEnumerable<string>>, IEnumerable<string>>) (d => d.Value));
        }
      }
      return (IDictionary<string, IEnumerable<string>>) new FriendlyDictionary<string, IEnumerable<string>>();
    }

    internal virtual IEntityType GetEntityType(ExecutionContext executionContext, string indexName)
    {
      IEnumerable<DocumentContractType> documentContracts = this.SearchPlatform.GetIndex(IndexIdentity.CreateIndexIdentity(indexName)).GetDocumentContracts(executionContext);
      if (!documentContracts.Any<DocumentContractType>())
        throw new SearchServiceException("Unable to fetch any contract type for the index: " + indexName);
      using (IEnumerator<DocumentContractType> enumerator = documentContracts.GetEnumerator())
      {
        if (enumerator.MoveNext())
          return enumerator.Current.GetEntityTypeForContractType();
      }
      throw new SearchServiceException("Unable to fetch any known contract type for the index: " + indexName);
    }
  }
}
