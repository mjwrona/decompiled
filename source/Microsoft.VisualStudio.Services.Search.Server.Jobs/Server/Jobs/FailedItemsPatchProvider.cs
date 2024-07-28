// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.FailedItemsPatchProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal class FailedItemsPatchProvider : IIndexPatchProvider
  {
    private readonly TraceMetaData m_traceMetaData;
    private Dictionary<string, List<PatchDescription>> m_branchNameToPatchDescriptions;
    private readonly FailedItemsPatchDescriptionCreatorFactory m_failedItemsPatchDescriptionCreatorFactory;

    internal FailedItemsPatchProvider(TraceMetaData traceMetaData)
      : this(traceMetaData, new FailedItemsPatchDescriptionCreatorFactory())
    {
    }

    internal FailedItemsPatchProvider(
      TraceMetaData traceMetaData,
      FailedItemsPatchDescriptionCreatorFactory failedItemsPatchDescriptionCreatorFactory)
    {
      this.m_traceMetaData = traceMetaData;
      this.m_failedItemsPatchDescriptionCreatorFactory = failedItemsPatchDescriptionCreatorFactory;
    }

    public IEnumerable<PatchDescription> GetPatches(
      IndexingExecutionContext iexContext,
      string branchName,
      CodeCrawlSpec codeCrawlSpec,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      this.m_branchNameToPatchDescriptions = this.CreatePatchDescriptions(iexContext, codeCrawlSpec);
      List<PatchDescription> patches;
      if (!this.m_branchNameToPatchDescriptions.TryGetValue(branchName, out patches))
        patches = new List<PatchDescription>();
      return (IEnumerable<PatchDescription>) patches;
    }

    public void PostPatchOperation(
      IndexingExecutionContext iexContext,
      string branchName,
      IEnumerable<PatchDescription> patchDescriptions)
    {
      List<ItemLevelFailureRecord> itemFailureRecords = this.GetSuccessfullyIndexedItemFailureRecords(branchName, patchDescriptions, iexContext.FailureRecordStore.GetFailedRecords());
      if (!itemFailureRecords.Any<ItemLevelFailureRecord>())
        return;
      iexContext.ItemLevelFailureDataAccess.RemoveSuccessfullyIndexedItemsFromFailedRecords(iexContext.RequestContext, iexContext.IndexingUnit, (IEnumerable<ItemLevelFailureRecord>) itemFailureRecords);
      this.PublishSucussfulRecordsKpi(itemFailureRecords, iexContext);
    }

    internal virtual void PublishSucussfulRecordsKpi(
      List<ItemLevelFailureRecord> successfullyIndexedRecords,
      IndexingExecutionContext iexContext)
    {
      Tracer.PublishKpi("TotalNumberOfFailedItemsReIndexedSuccessfully", "Indexing Pipeline", (double) successfullyIndexedRecords.Count, true);
      int num1 = int.MaxValue;
      int num2 = -1;
      Dictionary<int, int> dictionary = new Dictionary<int, int>();
      foreach (ItemLevelFailureRecord successfullyIndexedRecord in successfullyIndexedRecords)
      {
        num1 = Math.Min(num1, successfullyIndexedRecord.AttemptCount);
        num2 = Math.Max(num2, successfullyIndexedRecord.AttemptCount);
        if (!dictionary.TryAdd<int, int>(successfullyIndexedRecord.AttemptCount, 1))
          dictionary[successfullyIndexedRecord.AttemptCount]++;
      }
      double num3 = (double) dictionary[num1] * 1.0 / (double) successfullyIndexedRecords.Count * 100.0;
      double num4 = (double) dictionary[num2] * 1.0 / (double) successfullyIndexedRecords.Count * 100.0;
      ClientTraceData clientTraceData = new ClientTraceData();
      clientTraceData.Add("TotalNumberOfFailedItemsReIndexedSuccessfully", (object) successfullyIndexedRecords.Count);
      clientTraceData.Add("MinAttemptCountToPatchFailedFiles", (object) num1);
      clientTraceData.Add("MaxAttemptCountToPatchFailedFiles", (object) num2);
      clientTraceData.Add("PercentageOfFailedFilesPatchedWithMinAttemptCount", (object) num3);
      clientTraceData.Add("PercentageOfFailedFilesPatchedWithMaxAttemptCount", (object) num4);
      iexContext.ExecutionTracerContext.PublishClientTrace(this.m_traceMetaData.TraceArea, this.m_traceMetaData.TraceLayer, clientTraceData);
    }

    internal virtual Dictionary<string, List<PatchDescription>> CreatePatchDescriptions(
      IndexingExecutionContext iexContext,
      CodeCrawlSpec codeCrawlSpec)
    {
      int itemsMaxRetryCount = iexContext.ServiceSettings.JobSettings.FailedItemsMaxRetryCount;
      int attributesIfGitRepo = iexContext.IndexingUnit.GetBranchCountFromTFSAttributesIfGitRepo();
      if (attributesIfGitRepo > 0)
        itemsMaxRetryCount *= attributesIfGitRepo;
      int failedItemsToProcess = iexContext.ServiceSettings.JobSettings.MaxNumberOfFailedItemsToProcess;
      IEnumerable<ItemLevelFailureRecord> levelFailureRecords = iexContext.ItemLevelFailureDataAccess.GetItemsWithMaxAttemptCount(iexContext.RequestContext, iexContext.IndexingUnit, itemsMaxRetryCount, failedItemsToProcess) ?? (IEnumerable<ItemLevelFailureRecord>) new List<ItemLevelFailureRecord>();
      Dictionary<string, List<PatchDescription>> patchDescriptions = new Dictionary<string, List<PatchDescription>>();
      IFailedItemsPatchDescriptionCreator descriptionCreator = this.GetPatchDescriptionCreator(iexContext, codeCrawlSpec);
      foreach (ItemLevelFailureRecord levelFailureRecord in levelFailureRecords)
      {
        if (levelFailureRecord.Metadata is FileFailureMetadata metadata)
        {
          Branches branches = metadata.Branches;
          if (branches != null && branches.Count >= 1)
          {
            foreach (string str1 in (List<string>) branches)
            {
              string str2 = string.IsNullOrWhiteSpace(str1) ? string.Empty : str1;
              List<PatchDescription> patchDescriptionList;
              if (!patchDescriptions.TryGetValue(str2, out patchDescriptionList))
              {
                patchDescriptionList = new List<PatchDescription>();
                patchDescriptions[str2] = patchDescriptionList;
              }
              PatchDescription patchDescription = descriptionCreator.CreatePatchDescription(iexContext, levelFailureRecord.Item, codeCrawlSpec, str2, this.m_traceMetaData);
              if (patchDescription != null)
              {
                patchDescription.AttemptCount = levelFailureRecord.AttemptCount;
                patchDescriptionList.Add(patchDescription);
              }
            }
          }
        }
      }
      return patchDescriptions;
    }

    internal List<ItemLevelFailureRecord> GetSuccessfullyIndexedItemFailureRecords(
      string branchName,
      IEnumerable<PatchDescription> previousFailedFiles,
      IEnumerable<ItemLevelFailureRecord> currentFailedRecords)
    {
      List<ItemLevelFailureRecord> itemFailureRecords = new List<ItemLevelFailureRecord>();
      if (previousFailedFiles == null)
        return itemFailureRecords;
      HashSet<string> stringSet = new HashSet<string>();
      if (currentFailedRecords != null)
      {
        foreach (ItemLevelFailureRecord currentFailedRecord in currentFailedRecords)
          stringSet.Add(currentFailedRecord.Item);
      }
      foreach (PatchDescription previousFailedFile in previousFailedFiles)
      {
        if (!stringSet.Contains(previousFailedFile.FilePath))
        {
          List<ItemLevelFailureRecord> levelFailureRecordList = itemFailureRecords;
          ItemLevelFailureRecord levelFailureRecord = new ItemLevelFailureRecord();
          levelFailureRecord.Item = previousFailedFile.FilePath;
          levelFailureRecord.AttemptCount = previousFailedFile.AttemptCount;
          FileFailureMetadata fileFailureMetadata = new FileFailureMetadata();
          Branches branches = new Branches();
          branches.Add(branchName);
          fileFailureMetadata.Branches = branches;
          levelFailureRecord.Metadata = (FailureMetadata) fileFailureMetadata;
          levelFailureRecordList.Add(levelFailureRecord);
        }
      }
      return itemFailureRecords;
    }

    internal virtual IFailedItemsPatchDescriptionCreator GetPatchDescriptionCreator(
      IndexingExecutionContext indexingExecutionContext,
      CodeCrawlSpec codeCrawlSpec)
    {
      return this.m_failedItemsPatchDescriptionCreatorFactory.GetPatchDescriptorCreator(indexingExecutionContext, codeCrawlSpec, this.m_traceMetaData);
    }
  }
}
