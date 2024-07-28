// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Store.StoreService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Store, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 88C5F689-5CBE-419A-B234-7228E63B94DF
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Store.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Store
{
  public class StoreService : IStoreService
  {
    private IndexingExecutionContext m_indexingExecutionContext;
    private StorageContext m_storageContext;
    private int m_numOfFilesStored;
    private int m_numOfFilesFailedToStore;
    private int m_numOfFilesDeleted;
    private int m_numOfFilesFailedToDelete;

    public StoreService(IndexingExecutionContext executionContext, StorageContext storageContext)
      : this(executionContext, storageContext, (IFileStoreService) FileStoreService.Instance)
    {
    }

    internal StoreService(
      IndexingExecutionContext executionContext,
      StorageContext storageContext,
      IFileStoreService fileStoreService)
    {
      this.m_indexingExecutionContext = executionContext;
      this.Initialize(storageContext, fileStoreService);
    }

    internal IFileStoreService FileStore { get; set; }

    public void Run(IndexingExecutionContext indexingExecutionContext)
    {
      Tracer.TraceEnter(1080550, "Indexing Pipeline", "Store", "StoreService.Run");
      Stopwatch stopwatch = Stopwatch.StartNew();
      PipelineDocumentCollection<CodePipelineDocumentId, CodeDocument> pipelineDocs = this.GetPipelineDocs();
      try
      {
        foreach (IMetaDataStore metaDataStore in this.m_storageContext.StorageEndpointCollection.MetaDataStoreProvider.GetMetaDataStores())
        {
          IStorageEndpoint crawlStore = this.m_storageContext.StorageEndpointCollection.CrawlStore;
          IEnumerator<IMetaDataStoreItem> enumerator = metaDataStore.Cast<IMetaDataStoreItem>().GetEnumerator();
          while (enumerator.MoveNext())
          {
            IMetaDataStoreItem current = enumerator.Current;
            if (current.UpdateType == MetaDataStoreUpdateType.Delete)
              this.UpdatePipelineDocumentForAllBranches(pipelineDocs, current.Path, current.BranchesInfo, (string) null);
            else
              this.AddFile(metaDataStore, crawlStore, current);
          }
        }
        if (this.m_numOfFilesFailedToDelete != 0 || this.m_numOfFilesFailedToStore != 0)
          throw new StoreException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Store service failed to process successfully all items. No. of items failed to store [{0}]. No. of items failed to delete [{1}]", (object) this.m_numOfFilesFailedToStore, (object) this.m_numOfFilesFailedToDelete));
      }
      catch (Exception ex)
      {
        Tracer.TraceError(1080551, "Indexing Pipeline", "Store", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed while processing the store request. Exception : {0} ", (object) ex.ToString()));
        Tracer.PublishKpi("StoreServiceFailure", "Indexing Pipeline", 1.0);
        this.m_indexingExecutionContext.FaultService.SetError(ex);
        if (StoreUtility.IsNonWrappedException(ex))
          ExceptionDispatchInfo.Capture(ex).Throw();
        StoreUtility.WrapAndThrowException(ex, (string) null);
      }
      finally
      {
        indexingExecutionContext.ExecutionTracerContext.PublishKpi("StoringTime", "Indexing Pipeline", (double) stopwatch.ElapsedMilliseconds);
        indexingExecutionContext.ExecutionTracerContext.PublishKpi("NumOfFilesStored", "Indexing Pipeline", (double) this.m_numOfFilesStored);
        indexingExecutionContext.ExecutionTracerContext.PublishKpi("NumOfFilesDeletedFromStore", "Indexing Pipeline", (double) this.m_numOfFilesDeleted);
        indexingExecutionContext.ExecutionTracerContext.PublishKpi("NumOfFilesFailedToStore", "Indexing Pipeline", (double) this.m_numOfFilesFailedToStore);
        indexingExecutionContext.ExecutionTracerContext.PublishKpi("NumOfFilesFailedToDeleteFromStore", "Indexing Pipeline", (double) this.m_numOfFilesFailedToDelete);
        Tracer.TraceLeave(1080599, "Indexing Pipeline", "Store", "StoreService.Run");
      }
    }

    private void AddFile(
      IMetaDataStore metaDataStore,
      IStorageEndpoint crawlStore,
      IMetaDataStoreItem currentItem)
    {
      Hash id = new Hash(currentItem.ContentId.RawHash);
      PipelineDocumentCollection<CodePipelineDocumentId, CodeDocument> pipelineDocs = this.GetPipelineDocs();
      IObjectStoreItem objectStoreItem;
      if (!crawlStore.TryGet(new ContentId(id), out objectStoreItem))
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Store Service : Failed to get crawl store file for objectId: '{0}' and path: '{1}'", (object) currentItem.ContentId.HexHash, (object) currentItem.Path);
        this.UpdatePipelineDocumentForAllBranches(pipelineDocs, currentItem.Path, currentItem.BranchesInfo, "FailedToFindFileInCrawlStore");
        throw new InvalidDataException(message);
      }
      try
      {
        string hexHash = currentItem.ContentId.HexHash;
        if (hexHash == null)
          return;
        if (!this.FileStore.FileExists(this.m_indexingExecutionContext.RequestContext, metaDataStore["ProjectName"], metaDataStore["RepoName"], hexHash, currentItem.Path.NormalizePath()))
        {
          this.FileStore.AddFile(this.m_indexingExecutionContext.RequestContext, metaDataStore["ProjectName"], metaDataStore["RepoName"], hexHash, currentItem.Path.NormalizePath(), objectStoreItem.Blob);
          ++this.m_numOfFilesStored;
        }
        this.UpdatePipelineDocumentForAllBranches(pipelineDocs, currentItem.Path, currentItem.BranchesInfo, (string) null);
      }
      catch (Exception ex)
      {
        Tracer.TraceWarning(1080553, "Indexing Pipeline", "Store", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Failed to add file {0} to store in project [{1}], repo [{2}], branch [{3}]. Exception details: {4}", (object) currentItem.Path, (object) metaDataStore["ProjectName"], (object) metaDataStore["RepoName"], (object) metaDataStore["BranchName"], (object) ex.ToString()));
        this.UpdatePipelineDocumentForAllBranches(pipelineDocs, currentItem.Path, currentItem.BranchesInfo, "FailedToAddFileToContentStore");
        ++this.m_numOfFilesFailedToStore;
      }
    }

    internal virtual void UpdatePipelineDocumentForAllBranches(
      PipelineDocumentCollection<CodePipelineDocumentId, CodeDocument> pipelineDocs,
      string filePath,
      List<BranchInfo> branchList,
      string auditTrailMessage)
    {
      foreach (BranchInfo branch in branchList)
      {
        CodePipelineDocumentId docId = new CodePipelineDocumentId(filePath, branch.BranchName);
        CodeDocument doc;
        if (pipelineDocs.TryGetStrict(docId, out doc))
        {
          if (auditTrailMessage != null)
            doc.AuditTrail.Record(auditTrailMessage);
          else
            doc.CurrentState = PipelineDocumentState.FedToCustomStore;
        }
      }
    }

    public virtual PipelineDocumentCollection<CodePipelineDocumentId, CodeDocument> GetPipelineDocs() => GlobalPipelineContext.Get<CodePipelineDocumentId, CodeDocument>().PipelineDocs;

    private void Initialize(StorageContext storageContext, IFileStoreService fileStoreService)
    {
      this.m_storageContext = storageContext;
      this.FileStore = fileStoreService;
      this.m_numOfFilesStored = 0;
      this.m_numOfFilesFailedToStore = 0;
      this.m_numOfFilesDeleted = 0;
      this.m_numOfFilesFailedToDelete = 0;
    }
  }
}
