// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.CodeQueryScopingCache
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public class CodeQueryScopingCache : QueryScopingCache
  {
    public CodeQueryScopingCache()
    {
    }

    internal CodeQueryScopingCache(IDataAccessFactory dataAccessFactory)
      : base(dataAccessFactory)
    {
    }

    [Info("InternalForTestPurpose")]
    internal override bool IsCacheCreationViableOrPossible(List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits)
    {
      if (indexingUnits == null || indexingUnits.Count < 3)
      {
        this.QueryScopingCacheStatus = QueryScopingCacheStatus.CacheCreationNotPossible;
        return false;
      }
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnits[0];
      if (indexingUnit?.IndexingUnitType != "Collection")
      {
        this.QueryScopingCacheStatus = QueryScopingCacheStatus.CacheCreationNotPossible;
        return false;
      }
      if (indexingUnit.Properties?.QueryIndices == null || !indexingUnit.Properties.QueryIndices.Any<IndexInfo>())
      {
        this.QueryScopingCacheStatus = QueryScopingCacheStatus.CacheNotNeeded;
        return false;
      }
      if (indexingUnit != null)
      {
        int? count = indexingUnit.Properties?.QueryIndices?.Count;
        int num = 1;
        if (count.GetValueOrDefault() == num & count.HasValue)
        {
          string routing = indexingUnit.Properties.QueryIndices.First<IndexInfo>().Routing;
          if (string.IsNullOrWhiteSpace(routing))
          {
            this.QueryScopingCacheStatus = QueryScopingCacheStatus.CacheCreationNotPossible;
            return false;
          }
          if (routing.Trim().Split(',').Length == 1)
          {
            this.QueryScopingCacheStatus = QueryScopingCacheStatus.CacheNotNeeded;
            return false;
          }
        }
      }
      this.QueryScopingCacheStatus = QueryScopingCacheStatus.CacheCreationPossible;
      return true;
    }

    protected List<string> GetSupportedIndexingUnitTypes() => new List<string>()
    {
      "Collection",
      "Project",
      "Git_Repository",
      "TFVC_Repository",
      "CustomRepository"
    };

    [Info("InternalForTestPurpose")]
    internal override void ProcessCollectionIndexingUnit(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit)
    {
      base.ProcessCollectionIndexingUnit(requestContext, indexingUnit);
      this.m_queryingTree.LargeRepoIds = requestContext.GetLargeRepoIdSet();
      if (!requestContext.IsAccountWithLargeRepository())
        return;
      this.m_queryingTree.CollectionWithLargeRepo = true;
    }

    protected override List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> GetIndexingUnits(
      IVssRequestContext requestContext,
      IIndexingUnitDataAccess indexingUnitDataAccess)
    {
      List<string> indexingUnitTypes = this.GetSupportedIndexingUnitTypes();
      return indexingUnitDataAccess.GetIndexingUnitsRoutingInfo(requestContext, (IEntityType) CodeEntityType.GetInstance(), indexingUnitTypes);
    }

    private void ProcessIndexingUnit(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IDictionary<int, QueryingUnit> routingUnitMap,
      List<int> largeRepoIndexingUnitId)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (routingUnitMap == null)
        throw new ArgumentNullException(nameof (routingUnitMap));
      if (largeRepoIndexingUnitId == null)
        throw new ArgumentNullException(nameof (largeRepoIndexingUnitId));
      QueryingUnit queryingUnit = this.CacheRoutingUnits(indexingUnit, routingUnitMap);
      if (queryingUnit == null)
        return;
      this.UpdateAndMaintainLargeRepoInformation(indexingUnit, largeRepoIndexingUnitId, queryingUnit);
      if (!(indexingUnit.IndexingUnitType == "Project") && !queryingUnit.IsLargeRepo)
        return;
      routingUnitMap.Add(indexingUnit.IndexingUnitId, queryingUnit);
    }

    private QueryingUnit CacheRoutingUnits(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      IDictionary<int, QueryingUnit> RoutingUnitMap)
    {
      if (indexingUnit == null)
        throw new ArgumentNullException(nameof (indexingUnit));
      if (RoutingUnitMap == null)
        throw new ArgumentNullException(nameof (RoutingUnitMap));
      QueryingUnit queryingUnit1 = new QueryingUnit(indexingUnit);
      this.CreateQueryIndexInfos(indexingUnit, queryingUnit1);
      QueryingUnit queryingUnit2 = indexingUnit.IndexingUnitType == "Project" ? (QueryingUnit) this.m_queryingTree : RoutingUnitMap.GetValueOrDefault<int, QueryingUnit>(indexingUnit.ParentUnitId);
      if (queryingUnit2 == null)
        return (QueryingUnit) null;
      queryingUnit2.AddChildRoutingUnit(queryingUnit1);
      return queryingUnit1;
    }

    private void ProcessScopedPathForLargeRepositories(
      IVssRequestContext requestContext,
      List<int> largeRepoIndexingUnitIds,
      IDictionary<int, QueryingUnit> routingUnitMap)
    {
      IIndexingUnitDataAccess indexingUnitDataAccess = this.m_dataAccessFactory.GetIndexingUnitDataAccess();
      if (!this.m_queryingTree.CollectionWithLargeRepo)
        return;
      foreach (int repoIndexingUnitId in largeRepoIndexingUnitIds)
      {
        List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> unitsRoutingInfo = indexingUnitDataAccess.GetChildIndexingUnitsRoutingInfo(requestContext, "ScopedIndexingUnit", repoIndexingUnitId);
        for (int index = 0; index < unitsRoutingInfo.Count; ++index)
          this.CacheRoutingUnits(unitsRoutingInfo[index], routingUnitMap);
      }
    }

    [Info("InternalForTestPurpose")]
    internal void CreateQueryIndexInfos(Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit, QueryingUnit queryingUnit)
    {
      IndexInfo indexInfo = indexingUnit.GetIndexInfo();
      if (!(indexingUnit.IndexingUnitType != "Project") || indexInfo == null || indexInfo.IndexName == null)
        return;
      queryingUnit.QueryIndexInfos = (IEnumerable<IndexInfo>) this.m_queryingTree.GetIndexInfo(indexInfo);
    }

    private void UpdateAndMaintainLargeRepoInformation(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      List<int> largeRepoIdList,
      QueryingUnit queryingUnit)
    {
      if (this.m_queryingTree.CollectionWithLargeRepo)
      {
        queryingUnit.IsLargeRepo = this.m_queryingTree.LargeRepoIds.Contains(indexingUnit.TFSEntityId.ToString());
        if (!queryingUnit.IsLargeRepo)
          return;
        largeRepoIdList.Add(indexingUnit.IndexingUnitId);
      }
      else
        queryingUnit.IsLargeRepo = false;
    }

    private bool IsRepoAlreadyCachedAndUpdated(
      QueryingUnit projectRoutingUnit,
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit newIndexingUnit)
    {
      if (projectRoutingUnit == null || !this.IsRepoCached(projectRoutingUnit, newIndexingUnit.GetRepositoryNameFromTFSAttributes()))
        return false;
      IDictionary<string, QueryingUnit> childRoutingUnits = projectRoutingUnit.ChildRoutingUnits;
      IEnumerable<IndexInfo> queryIndexInfos = (childRoutingUnits != null ? childRoutingUnits.GetValueOrDefault<string, QueryingUnit>(newIndexingUnit.GetRepositoryNameFromTFSAttributes()) : (QueryingUnit) null).QueryIndexInfos;
      IndexInfo indexInfo1 = queryIndexInfos != null ? queryIndexInfos.FirstOrDefault<IndexInfo>() : (IndexInfo) null;
      IndexInfo indexInfo2 = newIndexingUnit?.GetIndexInfo();
      if (indexInfo1 == null && indexInfo2 == null)
        return false;
      if (indexInfo1 != null)
        ;
      return indexInfo1.Equals((object) indexInfo2);
    }

    private bool IsRepoCached(QueryingUnit projectRoutingUnit, string repositoryName)
    {
      IDictionary<string, QueryingUnit> childRoutingUnits = projectRoutingUnit.ChildRoutingUnits;
      return (childRoutingUnits != null ? childRoutingUnits.GetValueOrDefault<string, QueryingUnit>(repositoryName) : (QueryingUnit) null) != null;
    }

    internal void ProcessProjectRouting(ICollection<QueryingUnit> projectQueryingUnits)
    {
      foreach (QueryingUnit projectQueryingUnit in (IEnumerable<QueryingUnit>) projectQueryingUnits)
      {
        try
        {
          CollectionQueryingUnit queryingTree = this.m_queryingTree;
          IndexInfo indexInfo1;
          if (queryingTree == null)
          {
            indexInfo1 = (IndexInfo) null;
          }
          else
          {
            IEnumerable<IndexInfo> queryIndexInfos = queryingTree.QueryIndexInfos;
            indexInfo1 = queryIndexInfos != null ? queryIndexInfos.FirstOrDefault<IndexInfo>() : (IndexInfo) null;
          }
          IndexInfo indexInfo2 = indexInfo1;
          IDictionary<string, HashSet<string>> enumerable = (IDictionary<string, HashSet<string>>) new Dictionary<string, HashSet<string>>();
          if (projectQueryingUnit.ChildRoutingUnits.IsNullOrEmpty<KeyValuePair<string, QueryingUnit>>())
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfo(1081466, "Query Pipeline", "DocumentContractTypeService", FormattableString.Invariant(FormattableStringFactory.Create("Unable to cache project routing(project id: {0}). Probably the project does not contain any repository", (object) projectQueryingUnit.IndexingUnitId)));
          foreach (QueryingUnit queryingUnit in (IEnumerable<QueryingUnit>) projectQueryingUnit.ChildRoutingUnits.Values)
          {
            IEnumerable<IndexInfo> queryIndexInfos = queryingUnit.QueryIndexInfos;
            IndexInfo indexInfo3 = queryIndexInfos != null ? queryIndexInfos.FirstOrDefault<IndexInfo>() : (IndexInfo) null;
            if (indexInfo3 != null)
            {
              if (string.IsNullOrWhiteSpace(indexInfo3.Routing))
              {
                Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081466, "Query Pipeline", "DocumentContractTypeService", FormattableString.Invariant(FormattableStringFactory.Create("Unable to cache project:id {0}, Since routingId is missing for its child repository id:{1}", (object) projectQueryingUnit.IndexingUnitId, (object) queryingUnit.IndexingUnitId)));
              }
              else
              {
                HashSet<string> collection;
                if (!enumerable.TryGetValue(indexInfo3.IndexName, out collection))
                  enumerable.Add(indexInfo3.IndexName, new HashSet<string>((IEnumerable<string>) indexInfo3.Routing.Split(',')));
                else
                  collection.AddRange<string, HashSet<string>>((IEnumerable<string>) indexInfo3.Routing.Split(','));
              }
            }
          }
          if (!enumerable.IsNullOrEmpty<KeyValuePair<string, HashSet<string>>>())
          {
            if (indexInfo2 != null)
            {
              List<IndexInfo> indexInfoList = new List<IndexInfo>();
              foreach (KeyValuePair<string, HashSet<string>> keyValuePair in (IEnumerable<KeyValuePair<string, HashSet<string>>>) enumerable)
              {
                IndexInfo indexInfo4 = (IndexInfo) indexInfo2.Clone();
                indexInfo4.Routing = string.Join(",", keyValuePair.Value.ToArray<string>());
                indexInfo4.IndexName = keyValuePair.Key;
                indexInfoList.Add(indexInfo4);
              }
              projectQueryingUnit.QueryIndexInfos = (IEnumerable<IndexInfo>) indexInfoList;
            }
          }
        }
        catch (Exception ex)
        {
          string str = ex.ToString();
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1081466, "Query Pipeline", "DocumentContractTypeService", FormattableString.Invariant(FormattableStringFactory.Create("Unable to cache project routing(project id: {0}). Exception: {1}", (object) projectQueryingUnit.IndexingUnitId, (object) str)));
        }
      }
    }

    protected override void ProcessIndexingUnits(
      IVssRequestContext requestContext,
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits)
    {
      List<int> intList = new List<int>();
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnits[0];
      this.ProcessCollectionIndexingUnit(requestContext, indexingUnit);
      IDictionary<int, QueryingUnit> routingUnitMap = (IDictionary<int, QueryingUnit>) new Dictionary<int, QueryingUnit>();
      for (int index = 1; index < indexingUnits.Count; ++index)
        this.ProcessIndexingUnit(indexingUnits[index], routingUnitMap, intList);
      this.ProcessScopedPathForLargeRepositories(requestContext, intList, routingUnitMap);
      if (!this.m_queryingTree.ChildRoutingUnits.IsNullOrEmpty<KeyValuePair<string, QueryingUnit>>())
        this.ProcessProjectRouting(this.m_queryingTree.ChildRoutingUnits.Values);
      this.QueryScopingCacheStatus = QueryScopingCacheStatus.Cached;
    }

    internal void AddRepository(
      IVssRequestContext requestContext,
      int indexingUnitId,
      string parentName,
      int parentId)
    {
      if (string.IsNullOrWhiteSpace(parentName))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof (parentName));
      if (indexingUnitId <= 0)
        throw new ArgumentOutOfRangeException(nameof (indexingUnitId));
      if (parentId <= 0)
        throw new ArgumentOutOfRangeException(nameof (parentId));
      List<int> intList = new List<int>();
      IIndexingUnitDataAccess indexingUnitDataAccess = this.m_dataAccessFactory.GetIndexingUnitDataAccess();
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit = indexingUnitDataAccess.GetIndexingUnit(requestContext, indexingUnitId);
      if (indexingUnit.IsLargeRepository(requestContext))
      {
        this.m_queryingTree.CollectionWithLargeRepo = true;
        this.m_queryingTree.LargeRepoIds.Add(indexingUnit.TFSEntityId.ToString());
      }
      IDictionary<int, QueryingUnit> dictionary = (IDictionary<int, QueryingUnit>) new Dictionary<int, QueryingUnit>();
      QueryingUnit chilQueryingUnit1 = this.m_queryingTree.GetChilQueryingUnit(parentName);
      if (this.IsRepoAlreadyCachedAndUpdated(chilQueryingUnit1, indexingUnit))
        return;
      if (chilQueryingUnit1 == null)
        this.ProcessIndexingUnit(indexingUnitDataAccess.GetIndexingUnit(requestContext, parentId), dictionary, intList);
      QueryingUnit chilQueryingUnit2 = this.m_queryingTree.GetChilQueryingUnit(parentName);
      if (chilQueryingUnit2 != null)
      {
        dictionary.TryAdd<int, QueryingUnit>(chilQueryingUnit2.IndexingUnitId, chilQueryingUnit2);
        this.ProcessIndexingUnit(indexingUnit, dictionary, intList);
        this.ProcessProjectRouting((ICollection<QueryingUnit>) new List<QueryingUnit>()
        {
          chilQueryingUnit2
        });
      }
      if (intList.Count <= 0)
        return;
      this.m_queryingTree.CollectionWithLargeRepo = true;
      this.ProcessScopedPathForLargeRepositories(requestContext, intList, dictionary);
    }
  }
}
