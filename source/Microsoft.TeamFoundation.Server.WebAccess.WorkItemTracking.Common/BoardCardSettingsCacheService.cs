// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BoardCardSettingsCacheService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class BoardCardSettingsCacheService : VssMemoryCacheService<Guid, BoardCardSettings>
  {
    private static readonly TimeSpan s_cacheCleanupInterval = TimeSpan.FromHours(12.0);
    private static readonly TimeSpan s_maxCacheInactivityAge = TimeSpan.FromHours(24.0);

    public BoardCardSettingsCacheService()
      : base(BoardCardSettingsCacheService.s_cacheCleanupInterval)
    {
      this.InactivityInterval.Value = BoardCardSettingsCacheService.s_maxCacheInactivityAge;
    }
  }
}
