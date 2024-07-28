// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.CodeQueryScopingCacheUtil
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Utils;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public static class CodeQueryScopingCacheUtil
  {
    internal static void SqlNotifyForRepoAddition(
      IDataAccessFactory dataAccessFactory,
      IVssRequestContext tfsRequestContext,
      IndexingUnit indexingUnit)
    {
      if (!tfsRequestContext.IsFeatureEnabled("Search.Server.ScopedQuery") || !(indexingUnit.EntityType.Name == "Code") || !indexingUnit.IsRepository())
        return;
      QueryScopingCacheUpdateData notificationData = CodeQueryScopingCacheUtil.SqlNotificationChangeDataAtRepoLevel(tfsRequestContext, dataAccessFactory, indexingUnit, QueryScopingCacheUpdateEvent.RepositoryAdd);
      IEnumerable<Type> entitiesKnownTypes = tfsRequestContext.To(TeamFoundationHostType.Deployment).GetService<IEntityService>().GetEntitiesKnownTypes();
      SQLNotificationSenders.SendSqlNotification(tfsRequestContext, (object) notificationData, Microsoft.VisualStudio.Services.Search.Common.SqlNotificationEventClasses.QueryScopingCacheInvalidated, entitiesKnownTypes);
    }

    internal static void SqlNotifyForRepoDeletion(
      IndexingExecutionContext indexingExecutionContext,
      IDataAccessFactory dataAccessFactory)
    {
      IndexingUnit indexingUnit = indexingExecutionContext.IndexingUnit;
      if (!indexingExecutionContext.RequestContext.IsFeatureEnabled("Search.Server.ScopedQuery") || !(indexingUnit.EntityType.Name == "Code") || !indexingUnit.IsRepository())
        return;
      QueryScopingCacheUpdateData notificationData = CodeQueryScopingCacheUtil.SqlNotificationChangeDataAtRepoLevel(indexingExecutionContext.RequestContext, dataAccessFactory, indexingUnit, QueryScopingCacheUpdateEvent.RepositoryDeletion);
      IEnumerable<Type> entitiesKnownTypes = indexingExecutionContext.RequestContext.To(TeamFoundationHostType.Deployment).GetService<IEntityService>().GetEntitiesKnownTypes();
      SQLNotificationSenders.SendSqlNotification(indexingExecutionContext.RequestContext, (object) notificationData, Microsoft.VisualStudio.Services.Search.Common.SqlNotificationEventClasses.QueryScopingCacheInvalidated, entitiesKnownTypes);
    }

    private static QueryScopingCacheUpdateData SqlNotificationChangeDataAtRepoLevel(
      IVssRequestContext tfsRequestContext,
      IDataAccessFactory dataAccessFactory,
      IndexingUnit indexingUnit,
      QueryScopingCacheUpdateEvent updateEvent)
    {
      QueryScopingCacheUpdateData scopingCacheUpdateData = new QueryScopingCacheUpdateData(indexingUnit, updateEvent);
      IndexingUnit indexingUnit1 = dataAccessFactory.GetIndexingUnitDataAccess().GetIndexingUnit(tfsRequestContext, indexingUnit.ParentUnitId);
      if (!string.IsNullOrWhiteSpace(indexingUnit1.GetTFSEntityName()))
      {
        scopingCacheUpdateData.ParentHierarchy = (IDictionary<string, string>) new Dictionary<string, string>();
        scopingCacheUpdateData.ParentHierarchy.Add("ProjectFilters", indexingUnit1.GetTFSEntityName());
      }
      return scopingCacheUpdateData;
    }
  }
}
