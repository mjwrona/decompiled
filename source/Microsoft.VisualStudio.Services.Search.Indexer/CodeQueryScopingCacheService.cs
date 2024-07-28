// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.CodeQueryScopingCacheService
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Common.Utils;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public class CodeQueryScopingCacheService : 
    QueryScopingCacheService,
    IQueryScopingCacheService,
    IVssFrameworkService
  {
    internal CodeQueryScopingCache m_codeQueryScopingCache;

    public CodeQueryScopingCacheService()
      : this(DataAccessFactory.GetInstance())
    {
    }

    internal CodeQueryScopingCacheService(IDataAccessFactory dataAccessFactory)
    {
      this.m_codeQueryScopingCache = new CodeQueryScopingCache(dataAccessFactory);
      this.m_queryScopingCache = (IQueryScopingCache) this.m_codeQueryScopingCache;
    }

    private IEnumerable<IndexInfo> GetScopedIndexInfos(
      EntitySearchQuery searchQuery,
      string entityFilterId,
      QueryingUnit parentQueryingUnit)
    {
      IEnumerable<string> allFilters = searchQuery.GetAllFilters(entityFilterId);
      if (parentQueryingUnit == null)
        return (IEnumerable<IndexInfo>) null;
      IDictionary<string, QueryingUnit> childRoutingUnits = parentQueryingUnit.ChildRoutingUnits;
      if (allFilters.IsNullOrEmpty<string>() || entityFilterId == "PathFilters" && (allFilters.First<string>().Equals("\\") || allFilters.First<string>().Equals("/")))
      {
        this.TraceScopingApplied(entityFilterId, 0);
        return parentQueryingUnit.QueryIndexInfos;
      }
      if (allFilters.Count<string>() > 1)
      {
        IList<IndexInfo> indexInfoList = (IList<IndexInfo>) new List<IndexInfo>();
        foreach (string key in allFilters)
        {
          QueryingUnit queryingUnit;
          if (!childRoutingUnits.TryGetValue(key, out queryingUnit))
            return (IEnumerable<IndexInfo>) null;
          indexInfoList.AddRangeIfRangeNotNull<IndexInfo, IList<IndexInfo>>(queryingUnit.QueryIndexInfos);
        }
        this.TraceScopingApplied(entityFilterId, allFilters.Count<string>());
        return this.MergeQueryIndexInfos((IEnumerable<IndexInfo>) indexInfoList);
      }
      string key1 = entityFilterId == "PathFilters" ? this.GetScopedPath(allFilters.First<string>()) : allFilters.First<string>();
      QueryingUnit queryingUnit1;
      if (!childRoutingUnits.TryGetValue(key1, out queryingUnit1))
        return (IEnumerable<IndexInfo>) null;
      string childFilterId = this.GetChildFilterId(entityFilterId, queryingUnit1);
      if (!string.IsNullOrEmpty(childFilterId))
        return this.GetScopedIndexInfos(searchQuery, childFilterId, queryingUnit1);
      this.TraceScopingApplied(entityFilterId, 1);
      return queryingUnit1.QueryIndexInfos;
    }

    private void TraceScopingApplied(string entityFilterId, int filterCount)
    {
      switch (filterCount > 0 ? entityFilterId : this.GetParentFilterId(entityFilterId))
      {
        case "CollectionFilters":
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("CollectionScoped", "Query Pipeline", 1.0);
          break;
        case "ProjectFilters":
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi(filterCount <= 1 ? "ProjectScoped" : "MultipleProjectScoped", "Query Pipeline", 1.0);
          break;
        case "RepositoryFilters":
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi(filterCount <= 1 ? "RepositoryScoped" : "MultipleRepositoryScoped", "Query Pipeline", 1.0);
          break;
        case "PathFilters":
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("PathScoped", "Query Pipeline", 1.0);
          break;
      }
    }

    private IEnumerable<IndexInfo> MergeQueryIndexInfos(IEnumerable<IndexInfo> indexInfoList)
    {
      IDictionary<string, HashSet<string>> enumerable = (IDictionary<string, HashSet<string>>) new Dictionary<string, HashSet<string>>();
      IList<IndexInfo> indexInfoList1 = (IList<IndexInfo>) new List<IndexInfo>();
      IndexInfo indexInfo1 = indexInfoList.FirstOrDefault<IndexInfo>();
      foreach (IndexInfo indexInfo2 in indexInfoList)
      {
        HashSet<string> collection;
        if (!enumerable.TryGetValue(indexInfo2.IndexName, out collection))
          enumerable.Add(indexInfo2.IndexName, new HashSet<string>((IEnumerable<string>) indexInfo2.Routing.Split(',')));
        else
          collection.AddRange<string, HashSet<string>>((IEnumerable<string>) indexInfo2.Routing.Split(','));
      }
      if (enumerable.IsNullOrEmpty<KeyValuePair<string, HashSet<string>>>() || indexInfo1 == null)
        return (IEnumerable<IndexInfo>) null;
      foreach (KeyValuePair<string, HashSet<string>> keyValuePair in (IEnumerable<KeyValuePair<string, HashSet<string>>>) enumerable)
      {
        IndexInfo indexInfo3 = (IndexInfo) indexInfo1.Clone();
        indexInfo3.Routing = string.Join(",", keyValuePair.Value.ToArray<string>());
        indexInfo3.IndexName = keyValuePair.Key;
        indexInfoList1.Add(indexInfo3);
      }
      return (IEnumerable<IndexInfo>) indexInfoList1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override IEnumerable<IndexInfo> GetScopedIndexInfos(
      IVssRequestContext requestContext,
      EntitySearchQuery searchQuery)
    {
      if (requestContext.IsFeatureEnabled("Search.Server.ProjectScopedQuery"))
      {
        try
        {
          return this.GetScopedIndexInfos(searchQuery, "ProjectFilters", this.m_queryScopingCache.GetCacheRoot());
        }
        catch (Exception ex)
        {
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080004, "REST-API", "Query Pipeline", FormattableString.Invariant(FormattableStringFactory.Create("Cache Usage failed with exception: [{0}].", (object) ex)));
          return (IEnumerable<IndexInfo>) null;
        }
      }
      else
      {
        IEnumerable<SearchFilter> filters = searchQuery.Filters;
        if ((filters != null ? (filters.Count<SearchFilter>() == 1 ? 1 : 0) : 0) != 0)
          return (IEnumerable<IndexInfo>) null;
        string singleValuedElseNull1 = searchQuery.GetFilterIfPresentAndSingleValuedElseNull("ProjectFilters");
        string singleValuedElseNull2 = searchQuery.GetFilterIfPresentAndSingleValuedElseNull("RepositoryFilters");
        if (!string.IsNullOrWhiteSpace(singleValuedElseNull2))
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("IsScopedQuery", "Query Pipeline", 1.0);
        QueryingUnit chilQueryingUnit1 = this.m_queryScopingCache.GetCacheRoot()?.GetChilQueryingUnit(singleValuedElseNull1)?.GetChilQueryingUnit(singleValuedElseNull2);
        if (chilQueryingUnit1 == null)
          return (IEnumerable<IndexInfo>) null;
        string singleValuedElseNull3 = searchQuery.GetFilterIfPresentAndSingleValuedElseNull("PathFilters");
        if (chilQueryingUnit1.IsLargeRepo && chilQueryingUnit1.ChildRoutingUnits != null && !string.IsNullOrWhiteSpace(singleValuedElseNull3))
        {
          string scopedPath = this.GetScopedPath(singleValuedElseNull3);
          QueryingUnit chilQueryingUnit2 = chilQueryingUnit1.GetChilQueryingUnit(scopedPath);
          if (chilQueryingUnit2 != null)
          {
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("ScopedPathUsed", "Query Pipeline", 1.0);
            return chilQueryingUnit2.QueryIndexInfos;
          }
        }
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("RepoRoutingReturned", "Query Pipeline", 1.0);
        return chilQueryingUnit1.QueryIndexInfos;
      }
    }

    private string GetChildFilterId(string filter, QueryingUnit queryUnit)
    {
      switch (filter)
      {
        case "ProjectFilters":
          return "RepositoryFilters";
        case "RepositoryFilters":
          return queryUnit.IsLargeRepo ? "PathFilters" : string.Empty;
        default:
          return string.Empty;
      }
    }

    private string GetParentFilterId(string filter)
    {
      switch (filter)
      {
        case "ProjectFilters":
          return "CollectionFilters";
        case "RepositoryFilters":
          return "ProjectFilters";
        case "PathFilters":
          return "RepositoryFilters";
        default:
          return string.Empty;
      }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string GetScopedPath(string folderPath)
    {
      folderPath = folderPath.NormalizePath();
      string scopedPath = folderPath;
      int length = folderPath.IndexOf("\\", StringComparison.Ordinal);
      if (length != -1)
        scopedPath = folderPath.Substring(0, length);
      return scopedPath;
    }

    public void UpdateCodeQueryScopingCache(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(requestContext));
      try
      {
        if (requestContext.IsFeatureEnabled("Search.Server.ScopedQuery"))
        {
          if (this.m_queryScopingCache.QueryScopingCacheStatus != QueryScopingCacheStatus.Cached)
            return;
          IEnumerable<Type> entitiesKnownTypes = requestContext.To(TeamFoundationHostType.Deployment).GetService<IEntityService>().GetEntitiesKnownTypes();
          QueryScopingCacheUpdateData scopingCacheUpdateData = (QueryScopingCacheUpdateData) Serializers.FromXmlString(args.Data, typeof (QueryScopingCacheUpdateData), entitiesKnownTypes);
          this.GetUpdaterMapper(scopingCacheUpdateData.EntityType).GetQueryScopingCacheUpdater(scopingCacheUpdateData, (IQueryScopingCache) this.m_codeQueryScopingCache).UpdateQueryScopingCache(requestContext, scopingCacheUpdateData);
        }
        else
          this.m_queryScopingCache.QueryScopingCacheStatus = QueryScopingCacheStatus.CacheDisabled;
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceError(1080004, "REST-API", "Query Pipeline", FormattableString.Invariant(FormattableStringFactory.Create("Cache updation failed with exception [{0}].", (object) ex)));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    private void RemoveQueryingUnitCache(
      IVssRequestContext requestContext,
      NotificationEventArgs args)
    {
      this.m_queryScopingCache.QueryScopingCacheStatus = QueryScopingCacheStatus.Undefined;
      this.m_queryScopingCache.ClearCache();
    }

    [Info("InternalForTestPurpose")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal override bool IsScopedQuery(EntitySearchQuery searchQuery)
    {
      if (!base.IsScopedQuery(searchQuery))
        return false;
      IEnumerable<SearchFilter> filters = searchQuery.Filters;
      return filters != null && filters.Count<SearchFilter>() >= 1;
    }

    protected override void RegisterForSQLNotification(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, "Default", Microsoft.VisualStudio.Services.Search.Common.SqlNotificationEventClasses.QueryScopingCacheInvalidated, new SqlNotificationHandler(this.UpdateCodeQueryScopingCache), false);

    protected override void DeregisterSQLNotification(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, "Default", Microsoft.VisualStudio.Services.Search.Common.SqlNotificationEventClasses.QueryScopingCacheInvalidated, new SqlNotificationHandler(this.RemoveQueryingUnitCache), false);
  }
}
