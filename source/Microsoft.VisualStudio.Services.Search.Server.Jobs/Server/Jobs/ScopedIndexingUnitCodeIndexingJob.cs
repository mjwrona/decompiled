// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.ScopedIndexingUnitCodeIndexingJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class ScopedIndexingUnitCodeIndexingJob : AbstractSearchServiceIndexingJob
  {
    protected override int TracePoint => 1080250;

    protected override void NotifyIndexingUnitChangeEventProcessor()
    {
      if (((Microsoft.VisualStudio.Services.Search.Indexer.IndexingExecutionContext) this.IndexingExecutionContext).RepositoryIndexingUnit.IndexingUnitType == "CustomRepository")
        this.IndexingExecutionContext.RequestContext.GetService<IIndexingUnitChangeEventProcessor>().Process((ExecutionContext) this.IndexingExecutionContext, this.IndexingUnit.IndexingUnitId);
      else
        base.NotifyIndexingUnitChangeEventProcessor();
    }

    public override bool Initialize(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      StringBuilder resultMessage)
    {
      if (!base.Initialize(requestContext, jobDefinition, resultMessage))
        return false;
      if (((Microsoft.VisualStudio.Services.Search.Indexer.IndexingExecutionContext) this.IndexingExecutionContext).RepositoryIndexingUnit.IndexingUnitType == "CustomRepository")
      {
        this.IndexingExecutionContext.EventProcessingContext.EventProcessingBatchSize = this.IndexingExecutionContext.ServiceSettings.JobSettings.BatchCountForCustomRepoProcessing;
        this.IndexingExecutionContext.EventProcessingContext.EventsQueryBatchSize = this.IndexingExecutionContext.ServiceSettings.JobSettings.BatchCountForCustomRepoProcessing;
        this.IndexingExecutionContext.EventProcessingContext.IndexingUnitChangeEventSelector = (IIndexingUnitChangeEventSelector) new CreationTimeBasedIndexingUnitChangeEventSelector();
      }
      return true;
    }
  }
}
