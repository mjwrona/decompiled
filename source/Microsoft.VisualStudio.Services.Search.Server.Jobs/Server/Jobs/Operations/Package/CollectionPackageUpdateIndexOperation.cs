// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Package.CollectionPackageUpdateIndexOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
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
  internal class CollectionPackageUpdateIndexOperation : AbstractIndexingOperation
  {
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetaData = new TraceMetaData(1083064, "Indexing Pipeline", "IndexingOperation");
    private PackageCrawlSpec m_crawlSpec;
    private bool m_isJobYieldPopulated;

    protected internal IHttpClientWrapperFactory HttpClientWrapperFactory { get; set; }

    public CollectionPackageUpdateIndexOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers.HttpClientWrapperFactory.GetInstance())
    {
    }

    protected CollectionPackageUpdateIndexOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      IHttpClientWrapperFactory httpClientWrapperFactory)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.HttpClientWrapperFactory = httpClientWrapperFactory;
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(CollectionPackageUpdateIndexOperation.s_traceMetaData, nameof (RunOperation));
      OperationResult result = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      IndexingExecutionContext iexContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (!this.PreOperation((CoreIndexingExecutionContext) iexContext, ref result, resultMessage))
          return result;
        this.RunOperationInternal(iexContext);
        result.Status = OperationStatus.Succeeded;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(CollectionPackageUpdateIndexOperation.s_traceMetaData, nameof (RunOperation));
      }
      return result;
    }

    internal virtual void RunOperationInternal(IndexingExecutionContext iexContext)
    {
      CollectionPackageIndexingProperties properties = (CollectionPackageIndexingProperties) iexContext.CollectionIndexingUnit.Properties;
      this.m_isJobYieldPopulated = false;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnitToProcess = this.GetFeedIndexingUnitToProcess(properties, iexContext);
      if (indexingUnitToProcess == null)
      {
        iexContext.Log.Append("No valid IndexingUnit could be created or fetched");
      }
      else
      {
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> feedIndexingUnits = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>(1)
        {
          indexingUnitToProcess
        };
        this.m_crawlSpec = this.GetPackageCrawlSpec(iexContext, feedIndexingUnits);
        using (FirstPartyPipelineContext<PackageDocumentId, PackageDocument> pipelineContext = this.GetPipelineContext(iexContext))
        {
          this.ExecutePipeline(this.GetPipeline(pipelineContext));
          this.UpdateFeedIndexingUnit(iexContext, this.m_crawlSpec, indexingUnitToProcess);
        }
      }
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

    internal virtual bool PreOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      ref OperationResult result,
      StringBuilder resultMessage)
    {
      bool flag = true;
      string str = string.Empty;
      if (this.IndexingUnitChangeEvent.ChangeType == "UpdateIndex" && !coreIndexingExecutionContext.RequestContext.IsContinuousIndexingEnabled((IEntityType) PackageEntityType.GetInstance()))
      {
        str = "Search.Server.Package.ContinuousIndexing";
        flag = false;
      }
      if (!flag)
      {
        resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Feature flag '{0}' is disabled for this account.", (object) str);
        result.Message = resultMessage.ToString();
        result.Status = OperationStatus.Succeeded;
      }
      return flag;
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
        TeamFoundationEventLog.Default.Log(result.Message, SearchEventId.CollectionPackageUpdateIndexingOperationFailed, EventLogEntryType.Error);
      }
    }

    internal virtual Microsoft.VisualStudio.Services.Search.Common.IndexingUnit GetFeedIndexingUnitToProcess(
      CollectionPackageIndexingProperties collectionPackageIndexingProperties,
      IndexingExecutionContext indexingExecutionContext)
    {
      FeedIndexJobYieldData indexJobYieldData = collectionPackageIndexingProperties.FeedIndexJobYieldData;
      FeedHttpClientWrapper feedHttpClient = this.HttpClientWrapperFactory.GetFeedHttpClient((ExecutionContext) indexingExecutionContext);
      this.m_isJobYieldPopulated = indexJobYieldData != null && indexJobYieldData.HasData();
      Guid feedId = this.m_isJobYieldPopulated ? indexJobYieldData.FeedId : this.GetFeedToProcessFromChangeData();
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit unitDetailsForFeed = this.GetIndexingUnitDetailsForFeed(indexingExecutionContext.RequestContext, feedId);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit feedIndexingUnitToBeAdded = unitDetailsForFeed;
      if (unitDetailsForFeed == null)
      {
        Guid processFromChangeData = this.GetFeedProjectIdToProcessFromChangeData();
        Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed;
        FeedChange feedChange;
        if (processFromChangeData == new Guid())
        {
          feed = feedHttpClient.GetFeed(feedId);
          feedChange = feedHttpClient.GetFeedChange(feedId.ToString());
        }
        else
        {
          feed = feedHttpClient.GetFeed(feedId, processFromChangeData);
          feedChange = feedHttpClient.GetFeedChange(feedId.ToString(), processFromChangeData);
        }
        feedIndexingUnitToBeAdded = feed.ToFeedIndexingUnit(this.IndexingUnit, feedChange.FeedContinuationToken);
        this.AddFeedIndexingUnit(indexingExecutionContext, feedIndexingUnitToBeAdded);
        this.QueueFeedSecurityAcesSyncOperation(indexingExecutionContext);
      }
      else
      {
        try
        {
          Guid projectId = (unitDetailsForFeed.Properties as FeedIndexingProperties).ProjectId;
          if (projectId != new Guid())
            feedHttpClient.GetFeed(feedId, projectId);
          else
            feedHttpClient.GetFeed(feedId);
        }
        catch (Exception ex)
        {
          if (ex.InnerException is FeedIdNotFoundException || ex.Message.Contains("FeedIdNotFoundException"))
          {
            feedIndexingUnitToBeAdded = (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit) null;
            unitDetailsForFeed.Properties.IsDisabled = true;
            this.IndexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, unitDetailsForFeed);
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
            {
              IndexingUnitId = indexingExecutionContext.CollectionIndexingUnit.IndexingUnitId,
              ChangeType = "CleanUpFeeds",
              ChangeData = new ChangeEventData((ExecutionContext) indexingExecutionContext),
              State = IndexingUnitChangeEventState.Pending,
              AttemptCount = 0
            };
            this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent);
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(CollectionPackageUpdateIndexOperation.s_traceMetaData, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Feed was deleted before being processed for update. Hence cleaning up {0}", (object) unitDetailsForFeed.TFSEntityId));
          }
          else
            throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Exception thrown when trying to get feed info {0}", (object) ex)));
        }
      }
      return feedIndexingUnitToBeAdded;
    }

    internal Guid GetFeedToProcessFromChangeData()
    {
      PackageContinuousIndexEventData changeData = (PackageContinuousIndexEventData) this.IndexingUnitChangeEvent.ChangeData;
      if (changeData.Feeds != null && (changeData.Feeds.Count == 0 || changeData.Feeds.Count > 1))
        throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("{0} expects only 1 feed to process in update operation. Found {1}.", (object) this.IndexingUnitChangeEvent.ChangeType, (object) changeData.Feeds.Count)));
      return changeData.Feeds.First<Guid>();
    }

    internal Guid GetFeedProjectIdToProcessFromChangeData()
    {
      PackageContinuousIndexEventData changeData = (PackageContinuousIndexEventData) this.IndexingUnitChangeEvent.ChangeData;
      if (changeData.Feeds != null && (changeData.Feeds.Count == 0 || changeData.Feeds.Count > 1))
        throw new InvalidOperationException(FormattableString.Invariant(FormattableStringFactory.Create("{0} expects only 1 feed to process in update operation. Found {1}.", (object) this.IndexingUnitChangeEvent.ChangeType, (object) changeData.Feeds.Count)));
      return changeData.MappedProjectId;
    }

    internal virtual void AddFeedIndexingUnit(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit feedIndexingUnitToBeAdded)
    {
      this.IndexingUnitDataAccess.AddIndexingUnit(indexingExecutionContext.RequestContext, feedIndexingUnitToBeAdded);
      indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Added Indexing Unit for new Feed {0}. ", (object) feedIndexingUnitToBeAdded.TFSEntityId)));
    }

    internal virtual void UpdateFeedIndexingUnit(
      IndexingExecutionContext indexingExecutionContext,
      PackageCrawlSpec crawlSpec,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit feedIndexingUnit)
    {
      this.GetIndexingUnitDetailsForFeed(indexingExecutionContext.RequestContext, feedIndexingUnit.TFSEntityId).Properties = (IndexingProperties) crawlSpec.FeedIndexingProperties.First<FeedIndexingProperties>();
      this.IndexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, feedIndexingUnit);
    }

    internal virtual PackageCrawlSpec GetPackageCrawlSpec(
      IndexingExecutionContext indexingExecutionContext,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> feedIndexingUnits)
    {
      PackageCrawlSpec packageCrawlSpec = new PackageCrawlSpec();
      packageCrawlSpec.OrganizationName = indexingExecutionContext.RequestContext.GetOrganizationName();
      ((CoreCrawlSpec) packageCrawlSpec).CollectionName = indexingExecutionContext.RequestContext.GetCollectionName();
      ((CoreCrawlSpec) packageCrawlSpec).CollectionId = indexingExecutionContext.RequestContext.GetCollectionID().ToString();
      packageCrawlSpec.FeedIndexingProperties = feedIndexingUnits.Select<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, FeedIndexingProperties>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, FeedIndexingProperties>) (s => s.Properties as FeedIndexingProperties)).ToList<FeedIndexingProperties>();
      ((CoreCrawlSpec) packageCrawlSpec).JobYieldData = (AbstractJobYieldData) new FeedIndexJobYieldData();
      return packageCrawlSpec;
    }

    internal virtual void QueueFeedSecurityAcesSyncOperation(
      IndexingExecutionContext indexingExecutionContext)
    {
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

    private Microsoft.VisualStudio.Services.Search.Common.IndexingUnit GetIndexingUnitDetailsForFeed(
      IVssRequestContext requestContext,
      Guid feedId)
    {
      return this.IndexingUnitDataAccess.GetIndexingUnit(requestContext, feedId, "Feed", (IEntityType) PackageEntityType.GetInstance());
    }
  }
}
