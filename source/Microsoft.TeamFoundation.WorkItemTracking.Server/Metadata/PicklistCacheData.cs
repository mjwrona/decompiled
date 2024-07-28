// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.PicklistCacheData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class PicklistCacheData
  {
    private VssMemoryCacheList<Guid, WorkItemPickList> m_picklistCache;
    private IReadOnlyCollection<WorkItemPickListMetadata> m_picklistMetadataCache;

    public PicklistCacheData(VssCacheBase cacheService) => this.m_picklistCache = new VssMemoryCacheList<Guid, WorkItemPickList>((IVssCachePerformanceProvider) cacheService);

    public bool Update(WorkItemPickList picklist) => picklist != null && this.m_picklistCache != null && this.m_picklistCache.Add(picklist.Id, picklist, true);

    public bool Update(IEnumerable<WorkItemPickList> picklists)
    {
      bool flag = picklists != null && this.m_picklistCache != null;
      if (flag)
      {
        foreach (WorkItemPickList picklist in picklists)
          flag &= this.Update(picklist);
      }
      return flag;
    }

    public bool Update(
      IReadOnlyCollection<WorkItemPickListMetadata> metadata)
    {
      if (metadata == null)
        return false;
      this.m_picklistMetadataCache = metadata;
      return true;
    }

    public bool Remove(WorkItemPickList picklist) => picklist != null && this.m_picklistCache != null && this.m_picklistCache.Remove(picklist.Id);

    public bool TryGetPicklist(Guid picklistId, out WorkItemPickList picklist)
    {
      picklist = (WorkItemPickList) null;
      return this.m_picklistCache != null && this.m_picklistCache.TryGetValue(picklistId, out picklist);
    }

    public bool TryGetPicklistMetadata(
      out IReadOnlyCollection<WorkItemPickListMetadata> metadata)
    {
      metadata = this.m_picklistMetadataCache;
      return metadata != null;
    }
  }
}
