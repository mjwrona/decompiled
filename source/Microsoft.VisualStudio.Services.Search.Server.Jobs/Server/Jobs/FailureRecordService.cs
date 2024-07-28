// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.FailureRecordService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Feeder;
using Microsoft.VisualStudio.Services.Search.Feeder.Plugins;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  internal class FailureRecordService : IFailureRecordService
  {
    public const int MAX_ALLOWED_ITEM_LENGTH = 800;
    private readonly StorageContext m_storageContext;
    private readonly TraceMetaData m_traceMetaData;

    internal Dictionary<string, ItemLevelFailureRecord> ItemLevelFailureRecordsDictionary { get; set; }

    public FailureRecordService(StorageContext storageContext)
    {
      this.m_storageContext = storageContext;
      this.m_traceMetaData = new TraceMetaData(1082630, "Indexing Pipeline", "Feed");
      this.ItemLevelFailureRecordsDictionary = new Dictionary<string, ItemLevelFailureRecord>();
    }

    public void Run(
      IndexingExecutionContext indexingExecutionContext,
      ESIndexFeedResponseData feedResponseData)
    {
      foreach (IMetaDataStore metaDataStore in this.m_storageContext.StorageEndpointCollection.MetaDataStoreProvider.GetMetaDataStores())
      {
        if (feedResponseData != null && ((CoreFeedResponseData) feedResponseData).FailedFeedDocuments.Count > 0)
          this.ConstructFailedItemsList(indexingExecutionContext, feedResponseData, metaDataStore);
      }
      this.PersistFailedRecords(indexingExecutionContext, this.ItemLevelFailureRecordsDictionary.Values.ToList<ItemLevelFailureRecord>());
    }

    internal virtual void ConstructFailedItemsList(
      IndexingExecutionContext indexingExecutionContext,
      ESIndexFeedResponseData responseData,
      IMetaDataStore metaDataStore)
    {
      foreach (FeedResponseDocument failedFeedDocument in ((CoreFeedResponseData) responseData).FailedFeedDocuments)
        this.CreateFailureRecord(indexingExecutionContext, failedFeedDocument, metaDataStore);
      Tracer.TraceVerbose(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Successfully created {0} failed item records.", (object) this.ItemLevelFailureRecordsDictionary.Count)));
    }

    internal virtual void PersistFailedRecords(
      IndexingExecutionContext indexingExecutionContext,
      List<ItemLevelFailureRecord> records)
    {
      FailureRecordStore failureRecordStore = indexingExecutionContext.FailureRecordStore;
      records.ForEach((Action<ItemLevelFailureRecord>) (x => failureRecordStore.AddFailedRecord(x)));
    }

    internal void CreateFailureRecord(
      IndexingExecutionContext indexingExecutionContext,
      FeedResponseDocument document,
      IMetaDataStore metadataStore)
    {
      if (document.Item == null)
      {
        Tracer.TraceError(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Found Null document.Item for document {0}. Ignoring it.", (object) document)));
      }
      else
      {
        ItemLevelFailureRecord levelFailureRecord1 = new ItemLevelFailureRecord()
        {
          Reason = document.FailureReason?.ToString(),
          Stage = "Feed"
        };
        switch (indexingExecutionContext.IndexingUnit.EntityType.Name)
        {
          case "Code":
          case "Wiki":
            HashSet<string> source = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
            if (document.UpdateType == MetaDataStoreUpdateType.Add)
            {
              source.UnionWith((IEnumerable<string>) this.GetBranches(document.DocumentId, MetaDataStoreUpdateType.Add, metadataStore, document.MetaDataStoreItem));
              source.UnionWith((IEnumerable<string>) this.GetBranches(document.DocumentId, MetaDataStoreUpdateType.Edit, metadataStore, document.MetaDataStoreItem));
            }
            else
              source.UnionWith((IEnumerable<string>) this.GetBranches(document.DocumentId, document.UpdateType, metadataStore, document.MetaDataStoreItem));
            if (indexingExecutionContext.IndexingUnit.EntityType.Name == "Code" && indexingExecutionContext.RepositoryIndexingUnit.IndexingUnitType == "Git_Repository")
              source.Remove(string.Empty);
            Branches branches1 = new Branches();
            ItemLevelFailureRecord levelFailureRecord2;
            if (this.ItemLevelFailureRecordsDictionary.TryGetValue(document.Item, out levelFailureRecord2))
            {
              Branches branches2 = levelFailureRecord2.Metadata is FileFailureMetadata metadata ? metadata.Branches : (Branches) null;
              if (branches2 != null && branches2.Count > 0)
                source.UnionWith((IEnumerable<string>) branches2);
              branches1.AddRange((IEnumerable<string>) source.ToList<string>());
              (levelFailureRecord2.Metadata as FileFailureMetadata).Branches = branches1;
              levelFailureRecord1 = levelFailureRecord2;
            }
            else
            {
              branches1.AddRange((IEnumerable<string>) source.ToList<string>());
              levelFailureRecord1.Metadata = (FailureMetadata) new FileFailureMetadata()
              {
                Branches = branches1
              };
            }
            levelFailureRecord1.Item = document.Item;
            break;
          case "WorkItem":
            levelFailureRecord1.Item = document.Item;
            levelFailureRecord1.Metadata = (FailureMetadata) new WorkItemFailureMetadata();
            break;
          default:
            throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("Entity type [{0}] is not supported.", (object) indexingExecutionContext.IndexingUnit.EntityType.Name)));
        }
        if (levelFailureRecord1.Item.Length >= 800 && !indexingExecutionContext.ProvisioningContext.ContractType.IsNoPayloadContract())
          Tracer.TraceError(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Length of Item is {0}. Ignoring it.", (object) document.Item)));
        else
          this.ItemLevelFailureRecordsDictionary[document.Item] = levelFailureRecord1;
      }
    }

    internal virtual HashSet<string> GetBranches(
      string documentId,
      MetaDataStoreUpdateType updateType,
      IMetaDataStore metadataStore,
      IMetaDataStoreItem docMetadataStoreItem)
    {
      if (documentId == null)
        throw new ArgumentNullException(nameof (documentId));
      if (metadataStore == null)
        throw new ArgumentNullException(nameof (metadataStore));
      HashSet<string> collection = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      BasicMetaDataStoreItem metaDataStoreItem = (BasicMetaDataStoreItem) docMetadataStoreItem;
      if (docMetadataStoreItem != null)
      {
        collection.AddRange<string, HashSet<string>>((IEnumerable<string>) metaDataStoreItem.GetBranches(metadataStore));
      }
      else
      {
        Tracer.TraceError(this.m_traceMetaData, FormattableString.Invariant(FormattableStringFactory.Create("Could not find metadatastore item for failed item DocId: {0} UpdateType: {1}", (object) documentId, (object) updateType)));
        string str = metadataStore["BranchName"];
        if (str != null)
          collection.Add(str);
      }
      return collection;
    }
  }
}
