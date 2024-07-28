// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.IndexingUnitProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  internal class IndexingUnitProvider
  {
    private IIndexingUnitDataAccess m_indexingUnitDataAccess;

    public IndexingUnitProvider(IIndexingUnitDataAccess indexingUnitDataAccess) => this.m_indexingUnitDataAccess = indexingUnitDataAccess;

    public bool TryGetParentIndexingUnitId(
      IVssRequestContext rc,
      int indexingUnitId,
      out int parentIndexingUnitId)
    {
      IndexingUnitCacheService service = rc.GetService<IndexingUnitCacheService>();
      IndexingUnitCacheObject indexingUnitCacheObject;
      if (service.TryGetValue(rc, indexingUnitId, out indexingUnitCacheObject))
      {
        parentIndexingUnitId = indexingUnitCacheObject.ParentUnitId;
        return true;
      }
      IndexingUnit indexingUnit = this.m_indexingUnitDataAccess.GetIndexingUnit(rc, indexingUnitId);
      if (indexingUnit == null)
      {
        parentIndexingUnitId = -1;
        return false;
      }
      service.Set(rc, indexingUnitId, IndexingUnitProvider.CreateIndexingUnitCacheObject(indexingUnit));
      parentIndexingUnitId = indexingUnit.ParentUnitId;
      return true;
    }

    internal static IndexingUnitCacheObject CreateIndexingUnitCacheObject(IndexingUnit indexingUnit) => new IndexingUnitCacheObject()
    {
      ParentUnitId = indexingUnit.ParentUnitId
    };
  }
}
