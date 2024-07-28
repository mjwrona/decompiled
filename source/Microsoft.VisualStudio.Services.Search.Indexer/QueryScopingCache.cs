// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.QueryScopingCache
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public abstract class QueryScopingCache : IQueryScopingCache
  {
    private volatile QueryScopingCacheStatus m_queryScopingCacheStatus;
    internal readonly IDataAccessFactory m_dataAccessFactory;
    internal CollectionQueryingUnit m_queryingTree;

    public QueryScopingCacheStatus QueryScopingCacheStatus
    {
      get => this.m_queryScopingCacheStatus;
      set => this.m_queryScopingCacheStatus = value;
    }

    protected QueryScopingCache()
      : this(DataAccessFactory.GetInstance())
    {
    }

    internal QueryScopingCache(IDataAccessFactory dataAccessFactory)
    {
      this.m_dataAccessFactory = dataAccessFactory;
      this.QueryScopingCacheStatus = QueryScopingCacheStatus.Undefined;
    }

    [Info("InternalForTestPurpose")]
    internal virtual void ProcessCollectionIndexingUnit(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      this.m_queryingTree = new CollectionQueryingUnit(indexingUnit);
    }

    public virtual void CreateQueryScopingCache(IVssRequestContext requestContext)
    {
      try
      {
        IIndexingUnitDataAccess indexingUnitDataAccess = this.m_dataAccessFactory.GetIndexingUnitDataAccess();
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = this.GetIndexingUnits(requestContext, indexingUnitDataAccess);
        indexingUnits.Sort((Comparison<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>) ((a, b) => a.IndexingUnitId.CompareTo(b.IndexingUnitId)));
        if (!this.IsCacheCreationViableOrPossible(indexingUnits))
          return;
        this.ProcessIndexingUnits(requestContext, indexingUnits);
      }
      catch (Exception ex)
      {
        string str = ex.ToString();
        this.QueryScopingCacheStatus = QueryScopingCacheStatus.CacheCreationFailed;
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081466, "Query Pipeline", "DocumentContractTypeService", FormattableString.Invariant(FormattableStringFactory.Create("Query Caching failed. Exception: {0}", (object) str)));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1081466, "Query Pipeline", "DocumentContractTypeService", FormattableString.Invariant(FormattableStringFactory.Create("Final Status of cache {0}", (object) this.QueryScopingCacheStatus.ToString())));
      }
    }

    public QueryingUnit GetCacheRoot() => (QueryingUnit) this.m_queryingTree;

    public void UpdateCacheRootName(string cacheRootName)
    {
      CollectionQueryingUnit collectionQueryingUnit = (CollectionQueryingUnit) this.m_queryingTree.Clone();
      collectionQueryingUnit.EntityName = cacheRootName;
      this.m_queryingTree = collectionQueryingUnit;
    }

    public void ClearCache()
    {
      this.QueryScopingCacheStatus = QueryScopingCacheStatus.Undefined;
      this.m_queryingTree = (CollectionQueryingUnit) null;
    }

    protected abstract void ProcessIndexingUnits(
      IVssRequestContext requestContext,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits);

    protected abstract List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetIndexingUnits(
      IVssRequestContext requestContext,
      IIndexingUnitDataAccess indexingUnitDataAccess);

    [Info("InternalForTestPurpose")]
    internal abstract bool IsCacheCreationViableOrPossible(List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits);
  }
}
