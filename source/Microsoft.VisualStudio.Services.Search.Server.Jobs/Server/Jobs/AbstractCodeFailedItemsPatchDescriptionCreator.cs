// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.AbstractCodeFailedItemsPatchDescriptionCreator
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal abstract class AbstractCodeFailedItemsPatchDescriptionCreator : 
    IFailedItemsPatchDescriptionCreator
  {
    private readonly IIndexingUnitChangeEventHandler m_indexingUnitChangeEventHandler;
    protected TraceMetaData m_traceMetaData;

    internal AbstractCodeFailedItemsPatchDescriptionCreator(TraceMetaData traceMetaData)
      : this((IIndexingUnitChangeEventHandler) new IndexingUnitChangeEventHandler(), traceMetaData)
    {
    }

    internal AbstractCodeFailedItemsPatchDescriptionCreator(
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler,
      TraceMetaData traceMetaData)
    {
      this.m_indexingUnitChangeEventHandler = indexingUnitChangeEventHandler ?? throw new ArgumentNullException(nameof (indexingUnitChangeEventHandler));
      this.m_traceMetaData = traceMetaData ?? throw new ArgumentNullException(nameof (traceMetaData));
    }

    internal abstract VersionControlType GetVersionControlType();

    internal abstract void CreatePatchDescription(
      IndexingExecutionContext iexContext,
      string filePath,
      string branchName,
      out List<string> recordsToBeAdded,
      out List<string> recordsToBeDeleted,
      out string patchFile);

    public PatchDescription CreatePatchDescription(
      IndexingExecutionContext iexContext,
      string fileOrDirectoryPath,
      CodeCrawlSpec codeCrawlSpec,
      string branchName,
      TraceMetaData traceMetaData)
    {
      string bePatched = this.HandleIfFolderNeedsToBePatched(iexContext, fileOrDirectoryPath, branchName);
      return bePatched != null ? PatchDescription.CreatePathDescription(bePatched, MetaDataStoreUpdateType.Add, this.GetVersionControlType(), codeCrawlSpec.LastIndexedChangeId, codeCrawlSpec.LastIndexedChangeUtcTime) : (PatchDescription) null;
    }

    internal virtual string HandleIfFolderNeedsToBePatched(
      IndexingExecutionContext iexContext,
      string fileOrDirectoryPath,
      string branchName)
    {
      string patchFile = fileOrDirectoryPath;
      if (!iexContext.RequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return patchFile;
      List<string> recordsToBeAdded;
      List<string> recordsToBeDeleted;
      this.CreatePatchDescription(iexContext, fileOrDirectoryPath, branchName, out recordsToBeAdded, out recordsToBeDeleted, out patchFile);
      if (recordsToBeAdded != null && recordsToBeAdded.Any<string>())
      {
        List<ItemLevelFailureRecord> failedRecords = new List<ItemLevelFailureRecord>();
        foreach (string str in recordsToBeAdded)
        {
          List<ItemLevelFailureRecord> levelFailureRecordList = failedRecords;
          ItemLevelFailureRecord levelFailureRecord = new ItemLevelFailureRecord();
          levelFailureRecord.Item = str;
          levelFailureRecord.AttemptCount = 0;
          FileFailureMetadata fileFailureMetadata = new FileFailureMetadata();
          Branches branches = new Branches();
          branches.Add(branchName);
          fileFailureMetadata.Branches = branches;
          levelFailureRecord.Metadata = (FailureMetadata) fileFailureMetadata;
          levelFailureRecordList.Add(levelFailureRecord);
        }
        iexContext.ItemLevelFailureDataAccess.MergeFailedRecords(iexContext.RequestContext, iexContext.RepositoryIndexingUnit, (IEnumerable<ItemLevelFailureRecord>) failedRecords);
        this.QueueNextFailedFilesProcessingEvent(iexContext);
      }
      if (recordsToBeDeleted != null && recordsToBeDeleted.Any<string>())
      {
        List<ItemLevelFailureRecord> successfullyIndexedRecords = new List<ItemLevelFailureRecord>();
        foreach (string str in recordsToBeDeleted)
        {
          List<ItemLevelFailureRecord> levelFailureRecordList = successfullyIndexedRecords;
          ItemLevelFailureRecord levelFailureRecord = new ItemLevelFailureRecord();
          levelFailureRecord.Item = str;
          levelFailureRecord.AttemptCount = -1;
          FileFailureMetadata fileFailureMetadata = new FileFailureMetadata();
          Branches branches = new Branches();
          branches.Add(branchName);
          fileFailureMetadata.Branches = branches;
          levelFailureRecord.Metadata = (FailureMetadata) fileFailureMetadata;
          levelFailureRecordList.Add(levelFailureRecord);
        }
        iexContext.ItemLevelFailureDataAccess.RemoveSuccessfullyIndexedItemsFromFailedRecords(iexContext.RequestContext, iexContext.RepositoryIndexingUnit, (IEnumerable<ItemLevelFailureRecord>) successfullyIndexedRecords);
      }
      return patchFile;
    }

    internal virtual void QueueNextFailedFilesProcessingEvent(
      IndexingExecutionContext indexingExecutionContext)
    {
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent indexingUnitChangeEvent = new Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEvent()
      {
        IndexingUnitId = indexingExecutionContext.RepositoryIndexingUnit.IndexingUnitId,
        ChangeData = (ChangeEventData) new RepositoryPatchEventData((ExecutionContext) indexingExecutionContext)
        {
          Patch = Patch.ReIndexFailedItems
        },
        ChangeType = "Patch",
        State = IndexingUnitChangeEventState.Pending,
        AttemptCount = 0
      };
      this.m_indexingUnitChangeEventHandler.HandleEventWithAddingEventWhenNeeded((ExecutionContext) indexingExecutionContext, indexingUnitChangeEvent);
    }
  }
}
