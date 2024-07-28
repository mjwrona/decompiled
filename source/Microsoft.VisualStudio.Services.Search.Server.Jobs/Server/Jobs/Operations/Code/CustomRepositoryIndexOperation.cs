// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.CustomRepositoryIndexOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities.ChangeEventData;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using Microsoft.VisualStudio.Services.Search.Server.Store;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class CustomRepositoryIndexOperation : RepositoryCodeIndexingOperation
  {
    public CustomRepositoryIndexOperation(
      ExecutionContext executionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent, new TraceMetaData(1080619, "Indexing Pipeline", "IndexingOperation"))
    {
      this.VssRequestContext = executionContext.RequestContext;
      this.SupportsStoringFiles = true;
    }

    protected IVssRequestContext VssRequestContext { get; private set; }

    protected CustomRepoCodeTFSAttributes TFSEntityAttributes => this.IndexingUnit.TFSEntityAttributes as CustomRepoCodeTFSAttributes;

    protected CustomRepoCodeIndexingProperties Properties => this.IndexingUnit.Properties as CustomRepoCodeIndexingProperties;

    protected string RequestId { get; set; }

    protected string Branch { get; set; }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(this.TraceMetaData.TracePoint, this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, nameof (RunOperation));
      OperationResult operationResult = new OperationResult();
      IndexingExecutionContext iexContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        if (!iexContext.IsIndexingEnabled())
        {
          iexContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Indexing is disabled, bailing out.")));
          operationResult.Status = Microsoft.VisualStudio.Services.Search.Server.Pipeline.OperationStatus.Succeeded;
          return operationResult;
        }
        if (coreIndexingExecutionContext.RequestContext.GetConfigValue<bool>("/Service/ALMSearch/Settings/EnableCustomRepository", TeamFoundationHostType.ProjectCollection))
        {
          this.ExecuteCrawlerParserAndFeeder(iexContext, string.Empty, new List<string>());
          CoreIndexingExecutionContext.OutputLog log = coreIndexingExecutionContext.Log;
          string s;
          if (!(this.IndexingUnit.IndexingUnitType == "ScopedIndexingUnit"))
          {
            if (!(this.IndexingUnit.IndexingUnitType == "TemporaryIndexingUnit"))
              s = FormattableString.Invariant(FormattableStringFactory.Create("Successfully indexed Custom Repository Id:{0} for request Id:{1}", (object) this.IndexingUnit.TFSEntityId, (object) this.RequestId));
            else
              s = FormattableString.Invariant(FormattableStringFactory.Create("Successfully indexed Temporary Repository Id:{0}, Scoped IndexingUnit Id:{1}, Repository Id:{2} for request Id:{3}", (object) this.IndexingUnit.TFSEntityId, (object) this.IndexingUnit.ParentUnitId, (object) iexContext.RepositoryIndexingUnit.TFSEntityId, (object) this.RequestId));
          }
          else
            s = FormattableString.Invariant(FormattableStringFactory.Create("Successfully Tree Crawled Scoped Repository Id:{0}, Repository Id:{1} for request Id:{2}", (object) this.IndexingUnit.TFSEntityId, (object) iexContext.RepositoryIndexingUnit.TFSEntityId, (object) this.RequestId));
          log.Append(s);
        }
        else
          coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Registry setting '{0}' is not set to 'true' for this account.", (object) "/Service/ALMSearch/Settings/EnableCustomRepository")));
        operationResult.Status = Microsoft.VisualStudio.Services.Search.Server.Pipeline.OperationStatus.Succeeded;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(this.TraceMetaData.TracePoint, this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, nameof (RunOperation));
      }
      return operationResult;
    }

    internal override void PostRun(
      CoreIndexingExecutionContext executionContext,
      OperationResult result,
      Exception e = null)
    {
      if (result.Status == Microsoft.VisualStudio.Services.Search.Server.Pipeline.OperationStatus.Succeeded)
      {
        IndexingExecutionContext iexContext = (IndexingExecutionContext) executionContext;
        if (this.IndexingUnit.IndexingUnitType == "ScopedIndexingUnit")
        {
          LargeRepositoryMetadataCrawlerEventData changeData = (LargeRepositoryMetadataCrawlerEventData) this.IndexingUnitChangeEvent.ChangeData;
          BulkCodeIndexRequest request = RequestStoreService.Instance.GetRequest<BulkCodeIndexRequest>(iexContext.RequestContext, iexContext.ProjectName, iexContext.RepositoryName, changeData.RequestId.ToString(), string.Empty);
          this.UpdateIndexingFreshness(executionContext, iexContext, request, changeData);
        }
        else if (this.IndexingUnit.IndexingUnitType != "ScopedIndexingUnit")
        {
          try
          {
            RequestStoreService.Instance.DeleteRequest(iexContext.RequestContext, iexContext.ProjectName, iexContext.RepositoryName, this.RequestId, this.Branch);
          }
          catch (Exception ex)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(this.TraceMetaData.TracePoint, this.TraceMetaData.TraceArea, this.TraceMetaData.TraceLayer, ex);
          }
        }
      }
      base.PostRun(executionContext, result, e);
    }

    internal virtual void UpdateIndexingFreshness(
      CoreIndexingExecutionContext executionContext,
      IndexingExecutionContext iexContext,
      BulkCodeIndexRequest request,
      LargeRepositoryMetadataCrawlerEventData changeData)
    {
      if (request == null || !request.IsLastRequest)
        return;
      Dictionary<string, DepotLastChangeInfo> latestChangeIdMap = changeData?.BranchToLatestChangeIdMap;
      if (latestChangeIdMap == null || latestChangeIdMap.Count <= 0)
        return;
      CustomRepoCodeIndexingProperties properties = (CustomRepoCodeIndexingProperties) iexContext.RepositoryIndexingUnit.Properties;
      Dictionary<string, Dictionary<string, DepotIndexInfo>> depotIndexInfo1 = properties.DepotIndexInfo;
      Dictionary<string, Dictionary<string, DepotIndexInfo>> dictionary = depotIndexInfo1 == null ? new Dictionary<string, Dictionary<string, DepotIndexInfo>>() : depotIndexInfo1;
      string key = request.TopFolder.Replace("\\", "");
      Dictionary<string, DepotIndexInfo> source = dictionary.Count <= 0 || !dictionary.ContainsKey(key) ? new Dictionary<string, DepotIndexInfo>() : dictionary[key];
      foreach (KeyValuePair<string, DepotLastChangeInfo> keyValuePair in latestChangeIdMap)
      {
        DepotIndexInfo depotIndexInfo2 = new DepotIndexInfo(keyValuePair.Value.ChangeId, keyValuePair.Value.ChangeUtcTime, keyValuePair.Value.LastSyncedCommitTime);
        DepotIndexInfo depotIndexInfo3;
        if (source.TryGetValue(keyValuePair.Key, out depotIndexInfo3) && DateTime.Compare(keyValuePair.Value.ChangeUtcTime, depotIndexInfo3.LastIndexedChangeUtcTime) < 0)
          depotIndexInfo2 = new DepotIndexInfo(keyValuePair.Value.ChangeId, depotIndexInfo3.LastIndexedChangeUtcTime, keyValuePair.Value.LastSyncedCommitTime);
        source[keyValuePair.Key] = depotIndexInfo2;
      }
      if (source.Count > 1)
      {
        DateTime dateTime = source.Max<KeyValuePair<string, DepotIndexInfo>, DateTime>((Func<KeyValuePair<string, DepotIndexInfo>, DateTime>) (branchEntry => branchEntry.Value.LastIndexedChangeUtcTime));
        foreach (KeyValuePair<string, DepotIndexInfo> keyValuePair in source)
          keyValuePair.Value.LastIndexedChangeUtcTime = dateTime;
      }
      dictionary[key] = source;
      properties.DepotIndexInfo = dictionary;
      iexContext.RepositoryIndexingUnit = executionContext.IndexingUnitDataAccess.UpdateIndexingUnit(executionContext.RequestContext, iexContext.RepositoryIndexingUnit);
    }

    internal override CodeCrawlSpec CreateCrawlSpec(
      IndexingExecutionContext iexContext,
      ref string branchName,
      in List<string> branches)
    {
      CustomRepoCodeTFSAttributes entityAttributes = iexContext.RepositoryIndexingUnit.TFSEntityAttributes as CustomRepoCodeTFSAttributes;
      if (this.IndexingUnit.IndexingUnitType == "CustomRepository")
      {
        CustomRepositoryIndexRequestEventData changeData = this.IndexingUnitChangeEvent.ChangeData as CustomRepositoryIndexRequestEventData;
        this.Branch = changeData.Branches.ToList<string>().First<string>();
        this.RequestId = changeData.RequestId.ToString();
        return (CodeCrawlSpec) CustomCrawlSpec.Create(iexContext, this.Branch, this.RequestId, entityAttributes, this.RequestId, this.IndexingUnitChangeEvent.CreatedTimeUtc);
      }
      if (!(this.IndexingUnit.IndexingUnitType == "ScopedIndexingUnit"))
        return (CodeCrawlSpec) null;
      this.RequestId = (this.IndexingUnitChangeEvent.ChangeData as LargeRepositoryMetadataCrawlerEventData).RequestId.ToString();
      return (CodeCrawlSpec) CustomCrawlSpec.Create(iexContext, branchName, this.RequestId, entityAttributes, this.RequestId, this.IndexingUnitChangeEvent.CreatedTimeUtc);
    }
  }
}
