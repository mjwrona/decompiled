// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.WorkItemTrackingOutOfBoxCacheVersioning`1
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata
{
  internal abstract class WorkItemTrackingOutOfBoxCacheVersioning<T> : VssMemoryCacheService<Guid, T>
  {
    private int m_cacheVersion;

    protected override void ServiceStart(IVssRequestContext requestContext) => this.m_cacheVersion = requestContext.To(TeamFoundationHostType.Deployment).GetService<ProcessMetadataFileIdCache>().GetFileIdsCacheVersion();

    protected bool ClearCacheOnStaleCacheVersion(IVssRequestContext requestContext)
    {
      int fileIdsCacheVersion = requestContext.To(TeamFoundationHostType.Deployment).GetService<ProcessMetadataFileIdCache>().GetFileIdsCacheVersion();
      if (this.m_cacheVersion >= fileIdsCacheVersion)
        return false;
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Old Cache version", (double) this.m_cacheVersion);
      properties.Add("Current Cache version", (double) fileIdsCacheVersion);
      this.Clear(requestContext);
      this.m_cacheVersion = fileIdsCacheVersion;
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, this.GetType().Name, "Invalidating Cache", properties);
      return true;
    }
  }
}
