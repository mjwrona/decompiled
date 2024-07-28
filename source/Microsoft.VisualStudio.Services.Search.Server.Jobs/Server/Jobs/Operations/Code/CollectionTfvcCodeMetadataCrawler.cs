// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.CollectionTfvcCodeMetadataCrawler
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.HttpClientWrappers;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class CollectionTfvcCodeMetadataCrawler : AbstractCollectionCodeMetadataCrawler
  {
    private ProjectHttpClientWrapper m_projectHttpClientWrapper;
    private TfvcHttpClientWrapper m_tfvcHttpClientWrapper;
    private bool m_isReindexingIsProgress;
    [StaticSafe]
    private static readonly TraceMetaData s_traceMetadata = new TraceMetaData(1080611, AbstractCollectionCodeMetadataCrawler.s_traceArea, AbstractCollectionCodeMetadataCrawler.s_traceLayer);

    [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    internal CollectionTfvcCodeMetadataCrawler(
      IndexingExecutionContext executionContext,
      IDataAccessFactory dataAccessFactory,
      CodeFileContract codeFileContract)
      : this(executionContext, dataAccessFactory, new ProjectHttpClientWrapper((ExecutionContext) executionContext, new TraceMetaData(1080611, AbstractCollectionCodeMetadataCrawler.s_traceArea, AbstractCollectionCodeMetadataCrawler.s_traceLayer)), new TfvcHttpClientWrapper((ExecutionContext) executionContext, new TraceMetaData(1080611, AbstractCollectionCodeMetadataCrawler.s_traceArea, AbstractCollectionCodeMetadataCrawler.s_traceLayer)), codeFileContract)
    {
    }

    internal CollectionTfvcCodeMetadataCrawler(
      IndexingExecutionContext executionContext,
      IDataAccessFactory dataAccessFactory,
      ProjectHttpClientWrapper projectHttpClientWrapper,
      TfvcHttpClientWrapper tfvcHttpClientWrapper,
      CodeFileContract codeFileContract)
      : base(executionContext, dataAccessFactory, "TFVC_Repository", codeFileContract)
    {
      this.m_projectHttpClientWrapper = projectHttpClientWrapper;
      this.m_tfvcHttpClientWrapper = tfvcHttpClientWrapper;
      this.m_isReindexingIsProgress = executionContext.IsReindexingFailedOrInProgress(executionContext.DataAccessFactory, this.EntityType);
    }

    public override List<IndexingUnitWithSize> CrawlMetadata(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit collectionIndexingUnit,
      bool isShadowCrawlingRequired = false)
    {
      IEnumerable<TeamProjectReference> projects = this.m_projectHttpClientWrapper.GetProjects();
      List<IndexingUnitWithSize> indexingUnitWithSizeList = new List<IndexingUnitWithSize>();
      if (projects != null && projects.Any<TeamProjectReference>())
      {
        Dictionary<Guid, int> dictionary = this.CodeIndexingUnitDataAccess.AddOrUpdateIndexingUnits(this.CodeIndexingExecutionContext.RequestContext, projects.Select<TeamProjectReference, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>((Func<TeamProjectReference, Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) (project => project.ToProjectCodeIndexingUnit(collectionIndexingUnit.IndexingUnitId))).ToList<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>(), true).ToDictionary<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid, int>((Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, Guid>) (x => x.TFSEntityId), (Func<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit, int>) (x => x.IndexingUnitId));
        foreach (TeamProjectReference tfvcProject in this.m_tfvcHttpClientWrapper.GetTfvcProjects(projects.ToList<TeamProjectReference>()))
        {
          Microsoft.VisualStudio.Services.Search.Common.IndexingUnit codeIndexingUnit = tfvcProject.ToTfvcRepoCodeIndexingUnit(dictionary[tfvcProject.Id], isShadowCrawlingRequired);
          int estimatedDocCount;
          int estimatedDocCountGrowth;
          this.GetSizeEstimates(indexingExecutionContext, codeIndexingUnit, tfvcProject, out estimatedDocCount, out estimatedDocCountGrowth);
          if (estimatedDocCount > 0)
            indexingUnitWithSizeList.Add(new IndexingUnitWithSize(codeIndexingUnit, estimatedDocCount, estimatedDocCountGrowth, true)
            {
              ActualInitialDocCount = estimatedDocCount
            });
          else
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(CollectionTfvcCodeMetadataCrawler.s_traceMetadata, FormattableString.Invariant(FormattableStringFactory.Create("Skipping {0} project from indexing as it has 0 documents.", (object) tfvcProject.Id)));
        }
      }
      return indexingUnitWithSizeList;
    }

    internal virtual void GetSizeEstimates(
      IndexingExecutionContext indexingExecutionContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit tfvcRepoIndexingUnit,
      TeamProjectReference teamProjectReference,
      out int estimatedDocCount,
      out int estimatedDocCountGrowth)
    {
      this.m_tfvcHttpClientWrapper.GetDocumentCountEstimates(indexingExecutionContext.RequestContext, indexingExecutionContext.ProvisioningContext.ContractType, teamProjectReference.Id, "$/" + teamProjectReference.Name, out estimatedDocCount, out estimatedDocCountGrowth);
      if (!this.m_isReindexingIsProgress)
        return;
      int repoDocCount;
      if (!this.TryGetRepoDocCountFromOlderIndex(indexingExecutionContext, tfvcRepoIndexingUnit, out repoDocCount))
        repoDocCount = 0;
      if (repoDocCount <= estimatedDocCount)
        return;
      float currentHostConfigValue = indexingExecutionContext.RequestContext.GetCurrentHostConfigValue<float>("/Service/ALMSearch/Settings/Routing/Code/CodeEntityTfvcRepositoryGrowthFactor", true, 1f);
      estimatedDocCount = repoDocCount;
      double num = Math.Min((double) repoDocCount * (double) currentHostConfigValue, (double) int.MaxValue);
      estimatedDocCountGrowth = Convert.ToInt32(num);
    }
  }
}
