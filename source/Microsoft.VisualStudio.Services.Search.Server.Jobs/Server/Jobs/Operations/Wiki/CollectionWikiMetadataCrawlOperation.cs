// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki.CollectionWikiMetadataCrawlOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Server.Pipeline;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Wiki
{
  internal class CollectionWikiMetadataCrawlOperation : AbstractIndexingOperation
  {
    public CollectionWikiMetadataCrawlOperation(
      ExecutionContext executionContext,
      IndexingUnit indexingUnit,
      IndexingUnitChangeEvent indexingUnitChangeEvent)
      : base(executionContext, indexingUnit, indexingUnitChangeEvent)
    {
    }

    public override OperationResult RunOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      OperationResult operationResult = new OperationResult();
      StringBuilder resultMessage = new StringBuilder();
      IndexingExecutionContext indexingExecutionContext = (IndexingExecutionContext) coreIndexingExecutionContext;
      try
      {
        this.CrawlAllSupportedTypes(indexingExecutionContext, resultMessage);
        this.AddCollectionWikiBulkIndexOperation((ExecutionContext) indexingExecutionContext, this.IndexingUnit.IndexingUnitId);
        operationResult.Status = OperationStatus.Succeeded;
      }
      finally
      {
        operationResult.Message = resultMessage.ToString();
      }
      return operationResult;
    }

    internal virtual int CrawlAllSupportedTypes(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessage)
    {
      List<IndexingUnitWithSize> indexingUnitsWithSize = new CollectionWikiMetadataCrawler(indexingExecutionContext, this.DataAccessFactory).CrawlMetadata(indexingExecutionContext, this.IndexingUnit, false);
      this.AssignRoutingValues(indexingExecutionContext, indexingUnitsWithSize);
      resultMessage.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "Successfully added {0} Wiki IndexingUnits of type '{1}' to Collection Id {2}. ", (object) indexingUnitsWithSize.Count, (object) "Git_Repository", (object) this.IndexingUnit.TFSEntityId);
      return indexingUnitsWithSize.Count;
    }

    internal virtual void AssignRoutingValues(
      IndexingExecutionContext indexingExecutionContext,
      List<IndexingUnitWithSize> indexingUnitsWithSize)
    {
      List<IndexingUnit> wikiRepositoriesIndexingUnits = indexingUnitsWithSize.Select<IndexingUnitWithSize, IndexingUnit>((Func<IndexingUnitWithSize, IndexingUnit>) (x => x.IndexingUnit)).ToList<IndexingUnit>();
      bool currentHostConfigValue1 = indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/service/ALMSearch/Settings/Routing/WikiDocCountBasedIndexProvisioningEnabled", true);
      if (wikiRepositoriesIndexingUnits.Any<IndexingUnit>())
      {
        if (indexingExecutionContext.IsReindexingFailedOrInProgress(this.DataAccessFactory, (IEntityType) WikiEntityType.GetInstance()))
        {
          foreach (IndexingUnit indexingUnit in wikiRepositoriesIndexingUnits)
            indexingUnit.Properties.IndexIndices = new List<IndexInfo>();
        }
        bool currentHostConfigValue2 = indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<bool>("/Service/ALMSearch/Settings/Routing/SizeBasedCustomRoutingEnabled", true);
        if (currentHostConfigValue1)
          indexingExecutionContext.RequestContext.GetService<IRoutingService>().AssignIndex(indexingExecutionContext, indexingUnitsWithSize);
        else if (currentHostConfigValue2)
        {
          indexingExecutionContext.RequestContext.GetService<IRoutingService>().AssignShards(indexingExecutionContext, indexingUnitsWithSize);
        }
        else
        {
          wikiRepositoriesIndexingUnits.ForEach((Action<IndexingUnit>) (repo => repo.SetupIndexRouting(indexingExecutionContext, (IList<IndexingUnit>) wikiRepositoriesIndexingUnits)));
          wikiRepositoriesIndexingUnits = indexingExecutionContext.IndexingUnitDataAccess.UpdateIndexingUnits(indexingExecutionContext.RequestContext, wikiRepositoriesIndexingUnits);
        }
      }
      else
      {
        if (!currentHostConfigValue1)
          return;
        indexingExecutionContext.RequestContext.GetService<IRoutingService>().AssignIndex(indexingExecutionContext, indexingUnitsWithSize);
      }
    }

    internal virtual IndexingUnitChangeEvent AddCollectionWikiBulkIndexOperation(
      ExecutionContext indexingExecutionContext,
      int indexingUnitId)
    {
      IndexingUnitChangeEvent indexingUnitChangeEvent1 = new IndexingUnitChangeEvent(this.IndexingUnitChangeEvent.LeaseId);
      indexingUnitChangeEvent1.IndexingUnitId = indexingUnitId;
      WikiBulkIndexEventData bulkIndexEventData = new WikiBulkIndexEventData(indexingExecutionContext);
      bulkIndexEventData.Finalize = (this.IndexingUnitChangeEvent.ChangeData as WikiMetadataCrawlEventData).Finalize;
      bulkIndexEventData.Trigger = this.IndexingUnitChangeEvent.ChangeData.Trigger;
      indexingUnitChangeEvent1.ChangeData = (ChangeEventData) bulkIndexEventData;
      indexingUnitChangeEvent1.ChangeType = "BeginBulkIndex";
      indexingUnitChangeEvent1.State = IndexingUnitChangeEventState.Pending;
      indexingUnitChangeEvent1.AttemptCount = (byte) 0;
      IndexingUnitChangeEvent indexingUnitChangeEvent2 = indexingUnitChangeEvent1;
      return this.IndexingUnitChangeEventHandler.HandleEvent(indexingExecutionContext, indexingUnitChangeEvent2);
    }
  }
}
