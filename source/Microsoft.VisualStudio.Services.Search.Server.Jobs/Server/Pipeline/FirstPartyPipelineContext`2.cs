// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.FirstPartyPipelineContext`2
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using System;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  public class FirstPartyPipelineContext<TId, TDoc> : CorePipelineContext<TId, TDoc>, IDisposable
    where TId : IEquatable<TId>
    where TDoc : CorePipelineDocument<TId>
  {
    private IFailureRecordService m_failureRecordService;
    private readonly bool m_disposeStorageContext;
    private bool m_disposedValue;

    public FirstPartyPipelineContext(
      IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Indexer.IndexingExecutionContext indexingExecutionContext,
      CoreCrawlSpec crawlSpec,
      IndexingUnitChangeEvent indexingUnitChangeEvent,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler,
      bool isSingleExecutionPipeline)
      : this(indexingUnit, indexingExecutionContext, crawlSpec, indexingUnitChangeEvent, indexingUnitChangeEventHandler, new StorageContext(indexingExecutionContext.RequestContext, indexingExecutionContext.IndexingUnit.TFSEntityId, indexingExecutionContext.IndexingUnit.EntityType), isSingleExecutionPipeline)
    {
      this.m_disposeStorageContext = true;
    }

    protected internal FirstPartyPipelineContext(
      IndexingUnit indexingUnit,
      Microsoft.VisualStudio.Services.Search.Indexer.IndexingExecutionContext indexingExecutionContext,
      CoreCrawlSpec crawlSpec,
      IndexingUnitChangeEvent indexingUnitChangeEvent,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler,
      StorageContext storageContext,
      bool isSingleExecutionPipeline)
      : base(indexingUnit, (CoreIndexingExecutionContext) indexingExecutionContext, isSingleExecutionPipeline ? TimeSpan.Zero : TimeSpan.FromSeconds((double) indexingExecutionContext.ServiceSettings.JobSettings.MaxAllowedOnPremiseJobExecutionTimeInSec))
    {
      this.IndexingExecutionContext = indexingExecutionContext;
      this.CrawlSpec = crawlSpec;
      this.IndexingUnitChangeEvent = indexingUnitChangeEvent;
      this.IndexingUnitChangeEventHandler = indexingUnitChangeEventHandler;
      this.StorageContext = storageContext;
      this.m_disposeStorageContext = false;
    }

    public virtual Microsoft.VisualStudio.Services.Search.Indexer.IndexingExecutionContext IndexingExecutionContext { get; }

    public CoreCrawlSpec CrawlSpec { get; }

    public IndexingUnitChangeEvent IndexingUnitChangeEvent { get; }

    public virtual IIndexingUnitChangeEventHandler IndexingUnitChangeEventHandler { get; }

    internal virtual IFailureRecordService FailureRecordService() => this.m_failureRecordService ?? (this.m_failureRecordService = (IFailureRecordService) new Microsoft.VisualStudio.Services.Search.Server.Jobs.FailureRecordService(this.StorageContext));

    public StorageContext StorageContext { get; private set; }

    protected override void Dispose(bool disposing)
    {
      if (!this.m_disposedValue)
      {
        if (disposing && this.m_disposeStorageContext && this.StorageContext != null)
        {
          this.StorageContext.Dispose();
          this.StorageContext = (StorageContext) null;
        }
        this.m_disposedValue = true;
      }
      base.Dispose(disposing);
    }
  }
}
