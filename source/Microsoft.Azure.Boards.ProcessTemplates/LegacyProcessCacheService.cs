// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.ProcessTemplates.LegacyProcessCacheService
// Assembly: Microsoft.Azure.Boards.ProcessTemplates, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A94E8BA8-9851-4F5D-B619-9CF2FFF5B128
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.ProcessTemplates.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Boards.ProcessTemplates
{
  public class LegacyProcessCacheService : VssMemoryCacheService<Guid, IProcessTemplate>
  {
    private static readonly TimeSpan s_cacheCleanupInterval = TimeSpan.FromMinutes(2.0);
    private static readonly TimeSpan s_maxCacheInactivityAge = TimeSpan.FromMinutes(15.0);
    private const int c_maxCacheSize = 20;

    public LegacyProcessCacheService()
      : base((IEqualityComparer<Guid>) EqualityComparer<Guid>.Default, new MemoryCacheConfiguration<Guid, IProcessTemplate>().WithMaxElements(20).WithCleanupInterval(LegacyProcessCacheService.s_cacheCleanupInterval))
    {
      this.InactivityInterval.Value = LegacyProcessCacheService.s_maxCacheInactivityAge;
    }
  }
}
