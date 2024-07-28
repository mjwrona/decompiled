// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Package.CollectionPackagePatchOperation
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
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Package
{
  internal class CollectionPackagePatchOperation : AbstractIndexingPatchOperation
  {
    internal List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> m_ListOfFeedsToProcess;
    internal List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> m_ListOfFeedsToProcessForDelete;
    internal List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> m_feedIndexingUnitsFromDB;
    internal List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> m_feedPackageChangesToProcess;
    internal FeedHttpClientWrapper m_FeedHttpClientWrapper;
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080743, "Indexing Pipeline", "IndexingOperation");

    public CollectionPackagePatchOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(executionContext, indexingUnit, indexingUnitChangeEvent, Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance())
    {
    }

    internal CollectionPackagePatchOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent,
      IDataAccessFactory dataAccessFactory)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, dataAccessFactory)
    {
      this.m_ListOfFeedsToProcess = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      this.m_ListOfFeedsToProcessForDelete = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      this.m_feedIndexingUnitsFromDB = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      this.m_feedPackageChangesToProcess = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(CollectionPackagePatchOperation.s_traceMetadata, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessageBuilder = new StringBuilder();
      IndexingExecutionContext executionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        bool flag = true;
        string str = string.Empty;
        if (!coreIndexingExecutionContext.RequestContext.IsContinuousIndexingEnabled((IEntityType) PackageEntityType.GetInstance()))
        {
          str = "Search.Server.Package.ContinuousIndexing";
          flag = false;
        }
        if (!flag)
        {
          resultMessageBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Feature flag '{0}' is disabled for this account.", (object) str);
          operationResult.Message = resultMessageBuilder.ToString();
          operationResult.Status = OperationStatus.Succeeded;
          return operationResult;
        }
        this.SetUpHttpClients((ExecutionContext) executionContext);
        this.m_feedIndexingUnitsFromDB = FeedIndexingUnitHelper.GetFeedIndexingUnits((CoreIndexingExecutionContext) executionContext, this.IndexingUnit);
        this.PerformRequiredFeedChecks(executionContext);
        this.PrepareFeedsAndPackagesToQueueUpdateOperation(executionContext);
        this.QueueUpdateFeedsOperation(executionContext, resultMessageBuilder);
        this.QueueUpdateIndexOperation(executionContext, resultMessageBuilder);
        this.QueueFeedSecurityAcesSyncOperation(executionContext, resultMessageBuilder);
        this.QueueCleanUpFeedsOperationIfRequired(executionContext, resultMessageBuilder);
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = resultMessageBuilder.ToString();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(CollectionPackagePatchOperation.s_traceMetadata, nameof (RunOperation));
      }
      return operationResult;
    }

    internal virtual void QueueCleanUpFeedsOperationIfRequired(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessageBuilder)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> unitsToBeDeleted = FeedIndexingUnitHelper.GetFeedIndexingUnitsToBeDeleted((CoreIndexingExecutionContext) indexingExecutionContext, this.IndexingUnit);
      if (this.m_ListOfFeedsToProcessForDelete.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
        unitsToBeDeleted.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) this.m_ListOfFeedsToProcessForDelete);
      if (!unitsToBeDeleted.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
        return;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = this.IndexingUnit.IndexingUnitId,
        ChangeType = "CleanUpFeeds",
        ChangeData = new ChangeEventData((ExecutionContext) indexingExecutionContext),
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent);
      resultMessageBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Found {0} Feeds to be cleaned, Queued Cleanup Feeds Operation;", (object) unitsToBeDeleted.Count<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())));
    }

    internal virtual void QueueUpdateIndexOperation(
      IndexingExecutionContext executionContext,
      StringBuilder resultMessageBuilder)
    {
      int currentHostConfigValue = executionContext.RequestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/PackageSearch/FeedCIJobDelayInSec", true);
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in this.m_feedPackageChangesToProcess)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent();
        indexingUnitChangeEvent1.IndexingUnitId = this.IndexingUnit.IndexingUnitId;
        indexingUnitChangeEvent1.ChangeType = "UpdateIndex";
        PackageContinuousIndexEventData continuousIndexEventData = new PackageContinuousIndexEventData((ExecutionContext) executionContext);
        continuousIndexEventData.Feeds = new List<Guid>()
        {
          indexingUnit.TFSEntityId
        };
        continuousIndexEventData.Trigger = 11;
        continuousIndexEventData.Delay = TimeSpan.FromSeconds((double) currentHostConfigValue);
        indexingUnitChangeEvent1.ChangeData = (ChangeEventData) continuousIndexEventData;
        indexingUnitChangeEvent1.State = IndexingUnitChangeEventState.Pending;
        indexingUnitChangeEvent1.AttemptCount = (byte) 0;
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = indexingUnitChangeEvent1;
        this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) executionContext, indexingUnitChangeEvent2);
        resultMessageBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Queued Periodic UpdateIndex Operation for {0} feeds", (object) this.m_ListOfFeedsToProcess.Count)));
      }
    }

    internal virtual void QueueUpdateFeedsOperation(
      IndexingExecutionContext executionContext,
      StringBuilder resultMessageBuilder)
    {
      int currentHostConfigValue = executionContext.RequestContext.GetCurrentHostConfigValue<int>("/Service/ALMSearch/Settings/PackageSearch/FeedCIJobDelayInSec", true);
      List<Guid> guidList = new List<Guid>();
      if (!this.m_ListOfFeedsToProcess.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
        return;
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in this.m_ListOfFeedsToProcess)
        guidList.Add(indexingUnit.TFSEntityId);
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent1 = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent();
      indexingUnitChangeEvent1.IndexingUnitId = this.IndexingUnit.IndexingUnitId;
      indexingUnitChangeEvent1.ChangeType = "FeedUpdates";
      PackageContinuousIndexEventData continuousIndexEventData = new PackageContinuousIndexEventData((ExecutionContext) executionContext);
      continuousIndexEventData.Feeds = guidList;
      continuousIndexEventData.Trigger = 11;
      continuousIndexEventData.Delay = TimeSpan.FromSeconds((double) currentHostConfigValue);
      indexingUnitChangeEvent1.ChangeData = (ChangeEventData) continuousIndexEventData;
      indexingUnitChangeEvent1.State = IndexingUnitChangeEventState.Pending;
      indexingUnitChangeEvent1.AttemptCount = (byte) 0;
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent2 = indexingUnitChangeEvent1;
      this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) executionContext, indexingUnitChangeEvent2);
      resultMessageBuilder.Append(FormattableString.Invariant(FormattableStringFactory.Create("Queued Periodic UpdateIndex Operation for {0} feeds", (object) this.m_ListOfFeedsToProcess.Count)));
    }

    internal virtual void QueueFeedSecurityAcesSyncOperation(
      IndexingExecutionContext executionContext,
      StringBuilder resultMessageBuilder)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = executionContext.IndexingUnit.IndexingUnitId,
        ChangeData = new ChangeEventData((ExecutionContext) executionContext)
        {
          Trigger = 11
        },
        ChangeType = "FeedSecurityAcesSync",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      this.IndexingUnitChangeEventHandler.HandleEvent((ExecutionContext) executionContext, indexingUnitChangeEvent);
      resultMessageBuilder.AppendLine(FormattableString.Invariant(FormattableStringFactory.Create("Queued PackageSecurityAcesSync Operation for indexing unit {0} ", (object) executionContext.IndexingUnit.IndexingUnitId)));
    }

    internal virtual void PrepareFeedsAndPackagesToQueueUpdateOperation(
      IndexingExecutionContext indexingExecutionContext)
    {
      Dictionary<Guid, FeedChange> dictionary1 = this.GetDetailsOfChangedFeedsInCollection(indexingExecutionContext).ToDictionary<FeedChange, Guid, FeedChange>((Func<FeedChange, Guid>) (kvp => kvp.Feed.Id), (Func<FeedChange, FeedChange>) (kvp => kvp));
      Dictionary<Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> dictionary2 = this.m_feedIndexingUnitsFromDB.ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (kvp => kvp.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (kvp => kvp));
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      foreach (KeyValuePair<Guid, FeedChange> keyValuePair in dictionary1)
      {
        if (dictionary2.ContainsKey(keyValuePair.Key))
        {
          if (keyValuePair.Value.ChangeType == ChangeType.AddOrUpdate)
          {
            if (keyValuePair.Value.Feed.Project != (ProjectReference) null && (dictionary2[keyValuePair.Key].Properties as FeedIndexingProperties).ProjectId != keyValuePair.Value.Feed.Project.Id)
            {
              (dictionary2[keyValuePair.Key].Properties as FeedIndexingProperties).ProjectId = keyValuePair.Value.Feed.Project.Id;
              indexingUnitList.Add(dictionary2[keyValuePair.Key]);
            }
            this.m_ListOfFeedsToProcess.Add(dictionary2[keyValuePair.Key]);
          }
          else if (keyValuePair.Value.ChangeType == ChangeType.Delete || keyValuePair.Value.ChangeType == ChangeType.PermanentDelete)
            this.m_ListOfFeedsToProcessForDelete.Add(dictionary2[keyValuePair.Key]);
          else
            throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("ChangeType {0} is not a supported one", (object) keyValuePair.Value.ChangeType)));
        }
        else if (keyValuePair.Value.ChangeType == ChangeType.AddOrUpdate)
        {
          try
          {
            Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed;
            FeedChange feedChange;
            if (keyValuePair.Value.Feed.Project != (ProjectReference) null)
            {
              feed = this.m_FeedHttpClientWrapper.GetFeed(keyValuePair.Key, keyValuePair.Value.Feed.Project.Id);
              feedChange = this.m_FeedHttpClientWrapper.GetFeedChange(feed.Id.ToString(), keyValuePair.Value.Feed.Project.Id);
            }
            else
            {
              feed = this.m_FeedHttpClientWrapper.GetFeed(keyValuePair.Key);
              feedChange = this.m_FeedHttpClientWrapper.GetFeedChange(feed.Id.ToString());
            }
            Microsoft.VisualStudio.Services.Search.Common.IndexingUnit feedIndexingUnit = feed.ToFeedIndexingUnit(this.IndexingUnit, feedChange.FeedContinuationToken);
            indexingUnitList.Add(feedIndexingUnit);
            this.m_feedPackageChangesToProcess.Add(feedIndexingUnit);
          }
          catch (Exception ex)
          {
            if (!(ex.InnerException is FeedIdNotFoundException))
            {
              if (!ex.Message.Contains("FeedIdNotFoundException"))
                throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Exception thrown when trying to get feed details info {0}", (object) ex)));
            }
          }
        }
      }
      if (indexingUnitList.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
        this.IndexingUnitDataAccess.AddOrUpdateIndexingUnits(indexingExecutionContext.RequestContext, indexingUnitList, true);
      if (this.m_ListOfFeedsToProcessForDelete.Any<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>())
      {
        this.m_ListOfFeedsToProcessForDelete.ForEach((Action<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (iu => iu.Properties.IsDisabled = true));
        this.IndexingUnitDataAccess.UpdateIndexingUnits(indexingExecutionContext.RequestContext, this.m_ListOfFeedsToProcessForDelete);
      }
      this.m_feedPackageChangesToProcess.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) this.GetDetailsOfChangedPackagesInCollection(indexingExecutionContext));
    }

    internal virtual List<FeedChange> GetDetailsOfChangedFeedsInCollection(
      IndexingExecutionContext iexContext)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit = iexContext.CollectionIndexingUnit;
      long continuationToken = (collectionIndexingUnit.Properties as CollectionPackageIndexingProperties).NextFeedContinuationToken;
      bool isLastBatch = false;
      int num = 0;
      List<FeedChange> feedsInCollection = new List<FeedChange>();
      do
      {
        FeedChangesResponse feedChanges1 = this.m_FeedHttpClientWrapper.GetFeedChanges(continuationToken, out isLastBatch);
        IEnumerable<FeedChange> feedChanges2 = feedChanges1.FeedChanges;
        feedsInCollection.AddRange(feedChanges2);
        num += feedChanges2.Count<FeedChange>();
        if (feedChanges1.Count > 0)
          continuationToken = feedChanges1.NextFeedContinuationToken;
      }
      while (!isLastBatch);
      (collectionIndexingUnit.Properties as CollectionPackageIndexingProperties).NextFeedContinuationToken = continuationToken;
      this.IndexingUnitDataAccess.UpdateIndexingUnit(iexContext.RequestContext, collectionIndexingUnit);
      return feedsInCollection;
    }

    internal virtual void PerformRequiredFeedChecks(
      IndexingExecutionContext indexingExecutionContext)
    {
    }

    internal virtual List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetDetailsOfChangedPackagesInCollection(
      IndexingExecutionContext indexingExecutionContext)
    {
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> packagesInCollection = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnitList = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit in this.m_feedIndexingUnitsFromDB)
      {
        FeedIndexingProperties properties = indexingUnit.Properties as FeedIndexingProperties;
        try
        {
          Guid projectId = properties.ProjectId;
          FeedChange feedChange = !(properties.ProjectId != new Guid()) ? this.m_FeedHttpClientWrapper.GetFeedChange(indexingUnit.TFSEntityId.ToString()) : this.m_FeedHttpClientWrapper.GetFeedChange(indexingUnit.TFSEntityId.ToString(), properties.ProjectId);
          if (properties.LatestPackageContinuationToken < feedChange.LatestPackageContinuationToken)
            packagesInCollection.Add(indexingUnit);
        }
        catch (Exception ex)
        {
          if (ex.InnerException is FeedIdNotFoundException || ex.Message.Contains("FeedIdNotFoundException"))
          {
            indexingUnit.Properties.IsDisabled = true;
            this.IndexingUnitDataAccess.UpdateIndexingUnit(indexingExecutionContext.RequestContext, indexingUnit);
            indexingUnitList.Add(indexingUnit);
          }
          else
            throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Exception thrown when trying to get feedChange info {0}", (object) ex)));
        }
      }
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit1 in indexingUnitList)
      {
        Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnit1;
        this.m_feedIndexingUnitsFromDB.Remove(this.m_feedIndexingUnitsFromDB.Where<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, bool>) (item => item.TFSEntityId == indexingUnit.TFSEntityId)).First<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>());
      }
      return packagesInCollection;
    }

    internal virtual void SetUpHttpClients(ExecutionContext executionContext) => this.m_FeedHttpClientWrapper = this.GetHttpClientFactory().GetFeedHttpClient(executionContext);

    internal virtual IHttpClientWrapperFactory GetHttpClientFactory() => HttpClientWrapperFactory.GetInstance();
  }
}
