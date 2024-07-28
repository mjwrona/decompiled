// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code.AbstractCollectionCodeMetadataCrawler
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Code;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code
{
  internal abstract class AbstractCollectionCodeMetadataCrawler : ICollectionMetadataCrawler
  {
    protected static readonly string s_traceArea = "Indexing Pipeline";
    protected static readonly string s_traceLayer = "IndexingOperation";

    internal AbstractCollectionCodeMetadataCrawler(
      IndexingExecutionContext executionContext,
      IDataAccessFactory dataAccessFactory,
      string indexingUnitType,
      CodeFileContract codeFileContract)
    {
      this.CodeIndexingExecutionContext = executionContext;
      this.CodeIndexingUnitType = indexingUnitType;
      this.DataAccessFactory = dataAccessFactory;
      this.CodeIndexingUnitDataAccess = dataAccessFactory.GetIndexingUnitDataAccess();
      this.CodeFileContract = codeFileContract;
    }

    internal IDataAccessFactory DataAccessFactory { get; set; }

    protected string CodeIndexingUnitType { get; set; }

    protected IndexingExecutionContext CodeIndexingExecutionContext { get; set; }

    protected IIndexingUnitDataAccess CodeIndexingUnitDataAccess { get; set; }

    protected CodeFileContract CodeFileContract { get; }

    protected IEntityType EntityType => (IEntityType) CodeEntityType.GetInstance();

    public abstract List<IndexingUnitWithSize> CrawlMetadata(
      IndexingExecutionContext indexingExecutionContext,
      IndexingUnit indexingUnit,
      bool isShadowCrawlingRequired = false);

    protected IDictionary<Guid, IndexingUnit> GetExistingRepoIndexingUnits(
      bool isShadowCrawlingRequired = false)
    {
      return (IDictionary<Guid, IndexingUnit>) (this.CodeIndexingUnitDataAccess.GetIndexingUnits(this.CodeIndexingExecutionContext.RequestContext, this.CodeIndexingUnitType, isShadowCrawlingRequired, (IEntityType) CodeEntityType.GetInstance(), -1) ?? new List<IndexingUnit>()).ToDictionary<IndexingUnit, Guid, IndexingUnit>((Func<IndexingUnit, Guid>) (indexingUnit => indexingUnit.TFSEntityId), (Func<IndexingUnit, IndexingUnit>) (indexingUnit2 => indexingUnit2));
    }

    internal virtual bool TryGetRepoDocCountFromOlderIndex(
      IndexingExecutionContext indexingExecutionContext,
      IndexingUnit repoIndexingUnit,
      out int repoDocCount)
    {
      try
      {
        IEnumerable<IndexInfo> indexIndices = (IEnumerable<IndexInfo>) repoIndexingUnit.Properties.IndexIndices;
        if (indexIndices.IsNullOrEmpty<IndexInfo>())
          indexIndices = (IEnumerable<IndexInfo>) indexingExecutionContext.CollectionIndexingUnit.Properties.IndexIndices;
        int result;
        if (!int.TryParse(this.CodeFileContract.GetRepoDocCount(indexingExecutionContext.RequestContext, repoIndexingUnit.TFSEntityId, indexIndices).ToString((IFormatProvider) CultureInfo.InvariantCulture), out result))
          result = int.MaxValue;
        repoDocCount = result;
        return true;
      }
      catch (Exception ex)
      {
        indexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Unable to get estimates from ES. Message: {0}", (object) ex.Message)));
        repoDocCount = 0;
        return false;
      }
    }
  }
}
