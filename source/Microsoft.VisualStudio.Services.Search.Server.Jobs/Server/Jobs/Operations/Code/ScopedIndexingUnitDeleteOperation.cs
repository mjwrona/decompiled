// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.ScopedIndexingUnitDeleteOperation
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Arriba;
using Microsoft.VisualStudio.Services.Search.Common.Arriba.Expressions;
using Microsoft.VisualStudio.Services.Search.Crawler.CrawlSpecs;
using Microsoft.VisualStudio.Services.Search.Crawler.MetaDataCrawler.MultiBranchMetaData;
using Microsoft.VisualStudio.Services.Search.Crawler.MetaDataProvider;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using Microsoft.VisualStudio.Services.Search.V2Jobs.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal class ScopedIndexingUnitDeleteOperation : RepositoryDeleteOperation
  {
    public const int BatchSizeToDeleteFilesAtRootLevel = 500;
    private IndexOperations m_indexOperations;

    public ScopedIndexingUnitDeleteOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      IndexingUnit indexingUnit,
      IndexingUnitChangeEvent indexingUnitChangeEvent)
      : this(coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent, new IndexOperations())
    {
    }

    internal ScopedIndexingUnitDeleteOperation(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      IndexingUnit indexingUnit,
      IndexingUnitChangeEvent indexingUnitChangeEvent,
      IndexOperations indexOperations)
      : base(coreIndexingExecutionContext, indexingUnit, indexingUnitChangeEvent)
    {
      this.m_indexOperations = indexOperations;
    }

    internal override IndexOperationsResponse PerformBulkDelete(
      IndexingExecutionContext indexingExecutionContext)
    {
      long startingId;
      long endingId;
      indexingExecutionContext.FileMetadataStoreDataAccess.GetMinAndMaxFilePathIds(indexingExecutionContext.RequestContext, out startingId, out endingId);
      indexingExecutionContext.FileMetadataStoreDataAccess.DeleteFileMetadataRecordsByRange(indexingExecutionContext.RequestContext, startingId, endingId);
      indexingExecutionContext.TempFileMetadataStoreDataAccess.GetMinAndMaxIds(indexingExecutionContext.RequestContext, out startingId, out endingId);
      indexingExecutionContext.TempFileMetadataStoreDataAccess.DeleteRecords(indexingExecutionContext.RequestContext, startingId, endingId);
      return this.DeleteDocumentsFromElasticsearch(indexingExecutionContext);
    }

    internal virtual IndexOperationsResponse DeleteDocumentsFromElasticsearch(
      IndexingExecutionContext indexingExecutionContext)
    {
      IndexIdentity indexIdentity = IndexIdentity.CreateIndexIdentity(indexingExecutionContext.RepositoryIndexingUnit.GetIndexingIndexName());
      List<IExpression> set = new List<IExpression>();
      string str = indexingExecutionContext.IndexingUnit.GetScopePathFromTFSAttributesIfScopedIUElseNull();
      if (str != "/")
      {
        int num = str.IndexOf("/", StringComparison.Ordinal);
        if (num >= 0)
          str = str.Substring(num + 1);
        set.Add((IExpression) new TermExpression("repositoryId", Operator.Equals, indexingExecutionContext.RepositoryId.ToString()));
        set.Add((IExpression) new TermsExpression(CodeContractField.CodeSearchFieldDesc.FilePath.ElasticsearchFieldName(), Operator.In, (IEnumerable<string>) new List<string>()
        {
          str
        }));
        return this.m_indexOperations.PerformBulkDelete(indexingExecutionContext, indexIdentity, (IExpression) new AndExpression((IEnumerable<IExpression>) set), true, true);
      }
      if (indexingExecutionContext.IndexingUnit.Properties.IndexIndices.TrueForAll((Predicate<IndexInfo>) (indexInfo => indexingExecutionContext.ProvisioningContext.SearchPlatform.IndexExists((ExecutionContext) indexingExecutionContext, IndexIdentity.CreateIndexIdentity(indexInfo.IndexName)))))
      {
        ESMultiBranchDocMetadataProviderForRootLevelScopedIndexingUnit scopedIndexingUnit = this.GetEsMultiBranchDocMetadataProviderForRootLevelScopedIndexingUnit(indexingExecutionContext);
        List<string> terms = new List<string>();
        foreach (KeyValuePair<string, MultiBranchDocMetaData> keyValuePair in ((ESMultiBranchDocMetadataProvider) scopedIndexingUnit).GetMetaData())
        {
          terms.Add(keyValuePair.Key);
          if (terms.Count >= 500)
          {
            set.Add((IExpression) new TermExpression("repositoryId", Operator.Equals, indexingExecutionContext.RepositoryId.ToString()));
            set.Add((IExpression) new TermsExpression("filePathOriginal", Operator.In, (IEnumerable<string>) terms));
            this.m_indexOperations.PerformBulkDelete(indexingExecutionContext, indexIdentity, (IExpression) new AndExpression((IEnumerable<IExpression>) set), true, true);
            set = new List<IExpression>();
            terms = new List<string>();
          }
        }
        if (terms.Count > 0)
          return this.m_indexOperations.PerformBulkDelete(indexingExecutionContext, indexIdentity, (IExpression) new AndExpression((IEnumerable<IExpression>) new List<IExpression>()
          {
            (IExpression) new TermExpression("repositoryId", Operator.Equals, indexingExecutionContext.RepositoryId.ToString()),
            (IExpression) new TermsExpression("filePathOriginal", Operator.In, (IEnumerable<string>) terms)
          }), true, true);
      }
      else
        indexingExecutionContext.Log.Append("Skipped deleting any Elasticsearch document because the indices do not exist. This is common during cross-cluster re-indexing.");
      return new IndexOperationsResponse()
      {
        Success = true,
        IsOperationIncomplete = false
      };
    }

    internal virtual ESMultiBranchDocMetadataProviderForRootLevelScopedIndexingUnit GetEsMultiBranchDocMetadataProviderForRootLevelScopedIndexingUnit(
      IndexingExecutionContext indexingExecutionContext)
    {
      GitIndexCrawlSpec gitIndexCrawlSpec1 = new GitIndexCrawlSpec();
      ((CodeCrawlSpec) gitIndexCrawlSpec1).RepositoryId = indexingExecutionContext.RepositoryId.ToString();
      GitIndexCrawlSpec gitIndexCrawlSpec2 = gitIndexCrawlSpec1;
      return new ESMultiBranchDocMetadataProviderForRootLevelScopedIndexingUnit(indexingExecutionContext, (CodeCrawlSpec) gitIndexCrawlSpec2);
    }
  }
}
