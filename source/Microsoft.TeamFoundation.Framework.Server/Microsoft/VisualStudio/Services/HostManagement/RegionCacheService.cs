// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostManagement.RegionCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.HostManagement
{
  internal class RegionCacheService : VssMemoryCacheService<string, Region>
  {
    private static readonly TimeSpan s_expiryInterval = TimeSpan.FromHours(1.0);
    private static readonly TimeSpan s_cacheCleanupInterval = TimeSpan.FromHours(1.0);
    private static readonly TimeSpan s_maxCacheInactivityAge = TimeSpan.FromHours(1.0);

    public RegionCacheService()
      : base(RegionCacheService.s_cacheCleanupInterval)
    {
      this.ExpiryInterval.Value = RegionCacheService.s_expiryInterval;
      this.InactivityInterval.Value = RegionCacheService.s_maxCacheInactivityAge;
    }
  }
}
