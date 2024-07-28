// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.CodeQueryScopingCacheUpdaterMapper
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Common;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Search.Indexer
{
  public class CodeQueryScopingCacheUpdaterMapper : IQueryScopingCacheUpdaterMapper
  {
    public IQueryScopingCacheUpdater GetQueryScopingCacheUpdater(
      QueryScopingCacheUpdateData queryScopingCacheUpdate,
      IQueryScopingCache queryScopingCache)
    {
      if (queryScopingCacheUpdate == null)
        throw new ArgumentNullException(nameof (queryScopingCacheUpdate));
      switch (queryScopingCacheUpdate.IndexingUnitType)
      {
        case "Collection":
          if (queryScopingCacheUpdate.EventType == QueryScopingCacheUpdateEvent.CollectionRename)
            return (IQueryScopingCacheUpdater) new CollectionRenameUpdateCodeQueryScopingCache(queryScopingCache);
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} type update is not supported at {1} level of entity type {2}", (object) queryScopingCacheUpdate.EventType, (object) queryScopingCacheUpdate.IndexingUnitType, (object) queryScopingCacheUpdate.EntityType));
        case "Project":
          switch (queryScopingCacheUpdate.EventType)
          {
            case QueryScopingCacheUpdateEvent.ProjectRename:
              return (IQueryScopingCacheUpdater) new ProjectRenameUpdateCodeQueryScopingCache(queryScopingCache);
            case QueryScopingCacheUpdateEvent.ProjectDeletion:
              return (IQueryScopingCacheUpdater) new ProjectDeleteUpdateCodeQueryScopingCache(queryScopingCache);
            default:
              throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} type update is not supported at {1} level of entity type {2}", (object) queryScopingCacheUpdate.EventType, (object) queryScopingCacheUpdate.IndexingUnitType, (object) queryScopingCacheUpdate.EntityType.Name));
          }
        case "Git_Repository":
        case "TFVC_Repository":
        case "CustomRepository":
          switch (queryScopingCacheUpdate.EventType)
          {
            case QueryScopingCacheUpdateEvent.RepositoryRename:
              return (IQueryScopingCacheUpdater) new RepoRenameUpdateCodeQueryScopingCache(queryScopingCache);
            case QueryScopingCacheUpdateEvent.RepositoryAdd:
              return (IQueryScopingCacheUpdater) new RepoAdditionUpdateCodeQueryScopingCache(queryScopingCache);
            case QueryScopingCacheUpdateEvent.RepositoryDeletion:
              return (IQueryScopingCacheUpdater) new RepoDeletionUpdateCodeQueryScopingCache(queryScopingCache);
            default:
              throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} type update is not supported at {1} level of entity type {2}", (object) queryScopingCacheUpdate.EventType, (object) queryScopingCacheUpdate.IndexingUnitType, (object) queryScopingCacheUpdate.EntityType));
          }
        case "ScopedIndexingUnit":
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} type update is not supported at {1} level of entity type {2}", (object) queryScopingCacheUpdate.EventType, (object) queryScopingCacheUpdate.IndexingUnitType, (object) queryScopingCacheUpdate.EntityType));
        default:
          return (IQueryScopingCacheUpdater) null;
      }
    }
  }
}
