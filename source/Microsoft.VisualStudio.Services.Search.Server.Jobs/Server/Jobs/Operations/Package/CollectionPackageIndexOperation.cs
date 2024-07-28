// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Package.CollectionPackageIndexOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Package
{
  internal class CollectionPackageIndexOperation : CollectionBulkIndexOperationBase
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1080742, "Indexing Pipeline", "IndexingOperation");
    private PackageCrawlSpec m_crawlSpec;
    private bool m_isJobYieldPopulated;

    protected internal IHttpClientWrapperFactory HttpClientWrapperFactory { get; set; }

    public CollectionPackageIndexOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.HttpClientWrapperFactory.GetInstance())
    {
    }

    protected CollectionPackageIndexOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      IHttpClientWrapperFactory httpClientWrapperFactory)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, 1080742)
    {
      this.HttpClientWrapperFactory = httpClientWrapperFactory;
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(CollectionPackageIndexOperation.s_traceMetaData, nameof (RunOperation));
      OperationResult result = new OperationResult();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (!this.PreOperation((CoreIndexingExecutionContext) executionContext, result))
          return result;
        if (coreIndexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/service/ALMSearch/Settings/Routing/PackageDocCountBasedIndexProvisioningEnabled", true))
          coreIndexingExecutionContext.RequestContext.GetService<IRoutingService>().AssignIndex(executionContext, new List<IndexingUnitWithSize>());
        result = this.RunBaseOperation(coreIndexingExecutionContext);
        if (result.Status != OperationStatus.Succeeded)
          return result;
        executionContext.Log.Append(result.Message);
        this.RunOperationInternal(executionContext);
        this.QueueFeedSecurityAcesSyncOperation(executionContext);
        this.TriggerFinalizeIfNeeded(executionContext);
        result.Status = OperationStatus.Succeeded;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(CollectionPackageIndexOperation.s_traceMetaData, nameof (RunOperation));
      }
      return result;
    }

    protected internal virtual void RunOperationInternal(IndexingExecutionContext iexContext)
    {
      Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> feedIndexingUnits = this.GetFeedIndexingUnits((CoreIndexingExecutionContext) iexContext);
      if (feedIndexingUnits.Count == 0)
      {
        iexContext.Log.Append("No valid IndexingUnits could be created or fetched.");
      }
      else
      {
        this.m_crawlSpec = this.GetPackageCrawlSpec((CoreIndexingExecutionContext) iexContext, feedIndexingUnits);
        using (FirstPartyPipelineContext<PackageDocumentId, PackageDocument> pipelineContext = this.GetPipelineContext(iexContext))
        {
          this.ExecutePipeline(this.GetPipeline(pipelineContext));
          this.UpdateFeedIndexingUnits((CoreIndexingExecutionContext) pipelineContext.IndexingExecutionContext, this.m_crawlSpec, feedIndexingUnits);
        }
      }
    }

    internal virtual OperationResult RunBaseOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      return base.RunOperation(coreIndexingExecutionContext);
    }

    internal virtual FirstPartyPipelineContext<PackageDocumentId, PackageDocument> GetPipelineContext(
      IndexingExecutionContext indexingExecutionContext)
    {
      return new FirstPartyPipelineContext<PackageDocumentId, PackageDocument>(indexingExecutionContext.IndexingUnit, indexingExecutionContext, (CoreCrawlSpec) this.m_crawlSpec, this.IndexingUnitChangeEvent, this.IndexingUnitChangeEventHandler, false);
    }

    internal virtual PackageIndexingPipeline GetPipeline(
      FirstPartyPipelineContext<PackageDocumentId, PackageDocument> pipelineContext)
    {
      return new PackageIndexingPipeline(pipelineContext);
    }

    internal virtual void ExecutePipeline(PackageIndexingPipeline pipeline) => pipeline.Run();

    protected virtual bool PreOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      OperationResult result)
    {
      bool flag = true;
      string str = string.Empty;
      if (!coreIndexingExecutionContext.RequestContext.IsPackageIndexingEnabled())
      {
        str = "Search.Server.Package.Indexing";
        flag = false;
      }
      else if (this.IndexingUnitChangeEvent.ChangeType == "UpdateIndex" && !coreIndexingExecutionContext.RequestContext.IsFeatureEnabled("Search.Server.Package.ContinuousIndexing"))
      {
        str = "Search.Server.Package.ContinuousIndexing";
        flag = false;
      }
      if (!flag)
      {
        coreIndexingExecutionContext.Log.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Feature flag '{0}' is disabled for this account.", (object) str));
        result.Status = OperationStatus.Succeeded;
      }
      return flag;
    }

    internal virtual void TriggerFinalizeIfNeeded(IndexingExecutionContext indexingExecutionContext) => this.TriggerCompleteBulkIndexIfNeeded(indexingExecutionContext);

    protected void TriggerCompleteBulkIndexIfNeeded(
      IndexingExecutionContext indexingExecutionContext)
    {
      if (this.IndexingUnitChangeEvent.ChangeType != "BeginBulkIndex")
        return;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = indexingExecutionContext.IndexingUnit.IndexingUnitId,
        ChangeData = new ChangeEventData((ExecutionContext) indexingExecutionContext)
        {
          Trigger = this.IndexingUnitChangeEvent.ChangeData.Trigger
        },
        ChangeType = "CompleteBulkIndex",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(indexingExecutionContext.RequestContext, typeof (CollectionPackageIndexOperation).ToString(), this.IndexingUnitChangeEvent.ChangeData.Trigger);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = this.IndexingUnitChangeEventHandler.HandleEvent(indexingExecutionContext.RequestContext.GetExecutionContext(correlationDetails), indexingUnitChangeEvent1);
      indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Triggered event {0}", (object) indexingUnitChangeEvent2)));
    }

    protected internal override void HandleOperationFailure(
      IndexingExecutionContext indexingExecutionContext,
      OperationResult result,
      Exception ex)
    {
      base.HandleOperationFailure(indexingExecutionContext, result, ex);
      if (ex is ArgumentException || ex is NotSupportedException)
      {
        result.Status = OperationStatus.Failed;
      }
      else
      {
        if (result.Status != OperationStatus.Failed)
          return;
        this.TriggerFinalizeIfNeeded(indexingExecutionContext);
        TeamFoundationEventLog.Default.Log(indexingExecutionContext.Log.Content, SearchEventId.CollectionPackageBulkIndexingOperationFailed, EventLogEntryType.Error);
      }
    }

    protected override EntityFinalizerBase FinalizeHelper => (EntityFinalizerBase) new CollectionPackageIndexFinalizeOperationHelper();

    private Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetFeedIndexingUnits(
      CoreIndexingExecutionContext iexContext)
    {
      CollectionPackageIndexingProperties properties = (CollectionPackageIndexingProperties) iexContext.CollectionIndexingUnit.Properties;
      this.m_isJobYieldPopulated = false;
      this.m_isJobYieldPopulated = properties.FeedIndexJobYieldData != null && properties.FeedIndexJobYieldData.HasData();
      return !this.m_isJobYieldPopulated ? this.AddOrDeleteFeedIndexingUnits(iexContext) : this.GetFeedIndexingUnitsFromDB(iexContext);
    }

    internal virtual Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetFeedIndexingUnitsFromDB(
      CoreIndexingExecutionContext indexingExecutionContext)
    {
      return FeedIndexingUnitHelper.GetFeedIndexingUnits(indexingExecutionContext, this.IndexingUnit).ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (kvp => kvp.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (kvp => kvp));
    }

    internal virtual Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> AddOrDeleteFeedIndexingUnits(
      CoreIndexingExecutionContext indexingExecutionContext)
    {
      IEnumerable<FeedChange> feedChanges = this.GetFeedChanges(indexingExecutionContext);
      Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> dictionary1 = FeedIndexingUnitHelper.GetFeedIndexingUnits(indexingExecutionContext, this.IndexingUnit).ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (kvp => kvp.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (kvp => kvp));
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList1 = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList2 = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      foreach (FeedChange feedChange in feedChanges)
      {
        if (!dictionary1.ContainsKey(feedChange.Feed.Id))
        {
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit feedIndexingUnit = feedChange.Feed.ToFeedIndexingUnit(this.IndexingUnit, feedChange.FeedContinuationToken);
          dictionary1.Add(feedChange.Feed.Id, feedIndexingUnit);
          indexingUnitList2.Add(feedIndexingUnit);
        }
      }
      Dictionary<Guid, Guid> dictionary2 = feedChanges.ToDictionary<FeedChange, Guid, Guid>((Func<FeedChange, Guid>) (key => key.Feed.Id), (Func<FeedChange, Guid>) (value => value.Feed.Id));
      foreach (KeyValuePair<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> keyValuePair in dictionary1)
      {
        if (!dictionary2.ContainsKey(keyValuePair.Key))
          indexingUnitList1.Add(keyValuePair.Value);
      }
      if (indexingUnitList2.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
        this.AddFeedIndexingUnits(indexingExecutionContext, indexingUnitList2);
      if (indexingUnitList1.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
      {
        foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in indexingUnitList1)
          dictionary1.Remove(indexingUnit.TFSEntityId);
        this.QueueDeleteFeedOperation(indexingExecutionContext, indexingUnitList1);
      }
      this.UpdateFeedPropertiesToFeedIU(indexingExecutionContext, dictionary1.Select<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x.Value)).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>(), feedChanges);
      return dictionary1;
    }

    internal virtual IEnumerable<FeedChange> GetFeedChanges(
      CoreIndexingExecutionContext indexingExecutionContext)
    {
      FeedHttpClientWrapper feedHttpClient = this.HttpClientWrapperFactory.GetFeedHttpClient((ExecutionContext) indexingExecutionContext);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit = indexingExecutionContext.CollectionIndexingUnit;
      CollectionPackageIndexingProperties properties = collectionIndexingUnit.Properties as CollectionPackageIndexingProperties;
      bool flag;
      ref bool local = ref flag;
      FeedChangesResponse feedChanges1 = feedHttpClient.GetFeedChanges(0L, out local, false);
      IEnumerable<FeedChange> feedChanges2 = feedChanges1.FeedChanges;
      properties.NextFeedContinuationToken = feedChanges1.NextFeedContinuationToken;
      this.IndexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, collectionIndexingUnit);
      return feedChanges2;
    }

    internal virtual void UpdateFeedPropertiesToFeedIU(
      CoreIndexingExecutionContext indexingExecutionContext,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> feedIndexingUnits,
      IEnumerable<FeedChange> feedChanges)
    {
      foreach (FeedChange feedChange1 in feedChanges)
      {
        FeedChange feedChange = feedChange1;
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = feedIndexingUnits.Find((Predicate<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (x => x.TFSEntityId == feedChange.Feed.Id));
        if (indexingUnit != null)
          (indexingUnit.Properties as FeedIndexingProperties).FeedContinuationToken = feedChange.FeedContinuationToken;
      }
      if (!feedIndexingUnits.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
        return;
      this.IndexingUnitDataAccess.UpdateIndexingUnits(indexingExecutionContext.RequestContext, feedIndexingUnits);
    }

    internal virtual void AddFeedIndexingUnits(
      CoreIndexingExecutionContext indexingExecutionContext,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> feedIndexingUnitsToBeAdded)
    {
      if (feedIndexingUnitsToBeAdded.Count <= 0)
        return;
      this.IndexingUnitDataAccess.AddOrUpdateIndexingUnits(indexingExecutionContext.RequestContext, feedIndexingUnitsToBeAdded, true);
      indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Added {0} Indexing Unit for new Feeds.", (object) feedIndexingUnitsToBeAdded.Count)));
    }

    internal virtual void QueueDeleteFeedOperation(
      CoreIndexingExecutionContext indexingExecutionContext,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> feedIndexingUnitsToBeDeleted)
    {
      if (feedIndexingUnitsToBeDeleted.Count <= 0)
        return;
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in feedIndexingUnitsToBeDeleted)
      {
        ((FeedIndexingProperties) indexingUnit.Properties).IsPublicFeed = false;
        indexingUnit.Properties.IsDisabled = true;
      }
      this.IndexingUnitDataAccess.UpdateIndexingUnits(indexingExecutionContext.RequestContext, feedIndexingUnitsToBeDeleted);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeType = "CleanUpFeeds",
        ChangeData = new ChangeEventData((ExecutionContext) indexingExecutionContext),
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent);
      indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Found {0} Disabled Feeds, Queued Cleanup Feeds Operation", (object) feedIndexingUnitsToBeDeleted.Count)));
    }

    internal virtual void UpdateFeedIndexingUnits(
      CoreIndexingExecutionContext indexingExecutionContext,
      PackageCrawlSpec crawlSpec,
      Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> feedIndexingUnits)
    {
      foreach (FeedIndexingProperties indexingProperty in crawlSpec.FeedIndexingProperties)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit;
        if (!feedIndexingUnits.TryGetValue(indexingProperty.FeedId, out indexingUnit))
          throw new ArgumentException("Invalid FeedIndexingProperties in crawl spec");
        indexingUnit.Properties = (IndexingProperties) indexingProperty;
      }
      this.IndexingUnitDataAccess.UpdateIndexingUnits(indexingExecutionContext.RequestContext, feedIndexingUnits.Values.ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>());
    }

    internal virtual PackageCrawlSpec GetPackageCrawlSpec(
      CoreIndexingExecutionContext indexingExecutionContext,
      Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> feedIndexingUnits)
    {
      List<FeedIndexingProperties> list = feedIndexingUnits.Select<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>, FeedIndexingProperties>((Func<KeyValuePair<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>, FeedIndexingProperties>) (s => s.Value.Properties as FeedIndexingProperties)).ToList<FeedIndexingProperties>();
      list.Sort((Comparison<FeedIndexingProperties>) ((x, y) => x.FeedId.CompareTo(y.FeedId)));
      PackageCrawlSpec packageCrawlSpec = new PackageCrawlSpec();
      packageCrawlSpec.OrganizationName = indexingExecutionContext.RequestContext.GetOrganizationName();
      ((CoreCrawlSpec) packageCrawlSpec).CollectionName = indexingExecutionContext.RequestContext.GetCollectionName();
      ((CoreCrawlSpec) packageCrawlSpec).CollectionId = indexingExecutionContext.RequestContext.GetCollectionID().ToString();
      packageCrawlSpec.FeedIndexingProperties = list;
      ((CoreCrawlSpec) packageCrawlSpec).JobYieldData = (AbstractJobYieldData) new FeedIndexJobYieldData();
      PackageCrawlSpec crawlSpec = packageCrawlSpec;
      if (this.IndexingUnitChangeEvent.ChangeType == "BeginBulkIndex" && !this.m_isJobYieldPopulated)
      {
        crawlSpec.FeedIndexingProperties.ForEach((Action<FeedIndexingProperties>) (x => x.LatestPackageContinuationToken = 0L));
        this.UpdateFeedIndexingUnits(indexingExecutionContext, crawlSpec, feedIndexingUnits);
      }
      return crawlSpec;
    }

    internal override bool CanFinalize(IndexingExecutionContext executionContext) => ((PackageBulkIndexEventData) this.IndexingUnitChangeEvent.ChangeData).Finalize;

    internal override IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent> CreateBulkIndexOperationForSubEntities(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessage)
    {
      return (IList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>) new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent>();
    }

    internal override void CreateIndexPublishEvent(
      IndexingExecutionContext executionContext,
      IndexingUnitChangeEventPrerequisites indexPublishPreReq)
    {
    }

    internal virtual void QueueFeedSecurityAcesSyncOperation(
      IndexingExecutionContext indexingExecutionContext)
    {
      if (this.IndexingUnitChangeEvent.ChangeType != "BeginBulkIndex")
        return;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = indexingExecutionContext.IndexingUnit.IndexingUnitId,
        ChangeData = new ChangeEventData((ExecutionContext) indexingExecutionContext)
        {
          Trigger = this.IndexingUnitChangeEvent.ChangeData.Trigger
        },
        ChangeType = "FeedSecurityAcesSync",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      ITracerCICorrelationDetails correlationDetails = TracerCICorrelationDetailsFactory.GetCICorrelationDetails(indexingExecutionContext.RequestContext, typeof (CollectionPackageIndexOperation).ToString(), this.IndexingUnitChangeEvent.ChangeData.Trigger);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = this.IndexingUnitChangeEventHandler.HandleEvent(indexingExecutionContext.RequestContext.GetExecutionContext(correlationDetails), indexingUnitChangeEvent1);
      indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Triggered event {0}", (object) indexingUnitChangeEvent2)));
    }
  }
}
